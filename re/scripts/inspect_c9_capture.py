#!/usr/bin/env python3
"""Validate a bounded loopback C9 capture, exporting metadata without credentials.

Requires installed tshark and a complete, in-order TCP capture starting at SYN.
Gaps, retransmissions and incomplete protocol frames are rejected explicitly.
The decoder never exports packet payloads or authentication/session tokens.
"""
import argparse
from collections import Counter
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import shutil
import struct
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT))
from c9server.protocol import ConnectionCodec, FrameSplitter


def inspect(capture, tshark, ports):
    if not 0 < capture.stat().st_size <= 16 * 1024 * 1024:
        raise ValueError('Capture must be nonempty and no larger than 16 MiB')
    port_filter = ' or '.join(f'tcp.port == {port}' for port in sorted(ports))
    display_filter = f'ip.src == 127.0.0.1 and ip.dst == 127.0.0.1 and ({port_filter})'
    fields = ['frame.number', 'tcp.stream', 'tcp.srcport', 'tcp.seq_raw',
              'tcp.flags.syn', 'tcp.payload']
    command = [str(tshark), '-r', str(capture), '-Y', display_filter, '-T', 'fields']
    for field in fields:
        command.extend(['-e', field])
    result = subprocess.run(command, capture_output=True, text=True, timeout=45)
    if result.returncode:
        raise RuntimeError('tshark could not read the capture: ' + result.stderr[:2000])
    next_sequence, codecs, splitters = {}, {}, {}
    records = []
    rows = result.stdout.splitlines()
    for line in rows:
        number, stream, source, sequence, syn, payload = line.split('\t')
        direction = 'server_to_game' if int(source) in ports else 'game_to_server'
        key = (int(stream), direction)
        sequence = int(sequence)
        if syn in ('1', 'True', 'true'):
            if key in next_sequence:
                raise ValueError('Repeated SYN requires explicit TCP reassembly')
            next_sequence[key] = (sequence + 1) & 0xffffffff
        if not payload:
            continue
        if key not in next_sequence or sequence != next_sequence[key]:
            raise ValueError('Missing SYN, TCP gap or retransmission; explicit reassembly is required')
        wire = bytes.fromhex(payload.replace(':', ''))
        next_sequence[key] = (sequence + len(wire)) & 0xffffffff
        if key not in codecs:
            codecs[key] = ConnectionCodec(discover_receive_selector=direction == 'game_to_server')
            splitters[key] = FrameSplitter(8192)
        for frame in splitters[key].feed(wire):
            packet = codecs[key].decode(frame)
            record = dict(capture_frame=int(number), stream=int(stream), direction=direction,
                          type=f'0x{packet.kind:04x}', flag=packet.flag,
                          sequence=packet.sequence, payload_length=len(packet.payload),
                          wire_length=len(frame))
            if packet.kind == 0x2EEC and len(packet.payload) == 20:
                record['selected_server_key'] = struct.unpack_from('<I', packet.payload, 8)[0]
                record['reserved_tail_zero'] = not any(packet.payload[12:])
            if packet.kind == 0x2EE6 and len(packet.payload) >= 53:
                record['server_count'] = packet.payload[0]
                record['local_fixture_name_matches'] = packet.payload[6:24] == 'C9 Local\0'.encode('utf-16le')
                record['first_entry_gate_byte'] = packet.payload[52]
            if packet.kind == 0x2EED and len(packet.payload) >= 27:
                record['selected_server_key'] = struct.unpack_from('<I', packet.payload, 8)[0]
                record['tcp_endpoint_loopback'] = packet.payload[13:17] == bytes([127, 0, 0, 1])
                record['tcp_endpoint_port'] = struct.unpack_from('<H', packet.payload, 17)[0]
            if packet.kind == 0x0BBB and len(packet.payload) == 90:
                record['create_slot'] = packet.payload[0]
                record['create_template_id'] = struct.unpack_from('<H', packet.payload, 1)[0]
                record['create_mode'] = packet.payload[89]
                try:
                    name = packet.payload[3:45].decode('utf-16le').rstrip('\0')
                    record['create_name_characters'] = len(name)
                    record['create_name_valid'] = 0 < len(name) <= 20 and all(
                        c.isalnum() or c == '_' for c in name)
                except UnicodeDecodeError:
                    record['create_name_valid'] = False
            if packet.kind == 0x0EFA and len(packet.payload) >= 42:
                record['character_slots'] = packet.payload[40]
                count = packet.payload[41]
                record['character_count'] = count
                if len(packet.payload) == 42 + count * 136:
                    record['characters_active'] = all(struct.unpack_from('<Q', packet.payload, 42 + i * 136 + 60)[0]
                        == 0xffffffffffffffff for i in range(count))
            if packet.kind in (0x0EFB, 0x0BBF) and len(packet.payload) == 8:
                record['synthetic_character_id_matches'] = struct.unpack_from('<Q', packet.payload)[0] == 1
            if packet.kind == 0x0EFC and len(packet.payload) == 53:
                record['synthetic_character_id_matches'] = struct.unpack_from('<Q', packet.payload)[0] == 1
                record['equipment_count'] = packet.payload[52]
            if packet.kind == 0x0BC0 and len(packet.payload) == 7444:
                record['synthetic_character_id_matches'] = struct.unpack_from('<Q', packet.payload)[0] == 1
                record['base_job_code'] = struct.unpack_from('<I', packet.payload, 12)[0]
                record['level'] = struct.unpack_from('<H', packet.payload, 16)[0]
                record['progression_code_unverified'] = struct.unpack_from('<H', packet.payload, 0x6e)[0]
                record['progression_index_unverified'] = struct.unpack_from('<H', packet.payload, 0x70)[0]
            if packet.kind == 0x0BC3 and len(packet.payload) == 46:
                record['map_id'] = struct.unpack_from('<I', packet.payload)[0]
                record['submap_index'] = packet.payload[4]
                record['diagnostic_position'] = list(struct.unpack_from('<fff', packet.payload, 6))
            if packet.kind == 0xED1D and len(packet.payload) == 4:
                record['request_key'] = struct.unpack_from('<I', packet.payload)[0]
            records.append(record)
    for splitter in splitters.values():
        splitter.finish()
    if not records:
        raise ValueError('No complete C9 frames found; this is not interoperability evidence')
    return dict(recorded_at_utc=datetime.now(timezone.utc).isoformat(),
                capture_name=capture.name, capture_sha256=hashlib.sha256(capture.read_bytes()).hexdigest(),
                capture_bytes=capture.stat().st_size, matched_tcp_packets=len(rows),
                decoded_frames=len(records), tcp_direction_streams=len(codecs),
                type_counts=dict(sorted(Counter(record['type'] for record in records).items())),
                records=records, credentials_or_payload_exported=False)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('capture', type=Path)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--port', type=int, action='append', help='Server port; repeat for multiple endpoints')
    parser.add_argument('--tshark', type=Path)
    args = parser.parse_args()
    ports = set(args.port or [14000, 14001])
    if not ports or any(not 1 <= port <= 65535 for port in ports):
        parser.error('Server ports must be in 1..65535')
    tshark = args.tshark or shutil.which('tshark')
    if not tshark:
        tshark = Path(os.environ.get('ProgramFiles', r'C:\Program Files')) / 'Wireshark/tshark.exe'
    if not Path(tshark).is_file():
        parser.error('Installed tshark was not found; pass --tshark')
    report = inspect(args.capture, tshark, ports)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({key: report[key] for key in
                      ['capture_sha256', 'matched_tcp_packets', 'decoded_frames', 'type_counts']}))


if __name__ == '__main__':
    main()
