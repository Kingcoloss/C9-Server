"""Reverse call-graph search from a target function up to registered handlers.

python callers.py <pkl> <handlers.json> <target_hex> [maxdepth]
Function starts are approximated as the first instruction after an int3 run.
Also follows vtable-style references (mov/push of the address) as edges.
"""
import bisect
import collections
import json
import pickle
import re
import sys

d = pickle.load(open(sys.argv[1], "rb"))
ins = d["ins"]
addrs = [i[0] for i in ins]
handlers = {int(h["handler"], 16): h["id_hex"] for h in json.load(open(sys.argv[2], encoding="utf-8")) if h["handler"]}
target = int(sys.argv[3], 16)
maxd = int(sys.argv[4]) if len(sys.argv) > 4 else 6

starts = []
prev_pad = True
for i in ins:
    if i[2] == "int3":
        prev_pad = True
        continue
    if prev_pad:
        starts.append(i[0])
    prev_pad = False


def fstart(a):
    k = bisect.bisect_right(starts, a) - 1
    return starts[k] if k >= 0 else None


refs = collections.defaultdict(set)
H = re.compile(r"0x([0-9a-f]{5,8})")
for i in ins:
    if i[2] in ("call", "jmp", "mov", "push", "lea"):
        for m in H.finditer(i[3]):
            v = int(m.group(1), 16)
            refs[v].add(fstart(i[0]))

seen = {target: None}
q = collections.deque([(target, 0)])
found = []
while q:
    f, dpt = q.popleft()
    if f in handlers and f != target:
        found.append(f)
    if dpt >= maxd:
        continue
    for c in refs.get(f, ()):
        if c is not None and c not in seen:
            seen[c] = f
            q.append((c, dpt + 1))
for f in found:
    chain = [f]
    while seen[chain[-1]] is not None:
        chain.append(seen[chain[-1]])
    print(handlers[f], " <- ".join(f"{x:08X}" for x in chain))
print("functions reached", len(seen))
