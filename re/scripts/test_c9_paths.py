import json
import os
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

from c9_paths import resolve_client_dir, resolve_client_file


class ClientPathTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name).resolve()
        (self.root / 'config').mkdir()
        self.env = patch.dict(os.environ, {}, clear=True)
        self.env.start()
        self.addCleanup(self.env.stop)

    def folder(self, name):
        path = self.root / name
        path.mkdir()
        (path / 'C9.exe').write_bytes(b'not-an-executable-test-fixture')
        return path

    def test_explicit_location_precedes_environment(self):
        explicit = self.folder('moved client')
        os.environ['C9_CLIENT_DIR'] = str(self.folder('other'))
        self.assertEqual(resolve_client_file(explicit=explicit, root=self.root), explicit / 'C9.exe')

    def test_environment_precedes_local_config(self):
        folder = self.folder('environment')
        (self.root / 'config/research.local.json').write_text(json.dumps({'client_directory': 'missing'}))
        os.environ['C9_CLIENT_DIR'] = str(folder)
        self.assertEqual(resolve_client_dir(root=self.root), folder)

    def test_local_config_and_relative_path(self):
        folder = self.folder('external')
        (self.root / 'config/research.local.json').write_text(json.dumps({'client_directory': 'external'}))
        self.assertEqual(resolve_client_dir(root=self.root), folder)

    def test_historical_layout_fallback(self):
        folder = self.folder('52C9GL')
        self.assertEqual(resolve_client_dir(root=self.root), folder)

    def test_utf8_local_config_with_non_ascii_path(self):
        folder = self.folder('\u0e40\u0e01\u0e21 C9')
        (self.root / 'config/research.local.json').write_text(
            json.dumps({'client_directory': folder.name}, ensure_ascii=False), encoding='utf-8')
        self.assertEqual(resolve_client_dir(root=self.root), folder)

    def test_missing_and_invalid_config_are_explicit(self):
        with self.assertRaisesRegex(ValueError, 'C9_CLIENT_DIR'):
            resolve_client_dir(root=self.root)
        config = self.root / 'config/research.local.json'
        for document in [{'client_directory': ''}, {'client_directory': 123}, {'other': 'x'}]:
            config.write_text(json.dumps(document))
            with self.assertRaises(ValueError):
                resolve_client_dir(root=self.root)

    def test_missing_files_and_traversal_are_rejected(self):
        folder = self.folder('client')
        for name in ['missing.exe', '../config/research.local.json']:
            with self.assertRaises(ValueError):
                resolve_client_file(name, folder, self.root)


if __name__ == '__main__':
    unittest.main()
