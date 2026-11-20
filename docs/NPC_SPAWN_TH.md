# NPC ในเมือง: contract จาก client ปัจจุบัน (ร่าง)

อัปเดต 20 พ.ย. 2026 ~09:15 น. (Asia/Bangkok) โดย thread "แก้ NPC และ 2nd password ในเกม" (สำเนาจาก `/mnt/project-files/second-password/NPC_SPAWN_NOTES_TH.md`)
หลักฐานมาจาก image `C9.exe` ของ client b4580ed4 (static RE ใน cloud) ยังไม่ได้ยืนยันในเกม

## Current State

- **โค้ดฝั่ง server อยู่ใน branch `claude/second-password-npc` และผ่าน test (`C9.Validation` 73/73, รวม 2nd password)** แต่ยังไม่มีรายชื่อ NPC จริง จึงยังไม่ส่ง NPC ในเกม
  - ไฟล์: `src/C9.Server/NpcSpawns.cs`, `tests/C9.Validation/NpcSpawnCases.cs`; แก้ `ServerOptions.cs`, `Program.cs`, `SessionProcessor.cs`, `KnownPackets.cs` (+0x0BE3/0x0BE4)
  - Ref catalog 2014 ยืนยัน id 3042–3044 = 0x0BE2–0x0BE4; ตาราง registration ของ client: 0x0BE3 encrypted (flag 1), 0x0BE4 plain (flag 0)
  - option ใหม่ `npc_spawn_file` ใน profile character lobby (path เทียบกับไฟล์ profile) ชี้ไปที่ JSON: `{ "maps": [ { "map_id": 10, "submap": 0, "npcs": [ { "info_id": 1001, "x": 0, "y": 0, "z": 0, "yaw": 0, "hp": 1 } ] } ] }`
  - ไม่ตั้ง option = ไม่ส่ง NPC (พฤติกรรมเดิม)
  - หลัง `0x0BC5 → 0x0BC6` server ส่ง `0x0BE3` ครั้งเดียว (แบ่ง packet ละไม่เกิน 255 ตัว) และบันทึก event `npc_spawn`
  - object id ของ NPC = `0x4E50_0000_0000_0000 | map_id<<24 | submap<<16 | ลำดับ` จึงไม่ชนกับ id ตัวละคร
  - HP/MaxHP ตั้งค่าเริ่มต้น 1 (HP 0 ทำให้ client ถือว่าตาย ตามที่พบในงานเดินด้วยคีย์บอร์ด)
  - field ที่ยังไม่รู้ความหมาย (+0x00, +0x04, +0x38..+0x6F) เป็น 0; vector ที่สอง (+0x2C) ใส่ตำแหน่งเดียวกับตำแหน่ง NPC
- **แหล่งรายชื่อ NPC ที่น่าจะใช้ได้โดยไม่ต้องรอ Ref:** ไฟล์ marker ของแผนที่ฝั่ง client `C9_1th_Town_Waterford_VILLAGE.C9MM` (ใน `MapData/C9_1th_Town_Waterford/C9_1th_Town_Waterford.cfs`) มี 71 entity ขนาดรวม 47,821 ไบต์ (เฉลี่ย ~670 ไบต์/ตัว) แต่ละตัวมี id, kind และตำแหน่ง
  - ถ้าไฟล์นี้มี NPC info id อยู่ด้วย จะสร้าง `npc_spawn_file` ได้จาก client โดยตรง ต้องดึงไฟล์ .cfs จากเครื่องผู้ใช้มาแกะ (ตัวอ่าน CFS อยู่ใน `re/scripts/build_map_manifest.py`)
  - loader ฝั่ง client: `00C9B9B0` ต่อ `.C9MM` แล้วเรียก `00841F70`
- **ขั้นต่อไป:** (1) ดึง Waterford .cfs มาแกะ entity ทั้ง 71 ตัว หรือดู NPC spawn ใน Ref ServerData (2) สร้าง `config/npc-spawns.json` (3) deploy แล้วให้ผู้ใช้ดูว่า NPC ขึ้นที่ Waterford หรือไม่

## สรุป

- **NPC ถูกสร้างตามคำสั่ง server ไม่ได้สร้างเองจากข้อมูลแผนที่ฝั่ง client.**
  - คำสั่ง GM `/createnpc` (0x0086857A) เรียก sender `0x0089C460` ของ `C9NetworkAdminCommand` ส่ง packet `0x1786` (cheat, subtype 6) ไปให้ server สร้าง NPC
  - server ของเรายังไม่เคยส่ง packet สร้าง NPC เลย จึงไม่มี NPC ในเมือง
