import lzma
from pathlib import Path
import struct
import tempfile
import unittest

from c9_cfs import Archive


def fixture(mode=2):
    name = b'Map.C9MM'
    data = struct.pack('<I', 54) + bytes(range(128))
    filters = [dict(id=lzma.FILTER_LZMA1, dict_size=65536, lc=3, lp=0, pb=2)]
    packed = b'\x5d' + struct.pack('<I', 65536) + lzma.compress(data, format=lzma.FORMAT_RAW, filters=filters)
    if mode == 3:
        packed = bytes(b ^ 0xc8 for b in packed)
    offset = 20 + 4 + len(name) + 12
    encoded = bytes(b ^ (0x3a if mode == 2 else 0xf7) for b in name)
    archive = (bytes.fromhex('6366730a0108020d') + struct.pack('<III', mode, 1, 0) +
               struct.pack('<I', len(name)) + encoded + struct.pack('<III', len(data), len(packed), offset) + packed)
    return archive, data, offset


def fixture_2026(filler=b'\xa0\xfc\x65\x07\x5c\x96\x0d'):
    """24-byte header (mode, stamp, reserved, count) and a filler before the LZMA body (2026 Client)."""
    name = b'GameData.c9t'
    data = bytes(range(256)) * 40
    filters = [dict(id=lzma.FILTER_LZMA1, dict_size=1 << 22, lc=3, lp=0, pb=2)]
    packed = b'\x5d' + struct.pack('<I', 1 << 22) + lzma.compress(data, format=lzma.FORMAT_RAW, filters=filters)
    offset = 24 + 4 + len(name) + 12
    encoded = bytes(b ^ 0x3a for b in name)
    archive = (bytes.fromhex('6366730a0108020d') + struct.pack('<IIII', 2, 0x01642A57, 0, 1) +
               struct.pack('<I', len(name)) + encoded + struct.pack('<III', len(data), len(packed), offset) + filler + packed)
    return archive, data


class CfsCases(unittest.TestCase):
    def test_2026_header_and_body_filler(self):
        for filler in (b'', b'\x00' * 9, b'\xa0\xfc\x65\x07\x5c\x96\x0d\x77\x2c'):
            raw, expected = fixture_2026(filler)
            self.path.write_bytes(raw)
            archive = Archive(self.path)
            self.assertEqual((archive.mode, archive.stamp), (2, 0x01642A57))
            self.assertEqual(archive.read('gamedata.C9T'), expected)
            self.assertEqual(archive.prefix('GameData.c9t', 16), expected[:16])

    def test_2026_filler_without_properties_rejected(self):
        raw, _ = fixture_2026(b'')
        broken = bytearray(raw)
        start = len(raw) - (len(raw) - 24 - 4 - 12 - 12)
        broken[start:start + 5] = b'\xff' * 5
        self.path.write_bytes(bytes(broken))
        with self.assertRaises(ValueError):
            Archive(self.path).read('GameData.c9t')

    def test_read_whole_entry_and_bound(self):
        raw, expected, _ = fixture()
        self.path.write_bytes(raw)
        archive = Archive(self.path)
        self.assertEqual(archive.read('Map.C9MM'), expected)
        with self.assertRaises(ValueError):
            archive.read('Map.C9MM', limit=16)

    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.path = Path(self.directory.name) / 'map.cfs'

    def tearDown(self):
        self.directory.cleanup()

    def test_both_observed_modes_and_case_insensitive_lookup(self):
        for mode in (2, 3):
            raw, expected, _ = fixture(mode)
            self.path.write_bytes(raw)
            archive = Archive(self.path)
            self.assertEqual(archive.mode, mode)
            self.assertEqual(archive.find('MAP.c9mm').name, 'Map.C9MM')
            self.assertEqual(archive.prefix('map.c9mm'), expected[:64])

    def test_unknown_and_truncated_headers_rejected(self):
        raw, _, _ = fixture()
        for bad in (b'', raw[:19], b'bad!' + raw[4:], raw[:8] + struct.pack('<I', 4) + raw[12:], raw[:29]):
            self.path.write_bytes(bad)
            with self.assertRaises(ValueError):
                Archive(self.path)

    def test_entry_cannot_overlap_index_or_leave_file(self):
        raw, _, offset = fixture()
        for target in (offset - 1, len(raw) + 1):
            modified = bytearray(raw)
            struct.pack_into('<I', modified, offset - 4, target)
            self.path.write_bytes(modified)
            with self.assertRaises(ValueError):
                Archive(self.path)

    def test_lzma_dictionary_bound_checked_before_allocation(self):
        raw, _, offset = fixture()
        modified = bytearray(raw)
        struct.pack_into('<I', modified, offset + 1, 0xffffffff)
        self.path.write_bytes(modified)
        with self.assertRaises(ValueError):
            Archive(self.path).prefix('Map.C9MM')

    def test_prefix_bounds_and_missing_name(self):
        self.path.write_bytes(fixture()[0])
        archive = Archive(self.path)
        for maximum in (0, 4097):
            with self.assertRaises(ValueError):
                archive.prefix('Map.C9MM', maximum)
        with self.assertRaises(KeyError):
            archive.prefix('missing.C9MM')

    def test_archive_changed_after_indexing_rejected(self):
        raw, _, offset = fixture()
        self.path.write_bytes(raw)
        archive = Archive(self.path)
        self.path.write_bytes(raw[:offset + 3])
        with self.assertRaises(ValueError):
            archive.prefix('Map.C9MM')


if __name__ == '__main__':
    unittest.main()
