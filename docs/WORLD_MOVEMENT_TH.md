# การเดินของตัวละครในเกม (0x0C1E move speed) — Current State

## ✅ ยืนยันในเกมแล้ว (19 พ.ย. 2026 22:23 น. Asia/Bangkok) — เดินด้วย W/A/S/D และกระโดดด้วย Space ได้

ส่วนนี้ใหม่ที่สุด (thread "เพิ่ม 2nd password และแก้ exit hang"). ปิดเรื่อง keyboard เดิน/กระโดดไม่ได้

**ผลทดสอบ:** ผู้ใช้ยืนยัน 19 พ.ย. 15:23Z ว่าเดินด้วย `W` `S` `A` `D` และกระโดดด้วย `Space` ได้แล้ว

**Deploy ที่ทดสอบ:** image `c9-platform:local` id `065100ff62e7` build แบบ `--no-cache` เมื่อ 19 พ.ย. 16:29 น. (`docker build -f docker/Dockerfile -t c9-platform:local --no-cache`) แล้ว deploy ด้วย `scripts\docker\Start-C9Platform.ps1 -GameData -NoBuild`. container healthy: lobby (127.0.0.1:14001), login (14000), auth (14890), portal (14880), db (5432). overlay `artifacts/docker-state/portal/profiles/lab-official-character-lobby.json` มี `max_frame` = 40960. หมายเหตุ: lobby ยังรับได้ทีละ 1 connection (`max_connections` = 1)

**หลักฐาน trace:** `artifacts/docker-state/trace/lobby/20261119/d9c138-001.jsonl` session `lobby-d9c138-3`
- client ส่ง `0x0BCE` 62 ตัว (15:17:54Z → 15:19:18Z) แยกตามท่าที่ payload +16: `Move_Front` (`0xBC65518B`) 53 ตัว, `WAIT` (`0x4B7B6E51`) 6, `JUMP_UP` 1, `0xD697B24A` 2 (ยังไม่รู้ชื่อ)
- **ไม่มี `MOUSE_RUN` เลย** ⇒ การเดินมาจากคีย์บอร์ดจริง (ต่างจากรอบ 16 พ.ย. ที่ 0x0BCE เงียบหลังเกิด)
- session ไม่หลุดระหว่างเล่น (`0x270F` heartbeat 94 ตัว)
- วิธีหาชื่อท่า: CRC32 ของ string ใน image `C9.exe` (ลองทั้งตัวพิมพ์เดิมและตัวพิมพ์ใหญ่) ⇒ `0xBC65518B` = CRC32("MOVE_FRONT")

### สาเหตุ (สรุปสำหรับคนมาต่อ)
- **ก่อนแก้:** server ส่ง HP/MP ใน `0x0BC0` (select-ack) แค่ที่ payload 24/28 (ค่าที่ HUD ใช้) แต่ตอนสร้างตัวผู้เล่น client อ่าน "HP ปัจจุบันของ actor" จาก payload **32/36** (Player +0x28/+0x2C) ซึ่งเป็น 0 ⇒ `0x00C9C00A` ตั้ง flag ตาย (actor +0x790 = 1) ⇒ ตัวรวมคำสั่ง input `0x00823090` ตัดคำสั่งเดิน/กระโดดทั้งหมดทิ้ง
- ปุ่ม UI (`I` `K` `M` `C` `Esc`) และการคลิกเมาส์เดิน (`MOUSE_RUN`, `0x00824054`) ไม่เช็ก flag นี้ จึงยังใช้ได้ ทำให้ดูเหมือน keyboard เสีย. `0x0C20` เติม HP ทีหลังแต่ไม่ล้าง flag
- **สิ่งที่ทำให้ใช้ได้:** `CharacterLobby.SelectAsync` เขียน HP/MP ปัจจุบันที่ payload 32/36 ของ `0x0BC0`, ขยาย `0x0BC0` เป็น `0x83C6` ไบต์ (ตามตัวอ่าน client `008824D0`) และ `ServerOptions` ตั้ง `max_frame` default 40960 พร้อมบังคับให้ profile `character_lobby` มีค่า ≥ 33,748 (ขนาด frame จริงของ 0x0BC0) กันอาการ "Server connection lost"
- **สมมติฐานที่ตัดทิ้งแล้ว:** gate ที่ +0x118, key binding ว่าง, ค่า move speed ใน 0x0C1E (แก้ไว้ตั้งแต่ 15 พ.ย. แต่ไม่ใช่ต้นเหตุเรื่อง keyboard), การส่ง 0x0BC6 ซ้ำ

### สถานะ / งานต่อ
- source ยังไม่ commit: `src/C9.Server/CharacterLobby.cs`, `src/C9.Server/ServerOptions.cs`, tests 4 ไฟล์ (`CharacterLobbyCases.cs`, `Program.cs`, `GameDataLobbyCases.cs`, `MemorySessionCases.cs`). ไฟล์ก่อนแก้อยู่ที่ `artifacts/claude-sync/backup-20261119/`
- งานต่อ: NPC spawn (`C9NetworkNpc` 0x008F8FA0), 2nd password ในเกม (client ส่ง 0x2744), position reset `0x0E44` หลัง 0x0BC5 แบบ server 2014, server ยังไม่เก็บ/broadcast 0x0BCE, ชื่อท่า `0xD697B24A`

## 🟢 ROOT CAUSE keyboard (19 พ.ย. 2026 ~16:40 น.; ยืนยันในเกมแล้ว 22:23 น.) — client คิดว่าตัวละคร "ตาย" เพราะ 0x0BC0 ส่ง HP ปัจจุบัน = 0

รายละเอียดกลไก (thread "เพิ่ม 2nd password และแก้ exit hang"). ใช้แทนข้อสันนิษฐานเรื่อง keyboard ด้านล่างทั้งหมด

**ผลทดสอบผู้ใช้ 19 พ.ย.:** ปุ่ม UI (`I` `K` `M` `C` `Esc`) ใช้ได้ แต่ `Space` (กระโดด) และ `W`/ลูกศร (เดิน) ใช้ไม่ได้ ⇒ ระบบรับปุ่มทำงาน ตัวปิดอยู่ที่คำสั่ง "การเคลื่อนที่" เท่านั้น

