# Current State ล่าสุด (15 พฤศจิกายน 2026) และ Roadmap รอบถัดไป

อัปเดตวันที่ 15 พฤศจิกายน 2026 (Asia/Bangkok) เอกสารนี้เป็นสถานะล่าสุดที่ใช้เริ่มงานต่อ
ส่วนรายละเอียดการตรวจโค้ดและ Release scripts เมื่อวันที่ 14 อยู่ใน
[CURRENT_STATE_20261114_TH.md](CURRENT_STATE_20261114_TH.md) และแผนระยะยาว P0–P7
อยู่ใน [DEVELOPMENT_PLAN_TH.md](DEVELOPMENT_PLAN_TH.md)

## 1. สรุปสั้น

- **เข้าเกมด้วย Client รุ่นใหม่ได้แล้ว** ผู้ใช้เข้าเกมได้ด้วย VFUN Client ที่แพตช์เมื่อ
  14 พ.ย. 2026 เวลา 18:58 โดยเปิดผ่านโหมด **"ทดสอบ Client รุ่นใหม่ (Local)"** ของ L9
  และต้อง **Run as administrator**
- Client ปัจจุบัน: `C:\VFUN\C9-GL\C9.exe`, 6,886,296 bytes,
  SHA-256 `b4580ed4f8fa6670501577918ad61a3318d72b119bb83caaeab5aab5f31d3bb7`
- ก่อนหน้านี้ Build 55350 (SHA-256 `9c6816ab...`) เคยผ่าน login → lobby 17 ช่อง →
  สร้าง Fighter "Lab" → เข้า Waterford (HP 100/100) บน Windows ARM64 VM
- ระบบเกมหลังเข้าเมือง (movement, combat, inventory, persistence, cipher หลัง epoch แรก)
  **ยังไม่ได้ implement** ต้องพัฒนาต่อ

## 2. หลักฐานจากเครื่อง Windows x64

| รายการ | ผล |
|---|---|
| Hash Client | `b4580ed4...` (ไม่ตรง profile Build 55350 เดิม) |
| Build number | ไม่มีใน metadata (FileVersion 2.0.0.0); exe ถูกป้องกันด้วย WinLicense จึง RE แบบ static ได้จำกัด ต้องใช้ runtime packet trace |
| โหมดทดสอบ | `Observed20261114` ใน `LocalLaunchPlan.cs` + flag `--compatibility-test` / checkbox ใน L9 เปิด Client ใหม่ด้วยรูปแบบ argument ของ Build 55350 |
| Elevation | manifest ของ C9.exe เป็น `requireAdministrator`; การเปิดแบบไม่ elevate ล้มด้วย "requires elevation" ต้องเปิด L9 แบบ administrator |
| L9.local.log | เริ่ม Client 04:49 UTC (pid 16544, endpoint loopback) และปิดที่ 04:59 UTC ด้วย exit code 1 (ยังไม่ทราบสาเหตุ ควรตรวจในรอบถัดไป) |
| Git | HEAD ยังเป็น `2250837` (10 พ.ย.); งานของ 14–15 พ.ย. ยังไม่ commit (Working tree มีไฟล์ modified จำนวนมาก ควรตรวจว่าเป็นความต่างของ line ending หรือการแก้จริงก่อน commit) |

ข้อสังเกต: การเข้าเกมได้ยืนยันแค่ว่า handshake/login/lobby/world-entry ใช้ได้กับ Client ใหม่
ยังไม่ได้ยืนยันความเข้ากันของ packet ในระบบเกมส่วนอื่น

## 3. สิ่งที่ยังไม่เสร็จ (ยกจากเอกสารก่อนหน้า)

