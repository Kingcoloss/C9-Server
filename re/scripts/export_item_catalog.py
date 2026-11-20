"""Export the item catalog (ids, ENG/KOR names, descriptions, icons) from an installed Client.

    python re/scripts/export_item_catalog.py --game C:\\VFUN\\C9-GL --out artifacts/docker-state/item-catalog

Reads the Client folder only (never writes there). Writes items.json and icons/<icon>.png to --out.
The output is the game publisher's data: keep it in the Docker state folder, never in git.
The Admin Portal imports items.json into game.item_catalog (Items Catalog page).

Formats (docs/ITEM_CATALOG_DELIVERY_TH.md section 1):
- c9t: DWORD header; then sections = DWORD record count + records of [u32 id] + fields,
  each field a [u32 length][UTF-16 incl. NUL] string or a DWORD. Item names = largest (id, name, description) section.
- c9d item records: [u32 id][u32 length][Korean internal name UTF-16] ... [u32 length][icon file ASCII .dds];
  the icon is the first .dds string inside the record (found heuristically, see icon_map).
- Item icons: uncompressed 32-bit DDS in Ui_Icon/**/*.cfs.
"""
import argparse
import bisect
from datetime import datetime, timezone
import itertools
import json
from pathlib import Path
import re
import struct
import sys
import zlib

sys.path.insert(0, str(Path(__file__).resolve().parent))
from c9_cfs import Archive  # noqa: E402

SCHEMA_VERSION = 1
MAX_STRING = 20000
MAX_SECTIONS = 16
# Record layouts after the id: shortest first, then strings before DWORDs (an empty string and a 0 look alike).
_SCHEMAS = sorted({'i' + ''.join(t) for n in range(1, 6) for t in itertools.product('si', repeat=n) if t[0] == 's'},
                  key=lambda s: (len(s), s.count('i'), s))


def _string(data, at):
    if at + 4 > len(data):
        return None
    length = struct.unpack_from('<I', data, at)[0]
    end = at + 4 + 2 * length
    if length > MAX_STRING or end > len(data) or (length and data[end - 2:end] != b'\x00\x00'):
        return None
    return end, data[at + 4:end - 2].decode('utf-16le', 'replace') if length else ''


def _record(data, at, schema):
    values = []
    for field in schema:
        if field == 'i':
            if at + 4 > len(data):
                return None
            values.append(struct.unpack_from('<I', data, at)[0])
            at += 4
        else:
            parsed = _string(data, at)
            if parsed is None:
                return None
            at, text = parsed
            values.append(text)
    return at, values


def _section(data, at, schema):
    records = struct.unpack_from('<I', data, at)[0]
    position, rows = at + 4, []
    for _ in range(records):
        parsed = _record(data, position, schema)
        if parsed is None:
            return None
        position, row = parsed
        rows.append(row)
    return position, rows


def _next_plausible(data, at):
    """True at the end of data, or when a section starts here whose first record parses with some layout."""
    if at == len(data):
        return True
    if at + 4 > len(data):
        return False
    records = struct.unpack_from('<I', data, at)[0]
    if records >= 2_000_000:
        return False
    return 0 < records and any(_record(data, at + 4, schema) is not None for schema in _SCHEMAS)


def c9t_sections(data, limit=MAX_SECTIONS):
    """[(schema, records)] for the leading sections whose layout can be inferred (shortest layout that parses and
    is followed by a plausible section or the end of data). The first DWORD is a header value, not a count."""
    if len(data) < 8:
        raise ValueError('c9t too short')
    at, sections = 4, []
    while at < len(data) and len(sections) < limit:
        for schema in _SCHEMAS:
            parsed = _section(data, at, schema)
            if parsed is not None and _next_plausible(data, parsed[0]):
                break
        else:
            break
        sections.append((schema, parsed[1]))
        at = parsed[0]
    if not sections:
        raise ValueError('c9t has an unknown section layout')
    return sections


def item_names(data):
    """{id: (name, description)} from the largest (id, name, description) section."""
    candidates = [rows for schema, rows in c9t_sections(data) if schema == 'iss']
    if not candidates:
        raise ValueError('no item name section')
    return {row[0]: (row[1], row[2]) for row in max(candidates, key=len)}


def _text(code):
    return (0x20 <= code <= 0x7e or 0xac00 <= code <= 0xd7a3 or 0x3131 <= code <= 0x318e or
            0x4e00 <= code <= 0x9fff or code in (0x2019, 0x2026, 0xb7, 0x3000))


_DDS = re.compile(rb'[\x05-\x80]\x00\x00\x00([\x21-\x7e][\x20-\x7e]*?\.dds)\x00', re.I)
_SMALL = re.compile(rb'(?=[\x03-\xc8]\x00\x00\x00)')


