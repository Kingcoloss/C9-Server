# Windows initial handshake — 3 พฤศจิกายน 2026

อัปเดตต่อวันที่ 4 พฤศจิกายน: เปลี่ยนปลายทาง 14001 เป็น observer ที่รับข้อความ
ต่อเนื่องเพื่อไม่ตัดเมื่อพบ application type ใหม่ ดู
[WINDOWS_RESUME_20261104_TH.md](WINDOWS_RESUME_20261104_TH.md)
ผลและจำนวน tests ด้านล่างเป็นบันทึกของวันที่ 3 พฤศจิกายน

ผลล่าสุด: เกมแสดง **C9 Local / Good**, refresh list ทุกประมาณ 5 วินาทีได้
และทำ TCP handoff ไป 127.0.0.1:14001 ตามคำตอบ 2EED แล้ว
mode-1 greeting ที่จุดใหม่นี้ทำให้เกมส่ง 1771 / payload 49 / sequence 0 แล้ว
ยังไม่ยืนยัน authentication หรือการเข้าโลกเกม

Client ที่ใช้: C9.exe SHA-256
`b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25`
ตัว executable ไม่เปลี่ยน; PhysXLoader.dll ใช้ promotional-dialog patch สองไบต์
ตาม [startup notes](WINDOWS_CLIENT_STARTUP_TH.md)
ผู้ใช้กรอกบัญชีทดสอบเองในเกม ไม่มี authentication UI automation

## ผลรอบแรกกับ fixture

Server log `artifacts/diagnostics/first-game-fixture-exchange.log` บันทึก:

| ทิศทาง | Type | รายละเอียด |
|---|---|---|
| Server → game | 0004 | Plain greeting, wire 14 bytes |
| Game → server | 2EE1 | Encrypted, sequence 0, payload 2,162 bytes |
| Server → game | 2EE2 | Synthetic login reply, wire 36 bytes |
| Game → server | 2EE5 | Encrypted, sequence 1, empty payload |
| Server → game | 2EE6 | Synthetic one-entry list, wire 68 bytes |
| Game → server | 2EE5 | Encrypted, sequence 2, empty payload |

เกมแสดงหน้า server list เป็นแถวชื่อว่างและสถานะ Down จากนั้นแสดง
`Server connection lost.` เพราะ fixture เดิมไม่มี rule สำหรับ list request
ใน state `server_list_sent` ข้อมูลนี้ยืนยัน initial codec exchange กับเกมจริง
แต่ไม่ได้ยืนยันการตรวจรหัสผ่านหรือความหมายของ DWORD ใน login reply

Wireshark capture รอบแรกหมดเวลา 180 วินาทีก่อนผู้ใช้ลองและมี **0 packets**
จึงใช้ server metadata เป็นหลักฐานของรอบนี้ ไม่อ้างว่าได้ wire capture แล้ว
Raw logs/captures อยู่ใน artifacts ที่ไม่เข้า Git และไม่พิมพ์ payload/credentials

## การแก้ต่อจากผลจริง

- เพิ่ม list refresh rule ใน state `server_list_sent` ให้ตอบ 2EE6 ซ้ำ
  โดยรักษา codec/sequence ของ connection เดิม
- เพิ่มเวลา UTC ใน metadata events เพื่อเทียบเวลา request กับ capture
- เฉพาะ lab fixture ใช้ idle deadline 120 วินาที เพื่อให้มีเวลาตรวจ/เลือก server
  ส่วนค่า default ของ passive probe และขอบเขต codec ไม่เปลี่ยน
- Fixture ใหม่ใช้ชื่อ `C9 Local` แบบ UTF-16LE NUL-terminated ที่ entry +5
  และ byte +51 = 1 เพื่อทดลองผ่าน selection gate
- คง entry เป็น opaque 52 bytes ใน protocol API เพราะยังไม่รู้ทุก field
  ไม่สรุปว่า byte 1 คือ status enum ใดจนกว่าจะเห็นผลแสดงในเกม

หลักฐาน reader ที่อ่านจาก process แบบไม่แก้หน่วยความจำอยู่ใน
`artifacts/diagnostics/server-entry-readers.log`:

- Handler 00A621F0 ส่ง entry +5 ไปยัง 0045DFE0
- 0045DFFB อ่าน WORD, 0045E002 เลื่อนไปสองไบต์ และวนจน WORD เป็นศูนย์
- 0045E056 เรียก IAT 00F99308 ซึ่ง PE import ระบุ KERNEL32!WideCharToMultiByte
- 00B1D7CD / 00B1D7F1 อ่าน byte +33h; zero ไป error, nonzero ไป 00A631B0

Release build ผ่าน ไม่มี warning/error; .NET full suite **37/37**, 2,865 assertions
รวม regression ที่ขอ list สามครั้งบน connection เดียวและตรวจ sequence/entry
Python protocol/server suite **17/17** ผ่านหลังแก้ fixture
ผลนี้เป็นการทดสอบโปรแกรมที่เขียนใหม่ ผล runtime ที่ทำต่ออยู่ด้านล่าง

## ผลจากเกมและ Wireshark

เกมแสดงชื่อ C9 Local และสถานะ Good จาก entry ที่แก้แล้ว
ขอรายชื่อ 13 ครั้งบน connection เดียว, sequence 1..13 และรับคำตอบครบ
เมื่อกด Connect ส่ง 2EEC, sequence 14, payload 20 bytes, selected key 1
จากนั้น server รุ่นก่อนเพิ่ม handoff rule ปิด connection เพราะไม่มี rule นี้

