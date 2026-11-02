#!/usr/bin/env python3
"""Read-only PE inventory and string extraction; standard library only."""
import argparse
import hashlib
import json
import math
import re
import struct
from collections import Counter
from datetime import datetime, timezone
from pathlib import Path


class PE:
    def __init__(self, path):
        self.path = path
        self.data = path.read_bytes()
        self.pe = self.u32(0x3c)
        if self.data[:2] != b'MZ' or self.data[self.pe:self.pe + 4] != b'PE\0\0':
            raise ValueError('Not PE')
        coff = self.pe + 4
        self.machine, nsec, self.timestamp = struct.unpack_from('<HHI', self.data, coff)
        optsize = self.u16(coff + 16)
        opt = coff + 20
        self.magic = self.u16(opt)
        self.is64 = self.magic == 0x20b
        self.base = self.u64(opt + 24) if self.is64 else self.u32(opt + 28)
        self.entry = self.u32(opt + 16)
        dd = opt + (112 if self.is64 else 96)
        self.dirs = [struct.unpack_from('<II', self.data, dd + 8 * i) for i in range(16)]
        self.sections = []
        for i in range(nsec):
            p = opt + optsize + 40 * i
            name = self.data[p:p + 8].split(b'\0')[0].decode('ascii', 'replace')
            vs, va, rs, rp = struct.unpack_from('<IIII', self.data, p + 8)
            raw = self.data[rp:rp + rs]
            counts = Counter(raw)
            entropy = -sum((n / len(raw)) * math.log2(n / len(raw)) for n in counts.values()) if raw else 0
            self.sections.append(dict(name=name, rva=va, virtual_size=vs, raw_offset=rp,
                                      raw_size=rs, entropy=round(entropy, 3)))

    def u16(self, p): return struct.unpack_from('<H', self.data, p)[0]
    def u32(self, p): return struct.unpack_from('<I', self.data, p)[0]
    def u64(self, p): return struct.unpack_from('<Q', self.data, p)[0]

    def off(self, rva):
        for s in self.sections:
            delta = rva - s['rva']
            if 0 <= delta < max(s['virtual_size'], s['raw_size']):
                if delta >= s['raw_size']:
                    raise ValueError('RVA in zero-filled region')
                return s['raw_offset'] + delta
        if rva < self.sections[0]['raw_offset']:
            return rva
        raise ValueError(f'RVA not mapped: {rva:x}')

    def va_for_offset(self, p):
        for s in self.sections:
            if s['raw_offset'] <= p < s['raw_offset'] + s['raw_size']:
                return self.base + s['rva'] + p - s['raw_offset']
        return None

    def cstr(self, p):
        end = self.data.find(b'\0', p)
        if end == -1:
            raise ValueError('Unterminated string')
        return self.data[p:end].decode('ascii', 'replace')

    def imports(self):
        rva, size = self.dirs[1]
        if not rva: return []
        result = []
        p = self.off(rva)
        while any(self.data[p:p + 20]):
            oft, ts, fw, name, ft = struct.unpack_from('<IIIII', self.data, p)
            module = self.cstr(self.off(name))
            q = self.off(oft or ft)
            step = 8 if self.is64 else 4
            high = 1 << (63 if self.is64 else 31)
            funcs = []
            for i in range(20000):
                v = self.u64(q + i * step) if self.is64 else self.u32(q + i * step)
                if not v: break
                sym = f'ordinal_{v & 0xffff}' if v & high else self.cstr(self.off(v) + 2)
                funcs.append(dict(name=sym, iat_va=f'0x{self.base + ft + i * step:08x}'))
            result.append(dict(module=module, functions=funcs))
            p += 20
        return result

    def exports(self):
        rva, size = self.dirs[0]
        if not rva: return []
        p = self.off(rva)
        ordinal_base, count, names, funcs_rva, names_rva, ords_rva = struct.unpack_from('<IIIIII', self.data, p + 16)
        result = []
        for i in range(names):
            name = self.cstr(self.off(self.u32(self.off(names_rva) + i * 4)))
            index = self.u16(self.off(ords_rva) + i * 2)
            frva = self.u32(self.off(funcs_rva) + index * 4)
            result.append(dict(name=name, ordinal=ordinal_base + index, va=f'0x{self.base + frva:08x}'))
        return result

    def info(self):
        pdb = []
        for m in re.finditer(b'RSDS', self.data):
            p = m.start() + 24
            try: pdb.append(self.cstr(p))
            except ValueError: pass
        return dict(file=self.path.name, size=len(self.data), sha256=hashlib.sha256(self.data).hexdigest(),
                    machine=f'0x{self.machine:04x}', bitness=64 if self.is64 else 32,
                    image_base=f'0x{self.base:08x}', entry_va=f'0x{self.base + self.entry:08x}',
                    pe_timestamp_raw=self.timestamp,
                    pe_timestamp_utc=datetime.fromtimestamp(self.timestamp, timezone.utc).isoformat(),
                    timestamp_caution='PE timestamp can be modified and does not prove a build date.',
                    sections=self.sections, imports=self.imports(), exports=self.exports(), pdb_paths=pdb)


INTERESTING = re.compile(r'https?://|(?:\d{1,3}\.){3}\d{1,3}|login|logon|auth|packet|socket|connect|disconnect|server|channel|gateway|protocol|recv|send|version|\.pdb|\.ini\b|\.cfg\b|crypt|checksum|lobby', re.I)


def strings(pe):
    for enc, pattern in [('ascii', rb'[\x20-\x7e]{5,}'), ('utf16le', rb'(?:[\x20-\x7e]\x00){5,}')]:
        for m in re.finditer(pattern, pe.data):
            value = m.group().decode('ascii' if enc == 'ascii' else 'utf-16le')
            va = pe.va_for_offset(m.start())
            yield dict(file=pe.path.name, offset=f'0x{m.start():08x}', va=f'0x{va:08x}' if va is not None else '', encoding=enc, value=value)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('client', type=Path)
    parser.add_argument('output', type=Path)
    args = parser.parse_args()
    args.output.mkdir(parents=True, exist_ok=True)
    names = ['C9.exe', 'C9Launcher.exe', 'C9Patcher.exe', 'BaseCat.Network.dll', 'BaseCat.System.dll', 'PhysXLoader.dll', 'xp.dll']
    infos, hits = [], []
    for name in names:
        path = args.client / name
        if not path.exists(): continue
        pe = PE(path)
        infos.append(pe.info())
        ss = list(strings(pe))
        selected = [s for s in ss if INTERESTING.search(s['value'])]
        hits.extend(selected)
        (args.output / f'{name}.strings.json').write_text(json.dumps(ss, ensure_ascii=False, indent=2) + '\n')
        print(f'{name}: {len(pe.imports())} import DLLs, {len(pe.exports())} named exports, {len(selected)} relevant strings')
    (args.output / 'pe_inventory.json').write_text(json.dumps(infos, ensure_ascii=False, indent=2) + '\n')
    (args.output / 'network_strings.json').write_text(json.dumps(hits, ensure_ascii=False, indent=2) + '\n')
    with (args.output / 'imports.tsv').open('w') as out:
        out.write('file\tdll\tsymbol\tiat_va\n')
        for info in infos:
            for module in info['imports']:
                for func in module['functions']:
                    out.write(f"{info['file']}\t{module['module']}\t{func['name']}\t{func['iat_va']}\n")


if __name__ == '__main__':
    main()