**กลไกที่ถอดได้ (client b4580ed4, ยืนยันกับ `artifacts/claude-sync/C9.DMP` 16 พ.ย. 10:50):**
1. `0x00823090` = ตัวรวม bitmask คำสั่งจาก `C9DXInput` (`[0x017A008C]`, vtable+0x54 = IsCommand(cmd, mode)) ให้ผู้เล่น ก่อนอ่านกลุ่มคำสั่งเดิน (cmd 0–3 + NumLock auto-run) และกลุ่มถัดไป จะเช็ก **actor+0x790** (byte แบบเข้ารหัส: `byte[slot] ^ ((ptr+idx)>>1)`) ถ้า ≠ 0 จะข้ามคำสั่งเดิน/กระโดดทั้งหมด
2. actor+0x790 = **flag "ตาย"** ผู้เขียนมี 4 จุด: `0x00C9C00A` (ตอนสร้างตัวผู้เล่นเอง: `SetDead(actor, HP <= 0)`), `0x00C8462A` (ตอนตัวละครอื่นปรากฏ: HP ≤ 0), `0x007E610A` (ขั้นตอนตาย = 1), `0x0083B3AC` (ฟื้น = 0)
3. ตอนสร้างตัวผู้เล่น `0x00C9B9B0` → `0x00882AF0` → `0x00882B50` คัดลอก **Player +0x28 → actor HP (+0x844)** และ Player +0x2C → actor +0x85C แล้ว `0x007DECB0` อ่าน HP มาตั้ง flag ตาย. Player +0x28/+0x2C มาจาก **payload 0x0BC0 ตำแหน่ง 32/36** (payload +8 ↔ Player +0x10)
4. server เดิมเขียน HP/MP แค่ที่ payload 24/28 (Player +0x20/+0x24 ที่ HUD ใช้) ส่วน 32/36 เป็น 0 (test เดิมยัง assert ว่าต้องเป็น 0) ⇒ ตอนเกิด HP = 0 ⇒ flag ตาย = 1 ⇒ คีย์บอร์ดเดิน/กระโดดไม่ได้. 0x0C20 มาเติม HP เป็น 100 ทีหลังแต่ไม่ล้าง flag. คลิกเมาส์เดิน (`MOUSE_RUN`, `0x00824054`) ไม่เช็ก flag นี้ จึงเดินได้
5. ค่าใน dump: actor `0x396FF008` flag ตาย = **1**, HP (+0x844) = 100 (หลัง 0x0C20), ท่า `WAIT`

**แก้ใน source (uncommitted):**
- `src/C9.Server/CharacterLobby.cs`: เขียน HP/MP ปัจจุบันที่ payload 32/36 ของ 0x0BC0 (= `initial_hp`/`initial_mp`), ใช้ค่าคงที่ `SelectAckBytes = 0x83C6`
- `src/C9.Server/ServerOptions.cs`: ค่า default `max_frame` = 40960 และ profile `character_lobby` ต้องมี max_frame ≥ ขนาด frame จริงของ 0x0BC0 (33,748 ไบต์) แทนเลข 7460 เดิม ⇒ กัน defect "Server connection lost" เกิดซ้ำถ้ามีคนลืมตั้ง config
- tests: `CharacterLobbyCases` ตรวจขนาด 0x83C6 และ HP/MP ที่ 32/36; ตัวอ่าน frame ฝั่ง test ขยายเป็น 65535 (`Program.cs`, `GameDataLobbyCases.cs`, `MemorySessionCases.cs`)
- ผลตรวจ (cloud, .NET 10.0.115): build ผ่าน, offline 46/46 (6,329 assertions), **full 65/65 (6,761 assertions)**. สำรองไฟล์ก่อนแก้ที่ `artifacts/claude-sync/backup-20261119/`

**ขั้นทดสอบ:** หลัง deploy เข้าเมืองแล้วกด `W`/ลูกศร และ `Space` — ควรเดิน/กระโดดได้ ใน trace ต้องเห็น 0x0BCE ที่มีท่า `RUN` (`0xC6D1E64A`) หรือ `WALK*` แทน `MOUSE_RUN`

## 🔴 ผลทดสอบรอบ 3 (16 พ.ย. 2026 14:57Z / 21:57 น.) — 0x0BC0 33KB เข้าเกมได้ แต่ keyboard ยังเดินไม่ได้

บันทึก 19 พ.ย. 2026 (thread "เพิ่ม 2nd password และแก้ exit hang"). ส่วนนี้ใหม่ที่สุด ใช้แทนข้อสันนิษฐานด้านล่างที่ขัดกัน

**หลักฐาน:** trace `artifacts/docker-state/trace/lobby/20261116/933830-001.jsonl` session `lobby-933830-3` (account_id 4, character 2, map 10)

- หลังยก `max_frame=40960` (ดูหัวข้อ DEFECT ด้านล่าง) server ส่ง `0x0BC0` ขนาด **33,734 ไบต์ (flag 1)** สำเร็จที่ 14:57:11 → client โหลดโลกต่อ ส่ง 0x0BC5 → server ตอบ 0x0C1E/0x0C20/0x0BC6 → `world_ready` 14:57:30 **ไม่หลุด** ⇒ defect MaxFrame ปิดแล้ว และ client รับ 0x0BC0 ขนาดนี้ได้
- ผู้ใช้ยืนยัน **keyboard ยังเดินไม่ได้**
- `0x0BCE` (46B) จาก client: 2 ตัวตอนเกิด (14:57:31–32, ท่า idle hash `0x4B7B6E51`) แล้ว **เงียบ 78 วินาที (14:57:32 → 14:58:50) ไม่มี 0x0BCE เลยแม้แต่ตัวเดียว** ทั้งที่ผู้ใช้กดคีย์บอร์ด
- 14:58:50 → 14:59:10 มีการเดินต่อเนื่อง 19 วินาที (ท่า hash `0xF8736B63`) ทิศเปลี่ยนเป็นขั้นที่ waypoint (r 0.95→1.56→2.03→3.22) ความเร็ว ~700 หน่วย/วินาที = **การเดินแบบคลิก/นำทาง** แล้วกลับ idle
- layout 0x0BCE ที่อ่านได้: +0 x,y,z (float), +12 heading (float rad), +16 action hash (u32), +38 flag 2 ไบต์ (`0101` ตอนหยุด), +42 client tick (u32)
- packet อื่นที่ server ยังไม่ตอบ: `0x2744` flag 0 (63B: 8 ไบต์ศูนย์, u32 `0x0FCD41FD`, ไบต์ `0x04`, ข้อความ UTF-16 "birthday") เวลา 14:58:19 และ `0x0BE8` (ว่าง) 14:59:18 — ยังไม่รู้ความหมาย ห้ามเดา
- ออกเกม 0x17D4 → 0x17D5 ทำงานถูก (14:59:37)

