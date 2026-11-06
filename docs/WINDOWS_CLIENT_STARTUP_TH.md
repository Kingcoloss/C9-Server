# Client startup บน Windows — 3 พฤศจิกายน 2026

อัปเดตต่อจากรายงานนี้: เกมแลกเปลี่ยน encrypted login/list กับ fixture และแสดง
หน้า C9 Local / Good, refresh ซ้ำ และ TCP handoff ไป endpoint ในเครื่องได้แล้ว
ผลล่าสุดและขอบเขตที่ยังไม่ยืนยันอยู่ใน [WINDOWS_HANDSHAKE_TH.md](WINDOWS_HANDSHAKE_TH.md)

หลังเครื่องเริ่มใหม่ งาน source, SDK และ Client config ยังอยู่ครบ
เปิด update HTTP ที่ `127.0.0.1:18080` และ passive TCP probe ที่
`127.0.0.1:14000` กลับด้วย `scripts/Start-C9Lab.ps1`
launcher ดาวน์โหลดเวอร์ชัน 1017 แล้วแสดง `Patch success complete.`

## แก้จุดค้างของ wrapper

Client เดิมค้างที่กล่องข้อความประชาสัมพันธ์ภาษาจีน `提示` ก่อนเปิดหน้าต่างเกม
เก็บ stack ด้วย DbgEng ของ Windows โดย attach แบบ noninvasive/no-suspend
แล้วอ่าน WOW64 context/EBP chain เพิ่ม เพราะ stack x86 จาก `.effmach x86`
ของ engine บน ARM64 มีคำเตือนเรื่อง unwind และไม่แสดง Client caller ที่ถูกต้อง

