# Exit hang ("Transmiting to Server") + 2nd Password — Current State

## Current State 20 พ.ย. 2026 (ล่าสุด ใช้ส่วนนี้ก่อน)

อัปเดต 20 พ.ย. 2026 ~09:15 น. (Asia/Bangkok) โดย thread "แก้ NPC และ 2nd password ในเกม" มาตรฐาน ISO/IEC 27001, ISO 9001, ISO/IEC 29110

- **2nd Password ในเกม implement แล้ว** บน branch `claude/second-password-npc` (ต่อจาก `claude/project-thread-wk3q2i` @ `d407e60`)
  - C9.Validation **73/73** (7,306 assertions, mode full, รวม PostgreSQL 16 จริงผ่าน `C9_DB_ADMIN_CONNECTION`) บนฐาน branch status
  - migration `20261120020414_AccountSecondPassword` ต่อจาก `20261119162447_StatUpgradePoints`; snapshot มีทั้งสองงาน; `db/migrations/game-data.sql` สร้างใหม่ (idempotent)
  - รอผู้ใช้ทดสอบในเกม
- **ตาราง** `game.account_second_passwords` (PK/FK `account_id`): `password_hash`, `answer_hash` = PBKDF2-SHA256 210,000 รอบ (ไม่เก็บค่าดิบ), `question` 0–9, `fail_count` 0–5 (check constraint), `locked_at`, xmin
- **กฎ** (`SecondPasswordPolicy`): ผิด 5 ครั้ง = ล็อก 72 ชม. (ตรงกับ client FailCntMax และ 259,200 วินาที); สำเร็จแล้วนับใหม่; คำตอบไม่สนตัวพิมพ์และช่องว่างหัวท้าย ยาวไม่เกิน 25 ตัว
- **Protocol** (client b4580ed4):
  - รับ `0x2744` set (63 B), `0x2746` reset (59 B), `0x2748` change (16 B), `0x274A` certify (4 B) ในสถานะ character_lobby, world_loading, world_ready
  - ตอบ `0x2745/0x2747/0x2749` (flag 0) และ `0x274B` (flag 1) ตามตาราง registration ของ client
  - ถ้าผิดส่ง `0x0001` (28 B, popup) error code = CRC32(UTF-16LE) ของชื่อ string: INVALID_2NDPASSWORD_1 `0xC22AA395`, INVALID_2NDPASSWORD_2 `0xE907F056`, LOCKED_2NDPASSWORD `0xE853BEC9`, INVALID_2NDPASSWORD_Q_AND_A `0x48E08822`
  - `0x0BC0` +0x7234 ตั้งแล้ว, +0x7235 จำนวนครั้งที่ผิด, +0x7236 เวลาเริ่มล็อก (unix วินาที), +0x723F ยืนยันแล้วใน session นี้
- **Audit:** `account.second_password.set/change/reset/clear/lock` (ไม่มี hash หรือคำตอบ); trace ใหม่ไม่เก็บ payload ของ 2744/2746/2748/274A (Sensitive)
- **ข้อจำกัด:** client ส่งรหัสเป็น CRC32 มีได้เพียง 2^32 ค่า ถ้า DB รั่ว hash ยังเดาได้ด้วยเครื่องแรงพอ (ข้อจำกัดของ protocol)
- **ต้องตัดสินใจ:** trace เก่า `artifacts/docker-state/trace/lobby/20261115/{7fa510,af6f01}-001.jsonl` มีคำตอบ 2nd password แบบข้อความธรรมดาและ CRC (ยังไม่แตะไฟล์ รอผู้ใช้สั่งลบหรือ redact)
- **ขั้นทดสอบในเกม:** ตั้งรหัสในเมือง (หน้าต่างต้องไม่ค้าง WAITING) → ยืนยันด้วยรหัสผิด 1 ครั้ง (ขึ้นผิด 1/5) → ยืนยันด้วยรหัสถูก → ออกแล้วเข้าใหม่ เกมต้องจำว่ามีรหัสแล้ว → (ถ้าต้องการ) เปลี่ยนรหัส / ล้างด้วยคำตอบ
- NPC ในเมือง: ดู [NPC_SPAWN_TH.md](NPC_SPAWN_TH.md)

## ผลตรวจยืนยันเพิ่มเติม 16 พ.ย. 2026

- ผู้ใช้ยืนยันแล้วว่า Quit Game ใช้งานได้หลังแก้ `0x17D4 -> 0x17D5`; ยังต้องแก้ **กลับหน้าเลือกตัวละคร** ซึ่งเป็นคนละ flow (`0x0EC4 -> 0x2EEB -> reconnect 0x2EE8/0x2EE9`)
- ถอนข้อสรุปเดิมว่า Ref Server 2014 ไม่มี opcode ที่เกี่ยวข้อง: catalog มี `0x2744..0x274B` แต่ต้องตรวจความหมายทีละตัว เพราะหมายเลขอาจใช้ต่างกันระหว่าง build
- client ปัจจุบันมี dispatcher `C9Network2ndPassword` ที่ `0x0089B880`: รับ `2745/2747/2749/274B`; sender `0089C060/0089C130/0089C260/0089C1E0` ส่ง `2744/2746/2748/274A` ตามลำดับ การยืนยัน layout และ error flow ยังดำเนินอยู่
- Register/Login จริงและ account binding ทำแล้วตาม `LAUNCHER_ACCOUNTS_TH.md`; ยังไม่ได้ implement การจัดเก็บ/ตรวจ 2nd Password และยังไม่อ้างว่ารองรับแล้ว

