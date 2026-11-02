#!/usr/bin/env python3
"""Rebuild static Phase 3 catalogs from PE bytes and bounded Ghidra exports.

Reads the client as data only; does not load or execute Windows code.
"""
import argparse
import hashlib
import json
import re
from pathlib import Path

from c9_static_inventory import PE
from c9_rtti_map import executable_va
from c9_paths import resolve_client_file


def function_text(text, va):
    match = re.search(r'/\* ' + va + r' FUN_' + va +
                      r' \*/(.*?)(?=\n/\* [0-9a-f]{8} |\Z)', text, re.S)
    if not match:
        raise ValueError('Missing export: ' + va)
    return match.group(1)


def table_info(pe, va, slots):
    at = pe.off(va - pe.base)
    col = pe.u32(at - 4)
    col_at = pe.off(col - pe.base)
    if pe.u32(col_at) != 0 or pe.u32(col_at + 4) != 0:
        raise ValueError('Unexpected complete-object locator')
    td = pe.u32(col_at + 12)
    name = pe.cstr(pe.off(td - pe.base) + 8)
    entries = [pe.u32(at + i * 4) for i in range(slots)]
    if not all(executable_va(pe, entry) for entry in entries):
        raise ValueError('Non-code vtable entry')
    return {'vtable_va': f'{va:08x}', 'locator_va': f'{col:08x}',
            'type_descriptor_va': f'{td:08x}', 'rtti_name': name,
            'virtual_entries': [f'{v:08x}' for v in entries]}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root', type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument('--client-dir', type=Path)
    args = parser.parse_args()
    evidence = args.root / 're/evidence'
    pe = PE(resolve_client_file('C9.exe', args.client_dir, args.root))
    digest = hashlib.sha256(pe.data).hexdigest()
    if digest != 'b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25':
        raise ValueError('Export addresses belong to a different client build')
    export = evidence / 'ghidra'
    initializer = function_text((export / 'phase3_registry_init.c').read_text(), '00410ae0')
    matches = re.findall(r'\*puStack_24 = &(?:UNK|PTR_[A-Za-z0-9_]+)_([0-9a-f]{8});'
                         r'.*?uStack_28 = (0x[0-9a-f]+|[0-9]+);', initializer, re.S)
    registry = [{'selector': int(key, 0), **table_info(pe, int(vt, 16), 3)}
                for vt, key in matches]
    if [r['selector'] for r in registry] != list(range(100)):
        raise ValueError('Incomplete or ambiguous registration extraction')
    for row in registry:
        if row['rtti_name'] != f".?AVPacketTwister_{row['selector'] + 1}@@":
            raise ValueError('Registration / RTTI disagreement')
        row['reachable_via_observed_mod80_wrappers'] = row['selector'] < 80
    wrappers = (export / 'phase3_cipher_factory.c').read_text()
    for address in ['004121d0', '00412230']:
        if 'param_1 = param_1 % 0x50' not in function_text(wrappers, address):
            raise ValueError('Selector rule changed')
    payload = {'source_sha256': digest, 'initializer_va': '00410ae0',
               'registered_count': 100, 'observed_wrapper_modulus': 80,
               'observed_wrappers': ['00412130', '004121d0', '00412230'],
               'scope': 'No claim about reachability through other callers', 'entries': registry}

    constructors = (export / 'phase3_cipher_constructors.c').read_text()
    factory = function_text(wrappers, '00dd5070')
    top = [(0, '00dd3680'), (1, '00dd2880'), (2, '00dd2c10'), (3, '00dd3430'),
           (4, '00dd2ea0'), (5, '00dd3000'), (6, '00dd31a0'), (7, '00dd0c30'), (8, '00dd3430')]
    sub = ['00dd38d0', '00dd0ec0', '00dd1150', '00dd1320', '00dd1630',
           '00dd3bc0', '00dd4bb0', '00dd4e90']
    branches = []
    for selector, secondary, ctor in ([(n, None, c) for n, c in top] +
                                      [(9, n, c) for n, c in enumerate(sub)]):
        if 'FUN_' + ctor + '()' not in factory:
            raise ValueError('Constructor missing from factory')
        body = function_text(constructors, ctor)
        vt = re.search(r'\*param_1 = &UNK_([0-9a-f]{8});', body).group(1)
        info = table_info(pe, int(vt, 16), 4)
        algorithm = re.search(r'ConcreteCipher@V([^@]+)@CryptoPP', info['rtti_name']).group(1)
        branches.append({'selector_mod10': selector, 'key_byte0_and7': secondary,
                         'constructor_va': ctor, 'algorithm_class': algorithm, **info})
    hash_tables = [('1', '011d91a4'), ('3', '011d9214'), ('4', '011d9284'),
                   ('5', '011d92f4'), ('6/default(0,2)', '011d935c'),
                   ('7', '011d93c4'), ('8', '011d9434'), ('9', '011d94a4'), ('10', '011d9514')]
    hashes = [{'branch_mod11': branch, **table_info(pe, int(vt, 16), 1)}
              for branch, vt in hash_tables]
    cipher = {'source_sha256': digest, 'factory_va': '00dd5070',
              'selector_rule': 'selector % 10; case 9 uses key[0] & 7',
              'evidence': ['phase3_cipher_factory.c', 'phase3_cipher_constructors.c',
                           'phase2_codec_state.c', 'PE RTTI and vtable pointers'],
              'status': 'Static class selection confirmed; full key schedule and wire interoperability pending',
              'branches': branches, 'key_derivation_hash_classes': hashes}
    for name, data in [('phase3_payload_registry.json', payload), ('phase3_cipher_catalog.json', cipher)]:
        (evidence / 'static' / name).write_text(json.dumps(data, indent=2) + '\n')
    print(json.dumps({'registered_twisters': len(registry), 'observed_selector_values': 80,
                      'cipher_branches': len(branches),
                      'cipher_classes': len({r['algorithm_class'] for r in branches}),
                      'hash_classes': len(hashes)}))


if __name__ == '__main__':
    main()
