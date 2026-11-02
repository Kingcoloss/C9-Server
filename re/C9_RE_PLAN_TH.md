# แผน RE — ปรับถึง .NET Port Phase 5 และย้ายไป UTM

วันที่ 2 พฤศจิกายน 2026 · อ้างอิงรายงาน Phase 1–5 · C#/.NET 10 เป็น server runtime หลัก · เตรียม UTM สำหรับการพัฒนาระยะยาว

## ขอบเขตที่อนุญาต — RE และ server implementation โดยยังไม่เปิดเกม

ผู้ใช้แจ้งให้ทำงานต่อโดยไม่เปิดตัวเกม จนกว่าผู้ใช้จะแจ้งให้เปิดเพื่อ debug ได้ จึงพักการเริ่ม Client, Launcher, Patcher และการรันเกมเบื้องหลังไว้ การวิเคราะห์ PE ด้วย Ghidra/DiE และเครื่องมือ offline บน Mac ทำต่อได้ เพราะเป็นการอ่านไฟล์ ไม่ใช่การเริ่ม process ของเกม

ผู้ใช้อนุญาตให้ทำต่อได้จนถึงการ implement server แล้ว จึงทำ M0 baseline บน Mac, M2 codec, M3 schema และ M4 implementation/ทดสอบด้วยโค้ดที่เขียนใหม่และ client จำลองได้ทันที ส่วน M0 ที่ต้องตรวจ process ใน VM, M1 runtime trace และการทดสอบ Client จริงใน M4–M5 ยังรอผู้ใช้อนุมัติเปิดเกม การเก็บ runtime trace ยังคงเป็นเกณฑ์ตรวจสอบภายหลัง ไม่ถือว่าผ่านด้วย static analysis หรือ client จำลองแทน

ผลรอบ Static Phase 2 บันทึกใน [C9_RE_Phase2_Static_TH.md](C9_RE_Phase2_Static_TH.md): เพิ่ม manifest 38 ไฟล์ ตาม RTTI ถึง C9NetworkLogin handler ได้ และบันทึก constructors 7 รูปแบบกับ type ที่ handler ตรวจรับ 11 ค่า พร้อมความแตกต่างของ codec/state ระหว่าง TCP กับ BaseCat งาน runtime ยังคงพักอยู่

ผลรอบ Static Phase 3 บันทึกใน [C9_RE_Phase3_Static_TH.md](C9_RE_Phase3_Static_TH.md): ผูก outer cipher factory กับ RTTI ได้ 16 คลาส, ทำตาราง PacketTwister 100 ตัว โดย wrapper ที่ตรวจพบใช้ modulo 80, ยืนยันการคัดลอก server entry ขนาด 52 ไบต์ และเพิ่มเครื่องมือตรวจ wire framing/decoded fields/checksum แบบ offline พร้อม synthetic tests 8 กรณีที่ผ่านแล้ว ไฟล์ baseline 38 ไฟล์ยังมี hash ตรงเดิม ยังไม่ใช่หลักฐานความเข้ากันได้กับ Client จริง

ผลรอบ Server Phase 4 บันทึกใน [C9_RE_Phase4_Server_TH.md](C9_RE_Phase4_Server_TH.md): implement TCP server บน Mac, PacketTwister encode/decode 80 selector, initial RIPEMD160/RC6 epoch และ fixture response rules แล้ว ผ่าน unittest ใหม่ 17 กรณี รวมกับเดิม 8 กรณี และทดสอบ CLI process แยกผ่าน TCP loopback สำเร็จ ยังไม่พิสูจน์ authentication/handshake กับเกม และรองรับ initial epoch 256 encrypted frames ต่อทิศทางก่อนปฏิเสธการใช้ codec ต่อ เพราะยังทำ rotation ไม่ครบ

ผู้ใช้สามารถใช้คอมพิวเตอร์พร้อมงาน headless ได้ เพราะไม่ใช้เมาส์/คีย์บอร์ดหรือเปลี่ยนหน้าต่าง ตั้ง Ghidra ไว้สูงสุด 2 CPU และ Java heap 2 GiB หากต้องใช้ Computer Use กับหน้าต่าง ให้แจ้งผู้ใช้ก่อน ข้อจำกัดไม่เปิดเกมยังมีผลตลอดจนกว่าผู้ใช้จะอนุญาต debug

