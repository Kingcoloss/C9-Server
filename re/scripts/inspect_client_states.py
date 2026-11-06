#!/usr/bin/env python3
"""Read selected C9 state-object RTTI without suspending or modifying the game.

The process path, source hash and fixed state-getter bytes are checked first.
Only the state-array pointer, slots 2–6, vtable RTTI and known state DWORDs
are read. No packet buffers, account data or Client code are exported.
"""
import argparse
import ctypes
from ctypes import wintypes as w
from datetime import datetime, timezone
import hashlib
import json
from pathlib import Path
import struct

from c9_paths import resolve_client_dir

EXPECTED_SHA256 = 'b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25'
GETTER = bytes.fromhex('558bec51894dfc8b45088b4dfc8b04818be55dc20400')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--process-id', type=int, required=True)
    parser.add_argument('--client-dir', type=Path)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    expected_path = (resolve_client_dir(args.client_dir) / 'C9.exe').resolve()
    if hashlib.sha256(expected_path.read_bytes()).hexdigest() != EXPECTED_SHA256:
        raise ValueError('Selected Client hash differs from the fixed research target')
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
        if Path(path.value).resolve() != expected_path:
            raise ValueError('Selected process is not the configured Client')

        def read(address, size):
            if not 0x10000 <= address < address + size <= 0x80000000 or not 0 < size <= 128:
                raise ValueError('Read outside bounded x86 diagnostic range')
            buffer = ctypes.create_string_buffer(size)
            count = ctypes.c_size_t()
            if not api.ReadProcessMemory(process, address, buffer, size, ctypes.byref(count)):
                raise ctypes.WinError(ctypes.get_last_error())
            if count.value != size:
                raise ValueError('Incomplete diagnostic read')
            return buffer.raw

        def dword(address):
            return struct.unpack('<I', read(address, 4))[0]

        def image_address(address):
            return 0x00400000 <= address < 0x01938000

        if read(0x009bf800, len(GETTER)) != GETTER:
            raise ValueError('Runtime state getter differs from the verified fixed-base code')
        # The 0x2EE2 and 0x2EE9 dispatch branches pass this object to the getter.
        manager = dword(0x019207e8)
        active = dword(manager + 0x1c)
        states = []
        for index, state_offset in [(2, 0x4c), (3, None), (4, 0x6c), (5, 0x22c), (6, 0x62c)]:
            pointer = dword(manager + index * 4)
            item = {'index': index, 'available': bool(pointer), 'active': bool(pointer and pointer == active)}
            if pointer:
                vtable = dword(pointer)
                if not image_address(vtable):
                    raise ValueError('State vtable is outside the selected Client image')
                locator = dword(vtable - 4)
                if not image_address(locator):
                    raise ValueError('RTTI locator is outside the selected Client image')
                descriptor = dword(locator + 12)
                if not image_address(descriptor):
                    raise ValueError('RTTI descriptor is outside the selected Client image')
                name = read(descriptor + 8, 128).split(b'\0', 1)[0].decode('ascii')
                if not name.startswith('.?AV'):
                    raise ValueError('Unexpected state RTTI name')
                item.update(type=name, vtable_va=f'{vtable:08x}')
                if state_offset is not None:
                    item.update(state_offset=f'0x{state_offset:x}', state_value=dword(pointer + state_offset))
            states.append(item)
    finally:
        api.CloseHandle(process)
    report = {'recorded_at_utc': datetime.now(timezone.utc).isoformat(),
              'process_id': args.process_id, 'source_sha256': EXPECTED_SHA256,
              'read_only': True, 'process_suspended': False, 'states': states}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(report))


if __name__ == '__main__':
    main()
