# C9 RE Phase 2 — Codec และ Login Dispatch จาก Static Analysis

วันที่ 2 พฤศจิกายน 2026 · macOS ARM64 · อ้างอิง [Phase 1](C9_RE_Phase1_TH.md) และ [แผน RE](C9_RE_PLAN_TH.md)

## ขอบเขต

ดำเนินงานโดยไม่เปิดเกมตามเงื่อนไขของผู้ใช้: ไม่เริ่ม C9.exe, Launcher หรือ Patcher และไม่มี runtime trace/packet capture ในรอบนี้ ใช้ Ghidra headless อ่าน PE และฐานข้อมูลวิจัยเดิม พร้อมเครื่องมือ Python สำหรับอ่าน RTTI และสร้าง manifest เท่านั้น

ไบนารีที่อ้างอิง: `C9.exe` SHA-256 `b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25` ที่อยู่ทั้งหมดเป็น static VA อิง preferred image base `0x00400000`

รอบนี้ export ฟังก์ชันที่มีนิยามในฐานข้อมูล 77 ที่อยู่ไม่ซ้ำ รวมรายการที่เคยตรวจใน Phase 1 เพื่ออ่านบริบทเพิ่ม จำนวนนี้เป็นขอบเขต export ไม่ใช่จำนวนฟังก์ชันที่พิสูจน์พฤติกรรมครบ และไม่ได้หมายถึง auto-analysis ทั้งโปรแกรมเสร็จแล้ว

## 1. ตาม RTTI ไปถึง Login Handler ได้แล้ว

Phase 1 ยังผูก debug strings กลุ่ม login เข้ากับฟังก์ชันไม่ได้ รอบนี้ใช้ MSVC x86 RTTI: type descriptor → complete object locator → vtable → function pointers แทนการอาศัย log string เพียงอย่างเดียว

| Class | Type descriptor VA | Vtable VA | Function pointers ที่พบ |
|---|---|---|---|
| C9NetworkLogin | `0x012F4D4C` | `0x0112DC54` | `0x00A64110`, `0x00A621F0` |
| C9NetworkCommon | `0x012F4C28` | `0x0112616C` | `0x00A484F0`, `0x00A47CB0` |
| C9NetworkManager | `0x012F4D6C` | `0x0112E050` | `0x00A6B180` |

การสำรวจหยุดเมื่อ pointer ถัดไปไม่ชี้เข้า `.text` และจำกัดจำนวน slot จึงไม่ใช้จำนวน pointer ในตารางเป็นข้อสรุปโครงสร้าง class ทั้งหมด

`0x00A621F0` อ่าน ushort จาก packet offset `+3` แล้วเปรียบเทียบ type 11 ค่า:

`0x0004`, `0x23F0`, `0x23F1`, `0x2454`, `0x2EE2`, `0x2EE3`, `0x2EE6`, `0x2EE9`, `0x2EEB`, `0x2EED`, `0xED22`

จึงยืนยันความสัมพันธ์ระหว่าง offset 3 กับการ dispatch ใน class login จาก static code ได้ ชื่อ PS_* ของแต่ละค่าและ coverage ขณะรันจริงยังไม่ยืนยันครบ ไม่สรุปว่า 11 ค่านี้คือข้อความ login ทั้งหมดของเกม

## 2. Header Constructors ที่พบในเส้นทาง Login

ตรวจ setter ของ length, flag และ type ใน C กับ assembly แล้ว ได้ 7 รูปแบบต่อไปนี้ ความยาวเป็น **decoded/constructed length ก่อน encode** ไม่จำเป็นต้องเท่ากับ wire length หลัง transform

| Constructor VA | Type hex | Decimal | Length ก่อน encode | Flag |
|---|---:|---:|---:|---:|
| `0x00A64380` | `0x1771` | 6001 | 62 | 1 |
| `0x00A643D0` | `0x2EE1` | 12001 | 2175 | 1 |
| `0x00A64430` | `0x2EE5` | 12005 | 13 | 1 |
| `0x00A64480` | `0x2EEC` | 12012 | 33 | 1 |
| `0x00A644E0` | `0x2EE8` | 12008 | 41 | 1 |
| `0x00A64530` | `0x0EC4` | 3780 | 13 | 1 |
| `0x00A64580` | `0x2455` | 9301 | 29 | 1 |

