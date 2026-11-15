"""Dump ASCII and UTF-16LE strings with their VA from a PE (static read only)."""
import re
import sys

import pefile

path, out = sys.argv[1], sys.argv[2]
minlen = int(sys.argv[3]) if len(sys.argv) > 3 else 5
pe = pefile.PE(path, fast_load=True)
data = open(path, "rb").read()
base = pe.OPTIONAL_HEADER.ImageBase


def off2va(off):
    for s in pe.sections:
        if s.PointerToRawData <= off < s.PointerToRawData + s.SizeOfRawData:
            return base + s.VirtualAddress + (off - s.PointerToRawData), s.Name.rstrip(b"\0").decode()
    return None, "?"


with open(out, "w", encoding="utf-8") as o:
    for m in re.finditer(rb"[\x20-\x7e]{%d,}" % minlen, data):
        va, sec = off2va(m.start())
        if va:
            o.write(f"A {va:08X} {sec} {m.group().decode('ascii')}\n")
    for m in re.finditer(rb"(?:[\x20-\x7e]\x00){%d,}" % minlen, data):
        va, sec = off2va(m.start())
        if va:
            o.write(f"W {va:08X} {sec} {m.group().decode('utf-16le')}\n")
print("done")