1. World initialization / session lifecycle หลังเข้าเมือง (`SessionProcessor.cs` รับ request อื่นแล้วยังไม่ตอบ)
2. Movement, combat, inventory, persistence
3. Cipher rotation หลัง initial epoch 256 frames, multiplayer ข้ามเครื่อง และ UDP
4. ระบบบัญชีหลายบัญชี (ตอนนี้เป็น synthetic local account + JSON store)
5. Admin: คิวส่งของและ rates ยังไม่เชื่อมกับ game handlers
6. การจับ packet ของ Client ใหม่เพื่อยืนยัน layout ที่เปลี่ยน

## 4. Roadmap ที่ผู้ใช้กำหนด (15 พ.ย. 2026)

จัดตามความรุนแรง/ความซับซ้อน ยิ่งสูงยิ่งใช้โมเดลและ effort สูง:

| # | งาน | ระดับ | สถานะ |
|---|---|---|---|
| 0 | อัปเดต Current State ใน docs (เอกสารนี้) | ต่ำ | เสร็จ |
| 1 | Client Launcher ใช้ .NET MAUI ที่ responsive และใช้งานง่าย | กลาง | วางแผน |
| 2 | Client Launcher ตั้งค่า Directory ของเกมได้ (แทน path ตายตัว `C:\VFUN\C9-GL`) | กลาง | วางแผน (ทำร่วมกับ 1) |
| 3 | ย้าย Server จาก Windows Form/MAUI เป็น Web Application รันด้วย Docker Compose | **สูง** | วางแผน |
| 3.1 | Server Admin Portal: Dashboard ภาพรวม (Grafana ฟรีได้), Manage/Monitor server, **Trace TCP/UDP packet ระหว่าง Client–Server** เพื่อวิเคราะห์ error | สูง | วางแผน |
| 3.2 | Game Account Management: จัดการบัญชีและตัวละครทั้งหมดในบัญชี + ไอเดียเพิ่มเติม (เช่น ban/suspend, ประวัติ login, ส่งไอเทม/แก้ข้อมูลตัวละครพร้อม audit log, session ที่กำลังออนไลน์) | กลาง–สูง | วางแผน ออกแบบก่อน |
| 3.3 | Game World Management: Event, Economic Dashboard และไอเดียเพิ่มเติม (เช่น rates, รายชื่อผู้เล่นออนไลน์ต่อแผนที่, ประกาศระบบ, บันทึก drop/เงินเข้าออก) | กลาง–สูง | วางแผน ออกแบบก่อน |

ลำดับพึ่งพา: งาน 3.2 และ 3.3 ต้องใช้สถาปัตยกรรม/API จากงาน 3 และ 3.1 จึงออกแบบก่อนแล้วค่อย
สร้างบน API ของ Web Server เมื่อพร้อม ส่วนงาน 1–2 แยกทำได้อิสระ

### แนวทางเบื้องต้นของงาน 3 (ยังไม่ตัดสินใจเป็น ADR)

- Web Application (ต่อยอดจาก `src/C9.Admin`) + game server รันเป็นบริการใน `docker compose`
- Monitoring: Prometheus/Grafana (ฟรี) สำหรับ metrics; packet trace เก็บเป็น log/pcap-like
  ที่ฝั่ง server เพื่อดูได้จาก Portal โดยไม่ต้องพึ่ง Wireshark บนเครื่องผู้เล่น
- ข้อควรระวัง: Client ต้อง elevated บน Windows และ Docker บน Windows ต้องเปิด port 14000/14001/14880
  ให้ Client เข้าถึงได้ ต้องทดสอบเรื่อง loopback/port mapping

## 5. ข้อกำหนดการทำงานร่วมกัน

หลายงานแก้ใน `C:\MyProjects\C9-Server` พร้อมกัน แต่ละงานควรอยู่ในพื้นที่ของตน
(Launcher: `src/C9.LocalLauncher` หรือ project ใหม่ของ Launcher; Server/Admin: `src/C9.Server`, `src/C9.Admin*`;
Docs: `docs/`) และควรอัปเดตเอกสารนี้หรือเพิ่ม ADR ใน `docs/adr/` เมื่อมีการตัดสินใจสำคัญ

