# การเดินของตัวละครในเกม (0x0C1E move speed) — Current State

อัปเดต 15 พฤศจิกายน 2026 (Asia/Bangkok) โดย thread "Make the character move in game"
มาตรฐานอ้างอิง ISO/IEC 27001, ISO 9001, ISO/IEC 29110 (บันทึก work product + หลักฐานการตรวจ)

## 1. สรุปสั้น

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