**ข้อสรุป:**
1. การกดคีย์บอร์ด **ไม่สร้าง packet การเดินออกมาเลย** ⇒ ตัวปิดกั้นอยู่ **ฝั่ง client ก่อนถึงระบบเคลื่อนที่** (input → action) ไม่ใช่เรื่องความเร็ว/animation และไม่ใช่เพราะ server ไม่ตอบ 0x0BCE
2. การขยาย 0x0BC0 เป็น 33,734 ไบต์แบบ **เติมศูนย์** ไม่แก้ keyboard. ถ้าข้อมูลที่ client ต้องใช้ (เช่น key binding / shortcut / option) อยู่ใน 0x0BC0 หรือ packet อื่น การส่งศูนย์ก็ยังเท่ากับ "ไม่มีปุ่มผูกไว้" ต้องหาว่า client อ่านค่า key/input จากไหน
3. คงขนาด 33,734 ไว้ (ไม่ทำให้หลุด และกัน client copy ขยะ OOB) จนกว่าจะยืนยัน reader ของ 0x0BC0

**งานถัดไป (ไม่ต้องเปิดเกม):** RE client หา keyboard input path (key binding table, ตัวที่โหลดค่า default, เงื่อนไขที่ปิดรับ input) และเทียบ packet ที่ Ref 2014 ส่งหลัง 0x0BC5 ที่เรายังไม่ส่ง (0x07D1, 0x0E44, 0xEADA, 0xEAFD, 0xEAF6; 0xEAFE/0x2335 = Guild Battle ตัดออกแล้ว)

### RE ต่อ (19 พ.ย. 2026, ไม่ต้องเปิดเกม) — ตัดสมมติฐาน key binding ว่าง + ถอดชื่อท่า

- **ชื่อท่าใน 0x0BCE = CRC32 ของชื่อตัวพิมพ์ใหญ่** (ยืนยัน: `crc32("WAIT")=0x4B7B6E51`). ท่าที่ client ส่งตอนเดินได้คือ `0xF8736B63 = "MOUSE_RUN"` ⇒ ที่เดินได้คือ **คลิกเมาส์เดิน** ไม่ใช่ auto-navigation. ท่าของคีย์บอร์ดคือ `RUN`/`WALKLEFT`/`WALKRIGHT`/`WALKBACK` (+`Move_Front/Left/Right/Back`) ซึ่งไม่เคยโผล่ใน trace เลย
- ตาราง hash ท่าอยู่ที่ global `0x017A60D8..` (WAIT, …, RUN=`0x017A6120`, WALKLEFT/RIGHT/BACK=`+0x24/+0x28/+0x2C`, MOUSE_RUN=`0x017A6130`, Move_*=`0x017A6134..40`). โค้ดคลิกเดิน `0x00824054` เขียน actor+0x1178 = MOUSE_RUN, +0x119C = 5
- **KeyConfig (key binding) ไม่ว่าง — ตัดสมมติฐานนี้ทิ้ง:** ใน `artifacts/claude-sync/C9.DMP` (16 พ.ย. 10:50, ตอนคีย์บอร์ดเดินไม่ได้) พบ object `C9KeyConfig` (vtable `0x00FD2F58`) 3 ชุด (`0x05E9D370`, `0x05EA9FCC` = สมาชิก `C9DXInput+0xFE4`, `0x370C8B40`) ทุกชุดมีปุ่มผูกไว้ 49 จาก 81 คำสั่ง ⇒ ปุ่มไม่ได้หาย. (ไฟล์ `%s\%s\KeyConfig.INI` โหลดโดย `0x008961D0`; ctor `0x0040F5E0` ล้างค่าก่อน)
- `C9DXInput` (vtable `0x00FD1FFC`, instance `0x05EA8FE8`) มี device ของ DirectInput ครบ; สถานะคำสั่งต่อปุ่มเก็บเป็น bit ที่ `+0x328` (keyboard), `+0xC53`, `+0xEE8` index = command id
- `C9NetworkCharacterMove` (dispatcher `0x008CE070`) รับ s2c: 0x0BCD, 0x0BCF, 0x0BE2, 0x0BE3, 0x0BE4, **0x0E44**, 0x0E45, 0x0F21, 0x2461+. **0x0E44 = position reset** (payload 25B: u64 objectId, x,y,z f32, heading f32, u8 flag) — Ref 2014 ส่งตัวนี้หลัง 0x0BC5 แต่เรายังไม่ส่ง. ยังไม่มีหลักฐานว่าเกี่ยวกับคีย์บอร์ด (ตัวที่มันเคลียร์ `[0x017A00A8]+0x13AC` เป็น flag รอตอบตำแหน่ง ไม่ใช่ input gate)

- ค่าจาก dump เดียวกัน (ตอนยืนเฉย): ผู้เล่น `[0x017A00B0]=0x396FF008` (`C9ControlManagerPC`; update = vtable[3] `0x0081D110`), byte `+0x1440 = 0` (ไม่ได้ถูกปิด update), action state `+0x360 → 0x2A555B90` ท่าปัจจุบัน `WAIT` (+0xC0/+0xC4), scene `+0x344 → RCharacterScene2` flags `+0x338 = 0x1800051E`; `C9DXInput+0x328` (สถานะคำสั่งคีย์บอร์ด) ว่างทั้งหมดเพราะไม่ได้กดปุ่มตอน dump ⇒ ต้องมี dump ขณะกด `W` ค้างไว้ หรือผลทดสอบปุ่ม hotkey ก่อนจะไล่ต่อได้
- `[0x0179E948]` = `C9UserOption` (ใช้สร้าง path `%s\%s\KeyConfig.INI`)

**ขั้นทดสอบถัดไป (ให้ผู้ใช้ทำเมื่อเปิดเกมได้, ~2 นาที ไม่ต้อง rebuild):** เข้าเมืองแล้วกด `I` (กระเป๋า), `K` (สกิล), `M` (แผนที่), `C` (ตัวละคร), `Esc`, `Space` (กระโดด) แล้วค่อยกด `W`/ลูกศร บอกว่าปุ่มไหนตอบสนอง
- ถ้าปุ่มเปิดหน้าต่างใช้ได้แต่ W ไม่เดิน ⇒ ระบบ input ทำงาน ตัวปิดอยู่ที่ตัวควบคุมการเดิน/ท่า (น่าจะขึ้นกับข้อมูลตัวละครจาก server)
- ถ้าไม่มีปุ่มไหนทำงานเลย ⇒ ปัญหาอยู่ที่ DirectInput/โฟกัสหน้าต่าง/การโหลดปุ่ม ไม่ใช่ server