## 6. ความคืบหน้างาน 3.2/3.3 — Game Account/World Management (อัปเดต 15 พ.ย. 2026 ~12:35 น.)

- ออกแบบแล้ว: [GAME_ACCOUNT_WORLD_MANAGEMENT_TH.md](GAME_ACCOUNT_WORLD_MANAGEMENT_TH.md) (ฟีเจอร์, ไอเดียเพิ่ม, schema, เฟส + severity)
- เลือก data access: [ADR 0002](adr/0002-game-data-access.md) — EF Core 10 + Npgsql (option matrix, เจ้าของขอ ORM ที่กัน SQL injection)
- `NuGet.Config` เปลี่ยนจาก "ไม่มี package" เป็น nuget.org + `packageSourceMapping` เฉพาะ EF Core/Npgsql (และชุด design-time)
- สร้าง `src/C9.GameData` (15 ตาราง schema `game`/`ops`, migration, services), `tools/C9.GameData.Design`,
  `tests/C9.GameData.Validation` (17/17 ผ่านบน PostgreSQL 16 ใน container), `db/migrations/game-data.sql`; เพิ่มใน `C9.slnx`
- สถานะละเอียด วิธีรัน และหลักฐานทดสอบ: [GAME_DATA_TH.md](GAME_DATA_TH.md)
- อัปเดต ~16:10 น.: สร้าง Portal modules `src/C9.Portal.Accounts` + `src/C9.Portal.World` (REST + หน้า Blazor) และ
  `tests/C9.Portal.GameData.Validation` (11/11 ผ่าน, 103 assertions: RBAC, idempotency, audit, injection, prerender หน้าเว็บ); เพิ่มใน `C9.slnx`
  รายละเอียด/วิธีเสียบเข้า host: [GAME_PORTAL_MODULES_TH.md](GAME_PORTAL_MODULES_TH.md)
- ~16:35 น.: thread Server เสียบ module เข้า host `C9.Portal` แล้ว ตรวจใน Docker + PostgreSQL 18 ผ่าน (ดู SERVER_PLATFORM_ARCHITECTURE_TH.md §6.3)
- ~17:45 น.: lobby เก็บตัวละครใน DB เมื่อตั้ง `C9_DB_CONNECTION` (import JSON ครั้งแรก, id จาก DB, AccountId ใน SessionInfo); ไม่ตั้ง = JSON เหมือนเดิม. C9.Validation 56/56, GameData 17/17, Portal 11/11. thread Server ต่อ env ใน compose แล้ว และ Test-C9Platform.ps1 ผ่าน 14/14 บนเครื่องผู้ใช้ (~21:07 น.); เพิ่ม event `character_store_ready`/`legacy_import` (~21:15 น.). รายละเอียด: GAME_PORTAL_MODULES_TH.md §9
- ยังไม่ทำ (เดิม): เปิดหน้าเว็บด้วย browser จริง, เชื่อม `CharacterLobby` กับ DB, ผูกบัญชีจาก login packet 12036,
  รันทดสอบบน Windows ของเจ้าของ

## 7. ความคืบหน้างาน 3/3.1 — Server เป็น Web App + Docker Compose, Admin Portal, Packet trace (อัปเดต 15 พ.ย. 2026 ~16:40 น.)

- **เสร็จและทดสอบแล้วใน Linux container** รายละเอียด/วิธีใช้/API/ความเสี่ยง: [SERVER_PLATFORM_ARCHITECTURE_TH.md](SERVER_PLATFORM_ARCHITECTURE_TH.md),
  การตัดสินใจ: [ADR 0003](adr/0003-web-portal-docker-compose.md)
