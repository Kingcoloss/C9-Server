# ตัวเปิด local สำหรับ Official Client รุ่น 52893 และ 55350

อัปเดต 7 พฤศจิกายน 2026

ใช้ `C9LocalLauncher.exe` ที่สร้างจากโครงการนี้เปิดเข้า local lab และใช้
**VFUN Launcher** ของ VALOFE สำหรับช่องทาง Official ตัวเปิด local ไม่แก้
`C9.exe`, `C9Launcher.exe`, `C9Patcher.exe`, `C9Server.ini` หรือ `VerCli.C9`
และไม่ใช้ awesomium.dll/icudt.dll จาก launcher เก่า ไฟล์ที่ผู้ใช้คัดลอกไว้ยังคงอยู่

## สิ่งที่ทำได้และผลทดสอบจริง

- UI ภาษาไทยตรวจ SHA-256 ของ client และตรวจ greeting ของ server ก่อนเปิดเกม
- กดเปิดเกมเพื่อเริ่ม backend ที่ `127.0.0.1:14000` อัตโนมัติเมื่อยังไม่มีบริการ
  บันทึกข้อมูลชนิด/ขนาด packet ใน `L9.server.log` โดยไม่มี payload หรือข้อมูลยืนยันตัวตน
- เปิด C9 ด้วยรูปแบบพารามิเตอร์ที่อ่านโดย client ใหม่ แสดงสถานะเริ่มเกม/รอเกม/เกมปิด
- ปุ่มแยกสำหรับโฟลเดอร์เกมและ VFUN Launcher ที่ติดตั้งไว้ หากยังไม่กำหนด path
  จะเปิดหน้าดาวน์โหลด VFUN ทางการ
- บัญชีและค่าพารามิเตอร์เป็นข้อมูลสมมติสำหรับ local lab เท่านั้น ยังไม่มีระบบยืนยันบัญชีจริง
- ชุดติดตั้งสำหรับเครื่องทดสอบ Windows ARM64 รวม runtime ไว้ใน exe แล้ว
  การตรวจ `--check` ผ่านและไฟล์ Official ทั้งห้ารายการมี hash เท่าเดิม
- ทดสอบปุ่มเปิดเกมจริงเวลา 16:16 น. Server เริ่มได้ และ C9 เริ่ม GameMon.des
  แต่ประมาณ 4 นาทีต่อมาเกมแสดง **GameGuard Init Error (Code: 114)**
  ยังไม่มีคำขอจากเกมถึง local endpoint; การเชื่อมต่อที่พบใน log เป็น readiness probe ของ UI
- เครื่องทดสอบเป็น Windows 11 ARM64 บน QEMU VM ยังไม่ได้ยืนยันการเปิด client
  รุ่นใหม่นี้ผ่าน VFUN Launcher จนถึงหน้าจอเกมบนเครื่องเดียวกัน
- ผู้ใช้รายงานว่าเข้าสู่ระบบผ่าน launcher Webzen เดิมไม่ได้ ปุ่ม Official รุ่นแรกของเรา
  ชี้ผิดไปที่ `C9Launcher.exe` ในโฟลเดอร์เกม จึงแก้ให้ชี้ไป VFUN ที่ติดตั้งแยก
  ผลการเข้าสู่ระบบเดิมยังใช้เป็นผลทดสอบการเริ่มเกมผ่าน VFUN ไม่ได้

### หลังอัปเดตผ่าน VFUN: Build 55350

ผู้ใช้ดาวน์โหลดแพตช์และรายงานว่า nProtect อัปเดตได้เมื่อเปิดจาก Official
ไฟล์เกมเปลี่ยนเป็น SHA-256
`9c6816ab93a6de55e93a9c98cb3837879f0d8e9682e94932d122cf7995845529`
ขนาด 6,791,064 ไบต์ และลายเซ็น VALOFE ผ่านการตรวจ
ทดสอบเปิดจาก VFUN ที่ล็อกอินแล้วจริง: เกมแสดง Build 55350 และถึงหน้าเลือกโลก
โดยมี Acharon / Good จึงมีผลทดสอบ Official ที่สำเร็จสำหรับ client หลังแพตช์แล้ว

