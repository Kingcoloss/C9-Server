# ตัวเปิด local สำหรับ Official Client รุ่น 52893

อัปเดต 6 พฤศจิกายน 2026

ใช้ `C9LocalLauncher.exe` ที่สร้างจากโครงการนี้เปิดเข้า local lab และใช้
`C9Launcher.exe` ของ Official สำหรับอัปเดตเกมตามเดิม ตัวเปิด local ไม่แก้
`C9.exe`, `C9Launcher.exe`, `C9Patcher.exe`, `C9Server.ini` หรือ `VerCli.C9`
และไม่ใช้ awesomium.dll/icudt.dll จาก launcher เก่า ไฟล์ที่ผู้ใช้คัดลอกไว้ยังคงอยู่

## สิ่งที่ทำได้และผลทดสอบจริง

- UI ภาษาไทยตรวจ SHA-256 ของ client และตรวจ greeting ของ server ก่อนเปิดเกม
- กดเปิดเกมเพื่อเริ่ม observer ที่ `127.0.0.1:14000` อัตโนมัติเมื่อยังไม่มีบริการ
  บันทึกข้อมูลชนิด/ขนาด packet ใน `L9.server.log` โดยไม่มี payload หรือข้อมูลยืนยันตัวตน
- เปิด C9 ด้วยรูปแบบพารามิเตอร์ที่อ่านโดย client ใหม่ แสดงสถานะเริ่มเกม/รอเกม/เกมปิด
- ปุ่มแยกสำหรับโฟลเดอร์เกมและ Official Launcher
- บัญชีและค่าพารามิเตอร์เป็นข้อมูลสมมติสำหรับ local lab เท่านั้น ยังไม่มีระบบยืนยันบัญชีจริง
- ชุดติดตั้งสำหรับเครื่องทดสอบ Windows ARM64 รวม runtime ไว้ใน exe แล้ว
  การตรวจ `--check` ผ่านและไฟล์ Official ทั้งห้ารายการมี hash เท่าเดิม
- ทดสอบปุ่มเปิดเกมจริงเวลา 16:16 น. Server เริ่มได้ และ C9 เริ่ม GameMon.des
  แต่ประมาณ 4 นาทีต่อมาเกมแสดง **GameGuard Init Error (Code: 114)**
  ยังไม่มีคำขอจากเกมถึง local endpoint; การเชื่อมต่อที่พบใน log เป็น readiness probe ของ UI
- เครื่องทดสอบเป็น Windows 11 ARM64 บน QEMU VM ยังไม่ได้ยืนยันการเปิด client
  รุ่นใหม่นี้ผ่าน Official Launcher จนถึงหน้าจอเกมบนเครื่องเดียวกัน

Error 114 ยืนยันว่าการเริ่ม GameGuard ไม่สำเร็จ แต่ยังแยกไม่ได้ว่าเกิดจาก
สภาพแวดล้อม ARM/VM, โปรแกรมที่ทำงานอยู่, ไฟล์/การอัปเดต หรือปัจจัยอื่น
Microsoft ระบุว่า driver ต้องสร้างให้ตรงกับสถาปัตยกรรม OS แม้ตัวแอป x86 จะรัน
ผ่าน emulation ได้ ([เอกสาร Microsoft](https://learn.microsoft.com/en-us/windows/arm/apps-on-arm-troubleshooting-x86)).
นี่เป็นประเด็นตรวจสอบ ไม่ใช่ข้อสรุปว่า GameGuard รุ่นนี้ไม่รองรับ ARM

ขั้นตรวจถัดไปคือเปิดผ่าน Official Launcher ตามปกติบนเครื่องเดียวกัน
หากมีการลงชื่อเข้าใช้ ผู้ใช้ทำขั้นนั้นเอง แล้วเปรียบเทียบผลเริ่มเกมและ error
ยังไม่มีการแก้ ปิด หรือข้าม GameGuard และยังไม่ย้าย lobby/world profile เดิมมาใช้กับ build นี้

## ติดตั้งจาก source

ต้องมี .NET 10 SDK และ Python สำหรับขั้น build/install; ตัวเปิดที่เผยแพร่เป็น
self-contained ไม่ต้องใช้ SDK ในการดับเบิลคลิก ส่วน backend ยังใช้ .NET/ASP.NET Core runtime
ที่ระบุในไฟล์ติดตั้งของเครื่องนั้น

```powershell
dotnet publish src/C9.Server/C9.Server.csproj -c Release -o artifacts/official-backend
dotnet publish src/C9.LocalLauncher/C9.LocalLauncher.csproj -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o artifacts/local-launcher-standalone --source https://api.nuget.org/v3/index.json

# ตรวจรายการที่จะติดตั้งก่อน; ใส่ --apply เพื่อเขียนไฟล์จริง
python re/scripts/install_local_launcher.py --client-dir 'F:/C9NewClientVersion' --backend 'F:/C9-Server/artifacts/official-backend/C9.Server.dll' --dotnet 'C:/Users/kanganapong.s/.dotnet/dotnet.exe'
```

ตัวติดตั้งตรวจ ownership จาก record ใน `artifacts/client-backups/dual-launcher-*/`
เก็บไฟล์ local ชุดก่อนหน้า และแทนที่หรือลบเฉพาะไฟล์เก่าที่ hash ยังตรงกับ record
ไฟล์ที่ผู้ใช้แก้หรือคัดลอกเองจะไม่ถูกเขียนทับ ไฟล์ record และ local config ไม่อยู่ใน Git
ใช้ `win-x64` เมื่อต้อง build สำหรับเครื่อง Intel/AMD และปรับ path ของ backend/runtime
ให้ตรงกับเครื่องนั้น

## รูปแบบเริ่มเกมและการอัปเดตในอนาคต

Client SHA-256 ที่ตรวจแล้ว:
`704687c39a293db92999c8a1ef5f22d9e147d743a94d3decb3de0a35af69dd63`

ฟังก์ชัน `007E9480` อ่านสามส่วนที่คั่นด้วย comma ส่วน flow VFUN ใน
`007E9EC0` ยังเข้าถึง vector ที่แยกด้วย colon ตำแหน่ง 3/4/5/8/9
การใช้ argument แบบเก่าเพียงสามส่วนทำให้ access violation ก่อนเชื่อมต่อ
รูปแบบสำหรับ lab จึงมี colon สิบส่วนและระบุ endpoint ทั้งสองตำแหน่ง
ดู `LocalLaunchPlan.cs` และหลักฐานสรุปใน
`re/evidence/static/windows_official_52893_startup.json`

เมื่อ Official อัปเดตจน hash เปลี่ยน local launcher จะหยุดพร้อมข้อความให้ตรวจ
compatibility profile ใหม่ เพื่อไม่ใช้ layout/protocol เก่ากับ binary ที่ยังไม่ตรวจ
การรองรับ official updates จึงเป็นระบบจัดรุ่นและตรวจความเข้ากันได้ ยังไม่รับประกันว่า
client ทุกรุ่นในอนาคตจะเล่นกับ server นี้ได้ทันที

ตัวอ่าน CFS/GameData และ catalog ของ client เดิมยังใช้กับ GameData รุ่นใหม่นี้ไม่ได้
ต้องตรวจ format/content และ packet readers ของรุ่นใหม่ก่อนทดสอบตัวละครและทุกแผนที่ต่อ
