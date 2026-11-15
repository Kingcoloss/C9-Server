"""Recover static packet-type metadata initializers (static only).

Pattern in C9 server binaries (MSVC 2008):
    push <hash32>
    push <packet id>
    call <PacketInfo::Get>
    mov ecx, eax
    mov al, <0|1>             ; encrypted flag
    call <PacketInfo::SetEncrypted>
The Get function is auto-detected as the call target most often preceded by
two immediate pushes where the second push is a 16-bit id.

python pktmeta.py <pkl> <out.json>
"""
import collections
import json
import pickle
import re
import sys

d = pickle.load(open(sys.argv[1], "rb"))
ins = d["ins"]
IMM = re.compile(r"^0x([0-9a-f]+)$|^(\d+)$")


def imm(op):
    m = IMM.match(op.strip())
    if not m:
        return None
    return int(m.group(1), 16) if m.group(1) else int(m.group(2))


cand = collections.Counter()
for k in range(2, len(ins)):
    i = ins[k]
    if i[2] != "call":
        continue
    a, b = ins[k - 2], ins[k - 1]
    if a[2] == "push" and b[2] == "push":
        h, pid = imm(a[3]), imm(b[3])
        if h is not None and pid is not None and h > 0xFFFF and 0 < pid <= 0xFFFF:
            t = imm(i[3])
            if t:
                cand[t] += 1
get_fn, n = cand.most_common(1)[0] if cand else (None, 0)
print("PacketInfo::Get candidate", hex(get_fn) if get_fn else None, n)
res = {}
for k in range(2, len(ins) - 3):
    i = ins[k]
    if i[2] != "call" or imm(i[3]) != get_fn:
        continue
    a, b = ins[k - 2], ins[k - 1]
    if not (a[2] == "push" and b[2] == "push"):
        continue
    h, pid = imm(a[3]), imm(b[3])
    if h is None or pid is None:
        continue
    enc = None
    for j in range(k + 1, k + 5):
        x = ins[j]
        if x[2] == "mov" and x[3].startswith("al, "):
            enc = imm(x[3][4:])
            break
    e = res.setdefault(pid, {"id": pid, "id_hex": f"{pid:04X}", "hash": f"{h:08X}", "encrypted": enc, "sites": []})
    if e["hash"] != f"{h:08X}":
        e.setdefault("other_hashes", []).append(f"{h:08X}")
    if enc is not None and e["encrypted"] is None:
        e["encrypted"] = enc
    e["sites"].append(f"{i[0]:08X}")
out = sorted(res.values(), key=lambda e: e["id"])
for e in out:
    e["sites"] = e["sites"][:3]
json.dump(out, open(sys.argv[2], "w"), indent=1)
print(len(out), "packet types")
