#!/usr/bin/env python3
"""Watch an explicitly selected original C9 process for native exceptions.

Uses the Windows debugging API with kill-on-debugger-exit disabled. Events
are continued without changing exception handling, except the initial attach
breakpoint. Captures bounded x86 registers, stack addresses and code bytes;
does not inject code, patch memory, enable privileges, or change OS settings.
Keep output in ignored artifacts: stack values may contain local data.

API contracts: https://learn.microsoft.com/windows/win32/debug/debugging-events
"""
import argparse
import ctypes
from ctypes import wintypes as w
import hashlib
import json
from pathlib import Path
import struct
import time

from c9_static_inventory import PE


CLIENT_SHA256 = 'b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25'


class ExceptionRecord(ctypes.Structure):
    _fields_ = [('code', w.DWORD), ('flags', w.DWORD),
                ('record', ctypes.c_void_p), ('address', ctypes.c_void_p),
                ('parameter_count', w.DWORD), ('information', ctypes.c_size_t * 15)]


class ExceptionInfo(ctypes.Structure):
    _fields_ = [('record', ExceptionRecord), ('first_chance', w.DWORD)]


class EventData(ctypes.Union):
    _fields_ = [('exception', ExceptionInfo), ('raw', ctypes.c_ubyte * 160)]