**ผลกับ thread launcher:** ข้อ 8.1–8.2 ใน `LAUNCHER_ACCOUNTS_TH.md` (account 4 หลุดที่ 0x0BBF เพราะ "Encoded frame exceeds configured maximum") คือ defect MaxFrame ตัวเดียวกัน — แก้แล้ว 14:55Z และ session 933830-3 ของ account 4 เข้า `world_ready` ได้จริง

## ผลตรวจยืนยันล่าสุด 16 พ.ย. 2026 — ใช้ส่วนนี้แทนสมมติฐาน +0x118 ด้านล่าง

- **พบ blocking layout mismatch ของ `0BC0`:** server ส่ง 7,444 bytes แต่ current reader อ่าน payload อย่างน้อย 33,734 bytes ต้อง audit ทั้ง select/map startup ก่อนพัฒนา gameplay ต่อ ดู `PROTOCOL_BASELINE_TH.md`
- ตรวจ dump `artifacts/claude-sync/C9.DMP` เวลา 17:50 (ไม่ใช่ dump ในโฟลเดอร์ `world/`) ร่วมกับโค้ด client และ Ref Server 2014
- `FUN_007FCF20` เริ่มด้วยค่า float **1.0** จาก `0x00FC272C` และคืนค่านี้เมื่อ `[actor+0x360]+0x118 != 1` จึง **ไม่ใช่ gate ที่คืนความเร็ว 0** ตามข้อสันนิษฐานเดิม ตัวข้อมูลที่ `actor+0x360` มีชื่อ action `WAIT`; ยังไม่มีหลักฐานว่าเป็น input controller
- speed factors ของ player = 1000/1000 และ move/action multiplier ของ actor = 1.0 ทั้งคู่ ปัญหา Keyboard ยังไม่ทราบสาเหตุแน่นอน
- `0xEAFE` ถูก dispatch โดย **C9NetworkGuildBattle** (`0x008E0F70`) และ Ref `0x006A3A90` ส่งภายใต้เงื่อนไข Guild Battle; `0x2335` ก็มีเงื่อนไข จึงไม่เพิ่มสอง packet นี้เป็น world startup ทั่วไป
- ถอด workaround `0x0BCE -> 0x0BC6` ซ้ำแล้ว เพราะ trace ยืนยันว่าไม่ได้แก้ Keyboard และทำให้ validation เดิมผิดลำดับ คง `0x0BC5 -> 0x0BC6` ครั้งเดียว
- แก้ `0x0C20` ให้เป็น plain (`flag=0`) ตาม registration ใน client ปัจจุบัน ไม่ถือว่าเป็นหลักฐานว่าแก้ Keyboard ได้
- หลักฐาน decompile/assembly และ catalog ปัจจุบันอยู่ใน `artifacts/research/20261116/world-control/`
- ผลหลังถอด BC6 ซ้ำและแก้ flag 0C20: build ผ่าน; offline 46/46 (6,329 assertions); full TCP suite ติด Windows socket 10013 จึงยังอ้างว่าผ่านทั้งหมดไม่ได้

### ⛔ DEFECT ที่เจอรอบ deploy (16 พ.ย. 14:45) — 0x0BC0 33KB ชน MaxFrame

deploy build ที่ขยาย 0x0BC0 เป็น 33,734 ไบต์แล้ว ผู้ใช้กด Start Game → client ขึ้น **"Server connection lost" ทันที**. trace `db06f6-001.jsonl` ยืนยัน: หลัง `c2s 0x0BBF` (select) **server ไม่ได้ส่ง 0x0BC0 ออกไปเลย** และ session ปิดด้วย `close_reason="Encoded frame exceeds configured maximum"` → **server ตัด connection ตอน encode 0x0BC0 เพราะเกิน MaxFrame** (client ยังไม่ทันได้รับ packet)

**วิเคราะห์:** ทิศทาง 33KB น่าจะถูก (reader client อ่านถึง 33,734B) — 8192/MaxFrame เป็นเพดานฝั่ง server ของเราเอง ไม่ใช่ข้อจำกัด client. profile `character_lobby` บังคับ MaxFrame ≥ 7460 (ServerOptions.cs) และสูงสุด 65535. config ที่ lobby ใช้จริงคือ **overlay** `artifacts/docker-state/portal/profiles/lab-official-character-lobby.json` (ถูกใช้แทน baked config)

**แก้แล้ว (16 พ.ย. 14:55 น.):** ตั้ง `max_frame=40960` ทั้ง baked config `config/lab-official-character-lobby.json` และ overlay `artifacts/docker-state/portal/profiles/lab-official-character-lobby.json` (overlay คือตัวที่ lobby โหลดจริง) แล้ว **restart c9-lobby ผ่าน Remote Control** (ไม่ต้อง rebuild เพราะ ServerOptions อ่าน config ตอน startup). lobby กลับมา **Up/healthy** ฟังที่ 127.0.0.1:14001 ไม่มี error ตอน startup/config และยืนยันว่า overlay มี `"max_frame":40960`. session รอบก่อน (session 4, 14:44:11Z) ปิดด้วย `session_error "Encoded frame exceeds configured maximum"` หลังรับ type 3007 (0x0BBF) — ตรงกับอาการที่ max_frame bump นี้ต้องแก้

**การทดสอบชี้ขาดรอบนี้:** ผู้ใช้ login → เลือกตัวละคร → Start Game ควรเข้าได้ไม่ขึ้น connection lost แล้ว **เดิน keyboard อย่างเดียว**. ถ้ายังหลุดตอนรับ 0x0BC0 = client ไม่รับ frame ขนาดนี้สำหรับ 0x0BC0 จริง → reader 33KB ไม่ใช่ของ 0x0BC0 → revert `CharacterLobby.cs` กลับ `new byte[0x1D14]` (7,444) แล้ว rebuild

### บันทึกต่อ (thread exit/keyboard, 16 พ.ย. 14:40) — ตัวเลขชัด + experiment 0x0BC0

