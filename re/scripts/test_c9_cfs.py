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


class CfsCases(unittest.TestCase):
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
