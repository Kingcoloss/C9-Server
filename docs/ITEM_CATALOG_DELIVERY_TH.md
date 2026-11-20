# Item Catalog, ส่ง item ผ่าน Mail/คลัง และ Starter items — ผลค้นคว้า (อ่านอย่างเดียว) และแผน

อัปเดต: 20 พฤศจิกายน 2026 ~09:45 น. (Asia/Bangkok) — **ค้นคว้าเสร็จ เริ่มเขียนโค้ดขั้น A–B** (branch รวมงาน 2nd password แล้วที่ `7eacd4c`)
มาตรฐานอ้างอิง: ISO/IEC 29110 (บันทึกงาน/ส่งต่อ), ISO 9001 (หลักฐานตรวจย้อน), ISO/IEC 27001 (audit การให้ของ)
เกี่ยวข้อง: [CHARACTER_STATS_TH.md](CHARACTER_STATS_TH.md), [LEGACY_PATCH65_REFERENCE_TH.md](LEGACY_PATCH65_REFERENCE_TH.md), [GAME_DATA_TH.md](GAME_DATA_TH.md)

## 1. สิ่งที่พบ

### 1.1 ไฟล์ GameData ของ Client ปัจจุบันอ่านได้แล้ว

`GameData.cfs` รุ่นใหม่ (เช่น `C:\VFUN\C9-GL\GameData\GameData.cfs`, `Msg\ENG\ENG.cfs`) ไม่ใช่ mode ใหม่ แต่เป็น mode 2 เดิมที่
- header ยาว 24 ไบต์: `cfs\n\x01\x08\x02\x0d` + mode(2) + **DWORD เพิ่ม (0x01642A57)** + reserved(0) + count แล้วต่อด้วย index แบบเดิม (ชื่อ XOR 0x3A, size/packed/offset)
- body ของแต่ละไฟล์มี prefix 0–9 ไบต์ก่อน header LZMA (`5D 00 00 40 00`) ข้าม prefix แล้ว decompress แบบเดิม
- สคริปต์ทดลองอยู่ใน scratchpad ของ session (ยังไม่ได้ย้ายเข้า `re/scripts/c9_cfs.py`) ผลที่ถอดเก็บนอก git เท่านั้น (ลิขสิทธิ์ของผู้พัฒนาเกม)

### 1.2 ตารางชื่อ item (ใช้ทำ Item Catalog ได้ทันที)

`ENG_GameData.c9t` = DWORD จำนวน section (8) แล้วแต่ละ section = DWORD count + record (`[u32 id][u32 len][UTF-16 len ตัวรวม NUL]`)
- **section 3 = ชื่อ item:** 45,283 รายการ, record = id + ชื่อ + คำอธิบาย, id 0–62,248 ไม่ซ้ำ (ตัวอย่าง 20332 Skill Recycler, 24120 "[Class Change] Elite Mystic")
- ID ตรงกับรายการ `C9 Item List.txt` ของ GM tool ใน Ref Server (2014 มี ~24k รายการ) แปลว่า item ID คงที่ข้าม patch
- section อื่น: 0 ฉายา/class title (56), 1 NPC (99), 2 ชื่อมอนสเตอร์/NPC (17,661), 4 buff/สถานะ (197), 7 skill (1,101) ฯลฯ
- KOR มีไฟล์รูปแบบเดียวกัน; คุณสมบัติของ item (ประเภท, stack, level, อาชีพ, ช่องสวมใส่) อยู่ใน `GameData.c9d` (68 MB, columnar binary) **ยังไม่มีตัวอ่าน**

### 1.2.1 คุณสมบัติและรูปของ item (ถอดเพิ่ม 20 พ.ย.)

