#!/usr/bin/env python3
"""Locate MSVC x86 RTTI/vtable candidates without loading Windows code."""
import argparse
import json
import struct
from pathlib import Path
from c9_static_inventory import PE, strings


def occurrences(data, value):
    pattern = struct.pack('<I', value)
    start = 0
    while True:
        at = data.find(pattern, start)
        if at < 0:
            return
        yield at
        start = at + 1


def executable_va(pe, va):
    for section in pe.sections:
        if section['name'] == '.text':
            return pe.base + section['rva'] <= va < pe.base + section['rva'] + section['raw_size']
    return False


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('client', type=Path)
    parser.add_argument('output', type=Path)
    parser.add_argument('--classes', nargs='+', default=['C9NetworkLogin', 'C9NetworkManager', 'C9NetworkCommon', 'IC9NetworkHandler'])
    args = parser.parse_args()
    pe = PE(args.client)
    if pe.is64 or pe.machine != 0x14c:
        raise ValueError('This mapper is limited to MSVC x86 absolute RTTI layouts')
    results = []
    for s in strings(pe):
        names = [name for name in args.classes if s['value'] == '.?AV' + name + '@@']
        if not names or not s['va']:
            continue
        descriptor_va = int(s['va'], 16) - 8
        result = {'class': names[0], 'type_descriptor_va': f'{descriptor_va:08x}', 'vtables': []}
        for at in occurrences(pe.data, descriptor_va):
            col = at - 12
            if col < 0 or col + 20 > len(pe.data):
                continue
            signature, offset, cd_offset, td, hierarchy = struct.unpack_from('<IIIII', pe.data, col)
            col_va = pe.va_for_offset(col)
            if signature != 0 or offset > 0x10000 or cd_offset > 0x10000 or col_va is None:
                continue
            try:
                pe.off(hierarchy - pe.base)
            except ValueError:
                continue
            for pointer in occurrences(pe.data, col_va):
                table_va = pe.va_for_offset(pointer + 4)
                if table_va is None:
                    continue
                entries = []
                for slot in range(24):
                    pos = pointer + 4 + slot * 4
                    if pos + 4 > len(pe.data):
                        break
                    fn_va = pe.u32(pos)
                    if not executable_va(pe, fn_va):
                        break
                    entries.append({'slot': slot, 'function_va': f'{fn_va:08x}'})
                if entries:
                    result['vtables'].append({'locator_va': f'{col_va:08x}', 'vtable_va': f'{table_va:08x}', 'object_offset': offset, 'entries': entries})
        results.append(result)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(results, indent=2) + '\n')
    for item in results:
        print(item['class'], item['type_descriptor_va'])
        for table in item['vtables']:
            print('  vtable', table['vtable_va'], 'this_offset', table['object_offset'],
                  'functions', ','.join(e['function_va'] for e in table['entries']))


if __name__ == '__main__':
    main()