การเปิดของ VFUN ใช้ argument หนึ่งชุด คั่นด้วย colon 11 ช่อง ช่อง 0 เป็น
`fromVLauncher` ช่อง 1/7 ว่าง ช่อง 2 เป็น VALOFE, ช่อง 4 เป็น c9-gl,
ช่อง 8/9 เป็น endpoint และช่อง 10 เป็น EN
เก็บเฉพาะโครงสร้าง ไม่เก็บค่า account/token ของผู้ใช้ ตัวเปิด local ทดลองใช้รูปแบบนี้
กับข้อมูลสมมติและ endpoint loopback ยังไม่ได้ยืนยันความหมาย/รูปแบบครบทุกช่อง

ผล local ที่ตรวจแล้ว:

- ชุด arguments ของรุ่นก่อนทำให้เกมรายงาน `open version.ini failed!`
- ทดลองไฟล์ endpoint ขั้นต่ำแล้วเปลี่ยนเป็น `Get url failed!` จึงนำไฟล์ทดลองออก
  และกลับไปตรวจ flow ของ VFUN ไม่มี version.ini ทดลองเหลือในโฟลเดอร์เกม
- เปลี่ยนเป็นโครงสร้าง 11 ช่องด้วยค่าทดสอบ: เกมปิดก่อนเชื่อม local
  Windows บันทึก `0xC0000005`, module offset `0x0045E47E`
- ตอน 21:18 น. ตัวเปิดเริ่ม observer และเกมได้ แต่ไม่มี game packet เข้า observer
  การเชื่อมต่อใน log รอบนั้นเป็น readiness probe
- หลังแก้ marker เป็น `fromVLauncher` และรูปแบบช่องสมมติ เกมถึงหน้ากำลังโหลด
  และส่ง `0x2F04` ขนาด payload 1,092 ไบต์ แบบ plain (flag 0, sequence 0)
  เข้า local จริงสองรอบ เวลา 21:53:36 และ 21:56:07 น. วันที่ 6 พฤศจิกายน
  จึงผ่านจุดปิดก่อนเชื่อมต่อของการทดลองก่อนหน้าแล้ว
- เพิ่ม `config/lab-official-login-fixture.json` สำหรับทดลองตอบ `0x2EE2`
  และรายการ `0x2EE6` แบบ plain โดยอ้างอิง reader ของ build 52893 และ fixture เดิม
  ยังไม่ยืนยันว่า build 55350 รับผลตอบนี้ จึงแยกจาก observer และ lobby ของ client เดิม
- รอบควบคุมผ่าน VFUN วันที่ 7 พฤศจิกายนกลับขึ้น GameGuard Error 114
  ก่อนส่งข้อมูลเกม ไม่พบ packet ในช่วงที่สังเกต จึงยังไม่มีผลตอบจริงจาก Official
  สำหรับยืนยันโครงสร้าง login ใหม่

ขั้นถัดไปคือยืนยันผลตอบ login/list กับ build 55350 ก่อนทำ local account/ticket
และ handoff/character/world ของ build นี้ การที่ UI/ชุดทดสอบผ่านยังไม่หมายความว่า build นี้เล่น local ได้
ดูหลักฐานสรุป `re/evidence/static/windows_official_55350_startup.json`