class DebugEvent(ctypes.Structure):
    _fields_ = [('kind', w.DWORD), ('process_id', w.DWORD),
                ('thread_id', w.DWORD), ('data', EventData)]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--process-id', type=int, required=True)
    parser.add_argument('--client-exe', type=Path, required=True)
    parser.add_argument('--seconds', type=int, default=300)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    if not 1 <= args.seconds <= 600:
        parser.error('seconds must be between 1 and 600')
    if not args.output.resolve().is_relative_to(Path(__file__).resolve().parents[2] / 'artifacts'):
        parser.error('Raw diagnostics must remain in the ignored artifacts directory')
    if (ctypes.sizeof(ctypes.c_void_p) != 8 or ctypes.sizeof(ExceptionRecord) != 152 or
            ExceptionInfo.first_chance.offset != 152 or DebugEvent.data.offset != 16 or
            ctypes.sizeof(DebugEvent) != 176):
        raise RuntimeError('This helper requires the 64-bit Windows debugger structure layout')
    expected = args.client_exe.resolve(strict=True)
    if expected.name.lower() != 'c9.exe' or hashlib.sha256(expected.read_bytes()).hexdigest() != CLIENT_SHA256:
        raise ValueError('Client is not the selected original C9.exe')
    pe = PE(expected)
    api = ctypes.WinDLL('kernel32', use_last_error=True)
    signatures = {
        'OpenProcess': (w.HANDLE, [w.DWORD, w.BOOL, w.DWORD]),
        'OpenThread': (w.HANDLE, [w.DWORD, w.BOOL, w.DWORD]),
        'CloseHandle': (w.BOOL, [w.HANDLE]),
        'QueryFullProcessImageNameW': (w.BOOL, [w.HANDLE, w.DWORD, w.LPWSTR, ctypes.POINTER(w.DWORD)]),
        'ReadProcessMemory': (w.BOOL, [w.HANDLE, ctypes.c_void_p, ctypes.c_void_p,
                                     ctypes.c_size_t, ctypes.POINTER(ctypes.c_size_t)]),
        'Wow64GetThreadContext': (w.BOOL, [w.HANDLE, ctypes.c_void_p]),
        'DebugActiveProcess': (w.BOOL, [w.DWORD]),
        'DebugActiveProcessStop': (w.BOOL, [w.DWORD]),
        'DebugSetProcessKillOnExit': (w.BOOL, [w.BOOL]),
        'WaitForDebugEvent': (w.BOOL, [ctypes.POINTER(DebugEvent), w.DWORD]),
        'ContinueDebugEvent': (w.BOOL, [w.DWORD, w.DWORD, w.DWORD]),
    }
    for name, (result, arguments) in signatures.items():
        function = getattr(api, name)
        function.restype, function.argtypes = result, arguments

    def check(result):
        if not result:
            raise ctypes.WinError(ctypes.get_last_error())
        return result

    process = check(api.OpenProcess(0x1010, False, args.process_id))
    attached = False
    result = {'process_id': args.process_id, 'client_sha256': CLIENT_SHA256,
              'exceptions': [], 'snapshots': [], 'seconds': args.seconds}
    args.output.parent.mkdir(parents=True, exist_ok=True)

    def save():
        args.output.write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')

    def read(address, size):
        if not 0 < size <= 2048:
            raise ValueError('Read exceeds the diagnostic bound')
        buffer = ctypes.create_string_buffer(size)
        count = ctypes.c_size_t()
        check(api.ReadProcessMemory(process, address, buffer, size, ctypes.byref(count)))
        if count.value != size:
            raise OSError('Incomplete process memory read')
        return buffer.raw

    def snapshot(thread_id):
        # Debug events already suspend all target threads. No additional suspend.
        thread = check(api.OpenThread(0x48, False, thread_id))
        try:
            context = ctypes.create_string_buffer(716)
            struct.pack_into('<I', context, 0, 0x10003)
            check(api.Wow64GetThreadContext(thread, context))
            registers = {name: struct.unpack_from('<I', context, offset)[0]
                         for name, offset in [('edi', 156), ('esi', 160), ('ebx', 164),
                             ('edx', 168), ('ecx', 172), ('eax', 176), ('ebp', 180),
                             ('eip', 184), ('esp', 196)]}
            item = {'thread_id': thread_id, 'registers': {k: f'0x{v:08x}' for k, v in registers.items()}}
            frames, frame, seen = [], registers['ebp'], set()
            for _ in range(40):
                if frame in seen or not registers['esp'] <= frame <= registers['esp'] + 0x100000:
                    break
                seen.add(frame)
                try:
                    following, returned = struct.unpack('<II', read(frame, 8))
                except OSError:
                    break
                frames.append({'frame': f'0x{frame:08x}', 'return': f'0x{returned:08x}'})
                if following <= frame:
                    break
                frame = following
            item['ebp_chain'] = frames
            try:
                stack = read(registers['esp'], 512)
                item['stack_words'] = [f'0x{x:08x}' for (x,) in struct.iter_unpack('<I', stack)]
            except OSError as error:
                item['stack_error'] = str(error)
            try:
                if 0x00400000 <= registers['eip'] <= 0x01938000 - 32:
                    item['client_code'] = read(registers['eip'], 32).hex()
            except (OSError, ValueError):
                pass
            return item
        finally:
            api.CloseHandle(thread)

    try:
        path_buffer, length = ctypes.create_unicode_buffer(32768), w.DWORD(32768)
        check(api.QueryFullProcessImageNameW(process, 0, path_buffer, ctypes.byref(length)))
        if Path(path_buffer.value).resolve() != expected:
            raise ValueError('Selected PID does not refer to the selected Client')
        offset = pe.off(0x004b5890 - pe.base)
        if offset is None or read(0x004b5890, 32) != pe.data[offset:offset + 32]:
            raise ValueError('Selected process code does not match the fixed-base Client')
        check(api.DebugActiveProcess(args.process_id))
        attached = True
        check(api.DebugSetProcessKillOnExit(False))
        print('C9_CRASH_WATCH_ATTACHED', flush=True)
        deadline = time.monotonic() + args.seconds
        initial_breakpoint = True
        while time.monotonic() < deadline:
            event = DebugEvent()
            if not api.WaitForDebugEvent(ctypes.byref(event), 500):
                error = ctypes.get_last_error()
                if error in (121, 258):
                    continue
                raise ctypes.WinError(error)
            status = 0x00010002  # DBG_CONTINUE for non-exception events.
            exited = event.kind == 5
            try:
                if event.process_id != args.process_id:
                    raise RuntimeError('Unexpected process in debugger events')
                if event.kind == 1:
                    exception = event.data.exception
                    record = {'code': f'0x{exception.record.code:08x}',
                              'address': f'0x{exception.record.address or 0:016x}',
                              'thread_id': event.thread_id, 'first_chance': bool(exception.first_chance),
                              'information': [f'0x{x:x}' for x in exception.record.information[:min(exception.record.parameter_count, 15)]]}
                    result['exceptions'].append(record)
                    status = 0x80010001  # Pass application exceptions through normally.
                    if initial_breakpoint and exception.record.code in (0x80000003, 0x4000001f):
                        initial_breakpoint = False
                        status = 0x00010002
                    elif exception.record.code == 0xc0000005 and len(result['snapshots']) < 8:
                        try:
                            item = snapshot(event.thread_id)
                            item['exception_index'] = len(result['exceptions']) - 1
                            result['snapshots'].append(item)
                        except OSError as error:
                            record['snapshot_error'] = str(error)
                        print(json.dumps(record), flush=True)
                    save()
                elif event.kind in (3, 6):
                    # Debugger must close the image-file handle supplied by these events.
                    handle = struct.unpack_from('<Q', event.data.raw)[0]
                    if handle:
                        api.CloseHandle(handle)
                elif exited:
                    result['exit_code'] = f'0x{struct.unpack_from("<I", event.data.raw)[0]:08x}'
            finally:
                check(api.ContinueDebugEvent(event.process_id, event.thread_id, status))
            if exited:
                attached = False
                break
        result['finished'] = True
    finally:
        if attached:
            result['detached'] = bool(api.DebugActiveProcessStop(args.process_id))
        api.CloseHandle(process)
        save()
    print(json.dumps({'output': str(args.output.resolve()), 'snapshots': len(result['snapshots']),
                      'exit_code': result.get('exit_code'), 'detached': result.get('detached')}), flush=True)


if __name__ == '__main__':
    main()