รายละเอียดเก่าด้านล่างเป็นประวัติ; ใช้ผลตรวจส่วนนี้หากมีข้อความขัดกัน

อัปเดต 16 พฤศจิกายน 2026 (Asia/Bangkok) โดย thread "เพิ่ม 2nd password และแก้ exit hang"
มาตรฐานอ้างอิง ISO/IEC 27001, ISO 9001, ISO/IEC 29110

## 1. Exit hang — แก้แล้ว (รอ rebuild + ยืนยันในเกม)

อาการ: กดออกเกมตอนอยู่ในเมือง แล้วค้างที่ข้อความ "Transmiting to Server"

สาเหตุ (ยืนยัน 2 ทาง):
- Trace จริง (`artifacts/docker-state/trace/lobby/20261115/{af6f01,7fa510}-001.jsonl`): ตอนอยู่ในโลก
  client ส่ง packet `0x17D4` (logout, flag=1, payload 16 ไบต์ ขึ้นต้น 0x01) แต่ server ในสถานะ
  world_loading/world_ready "สังเกตเฉย ๆ ไม่ตอบ" → client รอ ack ไม่มา
- Decompile client (`FUN_00a47cb0`): เมื่อรับ type `0x17D5` client เรียก `PostQuitMessage(0)` ปิดเกมทันที
  โดยไม่อ่าน payload
- Ref server 2014: handler `0x17D4` (VA 0040C0F0) ตอบ `0x17D5` (+`0x0F08`), query `PSQueryCharacterLogout`

แก้ (ยังไม่ commit):
- `src/C9.Server/SessionProcessor.cs`: เพิ่ม branch — สถานะ world_loading/world_ready + รับ `0x17D4`
  → ส่ง `0x17D5` (encrypted ตาม catalog enc=1; payload ว่างเพราะ client ไม่อ่าน) + emit event `logout_ack`
- `src/C9.Contracts/KnownPackets.cs`: +`0x17D4` LogoutReq, +`0x17D5` LogoutAck
- `tests/C9.Validation/WorldStartupCases.cs`: เพิ่ม assertion (ส่ง 0x17D4 → รับ 0x17D5 flag=1) ทั้ง Legacy/Extended
- **ไม่แตะ login flow** (`0x2F04`/`0x2EF6`/`0x2EF7`)/account binding — ไม่ชนกับ thread launcher login

ตรวจ: cloud `dotnet run C9.Validation` = **62/62 ผ่าน** (6,697 assertions, mode full)
Deploy: ยังไม่ rebuild — `scripts\docker\Start-C9Platform.ps1 -GameData` จะ deploy fix นี้ + `0x0BC6`
world-startup (ของอีก provider ที่ยัง uncommitted) พร้อมกัน แล้วกด Exit ควรปิดเกมได้สะอาด

## 2. 2nd Password — กำลังทำ (RE/ออกแบบ ยังไม่เขียนโค้ด)

- เป็นระบบ secondary password ของ client จริง: `PS_Certify_2ndPassword_Req`, UI `C9UISecondaryPasswordPanel`/
  `C9Network2ndPassword`, error จาก server: `INVALID_2NDPASSWORD_1/2`, `LOCKED`, `Q_AND_A`
- เป็นฟีเจอร์ยุคใหม่: ref server 2014 **ไม่มี** handler นี้ → ต้อง RE จาก client ปัจจุบัน
  (opcode/layout ยังไม่ได้ เพราะ dump รอบนี้ capture code page ที่อ้างอิง string ไม่ครบ)
- อยู่เฟส certify/login (กลุ่มเดียวกับ `PS_Certify_Req`/`ServerList`/`Select_Server`) → ทับงาน thread
  launcher login (0x2F04 redeem ticket + bind account, 0x2EF6/0x2EF7)
- แผนที่ตกลงกับ thread launcher: เขาทำ bind account ก่อน; thread นี้ทำ challenge/verify 2nd password
  ต่อ "หลัง" bind (ต้องใช้ account identity) และจะแจ้งไฟล์/สถานะที่จะแก้ก่อนลงมือใน login flow

## 3. หลักฐาน/เครื่องมือ
- Catalog ref 2014: `re/evidence/legacy_patch65/packet_catalog_2014.json` (0x17D4 enc1, 0x17D5 enc1)
- Trace: `artifacts/docker-state/trace/lobby/20261115/{af6f01,7fa510}-001.jsonl` (มี frame 0x17D4)
- Client RE (cloud): `/mnt/project-files/c9-world/re/` + scratchpad

## Current State (ให้โมเดลอื่นมาต่อ)
- Exit hang: fix อยู่ในโค้ด (uncommitted) + C9.Validation 62/62 → เหลือ rebuild + ยืนยันในเกม
- 2nd Password: ยังไม่เขียนโค้ด — ขั้นต่อไปหา opcode จาก client เต็ม (`re/scripts/legacy/client_catalog.py`)
  + ประสาน thread launcher login
