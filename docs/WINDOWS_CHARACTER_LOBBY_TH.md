# ทดสอบเลือกและสร้างตัวละครในเครื่อง

เริ่ม launcher ที่แพตช์แล้วพร้อมบริการเกมในเครื่องจาก PowerShell

```powershell
Set-Location 'F:\C9-Server'
& .\scripts\Start-C9Lab.ps1 -OfflineLauncher -GameProfile fixture `
    -HandoffConfig .\config\lab-character-lobby.json -OpenLauncher
```

ในเกมใช้บัญชีทดสอบ `test/test` แล้วเลือก `C9 Local` โปรไฟล์นี้มีบัญชีจำลองหนึ่งบัญชี
กับช่องตัวละคร 3 ช่อง และเก็บข้อมูลชื่อ คลาส และรูปลักษณ์ใน
`artifacts/lab-characters.local.json` ตัว launcher ใช้ไฟล์เวอร์ชันในเครื่อง
จึงเปิดบริการเกมเพียงที่ `127.0.0.1:14000` และ `127.0.0.1:14001`

## หลักฐานโปรโตคอล

| ข้อความ | ขนาดข้อมูล | หลักฐานจาก client |
|---|---|---|
| `1771` | 49 ไบต์ | คำขอแรกหลัง greeting mode 1 |
| `0EFA` | 42 ไบต์ + 136 ต่อหนึ่งตัวละคร | `00A3A5B2` อ่านรายการและจำนวนช่อง `00A3A8EA` เรียก `00DA6160` ให้ `C9StageWorld` เปลี่ยนไป `C9StageCharacter` |
| `0BBB` | 90 ไบต์ | `00A3C5F0` สร้าง packet และ `00A3B740` เขียน slot, template ID, ชื่อ UTF-16LE และรูปลักษณ์ |
| `0BBC` | 113 ไบต์ | `00A393F8` สร้างรายการใน client และ `00A3952B` เรียกตัวจัดการสร้างสำเร็จ |
| `0EFB` / `0EFC` | 8 / 53 ไบต์เมื่อไม่มีอุปกรณ์ | `00A3C8A0` สร้างคำขอด้วย ID ตัวละคร `00A3AABA` อ่าน ID และ `00A3AB89` นำรูปลักษณ์ไปใช้ |

รายการคลาสอ้างอิงตาราง asset ที่ client ชุดนี้โหลดจากข้อมูลเกม โดยรหัสคลาสอยู่ที่
record `+0x04` และ template ID อยู่ที่ `+0x48` ตาม reader `00499F20` และ `00728F60`
คำขอสร้าง Fighter ส่ง template ID 0 และ server แปลงเป็นรหัสคลาส 1 ก่อนบันทึก
ทุกข้อความยังผ่านการตรวจ framing, checksum, crypto และ sequence ตามเดิม

ปุ่ม Create Character ตรวจสิทธิ์สร้างจาก `0EFA` payload `+8` แยกจากจำนวนช่อง
ที่ `+40` ตาม reader `00A3A97C` และเงื่อนไข `00CF65DB` ถึง `00CF6624`
เมื่อค่าสิทธิ์เป็น 0 เกมแจ้ง `You cannot create any more characters.` แม้แสดง 0/3 ช่อง

ทดสอบกับเกมจริงวันที่ 5 พฤศจิกายน 2026 แล้วว่าสร้าง Fighter ชื่อ Lab เลเวล 1
ในช่อง 0 ได้ และหลังเริ่ม server ใหม่ยังโหลดชื่อ คลาส และรูปลักษณ์กลับมาได้
รายการ `0EFA` ต้องส่งค่าเวลาลบที่ row `+0x3C` เป็น `0xFFFFFFFFFFFFFFFF`
เหมือนค่าเริ่มต้นของ client หลังสร้าง ค่า 0 ทำให้ client แสดงว่าตัวละครรอลบ
หลักฐานจับข้อมูลที่ตรวจ framing, checksum และ sequence แล้วอยู่ที่
`re/evidence/static/windows_character_create_validation.json` และ
`re/evidence/static/windows_character_reload_start_validation.json`

ชุดทดสอบ TCP ตรวจการสร้าง การส่งคำขอซ้ำ การปฏิเสธความยาวผิด การอ่านชื่อ คลาส
และรูปลักษณ์หลังเริ่ม server ใหม่ รวมถึงข้อมูลเลือกตัวละครและแผนที่ชุดแรก
Release build ผ่านโดยไม่มี warning และชุดทดสอบ .NET ผ่าน 41/41 กรณี
รวม 6,205 assertions หลังเพิ่มการตรวจเส้นทางของทุกแผนที่วันที่ 6 พฤศจิกายน 2026

ปุ่ม Previous ของรายการที่มีตัวละครส่ง `2EEA` ขนาด 16 ไบต์ และรับ `2EEB`
เพื่อกลับ Choose World ส่วนอีกเส้นทางส่ง `0EC4` ว่างและรับ `2EEB` เช่นกัน
เมื่อกลับ Choose World แล้ว ใช้ `scripts/Update-C9Handoff.ps1` เพื่อ build และ
เปลี่ยนเฉพาะบริการตัวละคร โดยคงบริการ login ไว้ สคริปต์ปฏิเสธการเปลี่ยนขณะที่
client ยังเชื่อมต่อพอร์ต 14001 และตรวจ PID เวลาเริ่ม และ assembly ของบริการก่อนหยุด

คำสั่ง Start Game ส่ง `0BBF` ขนาด 8 ไบต์ ตอนนี้ server ตอบ `0BC0` ขนาด 7,444 ไบต์
และ `0BC3` ขนาด 46 ไบต์ เกมจริงเปลี่ยนไปหน้าโหลด Waterford Village ได้แล้ว
แก้ข้อความ `The map key entered does not exist. Map Key:` โดยส่งลำดับส่วนย่อย 0
ที่ payload `+4` แทนค่าเดิม 1 แล้ว ยืนยันว่า Waterford มี `_VILLAGE` เพียงรายการเดียว
รอบใหม่ client ส่ง `ED1D`, `0E4B`, `0BC4`, `0EFD` และ `0EB1` เพิ่มก่อนปิดตัว
Windows บันทึก `0xC0000005` จึงยังเข้าไปเดิน ทดสอบกระเป๋า การต่อสู้ หรือระบบในฉากไม่ได้ หลังเปลี่ยนสถานะ
`world_loading` server รับข้อมูลต่อแบบ observer เพื่อหาข้อความที่ยังขาด
ยังมีขีดจำกัด cipher ชุดแรก 256 เฟรมต่อทิศทาง และยังไม่มีระบบโลกเกมจริง

ตัวสร้าง `0BC3` ปัจจุบันใช้ catalog ร่วมทุกแผนที่แล้ว ตั้ง `initial_map_id`,
`initial_submap` และ `initial_position` เมื่อจำเป็นใน config ดูรายการครบและ
เงื่อนไขของไฟล์/พิกัดใน [ทะเบียนแผนที่](MAP_CATALOG_TH.md)