EBP chain จาก main thread แสดง USER32 และ UIAutomation รออยู่ใต้
`PhysXLoader+0x26b3`, `+0x2b29`, `+0x11e85d`, `+0x11e7e4`
ตรงกับ static `FUN_10002520` ที่เรียก `MessageBoxW` ระหว่าง DLL initialization
สาเหตุมีลักษณะสอดคล้องกับ loader-lock/UI initialization deadlock;
ไม่ได้ใช้ wait-chain cycle เป็นหลักฐานว่าเป็น deadlock ที่ยืนยันครบทุก lock
[Microsoft อธิบายข้อจำกัดของ DllMain/User32](https://learn.microsoft.com/en-us/windows/win32/dlls/dynamic-link-library-best-practices)

แก้ใน `PhysXLoader.dll` เพียงสองไบต์:

| รายการ | ค่า |
|---|---|
| Original SHA-256 | `6f491b6073a6fd759f3eb5b83e010fa66c5e0e9075585d263bd63d1f622cec3f` |
| Patch SHA-256 | `58631b7fd96f28fecfbbdd2e71f578f8a44cfc5031c943a4c1b846f3fd7ebef3` |
| Preferred VA / file offset | `1000269f` / `1a9f` |
| เดิม → ใหม่ | `6a30` (`push 30h`) → `eb12` (`jmp 100026b3`) |
| ขอบเขต | ข้าม arguments และ promotional MessageBoxW call ที่ `100026ad` |
| ต้นฉบับสำรอง | `artifacts/client-backups/startup-notice/PhysXLoader.dll.original` |

branch ข้ามทั้งการ push arguments และ call จึงไม่เปลี่ยนสมดุล stack
และไม่ทับ HIGHLOW relocation ของ IAT operand ที่ `100026af`
การทดลองแรกที่แทน call หกไบต์ทับ relocation จึงถูกยกเลิกและแทนด้วย branch นี้
ไม่เก็บ write time เดิมเมื่อ patch เพื่อให้ translated-image cache ตรวจการเปลี่ยนไฟล์ได้

สคริปต์ตรวจ full-file hash, ตรวจ bytes รอบจุดแก้, เก็บ backup นอก Client folder
และเปลี่ยนไฟล์แบบ atomic ทดสอบคืน original hash แล้ว apply กลับสำเร็จ
ไม่ได้แก้ `C9.exe`, `xp.dll`, codec หรือการเรียกอื่นของ wrapper

```powershell
# ตรวจสถานะอย่างเดียว
python re/scripts/patch_startup_notice.py --client-dir 'F:\52C9GL'

# ปิด Client ก่อน apply/restore
python re/scripts/patch_startup_notice.py --client-dir 'F:\52C9GL' --apply
python re/scripts/patch_startup_notice.py --client-dir 'F:\52C9GL' --restore
```

## version.ini ต้องมี game endpoint ด้วย

หลังผ่าน wrapper เกมแสดง `Invalid version data detected`
พบ error literal ที่ preferred VA `010be854`, reference `009b6915`
และ strings `[SERVER]`, `PORT1` ใกล้กัน
กู้ไฟล์ `52C9GL/version.ini` ขนาด 5,788 ไบต์จาก RAR เดิมมานอก Client folder
ได้ SHA-256 `f2ac644689d523e96148de0f0c23bc366beafa85aa18eef134367d9826b3f141`
ตรงกับ frozen manifest จาก Mac

ไฟล์เดิมมี `OwnVersion=17`, `[SERVER] COUNT=1`, `IP1` และ `PORT1`
จึงเพิ่มส่วนเหล่านี้ในบริการ manifest โดย IP เป็น loopback และ port มาจาก
`--login-port` (default 14000) ให้ตรงกับ `-GamePort` ของ Start-C9Lab
ไม่คัดลอก endpoint ภายนอกหรือส่วน firewall จากไฟล์เดิมมาใช้

```ini
[SERVER]
COUNT=1
IP1=127.0.0.1
PORT1=14000
```

Start script ตรวจ IP/port ใน HTTP response ก่อนแก้ localinfo.dat
ข้อทดสอบ HTTP ใช้ advertised port 16123 แยกจาก default เพื่อยืนยันว่าค่าที่ส่งมา
ถูกนำไปสร้าง manifest จริง Full .NET suite ผ่าน 36/36, 2,836 assertions

## ผล runtime ที่สังเกตแล้ว

- launcher → PLAY → ผ่าน startup notice และ version-data checks
- แสดง `C9 Loading` แล้วเปิดหน้าต่างเกม `第九大陆`
- render ภาพและวิดีโอเปิดเกม `BLACK ROCK PIECES` ได้
- หลังวิดีโอ แสดงหน้า `Log In` พร้อม Username/Password และปุ่ม OK/Quit
- รอผู้ใช้กดล็อกอินด้วย test/test เพื่อสังเกต protocol จริง; ยังไม่กรอกหรือส่งบัญชีจาก automation
- ผู้ใช้ลอง test/test แล้ว: passive probe รับ TCP connection ได้ แต่ไม่ได้รับ frame
  ภายใน deadline 10 วินาที จึงปิดด้วย TimeoutException
  ยืนยัน endpoint ใน version.ini ว่าถูกใช้จริง แต่ยังไม่ยืนยัน codec จากรอบนี้
- `C9.exe` hash ยังเป็น `b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25`
- baseline 38 ไฟล์ตรง 35; เปลี่ยนโดยตั้งใจเฉพาะ PhysXLoader.dll, localinfo.dat, version.ini

การเปิด Client นี้ใช้ DLL wrapper ที่แก้ข้อความเริ่มต้น จึงต้องระบุ patch นี้เมื่อ
อ้างผล runtime ต่อไป ยังไม่ยืนยัน authentication, server list หรือการเข้าโลกเกม
passive probe ไม่มี credential/payload logging

เครื่องมือวิเคราะห์อยู่ใน `re/scripts/inspect_client_stacks.py` และ
`inspect_client_x86.py`; รายละเอียด stack/raw diagnostics เก็บที่
`artifacts/diagnostics/` ซึ่งไม่เข้า Git

## ทดสอบ handshake และจับข้อมูลด้วย Wireshark

Client handler `00a621f0` ตรวจ message type 4 และอ่าน mode byte ที่ offset 13;
mode 0 เลือก branch สร้างคำขอ login ตาม state ของ Client
เมื่อ passive host และ Client รอกันอยู่ จึงเปลี่ยนไปใช้ `lab-fixture.json`
ที่มี greeting นี้และคำตอบ login/server-list จำลองแบบ explicit
การเปลี่ยน profile ยังไม่ใช่หลักฐานว่า authentication semantics ถูกต้อง

```powershell
& .\scripts\Stop-C9Lab.ps1
& .\scripts\Start-C9Lab.ps1 -GameProfile fixture
& .\scripts\Start-C9Capture.ps1 -Seconds 180
```

Start-C9Lab ตรวจ profile ของ process เดิมก่อน reuse และปฏิเสธ profile ที่ไม่ตรง
แทนการ reuse passive host โดยเงียบ ๆ ทดสอบ reuse และ rejection แล้ว

เครื่องนี้มี Wireshark/dumpcap และ Npcap loopback ใช้งานได้
capture script ใช้เฉพาะ `\Device\NPF_Loopback` และ filter
`tcp port 14000 and host 127.0.0.1` ไม่เปิด NIC Ethernet หรือ capture traffic อื่น
หยุดเองเมื่อถึง 180 วินาที, 512 packets หรือ 1 MiB แล้วแต่ว่าอะไรถึงก่อน
ไฟล์อยู่ใน `artifacts/captures/`, process/file ล่าสุดอยู่ใน `artifacts/capture.local.json`
รอผู้ใช้กด test/test อีกรอบเพื่อเก็บ first handshake; ยังไม่กรอก auth UI จาก automation
