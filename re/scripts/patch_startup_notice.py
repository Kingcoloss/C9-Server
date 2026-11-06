#!/usr/bin/env python3
"""Apply/restore the hash-locked research fix for a promotional DllMain MessageBox.

Only the first argument-push at preferred VA 1000269f is changed to a short jump.
It skips all four arguments and the MessageBoxW call without touching relocations.
Runtime evidence: return address PhysXLoader+26b3 below USER32/UIAutomation waits.
"""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import tempfile

from c9_paths import PROJECT_ROOT, resolve_client_dir
from c9_static_inventory import PE

ORIGINAL_SHA256 = '6f491b6073a6fd759f3eb5b83e010fa66c5e0e9075585d263bd63d1f622cec3f'
PATCH_VA = 0x1000269f
ORIGINAL = bytes.fromhex('6a30')
REPLACEMENT = bytes.fromhex('eb12')  # jmp 100026b3, after arguments and MessageBoxW.
EXPECTED_BLOCK = bytes.fromhex('6a30681c81171068288117106a00ff151c681410')


def digest(data):
    return hashlib.sha256(data).hexdigest()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--client-dir', type=Path)
    parser.add_argument('--backup-dir', type=Path, default=PROJECT_ROOT / 'artifacts/client-backups/startup-notice')
    action = parser.add_mutually_exclusive_group()
    action.add_argument('--apply', action='store_true')
    action.add_argument('--restore', action='store_true')
    args = parser.parse_args()
    client = resolve_client_dir(args.client_dir)
    backup_dir = args.backup_dir.resolve()
    if backup_dir.is_relative_to(client):
        parser.error('The original DLL backup must be outside the Client directory')
    target = client / 'PhysXLoader.dll'
    backup = backup_dir / 'PhysXLoader.dll.original'
    image = PE(target)
    offset = image.off(PATCH_VA - image.base)
    current = target.read_bytes()
    current_hash = digest(current)
    if current_hash == ORIGINAL_SHA256:
        original = current
    else:
        if not backup.is_file():
            parser.error('Unrecognized DLL and no verified original backup; no files changed')
        original = backup.read_bytes()
        if digest(original) != ORIGINAL_SHA256:
            parser.error('Original backup hash mismatch; no files changed')
    if original[offset:offset+len(ORIGINAL)] != ORIGINAL:
        parser.error('Original call bytes do not match; no files changed')
    if original[offset:offset+20] != EXPECTED_BLOCK:
        parser.error('Unexpected argument/call block; no files changed')
    if original[offset+20:offset+23] != bytes.fromhex('8b4dfc'):
        parser.error('Unexpected code after the call; no files changed')
    patched = original[:offset] + REPLACEMENT + original[offset+len(ORIGINAL):]
    patched_hash = digest(patched)
    # Recognize and replace only our earlier, relocation-overlapping test patch.
    legacy = original[:offset+14] + bytes.fromhex('83c41033c090') + original[offset+20:]
    if current_hash not in (ORIGINAL_SHA256, patched_hash, digest(legacy)):
        parser.error('DLL is neither the known original nor this exact patch; no files changed')
    desired = original if args.restore else patched
    mutated = False
    if (args.apply or args.restore) and current != desired:
        backup_dir.mkdir(parents=True, exist_ok=True)
        if backup.exists():
            if digest(backup.read_bytes()) != ORIGINAL_SHA256:
                parser.error('Existing backup differs; no files changed')
        else:
            backup.write_bytes(original)
        temporary = None
        try:
            descriptor, name = tempfile.mkstemp(prefix='.c9-startup-', suffix='.tmp', dir=client)
            temporary = Path(name)
            with os.fdopen(descriptor, 'wb') as stream:
                stream.write(desired)
                stream.flush()
                os.fsync(stream.fileno())
            # Keep the new write time so Windows ARM's translated-image cache
            # does not mistake this image for the previously loaded original.
            os.replace(temporary, target)
            temporary = None
        finally:
            if temporary is not None and temporary.exists():
                temporary.unlink()
        mutated = True
    result = {
        'recorded_at_utc': datetime.now(timezone.utc).isoformat(),
        'target': str(target), 'action': 'restore' if args.restore else 'apply' if args.apply else 'check',
        'changed': mutated, 'original_sha256': ORIGINAL_SHA256, 'patched_sha256': patched_hash,
        'current_sha256': digest(target.read_bytes()), 'preferred_patch_va': hex(PATCH_VA),
        'skipped_call_va': '0x100026ad',
        'file_offset': hex(offset), 'original_bytes': ORIGINAL.hex(), 'replacement_bytes': REPLACEMENT.hex(),
        'backup': str(backup), 'scope': 'Promotional MessageBoxW only; protocol and other wrapper calls unchanged'
    }
    if args.apply or args.restore:
        (backup_dir / 'patch-record.json').write_text(json.dumps(result, indent=2) + '\n', encoding='utf-8')
    print(json.dumps(result, indent=2))


if __name__ == '__main__':
    main()