ทุก constructor ในตารางตั้ง checksum ที่ offset 5 และ sequence ที่ offset 9 เป็น 0 ก่อนเข้าเส้นทางส่ง ขณะ encode จึงมีการเขียนค่าใหม่

ตัวอย่างการเชื่อมทางเดินโค้ด:

- `0x00A63160` ใช้ wrapper `0x00A64320` → constructor `0x00A64430` เพื่อสร้าง type `0x2EE5` แล้วเรียกเส้นทางส่ง
- `0x00A631B0` ใช้ wrapper `0x00A64340` → constructor `0x00A64480` เพื่อสร้าง type `0x2EEC` และเติมข้อมูลเพิ่มเติม
- ภายใน handler ของ type 4 มี branch ที่สร้าง `0x2EE1`, `0x2EE8` หรือ `0x1771` ตาม mode/state ที่พบ ไม่ควรสมมติว่าทุก session ใช้ branch เดียวกัน

ตารางนี้ยังไม่ตั้งชื่อ type เหล่านี้เป็น PS_Certify_Req / PS_ServerList_Req / PS_Select_Server_Req แบบยืนยัน เพราะการจับคู่ชื่อยังต้องมีหลักฐานเพิ่มจาก caller, schema หรือ trace

## 3. Encode / Decode มีสองชั้นและมี State

### เส้นทางส่ง TCP

`0x00A38850` ตรวจ flag ที่ offset 2 หากไม่เป็นศูนย์จะเรียก `0x00563C10` แล้วส่ง buffer ที่ encode ผ่าน `0x00563930` → WSASend

ลำดับภายใน `0x00563C10`:

1. เก็บ 3 ไบต์แรกไว้ใน output
2. ใส่ sequence ที่ packet offset 9
3. ตั้งฟิลด์ offset 5 เป็น 100 แล้วคำนวณ checksum ของข้อมูลตั้งแต่ offset 3 จนท้าย constructed frame
4. ใส่ checksum กลับที่ offset 5
5. แปลง payload ตั้งแต่ offset 13 โดยใช้ selector ที่ประกอบจาก checksum กับค่า lookup ตาม type
6. ส่งส่วนตั้งแต่ offset 3 เข้า transform อีกชั้น ผ่าน `0x00DCEEC0`
7. เขียน wire length ใหม่เป็นจำนวนไบต์ที่ transform ได้บวก 3

`0x00564380` เป็นส่วนของ lookup ตาม type และมีข้อยกเว้นที่คืน 0 สำหรับ type `0x1771` กับ `0x2EE1` ยังต้องไล่ที่มาของค่า lookup และการปรับค่าที่เกี่ยวข้องต่อ ก่อน port ทั้ง codec

### เส้นทางส่ง BaseCat

`0x00666600` → `0x006667B0` → `0x00665BA0`

`0x006667B0` ตั้ง sequence/checksum และแปลง payload ก่อนเรียก `0x00DCEEC0` เช่นกัน แต่ selector ของ payload ในฟังก์ชันนี้ใช้ checksum โดยตรงผ่าน `0x004121D0` ไม่พบการบวกค่าจาก lookup ตาม type แบบ TCP ในฟังก์ชันที่ตรวจนี้

ทั้ง `0x004121D0` และ `0x00412230` นำ selector modulo `0x50` (80) ไปค้น registry แล้วเรียก virtual method สำหรับส่ง/รับ การพบเลข 80 ไม่ได้พิสูจน์ว่ามี algorithm ที่ register ครบ 80 ตัว

### เส้นทางรับและ state

- Decode ชั้นหลัง prefix อยู่ที่ `0x00DCEF50`; encode อยู่ที่ `0x00DCEEC0`
- ทั้งคู่ใช้ codec object ผ่าน virtual dispatch (`0x00DCF0E0`, `0x00DCF110`) และมี counter/threshold ที่มีผลต่อการ update/reset state
- object initializer/reset `0x00DCEE10` / `0x00DCECB0` แสดง state สองชุดที่ห่างกัน `0x98` ไบต์; counters เริ่มที่ 1 และ byte table 128 ไบต์ถูกเติมค่า 99 ในส่วนที่ตรวจพบ
- มีขั้นเลือก/สร้าง algorithm ใน `0x00DD5470`; พบ CryptoPP RTTI ในไบนารี แต่ยังไม่ระบุ algorithm, mode และ key schedule ที่ใช้ในแต่ละ session จนครบ
- การรับที่ตรวจใน Phase 1 ทำ transform ส่วน offset 3 ก่อน แล้วจึงแปลง payload ตรวจ checksum และ sequence ตามลำดับ

