# NPC ในเมือง: Current State และ contract ของ 0x0BE3

อัปเดต 21 พ.ย. 2026 ~05:50 น. (Asia/Bangkok) โดย thread "แก้ NPC และ 2nd password ในเกม"
มาตรฐานอ้างอิง: ISO/IEC 27001, ISO 9001, ISO/IEC 29110

## Current State

- **สาเหตุเดิมที่ NPC หาย:** server ไม่เคยส่ง packet สร้าง NPC client ไม่สร้าง NPC เองจากข้อมูลแผนที่
  - client รอให้ server ส่ง `0x0BE3` (Exist_NPC)
  - คำสั่ง GM `/createnpc` (0x0086857A) ก็ส่ง `0x1786` subtype 6 ให้ server เป็นผู้สร้าง
- **Deploy 20 พ.ย. 13:06Z (`50f55e2`) ผลทดสอบในเกม: NPC ยังไม่แสดง**
  - trace ยืนยันว่า server ส่ง `0x0BE3` ครบ 50 ตัว (flag 1, payload 5601 ไบต์) ทุกครั้งที่เข้าเมือง ข้อมูล entry ถูกต้อง
  - แต่ส่งหลัง `0x0BC5 → 0x0BC6` คือหลังโหลดแผนที่เสร็จ client ไม่มี log ตอนทิ้ง
- **สิ่งที่พบใน client (static RE, image b4580ed4):**
  - `0x0BC0` (`008CB980`) และ `0x0BC3` แบบเปลี่ยนแผนที่ (`008EF110` case 0xBC3) ตั้ง hold flag `[0179FF50]+0x48 = 1` event ของโลกทั้งหมดจึงไปรอใน list hold (`+0x24`)
  - `C9StageMapLoad` vfunc 11 (`00C9A8C0`) ตอน state 3 ดึง event สร้าง object (`0x16`) จาก hold ไปให้ loader thread (`00C9B9B0`, loop `00C9C352`) สร้าง object และโหลด visual ของตัวที่อยู่ในระยะทันที ถ้าอยู่ใน instance (type 1/3/4/6) จะส่งไปเฉพาะ object type 2 (NPC) แสดงว่า client ออกแบบให้ NPC มาถึงระหว่างโหลด
  - loader ส่ง `0x0BC4` (`00C9C71A`) หลังขั้นนี้ ส่วน `C9StageGame` vfunc 1 (`00C54320`) ย้าย hold ที่เหลือเข้า queue หลัก ตั้ง flag = 0 แล้วส่ง `0x0BC5`
  - ทางสร้างจริงทั้งสองทาง: `008464B0` → `00848A20` → factory `0083C5C0` (ต้องมี info id ใน `[0179FFD4]+0x74` มิฉะนั้นคืน 0 เงียบ ๆ)
- **แก้ (21 พ.ย.):** ส่ง `0x0BE3` ทันทีหลัง `0x0BC3` (ระหว่างโหลด) แทนหลัง `0x0BC6` ส่งครั้งเดียวต่อการเลือกตัวละคร test `MemoryNpcSpawn` ปรับตาม (C9.Validation 78/78 full ใน cloud)
- **ถ้ายังไม่แสดง:** ให้ผู้ใช้ทำ memory dump ตอนยืนใน Waterford แล้วรัน `npc_dump_probe.py <C9.DMP> config/npc-spawns.json` (อยู่ใน `/mnt/project-files/second-password/merge-npc3/` ไม่อยู่ใน git) จะบอกว่า event ค้างใน queue ไหน object NPC ถูกสร้างหรือไม่ และ info id ใดไม่มีในตาราง template
- รายละเอียดไฟล์และ packet เดิมของขั้นที่ 2:
  - `config/npc-spawns.json` มี NPC ของ Waterford 50 ตัว (map 10, sub-map 0)
  - compose ส่ง `--npc-spawn-file /c9/app/config/npc-spawns.json` ให้ c9-lobby และ Dockerfile คัดลอกไฟล์นี้เข้า image
  - `npc_spawn_file` ใน profile ใช้ได้ ถ้าใช้ path แบบ relative จะอ้างจากไฟล์ profile ที่โหลด แต่ `--npc-spawn-file` บน command line ชนะเสมอ
  - +0x2C ของ entry เป็น up vector `(0, 1, 0)`
