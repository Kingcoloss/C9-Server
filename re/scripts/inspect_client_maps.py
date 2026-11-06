#!/usr/bin/env python3
"""Read the selected Client's complete MapData catalog without modifying it.

Reads only verified asset-table nodes, map metadata and bounded sub-map arrays.
Does not read accounts, packet buffers, suspend threads or write process memory.
"""
import argparse
import ctypes
from ctypes import wintypes as w
from datetime import datetime, timezone
import hashlib
import json
from pathlib import Path
import re
import struct

from c9_paths import resolve_client_dir

EXPECTED_SHA256 = 'b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25'
GETTER = bytes.fromhex('558bec51894dfc8b4508506a0f')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--process-id', type=int, required=True)
    parser.add_argument('--client-dir', type=Path)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    client = resolve_client_dir(args.client_dir).resolve()
    executable = client / 'C9.exe'
    if args.output.resolve().is_relative_to(client):
        parser.error('Reports must be written outside the Client directory')
    if hashlib.sha256(executable.read_bytes()).hexdigest() != EXPECTED_SHA256:
        raise ValueError('Client hash differs from the fixed research target')
    api = ctypes.WinDLL('kernel32', use_last_error=True)
    api.OpenProcess.argtypes = [w.DWORD, w.BOOL, w.DWORD]
    api.OpenProcess.restype = w.HANDLE
    api.QueryFullProcessImageNameW.argtypes = [w.HANDLE, w.DWORD, w.LPWSTR, ctypes.POINTER(w.DWORD)]
    api.QueryFullProcessImageNameW.restype = w.BOOL
    api.ReadProcessMemory.argtypes = [w.HANDLE, ctypes.c_void_p, ctypes.c_void_p,
                                    ctypes.c_size_t, ctypes.POINTER(ctypes.c_size_t)]
    api.ReadProcessMemory.restype = w.BOOL
    api.CloseHandle.argtypes = [w.HANDLE]
    api.CloseHandle.restype = w.BOOL
    process = api.OpenProcess(0x1010, False, args.process_id)
    if not process:
        raise ctypes.WinError(ctypes.get_last_error())
    try:
        path = ctypes.create_unicode_buffer(32768)
        length = w.DWORD(len(path))
        if not api.QueryFullProcessImageNameW(process, 0, path, ctypes.byref(length)):
            raise ctypes.WinError(ctypes.get_last_error())
        if Path(path.value).resolve() != executable:
            raise ValueError('Selected process is not the configured Client')
        total_read = 0

        def read(address, size):
            nonlocal total_read
            if not 0x10000 <= address < address + size <= 0x80000000 or not 0 < size <= 512:
                raise ValueError('Read outside bounded x86 asset range')
            total_read += size
            if total_read > 8 * 1024 * 1024:
                raise ValueError('Asset metadata read budget exceeded')
            buffer = ctypes.create_string_buffer(size)
            count = ctypes.c_size_t()
            if not api.ReadProcessMemory(process, address, buffer, size, ctypes.byref(count)):
                raise ctypes.WinError(ctypes.get_last_error())
            if count.value != size:
                raise ValueError('Incomplete asset read')
            return buffer.raw

        def dword(address):
            return struct.unpack('<I', read(address, 4))[0]

        def string(address, wide=False):
            raw = read(address, 28)
            size, capacity = struct.unpack_from('<II', raw, 20)
            unit, inline = (2, 8) if wide else (1, 16)
            if not 0 <= size <= 240 or not size <= capacity <= 1048576:
                raise ValueError('Asset string outside bounds')
            if not size:
                return ''
            data = raw[4:4 + size * unit] if capacity < inline else read(dword(address + 4), size * unit)
            return data.decode('utf-16le' if wide else 'ascii')

        if read(0x004b5930, len(GETTER)) != GETTER:
            raise ValueError('Runtime MapData getter differs from the selected image')
        asset = dword(0x013550b0)
        # 004B5930 -> 007350A0: container = assets +0x24 +15*0x2C.
        head, count = dword(asset + 0x2c0), dword(asset + 0x2c4)
        if not 0 < count <= 4096:
            raise ValueError('Map table count outside bounds')
        node, visited, maps = dword(head), set(), []
        for _ in range(count):
            if node == head or node in visited:
                raise ValueError('Map table ended early or contains a cycle')
            visited.add(node)
            key, record = struct.unpack('<II', read(node + 8, 8))
            if key != dword(record + 4) or not 0 < key <= 65535:
                raise ValueError('Map asset key mismatch')
            folder = string(record + 0x9c)
            if not re.fullmatch(r'[A-Za-z0-9_-]+', folder):
                raise ValueError('Unexpected map folder syntax')
            # 004B5890 bounds-checks the zero-based index. Vector elements are
            # 0x88 bytes (0068CC90 / 00698840); +0x68 is the file suffix.
            begin, end, capacity = struct.unpack('<III', read(record + 0xd8, 12))
            if not 0 <= end - begin <= 136 * 256 or (end - begin) % 136 or not end <= capacity:
                raise ValueError('Sub-map vector outside bounds')
            variants = []
            for index in range((end - begin) // 136):
                suffix = string(begin + index * 136 + 0x68)
                if not re.fullmatch(r'[A-Za-z0-9_-]*', suffix):
                    raise ValueError('Unexpected sub-map suffix syntax')
                variants.append({'index': index, 'suffix': suffix,
                                 'marker_file': folder + suffix + '.C9MM'})
            flags = list(struct.unpack('<IIII', read(record + 0x40, 16)))
            maps.append({'id': key, 'name': string(record + 8, wide=True),
                         'folder': folder, 'asset_fields_40_to_4c': flags,
                         'submaps': variants})
            node = dword(node)
        if node != head or len({m['id'] for m in maps}) != count:
            raise ValueError('Map table count or identity mismatch')
    finally:
        api.CloseHandle(process)
    report = {'schema_version': 1, 'recorded_at_utc': datetime.now(timezone.utc).isoformat(),
              'client_exe_sha256': EXPECTED_SHA256, 'read_only': True, 'process_suspended': False,
              'map_count': len(maps), 'folder_count': len({m['folder'] for m in maps}),
              'submap_count': sum(len(m['submaps']) for m in maps), 'maps': sorted(maps, key=lambda m: m['id'])}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    print(json.dumps({k: report[k] for k in ['map_count', 'folder_count', 'submap_count']}))


if __name__ == '__main__':
    main()