เป้าหมายระยะถัดไปคือพิสูจน์โปรโตคอลช่วงเริ่มเชื่อมต่อและล็อกอินของ Client ชุดที่มีอยู่ แล้วสร้างเซิร์ฟเวอร์ขั้นต่ำที่ Client สื่อสารด้วยได้ ผลลัพธ์ระยะแรกต้องวัดจากพฤติกรรม Client และข้อมูลที่ตรวจสอบย้อนกลับได้ ส่วนระบบต่อสู้ เนื้อหาเก้าทวีป ฐานข้อมูลเต็มรูปแบบ และ admin panel อยู่หลังการยืนยันโปรโตคอลพื้นฐาน

## Stack สำหรับ server ที่นำไปรันจริง — เลือกและ port แล้ว

ผู้ใช้อนุญาตให้ port ส่วน server จริงแล้ว จึงใช้ C#/.NET 10 เป็น runtime หลักและเก็บ Python เป็น protocol reference/เครื่องมือ RE การ port ส่วนที่มีหลักฐานและ implementation อยู่แล้วเสร็จใน [C9_RE_Phase5_DotNet_Port_TH.md](C9_RE_Phase5_DotNet_Port_TH.md) ประกอบด้วย codec, managed crypto, transport, session processor และ host แต่ยังไม่ใช่การรับรอง authentication หรือการใช้งานกับ Client จริง

| ส่วน | Stack/สถานะ |
|---|---|
| Server หลัก | C# / .NET 10 LTS; เครื่อง Mac ปัจจุบันมี SDK 10.0.302 แล้ว |
| Network | System.Net.Sockets + System.IO.Pipelines สำหรับ TCP ที่กู้ได้ และแยก transport interface สำหรับเส้นทางอื่นที่หลักฐานยืนยัน |
| Codec | Binary format ตาม Client, field widths/endianness ชัดเจน, bounded buffers และ uint32 wrap ตาม algorithm |
| Cipher | ตรวจ managed implementation/backend ให้ครบตาม algorithm ที่จำเป็นบนแต่ละ OS ไม่สมมติว่า built-in crypto มีครบ |
| Database | PostgreSQL 18 เป็นตัวเลือกหลัก; ประเมิน Npgsql, transactions และ migrations เมื่อเริ่ม persistence |
| Architecture | เริ่ม host เดียวที่แยก modules protocol/session/auth/lobby/world/persistence; แยก process เมื่อ flow หรือผลวัดรองรับ |
| สภาพแวดล้อม | ผู้ใช้เลือกพัฒนาระยะยาวใน UTM/Windows; ผล build/offline ปัจจุบันมาจาก Mac ARM64 ส่วน Windows/UTM และ Linux deployment ยังต้องทดสอบจริง |
| เครื่องมือ RE | เก็บ Python/Ghidra และ fixtures ไว้สำหรับ extraction และตรวจเทียบกับ .NET |

