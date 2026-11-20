# Protocol baseline ก่อนพัฒนา C9 Server ต่อ

อัปเดต 16 พ.ย. 2026 จาก dump เวลา 17:50, trace ที่ export จาก Docker และ Ref Server 2014

## ข้อตกลงการทำงาน

ทำ inventory ของ packet ทั้งหมดก่อน แล้วตรวจ **ทั้ง flow ที่กำลังจะพัฒนา** ให้ครบก่อนแก้ server ไม่จำเป็นต้องถอดความหมายของทุกระบบในเกมก่อนเริ่มพัฒนา เช่น ระบบประมูล/PvP ที่ยังอยู่นอกงานปัจจุบัน

ใช้ client ปัจจุบันเป็นตัวกำหนด wire contract; ใช้ Ref Server ตรวจโครงสร้างและ business flow ข้ามรุ่น การมี opcode เดียวกันไม่ยืนยันว่า body หรือความหมายเหมือนกัน การส่ง packet ได้หรือ build ผ่านก็ไม่ยืนยันว่า client รับ state ได้ครบ

ผูกหลักฐานกับ client SHA-256 `b4580ed4f8fa6670501577918ad61a3318d72b119bb83caaeab5aab5f31d3bb7` ห้ามนำ layout จาก client hash อื่นมาใช้โดยไม่เทียบ reader/constructor ก่อน

## Inventory ที่ตรวจแล้ว

| รายการ | จำนวน | สิ่งที่ยืนยันได้ |
|---|---:|---|
| Registration ใน client ปัจจุบัน | 2,177 | ID, type hash และ encryption policy |
| Catalog ของ Ref 2014 | 1,304 | ID พร้อม handler บางส่วนของ Game/World/Community |
| ID ที่มีร่วมกัน | 1,290 | ต้องเทียบ layout อีกครั้ง |
| ID เฉพาะ client ปัจจุบัน | 887 | ห้ามอ้างว่ามี handler ใน Ref โดยอัตโนมัติ |
| ID เฉพาะ Ref | 14 | ห้ามส่งให้ client ปัจจุบันโดยอัตโนมัติ |
| ID ร่วมกันแต่ encryption policy ต่างกัน | 15 | ตัวอย่าง `2748/2749/274A`; ต้องแยกตาม build |

Catalog ทั้งหมด: `re/evidence/static/windows_official_20261116_packet_catalog.json`

ผลเทียบและข้อผิดที่ยืนยันแล้ว: `re/evidence/static/windows_official_20261116_protocol_audit.json`

**Inventory ไม่เท่ากับ decode ครบ:** ยังต้องระบุ direction, payload layout, field semantics, handler, state และ error flow ทีละระบบ และ catalog ที่ scan ได้อาจไม่ครอบคลุม initializer ทุกรูปแบบ

## แผนที่ flow ปัจจุบัน

ขนาดในตารางเป็น **payload bytes** ไม่รวม header 13 bytes หรือ encrypted wrapper; เครื่องหมาย ≥ คือขอบเขตขั้นต่ำที่ reader พิสูจน์ได้ ยังไม่ใช่ขนาด struct ที่สมบูรณ์

