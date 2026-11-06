#!/usr/bin/env python3
"""Match every captured MapData/sub-map record to the selected Client's CFS files.

Exports metadata and a bounded first-marker sample, never extracted game assets.
File presence and valid prefixes do not prove that a map is playable.
"""
import argparse
from collections import Counter
from datetime import datetime, timezone
import hashlib
import json
import lzma
import math
from pathlib import Path
import re
import struct

from c9_cfs import Archive
from c9_paths import resolve_client_dir
from inspect_client_maps import EXPECTED_SHA256


def write_markdown(report, path):
    maps = report['maps']
    fully_ready = sum(bool(m['submaps']) and all(s['files_ready'] for s in m['submaps']) for m in maps)
    some_ready = sum(any(s['files_ready'] for s in m['submaps']) for m in maps)
    lines = [
        '# ทะเบียนแผนที่ C9 และผลตรวจไฟล์', '',
        'รายงานสร้างจาก MapData ที่ client โหลดจริง และตารางไฟล์ CFS ใน MapData ทั้งหมด',
        'ตัวสร้างรายงาน: `re/scripts/build_map_manifest.py` พร้อม `--markdown`', '',
        f"- รหัสแผนที่: {report['map_count']}; โฟลเดอร์ฉาก: {report['folder_count']}; ส่วนย่อย: {report['submap_count']}",
        f"- ตรวจแพ็ก {report['archive_count']:,} ไฟล์ / {report['archive_entry_count']:,} รายการ; ตารางไฟล์ผิดพลาด {len(report['archive_index_issues'])} รายการ",
        f"- พบ geometry และ marker ที่อ่านส่วนต้นได้ {report['files_ready_submaps']}/{report['submap_count']} คู่",
        f'- มีไฟล์ตรงครบทุกส่วนย่อย {fully_ready} แผนที่; มีอย่างน้อยหนึ่งส่วนย่อยที่ตรง {some_ready} แผนที่',
        f"- พบพิกัดตัวอย่างจาก marker แรก {report['sample_position_submaps']} คู่", '',
        'คำว่าไฟล์พร้อมในรายงานหมายถึงพบชื่อไฟล์และอ่านส่วนต้นที่บีบอัดได้',
        'การเล่นจริงยังต้องทดสอบการโหลดฉากและระบบ world ของแต่ละคู่',
        'คู่ที่ยังไม่ครบต้องตรวจข้อมูลเก่า ชื่อเฉพาะภาษา และไฟล์ที่ขาดต่อไป',
        'พิกัดตัวอย่างเป็น marker แรกของไฟล์ ยังไม่ใช่จุดเกิดผู้เล่นที่ยืนยันแล้ว', '',
        '## ใช้กับบริการในเครื่อง', '',
        'ตั้ง `initial_map_id` และ `initial_submap` ใน `config/lab-character-lobby.json`',
        'ระบบตรวจคู่รหัสและส่วนย่อยจาก catalog ก่อนเปิดบริการ ส่วนย่อยเริ่มจาก 0',
        'ถ้าไม่มีพิกัดตัวอย่าง ต้องระบุ `initial_position` จากข้อมูลที่ตรวจสอบแล้ว',
        'ที่หน้า Choose World ใช้ `scripts/Update-C9Handoff.ps1` เพื่ออัปเดตบริการตัวละคร', '',
        '## รายการครบทุก Map ID', '',
        '| Map ID | ชื่อในข้อมูลเกม | โฟลเดอร์ | ไฟล์พร้อม / ส่วนย่อย | ลำดับที่ยังไม่ครบ |',
        '|---:|---|---|---:|---|',
    ]
    for m in maps:
        ready = sum(s['files_ready'] for s in m['submaps'])
        missing = ', '.join(str(s['index']) for s in m['submaps'] if not s['files_ready']) or '—'
        name = m['name'].replace('|', '\\|').replace('\n', ' ')
        lines.append(f"| {m['id']} | {name} | {m['folder']} | {ready}/{len(m['submaps'])} | {missing} |")
    lines += ['', 'รายละเอียด suffix ชื่อไฟล์ hash และพิกัดตัวอย่างอยู่ใน',
              '[map manifest](../re/evidence/static/windows_map_manifest.json) และ',
              '[catalog จาก client](../re/evidence/static/windows_map_catalog.json)', '']
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text('\n'.join(lines), encoding='utf-8')


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--catalog', type=Path, required=True)
    parser.add_argument('--client-dir', type=Path)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--markdown', type=Path)
    args = parser.parse_args()
    client = resolve_client_dir(args.client_dir).resolve()
    if args.output.resolve().is_relative_to(client):
        parser.error('Reports must be written outside the Client directory')
    if args.markdown and args.markdown.resolve().is_relative_to(client):
        parser.error('Markdown reports must be written outside the Client directory')
    catalog = json.loads(args.catalog.read_text(encoding='utf-8'))
    if catalog['client_exe_sha256'] != EXPECTED_SHA256 or hashlib.sha256((client / 'C9.exe').read_bytes()).hexdigest() != EXPECTED_SHA256:
        raise ValueError('Map catalog and Client must match the fixed research build')
    if len(catalog['maps']) != catalog['map_count'] or len({m['id'] for m in catalog['maps']}) != catalog['map_count']:
        raise ValueError('Map catalog identity/count mismatch')
    root = client / 'MapData'
    archives, modes, index_issues = {}, Counter(), []
    archive_files = sorted(root.rglob('*.cfs'))
    entry_count = 0
    for file in archive_files:
        try:
            archive = Archive(file)
            archives[file.relative_to(root).as_posix().casefold()] = archive
            modes[archive.mode] += 1
            entry_count += len(archive.entries)
        except (ValueError, UnicodeError) as error:
            index_issues.append({'archive': file.relative_to(root).as_posix(), 'reason': str(error)})
    maps, probe_cache, hashes = [], {}, {}

    def probe(archive, name):
        cache_key = (archive.path, name)
        if cache_key in probe_cache:
            return probe_cache[cache_key]
        entry = archive.find(name)
        result = {'present': entry is not None}
        if result['present']:
            try:
                prefix = archive.prefix(name)
                result.update(prefix_valid=True, uncompressed_bytes=entry.size, archive_entry=entry.name)
                if name.lower().endswith('.c9mm'):
                    version = struct.unpack_from('<I', prefix)[0]
                    result['version'] = version
                    # Version 54's first entity begins after the 13-byte header.
                    if version == 54 and len(prefix) >= 37:
                        count, marker_id, marker_kind = struct.unpack_from('<III', prefix, 9)
                        point = struct.unpack_from('<fff', prefix, 25)
                        if 0 < count <= 100000 and all(math.isfinite(v) and abs(v) <= 1e7 for v in point):
                            result.update(marker_count=count, sample_marker_id=marker_id,
                                          sample_marker_kind=marker_kind, sample_position=list(point))
            except (ValueError, struct.error, EOFError, lzma.LZMAError) as error:
                result.update(prefix_valid=False, reason=str(error))
        probe_cache[cache_key] = result
        return result

    for original in catalog['maps']:
        item = dict(original)
        folder = item['folder']
        if not re.fullmatch(r'[A-Za-z0-9_-]+', folder):
            raise ValueError('Unexpected map folder syntax')
        relative = f'{folder}/{folder}.cfs'
        archive = archives.get(relative.casefold())
        item['archive'] = relative
        item['archive_present'] = archive is not None
        item['submaps'] = []
        if archive:
            if archive.path not in hashes:
                hashes[archive.path] = hashlib.sha256(archive.path.read_bytes()).hexdigest()
            item['archive_sha256'] = hashes[archive.path]
            item['archive_mode'] = archive.mode
            item['geometry'] = probe(archive, folder + '.r3sd')
        for submap in original['submaps']:
            variant = dict(submap)
            variant['markers'] = probe(archive, variant['marker_file']) if archive else {'present': False}
            variant['files_ready'] = bool(archive and item['geometry'].get('prefix_valid') and variant['markers'].get('prefix_valid'))
            item['submaps'].append(variant)
        maps.append(item)
    submaps = [s for m in maps for s in m['submaps']]
    report = {
        'schema_version': 1, 'recorded_at_utc': datetime.now(timezone.utc).isoformat(),
        'client_exe_sha256': EXPECTED_SHA256,
        'source_catalog_sha256': hashlib.sha256(args.catalog.read_bytes()).hexdigest(),
        'scope': 'Complete MapData catalog and all MapData CFS indices; geometry/marker prefixes only.',
        'playability_verified': False,
        'map_count': len(maps), 'folder_count': len({m['folder'] for m in maps}),
        'submap_count': len(submaps), 'files_ready_submaps': sum(s['files_ready'] for s in submaps),
        'sample_position_submaps': sum('sample_position' in s['markers'] for s in submaps),
        'archive_count': len(archive_files), 'archive_modes': dict(modes), 'archive_entry_count': entry_count,
        'archive_index_issues': index_issues, 'maps': maps,
    }
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
    if args.markdown:
        write_markdown(report, args.markdown)
    print(json.dumps({k: report[k] for k in ['map_count', 'folder_count', 'submap_count', 'files_ready_submaps',
                                           'sample_position_submaps', 'archive_count', 'archive_entry_count']}))
    print(json.dumps({'unready_map_ids': sorted({m['id'] for m in maps for s in m['submaps'] if not s['files_ready']})}))


if __name__ == '__main__':
    main()