Capture `c9-login-20261103-204041-591.pcapng` มี 67 packets, 8,964 bytes,
ไม่รายงาน dropped packets และถอด C9 frames ได้ครบ 30 ข้อความ:
0004 ×1, 2EE1 ×1, 2EE2 ×1, 2EE5 ×13, 2EE6 ×13, 2EEC ×1
SHA-256: `f4a374204ae3c909b6373bf4b4002da80e30cbaefb252ad83854ae1e937f5dd2`
นับเฉพาะ metadata ไม่ส่งออก login payload หรือ token

## Synthetic TCP handoff

เพิ่ม rule ตอบ 2EEC ด้วย 2EED และเปิด observer ที่ 14001
เกมยอมรับคำตอบ 52 wire bytes, ปิด connection เดิม และเชื่อมต่อ 14001 จริง
ทดสอบจากเกมสองรอบและจับ traffic ของทั้งสอง endpoint ได้

รูปแบบขั้นต่ำจาก reader ของเกม (offset ใน decoded packet):

| Offset | ขนาด | การใช้ใน Client |
|---|---|---|
| 15h | DWORD | Server lookup key; fixture = 1 |
| 1Ah | 4 bytes | TCP IPv4; fixture = 127.0.0.1 |
| 1Eh | WORD LE | TCP port; fixture = 14001; Client เรียก htons |
| 20h | DWORD | เส้นทาง endpoint อีกชุดผ่าน ntohl; fixture เป็น loopback |
| 24h | WORD LE | port ของ endpoint อีกชุด; ยังไม่มี UDP service |
| 26h | signed WORD | ค่าที่เก็บผ่าน 00A65930/00A65950; fixture = 1, semantics ยังไม่ยืนยัน |

ส่วนต้นแปดไบต์และ byte 19h เป็นค่าจำลอง ไม่อ้างว่าเป็น auth token ที่ถูกต้อง
rule เป็น fixed fixture สำหรับ entry 1 ไม่ใช่การ validate ตัวตนหรือ lookup key ทั่วไป

การทดลอง observer greeting mode 0 ทำให้เกมส่ง 2EE1 แล้ว 2EE5 ซ้ำไปจุดใหม่
โดยยังไม่มี auth reply จาก observer จึงหลุดจาก rule ที่ไม่รองรับ
เก็บ logs แยกเป็น `first-handoff-login-endpoint.log` และ
`first-handoff-mode0-endpoint.log` ใน artifacts/diagnostics ก่อน restart
ปรับ observer เป็น mode 1 ตาม branch ใน 00A621F0 ซึ่งสร้าง 1771 ผ่าน 00A64380
(decoded length 62 / payload 49) เพื่อทดสอบ game-endpoint handshake ถัดไป
observer รับและบันทึก metadata แต่ไม่ส่ง auth/world reply

ยืนยัน mode 1 กับเกมจริงแล้ว: ส่ง 1771, payload 49, sequence 0 จากนั้นประมาณ
3 วินาทีส่ง 270F (9999), payload 8, sequence 1 ซึ่ง fixture เดิมยังไม่รองรับ
ทำให้เกิด `Server connection lost.` เพิ่ม rule รับ 9999 ใน state นี้แบบไม่ส่ง
auth/world reply และรักษา sequence/state เดิมแล้ว ขยาย observer budget เป็น
64 packets เพื่อไม่ให้ครบ limit หลัง heartbeat เพียงไม่กี่ครั้ง
ผล runtime ของการรับ heartbeat ต่อเนื่องยังต้องตรวจรอบใหม่

`windows_game_handshake.c/.asm.txt` ยืนยัน constructor 00A6B3F0 (length 15h)
และ sender 00A68740: ตรวจช่วงเวลา 3000 ms แล้วใส่ GetTickCount DWORD กับ flag
อีก DWORD ก่อนส่ง ไม่สมมติ response type หรือ echo payload โดยไม่มี reader รองรับ
export จาก Ghidra project ใหม่ใน artifacts/ghidra-client แบบ noanalysis,
เฉพาะฟังก์ชันที่เลือก, 2 CPU / heap 2 GiB; ไม่ได้วิเคราะห์ทั้งโปรแกรม

snapshot `c9-game-greeting-20261103-210028-snapshot.pcapng` มี 34 TCP packets
และ 10 decoded C9 frames, รวม 1771 และ 270F พร้อมทั้ง login/list/handoff ก่อนหน้า
SHA-256: `96cee6f7656973ddad3756762c7189d8d769e32187213435f67eef6caa020cd7`
เก็บ snapshot แยกจาก capture ที่ยังทำงานอยู่เพื่อให้ hash คงที่

.NET suite เพิ่มกรณี selection reply และ codec reset บน connection ใหม่
รวม **38/38**, 2,913 assertions; Python protocol/server suiteยังผ่าน **17/17**

ตรวจ capture ซ้ำได้ด้วยเครื่องมือที่ใช้ tshark ที่ติดตั้งอยู่แล้ว:

```powershell
python re/scripts/inspect_c9_capture.py artifacts/captures/CAPTURE.pcapng --output artifacts/diagnostics/capture-metadata.json
```

เครื่องมือต้องเห็น SYN และ TCP data ต่อเนื่องครบ; ปฏิเสธ gaps/retransmissions
แทนการเดาการประกอบ stream และไม่ export payload/credentials

## เปิดรอบถัดไป

```powershell
& .\scripts\Start-C9Lab.ps1 -GameProfile fixture -OpenLauncher
& .\scripts\Start-C9Capture.ps1 -Seconds 600 -IncludeHandoff
```

Capture เฉพาะ loopback 127.0.0.1 ports 14000 และ 14001 สูงสุด 600 วินาที / 512 packets / 1 MiB
เริ่มหลังการทดสอบอัตโนมัติเสร็จ เพื่อแยก traffic ของเกมออกจาก simulator
ยังไม่มี character service, world simulation หรือการเข้าเล่นจริง
