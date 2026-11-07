"""Observe one selected game endpoint live; retain metadata, never packet bodies.

No pcap is written. Payloads stay in bounded transient buffers and are discarded
after parsing. Gaps explicitly invalidate a direction instead of guessing bytes.
"""
import argparse
from datetime import datetime, timezone
import ipaddress
import json
from pathlib import Path
import struct
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT))
from c9server.protocol import ConnectionCodec, FrameSplitter


class Direction:
    def __init__(self):
        self.sequence = None
        self.codec = ConnectionCodec(max_frame=65535, discover_receive_selector=True)
        self.splitter = FrameSplitter(65535)
        self.failed = False

    def feed(self, sequence, syn, data):
        if self.failed:
            return []
        if syn:
            if self.sequence is not None:
                self.failed = True
                raise ValueError('Repeated SYN')
            self.sequence = (sequence + 1) & 0xffffffff
        if not data:
            return []
        if self.sequence is None:
            self.failed = True
            raise ValueError('Capture missed SYN')
        # SYN consumes one sequence number before any data on the same segment.
        data_sequence = (sequence + int(syn)) & 0xffffffff
        delta = (data_sequence - self.sequence + 0x80000000) % 0x100000000 - 0x80000000
        if delta > 0:
            self.failed = True
            raise ValueError('TCP gap or out-of-order segment')
        if delta < 0:
            data = data[-delta:]
        self.sequence = (self.sequence + len(data)) & 0xffffffff
        records = []
        for frame in self.splitter.feed(data):
            record = {'wire_length': len(frame), 'flag': frame[2]}
            try:
                packet = self.codec.decode(frame)
                record.update(type=f'0x{packet.kind:04x}', payload_length=len(packet.payload), sequence=packet.sequence)
                if packet.kind == 4 and len(packet.payload) == 1:
                    record['greeting_mode'] = packet.payload[0]
                # Classify small result fields without retaining IDs or tickets.
                if len(packet.payload) <= 64:
                    words = list(struct.iter_unpack('<I', packet.payload[:len(packet.payload) // 4 * 4]))
                    record['zero_word_offsets'] = [i * 4 for i, (v,) in enumerate(words) if v == 0]
                    record['one_word_offsets'] = [i * 4 for i, (v,) in enumerate(words) if v == 1]
                    record['max_word_offsets'] = [i * 4 for i, (v,) in enumerate(words) if v == 0xffffffff]
            except (ValueError, RuntimeError) as error:
                self.failed = True
                record['decode_error'] = type(error).__name__
            records.append(record)
        return records


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--peer', required=True)
    parser.add_argument('--port', type=int, required=True)
    parser.add_argument('--interface', required=True)
    parser.add_argument('--seconds', type=int, default=120)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--tshark', type=Path, default=Path('C:/Program Files/Wireshark/tshark.exe'))
    args = parser.parse_args()
    peer = str(ipaddress.IPv4Address(args.peer))
    if not 1 <= args.port <= 65535 or not 1 <= args.seconds <= 300:
        parser.error('Invalid endpoint or capture duration')
    command = [str(args.tshark), '-i', args.interface, '-f', f'host {peer} and tcp port {args.port}',
               '-a', f'duration:{args.seconds}', '-l', '-q', '-T', 'fields',
               '-e', 'ip.src', '-e', 'tcp.stream', '-e', 'tcp.seq_raw', '-e', 'tcp.flags.syn', '-e', 'tcp.payload']
    states, records, errors = {}, [], []
    budget_reached = False
    print(json.dumps({'event': 'observer_started', 'pcap_saved': False, 'payload_exported': False}), flush=True)
    with subprocess.Popen(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True) as capture:
        for line in capture.stdout:
            fields = line.rstrip('\r\n').split('\t')
            if len(fields) != 5 or not fields[1]:
                continue
            source, stream, sequence, syn, payload = fields
            direction = 'server_to_game' if source == peer else 'game_to_server'
            key = (stream, direction)
            if key not in states:
                if len(states) >= 16:
                    continue
                states[key] = Direction()
            state = states[key]
            try:
                for metadata in state.feed(int(sequence), syn in ('1', 'True', 'true'), bytes.fromhex(payload.replace(':', ''))):
                    record = {'direction': direction, 'stream': int(stream), **metadata}
                    records.append(record)
                    print(json.dumps(record), flush=True)
                    if len(records) >= 512:
                        budget_reached = True
                        capture.terminate()
                        break
            except (ValueError, RuntimeError) as error:
                if len(errors) < 16:
                    errors.append({'direction': direction, 'stream': int(stream), 'reason': str(error)})
            if budget_reached:
                break
        capture.wait()
        if capture.returncode and not budget_reached:
            raise RuntimeError('Packet observer failed: ' + capture.stderr.read()[:1000])
    report = {'recorded_at_utc': datetime.now(timezone.utc).isoformat(), 'records': records,
              'errors': errors, 'pcap_saved': False, 'payload_exported': False}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2) + '\n')
    print(json.dumps({'event': 'observer_finished', 'frames': len(records), 'errors': len(errors)}), flush=True)


if __name__ == '__main__':
    main()
