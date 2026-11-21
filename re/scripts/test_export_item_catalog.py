import json
import lzma
from pathlib import Path
import struct
import tempfile
import unittest
import zlib

import export_item_catalog as exporter


def text(value):
    raw = (value + '\0').encode('utf-16le') if value else b''
    return struct.pack('<I', len(value) + 1 if value else 0) + raw


def c9t(sections):
    """sections: list of (schema, rows); schema like 'iss'."""
    out = struct.pack('<I', len(sections))
    for schema, rows in sections:
        out += struct.pack('<I', len(rows))
        for row in rows:
            for field, value in zip(schema, row):
                out += struct.pack('<I', value) if field == 'i' else text(value)
    return out


def ascii_field(value):
    raw = value.encode('ascii') + b'\0'
    return struct.pack('<I', len(raw)) + raw


def c9d(records):
    """records: (id, internal name, icon or None); mimics [id][name] fixed... [icon] fixed..."""
    out = b'\x00' * 64
    for item, internal, icon in records:
        out += struct.pack('<I', item) + text(internal) + b'\x04\x06\x1b\xff' + b'\x00' * 12 + text('')
        out += b'\x01\x00\x00\x00' * 20 + (ascii_field(icon) if icon else b'') + b'\x00' * 40
    return out


def dds(width, height, bgra):
    header = bytearray(128)
    header[:4] = b'DDS '
    struct.pack_into('<IIIII', header, 4, 124, 0x100F, height, width, width * 4)
    struct.pack_into('<II4sIIIII', header, 76, 32, 0x41, b'\0\0\0\0', 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000)
    return bytes(header) + bgra * (width * height)


def cfs(files, stamp=None):
    names = list(files)
    header = bytes.fromhex('6366730a0108020d') + (struct.pack('<IIII', 2, stamp, 0, len(names)) if stamp else struct.pack('<III', 2, len(names), 0))
    index_size = sum(4 + len(n) + 12 for n in names)
    offset = len(header) + index_size
    index, bodies = b'', b''
    filters = [dict(id=lzma.FILTER_LZMA1, dict_size=1 << 16, lc=3, lp=0, pb=2)]
    for name in names:
        packed = b'\x5d' + struct.pack('<I', 1 << 16) + lzma.compress(files[name], format=lzma.FORMAT_RAW, filters=filters)
        filler = b'\x00\x00\x00' if stamp else b''
        index += struct.pack('<I', len(name)) + bytes(b ^ 0x3a for b in name.encode()) + struct.pack('<III', len(files[name]), len(packed), offset)
        bodies += filler + packed
        offset += len(filler) + len(packed)
    return header + index + bodies


