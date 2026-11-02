# C9 Phase 5 — Port server เป็น .NET 10 และเตรียมย้ายไป UTM

วันที่ 2 พฤศจิกายน 2026 (Asia/Bangkok)

## ผลที่ได้

Port runtime ของ server เดิมเป็น C#/.NET 10 แล้ว แยก `C9.Protocol` สำหรับ binary codec และ `C9.Server` สำหรับ Generic Host, TCP/Pipelines, configuration และ session processing เก็บ Python เป็น RE/reference implementation ไม่ต้องใช้ Python หรือ Client files ตอนรัน .NET server

Build Release ผ่านโดยไม่มี warning/error และ publish ไป `artifacts/server` แล้ว การตรวจ runtime แบบไม่เปิด socket ผ่าน ชุดทดสอบ .NET แบบ offline ผ่าน 24 กรณี รวม 2,739 assertions; Python protocol/offline/path tests อีก 24 กรณีผ่าน และ Python สื่อสารกับ .NET session processor ผ่าน stdio fixture flow ได้

ยังไม่ได้ยืนยัน TCP integration ของ .NET หรือการล็อกอินด้วยเกมจริง การลอง TCP loopback ใน environment รอบนี้ถูกปฏิเสธด้วย `SocketException (13): Permission denied` จึงแยก 10 socket tests เป็น skipped และใช้ in-memory/stdio validation ที่ไม่สร้าง connection แทน ไม่ถือว่าผลนี้แทน TCP หรือ original-Client acceptance

ผู้ใช้ตัดสินใจย้ายไปพัฒนาบน UTM ระยะยาวแล้ว และแจ้งว่า CrossOver Free Trial รันเกมไม่ได้ บันทึกเป็นผลทดลองจากผู้ใช้ โดยยังไม่สรุปสาเหตุของ CrossOver การเปิดเกมเพื่อ RE/debug ยังคงรอผู้ใช้อนุมัติ รอบนี้ไม่ได้เปิดเกม, Launcher, Patcher, UTM หรือ CrossOver

## สิ่งที่ port

| ส่วน | Implementation |
|---|---|
| Packet/header | field widths/endianness ชัดเจน, decoded header 13 ไบต์ และ bounded length |
| Checksum | signed-byte expansion และ unchecked uint32 wrap ตาม static evidence |
| Payload codec | PacketTwister ครบ 80 selector โดยใช้ extracted tables ที่ฝังใน assembly |
| Initial cipher | managed RIPEMD160 และ RC6, independent primitive vectors, initial key derivation |
| Sequence/state | แยกต่อทิศทางและต่อ connection, ไม่ commit receive state เมื่อ decode ไม่ผ่าน |
| Transport | TcpListener/Sockets และ System.IO.Pipelines สำหรับ fragmented/coalesced frames |
| Session | processor เดียวใช้ได้กับ TCP และ memory/stdio adapters, state/type/length rules |
| Lifecycle | Generic Host, cancellation, timeouts, connection/packet limits และ cleanup |
| Diagnostics | structured events ที่ไม่มี payload/credentials และ packet counter |
| Configuration | อ่าน fixture config เดิมได้; default probe และ loopback restriction คงไว้ |

Auth responses ใน fixture profile ยังเป็นข้อมูลจำลอง ค่า response และ server entry ยังไม่ใช่ semantics ที่พิสูจน์กับ Client แล้ว Subsequent key rotation ยังไม่ครบ จึงรองรับ 256 encrypted frames ต่อทิศทางใน initial epoch แล้วปฏิเสธการใช้ codec ต่อ การ port ไม่ได้แก้ช่องว่าง protocol เหล่านี้โดยอัตโนมัติ

## ผลตรวจสอบ

| รายการ | ผลและขอบเขต |
|---|---|
| .NET Release build | ผ่าน, 0 warnings / 0 errors, SDK 10.0.302 บน Mac ARM64 |
| .NET offline validation | 24/24 ผ่าน, 2,739 assertions |
| .NET socket validation | 10 กรณียังไม่ยืนยัน; loopback connect ถูก environment ปฏิเสธ |
| Python protocol | 10/10 ผ่าน |
| Python offline reader | 8/8 ผ่าน |
| Client path resolver | 6/6 ผ่าน รวม path ที่มีช่องว่างและ canonical path บน Mac |
| PacketTwister parity | 960 vectors ครอบคลุม 80 selector และ length boundaries |
| Wire/state parity | 64 sequential wire vectors และ direction state tables ตรงกับ Python |
| Independent primitives | RC6 6 vectors และ RIPEMD160 9 vectors; เพิ่ม OpenSSL padding-boundary comparisons |
| Cross-language stdio | Python request → .NET session → greeting/login/server-list replies ผ่าน |
| Published runtime check | ผ่าน โดยไม่ใช้ Python, Client directory หรือ socket |

ชุด offline ครอบคลุม framing, validation errors, sequence, epoch boundary, opaque server list, session isolation, concurrent sessions, timeout, packet limits, cancellation, backpressure และ StreamPipe adapters หลักฐานอยู่ใน `re/evidence/static/phase5_dotnet_validation.json`

Reference vectors ของ Python ช่วยตรวจความตรงกันข้ามภาษา แต่ทั้งสอง implementation อาจอิง specification ที่ผิดเหมือนกันได้ จึงยังต้องใช้ independent vectors และข้อมูลจริงจาก Client ก่อนรับรอง compatibility ผล Python TCP ของ Phase 4 เป็นผลของ implementation/สภาพแวดล้อมรอบก่อน ไม่ใช้แทนผล TCP ของ .NET รอบนี้

