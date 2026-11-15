# Portal modules: Game Account (3.2) และ Game World (3.3) — วิธีใช้และ Current State

อัปเดต: 15 พฤศจิกายน 2026 (Asia/Bangkok)
เกี่ยวข้อง: [GAME_ACCOUNT_WORLD_MANAGEMENT_TH.md](GAME_ACCOUNT_WORLD_MANAGEMENT_TH.md) (design),
[GAME_DATA_TH.md](GAME_DATA_TH.md) (data layer), [ADR 0002](adr/0002-game-data-access.md)

## 1. Current State (สำหรับส่งต่อให้ model/คนอื่น)

| รายการ | สถานะ |
|---|---|
| `src/C9.Portal.Accounts` | **เสร็จ** — REST `/api/v1/accounts/*` + หน้า `/accounts`, `/accounts/{id}`, `/characters/{id}`, `/accounts/online`; hosted service `SessionRecorder` |
| `src/C9.Portal.World` | **เสร็จ** — REST `/api/v1/world/*` + หน้า `/world`, `/world/events`, `/economy` |
| `tests/C9.Portal.GameData.Validation` | Kestrel จริง + auth จาก header + Blazor prerender, **ผ่าน 11/11 (103 assertions)** |
| เสียบเข้า host `C9.Portal` | **เสร็จ (15 พ.ย. ~16:35 น., thread Server)** — ตรวจใน Docker profile `gamedata` (postgres:18-alpine): migration apply ได้, `/api/v1/accounts` = 200, `/api/v1/world/rates` คืนข้อมูล; ไม่มี DB แล้ว `/accounts` แสดง GameDataGate. รายละเอียด compose/secret: [SERVER_PLATFORM_ARCHITECTURE_TH.md](SERVER_PLATFORM_ARCHITECTURE_TH.md) §6.3 |
| ทดสอบบนเครื่องเจ้าของ (17:00 น.) | Windows + Docker Desktop + PostgreSQL 18: migration ขึ้น, หน้า `/accounts`, `/accounts/online`, `/world`, `/economy` เปิดได้ไม่มี console error. หน้า "บัญชีผู้เล่น" ยังว่างเพราะ lobby ยังเก็บตัวละครใน `characters.local.json` |
| หน้าตา | ~17:30 น. ปรับให้ใช้ design system ของ Portal (`page-head`, `panel`, `cards`, `filters`/`field`, `kpis`/`metric`, `alert`, `badge`); CSS ของ module เหลือแค่ layout ภายใน panel. ตรวจด้วย screenshot ของหน้า prerender + `portal.css` (`C9_VALIDATION_DUMP=<folder>` เก็บ HTML) |
| Lobby ใช้ฐานข้อมูล | **เสร็จ (15 พ.ย. ~17:45 น.)** — เมื่อตั้ง `C9_DB_CONNECTION` lobby เก็บตัวละครในบัญชี `local` ของ DB (`src/C9.Server/CharacterStores.cs`), id ใน packet = id ใน DB, import `characters.local.json` ครั้งแรกครั้งเดียว (slot เดิม, เฉพาะเมื่อบัญชียังไม่มีตัวละครเลย) และไม่เขียน JSON อีก; ไม่ตั้งค่า = ใช้ JSON เหมือนเดิมทุก byte. lobby ไม่ migrate เอง: ถ้า schema ยังไม่พร้อม คำขอ login ใน lobby จะถูกปิดพร้อมข้อความ "Game database is not ready" แล้วลองใหม่ได้ในครั้งถัดไป |
| ผูก session ↔ บัญชี | **บางส่วน** — lobby บน DB ส่ง `SessionInfo.AccountId` (= บัญชี `local`) + character id จาก DB → `ops.sessions` ผูกบัญชีแล้ว; แยกผู้เล่นหลายบัญชีต้องรอ username จาก login 0x2F04 (thread Server) |
| แก้ inventory/gold | **ตั้งใจยังไม่เปิด** — game server ยังไม่มีระบบเหล่านี้ (design §7) |
| ทดสอบบน Windows + PostgreSQL 18 | validation suites รันใน Linux container (PostgreSQL 16); host + PostgreSQL 18 ผ่าน smoke test ใน Docker ของ thread Server; ยังไม่ได้เปิดหน้าเว็บด้วย browser จริง |