- ยืนยัน reader `0x008824D0` (client ปัจจุบัน) copy แบบ fixed-size (rep movsd) จาก decoded payload ถึง offset **+0x83C6 = 33,734 ไบต์ โดยไม่เช็กความยาว** ลงใน player object (`_DAT_017a00c8`) ที่ +0x1e48/+0x29d8/+0x31f0/+0x3a08 ... ถึง +0x83d3 → packet 7,444 ไบต์ทำให้ client copy ข้อมูล **out-of-bounds** เข้า block skill/action/item ของผู้เล่น (nav รอดเพราะ server เป็นคนขับการเดิน ไม่ผ่าน path นี้) = ผู้ต้องสงสัยหลักของ keyboard
- **ยังไม่แน่ใจ 100%:** comment เดิมใน `CharacterLobby.SelectAsync` อ้าง reader `0x00A397F8` (อ่านถึง 0x1D20); registration ของ opcode 0x0BC0 ชี้ class `0xf2a680`/`0xf32e30` แต่ resolve vtable แบบ static ไม่ได้ จึงยังไม่ฟันธงว่า 0x0BC0 ผูกกับ reader ตัวใหญ่หรือตัวเล็ก — การขยายขนาดปลอดภัยทั้งสองทาง (zero-pad ปลอดภัยกว่า garbage; ถ้าเป็น reader เล็ก byte ส่วนเกินถูกมองข้าม)
- **experiment ที่ลงโค้ดแล้ว (uncommitted):** `src/C9.Server/CharacterLobby.cs` `SelectAsync` ขยาย 0x0BC0 จาก `new byte[0x1D14]` → `new byte[0x83C6]` (zero-pad; field เดิมครบ). เป็น constant swap คอมไพล์ผ่านแน่ (ยังไม่ full-build เพราะ tar ผ่าน network mount ช้า; Docker build ตอน deploy จะ compile จริง)
- **การทดสอบชี้ขาด:** รอบหน้า deploy build นี้ แล้ว **เดินด้วย keyboard อย่างเดียว (ไม่คลิกเมาส์)** แล้วดู trace ว่ามี packet `0x0BCE` จาก keyboard หรือไม่ → ถ้าไม่มี = client ไม่ประมวลผล keyboard (ข้อมูลผู้เล่นไม่ครบ/คีย์ไม่ถูกผูก) ถ้ามีแต่ไม่เดิน = เรื่อง speed/animation

เนื้อหาถัดไปเป็นประวัติการตรวจ ข้อสรุปเรื่อง +0x118 และข้อเสนอ EAFE ที่ขัดกับส่วนนี้ถูกยกเลิกแล้ว

อัปเดต 15 พฤศจิกายน 2026 (Asia/Bangkok) โดย thread "Make the character move in game"
มาตรฐานอ้างอิง ISO/IEC 27001, ISO 9001, ISO/IEC 29110 (บันทึก work product + หลักฐานการตรวจ)

## 0. อัปเดตล่าสุด 16 พ.ย. 2026 (10:40 น. Asia/Bangkok) — thread "แก้ exit hang / keyboard"

### ⚠️ ผลทดสอบรอบ 2 (16 พ.ย. 11:00 น.) — fix เดิม "ยังไม่ผ่าน" + ทิศทางใหม่

ผู้ใช้ rebuild+deploy แล้วทดสอบ: **keyboard ยังเดินไม่ได้** และอัป dump ใหม่ (char slot 2).

**ยืนยันว่า fix ส่ง 0x0BC6 ซ้ำ "ทำงานจริง" แต่ไม่พอ:** trace `a653f7-001.jsonl` แสดงว่าทุกครั้งที่
client ส่ง `0x0BCE` server ตอบ `0x0BC6` (len 8, id=2) จริง แต่ dump ใหม่ยังได้ `controller+0x118 = 0`
(controlled actor 0x396ff008, controller 0x2a555b90, decoded id = 2 = ตรงกับที่ส่ง) ⇒
**การส่ง 0x0BC6 ซ้ำ ไม่ได้ทำให้ flag ติด** — สมมติฐานที่ว่า "event 0x118 ของ handler 0x0BC6 = field
controller+0x118" ผิด (เลข 0x118 บังเอิญตรงกันเฉยๆ)

**ค้นพบสำคัญ — nav เดินได้ keyboard ไม่ได้ เป็นคนละ gate:** ใน trace ค่าตำแหน่งใน `0x0BCE` **ไหลต่อเนื่อง**
(x เลื่อนจาก 38497 → ~29518 ≈ 9000 หน่วย) = ตัวละคร **เดินได้จริงผ่าน navigation/คลิกเดิน** ส่วน keyboard
ตายเพราะ getter ความเร็ว `FUN_007FCF20` เช็ก `controller+0x118==1` ก่อน (นี่คือเส้นทาง keyboard;
nav ไม่ผ่าน getter ตัวนี้) → ต้องทำให้ `controller+0x118=1`

**ตัวที่เซ็ต +0x118=1 หาไม่เจอแบบ static:** สแกนทั้ง image เขียน [reg+0x118] มีแค่ ctor (`0x62d5e2`=0)
กับ dtor (`0x62d8f3`=0) ไม่มีที่ไหนเขียนค่า 1 ⇒ เซ็ตผ่าน vtable/this-relative (มองไม่เห็นด้วยการสแกน offset)
หรือผ่าน event chain — ต้องตามหา method ที่เปิด control ของ controller class และ packet/เงื่อนไขที่ trigger

**ขั้นถัดไป (น่าจะใช่ที่สุด): server ขาด `0xEAFE` และ `0x2335`** — ref 2014 ตอบ 0x0BC5 ด้วย
0x0BC6 → (0x2335 ถ้ามี map-event) → **0xEAFE** (17 ไบต์, payload+0 = world objId u32) แต่ของเราส่งแค่
0x0BC6. client รุ่นนี้ลงทะเบียน **0xEAFE แบบเข้ารหัส (enc=1, hash 0xf3816eec, class 0xf2f3d0/0xf37b80)**.
ทดลอง: หลัง 0x0BC6 ส่ง `0xEAFE` flag=1 ขนาด 17 ไบต์ payload[0..3]=selectedCharacterId ที่เหลือ 0.
**ยังไม่ verify reader ของ 0xEAFE — ต้องถอด reader ให้ชัวร์ layout ก่อน deploy** กัน client hang

**สถานะโค้ด:** branch 0x0BCE→ส่ง 0x0BC6 ยังอยู่ใน working tree (ไม่มีผลเสีย แต่ไร้ประโยชน์ อาจถอดทีหลัง).
Docker build จาก working tree (`COPY src/ src/`). cloud build ผ่าน (dotnet 10.0.115 ต้องลบ global.json
+ มี c9server/data/twisters.json). ไฟล์ใช้ร่วมกับ thread launcher.