- `GameData.c9d` เป็นตารางแบบ record: ตาราง item อยู่ช่วงประมาณ 11–34 MB ของไฟล์ record = `[u32 id][u32 len][ชื่อภายในภาษาเกาหลี UTF-16]`
  + ส่วนคงที่ (ยาวต่างกันตามประเภท item) + `[u32 len][tooltip key UTF-16]` + … + `[u32 len][ชื่อไฟล์รูป ASCII .dds]`
- จับคู่ item กับรูปได้ **38,259 / 45,283** (ไฟล์ .dds ตัวแรกใน record) เช่น 20332 → `icon_GP_skillrecycle.dds`, 31093 → `icon_pw0000_E_body_d00.dds`
- ไฟล์รูปอยู่ใน `Ui_Icon/**/*.cfs` ของโฟลเดอร์เกม (70 archive, 20,346 ไฟล์) มีจริง **38,173 / 38,259**; เป็น DDS ไม่บีบอัด 32 บิต ส่วนใหญ่ 42×42 แปลงเป็น PNG ได้ตรง ๆ
- CFS มี header 2 แบบ: 20 ไบต์ (2014 และ Ui_Icon) กับ 24 ไบต์ (GameData/Msg รุ่นใหม่); prefix ก่อน LZMA ไม่นับใน packed size
- รูปและข้อมูลเป็นลิขสิทธิ์ผู้พัฒนาเกม: เครื่องมือ import จะอ่านจากโฟลเดอร์เกมของผู้ใช้เอง แล้วเก็บใน state ของ Docker (นอก git) ไม่ commit รูปเข้า repo

### 1.2.2 กระเป๋ารับของจาก Shop (ผู้ใช้ยืนยัน 20 พ.ย.: "กระเป๋ากลาง" = กระเป๋าที่รับของจาก Shop)

- ข้อความในเกม: "Item successfully purchased. Check your storage box.", "You have received a gift. Check your storage box.",
  "The ranking reward will be sent to the character's Gift Box." คือ **Gift Box / Storage Box ของตัวละคร**
- Ref Server: ร้านในเกม `C9NetworkInGameShop` (C2S 0x0D83–0x0D91 ผ่าน world server) เก็บของด้วย `UspSendPcMailEx`;
  ฝั่งรับใช้ `GetPcMailExList` (0x0E97), `GetPcMailExContent` (0x0E9D), `GetMailExCount` (0x0EB1), `DeletePcMailEx` (0x0E99, 0x2334)
  จึงสรุปว่า Gift Box = ระบบ **MailEx** ใน Post (ต้องยืนยัน layout กับ Client ปัจจุบันตอนทำขั้น E)

### 1.3 Protocol ที่เกี่ยวข้อง (ชื่อ class จาก Client ปัจจุบัน, handler/DB query จาก Ref Server 2014)

| ระบบ | Opcode | หลักฐาน |
|---|---|---|
| Mail = **Post** | C2S คี่ 0x0E8F–0x0EB1, S2C คู่ 0x0E8E–0x0EB4 (`C9NetworkPost` 12 ตัว) | Ref: 0x0E97 รายการ (`PSQueryGetPcMailList/ExList`), 0x0E93 เปิด (`OpenPcMail`), 0x0E95 เนื้อหา, 0x0E91/0x0E99 ลบ, 0x0E9B รับของแนบ (ส่ง 0x0CF0, 0x0EA1, 0x0EB3), 0x0EB1 นับ mail; DB `_UspSendPcMail`(15), `UspSendPcMailEx`(11), `UspOpenPcMail`(36) |
| คลัง = **Store/Warehouse** | `C9NetworkStore` (0x0BC2, 0x0CFE–0x0D10, 0x4589…) | Ref: 0x0D0A ฝาก/ถอน (`PSQueryPushItemToWarehouse/PopItemFromWarehouse`), 0x0D0C เงิน; DB `UspGetPcStoreRawData`, `UspPushItemToStore`, `UspAddPcMainStoreSlots` — ต้องเปิดผ่าน NPC เก็บของ และยังไม่ยืนยันว่าเป็นคลังรวมของบัญชีหรือแยกตัวละคร |
| Inventory | `C9NetworkInven` (0x0BC1, 0x0C22, 0x0CFB–0x0CFD, 0x0D9D–0x0DAD ฯลฯ) | server ของเรายังไม่ส่ง inventory เลย (ตัวละครเข้าเกมด้วยกระเป๋าว่าง) |
| ของถาวร/มีวันหมดอายุ | DB `UspPushItemDateEnd`(30) | น่าจะเป็นทางส่งของจากระบบ (เช่น item mall) |

