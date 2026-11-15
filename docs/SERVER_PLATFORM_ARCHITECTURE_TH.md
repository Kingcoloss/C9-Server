# C9 Server Platform: Web Admin Portal + Docker Compose (งาน 3 และ 3.1)

อัปเดต: 15 พฤศจิกายน 2026 (Asia/Bangkok) · ผู้รับผิดชอบ: thread "Move server to web app with Docker"
เกี่ยวข้อง: [ADR 0003](adr/0003-web-portal-docker-compose.md), [GAME_PORTAL_MODULES_TH.md](GAME_PORTAL_MODULES_TH.md) (โมดูล 3.2/3.3),
[GAME_DATA_TH.md](GAME_DATA_TH.md), [ADR 0002](adr/0002-game-data-access.md), [LOCAL_LAUNCHER_MAUI_TH.md](LOCAL_LAUNCHER_MAUI_TH.md)

เอกสารนี้คือ Current State + Runbook + สัญญา API ของ platform ฝั่ง server ใครรับงานต่อ (คนหรือ model อื่น) อ่านหัวข้อ 1, 3 และ 11 ก่อน

## 1. Current State (สำหรับส่งต่อ)

| รายการ | สถานะ | หลักฐาน |
|---|---|---|
| Game server "agent mode" (`C9.Server --agent-url`) | **เสร็จ** HTTP API ภายใน, metrics, start/stop/restart listener, reload config | `tests/C9.Validation` 54/54 |
| Packet trace (TCP) | **เสร็จ** ทุก frame ทั้งสองทิศ, ไบต์ตามสาย + payload ที่ถอดรหัส, ซ่อนข้อมูลล็อกอิน, เก็บลงดิสก์, export pcap/JSONL | smoke test ข้อ 6-7 |
| Web Admin Portal (`src/C9.Portal`) | **เสร็จ** Blazor Web App + REST `/api/v1`, Dashboard, Services, Sessions, Packet trace, Parameters, Audit | Playwright ผ่าน ไม่มี console error |
| Docker Compose (`compose.yaml`) | **เสร็จ** 7 services: c9-login, c9-lobby, c9-portal, prometheus, loki, alloy, grafana (+ c9-db ใน profile `gamedata`) | `tests/C9.Platform.Smoke` 14/14 |
| Grafana dashboard "C9 Server Overview" | **เสร็จ** provision อัตโนมัติ | smoke test ข้อ 13 |
| โมดูล Accounts/World (thread อื่น) | **เสียบเข้า host แล้ว** โหลดอัตโนมัติ; ต้องเปิด DB (`-GameData`) จึงใช้งานได้ | `/api/v1/reference/modules` |
| ทดสอบบนเครื่องผู้ใช้ (Windows + Docker Desktop) | **ผ่าน (15 พ.ย. 2026 17:05)** C9.exe จริงผ่าน L9 (MAUI) ล็อกอิน → สร้างตัวละคร → เข้าโลก (Waterford) กับ server ใน Docker; smoke test **14/14** หลังแก้เทสต์ (15 พ.ย. 21:08) | หัวข้อ 10.1 |
| Lobby เก็บตัวละครใน DB (profile `gamedata`) | **เสร็จ (thread Accounts/World)** compose ส่ง `C9_DB_CONNECTION`/`C9_DB_PASSWORD_FILE` ให้ c9-lobby; import "Lab" จาก JSON เข้าบัญชี `local` แล้ว; lobby ไม่ migrate เอง (Portal ทำ) | หัวข้อ 10.1, [GAME_PORTAL_MODULES_TH.md](GAME_PORTAL_MODULES_TH.md) §9 |
| UDP trace | **ไม่มี UDP ในเกมที่พบตอนนี้** ใช้ Wireshark/Npcap จับที่เครื่องแทน (หัวข้อ 5.4) | |
| Git | **ยังไม่ commit** ไฟล์ทั้งหมดอยู่ใน working tree ของ `C:\MyProjects\C9-Server` | |

ของเดิมยังใช้ได้เหมือนเดิม: `C9.Server --config ...` (ไม่ใส่ `--agent-url`) ทำงานแบบเดิมทุกอย่าง, L9 Launcher, Admin Console (`C9.Admin`) และ WinForms
ยังไม่ถูกลบ Portal ใช้ไฟล์ `parameters.local.json` รูปแบบเดียวกับ Admin Console เดิม

## 2. ภาพรวมสถาปัตยกรรม

