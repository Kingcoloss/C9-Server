"""Query helper over the pickled disassembly.

python dq.py <pkl> xref <hexaddr>...      instructions referencing the address
python dq.py <pkl> dis <hexaddr> [n]      disassemble n instructions from address
python dq.py <pkl> fn <hexaddr> [n]       disassemble from guessed function start containing addr
"""
import bisect
import pickle
import sys

d = pickle.load(open(sys.argv[1], "rb"))
ins = d["ins"]
addrs = [i[0] for i in ins]


def idx(a):
    return bisect.bisect_left(addrs, a)


def fmt(i):
    return f"{i[0]:08X}  {i[2]} {i[3]}"


def fn_start(a):
    k = idx(a)
    while k > 0:
        p = ins[k - 1]
        if p[2] in ("int3", "ret", "retn") or (p[2] == "jmp" and False):
            return ins[k][0]
        k -= 1
    return ins[0][0]


cmd = sys.argv[2]
if cmd == "xref":
    targets = [f"0x{int(x, 16):x}" for x in sys.argv[3:]]
    for i in ins:
        for t in targets:
            if t in i[3]:
                print(fmt(i), "   ; fn~", f"{fn_start(i[0]):08X}")
elif cmd == "dis":
    a = int(sys.argv[3], 16)
    n = int(sys.argv[4]) if len(sys.argv) > 4 else 40
    k = idx(a)
    for i in ins[k : k + n]:
        print(fmt(i))
elif cmd == "fn":
    a = int(sys.argv[3], 16)
    n = int(sys.argv[4]) if len(sys.argv) > 4 else 80
    s = fn_start(a)
    k = idx(s)
    for i in ins[k : k + n]:
        print(fmt(i))