**แก้ exit hang (0x17D4→0x17D5) ยืนยันในเกมจริงแล้ว.** Keyboard: พบสาเหตุที่แท้จริงจาก
live dump `C9.DMP` (16 พ.ย. 09:33) + packet trace `artifacts/docker-state/trace/lobby/20261116/c53478-001.jsonl`
และ**แก้ source แล้ว รอทดสอบในเกม**

**จุดสำคัญที่แก้ความเข้าใจเดิม:** การตั้งตัวคูณความเร็ว (`0x0C1E` → actor+0x8A0 = 1.0)
จำเป็นแต่**ยังไม่พอ** เพราะตัวอ่านความเร็ว `FUN_007FCF20` ตรวจ **controller (actor+0x360) และ
`controller+0x118 == 1` ก่อน** แล้วจึงค่อยอ่านค่าความเร็ว ถ้า `+0x118 != 1` จะคืนความเร็ว = 0
ทันทีโดยไม่สนค่า +0x8A0 เลย → อธิบายได้ว่าทำไม dump 22:42 ยืนยันค่า 1000/1000 ถึง player แล้ว
แต่ตัวละครยังเดินไม่ได้ (ติดที่ control flag ไม่ใช่ที่ค่าความเร็ว)

**controller+0x118 ถูกเปิดโดย handler ของ `0x0BC6` (binary 0x8ef2d9):** จะ dispatch event 0x118
(→ เซ็ต +0x118=1) ก็ต่อเมื่อ (ก) actor ผู้เล่นถูกผูกแล้ว (`_DAT_017a00a8+0x102c != 0`) และ
(ข) object id ใน packet ตรงกับ id ของ actor ที่ถอดจาก container ที่ actor+0x154

**id ตรงกันอยู่แล้ว = 1:** container ที่ actor+0x154 เป็นค่า 64-bit แบบ obfuscate
(mask=(index+ptr)>>13; id=elem^mask) ถอดจาก dump ได้ = **1** ซึ่งเท่ากับ selectedCharacterId=1
ที่ server ส่ง (ค่า 16627671/0xfdb7d7 ที่เคยเข้าใจว่าเป็น id จริงๆ เป็นแค่ pointer ของ container)
event manager `[0x179ff50]` ก็ valid → เส้นทาง dispatch พร้อม

**ปัญหาจริง = ลำดับเวลา (ordering):** server ส่ง `0x0BC6` ใน state `world_loading` (ตอบ `0x0BC5`)
ซึ่งตอนนั้น client **ยังไม่ผูก actor/controller** → gate ตก → event ถูกทิ้ง → ไม่เคยส่งซ้ำ → +0x118 ค้างที่ 0
(C9 จริงผูก actor ตั้งแต่ select/enter ก่อน 0x0BC5 จึงไม่พลาด) trace ยืนยัน server ส่งแค่
0x0004,0x0EFA,0x0EFC,0x0BC0,0x0BC3,0x0C1E,0x0C20,0x0BC6,0x2EEB — **ขาด 0xEAFE, 0x2335**
และตอนเข้า world_ready client ส่ง **0x0BCE ×2** (echo token จาก 0x0BC3) ที่ server เดิม**ไม่ตอบ**

**การแก้ (ลงใน working tree แล้ว `src/C9.Server/SessionProcessor.cs`):** เพิ่ม branch
`state=="world_ready" && Type==0x0BCE` → ส่ง `WorldStartupPackets.ActionStarted(selectedCharacterId)`
(0x0BC6) ซ้ำ เพราะตอน client ยืนยันเข้าโลกด้วย 0x0BCE นั้น actor+controller ถูกผูกแล้ว การส่งซ้ำ
จึงผ่าน gate และเปิด control ให้เดินได้ (ส่งซ้ำได้ไม่มีผลข้างเคียง = idempotent). build ผ่าน
(cloud dotnet 10.0.115 C9.Server 0 error). **ไฟล์นี้ใช้ร่วมกับ thread launcher** (มีการแก้ 1771/
launcherAuthority ค้างใน working tree เดียวกัน) — build+run+ทดสอบเดินตามหัวข้อ 7

**ยังเปิดค้าง:** NPC ยังไม่โผล่ (server ไม่ส่ง entity-spawn 0x42CE/0x42D2); `0x0C20` ส่ง flag=1(enc)
แต่ client น่าจะ register เป็น plain — latent (เข้าโลกได้) ยังไม่แตะจนกว่าจะยืนยัน keyboard
ถ้าส่งซ้ำ 0x0BC6 แล้วยังไม่เดิน ขั้นถัดไปคือส่ง 0xEAFE (17B, payload+0=world objId=1) และ/หรือ 0x2335

## 1. สรุปสั้น

**ผลล่าสุด 15 พ.ย. 2026 เวลา 23:00 น.:** ผู้ใช้ rebuild แล้ว ระบบนำทางพาตัวละครเดินได้
แต่ Keyboard ยังเดินไม่ได้ และไม่เห็น NPC. Dump รอบ 22:42 ยืนยันว่าค่า 1000/1000
ไปถึง player attribute จริงแล้ว จึงยังสรุปไม่ได้ว่าการแก้ความเร็วเพียงอย่างเดียวแก้อาการครบ
พบ handshake หลังโหลดโลกที่ขาดและแก้ source แล้วตามหัวข้อ 6; ยังรอยืนยัน Keyboard ในเกมจริง

หลังเข้า Waterford ได้แล้ว ตัวละคร **ยืนเฉยเดินไม่ได้** สาเหตุคือ server ส่ง packet
ค่าสถานะ (`0x0C1E`) โดย**ตั้งตัวคูณความเร็วเดินเป็น 0** และใช้ layout เก่า (build 52893)
ที่ขนาดไม่ตรงกับ client รุ่นที่ใช้จริง (`b4580ed4`) แก้โดยสร้าง `0x0C1E` เป็น layout ใหม่
ขนาด **398 ไบต์** และตั้งตัวคูณความเร็วเดิน/แอคชันเป็น 1.0 (ค่า 1000 = 1/1000)

**ไม่ใช่เรื่อง 2nd password / Security PIN** — ใน trace ไม่มี packet ตระกูล 2nd password
(client รองรับ `PS_*_2ndPassword_*` แต่ไม่ได้ส่งในรอบนี้) และตอนล็อกอิน server ตอบ
"ไม่ใช้ OTP" ไปแล้ว client โหลดแผนที่และขอค่าสถานะตามปกติ

