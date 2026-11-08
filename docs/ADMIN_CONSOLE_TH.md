# C9 Admin Console

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