| Flow | Packet และ layout ที่มีหลักฐาน | สถานะ / ช่องว่างที่ต้องปิด |
|---|---|---|
| Launcher identity | `2F04` C→S 1,092; `2EE2` S→C 20; `1771` C→S 49 | Login/Register จริงและ account binding ผู้ใช้ทดสอบแล้ว; ต้องรักษาการตรวจ handoff ทุกเส้นทาง |
| Login follow-up / server list | `2EF6` 4 → `2EF7` 4; `2EE5` 0 → `2EE6` 53 ตาม fixture | ใช้งานได้ในเส้นทาง login ปกติ; ทบทวน flag ของ `2EE6` ที่ fixture ใช้ plain แต่ catalog ระบุ encrypted |
| เลือก server | `2EEC` 20 → `2EED` 26 | มี endpoint handoff; ต้องแยกจาก reconnect |
| รายการ/สร้าง/รายละเอียดตัวละคร | `0EFA`; `0BBB` 90 → `0BBC` 113; `0EFB` 8 → `0EFC` 53 | ต้องรวมเข้าตาราง field ของ build ปัจจุบัน ไม่ถือว่า list ผ่านแล้ว select จะถูกด้วย |
| **เลือกตัวละคร** | `0BBF` 8 → `0BC0` **33,734** (`0x83C6`, frame 33,748; `max_frame` ≥ 33,748) | แก้แล้ว 19 พ.ย.: ขยายตาม reader `008824D0` และเขียน HP/MP ปัจจุบันที่ payload 32/36 (เดิมเป็น 0 ⇒ client ตั้ง flag ตาย) ยืนยันในเกมแล้ว; field inventory/equip/skill ที่เหลือยังเป็นศูนย์และยังไม่ถอด |
| เข้า map | `0BC3` 46; request `0E4B` 5 (map u32 + mode u8) | branch Sprint 1 (20 พ.ย.) ตอบ `0E4C` enc 7 B `{u32 map echo, u8 kind, u16 count=0}` ตาม case `0xE4C` ใน `008EF110` (entry 80 B อ่านด้วย `008F2F30`); การ echo kind ยังเป็นการอนุมาน รอทดสอบในเกม |
| โหลดเสร็จ | `0BC5` 0 → `0BC6` 8 plain | ส่งครั้งเดียว; ถอดการส่ง BC6 ซ้ำหลัง BCE เพราะไม่ได้แก้ Keyboard |
| Attributes / vitals | `0C1E` 398; `0C20` 32 plain | factors 1000/1000, multiplier 1.0 ยืนยันใน dump; แก้ flag C20 แล้ว ยังไม่ยืนยันว่าแก้ Keyboard |
| Keyboard / movement | `0BCE` 46 C→S (ท่า `Move_Front`/`JUMP_UP`/`WAIT`) | **ใช้ได้แล้ว 19 พ.ย. 22:23 น.** (W/A/S/D + Space): สาเหตุคือ HP ปัจจุบันใน `0BC0` payload 32 = 0 ⇒ actor +0x790 (ตาย) = 1 ⇒ `00823090` ตัดคำสั่งเดิน/กระโดด; ดู `WORLD_MOVEMENT_TH.md` ส่วนบนสุด. server ยังไม่เก็บ/broadcast `0BCE` |
| NPC | dispatcher `C9NetworkNpc` = `008F8FA0` | ยังไม่มี spawn contract ที่ยืนยันครบ; ข้อเสนอ `42CE/42D2` เดิมมีเพียง catalog ยังใช้เป็นหลักฐาน NPC spawn ไม่ได้ |
| กลับหน้าเลือกตัวละคร | `0EC4` 0 → `2EEB`; reconnect `2EE8` 28 → `2EE9` 0 | trace จบที่ ack แล้วค้าง; reader `008E64F0` เรียก `00C51640` ตั้ง StageCharacter+238 = 7 ต้องไล่ transition ถัดไปและ account binding |
| Quit Game | `17D4` 16 → `17D5` 0 | ผู้ใช้ยืนยันว่าออกเกมได้แล้ว แยกจาก return-to-character flow |
| 2nd Password | dispatcher `0089B880`; sender `2744/2746/2748/274A`, reply `2745/2747/2749/274B` | มีทั้ง client และ ID ใน Ref; ยังต้องยืนยัน operation, error/lockout และ challenge field ก่อนทำ persistence/handler |
| Cipher epoch | แยก send/receive; rekey ทุก N frame จากตาราง plaintext 128 B | **ถอดครบ 20 พ.ย.** (client `b4580ed4`): `00D04940/00D049D0` นับ frame, ครบ threshold ⇒ `00D0A880` hash ตาราง (เลือก 1 ใน 9 hash) ⇒ `00D0A480` เลือก 1 ใน 16 cipher ของ Crypto++ (N = 8/32/256 ตาม cipher). implement ใน `CipherDirection`/`CipherEpoch` ผ่าน vector Crypto++ และ stream 26,000 frame; ถอด guard แล้วและ `max_packets` เป็น 1,000,000 เพราะ contract ยืนยันแล้ว (ยังรอทดสอบเล่นเกิน 5 นาทีในเกม) |

## เหตุผลที่ต้องเริ่มจาก 0BC0

Current `C9NetworkCharacter` (`008CB980`) เรียก `008824D0` ก่อน copy character data ฟังก์ชันนี้อ่าน inventory/รายการอุปกรณ์หลาย block เช่น copy จาก **decoded+0x7F33 จำนวน 0x128 DWORD** ถึง exclusive end `0x83D3` เท่ากับ payload ขั้นต่ำ `0x83C6` หรือ 33,734 bytes แต่ server ยังจัดสรรเพียง `0x1D14` หรือ 7,444 bytes ตาม reader เก่า