RIPEMD160 อ้างอิง [specification และ vectors ของผู้พัฒนา algorithm](https://homes.esat.kuleuven.be/~bosselae/ripemd160.html); RC6 vectors จาก [Crypto++](https://github.com/weidai11/cryptopp/blob/master/TestData/rc6val.dat)

## คำสั่งหลักสำหรับ UTM/Windows

ติดตั้ง SDK ตาม `global.json` แล้วใช้คำสั่งจาก project root:

```powershell
dotnet build C9.slnx -c Release --disable-build-servers -m:1 -p:UseSharedCompilation=false
dotnet src/C9.Server/bin/Release/net10.0/C9.Server.dll --check-runtime
dotnet tests/C9.Validation/bin/Release/net10.0/C9.Validation.dll --offline
```

เมื่อ loopback sockets ใช้งานได้ ให้ทดสอบ TCP เพิ่มด้วยคำสั่ง validation ที่ไม่มี `--offline` และรัน server ของเราได้โดยไม่เปิดเกม:

```powershell
dotnet tests/C9.Validation/bin/Release/net10.0/C9.Validation.dll
dotnet src/C9.Server/bin/Release/net10.0/C9.Server.dll --config config/lab-fixture.json
```

การทดสอบ Windows/UTM และ Linux deployment ยังต้องทำใน environment เป้าหมาย โค้ดที่ publish เป็น framework-dependent output ต้องมี .NET/ASP.NET Core 10 shared frameworks ที่เครื่องปลายทาง

## Client folder ที่ย้ายแล้ว

ยืนยันได้ว่า `52C9GL/` ตำแหน่งเดิมไม่มีแล้ว แต่ยังไม่พบค่า absolute path ใหม่ใน project metadata ที่เครื่องมือเข้าถึงได้ จึงยังไม่อ้างว่าเทียบ hash ที่ตำแหน่งใหม่ผ่าน

เพิ่ม resolver กลางสำหรับ RE scripts โดยเลือก `--client-dir` ก่อน `C9_CLIENT_DIR` และ `config/research.local.json` มี `config/research.example.json` เป็นตัวอย่าง ส่วน `.NET server` ใช้ข้อมูล codec ที่ฝังไว้และไม่อ่าน Client folder การ import/เทียบ hash ครั้งถัดไปใช้ `check_client_integrity.py` หลังตั้งตำแหน่งจริงใน VM

## .gitignore และขอบเขตการย้าย

ปรับ `.gitignore` ให้แยก Client/archives, downloaded Ghidra/tools, working databases, .NET/Visual Studio/Python outputs, local config, logs/captures/dumps และ VM images ออกจาก source ตรวจด้วย Git engine ผ่าน 32 กรณีที่ต้อง ignore และ 27 กรณีที่ต้องเก็บไว้ ไม่มี tracked file ที่ต้องถอดจาก index ในขณะตรวจ และไม่มีการลบไฟล์จริง

เก็บ source .NET/Python, shared config, .NET solution, fixtures, exported `.c`/`.asm.txt`/`.json` และ reports ไว้ให้เข้า Git ได้ ไม่ ignore ทั้ง `re/evidence/`, `.vscode/` หรือไฟล์ JSON ทั้งหมด ชุด Ghidra ที่ติดตั้งมีขนาดประมาณ 891 MiB และฐาน Ghidra รวมประมาณ 594 MiB ซึ่งต้องเก็บ/ย้ายแยกหากต้องใช้ database เดิมต่อ

ประวัติ Git ยังมี Client archives ที่ reachable: `52C9GL.part1.rar` 5,368,709,120 bytes และ `part2.rar` 3,826,849,350 bytes; local packfiles ประมาณ 8.70 GiB การเพิ่ม ignore ไม่ลด history นี้ จึงต้องแยกวางแผนวิธีย้าย source/history ก่อนส่ง repository เต็มชุด ไม่มีการ rewrite history, stage, commit หรือ push ในงานรอบนี้

ขณะตรวจ Git tracked อยู่เพียง README กับ .gitignore; source ใหม่ยังต้องเข้า commit ก่อนใช้ clone เป็นวิธีย้ายทั้งหมด หลักฐานการตรวจอยู่ใน `re/evidence/static/gitignore_utm_validation.json` และขั้นตอนย้ายงานอยู่ใน `docs/UTM_HANDOFF_TH.md`

## งานต่อไป

1. เลือกวิธีย้าย source/history ไป UTM โดยแยก Client, VM และ Ghidra databases ออกจาก repository
2. ตั้ง Client path จริงใน VM และเทียบกับ manifest เดิมก่อนใช้การ import/trace ชุดใหม่
3. รัน .NET TCP integration suite ใน VM และแก้ปัญหาที่พบ; ยังไม่ต้องเปิดเกมเพื่อทดสอบ server กับ client จำลอง
4. ทำ static codec rotation และ schema ต่อเท่าที่มีหลักฐาน เมื่อจำเป็นต้องเปิดเกมเพื่อ RE ให้รอผู้ใช้อนุมัติ
5. เพิ่ม auth/persistence/character/world ตาม protocol ที่ยืนยันแล้ว และตรวจ Client/load/recovery ก่อนนำไปใช้งานจริง

Phase 5 ในรายงานนี้หมายถึงรอบ port และ handoff ไม่ใช่การผ่าน milestone M5 ด้าน world/gameplay ในแผน