จึงยังสร้าง codec ที่สมบูรณ์จากสูตร checksum เพียงอย่างเดียวไม่ได้ และยังไม่เรียก transform ชั้นนอกว่า compression หรือ cipher ชนิดใดชนิดหนึ่งโดยไม่มีหลักฐานของ concrete virtual target

## 4. Counter ของ TCP และ BaseCat ใช้เกณฑ์เพิ่มต่างกัน

| เส้นทาง | Object offsets ที่พบ | หลักฐานเรื่องการเพิ่ม |
|---|---|---|
| TCP send | `+0x20` | ใน `0x00A38850` เพิ่มหลัง encode ผ่าน branch ตรวจผล และก่อนเรียก WSASend wrapper |
| TCP receive | `+0x24` | เพิ่มใน receive processing หลัง decode/validation ที่เรียกสำเร็จ |
| BaseCat send | `+0x184` | `0x00666600` เพิ่มเมื่อ `Link::Send` รายงานสำเร็จ |
| BaseCat receive | `+0x188` | เพิ่มใน `0x00666260` หลัง validation ผ่าน |

`0x00666A70` ตั้ง BaseCat counters ทั้งคู่เป็น 0 และ reset codec; connected callback `0x006661D0` เรียก reset นี้ก่อนสร้างและส่ง control packet

ผลต่อ implementation: ต้องรักษา state แยกตาม transport และทิศทาง รวมกรณีส่งไม่สำเร็จ ยังไม่ถือว่าพฤติกรรม retry/reconnect ทั้งหมดถูกพิสูจน์แล้วจากเส้นทางย่อยเหล่านี้

## 5. พบ Outer Packet อีกโครงสร้างหนึ่ง

เส้นทาง `0x00665950` ใช้ constructor `0x00666AC0` ซึ่งตั้ง type `0x36B8` (14008), flag 0 แล้วกำหนดความยาวจริงเป็น payload length + `0x26` (38)

ฝั่งรับ `0x00667C90` ตรวจ type `0x36B8` และอ่านข้อมูลภายในจาก offset `0x26` ก่อนเรียก payload processing และสร้าง frame ชั้นในต่อ

นี่เป็น envelope ที่มี header พื้นฐานร่วมกับ frame 13 ไบต์ แต่มี fields เพิ่มก่อนข้อมูลชั้นใน ต้องแยกออกจากข้อความที่เริ่ม application payload ที่ offset 13 โดยตรง และยังไม่สรุปบทบาทว่าเป็น auth, lobby หรือ gameplay จากตำแหน่งฟังก์ชันเพียงอย่างเดียว

อีก constructor `0x00666B20` สร้าง control packet type `0x36BA` (14010), constructed length `0x25` (37) และ flag 1 ซึ่งถูกใช้ใน connected callback path ผ่าน `0x00666600`

ผลสำคัญ: frame flag 0 บางชนิดในเส้นทางที่พบมี checksum เริ่มต้นเป็น 0 และข้าม transform/validation ชั้นเดียวกับ flag 1 จึงไม่ควรกำหนดกฎว่าแพ็กเก็ตทุกชนิดต้องผ่าน checksum routine เดียวกันโดยอัตโนมัติ

## 6. Schema และ Transition ที่เจาะต่อได้โดยไม่เปิดเกม

