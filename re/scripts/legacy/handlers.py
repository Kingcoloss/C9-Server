"""Recover packet-id -> handler registrations from a C9 server binary (static only).

Pattern (MSVC 2008, boost::function):
    mov eax, <handler>        ; handler address
    push reg ; push eax
    call <make_function>
    mov ecx, [<dispatcher singleton>]
    push <packet id>
    call <Register>
For each call to a candidate Register function, take the nearest preceding
'push imm' as the id and the nearest preceding 'mov reg, imm' that lands in
.text as the handler. Handlers are labeled with the strings they reference.

python handlers.py <pe> <pkl> <out.json> [register_va_hex ...]
If no register VA is given, candidates are discovered automatically: call targets
whose callers are preceded by 'mov ecx, dword ptr [X]' + 'push imm16'.
"""
import bisect
import collections
import json
import pickle
import re
import sys

import pefile

pe_path, pkl, out = sys.argv[1], sys.argv[2], sys.argv[3]
regs = [int(x, 16) for x in sys.argv[4:]]
pe = pefile.PE(pe_path, fast_load=True)
base = pe.OPTIONAL_HEADER.ImageBase
data = open(pe_path, "rb").read()
d = pickle.load(open(pkl, "rb"))
ins = d["ins"]
addrs = [i[0] for i in ins]
text = [s for s in pe.sections if s.Name.startswith(b".text")][0]
tlo, thi = base + text.VirtualAddress, base + text.VirtualAddress + text.Misc_VirtualSize


def va2off(va):
    for s in pe.sections:
        lo = base + s.VirtualAddress
        if lo <= va < lo + max(s.Misc_VirtualSize, s.SizeOfRawData):
            o = s.PointerToRawData + (va - lo)
            return o if o < s.PointerToRawData + s.SizeOfRawData else None
    return None


def read_str(va):
    o = va2off(va)
    if o is None:
        return None
    b = data[o : o + 400]
    # utf-16le
    m = re.match(rb"((?:[\x20-\x7e]\x00){4,})", b)
    if m:
        return m.group(1).decode("utf-16le")
    m = re.match(rb"([\x20-\x7e]{4,})", b)
    if m:
        return m.group(1).decode()
    return None


IMM = re.compile(r"^0x([0-9a-f]+)$")


def imm(op):
    m = IMM.match(op.strip())
    return int(m.group(1), 16) if m else None


calls = collections.defaultdict(list)
for k, i in enumerate(ins):
    if i[2] == "call":
        t = imm(i[3])
        if t is not None:
            calls[t].append(k)

if not regs:
    score = collections.Counter()
    for t, ks in calls.items():
        n = 0
        for k in ks:
            w = ins[max(0, k - 4) : k]
            if any(x[2] == "mov" and x[3].startswith("ecx, dword ptr [0x") for x in w) and any(
                x[2] == "push" and (imm(x[3]) or 0) in range(0x100, 0x10000) for x in w
            ):
                n += 1
        if n >= 15:
            score[t] = n
    regs = [t for t, _ in score.most_common(20)]
    print("auto register candidates:", [(hex(t), score[t]) for t in regs])


def fn_bounds(a):
    k = bisect.bisect_left(addrs, a)
    j = k
    while j < len(ins) and ins[j][2] not in ("int3",):
        j += 1
    return k, j


def strings_in(a, limit=6):
    k, j = fn_bounds(a)
    res = []
    for i in ins[k : min(j, k + 3000)]:
        for m in re.finditer(r"0x([0-9a-f]{6,8})", i[3]):
            v = int(m.group(1), 16)
            if not (tlo <= v < thi):
                s = read_str(v)
                if s and s not in res:
                    res.append(s)
        if len(res) >= limit:
            break
    return res


result = []
for r in regs:
    for k in calls.get(r, []):
        pid = None
        h = None
        disp = None
        for back in range(1, 14):
            x = ins[k - back]
            if pid is None and x[2] == "push" and imm(x[3]) is not None:
                pid = imm(x[3])
                continue
            if disp is None and x[2] == "mov" and x[3].startswith("ecx, dword ptr [0x"):
                disp = x[3].split("[")[1].rstrip("]")
            if pid is not None and h is None and x[2] in ("mov", "push", "lea"):
                m = re.search(r"0x([0-9a-f]+)$", x[3])
                if m:
                    v = int(m.group(1), 16)
                    if tlo <= v < thi:
                        h = v
            if h is not None:
                break
        if pid is None:
            continue
        result.append(
            {
                "register": f"{r:08X}",
                "site": f"{ins[k][0]:08X}",
                "dispatcher": disp,
                "id": pid,
                "id_hex": f"{pid:04X}",
                "handler": f"{h:08X}" if h else None,
                "strings": strings_in(h) if h else [],
            }
        )
result.sort(key=lambda e: e["id"])
json.dump(result, open(out, "w", encoding="utf-8"), indent=1, ensure_ascii=False)
print(len(result), "registrations")