โค้ดที่ตรวจนี้ตรงกับ dump ล่าสุด การอ่านเกิน body เป็นข้อผิดที่ยืนยันได้; **ยังไม่ยืนยันว่าเป็นสาเหตุเดียวของ Keyboard/NPC** และการเติมศูนย์ให้ยาวขึ้นอย่างเดียวไม่พอพิสูจน์ว่า contract ถูก ต้องตรวจ count, enum, defaults, identity และขนาด frame ไปด้วย

หลักฐาน: `artifacts/research/20261116/world-control/selection_stage.c` และ `.asm.txt`; probe `probe_selection.py` ยืนยัน code bytes ตรงกับ dump ล่าสุด ส่วน MaxFrame ปัจจุบัน 8,192 ก็ไม่พอสำหรับ body ขั้นต่ำนี้

## ลำดับงานและเกณฑ์ก่อนเริ่ม implement

1. **ล็อก build และ contract ของเลือกตัวละคร/เข้าเมือง:** ตรวจ constructor/reader ของ `0BC0`, field map, count/array bounds, frame size และ message order จนสร้าง fixture ที่ตรวจขอบเขตได้ ครอบคลุมปกติ/ผิดขนาด/ส่งซ้ำ จากนั้นจึงปรับ server และทดสอบในเกมหนึ่งรอบที่กำหนด action ชัดเจน
2. **Keyboard และ NPC:** หลัง startup contract ถูก ตรวจ input gate ด้วย dump/trace และหา NPC creation contract จาก reader ปัจจุบัน เทียบ Ref เป็นราย field รวม template ID, object ID และแหล่ง spawn position ของ map
3. **Return/reconnect:** ระบุเจ้าของ connection และ state หลัง `2EE9` ให้ครบ รวม account/handoff validation และการเข้า lobby รอบใหม่ โดยไม่แตะ Quit flow ที่ผ่านแล้ว
4. **2nd Password:** จับคู่ UI action → sender → reply/error → account state ก่อนออกแบบ DB การเก็บ hash, retry/lockout และการบังคับตรวจ ห้ามใช้ ack สำเร็จแทนการตรวจรหัส
5. **Cipher rotation:** ตรวจ native send/receive boundary และสร้าง reference vectors ข้าม frame 256 ก่อนเพิ่ม session budget

แต่ละ packet contract ต้องมี: build hash, direction, flag, payload size/bounds, field offsets/types, state ก่อน/หลัง, side effects, success/error response, reader/sender VA และ trace/Ref evidence แยก **ยืนยันแล้ว / อนุมาน / ยังไม่รู้** ใช้หนึ่งรายการเป็นแหล่งอ้างอิงหลัก ไม่สร้างคำอธิบายใหม่ซ้ำตามแต่ละ thread

เมื่อจุดใดยังไม่มีหลักฐาน ให้ทำ RE เฉพาะจุดที่ขวาง flow นั้นก่อน ไม่วน deploy packet ที่เดาความหมาย และไม่ขยายไปทุกระบบของเกมพร้อมกัน

## ข้อสันนิษฐานที่ยกเลิก

- `action+0x118 != 1` ไม่ได้ทำให้ `007FCF20` คืนความเร็ว 0: ค่าเริ่มต้นจาก `00FC272C` คือ **1.0**
- event ID `0x118` ไม่เท่ากับ field offset `+0x118`
- `EAFE` อยู่ใน Guild Battle และ Ref ส่งแบบมีเงื่อนไข จึงไม่ใช่ keyboard-enable packet ทั่วไป; `2335` ก็มีเงื่อนไข
- Ref 2014 มี ID `2744..274B`; ข้อสรุปเดิมว่าไม่มี 2nd Password handler ทั้งหมดไม่มีหลักฐานพอ

## การตรวจงานรอบนี้

การแก้ production จำกัดไว้ที่ถอด BC6 ซ้ำ และแก้ C20 เป็น plain ตามหลักฐาน ไม่ได้ deploy และยังไม่แก้ 0BC0/NPC/reconnect/2nd Password ผล build/offline ล่าสุดจะบันทึกใน CURRENT_STATE ส่วน full TCP validation ติด Windows socket 10013 ของ session นี้ และ Docker API อ่านตรงไม่ได้ จึงใช้ trace ที่ export ไว้ ไม่อ้างว่าผ่าน runtime ทั้งหมด