`C9Launcher.exe` รุ่น 1.0.0.26 ที่มากับโฟลเดอร์ยังอ่าน auth URL ของ Webzen
จาก GameInfo.txt ขณะที่ C9 ย้ายบริการมา VALOFE/VFUN แล้วตาม
[ประกาศ C9 บน Steam](https://store.steampowered.com/news/posts/?appids=212390&enddate=1634123662&feed=steam_community_announcements).
หน้า [VFUN Launcher Download](https://vfun.valofe.com/game/download) เปิดได้จริงใน Edge
ส่วนหน้า C9 เก่าที่ `c9-gl.valofe.com` เปลี่ยนทางไปหน้า 404 ในรอบตรวจนี้

Error 114 ยืนยันว่าการเริ่ม GameGuard ไม่สำเร็จ แต่ยังแยกไม่ได้ว่าเกิดจาก
สภาพแวดล้อม ARM/VM, โปรแกรมที่ทำงานอยู่, ไฟล์/การอัปเดต หรือปัจจัยอื่น
Microsoft ระบุว่า driver ต้องสร้างให้ตรงกับสถาปัตยกรรม OS แม้ตัวแอป x86 จะรัน
ผ่าน emulation ได้ ([เอกสาร Microsoft](https://learn.microsoft.com/en-us/windows/arm/apps-on-arm-troubleshooting-x86)).
นี่เป็นประเด็นตรวจสอบ ไม่ใช่ข้อสรุปว่า GameGuard รุ่นนี้ไม่รองรับ ARM

Error 114 เคยหายหลังผู้ใช้โหลดไฟล์ nProtect ใหม่ด้วยการกด GAME START จาก VFUN
และกลับมาอีกครั้งในรอบวันที่ 7 พฤศจิกายน การเปิด VFUN เฉย ๆ ไม่ใช่ขั้นที่โหลด GameGuard
ยังไม่มีการแก้ ปิด หรือข้าม GameGuard และยังไม่ย้าย lobby/world profile เดิมมาใช้กับ build ใหม่

## ติดตั้งจาก source

ต้องมี .NET 10 SDK และ Python สำหรับขั้น build/install; ตัวเปิดที่เผยแพร่เป็น
self-contained ไม่ต้องใช้ SDK ในการดับเบิลคลิก ส่วน backend ยังใช้ .NET/ASP.NET Core runtime
ที่ระบุในไฟล์ติดตั้งของเครื่องนั้น

```powershell
dotnet publish src/C9.Server/C9.Server.csproj -c Release -o artifacts/official-backend
dotnet publish src/C9.LocalLauncher/C9.LocalLauncher.csproj -c Release -r win-arm64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o artifacts/local-launcher-standalone --source https://api.nuget.org/v3/index.json

# ตรวจรายการที่จะติดตั้งก่อน; ใส่ --apply เพื่อเขียนไฟล์จริง
python re/scripts/install_local_launcher.py --client-dir 'F:/C9NewClientVersion' --backend 'F:/C9-Server/artifacts/official-backend/C9.Server.dll' --dotnet 'C:/Users/kanganapong.s/.dotnet/dotnet.exe' --official-launcher 'C:/VFUN/VLauncher/VFUNLauncher.exe'
```

ค่าปริยายยังเป็น observer ถ้าต้องทดสอบผลตอบ login ที่ยังไม่ยืนยัน ให้เพิ่ม
`--backend-config config/lab-official-login-fixture.json` และตรวจ preview ก่อน `--apply`
ชุดนี้รองรับเฉพาะ loopback และยังไม่มี handoff หรือ character/world ของ build ใหม่

ผลตรวจชุดทดลอง 7 พฤศจิกายน: build ไม่มี warning/error, validation ของ server
ผ่าน 45 ชุด / 6,323 assertions, ตัวติดตั้งผ่าน 8 ชุด และทดสอบ TCP กับ backend
ที่ติดตั้งแล้วได้รับ greeting → 2EE2 → 2EE6 ครบ ผล TCP นี้ใช้ client จำลอง
จึงยังไม่ถือเป็นการยืนยันการเข้าเกมจริง

เครื่องมือ `re/scripts/observe_game_frames.py` ใช้สังเกตชนิด/ขนาดข้อความของ endpoint
ที่เลือกผ่าน tshark โดยไม่เขียน pcap หรือเก็บ packet body และรายงานเมื่อมีช่องว่างของ TCP
การตรวจ `.erl` แยกต่างหากยังไม่ได้ถอดรหัสเนื้อหาภายใน GameGuard

ตัวติดตั้งตรวจ ownership จาก record ใน `artifacts/client-backups/dual-launcher-*/`
เก็บไฟล์ local ชุดก่อนหน้า และแทนที่หรือลบเฉพาะไฟล์เก่าที่ hash ยังตรงกับ record
ไฟล์ที่ผู้ใช้แก้หรือคัดลอกเองจะไม่ถูกเขียนทับ ไฟล์ record และ local config ไม่อยู่ใน Git
ใช้ `win-x64` เมื่อต้อง build สำหรับเครื่อง Intel/AMD และปรับ path ของ backend/runtime
ให้ตรงกับเครื่องนั้น

## รูปแบบเริ่มเกมและการอัปเดตในอนาคต

Client SHA-256 ที่ตรวจแล้ว:
`704687c39a293db92999c8a1ef5f22d9e147d743a94d3decb3de0a35af69dd63`
สำหรับรุ่น 52893; รุ่น 55350 ใช้ hash ที่ระบุในผลหลังอัปเดตข้างต้น

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
