# C9 Server Manager

C9 Server Manager เป็นแอป Windows ที่ใช้ .NET MAUI Blazor Hybrid สำหรับ Build 55350
เปิด `artifacts/admin-desktop/win-arm64/C9.AdminDesktop.exe` บนเครื่อง ARM64
หรือ `artifacts/admin-desktop/win-x64/C9.AdminDesktop.exe` สำหรับ build x64
เก็บไฟล์ในโฟลเดอร์ build เดียวกันรวมถึงโฟลเดอร์ย่อย `game`
แอปใช้สิทธิ์ของผู้ใช้ Windows ที่เปิดแอป

1. เปิด Manager แล้วกด **Start all** ใน **Server & Monitor**
2. เมื่อ Login และ Character & World ขึ้น **Running** ให้เปิด C9 Local Launcher
3. เปิด Manager ค้างไว้ขณะเล่น ใช้ **Stop all** เพื่อปิด หรือปิดหน้าต่าง Manager
   เพื่อปิดบริการที่ Manager เปิดไว้

Monitor แสดงพอร์ต PID เวลาเปิดใช้ หน่วยความจำ จำนวน TCP และเหตุการณ์ล่าสุด
จำนวน TCP รวมการเชื่อมต่อทดสอบ พอร์ตที่เปิดจากโปรแกรมอื่นจะแสดง **External**
ให้ปิดบริการจากโปรแกรมที่เปิดพอร์ตไว้ก่อนเริ่มจาก Manager

- **บัญชีและส่งของ** แสดงบัญชี Local และตัวละครจากไฟล์เซฟจริง เปิดหรือระงับสิทธิ์
  เข้าบัญชีได้ หลังบันทึกให้ Restart all เพื่อใช้สถานะใหม่ ระบบเกมยังใช้บัญชีทดสอบเดียว
- **คิวรอส่ง** บันทึกรายการไอเท็มตามรหัส Gold และ EXP ให้ตัวละครที่เลือก
  คิวรอระบบคลังและระบบรางวัลในเกม และรหัสไอเท็มยังต้องตรวจจากข้อมูลเกม
- **ตั้งค่าเกม** ปรับ HP/MP เริ่มต้น จำนวนการเชื่อมต่อ และ timeout มีผลหลัง Restart all
  ตัวคูณ Drop, Rare Drop, Upgrade และ EXP แยกทั้งเซิร์ฟเวอร์ Event และบัญชี Local
  บันทึกไว้รอระบบเกมที่ใช้ค่าเหล่านี้

ค่าและคิวอยู่ใน `artifacts/admin-console` บริการที่เปิดจาก Manager หรือ Web Console
ใช้ค่าชุดเดียวกัน ตัวเปิดเกมใช้บริการที่เปิดอยู่ตามพอร์ตเดิม

## สร้างแอป Windows

ใช้ .NET 10 SDK และ WebView2 Runtime จาก PowerShell ในโฟลเดอร์โครงการ

```powershell
dotnet workload install maui-windows --source https://api.nuget.org/v3/index.json
.\scripts\Build-C9AdminDesktop.ps1 -Launch
```

สคริปต์เลือก ARM64 หรือ x64 ตามเครื่อง ใช้ `-RuntimeIdentifier win-x64` หรือ
`-RuntimeIdentifier win-arm64` เพื่อระบุเอง

## Web Console

เปิด Console บนเครื่องที่รัน C9 Server ด้วย .NET 10 SDK จาก PowerShell ในโฟลเดอร์โครงการ

```powershell
.\scripts\Start-C9Admin.ps1 -ClientDirectory 'F:/C9NewClientVersion'
```

เปิด [Admin Console](http://127.0.0.1:14880/) แล้วคัดลอกกุญแจจาก
`artifacts/admin-console/access-key.local.txt` ลงในหน้าเข้าสู่ระบบ สคริปต์สร้างกุญแจ
ในครั้งแรกและใช้กุญแจเดิมในการเปิดครั้งถัดไป `-ClientDirectory` ใช้ระบุที่อ่าน log
จาก Local Launcher สามารถละไว้ได้ถ้าจะเปิดบริการจาก Console อย่างเดียว

- **Start all** เปิด Character Lobby ที่พอร์ต `14001` ก่อน Login ที่พอร์ต `14000`
  เมื่อขึ้น Running ทั้งคู่ เปิดเกมจาก C9 Local Launcher ได้ตามปกติ
- **Start, Stop และ Restart** ใช้ควบคุมบริการแยกกัน Stop และ Restart จะตัดการเชื่อมต่อ
  ของบริการที่เลือก การปิด Console จะปิดบริการที่ Console เปิดไว้ด้วย
- **Monitor** อัปเดตทุก 5 วินาที แสดงสถานะ พอร์ต PID เวลาเปิดใช้งาน หน่วยความจำ
  จำนวนการเชื่อมต่อ TCP และเหตุการณ์ล่าสุด ตัวเลข TCP รวมการเชื่อมต่อทดสอบ
- **ประวัติคำสั่ง** เก็บคำสั่ง เวลา UTC บริการเป้าหมาย และผลที่ `artifacts/admin-console/operations.jsonl`
  แต่ละรอบที่เปิดบริการจาก Console มีไฟล์ metadata แยกใน `artifacts/admin-console/logs`

ถ้า Local Launcher หรือโปรแกรมอื่นเปิดพอร์ตไว้ก่อน Console จะแสดงสถานะ External
ให้ปิดบริการจากโปรแกรมที่เปิดไว้ก่อนใช้ Start ใน Console
โหมดนี้ใช้กับเซิร์ฟเวอร์ทดสอบในเครื่อง และใช้กุญแจสำหรับผู้ดูแลคนเดียว