```
 Windows (ผู้ใช้)                         Docker Desktop (WSL2) · project "c9" · network c9_default
 ┌──────────────┐  TCP 127.0.0.1:14000   ┌───────────────────────────┐   agent API :9101 (Bearer token)
 │ C9.exe       │───────────────────────▶│ c9-login  (C9.Server)     │◀───────────┐
 │ (เปิดผ่าน L9) │  TCP 127.0.0.1:14001   ├───────────────────────────┤   :9102     │   ┌────────────────────┐
 │              │───────────────────────▶│ c9-lobby  (C9.Server)     │◀───────────┼───│ c9-portal          │
 └──────────────┘                        └───────────┬───────────────┘   SSE live  │   │ Blazor + /api/v1   │
 Browser ──── http://127.0.0.1:14880 ───────────────────────────────────────────────────▶│ + modules 3.2/3.3  │
 Browser ──── http://127.0.0.1:3000 ──▶ grafana ──▶ prometheus (scrape /metrics ทุก 10 วิ)   └─────────┬──────────┘
                                                └─▶ loki ◀── alloy (อ่าน log container ผ่าน docker.sock)  │
                                   ./artifacts/docker-state  (bind mount /c9/state) ◀─────────────────────┘
                                   ├─ secrets/  agent.token · grafana/ · db/
                                   ├─ portal/   access-key.txt · audit/ · parameters.local.json · profiles/ · keys/
                                   ├─ trace/    login/ lobby/ YYYYMMDD/*.jsonl + sessions-index.jsonl
                                   └─ lobby/    characters.local.json
```

หลักการออกแบบ
- **Game process ไม่ผูกกับ UI**: แต่ละ service (login, lobby) เป็น process ของตัวเองพร้อม HTTP agent ภายใน Portal สั่งงานผ่าน agent เท่านั้น
  ทำให้ Portal ล่มได้โดยเกมไม่หลุด และย้าย Portal ไปเครื่องอื่นได้ในอนาคต
- **สัญญากลาง** (`src/C9.Contracts`) ใช้ร่วมกันระหว่าง agent, Portal, โมดูล และ smoke test; JSON เป็น snake_case
- **Port ทุกตัว bind ที่ 127.0.0.1** บนเครื่องผู้ใช้ Client ยังต่อ 127.0.0.1:14000 เหมือนเดิม (handoff 12013 ชี้ 127.0.0.1:14001)
- **ไม่มี package ใหม่ฝั่ง server** นอกจาก `Microsoft.AspNetCore.App.Internal.Assets` (ไฟล์ blazor.web.js ของ Microsoft เอง, ADR 0003)
  และ EF Core/Npgsql ของโมดูล 3.2/3.3 (ADR 0002)

### 2.1 Projects ที่เพิ่ม/แก้

| Project | หน้าที่ |
|---|---|
| `src/C9.Contracts` (ใหม่) | DTO ของ agent API, trace, packet catalog (`KnownPackets`), `SessionKeys`, `ContractJson` |
| `src/C9.Portal.Abstractions` (ใหม่) | สัญญาโมดูล `IPortalModule`, RBAC (`PortalRoles`, `PortalPolicies`), `IAuditLog`, `IGameServices`, `ISessionFeed`, `IPortalUserStore` |
| `src/C9.Portal` (ใหม่) | host ของ Admin Portal (Blazor Web App, interactive server) + REST + auth + audit + live feed + โมดูล "reference" |
| `src/C9.Server` (แก้) | agent mode: `Agent/` (AgentHost, GameServiceController, ConfigSource, AgentToken), `Runtime/` (SessionContext, ServiceRuntime, GameMetrics, PrometheusText, AgentStreamHub), `Tracing/` (PacketTracer, TraceRing, TraceFileStore, PcapExport) |
| `src/C9.Admin.Core` (แก้เล็กน้อย) | `AdminSettings.LoginConfig/LobbyConfig/PrepareConfig` เป็น public ให้ Portal ใช้ |
| `tests/C9.Validation` (เพิ่ม case) | 4 case ใหม่: agent trace/control, HTTP API/token/stream, token+container bind, payload policy |
| `tests/C9.Platform.Smoke` (ใหม่) | ทดสอบ end-to-end กับ stack ที่รันอยู่จริง |
| `docker/`, `compose.yaml`, `.dockerignore`, `.env.example`, `scripts/docker/*.ps1` (ใหม่) | การรันด้วย Docker |

## 3. วิธีใช้งาน (Runbook บน Windows)

ต้องมี: Docker Desktop (WSL2 backend) และ .NET SDK 10 (เฉพาะถ้าจะรัน smoke test)

```powershell
cd C:\MyProjects\C9-Server
./scripts/docker/Start-C9Platform.ps1          # build image + docker compose up -d (ครั้งแรกใช้เวลาหลายนาที)
./scripts/docker/Get-C9PortalKey.ps1 -Copy     # คัดลอก Operator key
start http://127.0.0.1:14880                    # ใส่ key เพื่อเข้า Admin Portal
```