def icon_map(c9d, known_ids, window=2000):
    """{item id: icon file (lower case)}. A record head is [id][length][internal name] with a Korean or bracketed
    name; the icon of a head with a known item id is the first .dds string before the next head (any id, so that
    records of other tables bound the search) and at most ``window`` bytes away."""
    heads = []
    for match in _SMALL.finditer(c9d, 4):
        start = match.start() - 4
        item = struct.unpack_from('<I', c9d, start)[0]
        if item > 1_000_000:
            continue
        length = c9d[match.start()]
        end = start + 8 + 2 * length
        if end > len(c9d) or c9d[end - 2:end] != b'\x00\x00':
            continue
        codes = struct.unpack_from(f'<{length - 1}H', c9d, start + 8)
        if not all(_text(c) for c in codes):
            continue
        name = c9d[start + 8:end - 2].decode('utf-16le')
        if name.startswith('[') or any(0xac00 <= ord(c) <= 0xd7a3 for c in name):
            heads.append((start, item))
    icons = [(m.start(), m.group(1).decode('ascii').lower()) for m in _DDS.finditer(c9d)]
    starts = [h[0] for h in heads]
    result = {}
    for index, (start, item) in enumerate(heads):
        if item in result or item not in known_ids:
            continue
        end = min(starts[index + 1] if index + 1 < len(heads) else len(c9d), start + window)
        k = bisect.bisect_left(icons, (start, ''))
        if k < len(icons) and icons[k][0] < end:
            result[item] = icons[k][1]
    return result


def dds_rgba(data):
    """(width, height, RGBA bytes) of an uncompressed 32-bit DDS, or None for other formats."""
    if len(data) < 128 or data[:4] != b'DDS ':
        return None
    height, width = struct.unpack_from('<II', data, 12)
    flags, fourcc, bits, red, green, blue, alpha = struct.unpack_from('<I4sIIIII', data, 80)
    if not (flags & 0x40) or bits != 32 or not 0 < width <= 1024 or not 0 < height <= 1024:
        return None
    pixels = data[128:128 + width * height * 4]
    if len(pixels) != width * height * 4:
        return None
    if (red, green, blue) == (0x00ff0000, 0x0000ff00, 0x000000ff) and alpha in (0xff000000, 0):
        out = bytearray(len(pixels))  # BGRA (the observed layout): swap channels with slices.
        out[0::4], out[1::4], out[2::4] = pixels[2::4], pixels[1::4], pixels[0::4]
        out[3::4] = pixels[3::4] if alpha else b'\xff' * (width * height)
        return width, height, bytes(out)
    shifts = [(mask, (mask & -mask).bit_length() - 1) for mask in (red, green, blue, alpha)]
    out = bytearray(width * height * 4)
    for i, (value,) in enumerate(struct.iter_unpack('<I', pixels)):
        for c, (mask, shift) in enumerate(shifts):
            out[i * 4 + c] = ((value & mask) >> shift) & 0xff if mask else 0xff
    return width, height, bytes(out)


def png(width, height, rgba):
    def chunk(kind, body):
        return struct.pack('>I', len(body)) + kind + body + struct.pack('>I', zlib.crc32(kind + body) & 0xffffffff)
    rows = b''.join(b'\x00' + rgba[y * width * 4:(y + 1) * width * 4] for y in range(height))
    return (b'\x89PNG\r\n\x1a\n' + chunk(b'IHDR', struct.pack('>IIBBBBB', width, height, 8, 6, 0, 0, 0)) +
            chunk(b'IDAT', zlib.compress(rows, 9)) + chunk(b'IEND', b''))


def export(game, out):
    game, out = Path(game), Path(out)
    english = item_names(Archive(game / 'GameData/Msg/ENG/ENG.cfs').read('ENG_GameData.c9t'))
    data = Archive(game / 'GameData/GameData.cfs')
    korean = item_names(data.read('GameData.c9t'))
    ids = set(english) | set(korean)
    icons = icon_map(data.read('GameData.c9d'), ids)
    where = {}
    for archive in sorted((game / 'Ui_Icon').rglob('*.cfs')):
        opened = Archive(archive)
        for name in opened.entries:
            where.setdefault(name.lower(), (opened, name))
    (out / 'icons').mkdir(parents=True, exist_ok=True)
    written, unsupported, exported = set(), 0, {}
    for icon in sorted(set(icons.values())):
        if icon not in where:
            continue
        opened, name = where[icon]
        image = dds_rgba(opened.read(name, limit=4 * 1024 * 1024))
        if image is None:
            unsupported += 1
            continue
        key = re.sub(r'[^a-z0-9_.-]', '_', icon[:-4])
        (out / 'icons' / f'{key}.png').write_bytes(png(*image))
        written.add(icon)
        exported[icon] = key
    items = [{
        'id': item,
        'name_en': english.get(item, (None, None))[0] or None,
        'name_ko': korean.get(item, (None, None))[0] or None,
        'description_en': english.get(item, (None, None))[1] or None,
        'icon': exported.get(icons.get(item)),
    } for item in sorted(ids)]
    report = {
        'schema': SCHEMA_VERSION,
        'exported_at': datetime.now(timezone.utc).isoformat(timespec='seconds'),
        'counts': {'items': len(items), 'english': len(english), 'korean': len(korean),
                   'with_icon_record': len(icons), 'icons_written': len(written), 'icons_unsupported': unsupported},
        'items': items,
    }
    (out / 'items.json').write_text(json.dumps(report, ensure_ascii=False), encoding='utf-8')
    return report['counts']


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__.split('\n')[0])
    parser.add_argument('--game', required=True, help='Client folder that contains GameData and Ui_Icon')
    parser.add_argument('--out', required=True, help='Output folder (Docker state), for example artifacts/docker-state/item-catalog')
    args = parser.parse_args(argv)
    print(json.dumps(export(args.game, args.out)))


if __name__ == '__main__':
    main()
