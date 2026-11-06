"""Bounded, read-only CFS index and LZMA-prefix reader for map research.

No files are extracted or executed. Version 2 uses name XOR 0x3A; version 3
uses name XOR 0xF7 and compressed-body XOR 0xC8 in the selected Client.
"""
from dataclasses import dataclass
import lzma
from pathlib import Path
import struct


@dataclass(frozen=True)
class Entry:
    name: str
    size: int
    packed_size: int
    offset: int


class Archive:
    def __init__(self, path):
        self.path = Path(path)
        self.entries = {}
        self.case_names = {}
        length = self.path.stat().st_size
        with self.path.open('rb') as stream:
            def exact(count):
                result = stream.read(count)
                if len(result) != count:
                    raise ValueError('Truncated CFS index')
                return result

            header = exact(20)
            if header[:8] != bytes.fromhex('6366730a0108020d'):
                raise ValueError('Unknown CFS signature/version')
            self.mode, count, reserved = struct.unpack_from('<III', header, 8)
            if self.mode not in (2, 3) or not 0 < count <= 65536 or reserved:
                raise ValueError('Unsupported CFS mode or entry count')
            name_key = 0x3a if self.mode == 2 else 0xf7
            for _ in range(count):
                name_length = struct.unpack('<I', exact(4))[0]
                if not 0 < name_length <= 1024:
                    raise ValueError('CFS filename exceeds bounds')
                raw_name = bytes(b ^ name_key for b in exact(name_length))
                name = raw_name.decode('ascii')
                if '\0' in name or name.casefold() in self.case_names:
                    raise ValueError('Duplicate or NUL-containing CFS filename')
                size, packed, offset = struct.unpack('<III', exact(12))
                if not size or not packed or offset + packed > length:
                    raise ValueError('CFS entry outside archive')
                self.entries[name] = Entry(name, size, packed, offset)
                self.case_names[name.casefold()] = name
            index_end = stream.tell()
        ranges = sorted((e.offset, e.offset + e.packed_size) for e in self.entries.values())
        previous_end = index_end
        for start, end in ranges:
            if start < previous_end:
                raise ValueError('Overlapping CFS entries or index')
            previous_end = end

    def find(self, name):
        actual = self.case_names.get(name.casefold())
        return self.entries[actual] if actual is not None else None

    def prefix(self, name, maximum=64):
        if not 1 <= maximum <= 4096:
            raise ValueError('Prefix size outside bounds')
        entry = self.find(name)
        if entry is None:
            raise KeyError(name)
        if entry.size > 64 * 1024 * 1024 or entry.packed_size > 16 * 1024 * 1024:
            raise ValueError('Entry exceeds diagnostic decompression bounds')
        with self.path.open('rb') as stream:
            stream.seek(entry.offset)
            packed = stream.read(entry.packed_size)
        if len(packed) != entry.packed_size:
            raise ValueError('Archive changed or entry was truncated')
        if self.mode == 3:
            packed = bytes(b ^ 0xc8 for b in packed)
        if len(packed) < 6:
            raise ValueError('Truncated LZMA properties')
        properties, dictionary = packed[0], struct.unpack_from('<I', packed, 1)[0]
        remainder = properties // 9
        if properties >= 225 or not 4096 <= dictionary <= 64 * 1024 * 1024:
            raise ValueError('LZMA properties exceed bounds')
        decoder = lzma.LZMADecompressor(format=lzma.FORMAT_RAW, filters=[{
            'id': lzma.FILTER_LZMA1, 'dict_size': dictionary,
            'lc': properties % 9, 'lp': remainder % 5, 'pb': remainder // 5}])
        wanted = min(maximum, entry.size)
        data = decoder.decompress(packed[5:], max_length=wanted)
        if len(data) != wanted:
            raise ValueError('Incomplete decompressed prefix')
        return data
