# Legacy server reference scripts (read-only, static)

Used to build `docs/LEGACY_PATCH65_REFERENCE_TH.md` from the LouLx C9 backup (server build 2014-11-20).
None of these scripts execute code from the backup. They open source files read-only.

Setup (separate venv, outside the backup folder):

    python -m venv <venv>
    <venv>/Scripts/python -m pip install dissect.hypervisor dissect.ntfs dissect.volume pefile capstone

Run every script with `python -I` and pass paths as arguments.

| Script | Purpose |
|---|---|
| `vmfs.py` | Browse/copy files from the split VMDK -> NTFS (`parts`, `ls`, `find`, `get`, `getdir`; `PART=<n>` env selects the partition). The descriptor path is set at the top of the file. |
| `pestrings.py` | Dump ASCII/UTF-16 strings with VA |
| `disasm_index.py` | Linear-sweep disassembly of `.text` to a pickle |
| `dq.py` | Query the pickle (`xref`, `dis`, `fn`) |
| `pktmeta.py` | Recover the static packet catalog (id, typeHash, encrypted flag) |
| `handlers.py` | Recover packet-id -> handler registrations |
| `handler_profile.py` | Per-handler strings / packet ids used / RTTI classes (heuristic) |
| `callers.py` | Reverse call graph from a function to registered handlers |
| `client_catalog.py` | Scan a client image or unpacked memory dump for the catalog byte pattern |
| `cfs_list.py` | List CFS archive entries with `re/scripts/c9_cfs.py` |

Outputs that are facts (ids, hashes, procedure names) live in `re/evidence/legacy_patch65/`.
Binaries, databases and game data from the backup must stay outside git.