## 2. โครงสร้าง

```
src/C9.Portal.Accounts/            (Razor class library, Id "accounts", Order 200)
  AccountsModule.cs                IPortalModule: ConfigureServices + MapApi
  Shared/GameDataApi.cs            helper กลางของทั้งสอง module: Write() = Idempotency-Key + reason + audit + map error
  Shared/PortalActions.cs          ตัวช่วยของหน้าเว็บ (เขียนผ่าน service เดียวกับ API, audit เหมือนกัน)
  Api/AccountsDtos.cs, AccountsQueries.cs (timeline, session history)
  SessionRecorder.cs               ISessionFeed → ops.sessions; refresh ban ที่หมดอายุทุก 5 นาที
  Components/GameDataGate.razor    แสดงคำแนะนำเมื่อยังไม่ตั้ง C9_DB_CONNECTION
  Pages/                           AccountList, AccountDetail, CharacterDetail, OnlineSessions
src/C9.Portal.World/               (Id "world", Order 210; reference C9.Portal.Accounts เพื่อใช้ helper กลาง)
  WorldModule.cs, Api/EconomyQueries.cs
  Pages/                           WorldOverview, WorldEvents, Economy
tests/C9.Portal.GameData.Validation/  Host.cs (test host), Render.cs (root component), Program.cs (cases)
```

## 3. API และสิทธิ์ (RBAC)

ทุก endpoint อยู่ใต้ `/api/v1/{module}` ที่ host บังคับ login แล้ว; module เพิ่ม policy เอง

| Policy | Roles | ใช้กับ |
|---|---|---|
| `AccountsRead` | Owner, Administrator, GM, ReadOnly | GET บัญชี/ตัวละคร/timeline/sessions/online |
| `AccountsWrite` | Owner, Administrator, GM | สร้าง/แก้บัญชี, reset password, ban/revoke, แก้ตัวละคร, ย้าย/ลบ/กู้, import, disconnect |
| (ใน handler) | Owner, Administrator | ตั้ง/ถอดสถานะ GM ของบัญชี |
| `ServerOperate` | Owner, Administrator | `PUT /world/maintenance`, `POST /accounts/import-legacy` |
| `WorldRead` | ทุก role | GET rates, events, maintenance, announcements, population, economy |
| `WorldWrite` | Owner, Administrator, ContentEditor | rates, events, ประกาศ |

Accounts: `GET /`, `GET /{id}`, `POST /`, `PATCH /{id}`, `POST /{id}/password`, `POST /{id}/sanctions`,
`POST /{id}/sanctions/{sid}/revoke`, `GET /{id}/timeline`, `GET /{id}/sessions`, `GET|PUT /{id}/rates`,
`GET /{id}/characters`, `GET /characters/{cid}` (+ `/snapshots`, `/export`), `POST /characters/{cid}/rename|move|delete|restore`,
`PATCH /characters/{cid}`, `POST /{id}/characters/import`, `POST /import-legacy` (รับ JSON content ไม่รับ path),
`GET /online`, `POST /online/{key}/disconnect`

World: `GET|PUT /rates`, `GET|POST /events`, `PUT /events/{id}`, `POST /events/{id}/delete`, `GET|PUT /maintenance`,
`GET|POST /announcements`, `GET /population`, `GET /economy?from&to&top`