## 2. หลักฐานจาก memory dump ของ client (RE)

C9.exe ถูกห่อด้วย WinLicense (RE แบบ static จากไฟล์บนดิสก์ไม่ได้) จึงใช้ **memory dump**
ที่ผู้ใช้สร้างจาก Task Manager (`C9.DMP`, 15 พ.ย. ~10:30 น.) แล้วดึงเฉพาะ image ของ
`C9.exe` (base `0x00400000`, 0x1B95000 ไบต์, captured เต็ม) ด้วย
`re/scripts/extract_minidump_modules.py` ซึ่งดึงเฉพาะช่วง address ของโมดูลที่เลือก
(ไม่ส่งออก heap/stack — data minimization ตาม 27001) แล้ว disassemble ด้วย radare2/capstone
ในคลาวด์ หลักฐานที่เกี่ยวข้อง (VA เป็น preferred image ของ client รุ่นนี้ ไม่ใช่รุ่น 52893 เดิม):

| ข้อเท็จจริง | หลักฐานใน C9.exe (b4580ed4) |
|---|---|
| โครง `0x0C1E` | handler `008CFF30`: block A copy จาก decoded payload `+0x0C` ไป player `+0x560` (0x30 dwords = 192 ไบต์), block B จาก payload `+0xCC` ไป player `+0x620` (192 ไบต์), แล้ว WORD count ที่ payload `+0x18C` ของ entry ละ 8 ไบต์ → count=0 รวม **398 ไบต์** |
| ตัวคูณความเร็ว | `00882C50` อ่าน block A `+0x2C`/`+0x2E` (= payload `+0x38`/`+0x3A`, signed WORD) คูณ `0.001` (const `0x10A9778`) เก็บเป็น actor `+0x8A0` (move) และ `+0x8B8` (action) |
| ตัวอ่านความเร็วตอนเดิน | `007FCF20` อ่าน actor `+0x8A0` clamp ไม่เกิน 1.7 (`0x10AA958`); `007FCEA0` อ่าน `+0x8B8` clamp 0..1.3 (`0x10A9964`) → ถ้า `+0x8A0` = 0 ความเร็วเดิน = 0 |
| ไม่ใช่ 2nd password | RTTI มี `C9Network2ndPassword` แต่ dispatcher เดิน (`C9NetworkCharacterMove` vtable, jump table ที่ `008CE0CD`) และ `C9NetworkCharacterStatus` (`008CFE90`) ทำงานปกติ ไม่มี packet 2nd password ใน trace |
| packet เดินของ client | `0x0BCE` (46 ไบต์, สร้างที่ `008CED80`) = PS_PositionUpdateVillage; client ส่ง 2 ครั้งแล้วหยุดเพราะความเร็ว = 0 ไม่มีการเดินต่อ (ไม่ใช่รอ ack จาก server) |

เครื่องมือ/หลักฐานอยู่ใน `/mnt/project-files/c9-world/` (dump images, trace, tools) —
ไม่ commit image ของ client ลง repo

## 3. สิ่งที่แก้

ไฟล์เดียว: `src/C9.Server/CharacterLobby.cs` เมธอด `ExtendedAttributes()`
- เปลี่ยนจาก layout build-52893 (354 ไบต์, HP/MP อย่างเดียว) เป็น **398 ไบต์**
  (block A 192 + block B 192 เริ่มที่ payload `+0x0C`/`+0xCC`, WORD count=0 ที่ `+0x18C`)
- ตั้ง `payload[0x38]` และ `payload[0x3A]` = 1000 (ความเร็วเดิน/แอคชัน = 1.0)
- HP/MP ยังมาจาก `0x0C20` (ExtendedVitals) เหมือนเดิม — ไม่แตะ
- เทสต์ `tests/C9.Validation/CharacterLobbyCases.cs` อัปเดตให้ตรวจขนาด 398 + ช่องความเร็ว

**ไม่แตะ**: `SessionProcessor.cs`, `CharacterLobby` store/id/AccountId (thread Account/World เป็นเจ้าของ),
`Select()`/`0x0BC0` (คง `0x1D14` — client อ่าน buffer ใหญ่กว่า แต่เข้าเมืองได้อยู่แล้ว การส่งเพิ่ม
โดยยังไม่รู้ layout เต็ม 33KB เสี่ยงกว่า)

## 4. การตรวจ (ISO 9001 traceability)

- Cloud: `dotnet build` 0 warning/0 error, `C9.Validation` **ผ่าน 56/56** (6,589 assertions) ทั้งโหมด JSON และ DB
- ยังรอ: ทดสอบในเครื่องผู้ใช้ — rebuild `scripts\docker\Start-C9Platform.ps1 -GameData` แล้วเดินในเกมจริง

## 5. ความเสี่ยง / งานต่อ

1. **Cipher epoch**: codec รองรับ 256 frame แรกต่อทิศทางเท่านั้น (ยังไม่ทำ key rotation)
   เล่นเกิน ~5–8 นาที (เกิน 256 frame) การเชื่อมต่อจะหลุด — งานแยก
2. **`0x0BC0` ขนาด**: client อ่านถึง payload `+0x83C6` (~33,734 ไบต์) แต่ server ส่ง `0x1D14`
   (7,444) client อ่าน buffer ส่วนเกินเป็นศูนย์ — inventory/equipment/skill ยังว่าง ยังไม่ decode layout เต็ม
3. **`0x0BCE`/`0x0BCC`** (position update): server ยังไม่เก็บ/ตอบ — จำเป็นเมื่อทำ persistence และ multiplayer
4. combat, skill, inventory, item ยังไม่ทำ

## Current State (สำหรับโมเดลอื่นมาต่อ)

- โค้ดแก้บนเครื่องผู้ใช้ `C:\MyProjects\C9-Server` (ยังไม่ commit git) ไฟล์: `CharacterLobby.cs`, `CharacterLobbyCases.cs`
- ตัวคูณความเร็วอยู่ที่ `0x0C1E` payload `+0x38`/`+0x3A` (block A `+0x2C`/`+0x2E`) หน่วย 1/1000
- ตัว extract dump: `re/scripts/extract_minidump_modules.py` (ดึงเฉพาะโมดูลที่เลือกจาก minidump)
- ต่อไป: ยืนยันในเกมจริง → แล้วไป cipher rotation (ข้อ 5.1) เป็นงานถัดไปที่สำคัญสุด

