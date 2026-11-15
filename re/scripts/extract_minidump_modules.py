"""Extract loaded module images from a Windows minidump (e.g. Task Manager C9.DMP).

Only the address ranges of the selected modules are written. Heap, stack and
other process memory stay inside the dump, which keeps credentials that may be
resident elsewhere out of the extracted files (ISO/IEC 27001 data minimisation).

Output per module: <name>.img (flat image mapped at its base, missing pages are
zero) and a modules.json manifest with base, size, captured ranges and the
memory protections recorded by MemoryInfoListStream.
"""
import argparse
import hashlib
import json
import struct
import sys
from pathlib import Path

MODULE_LIST = 4
MEMORY_LIST = 5
MEMORY64_LIST = 9
MEMORY_INFO_LIST = 16


def read_streams(f):
    header = f.read(32)
    sig, version, count, directory_rva = struct.unpack_from("<4sIII", header)
    if sig != b"MDMP":
        raise SystemExit("not a minidump")
    f.seek(directory_rva)
    streams = {}
    for _ in range(count):
        stype, size, rva = struct.unpack("<III", f.read(12))
        streams.setdefault(stype, []).append((size, rva))
    return streams


def read_utf16(f, rva):
    f.seek(rva)
    (length,) = struct.unpack("<I", f.read(4))
    return f.read(length).decode("utf-16-le", "replace")


def modules(f, streams):
    result = []
    for size, rva in streams.get(MODULE_LIST, []):
        f.seek(rva)
        (count,) = struct.unpack("<I", f.read(4))
        raw = f.read(count * 108)
        for i in range(count):
            base, image_size, checksum, stamp, name_rva = struct.unpack_from("<QIIII", raw, i * 108)
            result.append({"base": base, "size": image_size, "timestamp": stamp, "name_rva": name_rva})
    for m in result:
        m["path"] = read_utf16(f, m.pop("name_rva"))
        m["name"] = m["path"].replace("\\", "/").rsplit("/", 1)[-1]
    return result


def memory_ranges(f, streams):
    ranges = []  # (start, size, file_offset)
    for size, rva in streams.get(MEMORY64_LIST, []):
        f.seek(rva)
        count, base_rva = struct.unpack("<QQ", f.read(16))
        offset = base_rva
        for _ in range(count):
            start, data_size = struct.unpack("<QQ", f.read(16))
            ranges.append((start, data_size, offset))
            offset += data_size
    for size, rva in streams.get(MEMORY_LIST, []):
        f.seek(rva)
        (count,) = struct.unpack("<I", f.read(4))
        for _ in range(count):
            start, data_size, data_rva = struct.unpack("<QII", f.read(16))
            ranges.append((start, data_size, data_rva))
    return sorted(ranges)


def memory_info(f, streams):
    infos = []
    for size, rva in streams.get(MEMORY_INFO_LIST, []):
        f.seek(rva)
        header_size, entry_size, count = struct.unpack("<IIQ", f.read(16))
        f.seek(rva + header_size)
        for _ in range(count):
            raw = f.read(entry_size)
            base, alloc_base, alloc_protect, _a, region, state, protect, mtype = struct.unpack_from("<QQIIQIII", raw)
            infos.append({"base": base, "size": region, "state": state, "protect": protect, "type": mtype})
    return infos


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("dump", type=Path)
    parser.add_argument("out", type=Path)
    parser.add_argument("--module", action="append", default=[],
                        help="module file name to extract (repeatable, case-insensitive); default C9.exe")
    parser.add_argument("--list", action="store_true", help="only list modules")
    args = parser.parse_args()
    wanted = {m.lower() for m in (args.module or ["C9.exe"])}

    with args.dump.open("rb") as f:
        streams = read_streams(f)
        mods = modules(f, streams)
        if args.list:
            for m in mods:
                print(f"{m['base']:#010x} {m['size']:#010x} {m['path']}")
            return
        ranges = memory_ranges(f, streams)
        infos = memory_info(f, streams)
        args.out.mkdir(parents=True, exist_ok=True)
        manifest = {"dump": args.dump.name, "streams": sorted(streams), "modules": []}
        for m in mods:
            if m["name"].lower() not in wanted:
                continue
            base, end = m["base"], m["base"] + m["size"]
            image = bytearray(m["size"])
            captured = []
            for start, size, offset in ranges:
                lo, hi = max(start, base), min(start + size, end)
                if lo >= hi:
                    continue
                f.seek(offset + (lo - start))
                image[lo - base:hi - base] = f.read(hi - lo)
                captured.append([lo, hi - lo])
            target = args.out / (m["name"] + ".img")
            target.write_bytes(image)
            manifest["modules"].append({
                "name": m["name"], "path": m["path"], "base": base, "size": m["size"],
                "timestamp": m["timestamp"], "captured": captured,
                "captured_bytes": sum(c[1] for c in captured),
                "sha256": hashlib.sha256(image).hexdigest(),
                "protections": [i for i in infos if i["base"] < end and i["base"] + i["size"] > base],
            })
            print(f"{m['name']}: base {base:#x} size {m['size']:#x} captured {manifest['modules'][-1]['captured_bytes']:#x}")
        manifest["all_modules"] = [{"name": m["name"], "base": m["base"], "size": m["size"]} for m in mods]
        (args.out / "modules.json").write_text(json.dumps(manifest, indent=1))
        if not manifest["modules"]:
            sys.exit("no selected module found; use --list")


if __name__ == "__main__":
    main()
