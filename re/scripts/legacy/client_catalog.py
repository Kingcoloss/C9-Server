"""Scan a client image (file or unpacked memory dump) for packet catalog initializers.

Byte pattern seen in the 2018 C9.exe (MSVC):
    BF <hash32>        mov edi, hash
    B0 <enc>           mov al, enc     (or 32 C0 = xor al, al)
    B9 <id32>          mov ecx, id
    E8 <rel32>         call PacketInfo::Get/Set
Writes JSON list {id, id_hex, hash, encrypted, offset}.

python client_catalog.py <image> <out.json>
"""
import json
import re
import struct
import sys

data = open(sys.argv[1], "rb").read()
pat = re.compile(rb"\xBF(.{4})(?:\xB0(.)|\x32\xC0)\xB9(.{2})\x00\x00\xE8", re.S)
res = {}
for m in pat.finditer(data):
    h = struct.unpack("<I", m.group(1))[0]
    enc = m.group(2)[0] if m.group(2) is not None else 0
    pid = struct.unpack("<H", m.group(3))[0]
    if pid == 0:
        continue
    e = res.setdefault(pid, {"id": pid, "id_hex": f"{pid:04X}", "hash": f"{h:08X}", "encrypted": enc, "offset": m.start()})
    if e["hash"] != f"{h:08X}":
        e.setdefault("other_hashes", []).append(f"{h:08X}")
out = sorted(res.values(), key=lambda e: e["id"])
json.dump(out, open(sys.argv[2], "w"), indent=1)
print(len(out), "packet types")
