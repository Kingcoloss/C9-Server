#!/usr/bin/env python3
"""Install our independent local UI beside a verified official Client.

Official updater/game files and user-copied libraries remain untouched. Only
matching files recorded by a previous installation may be replaced or retired.
Client binaries, local paths and installation records stay outside Git.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import tempfile

from c9_paths import PROJECT_ROOT

OFFICIAL_52893_SHA256 = '704687c39a293db92999c8a1ef5f22d9e147d743a94d3decb3de0a35af69dd63'
OFFICIAL_FILES = ['C9.exe', 'C9Launcher.exe', 'C9Patcher.exe', 'C9Server.ini', 'VerCli.C9']
UI_FILES = {'C9LocalLauncher.exe': 'L9.exe'}
# These obsolete files came only from our earlier legacy-UI prototype.
RETIRABLE = {'L9.exe', 'version.dat', 'version.ini', 'localinfo.dat',
             'L9.dll', 'L9.deps.json', 'L9.runtimeconfig.json', 'C9.ClientProfiles.dll'}


def sha(data):
    return hashlib.sha256(data).hexdigest()


def atomic_write(path, contents):
    descriptor, name = tempfile.mkstemp(prefix='.c9-local-', suffix='.tmp', dir=path.parent)
    temporary = Path(name)
    try:
        with os.fdopen(descriptor, 'wb') as stream:
            stream.write(contents)
            stream.flush()
            os.fsync(stream.fileno())
        os.replace(temporary, path)
    finally:
        temporary.unlink(missing_ok=True)


def child(client, name):
    path = client / name
    if Path(name).name != name or path.resolve().parent != client or path.is_symlink():
        raise ValueError('Installation target must be a regular file directly inside the Client folder')
    return path


def prepare(ui_directory, backend, settings, dotnet, port):
    if not 1 <= port <= 65535:
        raise ValueError('Invalid local server port')
    backend, settings, dotnet = [path.resolve(strict=True) for path in (backend, settings, dotnet)]
    if backend.name != 'C9.Server.dll' or dotnet.name.lower() != 'dotnet.exe':
        raise ValueError('Select the published server assembly and installed dotnet.exe')
    config = json.loads(settings.read_text(encoding='utf-8-sig'))
    if config.get('host') != '127.0.0.1' or config.get('port') != port or config.get('profile') != 'observer':
        raise ValueError('New Client currently requires the matching loopback observer configuration')
    files = {target: (ui_directory / source).read_bytes() for target, source in UI_FILES.items()}
    files['L9.local.json'] = (json.dumps({'host': '127.0.0.1', 'port': port,
        'backend_assembly': str(backend), 'backend_config': str(settings), 'dotnet_path': str(dotnet)},
        indent=2) + '\n').encode('utf-8')
    return files


def install(client, ui_directory, backend, settings, dotnet, record_directory, port, apply):
    client = client.resolve(strict=True)
    record_directory = record_directory.resolve()
    if record_directory.is_relative_to(client):
        raise ValueError('Keep installation records outside the Client folder')
    official = {name: sha((client / name).read_bytes()) for name in OFFICIAL_FILES}
    if official['C9.exe'] != OFFICIAL_52893_SHA256:
        raise ValueError('Updated game build requires a verified launch-argument profile')
    files = prepare(ui_directory, backend, settings, dotnet, port)
    record_file = record_directory / 'install.local.json'
    previous = json.loads(record_file.read_text(encoding='utf-8')) if record_file.exists() else None
    if previous is not None and previous['client_directory'] != str(client):
        raise ValueError('Installation record belongs to a different Client folder')
    owned = previous['owned_files'] if previous else {}
    for name in files:
        path = child(client, name)
        if path.exists() and sha(path.read_bytes()) != owned.get(name):
            raise ValueError(f'Existing {name} is not owned by this installation; leaving it untouched')
    retired, preserved = [], []
    for name in RETIRABLE & owned.keys():
        path = child(client, name)
        if path.exists():
            (retired if sha(path.read_bytes()) == owned[name] else preserved).append(name)
    if apply:
        record_directory.mkdir(parents=True, exist_ok=True)
        # Retain the exact prior local installation before replacing it.
        if previous:
            snapshot = record_directory / ('ui-migration-' + sha(record_file.read_bytes())[:16])
            snapshot.mkdir(exist_ok=True)
            for name in owned:
                path = child(client, name)
                if path.is_file() and sha(path.read_bytes()) == owned[name]:
                    atomic_write(snapshot / name, path.read_bytes())
        for name, data in files.items():
            path = child(client, name)
            if path.exists() and sha(path.read_bytes()) != owned.get(name):
                raise ValueError(f'{name} changed during installation')
            atomic_write(path, data)
        for name in retired:
            path = child(client, name)
            if sha(path.read_bytes()) != owned[name]:
                raise ValueError(f'{name} changed during installation')
            path.unlink()  # Single verified owned file, never a recursive removal.
        after = {name: sha((client / name).read_bytes()) for name in OFFICIAL_FILES}
        if official != after:
            raise ValueError('Official files changed concurrently; local launch remains profile-checked')
        for name, data in files.items():
            if child(client, name).read_bytes() != data:
                raise ValueError('Local installation verification failed')
        record = {'schema_version': 2, 'client_directory': str(client),
                  'official_files_unchanged': official,
                  'owned_files': {name: sha(data) for name, data in files.items()},
                  'synthetic_account_only': True, 'ui': 'independent_winforms',
                  'retired_files': sorted(retired), 'preserved_changed_files': sorted(preserved)}
        atomic_write(record_file, (json.dumps(record, indent=2) + '\n').encode('utf-8'))
    return {'state': 'installed' if apply else 'ready', 'client_directory': str(client),
            'local_launcher': str(client / 'C9LocalLauncher.exe'),
            'official_files_unchanged': True, 'local_files': list(files),
            'retired_owned_files': sorted(retired), 'preserved_changed_files': sorted(preserved),
            'record': str(record_file)}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--client-dir', type=Path, required=True)
    parser.add_argument('--ui-dir', type=Path, default=PROJECT_ROOT / 'artifacts/local-launcher-standalone')
    parser.add_argument('--backend', type=Path, required=True)
    parser.add_argument('--backend-config', type=Path, default=PROJECT_ROOT / 'config/lab-official-observer.json')
    parser.add_argument('--dotnet', type=Path, required=True)
    parser.add_argument('--port', type=int, default=14000)
    parser.add_argument('--apply', action='store_true')
    args = parser.parse_args()
    identity = sha(str(args.client_dir.resolve()).casefold().encode('utf-8'))[:16]
    record = PROJECT_ROOT / 'artifacts/client-backups' / ('dual-launcher-' + identity)
    try:
        print(json.dumps(install(args.client_dir, args.ui_dir, args.backend, args.backend_config,
                                 args.dotnet, record, args.port, args.apply)))
    except (OSError, ValueError, KeyError) as error:
        parser.error(str(error))


if __name__ == '__main__':
    main()
