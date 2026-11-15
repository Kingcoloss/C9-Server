"""Profile packet handlers of a C9 server binary (static only, heuristic).

For each registered handler collect, over its call closure (depth N):
  - strings referenced (UTF-16/ASCII)
  - catalog packet ids written as immediates (candidate replies / forwards)
  - RTTI class names of vtables stored (objects/queries created)
  - stored-procedure call strings
python handler_profile.py <pe> <pkl> <handlers.json> <meta.json> <out.json> [depth]
"""
import bisect
import json
import pickle
import re
import struct
import sys

import pefile

pe_path, pkl, hjson, mjson, out = sys.argv[1:6]
depth = int(sys.argv[6]) if len(sys.argv) > 6 else 2
pe = pefile.PE(pe_path, fast_load=True)
base = pe.OPTIONAL_HEADER.ImageBase
data = open(pe_path, "rb").read()
d = pickle.load(open(pkl, "rb"))
ins = d["ins"]
addrs = [i[0] for i in ins]
text = [s for s in pe.sections if s.Name.startswith(b".text")][0]
tlo, thi = base + text.VirtualAddress, base + text.VirtualAddress + text.Misc_VirtualSize
meta = {e["id"]: e for e in json.load(open(mjson))}
handlers = json.load(open(hjson, encoding="utf-8"))


def va2off(va):
    for s in pe.sections:
        lo = base + s.VirtualAddress
        if lo <= va < lo + s.SizeOfRawData:
            return s.PointerToRawData + (va - lo)
    return None


def rd(va, n):
    o = va2off(va)
    return data[o : o + n] if o is not None else None


def read_str(va):
    b = rd(va, 600)
    if not b:
        return None
    m = re.match(rb"((?:[\x20-\x7e]\x00){4,})", b)
    if m:
        return m.group(1).decode("utf-16le")
    m = re.match(rb"([\x20-\x7e]{4,})", b)
    if m:
        return m.group(1).decode()
    return None


def rtti_of_vtable(va):
    b = rd(va - 4, 4)
    if not b:
        return None
    col = struct.unpack("<I", b)[0]
    c = rd(col, 16)
    if not c:
        return None
    sig, off, cdoff, td = struct.unpack("<IIII", c)
    if sig != 0:
        return None
    n = rd(td + 8, 200)
    if not n or not n.startswith(b".?AV") and not n.startswith(b".?AU"):
        return None
    return n.split(b"\0")[0].decode("ascii", "replace")


fn_cache = {}


def fn_body(a):
    if a in fn_cache:
        return fn_cache[a]
    k = bisect.bisect_left(addrs, a)
    j = k
    while j < len(ins) and j - k < 6000:
        if ins[j][2] == "int3" and j > k:
            break
        j += 1
    fn_cache[a] = (k, j)
    return k, j


HEX = re.compile(r"0x([0-9a-f]+)")


def profile(a):
    seen = set()
    frontier = [a]
    strings, ids, classes, procs = [], [], [], []
    for lvl in range(depth + 1):
        nxt = []
        for f in frontier:
            if f in seen:
                continue
            seen.add(f)
            k, j = fn_body(f)
            for i in ins[k:j]:
                mn, op = i[2], i[3]
                if mn == "call":
                    m = re.match(r"^0x([0-9a-f]+)$", op)
                    if m:
                        t = int(m.group(1), 16)
                        if tlo <= t < thi:
                            nxt.append(t)
                    continue
                for m in HEX.finditer(op):
                    v = int(m.group(1), 16)
                    if tlo <= v < thi:
                        continue
                    if v > 0x400000:
                        s = read_str(v)
                        if s:
                            if "CALL " in s:
                                if s not in procs:
                                    procs.append(s)
                            elif s not in strings and len(strings) < 25:
                                strings.append(s)
                            continue
                        r = rtti_of_vtable(v)
                        if r and r not in classes:
                            classes.append(r)
                    elif 0 < v <= 0xFFFF and v in meta and mn in ("mov", "push") and lvl <= 1:
                        if v not in ids:
                            ids.append(v)
        frontier = nxt
    return strings, ids, classes, procs


res = []
for h in handlers:
    if not h["handler"]:
        continue
    a = int(h["handler"], 16)
    s, ids, cl, pr = profile(a)
    res.append(
        {
            "id": h["id_hex"],
            "handler": h["handler"],
            "hash": meta.get(h["id"], {}).get("hash"),
            "encrypted": meta.get(h["id"], {}).get("encrypted") or 0,
            "ids_used": [f"{x:04X}" for x in ids if x != h["id"]][:12],
            "classes": [c for c in cl if "PSQuery" in c or "Bind" not in c][:12],
            "procs": pr[:8],
            "strings": s[:12],
        }
    )
json.dump(res, open(out, "w", encoding="utf-8"), indent=1, ensure_ascii=False)
print(len(res), "profiles")
