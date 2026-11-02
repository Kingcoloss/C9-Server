#!/usr/bin/env python3
"""Build repeatable Python-reference fixtures for the .NET port.

These are cross-language regression vectors, not captured original-game traffic.
"""
import hashlib
import json
import random
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(ROOT))
from c9server.protocol import ConnectionCodec, Packet, signed_checksum
from c9server.twister import transform


def main():
    rng = random.Random(0xC91017)
    twisters = []
    sizes = [0, 1, 3, 4, 7, 8, 15, 16, 31, 32, 52, 2162]
    for selector in range(80):
        for size in sizes:
            payload = rng.randbytes(size)
            encoded = transform(payload, selector)
            assert transform(encoded, selector, decode=True) == payload
            twisters.append({'selector': selector, 'input_hex': payload.hex(), 'encoded_hex': encoded.hex()})
    sender = ConnectionCodec()
    receiver = ConnectionCodec(discover_receive_selector=True)
    packets = []
    for i in range(64):
        size = sizes[i % len(sizes)]
        payload = rng.randbytes(size)
        kind = [0x1771, 0x2EE1, 0x2EEC, 0x2EE5, 0x2EE6][i % 5]
        flag = 0 if i % 11 == 0 else [1, 2, 255][i % 3]
        adjustment = [0, 17, 0xFFFFFFF1][i % 3]
        wire = sender.encode(Packet(kind, payload, flag), selector_adjustment=adjustment)
        decoded = receiver.decode(wire).decoded_bytes()
        packets.append({'type': kind, 'payload_hex': payload.hex(), 'flag': flag,
                        'selector_adjustment': adjustment, 'wire_hex': wire.hex(), 'decoded_hex': decoded.hex()})
    ripemd = []
    checksums = []
    for size in [0, 1, 3, 55, 56, 63, 64, 65, 127, 128, 129, 1024]:
        data = rng.randbytes(size)
        ripemd.append({'input_hex': data.hex(), 'digest_hex': hashlib.new('ripemd160', data).hexdigest()})
        checksums.append({'input_hex': data.hex(), 'checksum': signed_checksum(data)})
    data = {'classification': 'Python reference vectors; not original-Client wire fixtures',
            'client_sha256': 'b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25',
            'twister_catalog_sha256': hashlib.sha256((ROOT / 'c9server/data/twisters.json').read_bytes()).hexdigest(),
            'twisters': twisters, 'packets': packets, 'ripemd_openssl': ripemd, 'checksums': checksums,
            'send_state_table_hex': bytes(sender.send_outer.table).hex(),
            'receive_state_table_hex': bytes(receiver.receive_outer.table).hex()}
    path = ROOT / 'tests/fixtures/python_port_vectors.json'
    path.write_text(json.dumps(data, separators=(',', ':')) + '\n')
    print(json.dumps({'twister_vectors': len(twisters), 'wire_vectors': len(packets),
                      'ripemd_openssl_vectors': len(ripemd), 'checksum_vectors': len(checksums),
                      'fixture_sha256': hashlib.sha256(path.read_bytes()).hexdigest()}))


if __name__ == '__main__':
    main()