## 6. ตรวจ dump และ Docker stdout รอบใหม่ — 15 พ.ย. 2026 เวลา 23:00 น.

### หลักฐานที่ยืนยันแล้ว

- Dump `artifacts/claude-sync/world/C9.DMP` timestamp ใน header = 22:42:04 น. UTC+7,
  ขนาด 1,499,734,585 ไบต์. ดึงเฉพาะ module images ด้วย extractor เดิมไปที่
  `artifacts/claude-sync/world/analysis-2242/img/`; ไม่แก้ dump หรือไฟล์เกม
- ค่า pointer ที่ `0x017A00C8` = `0x05F58AF0`; signed WORD ที่ player `+0x58C`/`+0x58E`
  = 1000/1000. Client `008CFF30` copy จาก decoded packet `+0x19` เข้าบล็อก player `+0x560`;
  เมื่อหัก header 13 ไบต์ ตรงกับ payload `+0x0C` และ speed ที่ payload `+0x38/+0x3A`
- อ่าน stdout จริงจาก `artifacts/claude-sync/world/docker-server.log` ที่ผู้ใช้ export
  (UTF-16, 514 JSON events). Session `lobby-af6f01-9` เวลา 22:36:48–22:41:31 น.
  ส่ง 0x0BC0, 0x0BC3, 0x0C1E, 0x0C20 แต่ไม่มี 0x0BC6; state มีเพียง
  `character_lobby` และ `world_loading`; ไม่มี lobby `session_error` ในรอบนี้
- Trace ในรอบเดียวกันมี 0x0BCE 65 ครั้ง, 63 พิกัดไม่ซ้ำ. **ผู้ใช้ยืนยันว่าเป็นระบบนำทาง**
  จึงไม่ใช้เป็นหลักฐานว่า Keyboard ใช้ได้
- Ref Server SHA-256 `567d2f38066a908d0276f3d44fbdb808ae5748b2adde57446d44d3cfac9ffb51`:
  handler `006A3A90` รับ 0x0BC5 แล้วตอบ 0x0BC6 แบบ plain, header 13 + object ID 8 = 21 ไบต์
- Client รุ่นปัจจุบัน: `C9NetworkMap` dispatcher `008EF110` รับ 0x0BC6,
  อ่าน object ID ที่ decoded `+0x0D/+0x11`, queue entity action event 10;
  ถ้า ID ตรง local actor จะ queue event `0x118` เพิ่ม. นี่เป็นขั้นเริ่ม action ที่ Server ขาดจริง
  **ผลต่อ Keyboard ยังเป็นสมมติฐานที่ต้องทดสอบหลัง deploy**
- Ghidra output เก็บใน `analysis-2242/{world_functions,client_dispatch,ref_handlers,ref_option_sender}.{c,asm.txt}`
  ใต้ artifacts เท่านั้น; facts ที่อ่านต่อได้อยู่ใน
  `re/evidence/static/windows_official_20261115_world_start.json`

### Source ที่แก้ในรอบนี้

- `WorldStartupPackets.ActionStarted`: สร้าง 0x0BC6 แบบ plain พร้อม object ID เต็ม 64 บิต
- `SessionProcessor`: หลัง 0x0BC5 ที่ payload ว่าง ส่ง extended attributes/vitals ตาม profile,
  ส่ง acknowledgement แล้วเปลี่ยนเป็น `world_ready`. Legacy profile ได้ acknowledgement เช่นกัน
- Duplicate 0x0BC5 หลัง ready ไม่ส่งซ้ำ; heartbeat/position ที่ยังไม่ implement และ return-to-world-list
  ยังรับต่อได้ใน `world_ready`. Metadata ของบัญชี/ตัวละคร/แผนที่ยังอยู่ใน session
- เพิ่มชื่อ packet ใน trace catalog และสีสถานะ Portal สำหรับ `world_ready`
- เพิ่ม memory integration cases ทั้ง legacy/extended และอัปเดต TCP cases เพื่อตรวจ
  identity, plain flag, malformed/duplicate load notification และ encrypted reply หลัง plain ack

### ผลตรวจและข้อจำกัด

- Isolated check ที่ compile **factory จาก production source จริง** + `C9.Protocol`:
  ผ่าน 14 assertions (wire fixture 21 ไบต์, 64-bit ID, plain ack ไม่กิน sequence/cipher,
  encrypted reply ถัดไปยัง decode ได้, C# syntax ของไฟล์ที่แก้)
- ชุด `C9.Validation` เต็ม **ยังไม่ได้รัน**: restore ติด NU1801/NU1101 เพราะ session นี้เข้าถึง
  nuget.org ไม่ได้และไม่มี Npgsql cache. Isolated check ไม่แทนการทดสอบ session/DB
- ไม่ได้ rebuild/restart Docker หรือเปิดเกมในรอบนี้: Docker API pipe ถูกปฏิเสธสิทธิ์
- ยังไม่มี NPC runtime/spawn handlers ใน source ของเรา. ต้องถอด world/entity initialization
  จาก Ref Server ต่อและยืนยันกับ Client; ยังไม่อ้างว่า acknowledgement ตัวเดียวทำให้ NPC กลับมา
- Error อื่นใน stdout: หลังกลับไป login, `login-fd0287-10` reconnect 0x2EE8 สำเร็จ
  แต่รับ 0x26AC (9900), payload 40 ใน `server_list_sent` แล้วเกิด `No fixture rule for session state/type`
  เวลา 22:42:11 น. แยกเป็นงาน reconnect/heartbeat ภายหลัง; ยังไม่ได้แก้ในรอบนี้

### วิธีทดสอบรอบถัดไป

```powershell
dotnet run --project tests/C9.Validation/C9.Validation.csproj -c Release -- --offline
.\scripts\docker\Start-C9Platform.ps1 -GameData
.\scripts\docker\Test-C9Platform.ps1
```

เปิด L9 เข้าเมืองใหม่ แล้วกด Keyboard โดยไม่ใช้ระบบนำทาง. ตรวจ trace ว่าหลัง 0x0BC5
มี 0x0BC6 (flag 0, payload 8 ไบต์) และ state เปลี่ยนเป็น `world_ready`.
ถ้ายังเดินไม่ได้ ให้เก็บ dump ขณะตัวละครยังอยู่ในเมืองพร้อม trace/stdout รอบเดียวกัน
เพื่อตรวจ handler ของ event `0x118` และ input state ต่อ ไม่ย้อนแก้ค่าความเร็วโดยไม่มีหลักฐานใหม่