เล่นเกม: เปิด L9 (Run as administrator) แล้วกด "ทดสอบ Client รุ่นใหม่ (Local)" ตามเดิม L9 ตรวจ greeting ที่ 14000/14001 ก่อน
ถ้า Docker ตอบแล้วจะไม่เริ่ม server ของตัวเอง (`LocalLauncherRuntime.EnsureBackendAsync`) **อย่ารัน server ของ L9 ค้างไว้พร้อม Docker**
เพราะ port ชนกัน (`Start-C9Platform.ps1` ตรวจให้)

| งาน | คำสั่ง |
|---|---|
| ดูสถานะ | `docker compose ps` |
| ดู log | `docker compose logs -f c9-login c9-lobby c9-portal` หรือ Grafana > Explore > Loki |
| หยุด (เก็บข้อมูล) | `./scripts/docker/Stop-C9Platform.ps1` |
| หยุดและลบ volume metrics/logs/DB | `./scripts/docker/Stop-C9Platform.ps1 -RemoveData` (ถามยืนยัน; `artifacts/docker-state` ไม่ถูกลบ) |
| เปิดฐานข้อมูลเกม (Accounts/World + ตัวละครของ lobby) | `./scripts/docker/Start-C9Platform.ps1 -GameData` (lobby เก็บตัวละครใน PostgreSQL บัญชี `local` และ import `lobby/characters.local.json` ครั้งแรกครั้งเดียว; ไม่ใส่ = ใช้ไฟล์ JSON เหมือนเดิม) |
| ทดสอบทั้งระบบ | `./scripts/docker/Test-C9Platform.ps1` (ต้องรัน stack อยู่) |
| เปลี่ยน Operator key | ลบ `artifacts/docker-state/portal/access-key.txt` แล้ว `docker compose restart c9-portal` |
| รหัส admin ของ Grafana | `artifacts/docker-state/secrets/grafana/admin-password.txt` (ผู้ชมทั่วไปเข้าแบบ Viewer ได้โดยไม่ล็อกอิน) |
| เปลี่ยน port / นโยบาย trace | คัดลอก `.env.example` เป็น `.env` แล้วแก้ |
| อยู่หลัง proxy ที่ตรวจ TLS | `Start-C9Platform.ps1 -CaBundle C:\path\ca.crt` (ส่งเป็น build secret ไม่ติดไปใน image) |

URL: Portal http://127.0.0.1:14880 · Grafana http://127.0.0.1:3000/d/c9-overview · Prometheus http://127.0.0.1:9090

## 4. Admin Portal

| หน้า | Policy | ทำอะไร |
|---|---|---|
| `/` ภาพรวม | server.read | KPI, การ์ดต่อ service (state, sessions, packets, bytes, errors, trace, live stream), เหตุการณ์ล่าสุด, ลิงก์ Grafana |
| `/services` | server.read / server.operate | Start / Stop (ต้องกดยืนยัน) / Restart listener, ดู config ที่ใช้จริง, เหตุการณ์ 100 รายการล่าสุด |
| `/sessions` | server.read / server.operate | session ที่ต่ออยู่และที่ปิดแล้ว, state, character/map, packets, เหตุผลที่ปิด, ตัดการเชื่อมต่อ, ลิงก์ไป trace |
| `/trace` | trace.read (+ trace.export สำหรับไบต์และ export) | รายการ session ที่บันทึก, ตาราง frame แบบ Wireshark (บน: รายการ, ล่าง: field + hex dump), กรอง ทิศทาง/ชนิด/error, live tail, โหลดเก่ากว่า, export |
| `/settings` | settings.write | HP/MP เริ่มต้น, connection limit, timeout, local account; บันทึกแล้วกด Restart ทุกบริการ |
| `/audit` | audit.read | ทุกการกระทำที่เปลี่ยนสถานะ (append-only JSONL) |
| `/reference/packets`, `/reference/modules` | trace.read / server.read | packet catalog, โมดูลที่โหลด, รายการ API |
| `/accounts`, `/world`, ... | ตาม [GAME_PORTAL_MODULES_TH.md](GAME_PORTAL_MODULES_TH.md) | โมดูล 3.2/3.3 |

Sign-in: Operator key 64 hex (สร้างอัตโนมัติครั้งแรก, ได้ role Owner) cookie `C9.Portal` HttpOnly + SameSite=Strict อายุ 8 ชม.
ผิด 5 ครั้งใน 5 นาทีต่อ IP จะถูกพัก; sign-in/sign-out ลง audit; สคริปต์ใช้ `Authorization: Bearer <key>` แทน cookie ได้
ระบบผู้ใช้หลายคน (login/password ต่อคน) เปลี่ยนได้โดยลงทะเบียน `IPortalUserStore` ตัวใหม่ (ยังไม่ทำ)

