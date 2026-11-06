# ทำต่อหลังปิดเครื่อง — 4 พฤศจิกายน 2026

source, Client และหลักฐานจากวันที่ 3 พฤศจิกายนยังอยู่ครบ
ตรวจ C9.exe ได้ hash เดิม `b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25`
และ PhysXLoader.dll ยังเป็น promotional-dialog patch สองไบต์ตาม
[startup notes](WINDOWS_CLIENT_STARTUP_TH.md)

## ผลรอบก่อนที่ใช้แก้ต่อ

ปลายทาง 14001 รับ 1771 / payload 49 / sequence 0 และ 9999 / payload 8 /
sequence 1 ได้แล้ว แต่เกมส่ง 2EE5 / empty payload / sequence 2 ตามมา
ขณะยังรอ application reply; fixture ไม่มี rule จึงเป็นผู้ปิด connection
ยังไม่ใช่หลักฐานว่า heartbeat จำเป็นต้องมี echo หรือ response type ใด

snapshot `c9-game-keepalive-20261103-211305-snapshot.pcapng` มี 49 TCP packets
และ 14 decoded C9 frames รวมการ reconnect ก่อน flow หลัก
SHA-256 `5d83c971b770fa8fc8b804a35889b53116e0c3b616bd8b4d3beae8929aea7f38`
metadata อยู่ใน artifacts/diagnostics/keepalive-fixed-capture-metadata.json
raw payload/credentials ไม่ถูกส่งออกในรายงาน

## โหมด observer

เพิ่ม profile `observer` ทั้ง .NET runtime และ Python reference:

- configuration อนุญาตเพียง plain type-4 greeting หนึ่งข้อความ, mode 0 หรือ 1
- ไม่มี application response rules และไม่ส่ง auth/character/world replies
- หลัง greeting รับทุก application type ที่ผ่าน framing, crypto, checksum และ
  sequence validation แล้วบันทึกเฉพาะ metadata ด้วย state `observing`
- ใช้ connection/frame/timeout/packet/cipher-epoch limits เดิม

config/lab-handoff-probe.json ใช้ observer + mode 1, loopback 14001,
64 packets, idle deadline 120 วินาที; จุด 14000 ยังใช้ explicit fixture
สำหรับ login/list/selection และส่ง handoff กลับไปยัง 14001
การเก็บข้อมูลต่อได้ไม่ใช่การยืนยัน authentication หรือความเข้ากันได้กับโลกเกม

## การตรวจสอบ

Release build ไม่มี warning/error; .NET full **39/39**, **2,919 assertions**,
Python protocol/server **19/19**

regression ครอบคลุม 1771 → 9999 → 2EE5 → 9901 → unknown type → 9999
บน connection เดียวโดย sequence ต่อเนื่องและไม่มี application reply
ตรวจ connection ใหม่เริ่ม sequence 0, metadata ไม่เก็บ payload และ malformed
padding ยังทำให้ connection ปิดตาม protocol validation
configuration ที่แทรก auth reply/rule, greeting หลายข้อความ หรือ mode ไม่ถูกต้อง
ใน observer ถูกปฏิเสธ

เริ่มบริการกลับด้วย:

```powershell
& .\scripts\Start-C9Lab.ps1 -GameProfile fixture -OpenLauncher
& .\scripts\Start-C9Capture.ps1 -Seconds 600 -IncludeHandoff
```

launcher แสดง Patch success complete. แล้วหลังเริ่มบริการใหม่
เกมจริงเชื่อมต่อ 14001 เวลา 02:11:31 และส่ง 64 ข้อความในประมาณ 100 วินาที
ลำดับ 0–63 ต่อเนื่อง รวม 1771 หนึ่งข้อความ, 9999 จำนวน 33 ข้อความ,
2EE5 จำนวน 20 ข้อความ และ 9901 จำนวน 10 ข้อความ ฝั่ง observer ส่งเฉพาะ
greeting ผู้ใช้เห็น `Connecting` แล้วเห็น `Server Connection lost` เมื่อ
observer ถึงขีดจำกัด 64 ข้อความและปิดการเชื่อมต่อรอบทดสอบ
เวลาจับ Wireshark หมดก่อนผู้ใช้เริ่มล็อกอิน รอบนี้ยืนยันจากบันทึกฝั่ง server

## จุดวิเคราะห์ถัดไปที่เก็บไว้แล้ว

เพิ่ม RTTI map ของ C9NetworkBase, C9NetworkDefault และ C9NetworkCharacter ใน
re/evidence/static/windows_game_network_rtti.json และ export readers ที่
re/evidence/ghidra/windows_game_entry.c/.asm.txt จาก PE hash เดิม
Character handler 00A391C0 มี assembly export แต่ decompiler ถึงขีดจำกัดเวลา;
จึงไม่อ้างว่าถอด schema ของ handler นี้ครบแล้ว
ยังต้องตาม dispatch/registration ไปยังคำตอบของ 1771 และ character/lobby flow
ก่อนสร้าง application reply ที่มีความหมายตรงกับ Client

วันที่ 5 พฤศจิกายน อ่าน RTTI จาก process จริงแบบ read-only แล้วพบว่า stage
manager ที่ `019207e8` มี slot 2 เป็น `C9StageLogin`, slot 3 เป็น
`C9StageMapLoad`, slot 4 เป็น `C9StageWorld` และ slot 5 เป็น `C9StageCharacter`
branch `2EE9` เรียก `00D52AB0` เพื่อเขียน state 7 ใน `C9StageCharacter`
และเมื่อ stage นี้ประมวลผล state 7 ที่ `00D53210` จะขอเปลี่ยนไป slot 4
จึงยังต้องหาข้อความที่ทำให้ client ออกจากขั้น Connecting ไปถึง character stage
ก่อนนำ `2EE9` มาใช้ ไม่ได้เพิ่มข้อความนี้เป็นคำตอบของ 1771 ใน fixture