class ExportCases(unittest.TestCase):
    def test_c9t_sections_pick_item_names(self):
        data = c9t([('isss', [(8, 'Elite Hunter', '', 'desc')]), ('is', [(8, 'Danus'), (9, 'Luid')]),
                    ('iss', [(20332, 'Skill Recycler', 'A token.'), (7380, 'Scarab Horn', '')]), ('iss', [(1, 'x', 'y')])])
        sections = exporter.c9t_sections(data)
        self.assertEqual([s for s, _ in sections], ['isss', 'is', 'iss', 'iss'])
        self.assertEqual(exporter.item_names(data), {20332: ('Skill Recycler', 'A token.'), 7380: ('Scarab Horn', '')})

    def test_c9t_bounds(self):
        for bad in (b'', struct.pack('<I', 0), struct.pack('<I', 999), struct.pack('<II', 1, 5) + struct.pack('<I', 1) + struct.pack('<I', 900000)):
            with self.assertRaises(ValueError):
                exporter.c9t_sections(bad)

    def test_icon_map_uses_first_dds_inside_record(self):
        data = c9d([(20332, '[GP][이벤트]Skill Recycle', 'icon_GP_skillrecycle.dds'), (7380, '[4-1]스카라브의 뿔', 'IQ_P02_015.dds'),
                    (555, '[없음]', None), (99999, '[모름]', 'icon_unknown.dds')])
        self.assertEqual(exporter.icon_map(data, {20332, 7380, 555}),
                         {20332: 'icon_gp_skillrecycle.dds', 7380: 'iq_p02_015.dds'})

    def test_dds_to_png(self):
        image = exporter.dds_rgba(dds(2, 2, bytes([10, 20, 30, 255])))
        self.assertEqual(image, (2, 2, bytes([30, 20, 10, 255]) * 4))
        self.assertIsNone(exporter.dds_rgba(b'DDS ' + b'\0' * 200))
        encoded = exporter.png(*image)
        self.assertEqual(encoded[:8], b'\x89PNG\r\n\x1a\n')
        idat = encoded.index(b'IDAT')
        length = struct.unpack('>I', encoded[idat - 4:idat])[0]
        self.assertEqual(zlib.decompress(encoded[idat + 4:idat + 4 + length]), (b'\x00' + bytes([30, 20, 10, 255]) * 2) * 2)

    def test_export_end_to_end(self):
        with tempfile.TemporaryDirectory() as folder:
            game, out = Path(folder) / 'game', Path(folder) / 'out'
            (game / 'GameData/Msg/ENG').mkdir(parents=True)
            (game / 'Ui_Icon/items').mkdir(parents=True)
            english = c9t([('iss', [(20332, 'Skill Recycler', 'A token for returning users.')])])
            korean = c9t([('iss', [(20332, '스킬 리사이클러', ''), (7380, '스카라브의 뿔', '')])])
            (game / 'GameData/Msg/ENG/ENG.cfs').write_bytes(cfs({'ENG_GameData.c9t': english}, stamp=0x01642A57))
            (game / 'GameData/GameData.cfs').write_bytes(cfs({
                'GameData.c9t': korean,
                'GameData.c9d': c9d([(20332, '[GP]스킬', 'icon_GP_skillrecycle.dds'), (7380, '[4-1]뿔', 'missing.dds')])}, stamp=0x01642A57))
            (game / 'Ui_Icon/items/etc.cfs').write_bytes(cfs({'icon_GP_skillrecycle.dds': dds(2, 2, bytes([1, 2, 3, 255]))}))
            counts = exporter.export(game, out)
            self.assertEqual(counts['items'], 2)
            self.assertEqual(counts['icons_written'], 1)
            report = json.loads((out / 'items.json').read_text(encoding='utf-8'))
            first, second = report['items']
            self.assertEqual((first['id'], first['name_en'], first['name_ko'], first['icon']), (7380, None, '스카라브의 뿔', None))
            self.assertEqual((second['id'], second['name_en'], second['icon']), (20332, 'Skill Recycler', 'icon_gp_skillrecycle'))
            self.assertTrue((out / 'icons/icon_gp_skillrecycle.png').read_bytes().startswith(b'\x89PNG'))
            # The synthetic record's category bytes are 04 06 1B FF: material / middle 27 (unknown skill books).
            self.assertEqual(second['tags'], ['material', 'skill'])
            self.assertEqual((second['grade'], second['use_level'], second['admin_only']), (None, None, False))

    def test_classify_tags_grade_level_admin(self):
        armor = {'internal': '[위치][방어구][레전더리][82]불굴의 투구', 'large': 0, 'middle': 0, 'small': 255, 'service': 7}
        self.assertEqual(exporter.classify(38304, armor, 'Helm'), (['equipment', 'armor', 'head'], 7, 82, False))
        ore = {'internal': '[재료]광석', 'large': 6, 'middle': 8, 'small': 255, 'service': 7}
        self.assertEqual(exporter.classify(1, ore, 'Armor Ore [Magic]: Lv. 57'), (['material', 'ore', 'craft_material'], 3, None, False))
        book = {'internal': '[스킬북][유료]', 'large': 10, 'middle': 0, 'small': 0, 'service': 7}
        self.assertEqual(exporter.classify(2, book, 'Book')[0], ['skill_book', 'skill', 'cash'])
        self.assertTrue(exporter.classify(11610, None, 'Ring')[3])
        test_potion = {'internal': '[테스트]물약', 'large': 1, 'middle': 0, 'small': 255, 'service': 7}
        self.assertTrue(exporter.classify(5, test_potion, 'Test: Potion')[3])
        self.assertEqual(exporter.classify(6, None, None), ([], None, None, False))
        guardian_ring = {'internal': '[수호]수호자 네파시 정령 반지', 'large': 0, 'middle': 8, 'small': 255, 'service': 7}
        self.assertNotIn('auspice', exporter.classify(45485, guardian_ring, 'Guardian Nepahi Elf Ring[WIS]')[0])
        auspice = {'internal': '[이벤트]수호: 모험의 시작', 'large': 3, 'middle': 255, 'small': 255, 'service': 7}
        self.assertEqual(exporter.classify(12700, auspice, 'Auspice: Beginning Adventurer')[0], ['usable', 'auspice', 'event'])


if __name__ == '__main__':
    unittest.main()