## 2. ข้อสรุปสำคัญ

ทุกทางที่ให้ item ไปถึงตัวละคร (mail, คลัง, starter) ต้องมี **ระบบ inventory ฝั่ง server** ก่อน คือส่งรายการ item ให้ Client ตอนเข้าโลก
และรู้ layout ของ item 1 ชิ้นใน packet (serial, item id, จำนวน, enchant, option, วันหมดอายุ) ซึ่งยังไม่ได้ RE

## 3. แผนที่เสนอ (เรียงตามสิ่งที่ทำได้ก่อน)

| ขั้น | งาน | ต้อง RE ไหม | ขึ้นกับ |
|---|---|---|---|
| A | Item Catalog: import 45,283 รายการ (id/ชื่อ ENG+KOR/คำอธิบาย) เข้า `game.item_catalog`, หน้า Portal ค้นหา/ดู item | ไม่ (ทำได้เลย) | รวม migration กับเธรด 2nd password |
| B | Portal "ส่ง item": คิวรายการให้ของ (ตัวละคร, item, จำนวน, ช่องทาง mail/คลัง, เหตุผล, audit) เก็บใน DB ก่อน แล้วค่อยส่งเข้าเกมเมื่อขั้น C–E พร้อม | ไม่ | A |
| C | Inventory sync ตอนเข้าโลก (item จาก `game.character_items`) | **ต้อง RE**: reader ของ 0x0BC1 / item block ใน 0x0BC0 | — |
| D | Starter items: ตั้งชุดของเริ่มเล่นต่ออาชีพใน Portal ให้ตอนสร้างตัวละคร | ไม่ (DB) + ใช้ C ให้เห็นในเกม | C |
| E | Mail (Post) พร้อมของแนบ: รายการ, เปิด, รับของ | **ต้อง RE**: layout 0x0E98/0x0E96/0x0E9C และ flow 0x0E9B | C |
| F | คลัง (Warehouse) | **ต้อง RE** + ต้องมี NPC เก็บของ | C และเธรด NPC |

ค่าคุณสมบัติ item จาก `GameData.c9d` (stack/ประเภท/อาชีพ) ทำคู่ขนานได้ แต่ไม่ขวางขั้น A–B (กรอกจำนวนเองได้)

## 4. คำขอเพิ่มของผู้ใช้ (20 พ.ย. 2026) และการตีความ

- เมนู **Items Catalog** พร้อมรูปของ item: ขั้น A (รูปจาก Ui_Icon แปลงเป็น PNG ตอน import)
- เมนู **Items management**: สร้าง item bundle, ส่งให้ Character หรือ Account แบบ single และ batch: ขั้น B
  - ตีความ "ส่งให้ Account" = ของเข้ากระเป๋ารับของของบัญชี ตัวละครไหนของบัญชีนั้นรับก่อนก็ได้ (รับได้ครั้งเดียว)
  - batch = วางรายชื่อตัวละคร/บัญชีหลายรายการ หรือเลือกทุกบัญชีที่ active; ทุกรายการมี audit และ request id กันส่งซ้ำ
  - ก่อนขั้น C/E เสร็จ รายการส่งจะอยู่สถานะ "รอส่งเข้าเกม" ใน Portal (ยังไม่เห็นในเกม)
- ผู้ใช้เห็นด้วยกับลำดับ A → B → C → D → E → F

## 5. Current State (20 พ.ย. 2026 ~19:50 น.)