- **packet ที่สร้าง NPC น่าจะเป็น `0x0BE3`** (dispatcher `C9NetworkCharacterMove` 0x008CE070, case 0x008CE16F)
  - ใน client รุ่นก่อน string log เรียงกันเป็น `PS_Exist_PC_Ack`, `PS_Exist_NPC_Ack`, `PS_Remove_Object_Ack` ตรงกับ handler สามตัวที่อยู่ติดกัน:

| opcode | รูปแบบ payload | ความหมาย (อนุมาน) |
|---|---|---|
| `0x0BE2` | u8 count + entry ละ 0x270 (624) ไบต์ → `0x008CE7B0` | ผู้เล่นอื่นที่อยู่ในระยะ (Exist_PC) |
| `0x0BE3` | u8 count + entry ละ 0x70 (112) ไบต์ → `0x008CEAB0` | NPC/มอนสเตอร์ที่อยู่ในระยะ (Exist_NPC) |
| `0x0BE4` | u8 count + u64 object id ละ 8 ไบต์ → `0x00780180` | ลบ object ออกจากฉาก (ข้าม id ของตัวเอง) |

## Entry ของ `0x0BE3` (112 ไบต์)

handler `0x008CEAB0` สร้าง event ชนิด 0x16 ผ่าน `0x007E0FA0` ด้วย object type = 2 (PC = 1) แล้วใส่คิวสร้าง actor

| offset | ชนิด | ใช้ทำอะไร |
|---|---|---|
| +0x00 | u32 | event +0x50 |
| +0x04 | u32 | event +0x38 |
| +0x08 | u64 | **object id** (ถ้าตรงกับ id ของผู้เล่นเองจะข้าม) |
| +0x10, +0x14 | u32, u32 | event +0x40, +0x44 (ใน PC ตำแหน่งนี้น่าจะเป็น HP/MaxHP; ต้องยืนยัน) |
| +0x18 | u32 | **NPC info id** (template; client ค้นในตาราง `[0x179FFD4]+0x74`) |
| +0x1C | float×3 | **ตำแหน่ง x, y, z** |
| +0x28 | float | **ทิศ (yaw)** |
| +0x2C | float×3 | vector ที่สอง (event +0x20..0x28: ทิศหรือจุดหมาย) |
| +0x38 | u32 | event +0x48 |
| +0x40, +0x44 | u32, u32 | event +0x30, +0x34 |
| +0x48, +0x4C | u32, u32 | event +0x82, +0x86 |
| +0x50..+0x6F | 32 ไบต์ | คัดลอกเข้า record ต่อ object (`0x008CF4D0`) |

## สิ่งที่ยังขาด (ต้องใช้เครื่องผู้ใช้)

1. ~~ยืนยันกับ catalog ของ Ref 2014~~ ยืนยันแล้วว่ามี id 3043 (encrypted) แต่ catalog ไม่ได้ระบุ handler ที่ส่ง ต้องดูโค้ด Ref Game server เพื่อรู้ความหมาย field ที่เหลือ
2. รายชื่อ NPC ของแต่ละเมือง (info id + ตำแหน่ง + ทิศ) จาก `ServerData`/MapData ฝั่ง server ของ Ref (`C:\MyProjects\C9-Legacy-Ref\patch65-loulx\vm-d\`, 142 โฟลเดอร์แผนที่)
3. ทดสอบในเกม: หลัง `0x0BC5 → 0x0BC6` ให้ server ส่ง `0x0BE3` ที่มี NPC ของแผนที่นั้น แล้วดูว่า NPC ขึ้นและคุยได้หรือไม่ (โค้ดส่งพร้อมแล้ว ขาดแค่ข้อมูล)

## ผลพลอยได้สำหรับ thread อื่น

ตาราง opcode → handler ของ client ครบ 898 ตัว (ทุก dispatcher 79 คลาส) อยู่ที่ `/mnt/project-files/second-password/re/routes.json` (สร้างด้วย `re/route.py`, `re/routeall.py`; ไม่ได้อยู่ใน repo)
- `0x0E4C` (ที่ thread ระบบพื้นฐานยังหาไม่เจอ) อยู่ใน `C9NetworkMap` case `0x008EF7CC`
- `0x0BC3` / `0x0BC6` / `0x0BC8` อยู่ใน `C9NetworkMap` (`0x008EF349` / `0x008EF289` / `0x008EF1B1`)
- `0xEADA/0xEAF6/0xEAFD/0xEAFE` อยู่ใน `C9NetworkGuildBattle`