- `C9.Server --agent-url ...` (agent mode): HTTP API ภายใน (status/control/sessions/events/trace/export/SSE, Bearer token), `/metrics`
  สำหรับ Prometheus, start/stop/restart listener โดย process ไม่ตาย, packet trace ทุก frame (wire + payload ที่ถอดรหัส, ซ่อนข้อมูลล็อกอิน
  เป็นค่าเริ่มต้น), export pcap/JSONL + Wireshark dissector; ไม่ใส่ `--agent-url` = พฤติกรรมเดิมทุกอย่าง
- ใหม่: `src/C9.Contracts`, `src/C9.Portal.Abstractions`, `src/C9.Portal` (Blazor Web App + REST `/api/v1`, RBAC 5 role, audit,
  หน้า Dashboard/Services/Sessions/Packet trace/Parameters/Audit) — โมดูล Accounts/World เสียบเข้า host แล้ว (host เรียก `AddC9GameData` + migrate เมื่อมี `C9_DB_CONNECTION`)
- Docker: `compose.yaml` + `docker/` (c9-login, c9-lobby, c9-portal, Prometheus, Loki, Alloy, Grafana + c9-db ใน profile `gamedata`),
  ทุก port bind 127.0.0.1, state ที่ `artifacts/docker-state`; สคริปต์ `scripts/docker/{Start,Stop,Test}-C9Platform.ps1`, `Get-C9PortalKey.ps1`
- `NuGet.Config` เพิ่ม `Microsoft.AspNetCore.App.Internal.Assets` (ไฟล์ blazor.web.js ของ Microsoft, ADR 0003)
- ผลตรวจ: `tests/C9.Validation` 55/55, `tests/C9.Platform.Smoke` 14/14 กับ stack จริง, Playwright ทุกหน้าไม่มี console error,
  profile `gamedata` migrate ผ่านและ `/api/v1/accounts` ตอบ 200
- ยังไม่ทำ: ทดสอบบน Windows + Docker Desktop กับ C9.exe จริง, ถอด username จาก 12036 เพื่อผูก session กับบัญชี,
  ย้าย `CharacterLobby` ไปใช้ DB, ผู้ใช้ Portal หลายคน, ยังไม่ commit git


## 8. ข้อมูลอ้างอิงจาก Server C9 รุ่นเก่า (LouLx, build 2014) (อัปเดต 15 พ.ย. 2026 ~21:45 น.)

- ผู้ใช้พบ backup VM ที่ `C:\MyProjects\New folder\C9-5-Class-LouLx` (server ต้นฉบับครบชุด build 20 พ.ย. 2014, Client Version 1495, China)
  อ่านแบบ read-only ไม่รันไฟล์ใด สำเนาอ้างอิงเก็บนอก git ที่ `C:\MyProjects\C9-Legacy-Ref\patch65-loulx\`
- รายละเอียดทั้งหมด: [LEGACY_PATCH65_REFERENCE_TH.md](LEGACY_PATCH65_REFERENCE_TH.md); หลักฐาน: `re/evidence/legacy_patch65/`; สคริปต์: `re/scripts/legacy/`
- ผลหลัก: catalog 1,304 packet + handler 521 ตัวของ GameServer; opcode คงเดิมถึง Client ปัจจุบัน (layout 0x0BCE 46 bytes ตรงกับ trace);
  packet ในโลกที่ยังไม่ตอบมี handler เดิม 8/10 (ไม่มี 0xF74F, 0xEEB3); reply ที่ควรทำก่อน: 0x0BC6, 0x0EFE, 0xED1E
- Cipher: C2S selector = (checksum + typeHash[type]) mod N, S2C ไม่บวก typeHash; ±16 ใน trace มาจาก 32-bit overflow (2^32 mod 80 = 16)
- DB เดิม SQL Server 2008 R2 9 ฐาน + API 297 stored procedure (เทียบกับ EF Core GameData แล้ว); ยังไม่ได้อ่านตาราง/คอลัมน์จาก .mdf (รอผู้ใช้ตัดสินใจเรื่อง license SQL Server)