เหตุผลที่เสนอ .NET คือทำ server runtime และระบบประกอบด้วย stack หลักเดียวได้ โดยยังรองรับงาน binary streaming ผ่าน Pipelines เป็นข้อเสนอทางวิศวกรรม ยังไม่ใช่ผล benchmark ว่ารองรับผู้เล่นได้จำนวนใด .NET 10 เป็น LTS ที่มี support ถึงเดือนพฤศจิกายน 2028 ตาม [นโยบาย Microsoft](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core) ส่วน PostgreSQL 18 อยู่ในรายการ supported ตาม [นโยบาย PostgreSQL](https://www.postgresql.org/support/versioning/)

รายละเอียดการตัดสินใจเก็บที่ `docs/adr/0001-server-stack.md` มี .NET solution และ codec พร้อม differential tests แล้ว ผล Release build/publish ผ่านและ runtime check ไม่ใช้ Python หรือ Client files; .NET offline 24/24 กรณี (2,739 assertions) และ Python 24/24 กรณีผ่าน รวมถึง Python → .NET session ผ่าน stdio มี RC6/RIPEMD160 independent vectors เสริม การตรวจ .NET TCP 10 กรณียังไม่ยืนยัน เพราะ environment ปัจจุบันปฏิเสธ loopback connect ด้วย SocketException (13) ต้องรันใน VM ก่อนสรุปผล TCP การตรงกับ Python หรือผล memory/stdio ไม่ใช้แทน original-client fixtures

เกณฑ์ server ที่พร้อมนำไปรันจริงต้องรวม Client interoperability, persistence/transactions ที่ถูกต้อง, long-session/reconnect/recovery, load test และการวัด latency/tick/CPU/GC/memory ตามเป้าหมายจำนวนผู้เล่นที่กำหนดร่วมกัน ยังไม่กำหนดตัวเลข CCU จากการคาดเดา และยังไม่มีการติดตั้ง DB หรือ deploy ในรอบ port นี้

การ RE Client ช่วยกู้ protocol, data และบางส่วนของ logic ได้ แต่ไม่ได้ให้ server-authoritative rules ครบทั้งหมด จึงต้องออกแบบและตรวจระบบ combat/AI/economy/world/persistence ที่ไม่มีหลักฐานฝั่ง Client เพิ่มด้วย ข้อจำกัดไม่เปิดเกมยังคงเดิม

## ย้ายไป UTM และเตรียม repository

ผู้ใช้เลือกพัฒนาใน UTM ระยะยาว และแจ้งว่า CrossOver Free Trial รันเกมไม่ได้ ยังไม่มี error log ที่ยืนยันสาเหตุ จึงไม่สรุปว่าเป็นข้อจำกัดของ Trial และไม่ให้ CrossOver เป็นเงื่อนไขของงานถัดไป การเปิด Client/Launcher/Patcher เพื่อ RE/debug ยังต้องรอผู้ใช้อนุมัติ การ build และทดสอบ server ด้วย client จำลองทำได้ก่อน

ปรับ `.gitignore` ให้แยก Client/archives, downloaded tools, Ghidra databases, .NET/Visual Studio/Python outputs, local paths/secrets, logs/captures/dumps และ VM images แล้ว ตรวจด้วย Git engine ผ่าน 32 กรณีที่ต้อง ignore และ 27 กรณีที่ต้องเก็บ ไม่มี tracked file ที่ถูก ignore ในขณะตรวจ เก็บ shared config, fixtures, exported C/assembly/JSON และรายงานไว้ให้เข้า Git ได้ ไม่ ignore ทั้ง `re/evidence/` หรือ `.vscode/`

ต้องแยกเก็บ/ย้าย Client และ Ghidra project `.gpr` พร้อม `.rep/` หากจะใช้ฐานวิเคราะห์เดิมต่อ ชุด Ghidra ติดตั้งประมาณ 891 MiB และฐานวิเคราะห์รวมประมาณ 594 MiB ไม่รวมอยู่ในการส่ง source ผ่าน Git

ข้อค้นพบก่อนย้าย: Git packfiles ประมาณ 8.70 GiB และ history ยังอ้างถึง `52C9GL.part1.rar` 5,368,709,120 bytes กับ `part2.rar` 3,826,849,350 bytes การ revert/ignore ไม่เอา archives ออกจาก history ต้องเลือกวิธีส่ง source หรือจัดการ history แยกก่อน clone เต็มชุด รอบนี้ไม่ rewrite history, stage, commit หรือ push ขณะตรวจ tracked อยู่เพียง README และ .gitignore; source ใหม่ยัง untracked จึงยังย้ายครบด้วย clone อย่างเดียวไม่ได้

ขั้นตอนและคำสั่ง VM อยู่ที่ `docs/UTM_HANDOFF_TH.md` ใช้ .NET SDK ตาม `global.json` และตั้ง `C9_CLIENT_DIR` หรือ `config/research.local.json` ใน VM โดยไม่ commit absolute path ของเครื่อง เมื่อมี path จริงให้ใช้ `re/scripts/check_client_integrity.py` เทียบ manifest ก่อน import/trace เพิ่ม การย้ายครั้งนี้ยังไม่ผ่าน M1/M4 original-Client acceptance หรือ M5 world/gameplay

## เหตุผลที่ปรับลำดับงาน

| หลักฐานใน Phase 1 | ผลต่อแผน |
|---|---|
| พบทั้ง Winsock และ BaseCat ในโค้ด | ต้องตรวจว่าเส้นทางใดถูกใช้จริง ในสถานะใด และมีการใช้ทั้งสองเส้นทางหรือไม่ ก่อนแบ่งบริการเป็น auth/lobby/game |
| พบโครงสร้าง frame 13 ไบต์และสูตร checksum จาก assembly | เริ่มเขียนข้อกำหนด codec แบบมีหลักฐานกำกับได้ แต่ยังต้องพิสูจน์ transform, state และข้อมูลบน wire |
| สำหรับ frame ที่เข้า transform ส่วนตั้งแต่ offset 3 ถูกแปลง | ตัวอ่านแพ็กเก็ตต้องแยก wire frame ออกจาก decoded frame; ห้ามอ่าน opcode ตรงจาก wire ทุกกรณี |
| พบชื่อ PS_* ไม่ซ้ำ 459 ชื่อใน strings | ใช้เป็นรายการค้นหา ยังไม่ใช้แทนตาราง numeric opcode หรือจำนวนข้อความที่ implement ได้ |
| พบ PhysXLoader.dll ที่โหลด xp.dll และมี initialization เพิ่มเติม | ต้องรวม DLL chain ใน baseline และ runtime observation เพื่อไม่ตีความพฤติกรรมของ wrapper เป็นพฤติกรรม EXE เดิม |
| version.dat, VerCli.C9, Version.cfs และ build string ให้ตัวระบุคนละค่า | กำหนดเป้าหมายด้วยชุดไฟล์และ SHA-256 พร้อมแยกชนิดของตัวระบุเวอร์ชัน |
| Auto-analysis รอบแรก timeout และยังไม่มี runtime trace | แยกงานที่วิเคราะห์แล้วออกจากงานที่ยังขาด และเก็บ runtime หลักฐานตั้งแต่ต้นแทนการรอให้ static analysis ทั้งโปรแกรมเสร็จ |

## ลำดับงานและเกณฑ์ผ่าน

### M0 — ยืนยันชุด Client และสภาพแวดล้อมทดสอบ

สถานะ: มี baseline 38 ไฟล์บน Mac แล้ว; ผู้ใช้ย้าย Client ออกจากตำแหน่งเดิมและยังไม่พบ absolute path ใหม่ในข้อมูลที่อ่านได้ ต้องตั้ง path และเทียบ hash ของชุดที่ย้าย/ใช้ใน Windows VM

งาน:

- เก็บรายการและ hash ของ EXE, DLL ที่เกี่ยวข้อง และไฟล์ตั้งค่า/เวอร์ชันของชุดทดสอบ
- เปรียบเทียบ hash กับ Client ที่ติดตั้งใน drive ภายใน VM ก่อนใช้ trace ของ VM เป็นหลักฐานของชุดบน Mac
- บันทึกคำสั่งเริ่มเกม, arguments, working directory, Windows architecture, modules ที่โหลดจริง และขั้นตอนที่ Client ไปถึงได้
- ตรวจว่าเครื่องมือ debug/instrumentation ใน VM เข้าถึง process x86 ที่ต้องการได้จริงก่อนวางงานที่ต้องพึ่ง breakpoint
- เก็บต้นฉบับและใช้ working copy สำหรับการทดลองที่เปลี่ยนไฟล์

ผลส่งมอบ: Client manifest, ขั้นตอนเริ่มซ้ำได้ และตาราง module ที่โหลดจริง

เกณฑ์ผ่าน: ระบุได้ว่า trace และ static addresses อ้างถึงไบนารีชุดใด; หากไฟล์ต่างกัน ต้องบันทึกเป็นคนละชุดทดสอบ การพบ runtime base ต่างจาก preferred image base ต้องแปลง VA/RVA ให้ตรงก่อนเทียบตำแหน่ง

### M1 — ตรวจ runtime ช่วงเริ่มเกมและการเชื่อมต่อ

สถานะ: ยังไม่ได้เก็บ runtime trace ใน Phase 1

งาน:

- บันทึกเหตุการณ์เริ่ม process, โหลด DLL, resolve host และสร้าง connection พร้อมเวลาที่สัมพันธ์กัน
- เชื่อมข้อมูล socket กับ PID/module/call site จากการสังเกต runtime แล้วใช้ connection tuple ที่ยืนยันแล้วจำกัด packet capture
- เริ่มติดตาม caller ของ `0x00563820` และ `0x00665650` เพื่อหาที่มาของ host/port และเงื่อนไขเลือก transport
- รวมผลของ `PhysXLoader.dll` และ `xp.dll` ในการอธิบาย startup
- ตรวจที่มาของ endpoint แต่ละรายการ; ค่า IP ใน strings หรือ URL update/news ยังไม่ใช่หลักฐานของ login endpoint

ผลส่งมอบ: startup timeline, connection map และ trace ของการเชื่อมต่อที่สังเกตได้ โดยระบุ source hash และสถานะ Client

เกณฑ์ผ่าน: ระบุเส้นทางที่ Client เรียกจริงสำหรับเหตุการณ์ที่สังเกต พร้อมแหล่งกำเนิด host/port หรือระบุจุดที่ยังไม่ทราบได้ชัดเจน การเชื่อมต่อล้มเหลวไม่ทำให้ขั้นนี้ไร้ค่า แต่ไม่ถือเป็นหลักฐานของ server reply หรือ login handshake ที่สำเร็จ

M1 และงาน static ใน M2 ทำสลับกันได้ ข้อมูล runtime แต่ละรอบควรช่วยจำกัดขอบเขต static analysis รอบถัดไป

### M2 — ยืนยัน codec และ state ของทั้งรับและส่ง

สถานะ: implement และ port เป็น .NET แล้วทั้ง payload transforms ครบ 80 selector และ initial outer cipher RIPEMD160/RC6 พร้อมตรวจ primitive ด้วย independent vectors; subsequent rotation และ codec compatibility กับเกมยังไม่ผ่าน

งาน:

- เจาะ `0x00563E40`, `0x00666260`, `0x00563D00`, `0x00666890` และ helper ที่ export แล้ว
- ไล่ transform จาก `0x00DCEF50` และ payload processor จาก `0x00412230` / `0x00563600` รวมเส้นทางส่งที่คู่กัน
- ใช้ `phase3_cipher_catalog.json` ตรวจ key derivation และคู่ encode/decode จาก assembly; ยังไม่กำหนด mode/padding จากชื่อ algorithm เพียงอย่างเดียว
- ใช้ `phase3_payload_registry.json` ไล่ operation ของ 80 selector ที่ wrapper ปัจจุบันเข้าถึง แยก length branches และตรวจ inverse; อีก 20 registered entries ยังไม่สรุปว่าไม่มีผู้เรียก
- Phase 4 ทำตาราง portable encode/decode สำหรับ 80 selector แล้ว มี 67,520 differential/inverse basis checks จาก recovered C; ตรวจ original-client wire fixtures เพิ่มเมื่อได้รับอนุมัติเปิดเกม
- ทำ key rotation ต่อจาก initial epoch 256 frames และขยาย cipher paths ที่จำเป็น โดยคงการปฏิเสธกรณีที่ยังไม่รองรับ
- ทำตาราง field offsets แยก wire/decoded พร้อมระบุความหมายที่ยืนยันแล้วและชื่อที่ยังเป็นข้อสันนิษฐาน
- ตรวจ checksum ให้ตรง signed-byte expansion, 32-bit wrap, ช่วงข้อมูลที่นำมาคำนวณ และการตั้งฟิลด์ offset 5 เป็น 100 ใน validation path ที่พบ
- ยืนยันการตั้งค่าเริ่มต้น การเพิ่ม และการ reset state/sequence เมื่อเชื่อมต่อใหม่; ไม่อนุมานว่าทั้งสองทิศใช้ state เดียวกัน
- ตรวจขนาด frame แยกตาม transport; ค่าจำกัด 4,096 ไบต์ที่พบใน BaseCat ไม่ใช้แทนข้อจำกัดทุกเส้นทาง

ผลส่งมอบ: codec specification ที่ผูกกับที่อยู่โค้ด, reference fixtures จากข้อมูลจริงหรือจาก routine ต้นฉบับที่รันในสภาพแวดล้อมทดสอบ และตัวอ่าน frame สำหรับตรวจ trace แบบ offline

เกณฑ์ผ่าน:

- อธิบาย byte ที่ใช้ใน trace/fixture ได้ทั้งก่อนและหลัง transform ในขอบเขตข้อความที่นำมาทดสอบ
- ตรวจเทียบ checksum/transform กับหลักฐานอิสระจาก implementation ที่เขียนใหม่
- ตัวอ่าน TCP รองรับ frame ที่แบ่งหลายครั้งและหลาย frame ที่มารวมกันในการอ่านครั้งเดียว
- มีกรณีตรวจ length ผิด, ข้อมูลไม่ครบ, checksum ผิด, byte ที่มี high bit และ sequence ตามกติกาที่พิสูจน์ได้

การ encode แล้ว decode ด้วยโค้ดใหม่ชุดเดียวกันผ่านเพียงอย่างเดียว ยังไม่พิสูจน์ความเข้ากันได้กับ Client และ frame ที่ flag เป็นศูนย์เพียงกรณีเดียวไม่ครอบคลุมเส้นทาง transform

### M3 — จับคู่ opcode กับ schema และสถานะล็อกอิน

สถานะ: ยืนยัน static numeric types บางส่วนแล้ว (constructors 7 รูปแบบ, handler ตรวจรับ 11 ค่า); ชื่อ PS_* และ handshake จริงยังไม่ครบ

งาน:

- ไล่จาก receive queue `0x00561630` ไปถึง dispatch และ handler
- หา serializer/constructor ฝั่งส่ง แล้วจับคู่กับ numeric type หลัง decode
- ใช้ `PS_Certify_*`, `PS_LoginData_Packet_REQ`, `PS_ServerList_*`, `PS_Select_Server_*` เป็นเบาะแส ไม่ถือว่าลำดับชื่อเหล่านี้คือลำดับ handshake
- บันทึกทิศทาง, opcode, field widths, string encoding, length/count, state ก่อน/หลัง และหลักฐานอ้างอิงของแต่ละข้อความ
- ขยาย `0x2EE6` จาก count +13, entry +14/stride 52 และ lookup DWORD ใน entry +1; ไล่ consumer เพื่อหาความหมาย field และ encoding ที่ pointer entry +5
- ตรวจ response/error และการเปลี่ยนปลายทาง/session เฉพาะส่วนที่ต้องใช้ให้ Client ไปต่อถึงเป้าหมายแรก

ผลส่งมอบ: message catalog สำหรับช่วงเริ่มเชื่อมต่อและ state machine ที่มี code/trace รองรับแต่ละ transition

เกณฑ์ผ่าน: ระบุข้อความและเงื่อนไขที่จำเป็นต่อเป้าหมายแรกได้ พร้อมแยกข้อความที่ยืนยันแล้วออกจากข้อความที่ยังไม่ทราบ ไม่ใช้ตัวเลข 459 เป็นตัววัดความครบของ protocol

หากไม่มีเซิร์ฟเวอร์ที่ทำงานและเก็บ trace ได้: บันทึกขอบเขตของ outgoing trace ที่มีอยู่ แล้วใช้ handler/serializer ใน Client และการทดสอบแบบควบคุมเติมหลักฐานฝั่งรับ การไม่มี server reply เป็นช่องว่างที่ต้องแก้ ไม่ใช่เหตุให้สมมติ response สำเร็จขึ้นมา

### M4 — สร้างเซิร์ฟเวอร์ขั้นต่ำสำหรับจุดทดสอบแรก

สถานะ: มี .NET implementation พร้อม offline/memory/stdio fixture flow แล้ว; Python loopback เป็นผล Phase 4 ส่วน .NET TCP ต้องยืนยันใน VM เพราะ environment รอบนี้ปฏิเสธ connection; original-Client acceptance ยังรอ schema/rotation และอนุมัติเปิดเกม

งาน:

- เขียน transport/codec ที่ผ่าน fixture และเก็บ log เชื่อมแต่ละข้อความกับสถานะ session ได้
- ใช้ `C9.Server` เป็น runtime หลัก (`dotnet src/C9.Server/bin/Release/net10.0/C9.Server.dll`) และ `--config config/lab-fixture.json` สำหรับข้อมูลจำลองอย่างชัดเจน มี bounded framing, sequence, timeout, connection/packet limits และ metadata logs; เก็บ `python3 -m c9server` ไว้ตรวจเทียบ
- fixture login/server-list replies เป็นข้อมูลทดสอบ ไม่ใช่ auth ที่ยืนยันแล้ว; ห้ามถือว่าเกมล็อกอินสำเร็จจากผล loopback tests
- implement เฉพาะข้อความที่พิสูจน์แล้วและจำเป็นต่อเป้าหมายแรก
- กำหนดเป้าหมายยอมรับเบื้องต้นเป็น Client แลกเปลี่ยนข้อความเริ่มเชื่อมต่อ และแสดง server list จากเซิร์ฟเวอร์ทดสอบได้ หาก flow ที่ตรวจพบต้องผ่าน auth ก่อน ให้รวม auth ขั้นต่ำตามหลักฐานนั้น
- ใช้ข้อมูลทดสอบขนาดเล็กเพื่อแยกปัญหา protocol ออกจากระบบจัดเก็บข้อมูล

ผลส่งมอบ: เซิร์ฟเวอร์ที่รันได้บนเครื่องพัฒนาและหลักฐาน Client ใช้งาน flow เป้าหมายจริง

เกณฑ์ผ่าน: หลังเริ่ม Client ใหม่ สามารถทำ flow เป้าหมายซ้ำได้ พร้อมยืนยัน codec, state และ error ที่เกี่ยวข้อง การเปิด TCP listener ได้อย่างเดียวไม่ใช่เกณฑ์ผ่าน

### M5 — ขยายจากล็อกอินสู่การเข้าโลกเกม

ทำหลัง M4 ผ่าน:

1. เลือก server/channel และยืนยัน session handoff ตาม flow ที่พบ
2. แสดง/เลือกตัวละครและ persistence ที่ต้องใช้
3. โหลดเข้าเมืองหนึ่งแห่งและยืนยัน movement/state synchronization
4. เพิ่ม instance, combat และข้อมูลเกมเฉพาะระบบที่มี protocol รองรับ
5. ประเมินความครบของเนื้อหาเก้าทวีปจากทั้งข้อมูล Client และพฤติกรรม server ที่ implement แล้ว

แต่ละขั้นต้องมี acceptance scenario ที่ Client ทำได้จริง การมี asset ของทวีปใน Client ยังไม่ยืนยันว่ามี logic ฝั่ง server ครบ

## วิธีทำงานและการเก็บหลักฐาน

- ย้ายการพัฒนาระยะยาวไป UTM/Windows ตามการตัดสินใจล่าสุด; ผลบน macOS เก็บเป็นหลักฐานเดิมและยังใช้เครื่องมือ offline ได้ โดยไม่ถือว่าเป็นผลทดสอบ Windows
- ใช้ Computer Use กับงานที่ต้องดูหน้าต่าง/พฤติกรรม ส่วนการ export และวิเคราะห์ซ้ำใช้สคริปต์ที่มีอยู่เพื่อลดปัญหาการควบคุม Java UI
- เก็บรายการฟังก์ชัน/region ที่วิเคราะห์แล้ว พร้อม timeout/warnings ที่ยังมี ไม่ถือว่า export สำเร็จเท่ากับ auto-analysis ครบทุกส่วน
- ผูกข้อสรุปกับ hash, VA/RVA, code/assembly และ runtime event ที่เกี่ยวข้อง
- แยกสถานะหลักฐานเป็น: ยืนยันจาก static / สังเกตจาก runtime / ตีความ / ยังไม่ทราบ การยืนยันคนละชนิดช่วยเสริมกัน แต่ใช้แทนกันทั้งหมดไม่ได้
- รักษารายงาน Phase 1 เป็นบันทึกผลเดิม และสร้างบันทึกผลระยะถัดไปเมื่อได้หลักฐานใหม่

## งานสามรายการแรกที่เริ่มต่อได้

| ลำดับ | งาน | สิ่งที่ต้องได้ |
|---:|---|---|
| 1 | ย้าย source ไป UTM โดยเลือกวิธีส่ง source/history; ตั้ง SDK และ Client path พร้อมเทียบ hash | source ครบ, baseline ตรง และ .NET TCP validation ใน VM โดยยังไม่เปิดเกม |
| 2 | ขยาย key rotation, auth response, server entry และ handoff ด้วย static evidence | ขอบเขต codec และ field semantics ที่มีหลักฐานเพิ่ม; จุดที่ต้องเปิดเกมพักรออนุมัติ |
| 3 | ตรวจ Client interoperability หลังอนุมัติเปิดเกม แล้วเพิ่ม persistence/load/recovery tests | Client flows ที่ทำได้จริงและเกณฑ์ก่อนนำ server ไปรันใช้งาน |

งานที่ลดลำดับความสำคัญในช่วงนี้คือการไล่ชื่อ packet ทั้ง 459 ชื่อให้ครบ การกำหนด schema ฐานข้อมูลทั้งเกม และการสร้าง admin panel เพราะยังไม่ช่วยยืนยันว่า Client สื่อสารกับเซิร์ฟเวอร์ใหม่ได้