**ขั้น A และ B เขียนเสร็จ ทดสอบผ่าน commit แล้ว ยังไม่ deploy** (Docker ตอนนี้รันโค้ดของเธรด Sprint 1 จาก worktree `sprint1-world-session` ต้องรวมก่อน deploy)

| ส่วน | ไฟล์ | รายละเอียด |
|---|---|---|
| ตัวอ่าน CFS | `re/scripts/c9_cfs.py` (+ test) | รองรับ header 24 ไบต์ของ Client 2026 และ filler ก่อน LZMA; เพิ่ม `read()` อ่านทั้งไฟล์ (จำกัด 128 MB) |
| สคริปต์ export | `re/scripts/export_item_catalog.py` (+ `test_export_item_catalog.py`) | อ่านโฟลเดอร์เกม → `items.json` (id, ชื่อ EN/KO, คำอธิบาย EN, icon) + `icons/*.png`; รันจริง 44 วินาที ได้ 45,996 item, มีรูป 38,638, PNG 10,584 ไฟล์ (DDS ที่ไม่ใช่ 32 บิต 27 ไฟล์ข้าม) ผลอยู่ที่ `artifacts/docker-state/item-catalog` (นอก git) |
| DB | `Entities/Items.cs`, `CatalogItem` (+DescriptionEn, Icon), migration `20261120063022_ItemCatalogDelivery`, `db/migrations/game-data.sql` | ตาราง `game.item_bundles`, `item_bundle_entries`, `item_deliveries` (1 แถวต่อผู้รับต่อ stack, batch id = request id, สถานะ Pending/Delivered/Cancelled) |
| Service | `Services/ItemService.cs` | ค้นหา catalog (ID หรือชื่อ EN/KO, กัน LIKE wildcard), import (เพิ่ม/อัปเดต ไม่ลบ, icon key ไม่ปลอดภัยถูกตัด), bundle สร้าง/แก้ (xmin), ส่งของให้ตัวละคร/บัญชี/ทุกบัญชี active (ทั้งหมดหรือไม่มีเลย, สูงสุด 5,000 ผู้รับ/100,000 แถว), ประวัติ, ยกเลิกทั้ง batch ที่ยัง Pending |
| Portal | `src/C9.Portal.World/ItemsModule.cs`, `Pages/ItemsCatalog.razor` (`/items`), `Pages/ItemsManage.razor` (`/items/manage`), `compose.yaml` (`Items__CatalogDirectory`) | เมนูกลุ่ม Items: Items Catalog (ค้นหา, รูป, ปุ่มนำเข้า) และ Items management (ส่งของ, bundle, ประวัติ/ยกเลิก); REST `/api/v1/items/...`; รูปเสิร์ฟจาก `/api/v1/items/icons/{key}.png` (ชื่อไฟล์ whitelist กัน path traversal) |

สิทธิ์: ดู catalog = WorldRead, นำเข้า/แก้ bundle = WorldWrite (Owner/Admin/ContentEditor), ส่ง/ยกเลิก = AccountsWrite (Owner/Admin/GM), หน้า management = AccountsRead

การทวนสอบ (Windows CNNB886, PostgreSQL 18 ใน Docker): GameData **22/22** (182 assertions), Portal **12/12** (135), C9.Validation **73/73** (7,306), Python **14/14**; build 0 warning

วิธีใช้หลัง deploy: 1) `python re/scripts/export_item_catalog.py --game C:\VFUN\C9-GL --out artifacts/docker-state/item-catalog`
2) Portal → Items Catalog → ใส่เหตุผล → "นำเข้า catalog" 3) Items management → สร้าง bundle / ส่งของ

ยังไม่ทำ: ขั้น C–F (ของยังไม่เข้าเกมจริง รายการค้างสถานะ "รอส่งเข้าเกม"), คุณสมบัติ item (ประเภท/stack/อาชีพ), starter items