## 5. Packet trace

### 5.1 เก็บอะไร
ทุก frame ของทุก session ทั้ง C→S และ S→C: เวลา, stream offset, ความยาว, flag, type, sequence, checksum, state ของ session ณ ตอนนั้น,
**ไบต์ตามสาย (wire, เข้ารหัส)** และ **payload ที่ถอดรหัสแล้ว** (base64), จำนวน cipher frames, selector, และ error
ถ้า framing ผิด (ความยาวผิด, frame ไม่ครบ) จะเก็บไบต์ต้นทางไม่เกิน 256 byte ไว้ดูด้วย

Session key รูปแบบ `{service}-{bootId}-{n}` เช่น `login-a99fa0-3` (bootId เปลี่ยนทุกครั้งที่ process เริ่ม) ใช้ตรงกันใน
sessions API, trace, events, log (Loki) และ audit ทำให้ไล่ข้ามระบบได้

### 5.2 นโยบายข้อมูล (`--trace-payloads`, `.env` `C9_TRACE_PAYLOADS`)
| นโยบาย | ผล |
|---|---|
| `redact-credentials` (ค่าเริ่มต้น) | 0x2F04 LoginReq และ 0x2EE1 LoginReqLegacy เก็บแค่ความยาว ไม่เก็บไบต์/hash; frame ก่อนล็อกอินที่ถอดไม่ได้เก็บแค่ 16 byte แรก (เพราะ key เริ่มต้นเป็นค่าคงที่) |
| `headers` | ไม่เก็บไบต์เลย เก็บแต่ header |
| `full` | เก็บทุกอย่างรวมข้อมูลล็อกอิน ใช้เฉพาะตอนพัฒนา Portal ขึ้นแถบเตือนสีส้ม และ log บอกตอนเริ่ม |

ผู้ใช้ role GM เห็นแค่ header (ไม่เห็นไบต์และ export ไม่ได้) ส่วน Owner/Administrator เห็นและ export ได้ ทุก export ลง audit

### 5.3 การเก็บและอายุข้อมูล
หน่วยความจำ: ring 20,000 frames ต่อ service · ดิสก์: `trace/{service}/{yyyyMMdd}/{boot}-{NNN}.jsonl` ไฟล์ละไม่เกิน 128 MiB,
เก็บ 7 วัน และรวมไม่เกิน 2 GiB (ลบไฟล์เก่าสุดก่อน) · `sessions-index.jsonl` ทำให้ดู session เก่าหลัง restart ได้