- **ยังต้องทดสอบในเกม:** NPC ขึ้นตรงตำแหน่งไหม หันถูกทางไหม คลิกคุยได้ไหม

## ที่มาของรายชื่อ NPC

- ไฟล์ `C9_1th_Town_Waterford_VILLAGE.C9MM` ใน `MapData/C9_1th_Town_Waterford/C9_1th_Town_Waterford.cfs` ของ client (ไม่ได้ copy เข้า repo)
  - Ref server 2014 โหลดไฟล์เดียวกัน (log: `MapLoad = C9_1th_Town_Waterford_VILLAGE.C9MM` แล้ว `mapKey:10`)
- **CFS รุ่น 2026:**
  - header 24 ไบต์ จำนวน entry ที่ offset 20
  - ชื่อไฟล์ XOR 0x3A
  - ข้อมูล LZMA มี junk prefix 0–9 ไบต์ที่ไม่นับรวมใน packed_size
  - `re/scripts/c9_cfs.py` เดิมอ่านรุ่นนี้ไม่ได้
- **C9MM v54 (little-endian):**
  - header: u32 version = 54, u32 0, u8 0, u32 count (73)
  - **section 1** = ตารางวาง NPC/object: 73 record ขนาดคงที่ 44 ไบต์

| offset | ชนิด | ความหมาย |
|---|---|---|
| +0x00 | u32 | NPC info id (template) |
| +0x04 | u32 | spawn id (ไม่ซ้ำ; บทพูดใน `.C9LW` อ้าง id นี้) |
| +0x08 | u32 | 0 |
| +0x0C | f32×3 | x, y (ความสูง), z |
| +0x18 | f32 | yaw เป็น radian (ไม่ได้ normalize เช่น 14.0832 = 7.8 + 2π) |
| +0x1C | u32 | 0x2713 ทุก record |
| +0x20 | f32×3 | unit vector ((0,1,0) ใน 42/73) |

  - **section 2:** จุดที่มีชื่อ (CP949) เช่น `리스폰위치N` 47 จุด และจุดย้ายเมือง ไม่ใช่ NPC
  - **section 3:** trigger/event ยังไม่ได้ parse
- **วิธีแยก NPC:**
  - info id ทั้ง 67 ค่ามีชื่อในตารางชื่อ NPC ของ client (`ENG_GameData.c9t`) และ 63 ค่ามีใน GM list ของ Ref (`怪物NPC.txt`)
  - คีย์บทพูด `1th_<npcId>_..` ใน `.C9LW` ชี้ไปที่ spawn id ของ NPC ตัวเดียวกัน
  - เลือกเฉพาะ NPC 50 ตัว และยังไม่ใส่ object 23 ชิ้น (ตู้จดหมาย, เครื่องมือช่าง, กระดานภารกิจ, ทางเข้า stage/portal, ต้นซากุระ, Hieronymus Tower ของ event) เพราะยังไม่รู้ว่าต้องส่งผ่าน `0x0BE3` ด้วยหรือไม่
  - ข้อมูลดิบทั้ง 73 record: `/mnt/project-files/second-password/re/waterford_c9mm_placements.json`; ผลวิเคราะห์บนเครื่องผู้ใช้: `artifacts/claude-sync/npc/analysis/` (ไม่อยู่ใน git)
  - `_TUTORIAL.C9MM` (190 record) และ `_Event.C9MM` (48 record) ใช้ section 1 แบบเดียวกัน แต่ยังไม่ได้ใช้

## Packet (client b4580ed4, ยืนยันกับ Ref catalog 2014)

- dispatcher `C9NetworkCharacterMove` 0x008CE070

