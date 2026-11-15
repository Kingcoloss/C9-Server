"""Read-only browser for the LouLx C9 VM disk (VMDK -> MBR/GPT -> NTFS).

Usage:
  python -I vmfs.py parts
  python -I vmfs.py ls <ntfs-path> [depth]
  python -I vmfs.py find <regex> [root]        (walk filenames)
  python -I vmfs.py get <ntfs-path> <out-file>  (copy one file out, read-only source)
All source files are opened 'rb' by dissect; nothing is written to the backup.
"""
import re
import sys
from pathlib import Path

from dissect.hypervisor.disk.vmdk import VMDK
from dissect.ntfs import NTFS
from dissect.volume.disk import Disk

DESC = Path(r"C:\MyProjects\New folder\C9-5-Class-LouLx\LouLx-Game\Server\BL20166.Com.vmdk")


SHORT = re.compile(r"^[^~ ]{1,6}~[0-9]+(\.[^.]{0,3})?$")
sys.stdout.reconfigure(encoding="utf-8", errors="replace")


def open_fs(index=None):
    vmdk = VMDK(DESC)
    disk = Disk(vmdk)
    parts = list(disk.partitions)
    fss = []
    for i, p in enumerate(parts):
        fh = p.open()
        fh.seek(3)
        if fh.read(4) == b"NTFS":
            fh.seek(0)
            fss.append((i, p, NTFS(fh)))
    if index is None:
        # pick largest NTFS partition
        return max(fss, key=lambda t: t[1].size)
    return [t for t in fss if t[0] == index][0]


def cmd_parts():
    vmdk = VMDK(DESC)
    print("disk size", vmdk.size)
    disk = Disk(vmdk)
    for i, p in enumerate(disk.partitions):
        fh = p.open()
        fh.seek(3)
        sig = fh.read(8)
        print(i, p.type, p.offset, p.size, sig)


def walk(entry, path, depth, maxdepth, out):
    try:
        items = {}
        for name, ie in entry.listdir().items():
            if name in (".", ".."):
                continue
            if SHORT.match(name):
                continue
            try:
                key = ie.mft_reference.segment_number if hasattr(ie, "mft_reference") else name
            except Exception:
                key = name
            if key not in items or len(name) > len(items[key][0]):
                items[key] = (name, ie)
        for name, ie in sorted(items.values(), key=lambda t: t[0].lower()):
            try:
                child = ie.dereference()
            except Exception as e:  # noqa
                print("  !deref", path, name, e)
                continue
            full = path + "\\" + name
            isdir = child.is_dir()
            try:
                size = 0 if isdir else child.size()
            except Exception:
                size = -1
            out(full, isdir, size, child)
            if isdir and depth < maxdepth:
                walk(child, full, depth + 1, maxdepth, out)
    except Exception as e:  # noqa
        print("  !err", path, e)


def get_entry(fs, p):
    p = p.strip("\\/")
    if not p:
        return fs.mft.root
    return fs.mft.get(p.replace("\\", "/"))


def main():
    cmd = sys.argv[1]
    if cmd == "parts":
        cmd_parts()
        return
    import os
    pi = os.environ.get("PART")
    _, _, fs = open_fs(int(pi) if pi else None)
    if cmd == "ls":
        path = sys.argv[2] if len(sys.argv) > 2 else ""
        depth = int(sys.argv[3]) if len(sys.argv) > 3 else 0
        e = get_entry(fs, path)

        def out(full, isdir, size, child):
            print(("D " if isdir else "F ") + f"{size:>12} " + full)

        walk(e, path, 0, depth, out)
    elif cmd == "find":
        rx = re.compile(sys.argv[2], re.I)
        root = sys.argv[3] if len(sys.argv) > 3 else ""
        maxd = int(sys.argv[4]) if len(sys.argv) > 4 else 12

        def out(full, isdir, size, child):
            if rx.search(full.rsplit("\\", 1)[-1]):
                print(("D " if isdir else "F ") + f"{size:>12} " + full)

        walk(get_entry(fs, root), root, 0, maxd, out)
    elif cmd == "getdir":
        # getdir <ntfs-dir> <out-dir> [name-regex] [maxdepth]
        src = sys.argv[2]
        dst = Path(sys.argv[3])
        rx = re.compile(sys.argv[4], re.I) if len(sys.argv) > 4 else None
        maxd = int(sys.argv[5]) if len(sys.argv) > 5 else 20
        n = [0, 0]

        def out(full, isdir, size, child):
            leaf = full.rsplit("\\", 1)[-1]
            if isdir:
                return
            if rx and not rx.search(leaf):
                return
            rel = full[len(src):].strip("\\/").replace("\\", "/")
            o = dst / rel
            o.parent.mkdir(parents=True, exist_ok=True)
            with child.open() as fi, o.open("wb") as fo:
                while True:
                    b = fi.read(1 << 22)
                    if not b:
                        break
                    fo.write(b)
            n[0] += 1
            n[1] += size

        walk(get_entry(fs, src), src, 0, maxd, out)
        print("copied files", n[0], "bytes", n[1])
    elif cmd == "get":
        e = get_entry(fs, sys.argv[2])
        dst = Path(sys.argv[3])
        dst.parent.mkdir(parents=True, exist_ok=True)
        with e.open() as src, dst.open("wb") as o:
            while True:
                b = src.read(1 << 20)
                if not b:
                    break
                o.write(b)
        print("copied", dst, dst.stat().st_size)


if __name__ == "__main__":
    main()