| Type ที่ handler รับ | การเข้าถึงข้อมูลที่ตรวจพบ | สิ่งที่ยังต้องยืนยัน |
|---|---|---|
| `0x0004` | อ่าน mode byte ที่ offset 13 แล้วเลือก branch สร้างข้อความตอบจาก Client | ชื่อข้อความและหน้าที่ของ mode/state |
| `0x2EE2` | อ่านฟิลด์ขนาด 4 ไบต์ที่ offsets 13, 17, 21, 25, 29 | ความหมาย account/session และขอบเขต schema ทั้งหมด |
| `0x2EE3` | อ่านผล/เวลาหรือเงื่อนไขที่ offsets 13, 17, 21, 25 และมีทางแสดงข้อความปฏิเสธ | ชื่อ error fields และกติกาของค่าต่าง ๆ |
| `0x2EE6` | อ่านจำนวนรายการ 1 ไบต์ที่ offset 13 แล้ววน entry เริ่ม offset 14 ด้วย stride `0x34` (52) | ชื่อแต่ละ field, string length, ข้อกำหนดความยาวข้อความและการจับคู่ชื่อ PS_* |
| `0x2EED` | บาง branch นำฟิลด์ 4 ไบต์ที่ offset `0x1A` และ 2 ไบต์ที่ `0x1E` ไปเส้นทาง connect ผ่าน `0x00A68D40` | endpoint semantics, byte order และสถานะการ handoff ขณะรันจริง |

ตารางนี้แสดง accesses ที่พบใน branch ที่อ่าน ไม่รับรองว่า schema ทั้งหมดครบหรือ branch เหล่านี้เกิดจริงในการทดสอบ Client ชุดนี้

## 7. หลักฐานที่เพิ่มและการตรวจสอบ

| ไฟล์ | เนื้อหา |
|---|---|
| `evidence/static/phase2_client_manifest.json` | hash ของ top-level binaries/config/version/launch scripts 38 ไฟล์ ไม่รวม assets |
| `evidence/static/phase2_network_rtti.json` | type descriptors, locators และ vtable candidates ที่ตามได้ |
| `evidence/static/phase2_message_catalog.json` | constructors 7 รูปแบบและ login comparisons 11 ค่า พร้อมสถานะ runtime_verified=false |
| `evidence/ghidra/phase2_codec_core.*` | encode/decode helpers และ registry lookup |
| `evidence/ghidra/phase2_codec_init.*` / `phase2_codec_state.*` / `phase2_codec_reset.*` | initialization, counters และ reset paths |
| `evidence/ghidra/phase2_login_dispatch.*` | login/common handlers ที่ตามจาก RTTI |
| `evidence/ghidra/phase2_login_constructors.*` | bounded export ของ constructor region |
| `evidence/ghidra/phase2_login_messages.*` / `phase2_login_requests.*` | caller chains และข้อความที่สร้างใน Client |
| `scripts/c9_rtti_map.py` | อ่าน MSVC x86 RTTI จากไฟล์โดยไม่ load Windows DLL |
| `scripts/ExportC9Range.java` | export ฟังก์ชันที่มีนิยามใน address range ที่จำกัด |

ไฟล์ `.*` หมายถึง `.c` pseudocode และ `.asm.txt` assembly ค่าฟิลด์ที่สำคัญตรวจประกอบ assembly ไม่ใช้ prototype ที่ decompiler เดาเป็นข้อกำหนด implementation โดยตรง เช่น arguments ที่วางบน stack อาจถูกใช้โดย call ถัดจาก getter

ก่อนส่งงานตรวจความสอดคล้องของ RTTI กับ PE bytes, setter ของ type/flag กับ assembly, JSON catalog และ hash ของ source binaries เทียบ baseline

## 8. งาน Static ที่ทำต่อได้ และงานที่พักไว้

งานที่ทำต่อบน Mac ได้:

1. ผูก virtual targets ของ codec และ registry initialization เพื่อระบุ algorithm/mode/state ที่แน่นอน
2. ไล่ entry definition ของ type `0x2EE6` และ caller/constructor ของ request ที่เกี่ยวข้อง เพื่อเติม schema แบบมี confidence กำกับ
3. ไล่ queue consumer ไป dispatch registration และความสัมพันธ์ระหว่าง C9NetworkManager กับ login/common handler
4. ทำ offline fixtures จากค่าที่พิสูจน์ได้ และเตรียมจุดสังเกตสำหรับ debug ภายหลัง โดยแยก synthetic fixture ออกจากข้อมูลที่มาจาก Client จริง

งานที่พักจนกว่าผู้ใช้แจ้งให้เปิดเกมเพื่อ debug: ตรวจ loaded modules ขณะรัน, endpoint จริง, การ attach debugger, packet capture และการยืนยัน handshake กับ Client จริง

M2 และ M3 มีความคืบหน้าฝั่ง static แต่ยังไม่ผ่านเกณฑ์ runtime/interoperability และยังไม่ถือว่า server emulator พร้อมล็อกอิน
