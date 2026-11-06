#!/usr/bin/env python3
"""Compare a relocated Client to the static baseline, reading bytes only."""
import argparse
import hashlib
import json
from datetime import datetime, timezone
from pathlib import Path

from c9_paths import PROJECT_ROOT, resolve_client_dir


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--client-dir', type=Path)
    parser.add_argument('--output', type=Path, default=PROJECT_ROOT / 're/evidence/static/client_integrity_latest.json')
    args = parser.parse_args()
    try:
        client = resolve_client_dir(args.client_dir)
    except ValueError as error:
        parser.error(str(error))
    if args.output.resolve().is_relative_to(client):
        parser.error('Integrity reports must be written outside the Client folder')
    baseline = json.loads((PROJECT_ROOT / 're/evidence/static/phase2_client_manifest.json').read_text(encoding='utf-8'))
    rows = []
    for item in baseline['files']:
        path = (client / item['file']).resolve()
        if not path.is_relative_to(client):
            raise ValueError('Baseline contains a path outside the Client folder')
        digest = hashlib.sha256(path.read_bytes()).hexdigest() if path.is_file() else None
        rows.append({'file': item['file'], 'sha256': digest, 'unchanged': digest == item['sha256']})
    output = {'recorded_at_utc': datetime.now(timezone.utc).isoformat(), 'client_directory': str(client),
              'scope': baseline['scope'], 'checked': len(rows), 'files': rows}
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(output, indent=2) + '\n', encoding='utf-8')
    passed = sum(r['unchanged'] for r in rows)
    print(json.dumps({'unchanged': passed, 'checked': len(rows)}))
    return 0 if passed == len(rows) else 1


if __name__ == '__main__':
    raise SystemExit(main())