กติกาการเขียน (ทุก POST/PUT/PATCH):
- ต้องมี header `Idempotency-Key` (GUID) และ `reason` ใน body ไม่งั้นได้ 400; ส่งซ้ำด้วย key เดิมได้ผลเดิม ไม่ทำซ้ำ
- แก้ข้อมูลที่มี `version` เก่า → 412; ชื่อซ้ำ → 409; ไม่พบ → 404; ไม่มีสิทธิ์ → 403; ยังไม่ตั้ง DB → 503 พร้อมคำแนะนำ
- ทุกการเขียนลง `IAuditLog` ของ host ทั้งสำเร็จและล้มเหลว และลง `ops.audit_log` ใน transaction เดียวกับการแก้

## 4. การเสียบเข้า host (สำหรับ thread Server)

1. เพิ่ม `ProjectReference` ไป `src/C9.Portal.Accounts` และ `src/C9.Portal.World` ใน `C9.Portal.csproj` (Condition="Exists(...)")
2. host เรียก `AddC9GameData` + `MigrateGameDataAsync` เมื่อมี `C9_DB_CONNECTION` (ตกลงกันไว้แล้ว)
3. `MapRazorComponents<App>().AddAdditionalAssemblies(...)` ต้องรวม assembly ของทั้งสอง module เพื่อให้ routing หน้าเว็บทำงาน
4. ต้องมี `AddCascadingAuthenticationState()` และ `UseAntiforgery()` (Blazor Web App มาตรฐาน); `PortalActions` module ลงทะเบียนเอง
5. `C9.Portal.World` ขึ้นกับ `C9.Portal.Accounts` — โหลด World ได้เฉพาะเมื่อมี Accounts

## 5. วิธีรันทดสอบ

```powershell
# ไม่มี DB: เฉพาะ case "503 + คำแนะนำ"
dotnet run --project tests/C9.Portal.GameData.Validation
# ครบทุก case (สร้าง DB ชั่วคราวแล้วลบเอง)
$env:C9_DB_ADMIN_CONNECTION = "Host=127.0.0.1;Port=5432;Username=c9;Password=...;Database=postgres"
dotnet run --project tests/C9.Portal.GameData.Validation
# log ของ host เวลา debug
$env:C9_VALIDATION_VERBOSE = "1"
```

## 6. การควบคุมตาม ISO/IEC 27001 / 9001 / 29110

| ข้อกำหนด | วิธีที่ทำ | หลักฐาน (test case) |
|---|---|---|
| Access control (27001 A.5.15, A.8.2) | policy ต่อ endpoint และต่อหน้า; ฟอร์มเขียนซ่อนจาก role ที่ไม่มีสิทธิ์; สิทธิ์ GM เฉพาะ Owner/Admin | "authentication and RBAC are enforced per endpoint", "GM status needs Owner/Administrator", "module pages render ... hide write forms" |
| Logging (A.8.15) | audit ทุกการเขียนรวมถึงที่ล้มเหลว, disconnect ก็ audit | "writes require Idempotency-Key and a reason", "online sessions ... audited disconnect" |
| SQL injection (A.8.28) | input เป็น parameter, sort whitelist | "search input is data (injection probes)" |
| ไม่อ่านไฟล์ตามใจบน server | import รับเนื้อ JSON ไม่รับ path | "legacy import ... through the API" |
| Integrity | xmin → 412, idempotency key | "stale versions get 412", "retries are idempotent" |
| Traceability (29110) | A1–A5, A11–A14, A18, W1–W6 → endpoint/หน้าตามตาราง §3 | — |

## 7. ผลการตรวจ (15 พ.ย. 2026, Linux container, PostgreSQL 16)

```
dotnet build tests/C9.Portal.GameData.Validation   → Build succeeded, 0 warnings
C9.Portal.GameData.Validation                      → 11/11 passed, 103 assertions, 5.6s
C9.GameData.Validation                             → 17/17 passed, 111 assertions, 5.8s
```

## 8. ขั้นถัดไป

