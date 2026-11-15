"""Linear-sweep disassembly of a PE .text (static only) -> pickle of instructions.

Stores (address, size, mnemonic, op_str) for every instruction so later scripts can
query patterns without re-disassembling.
"""
import pickle
import sys

import pefile
from capstone import CS_ARCH_X86, CS_MODE_32, Cs

path, out = sys.argv[1], sys.argv[2]
pe = pefile.PE(path, fast_load=True)
base = pe.OPTIONAL_HEADER.ImageBase
text = [s for s in pe.sections if s.Name.startswith(b".text")][0]
code = text.get_data()
va0 = base + text.VirtualAddress
md = Cs(CS_ARCH_X86, CS_MODE_32)
md.skipdata = True
ins = []
for i in md.disasm_lite(code, va0):
    ins.append(i)
with open(out, "wb") as f:
    pickle.dump({"base": base, "va0": va0, "ins": ins}, f)
print(len(ins), "instructions")
