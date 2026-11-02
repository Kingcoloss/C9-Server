#!/usr/bin/env python3
"""Inspect supplied C9 bytes offline. No network access or client execution.

Wire mode splits frames using the clear length only. Decoded mode requires
bytes already transformed by an independently verified decoder. This tool
does not implement the PacketTwister registry or outer cipher.
"""
import argparse
import json
import struct
from pathlib import Path


# Share the server's protocol implementation while preserving the offline CLI.
import sys
sys.path.insert(0, str(Path(__file__).resolve().parents[2]))
from c9server.protocol import HEADER_SIZE, FrameSplitter, decoded_checksum, signed_checksum


def inspect_decoded(frame, verify_checksum=False):
    if len(frame) < HEADER_SIZE:
        raise ValueError('Decoded frame is shorter than 13 bytes')
    length, flag, kind, checksum, sequence = struct.unpack_from('<HBHII', frame)
    if length != len(frame):
        raise ValueError(f'Decoded length {length} differs from supplied size {len(frame)}')
    result = {'length': length, 'flag': flag, 'type': f'0x{kind:04x}',
              'checksum_field': f'0x{checksum:08x}', 'sequence': sequence,
              'payload_hex': frame[HEADER_SIZE:].hex()}
    if verify_checksum:
        if flag == 0:
            raise ValueError('Checksum validation for flag=0 is not established')
        computed = decoded_checksum(frame)
        result['computed_checksum'] = f'0x{computed:08x}'
        result['checksum_matches'] = computed == checksum
    if kind == 0x2EE6:
        if len(frame) < 14:
            raise ValueError('0x2ee6 is missing its count byte')
        count = frame[13]
        required = 14 + count * 52
        if len(frame) < required:
            raise ValueError(f'0x2ee6 count={count} requires at least {required} bytes')
        entries = []
        for i in range(count):
            entry = frame[14 + i * 52:14 + (i + 1) * 52]
            entries.append({'raw_hex': entry.hex(),
                            'lookup_key_u32_le': struct.unpack_from('<I', entry, 1)[0],
                            'name_source_tail_hex': entry[5:].hex()})
        result['server_list_static_layout'] = {
            'count': count, 'entry_size': 52, 'entries': entries,
            'trailing_hex': frame[required:].hex(),
            'limits': 'Entry byte 0, full field layout, name capacity and encoding remain unresolved'}
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('input', type=Path, help='Supplied binary frame stream')
    parser.add_argument('--input-kind', choices=['wire', 'decoded'], required=True)
    parser.add_argument('--max-frame', type=int, default=65535,
                        help='Caller limit; use 4096 only for the observed BaseCat path')
    parser.add_argument('--verify-checksum', action='store_true',
                        help='Decoded nonzero-flag validation path only')
    args = parser.parse_args()
    if args.verify_checksum and args.input_kind != 'decoded':
        parser.error('--verify-checksum requires --input-kind decoded')
    splitter = FrameSplitter(args.max_frame)
    output = []
    with args.input.open('rb') as stream:
        while chunk := stream.read(8192):
            for frame in splitter.feed(chunk):
                if args.input_kind == 'decoded':
                    output.append(inspect_decoded(frame, args.verify_checksum))
                else:
                    output.append({'wire_length': len(frame), 'flag': frame[2],
                                   'wire_hex': frame.hex()})
    splitter.finish()
    print(json.dumps(output, indent=2))


if __name__ == '__main__':
    main()