### 5.4 วิเคราะห์ด้วย Wireshark
1. หน้า Packet trace เลือก session แล้วกด **pcap (decoded)** และกด **Wireshark dissector** (ได้ `c9_decoded.lua`)
2. คัดลอก `c9_decoded.lua` ไปที่ `%APPDATA%\Wireshark\plugins\` แล้วเปิด pcap: ทุก frame เป็น UDP datagram หนึ่งอันที่ port 49099
   (client 10.99.0.2, server 10.99.0.1) Wireshark แสดงชื่อ packet, ทิศทาง, sequence และ payload ที่ถอดรหัสแล้ว
3. **pcap (wire)** คือ TCP stream สังเคราะห์ของไบต์ตามสายจริง (frame ที่ถูกซ่อนเติม 0) ใช้ดู timing/ขนาด/การแบ่ง segment
4. จับ traffic จริงที่เครื่อง: Wireshark บน adapter `Npcap Loopback` filter `tcp.port in {14000 14001}` (กรณีมี UDP ในอนาคตใช้วิธีเดียวกัน
   `udp.port == N`) แล้วเทียบเวลากับหน้า trace ได้ เพราะเวลาเป็น UTC เดียวกัน

## 6. สัญญา API

### 6.1 Agent API (ภายใน network ของ compose เท่านั้น, ไม่ publish ออก host)
ทุก path ใต้ `/agent/v1` ต้องมี `Authorization: Bearer <secrets/agent.token>` (64 hex, สร้างโดย service แรกที่เริ่ม, สิทธิ์ไฟล์ 0600)

| Method | Path | หมายเหตุ |
|---|---|---|
| GET | `/healthz`, `/readyz` | ไม่ต้องใช้ token; readyz = 503 ถ้า listener ไม่ Running |
| GET | `/metrics` | Prometheus text, ไม่ต้องใช้ token |
| GET | `/agent/v1/status` | `AgentStatus` |
| POST | `/agent/v1/control` | `{"action":"start"\|"stop"\|"restart"}` → `ControlResult` (409 ถ้าไม่สำเร็จ); start/restart อ่าน config + overlay ใหม่ |
| GET | `/agent/v1/config` | config ที่ใช้จริง (หลัง overlay และ override) |
| GET | `/agent/v1/sessions?include_closed=true` | `SessionInfo[]` |
| POST | `/agent/v1/sessions/{key}/disconnect` | 404 ถ้าไม่ได้เชื่อมต่อ |
| GET | `/agent/v1/events?limit=` | `ServiceEvent[]` ล่าสุด (สูงสุด 512) |
| GET | `/agent/v1/trace/sessions?limit=` | `TraceSessionSummary[]` |
| GET | `/agent/v1/trace/frames?session=&dir=c2s\|s2c&type=0x2F04,LoginAck,3003&errors_only=&after_id=&before_id=&since=&limit=` | `TracePage` (เรียง id จากน้อยไปมาก, limit สูงสุด 5000) |
| GET | `/agent/v1/trace/sessions/{key}/export?format=jsonl\|pcap\|pcap-decoded` | ไฟล์ |
| GET | `/agent/v1/stream?session=&frames=true` | SSE: `frame`, `session`, `event`, `status`, ping ทุก 15 วิ |

### 6.2 Portal REST `/api/v1` (cookie หรือ `Authorization: Bearer <operator key>`)
`GET /me` · `GET /services` · `GET /services/{id}` · `POST /services/{id}/control` · `GET /services/{id}/events?limit=` ·
`GET /services/{id}/config` · `GET /sessions?include_closed=true` · `POST /sessions/{key}/disconnect` ·
`GET /trace/sessions?service=&limit=` · `GET /trace/frames?service=|session=&...` (ตัวกรองเดียวกับ agent) ·
`GET /trace/sessions/{key}/export?format=` · `GET /packet-types` · `GET /tools/wireshark/c9.lua` ·
`GET|PUT /settings/parameters` · `GET /audit?action=&limit=` · `GET /reference/modules` · โมดูล: `/api/v1/accounts/*`, `/api/v1/world/*`

Error เป็น JSON `{"error": "..."}` (โมดูลใช้ ProblemDetails); 401 = ไม่ได้ล็อกอิน, 403 = role ไม่พอ

### 6.3 สัญญาโมดูล (`IPortalModule`)
host โหลดทุก class ที่ implement `IPortalModule` ใน `C9.Portal.*.dll` ข้าง host (ต้องมี constructor เปล่า, Id ไม่ซ้ำ, a-z0-9-)
`ConfigureServices` รันก่อน build, `MapApi` ได้ group `/api/v1/{Id}` ที่บังคับล็อกอินแล้ว, หน้า Razor ใน assembly ของโมดูลถูกเพิ่มเข้า router อัตโนมัติ
(`@rendermode InteractiveServer` + `[Authorize(Policy = ...)]`) และเมนูมาจาก `Navigation` (ซ่อนตาม policy)
บริการที่โมดูลใช้ได้: `IGameServices`, `ISessionFeed` (session lifecycle สด), `IAuditLog`, `IPortalUserStore`, `PortalPolicies`
host ลงทะเบียน `C9.GameData` (`AddC9GameData` + migrate, retry 90 วิ) เมื่อมี `C9_DB_CONNECTION` (+ `C9_DB_PASSWORD_FILE`)

### 6.4 RBAC

| Policy | Owner | Administrator | GM | ContentEditor | ReadOnly |
|---|:-:|:-:|:-:|:-:|:-:|
| server.read | ✓ | ✓ | ✓ | ✓ | ✓ |
| server.operate (start/stop/restart, ตัด session) | ✓ | ✓ | | | |
| trace.read (header) | ✓ | ✓ | ✓ | | |
| trace.export (ไบต์, pcap/JSONL) | ✓ | ✓ | | | |
| settings.write | ✓ | ✓ | | | |
| accounts.read / accounts.write | ✓/✓ | ✓/✓ | ✓/✓ | | ✓/ |
| world.read / world.write | ✓/✓ | ✓/✓ | ✓/ | ✓/✓ | ✓/ |
| audit.read | ✓ | ✓ | | | |

## 7. Monitoring

Metrics (agent `/metrics`, label `service`): `c9_listener_up`, `c9_sessions_active`, `c9_sessions_max`, `c9_sessions_total`,
`c9_connection_rejections_total`, `c9_online_players{map}`, `c9_sessions_by_state{state}`, `c9_packets_total{direction,type,name}`,
`c9_bytes_total{direction}`, `c9_session_errors_total{reason}` (timeout, connection, disconnected, truncated_frame, cipher_epoch,
checksum, sequence, length, account_disabled, protocol), `c9_session_duration_seconds` (histogram), `c9_trace_frames_total`,
`c9_trace_dropped_total`, `c9_build_info`, process/GC gauges · Portal: `c9_portal_up`, `c9_portal_agent_up`, `c9_portal_stream_up`

Grafana "C9 Server Overview" (`docker/observability/grafana/dashboards/c9-overview.json`): สถานะ, traffic, packet types, sessions/limit,
state, errors ตามสาเหตุ, ระยะเวลา session p50/p95, trace, memory/CPU และ log จาก Loki

Loki labels: `project="c9"`, `service` (ชื่อ compose service), `container`, และ `event` สำหรับ c9-login/c9-lobby ตัวอย่าง LogQL:
`{service="c9-login", event="session_error"}` · `{project="c9"} |= "lobby-91d57e-3"` (ทุกอย่างของ session เดียว)

## 8. ความปลอดภัยและคุณภาพ (ISO/IEC 27001 · 9001 · 29110)

| มาตรฐาน / control | สิ่งที่ทำ |
|---|---|
| 27001 A.5.15/A.8.2/A.8.3 Access control, least privilege | RBAC 5 role, policy ต่อ endpoint และต่อเมนู, GM ไม่เห็นไบต์ packet |
| 27001 A.8.5 Secure authentication | key 256-bit, เปรียบเทียบแบบ fixed-time, จำกัดการลองผิด, cookie HttpOnly/SameSite=Strict, antiforgery ทุกฟอร์ม |
| 27001 A.8.15/A.8.16 Logging & monitoring | audit append-only (ใคร ทำอะไร กับอะไร ผล ก่อน/หลัง), Prometheus + Loki + dashboard |
| 27001 A.8.11/A.8.12 Data masking / DLP | trace ซ่อนข้อมูลล็อกอินเป็นค่าเริ่มต้น; smoke test ตรวจว่าไม่มีข้อมูลล็อกอินหลุดใน export และไฟล์ trace |
| 27001 A.8.20/A.8.22 Network security / segregation | ทุก port ที่ host bind 127.0.0.1, agent API ไม่ publish, token แยกจาก operator key, bind 0.0.0.0 ได้เฉพาะเมื่อสั่ง `--allow-container-bind` |
| 27001 A.8.9 Configuration management | container read-only rootfs, non-root (uid 1654), `cap_drop: ALL`, `no-new-privileges`, image version ตรึงไว้, CSP + security headers |
| 27001 A.8.24 Cryptography / secrets | secrets สร้างแบบสุ่มตอนติดตั้ง (ไม่ฝังใน repo/image), สิทธิ์ไฟล์แยกต่อ service (0600/0400) |
| 27001 A.8.13 Backup | ข้อมูลสำคัญอยู่ที่ `artifacts/docker-state` โฟลเดอร์เดียว สำรองได้ด้วยการคัดลอก (หยุด stack ก่อน) |
| 9001 §7.5 Documented information | เอกสารนี้ + ADR 0003 + Current State; การเปลี่ยนแปลงบันทึกใน audit |
| 9001 §8.5.1/§9.1 Control & monitoring | dashboard, readiness healthcheck, smoke test ซ้ำได้ |
| 29110 SI (Software Implementation) | SI.2 requirements (หัวข้อ 4-7), SI.3 architecture (หัวข้อ 2, ADR), SI.4 construction + unit tests (`C9.Validation`), SI.5 integration/test (`C9.Platform.Smoke`, Playwright), SI.6 delivery (runbook หัวข้อ 3) |
| 29110 PM (Project Management) | แผนเป็นขั้น (หัวข้อ 9), risk register (หัวข้อ 9), บันทึกผลตรวจ (หัวข้อ 10) |

## 9. ความเสี่ยงและแผนเป็นขั้น

| ID | ความเสี่ยง | Severity | การจัดการ / สถานะ |
|---|---|---|---|
| R-01 | Client ต่อ server ใน container ไม่ได้ (NAT/port forward, handoff ชี้ 127.0.0.1:14001) | สูง | **ปิดแล้ว** ยืนยันบน Windows + Docker Desktop 4.99 กับ C9.exe จริง ล็อกอินจนเข้าโลกได้ (หัวข้อ 10.1) |
| R-02 | ข้อมูลล็อกอินหลุดผ่าน trace/export | สูง | redact เป็นค่าเริ่มต้น, ทดสอบอัตโนมัติ, `full` มีคำเตือน |
| R-03 | Portal/agent ถูกเข้าถึงจากเครื่องอื่น | สูง | bind 127.0.0.1, token, RBAC; ถ้าจะเปิดให้เครื่องอื่นต้องมี TLS + ผู้ใช้รายคนก่อน |
| R-04 | Port ชนกับ server ที่ L9 เปิดเอง | กลาง | สคริปต์ตรวจ port ก่อนเริ่ม; L9 ใช้ server ที่ตอบอยู่แล้ว |
| R-05 | Trace กินดิสก์/หน่วยความจำ | กลาง | ring จำกัด, ไฟล์จำกัดขนาด/อายุ/รวม, นับ frame ที่ทิ้ง (`c9_trace_dropped_total`) |
| R-06 | IP ของ client ใน container เป็น gateway (172.x) ไม่ใช่ IP จริง | ต่ำ | ข้อจำกัดของ Docker Desktop; ใช้เวลา/session key เทียบกับ Wireshark แทน |
| R-07 | Docker Desktop ไม่มี/ไม่เปิด | กลาง | ยังรัน server แบบเดิม (L9) ได้; Portal รันนอก Docker ได้ด้วย `dotnet run --project src/C9.Portal` + agent `--agent-url http://127.0.0.1:9101` |
| R-08 | Blazor ต้องใช้ package `Microsoft.AspNetCore.App.Internal.Assets` จาก nuget.org | ต่ำ | เป็นของ Microsoft, จำกัดด้วย packageSourceMapping (ADR 0003) |
| R-09 | Alloy ใช้ docker.sock (สิทธิ์สูงต่อ Docker) | กลาง | ใช้ image ที่ตรึงเวอร์ชัน, ไม่ publish port ของ Alloy; ถ้าไม่ต้องการ log ใน Grafana ให้ `docker compose stop alloy` |
| R-10 | Grafana เปิดให้ดูแบบ anonymous Viewer | ต่ำ | bind 127.0.0.1 เท่านั้น, แก้ไขได้เฉพาะ admin |
| R-11 | Session ↔ บัญชียังผูกกันไม่ได้ (login 12036 ยังไม่ได้ถอด username) | กลาง | งานถัดไป (หัวข้อ 11) ยืนยันบน Windows: ผู้เล่นอยู่ในโลกแต่หน้า "บัญชีผู้เล่น" ยังว่าง ตัวละครอยู่ใน `lobby/characters.local.json` ไม่ใช่ DB |
| R-12 | ASP.NET Data Protection keys ของ Portal เก็บแบบไม่เข้ารหัส (`state/portal/keys`, log `XmlKeyManager[35]`) | ต่ำ | ไฟล์อยู่ใน bind mount สิทธิ์ uid 1654 เท่านั้น, bind 127.0.0.1; ถ้าเปิดให้เครื่องอื่นใช้ ให้ `ProtectKeysWithCertificate` (ISO 27001 A.8.24) |

ขั้นที่ทำแล้ว: 0 วางแผน/ประเมิน → 1 สัญญา API → 2 agent + trace (+tests) → 3 Portal → 4 Docker Compose + observability → 5 E2E + เอกสาร

## 10. บันทึกผลการตรวจ (15 พ.ย. 2026, Linux container, Docker Engine 29.9 + Compose v5.6, .NET SDK 10.0.404)

| การตรวจ | ผล |
|---|---|
| `dotnet run --project tests/C9.Validation -c Release` | 54/54 passed, 6,571 assertions (รวม 4 case ใหม่ของ agent/trace) |
| `docker build -f docker/Dockerfile` | ผ่าน image `c9-platform:local` 358 MB |
| `docker compose up -d` | 7 services running, c9-login/c9-lobby/c9-portal healthy |
| `tests/C9.Platform.Smoke` | 14/14: auth (401 ทั้งไม่มี key และ key ผิด), login flow ถึง handoff 0x2EED → 127.0.0.1:14001, lobby 0x1771 → 0x0EFA, sessions, trace + redaction + framing error, export 3 แบบไม่มีข้อมูลล็อกอิน, parameters → overlay, restart lobby โหลด overlay, audit, modules (reference, accounts, world), Prometheus, Grafana dashboard, Loki |
| Playwright (Chromium) | sign-in ผิด/ถูก, redirect กลับหน้าเดิม, Trace เลือก session/frame + hex dump, Dashboard, Services (Restart), Sessions, Settings, Audit, Accounts/World (แสดงคำแนะนำเมื่อยังไม่มี DB), มือถือกว้าง 390px ไม่มี scroll แนวนอน, ไม่มี console error |
| ยังไม่ได้ตรวจ | Windows + Docker Desktop จริง, C9.exe จริงผ่าน container, profile `gamedata` กับ Postgres 18 ใน compose |

### 10.1 ผลบนเครื่องผู้ใช้ (15 พ.ย. 2026 16:45–17:05 น., Windows x64, Docker Desktop 4.99.0, Engine 29.9.3)

| การตรวจ | ผล |
|---|---|
| `Start-C9Platform.ps1 -GameData` | ผ่าน 2 นาที 05 วินาที (build + up); login/lobby/portal/db healthy ใน ~30 วินาที, c9-init Exited (0) |
| `Test-C9Platform.ps1` | 13/14: ข้อ "exports ... carry no credentials" ตกเพราะ `File.ReadAllBytes` เปิดไฟล์ trace `.jsonl` ที่ container เขียนค้างอยู่ไม่ได้บน Windows (sharing violation) ตัว export ทั้ง 3 แบบผ่าน และสแกนไฟล์ trace ด้วย `FileShare.ReadWrite` แล้วไม่พบ marker แก้เทสต์เป็น `ReadShared()` แล้ว (รอรันซ้ำบนเครื่อง) |
| Portal (Brave/Chromium) | sign-in ด้วย operator key, Dashboard, Sessions, Packet trace, บัญชีผู้เล่น, ผู้เล่นออนไลน์, โลกเกม, เศรษฐกิจ แสดงผลได้ ไม่มี console error |
| L9 (MAUI) + C9.exe จริง | L9 ตรวจพบ server ใน Docker และไม่เปิด server ของตัวเอง → UAC → ล็อกอิน → server list → handoff → CharacterList → CharacterCreate (0x0BBB/0x0BBC) → CharacterSelect (0x0BBF/0x0BC0) → WorldEntry 0x0BC3 → WorldLoaded 0x0BC5 → InitialAttributes 0x0C1E + ExtendedVitals 0x0C20 → ตัวละครยืนในเมือง Waterford |
| Packet trace | จับ flow ข้างบนครบทั้งสองบริการ, LoginReq แสดงเป็น `redacted` |
| Grafana `c9-overview` | listener/agent UP, sessions 1, ผู้เล่นในโลก 1, กราฟ packets/bytes ขึ้นตามจริง |
| ข้อสังเกต | (1) client ส่ง packet ในโลกที่ยังไม่มีใน catalog และ server ยังไม่ตอบ: 0xF74F, 0xED1D, 0x0E4B, 0x0BC4, 0x0EFD, 0x0EB1, 0x2347, 0xEEB3, 0x11A1, 0x0BCE (state ค้างที่ `world_loading`) (2) การ probe ของ L9 ขึ้นเป็น session 1 frame ที่ `peer_closed` ใน trace (3) log ครั้งแรกมี `Failed executing DbCommand ... __ef_migrations_history` ตามด้วย "migrations applied" เป็นพฤติกรรมปกติของ EF |

รอบที่ 2 (15 พ.ย. 21:07–21:10 น.) หลังแก้ `ReadShared()` และย้ายตัวละคร lobby ลง DB: `Start-C9Platform.ps1 -GameData` 45.6 วินาที (build ใหม่ + recreate เฉพาะ c9-init/login/lobby/portal), healthy ใน ~35 วินาที, `Test-C9Platform.ps1` **14/14**; `game.accounts` มี `local` 1 แถว, `game.characters` มี "Lab" slot 0 (map/pos ยังว่าง); log ของ c9-lobby ไม่มี error (รอบนั้น store ยังไม่ log ตอนต่อ DB/import; เพิ่ม event แล้วภายหลัง)

ภาพหน้าจอ: `/mnt/project-files/c9-platform/screenshots/windows-e2e/`

## 11. งานถัดไป (ตามลำดับความสำคัญ)

1. ~~ทดสอบบนเครื่องผู้ใช้~~ เสร็จแล้ว รวม smoke test 14/14 (หัวข้อ 10.1)
1a. (ส่งต่อให้ thread "Make the character move in game") ถอดและเพิ่ม packet ในโลก (0x0BCE, 0xED1D, 0x0BC4 ฯลฯ) ลง `KnownPackets` แล้วเขียน handler ทีละตัวหลังตรวจ reader ใน Ghidra (state ยังค้าง `world_loading`)
2. ถอด username จาก login 0x2F04 (12036) ใน server แล้วส่งใน `SessionInfo` เพื่อให้โมดูล Accounts ผูก session กับบัญชี (R-11)
3. ~~ย้าย `CharacterLobby` ไปใช้ DB~~ เสร็จแล้วโดย thread Accounts/World (`src/C9.Server/CharacterStores.cs`); เพิ่ม event `character_store_ready` และ `legacy_import` ใน log ของ lobby แล้ว (ดูได้จาก Loki/Portal หลัง rebuild ครั้งถัดไป)
4. ผู้ใช้ Portal หลายคน (login/password + MFA) ผ่าน `IPortalUserStore` บน DB
5. ถ้ามี UDP ในอนาคต: เพิ่ม transport `udp` ใน `TraceFrame` (field `Transport` มีแล้ว) และ import pcap จาก Wireshark เข้า Portal
6. ยกเลิก WinForms/MAUI server manager และ `C9.Admin` เดิมหลังผู้ใช้ยืนยันว่า Portal แทนได้ครบ