1. ~~เสียบ module เข้า `C9.Portal`~~ เสร็จแล้ว — เหลือเปิดหน้า `/accounts`, `/world`, `/economy` ด้วย browser จริงและลองเขียนข้อมูล
2. หา username ใน login 0x2F04 (thread Server) → lobby เลือกบัญชีตาม login แทนบัญชี `local`
3. ~~ย้าย `CharacterLobby` ไปใช้ `CharacterService`~~ เสร็จแล้ว — ต่อไป: ส่ง level/ตำแหน่งจาก DB ใน packet (ตอนนี้ยังส่ง level 1 เหมือนเดิม เพราะความหมาย field ยังยืนยันไม่ครบ)
4. รันทั้งสองชุดทดสอบบน Windows ของเจ้าของ + PostgreSQL 18 ใน Docker (profile `gamedata`)

หมายเหตุ log: ครั้งแรกที่ migrate บน DB ว่าง EF จะ log "Failed executing DbCommand ... ops.__ef_migrations_history" ก่อน "migrations applied" — เป็นการตรวจว่ามีตาราง history หรือยัง ไม่ใช่ error จริง

## 9. Lobby บนฐานข้อมูล (สำหรับ thread Server / ผู้ดูแล compose)

- ตัวแปรของ `c9-lobby`: `C9_DB_CONNECTION` (ค่าเดียวกับ portal, ว่าง = JSON), `C9_DB_PASSWORD_FILE=/c9/state/secrets/db/password.txt` (lobby ต้องอ่านไฟล์นี้ได้), `depends_on: c9-db` แบบ `required: false`; คง `--character-store` ไว้เพื่อ import ครั้งแรก
- `C9.Server` อ้าง `C9.GameData` แบบ `PrivateAssets=all` ดังนั้น `C9.Admin.Core`/เครื่องมือ admin ไม่ได้ EF Core/Npgsql ติดไปด้วย; โค้ด DB อยู่ใน `GameDataCharacterStore` ที่สร้างเฉพาะเมื่อตั้งค่า
- `GameDataConnection.Resolve` (C9.GameData) อ่านรหัสผ่านจากไฟล์เหมือน helper ใน Portal; ค่า option `GameDatabase` เก็บ connection string ที่ไม่มีรหัสผ่าน
- ถ้า lobby เริ่มก่อน Portal migrate: data source ของ Npgsql ถูกทิ้งแล้วสร้างใหม่ (type `citext` ถึงจะโหลดได้)
- ทดสอบ: `tests/C9.Validation` 56/56 (เพิ่ม case "Lobby characters in the game database" ที่รันจริงเมื่อมี `C9_DB_ADMIN_CONNECTION`: ยังไม่ migrate → ปฏิเสธ, import Lab/Lab2 slot เดิม, id จาก DB ไม่ใช่ Slot+1, สร้าง/ส่งซ้ำ/เลือกตัวละคร, AccountId ใน SessionInfo, เปลี่ยนชื่อ/ลบใน Portal แล้วเห็นผลใน lobby, restart ไม่ import ซ้ำ, ไฟล์ JSON ไม่ถูกแก้), `C9.GameData.Validation` 17/17, `C9.Portal.GameData.Validation` 11/11 — รันใน Linux container + PostgreSQL 16; `Test-C9Platform.ps1` ต้องรันบน Windows + Docker
- Event สำหรับ Loki/Portal: `character_store_ready` (ครั้งเดียวเมื่อเปิด store บน DB สำเร็จ, Reason "game database, account local (id N)") และ `legacy_import` (ครั้งเดียวเมื่อ import JSON, Reason "N imported, M skipped from <ชื่อไฟล์>"); ไม่มี connection string หรือชื่อตัวละครใน event
- ยืนยันบนเครื่องผู้ใช้ (Windows + Docker, 15 พ.ย. 2026 ~21:07 น.): thread Server ต่อ env ใน compose แล้ว, `Test-C9Platform.ps1` ผ่าน 14/14, `game.accounts` มี `local`, ตัวละคร Lab slot 0 ถูก import
- ส่วน 0x0BC0/0x0C1E (ขนาด payload ของ client ใหม่, ค่าความเร็ว) เป็นงานของ thread "Make the character move in game"; store/id ของ lobby ต้องคงเดิม
