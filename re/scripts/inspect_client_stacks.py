#!/usr/bin/env python3
"""Inspect an explicitly selected Windows process using the system DbgEng engine.

Uses noninvasive/no-suspend attach, fixed read commands, and detaches in finally.
Does not inject code, change Client files, install handlers, or enable privileges.
Interface slots verified against Microsoft's DbgEng.h in win32metadata.
"""
import argparse
import ctypes
from pathlib import Path
import uuid


GUID = ctypes.c_ubyte * 16
HRESULT = ctypes.c_long
ULONG = ctypes.c_ulong
PTR = ctypes.c_void_p


def guid(value):
    return GUID.from_buffer_copy(uuid.UUID(value).bytes_le)


def method(interface, index, *arguments):
    table = ctypes.cast(interface, ctypes.POINTER(ctypes.POINTER(PTR))).contents
    return ctypes.WINFUNCTYPE(HRESULT, PTR, *arguments)(table[index])


def check(result, operation):
    if result < 0:
        raise RuntimeError(f'{operation} failed: HRESULT 0x{result & 0xffffffff:08x}')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--process-id', type=int)
    parser.add_argument('--server-fields', action='store_true', help='Read fixed server-name conversion and selection routines')
    parser.add_argument('--handoff-fields', action='store_true', help='Read fixed login-to-server handoff routines')
    parser.add_argument('--entry-state', action='store_true', help='Read fixed client state lookup and manager pointer')
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    args.output.parent.mkdir(parents=True, exist_ok=True)
    engine = ctypes.WinDLL('dbgeng')
    engine.DebugCreate.argtypes = [ctypes.POINTER(GUID), ctypes.POINTER(PTR)]
    engine.DebugCreate.restype = HRESULT
    client, control = PTR(), PTR()
    check(engine.DebugCreate(ctypes.byref(guid('27fe5639-8407-4f47-8364-ee118fb08ac8')),
                             ctypes.byref(client)), 'DebugCreate')
    attached = False
    logging = False
    try:
        check(method(client, 0, ctypes.POINTER(GUID), ctypes.POINTER(PTR))(
            client, ctypes.byref(guid('5182e668-105e-416e-ad92-24ef800424ba')), ctypes.byref(control)),
            'QueryInterface(IDebugControl)')
        check(method(control, 8, ctypes.c_char_p, ctypes.c_int)(
            control, str(args.output.resolve()).encode('mbcs'), 0), 'OpenLogFile')
        logging = True
        execute = method(control, 66, ULONG, ctypes.c_char_p, ULONG)
        check(execute(control, 0, b'.echo C9_NONINVASIVE_DIAGNOSTIC', 0), 'Engine check')
        if args.process_id is not None:
            check(method(client, 12, ctypes.c_ulonglong, ULONG, ULONG)(
                client, 0, args.process_id, 5), 'AttachProcess(NONINVASIVE|NO_SUSPEND)')
            attached = True
            result = method(control, 93, ULONG, ULONG)(control, 0, 5000)
            check(result, 'WaitForEvent')
            if result != 0:
                raise TimeoutError('DbgEng attach event did not arrive within five seconds')
            commands = [b'.sympath', b'lm', b'~* kv', b'.effmach x86',
                        b'db PhysXLoader+0x269f L20', b'~* kv']
            if args.server_fields:
                # C9 is also a valid hexadecimal literal to the MASM evaluator;
                # use verified preferred VAs for this fixed-base Client image.
                commands = [b'.effmach x86', b'lm m C9', b'u 0045dfe0 L40',
                            b'u 0045e0b0 L10', b'u 00b1d770 L60']
            if args.handoff_fields:
                commands = [b'.effmach x86', b'lm m C9', b'u 00a65910 L40',
                            b'u 00a68d40 L40', b'u 00a68da0 L30',
                            b'u 00a62dc0 L90', b'u 00563820 L80']
            if args.entry_state:
                commands = [b'.effmach x86', b'lm m C9', b'u 009bf800 L60',
                            b'dd 019207e8 L1']
            for command in commands:
                check(execute(control, 0, command, 0), command.decode('ascii'))
    finally:
        if attached:
            method(client, 26, ULONG)(client, 0)  # DEBUG_END_PASSIVE: leave target running.
        if control:
            if logging:
                method(control, 9)(control)
            method(control, 2)(control)
        if client:
            method(client, 2)(client)
    print(args.output.resolve())


if __name__ == '__main__':
    main()
