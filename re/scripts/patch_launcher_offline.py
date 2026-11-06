#!/usr/bin/env python3
"""Apply or restore the offline version check for the installed C9Launcher build."""
import argparse
import configparser
import hashlib
import os
from pathlib import Path
import re
import tempfile

from c9_paths import PROJECT_ROOT, resolve_client_dir
from c9_static_inventory import PE

ORIGINAL_SHA256 = 'c3221361367f84721e92bf360d54be7490e3aa443ebce908ee6f3064218439bf'
NEWS_URL = 'https://c9ol.net/newspatch.php\0'.encode('utf-16le')
PATCHES = (
    # Drop the maintenance downloader's arguments and take its failure path.
    (0x00408965, bytes.fromhex('e8969e0000'), bytes.fromhex('83c40831c0')),
    # Go straight to the existing local file check, before any download or write.
    (0x0041a9a6, bytes.fromhex('7538'), bytes.fromhex('eb4f')),
    # If the local file exists, take the existing INI parser path in every mode.
    (0x0041aa02, bytes.fromhex('7554'), bytes.fromhex('eb54')),
    # Keep the embedded news panel local as well.
    (0x004769f8, NEWS_URL, 'about:blank\0'.encode('utf-16le').ljust(len(NEWS_URL), b'\0')),
)


def validate_manifest(client):
    version_bytes = (client / 'version.dat').read_bytes()
    version = int.from_bytes(version_bytes, 'little')
    if len(version_bytes) != 4 or not 0 < version <= 0x7fffffff:
        raise ValueError('version.dat must contain a valid 4-byte installed version')

    manifest = configparser.ConfigParser(interpolation=None)
    manifest.read_string((client / 'version.ini').read_text(encoding='ascii'))
    if (manifest.getint('VERSION', 'LatestVersion') != version
            or manifest.getint('VERSION', 'MinimalVersion') != version
            or manifest.getint('VERSION', 'OwnVersion') != 17
            or manifest.getint('VERSION', 'CoreVersion') != 0):
        raise ValueError('version.ini must describe this installed version without an update')
    downloads = manifest['DOWNLOAD']
    base = re.fullmatch(r'http://127\.0\.0\.1:([0-9]{1,5})/C9Update/EN/', downloads.get('server1', ''))
    if set(downloads) != {'server1'} or not base or not 1 <= int(base[1]) <= 65535:
        raise ValueError('version.ini must have a single loopback download base')
    if (manifest.getint('SERVER', 'COUNT') != 1
            or manifest.get('SERVER', 'IP1') != '127.0.0.1'
            or not 1 <= manifest.getint('SERVER', 'PORT1') <= 65535):
        raise ValueError('version.ini must advertise the local login server')


def replace_launcher(target, contents):
    descriptor, name = tempfile.mkstemp(prefix='.c9-launcher-', suffix='.tmp', dir=target.parent)
    temporary = Path(name)
    try:
        with os.fdopen(descriptor, 'wb') as stream:
            stream.write(contents)
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, target)
    finally:
        temporary.unlink(missing_ok=True)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--client-dir', type=Path)
    parser.add_argument('--backup-dir', type=Path,
                        default=PROJECT_ROOT / 'artifacts/client-backups/offline-launcher')
    action = parser.add_mutually_exclusive_group()
    action.add_argument('--apply', action='store_true')
    action.add_argument('--restore', action='store_true')
    args = parser.parse_args()
    try:
        client = resolve_client_dir(args.client_dir)
        backup_dir = args.backup_dir.resolve()
        if backup_dir.is_relative_to(client):
            raise ValueError('Keep the backup outside the Client folder')
        target = client / 'C9Launcher.exe'
        backup = backup_dir / 'C9Launcher.exe.original'
        current = target.read_bytes()
        source = target if hashlib.sha256(current).hexdigest() == ORIGINAL_SHA256 else backup
        image = PE(source)
        if hashlib.sha256(image.data).hexdigest() != ORIGINAL_SHA256:
            raise ValueError('Original launcher SHA-256 mismatch')
        if backup.exists() and backup.read_bytes() != image.data:
            raise ValueError('Existing backup differs from the verified original')

        patched = bytearray(image.data)
        for address, original, replacement in PATCHES:
            offset = image.off(address - image.base)
            end = offset + len(original)
            if len(replacement) != len(original) or image.data[offset:end] != original:
                raise ValueError(f'Unexpected launcher bytes at {address:#x}')
            patched[offset:end] = replacement
        if current not in (image.data, patched):
            raise ValueError('Launcher is neither the known original nor this exact patch')

        if args.apply:
            validate_manifest(client)
            if not backup.exists():
                backup_dir.mkdir(parents=True, exist_ok=True)
                with backup.open('xb') as stream:
                    stream.write(image.data)
                    stream.flush()
                    os.fsync(stream.fileno())
        desired = image.data if args.restore else patched
        if (args.apply or args.restore) and current != desired:
            if target.read_bytes() != current:
                raise ValueError('Launcher changed during validation')
            replace_launcher(target, desired)
            current = target.read_bytes()
            if current != desired:
                raise ValueError('Launcher bytes after replacement differ from the selected image')
        print(f'Launcher: {target}')
        print(f'State: {"offline" if current == patched else "original"}')
        print(f'SHA-256: {hashlib.sha256(current).hexdigest()}')
        print(f'Offline SHA-256: {hashlib.sha256(patched).hexdigest()}')
        print(f'Backup: {backup}')
    except (OSError, ValueError, KeyError, configparser.Error) as exc:
        parser.error(str(exc))


if __name__ == '__main__':
    main()
