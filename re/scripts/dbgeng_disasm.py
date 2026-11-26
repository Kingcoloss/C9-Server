"""Disassemble x86 code (and read memory) from a C9 client minidump with the Windows dbgeng.dll, read-only.

Usage: python re/scripts/dbgeng_disasm.py <C9.DMP> <hex address>[:<instructions>] ...
The dump is opened as a debugger target: no process is started and no symbols are downloaded (_NT_SYMBOL_PATH points to
an empty local folder). The target is switched to 32-bit (the client runs under WOW64). Used for
docs/ITEM_CATALOG_DELIVERY_TH.md section 7.1. The dump and anything read from it stay outside git.
"""
import ctypes, os, sys
from ctypes import c_void_p, c_ulong, c_ulonglong, c_char_p, POINTER, byref, HRESULT
os.environ['_NT_SYMBOL_PATH'] = os.path.join(os.environ.get('TEMP', '.'), 'c9-dbgeng-nosyms')
os.makedirs(os.environ['_NT_SYMBOL_PATH'], exist_ok=True)
os.environ['_NT_ALT_SYMBOL_PATH'] = os.environ['_NT_SYMBOL_PATH']
class GUID(ctypes.Structure):
    _fields_ = [('a', c_ulong), ('b', ctypes.c_ushort), ('c', ctypes.c_ushort), ('d', ctypes.c_ubyte * 8)]
def guid(s):
    import uuid
    u = uuid.UUID(s); g = GUID(); g.a, g.b, g.c = u.fields[0], u.fields[1], u.fields[2]
    g.d[:] = list(u.bytes[8:]); return g
eng = ctypes.WinDLL(r'C:\Windows\System32\dbgeng.dll')
def method(obj, index, *argtypes):
    vtbl = ctypes.cast(obj, POINTER(POINTER(c_void_p))).contents
    fn = ctypes.WINFUNCTYPE(HRESULT, c_void_p, *argtypes)(vtbl[index])
    return lambda *a: fn(obj, *a)
class Dump:
    def __init__(self, path):
        self.client = c_void_p()
        eng.DebugCreate(byref(guid('27fe5639-8407-4f47-8364-ee118fb08ac8')), byref(self.client))
        self.control = c_void_p()
        method(self.client, 0, POINTER(GUID), POINTER(c_void_p))(byref(guid('5182e668-105e-416e-ad92-24ef800424ba')), byref(self.control))
        self.data = c_void_p()
        method(self.client, 0, POINTER(GUID), POINTER(c_void_p))(byref(guid('88f7dfab-3ea7-4c3a-aefb-c4e8106173aa')), byref(self.data))
        method(self.client, 19, c_char_p)(path.encode())          # IDebugClient::OpenDumpFile
        method(self.control, 93, c_ulong, c_ulong)(0, 0xffffffff)  # IDebugControl::WaitForEvent
        method(self.control, 48, c_ulong)(0x14c)                  # SetEffectiveProcessorType(I386)
        self._dis = method(self.control, 26, c_ulonglong, c_ulong, c_char_p, c_ulong, POINTER(c_ulong), POINTER(c_ulonglong))
        self._read = method(self.data, 3, c_ulonglong, c_void_p, c_ulong, POINTER(c_ulong))  # IDebugDataSpaces::ReadVirtual
    def dis(self, addr):
        buf = ctypes.create_string_buffer(512); size = c_ulong(); end = c_ulonglong()
        hr = self._dis(addr, 0, buf, 512, byref(size), byref(end))
        return hr, buf.value.decode('latin1').rstrip(), end.value
    def read(self, addr, n):
        buf = ctypes.create_string_buffer(n); got = c_ulong()
        hr = self._read(addr, buf, n, byref(got)); return buf.raw[:got.value]
if __name__ == '__main__':
    d = Dump(sys.argv[1])
    for spec in sys.argv[2:]:
        a, n = spec.split(':') if ':' in spec else (spec, '40')
        a = int(a, 16); n = int(n)
        for _ in range(n):
            hr, text, end = d.dis(a)
            if hr != 0 or not end: print(f'{a:08x} ?? hr={hr & 0xffffffff:#x}'); break
            print(text.replace('\n', ' | ')); a = end
        print('-' * 20)