| opcode | Ref id | flag | payload | ความหมาย |
|---|---|---|---|---|
| `0x0BE2` | 3042 | 0 | u8 count + 0x270 ไบต์ต่อ entry → `0x008CE7B0` | ผู้เล่นอื่น (Exist_PC) |
| `0x0BE3` | 3043 | 1 | u8 count + 0x70 ไบต์ต่อ entry → `0x008CEAB0` | NPC (Exist_NPC) |
| `0x0BE4` | 3044 | 0 | u8 count + u64 object id | ลบ object |

- flag มาจากตาราง registration ของ client และตรงกับ Ref catalog
- **entry ของ 0x0BE3 (112 ไบต์):** `0x008CEAB0` สร้าง event 0x16 ผ่าน `0x007E0FA0` ด้วย object type 2 (PC = 1)

| offset | ชนิด | ค่าที่ server ส่ง |
|---|---|---|
| +0x00, +0x04 | u32 | 0 (→ event +0x50, +0x38; ยังไม่รู้ความหมาย) |
| +0x08 | u64 | object id = `0x4E50_0000_0000_0000 | map<<24 | submap<<16 | ลำดับ` (ข้ามถ้าตรงกับ id ผู้เล่นเอง) |
| +0x10, +0x14 | u32 | HP, MaxHP = 1 (HP 0 ทำให้ client ถือว่าตาย) |
| +0x18 | u32 | NPC info id (client ค้นในตาราง `[0x179FFD4]+0x74`) |
| +0x1C | f32×3 | ตำแหน่ง |
| +0x28 | f32 | yaw |
| +0x2C | f32×3 | up vector: client ใส่ `(0, 1.0, 0)` ให้ผู้เล่นเองใน `0x008CE7B0` (ค่าคงที่ 1.0 ที่ `0x00FC272C`) |
| +0x38..+0x6F | | 0 (32 ไบต์ที่ +0x50 ถูกคัดลอกเข้า record ต่อ object ที่ `0x008CF4D0`) |

- ลำดับ info id → ตำแหน่ง → yaw → vector ใน entry ตรงกับ record ของ C9MM section 1 จึงเชื่อว่าเป็นข้อมูลชุดเดียวกัน

## รูปแบบไฟล์ `npc_spawn_file`

```json
{ "maps": [ { "map_id": 10, "submap": 0, "npcs": [
  { "info_id": 128, "x": 38496.957, "y": 3501.44922, "z": 35871.8672, "yaw": 14.0832071, "name": "Blacksmith Abramo" }
] } ] }
```

- ไม่บังคับ: `hp` (ค่าเริ่มต้น 1), `up: [x, y, z]` (ค่าเริ่มต้น `[0, 1, 0]`)
- key อื่นอย่าง `name`, `c9mm` เป็นแค่หมายเหตุ
- ถ้าไม่มีไฟล์ server จะไม่ส่ง NPC เลย (เหมือนพฤติกรรมเดิม)
- ไฟล์ผิดรูปแบบหรือหาไม่เจอ lobby จะไม่เริ่มทำงาน (fail fast)

## ขั้นต่อไป

1. รวมชุด `apply_npc_waterford.py` บน branch ล่าสุดที่ deploy อยู่ (Sprint 1 ของเธรดระบบพื้นฐาน) แจ้งเธรด status/item ก่อน commit แล้ว deploy
2. ผู้ใช้ทดสอบที่ Waterford: NPC ขึ้นไหม ตำแหน่งและทิศถูกไหม คลิกคุยได้ไหม
3. ถ้าคลิกแล้วไม่มีอะไรเกิดขึ้น ต่อด้วยระบบคุย NPC (`PS_NPCSpeech_Query_Req/Ack`) และบทพูดใน `Script.cfs` (`<id>.xmss`)
4. ทดลองใส่ object (ตู้จดหมาย ฯลฯ) และขยายไปเมืองอื่น (Vimpeli 19, Runnersvill 33) ด้วยวิธีเดียวกัน
