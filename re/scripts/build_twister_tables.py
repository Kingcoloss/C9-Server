#!/usr/bin/env python3
"""Lift bounded byte/bit transforms into portable permutation/XOR tables.

The native reference is compiled from recovered C, never from the client PE.
Basis-vector extraction is accepted only for bijective affine bit mappings,
and checked against the reference at all length boundaries and random inputs.
"""
import ctypes
import hashlib
import json
import random
import re
import subprocess
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]


def apply_mapping(data, spec):
    width = spec['width']
    original = int.from_bytes(data[:width], 'little')
    result = original
    for destination, source in spec['moves']:
        result = (result & ~(1 << destination)) | (((original >> source) & 1) << destination)
    result ^= int(spec['xor_hex'], 16)
    return result.to_bytes(width, 'little') + data[width:]


def main():
    source = ROOT / 're/evidence/ghidra/phase4_all_twisters.c'
    text = source.read_text()
    blocks = re.findall(r'(void FUN_[0-9a-f]+\([^)]*\)\s*\{.*?\n\})', text, re.S)
    if len(blocks) != 160 or 'Unavailable:' in text:
        raise ValueError('Expected 160 complete transforms')
    # Correct x86 integer pointer declarations for the host's pointer width.
    # Byte stores and shifts use unsigned storage to avoid signed-shift C UB.
    blocks = [re.sub(r'(void FUN_[0-9a-f]+\()int param_1', r'\1uintptr_t param_1', b)
              .replace('*(char *)', '*(byte *)') for b in blocks]
    for block in blocks:
        if len(re.findall(r'FUN_[0-9a-f]+\(', block)) != 1:
            raise ValueError('Transform calls another routine')
    code = ('// Recovered static byte transforms; no PE is loaded.\n'
            '#include <stdint.h>\n#include <stdbool.h>\n'
            'typedef uint8_t byte; typedef uint8_t undefined1; typedef unsigned int uint;\n' +
            '\n\n'.join(blocks) + '\n')
    ref = ROOT / 're/evidence/static/phase4_twisters_reference.c'
    ref.write_text(code)
    rows = json.loads((ROOT / 're/evidence/static/phase3_payload_registry.json').read_text())['entries'][:80]
    rng = random.Random(0xC91017)
    checks = 0
    result = []
    with tempfile.TemporaryDirectory(prefix='c9-twister-') as temp:
        library = Path(temp) / 'twisters.dylib'
        subprocess.run(['/usr/bin/clang', '-O2', '-Wall', '-Wextra', '-Werror',
                        '-Wno-self-assign', '-Wno-unused-variable', '-dynamiclib',
                        str(ref), '-o', str(library)], check=True, capture_output=True)
        native = ctypes.CDLL(str(library))

        def run(function, data):
            # Guard the memory after the supplied payload.
            buffer = (ctypes.c_ubyte * (len(data) + 64))(*data, *([0xA5] * 64))
            function(buffer, len(data))
            if bytes(buffer[len(data):]) != b'\xa5' * 64:
                raise ValueError('Transform exceeded payload length')
            return bytes(buffer[:len(data)])

        for row in rows:
            item = {'selector': row['selector']}
            for direction, address in zip(['encode', 'decode'], row['virtual_entries'][1:]):
                fn = getattr(native, 'FUN_' + address)
                fn.argtypes = [ctypes.c_void_p, ctypes.c_int]
                fn.restype = None
                specs = []
                for width, lengths in [(4, [4, 5, 7]), (8, [8, 9, 15]),
                                       (16, [16, 17, 31]), (32, [32, 33, 52, 127, 256, 2162])]:
                    bias_bytes = run(fn, bytes(width))
                    bias = int.from_bytes(bias_bytes, 'little')
                    mapping = [None] * (8 * width)
                    for source_bit in range(8 * width):
                        delta = int.from_bytes(run(fn, (1 << source_bit).to_bytes(width, 'little')), 'little') ^ bias
                        if delta == 0 or delta & (delta - 1):
                            raise ValueError('Not a bit permutation: ' + address)
                        destination = delta.bit_length() - 1
                        if mapping[destination] is not None:
                            raise ValueError('Mapping is not bijective')
                        mapping[destination] = source_bit
                    spec = {'width': width, 'xor_hex': f'{bias:0{width * 2}x}',
                            'moves': [[d, s] for d, s in enumerate(mapping) if d != s]}
                    for length in lengths:
                        for sample in range(12):
                            data = bytes(rng.randrange(256) for _ in range(length))
                            if apply_mapping(data, spec) != run(fn, data):
                                raise ValueError(f'Native/table mismatch {address} length={length}')
                            checks += 1
                    specs.append(spec)
                for size in range(4):
                    data = bytes(rng.randrange(256) for _ in range(size))
                    if run(fn, data) != data:
                        raise ValueError('Unexpected short-payload transform')
                item[direction] = {'source_va': address, 'length_branches': specs}
            # Prove the independently recovered forward and inverse affine maps
            # compose to identity on zero and every basis vector in each branch.
            for e, d in zip(item['encode']['length_branches'], item['decode']['length_branches']):
                width = e['width']
                for value in [0] + [1 << bit for bit in range(8 * width)]:
                    data = value.to_bytes(width, 'little')
                    if apply_mapping(apply_mapping(data, e), d) != data:
                        raise ValueError('Recovered encode/decode do not invert')
                    checks += 1
            result.append(item)
    data = {'client_sha256': 'b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25',
            'export_sha256': hashlib.sha256(source.read_bytes()).hexdigest(),
            'reference_c_sha256': hashlib.sha256(ref.read_bytes()).hexdigest(),
            'format': 'C9 bit permutation + XOR v1', 'selector_modulus': 80,
            'status': 'Static recovered transforms, no original-client runtime validation',
            'entries': result}
    destination = ROOT / 'c9server/data/twisters.json'
    destination.parent.mkdir(parents=True, exist_ok=True)
    destination.write_text(json.dumps(data, separators=(',', ':')) + '\n')
    validation = {'selectors': 80, 'directions': 160, 'length_branches': 640,
                  'native_comparisons_and_inverse_basis_checks': checks,
                  'reference': 'Newly compiled recovered C, not the original PE',
                  'runtime_table_sha256': hashlib.sha256(destination.read_bytes()).hexdigest()}
    (ROOT / 're/evidence/static/phase4_twister_validation.json').write_text(json.dumps(validation, indent=2) + '\n')
    print(json.dumps(validation))


if __name__ == '__main__':
    main()
