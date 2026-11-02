"""Resolve the read-only Client location independently of the server runtime."""
import json
import os
from pathlib import Path

PROJECT_ROOT = Path(__file__).resolve().parents[2]


def resolve_client_dir(explicit=None, root=None):
    root = Path(root) if root is not None else PROJECT_ROOT
    value = explicit or os.environ.get('C9_CLIENT_DIR')
    if not value:
        config = root / 'config/research.local.json'
        if config.exists():
            document = json.loads(config.read_text())
            if not isinstance(document, dict) or set(document) != {'client_directory'}:
                raise ValueError('research.local.json must contain only client_directory')
            value = document['client_directory']
            if not isinstance(value, str) or not value.strip():
                raise ValueError('client_directory must be a nonempty path string')
        elif (root / '52C9GL').is_dir():
            value = root / '52C9GL'
    if not value:
        raise ValueError('Client folder is not configured: set C9_CLIENT_DIR, pass --client-dir, '
                         'or create config/research.local.json from config/research.example.json')
    path = Path(value).expanduser()
    if not path.is_absolute():
        path = root / path
    path = path.resolve()
    if not path.is_dir():
        raise ValueError(f'Configured Client folder does not exist: {path}')
    return path


def resolve_client_file(name='C9.exe', explicit=None, root=None):
    folder = resolve_client_dir(explicit, root)
    path = (folder / name).resolve()
    if not path.is_relative_to(folder) or not path.is_file():
        raise ValueError(f'Client file is missing or outside the configured folder: {name}')
    return path
