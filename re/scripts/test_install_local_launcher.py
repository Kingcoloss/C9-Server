"""Exercise installer ownership boundaries without any real Client binaries."""
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

import install_local_launcher as installer


class InstallationOwnership(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        self.root = Path(self.temporary.name)
        self.client = self.root / 'client'
        self.ui = self.root / 'ui'
        self.records = self.root / 'records'
        self.client.mkdir()
        self.ui.mkdir()
        for name in installer.OFFICIAL_FILES:
            (self.client / name).write_bytes(('fixture-' + name).encode())
        self.official = {name: (self.client / name).read_bytes() for name in installer.OFFICIAL_FILES}
        self.profile = patch.object(installer, 'OFFICIAL_52893_SHA256', installer.sha(self.official['C9.exe']))
        self.profile.start()
        self.addCleanup(self.profile.stop)
        (self.ui / 'L9.exe').write_bytes(b'synthetic-ui')
        self.backend = self.root / 'C9.Server.dll'
        self.dotnet = self.root / 'dotnet.exe'
        self.settings = self.root / 'observer.json'
        self.backend.write_bytes(b'synthetic-backend')
        self.dotnet.write_bytes(b'synthetic-host')
        self.settings.write_text(json.dumps({'host': '127.0.0.1', 'port': 14000, 'profile': 'observer'}))

    def install(self, apply=True):
        return installer.install(self.client, self.ui, self.backend, self.settings,
                                 self.dotnet, self.records, 14000, apply)

    def assert_official_unchanged(self):
        for name, data in self.official.items():
            self.assertEqual(data, (self.client / name).read_bytes())

    def test_preview_and_reinstall_preserve_unowned_and_official_files(self):
        library = self.client / 'awesomium.dll'
        library.write_bytes(b'user-owned-library')
        self.install(False)
        self.assertFalse((self.client / 'C9LocalLauncher.exe').exists())
        self.install()
        (self.ui / 'L9.exe').write_bytes(b'updated-synthetic-ui')
        self.install()
        self.assertEqual(b'updated-synthetic-ui', (self.client / 'C9LocalLauncher.exe').read_bytes())
        self.assertEqual(b'user-owned-library', library.read_bytes())
        self.assert_official_unchanged()

    def test_changed_user_file_refuses_overwrite(self):
        self.install()
        launcher = self.client / 'C9LocalLauncher.exe'
        launcher.write_bytes(b'user-change')
        with self.assertRaisesRegex(ValueError, 'not owned'):
            self.install()
        self.assertEqual(b'user-change', launcher.read_bytes())
        self.assert_official_unchanged()

    def test_migration_retires_only_matching_owned_files(self):
        self.install()
        record_file = self.records / 'install.local.json'
        record = json.loads(record_file.read_text())
        for name in ('version.dat', 'localinfo.dat'):
            (self.client / name).write_bytes(b'old-owned-file')
            record['owned_files'][name] = installer.sha(b'old-owned-file')
        record_file.write_text(json.dumps(record))
        (self.client / 'localinfo.dat').write_bytes(b'user-change')
        result = self.install()
        self.assertEqual(['version.dat'], result['retired_owned_files'])
        self.assertEqual(['localinfo.dat'], result['preserved_changed_files'])
        self.assertFalse((self.client / 'version.dat').exists())
        self.assertEqual(b'user-change', (self.client / 'localinfo.dat').read_bytes())
        self.assert_official_unchanged()

    def test_updated_client_requires_new_profile(self):
        (self.client / 'C9.exe').write_bytes(b'new-build')
        with self.assertRaisesRegex(ValueError, 'verified launch-argument profile'):
            self.install()
        self.assertFalse((self.client / 'C9LocalLauncher.exe').exists())

    def test_updated_profile_does_not_install_legacy_patch_manifest(self):
        with patch.object(installer, 'OFFICIAL_UPDATED_20261106_SHA256', installer.sha(self.official['C9.exe'])):
            self.install()
            self.install()
        self.assertFalse((self.client / 'version.ini').exists())
        self.assert_official_unchanged()

    def test_explicit_vfun_path_is_local_configuration_only(self):
        vfun = self.root / 'VFUNLauncher.exe'
        vfun.write_bytes(b'fixture-vfun')
        installer.install(self.client, self.ui, self.backend, self.settings, self.dotnet,
                          self.records, 14000, True, vfun)
        config = json.loads((self.client / 'L9.local.json').read_text())
        self.assertEqual(str(vfun.resolve()), config['official_launcher'])
        self.assertEqual(b'fixture-vfun', vfun.read_bytes())
        self.assert_official_unchanged()

    def test_loopback_fixture_can_replace_observer_without_client_changes(self):
        self.install()
        fixture = self.root / 'fixture.json'
        fixture.write_text(json.dumps({'host': '127.0.0.1', 'port': 14000, 'profile': 'fixture'}))
        installer.install(self.client, self.ui, self.backend, fixture, self.dotnet,
                          self.records, 14000, True)
        config = json.loads((self.client / 'L9.local.json').read_text())
        self.assertEqual(str(fixture.resolve()), config['backend_config'])
        self.assert_official_unchanged()

    def test_public_mismatched_or_legacy_lobby_settings_are_rejected(self):
        for overrides in ({'host': '0.0.0.0'}, {'port': 14001}, {'profile': 'character_lobby'}):
            with self.subTest(overrides=overrides):
                config = {'host': '127.0.0.1', 'port': 14000, 'profile': 'fixture', **overrides}
                self.settings.write_text(json.dumps(config))
                with self.assertRaisesRegex(ValueError, 'matching loopback'):
                    self.install()
                self.assertFalse((self.client / 'C9LocalLauncher.exe').exists())


if __name__ == '__main__':
    unittest.main()
