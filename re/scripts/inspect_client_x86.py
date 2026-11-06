#!/usr/bin/env python3
"""Read a selected WOW64 thread context and a bounded stack snapshot.

The selected thread is briefly suspended and always resumed in finally.
No code, Client files, security settings, or privileges are changed.
"""
import argparse
import ctypes
from ctypes import wintypes as w
import json
from pathlib import Path
import re
import struct


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--process-id', type=int, required=True)
    parser.add_argument('--thread-id', type=int, required=True)
    parser.add_argument('--modules-log', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    api = ctypes.WinDLL('kernel32', use_last_error=True)
    api.OpenProcess.argtypes = [w.DWORD, w.BOOL, w.DWORD]
    api.OpenProcess.restype = w.HANDLE
    api.OpenThread.argtypes = [w.DWORD, w.BOOL, w.DWORD]
    api.OpenThread.restype = w.HANDLE
    api.GetProcessIdOfThread.argtypes = [w.HANDLE]
    api.GetProcessIdOfThread.restype = w.DWORD
    api.SuspendThread.argtypes = [w.HANDLE]
    api.SuspendThread.restype = w.DWORD
    api.ResumeThread.argtypes = [w.HANDLE]
    api.ResumeThread.restype = w.DWORD
    api.Wow64GetThreadContext.argtypes = [w.HANDLE, ctypes.c_void_p]
    api.Wow64GetThreadContext.restype = w.BOOL
    api.ReadProcessMemory.argtypes = [w.HANDLE, ctypes.c_void_p, ctypes.c_void_p,
                                     ctypes.c_size_t, ctypes.POINTER(ctypes.c_size_t)]
    api.ReadProcessMemory.restype = w.BOOL
    api.CloseHandle.argtypes = [w.HANDLE]
    api.CloseHandle.restype = w.BOOL
    modules = []
    for line in args.modules_log.read_text(encoding='utf-8', errors='replace').splitlines():
        match = re.match(r'^([0-9a-f`]+)\s+([0-9a-f`]+)\s+(\S+)\s+\(', line)
        if match:
            modules.append((int(match[1].replace('`', ''), 16),
                            int(match[2].replace('`', ''), 16), match[3]))

    def describe(address):
        for start, end, name in modules:
            if start <= address < end:
                return f'{name}+0x{address-start:x}'
        return None

    process = api.OpenProcess(0x410, False, args.process_id)
    if not process:
        raise ctypes.WinError(ctypes.get_last_error())
    thread = None
    suspended = False
    try:
        thread = api.OpenThread(0x4a, False, args.thread_id)
        if not thread:
            raise ctypes.WinError(ctypes.get_last_error())
        if api.GetProcessIdOfThread(thread) != args.process_id:
            raise ValueError('The selected thread no longer belongs to the selected process')
        if api.SuspendThread(thread) == 0xffffffff:
            raise ctypes.WinError(ctypes.get_last_error())
        suspended = True
        context = ctypes.create_string_buffer(716)
        struct.pack_into('<I', context, 0, 0x10003)
        if not api.Wow64GetThreadContext(thread, context):
            raise ctypes.WinError(ctypes.get_last_error())
        registers = {name: struct.unpack_from('<I', context, offset)[0]
                     for name, offset in [('edi',156), ('esi',160), ('ebx',164), ('edx',168),
                                          ('ecx',172), ('eax',176), ('ebp',180), ('eip',184), ('esp',196)]}

        def read(address, size):
            buffer = ctypes.create_string_buffer(size)
            count = ctypes.c_size_t()
            if not api.ReadProcessMemory(process, address, buffer, size, ctypes.byref(count)):
                raise ctypes.WinError(ctypes.get_last_error())
            return buffer.raw[:count.value]

        frames = [{'address': f'0x{registers["eip"]:08x}', 'module': describe(registers['eip'])}]
        frame = registers['ebp']
        seen = set()
        for _ in range(40):
            if frame in seen or frame < registers['esp'] or frame > registers['esp'] + 0x100000:
                break
            seen.add(frame)
            try:
                following, returned = struct.unpack('<II', read(frame, 8))
            except (OSError, struct.error):
                break
            frames.append({'frame': f'0x{frame:08x}', 'address': f'0x{returned:08x}', 'module': describe(returned)})
            if following <= frame:
                break
            frame = following
        stack = read(registers['esp'], 2048)
        candidates = []
        for index, (word,) in enumerate(struct.iter_unpack('<I', stack)):
            module = describe(word)
            if module:
                candidates.append({'stack_offset': index*4, 'value': f'0x{word:08x}', 'module': module})
        result = {'process_id': args.process_id, 'thread_id': args.thread_id,
                  'registers': {k: f'0x{v:08x}' for k, v in registers.items()},
                  'ebp_chain': frames, 'possible_stack_pointers_not_verified_frames': candidates}
    finally:
        if suspended:
            if api.ResumeThread(thread) == 0xffffffff:
                raise ctypes.WinError(ctypes.get_last_error())
        if thread:
            api.CloseHandle(thread)
        api.CloseHandle(process)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(result, indent=2))


if __name__ == '__main__':
    main()
