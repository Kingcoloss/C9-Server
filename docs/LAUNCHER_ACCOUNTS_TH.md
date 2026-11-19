# L9 Launcher — Register / Login (Current State)

อัปเดต: 16 พฤศจิกายน 2026 ~17:00 น. (Asia/Bangkok) — **เฟส 1–2 เสร็จ ผู้ใช้ยืนยันในเกมแล้ว** (ยังไม่ commit)
มาตรฐานอ้างอิง: ISO/IEC 29110 (บันทึกงาน/ส่งต่อ), ISO 9001 (การทวนสอบตรวจย้อนได้), ISO/IEC 27001 (ความปลอดภัย)
เกี่ยวข้อง: [LOCAL_LAUNCHER_MAUI_TH.md](LOCAL_LAUNCHER_MAUI_TH.md), [GAME_DATA_TH.md](GAME_DATA_TH.md), [SERVER_PLATFORM_ARCHITECTURE_TH.md](SERVER_PLATFORM_ARCHITECTURE_TH.md)

## 1. เป้าหมายและสถานะ

ผู้ใช้ต้องการให้ L9 มี Register และ Login ก่อนเริ่มเกม แทนการบังคับใช้บัญชี `local`

| เฟส | งาน | Severity | สถานะ |
|---|---|---|---|
| 1 | บริการ `c9-auth` + หน้า Login/Register ใน L9 + ส่ง ticket ให้ C9.exe | กลาง | **เสร็จ ทดสอบแล้ว** (ยังไม่ commit); 16 พ.ย. เอาปุ่ม "Local Lab" ออก: L9 เปิดเกมได้เฉพาะเมื่อ login ผ่าน (ผู้ใช้ขอ) |
| 2 | หา ticket ใน 0x2F04, login server ตรวจ ticket, ผูกบัญชีกับ session, ส่งตัวตนต่อให้ lobby | สูง | **เสร็จ** build 0 warning, validation 65/65 (6,758 assertions), ผู้ใช้ยืนยันในเกม 16 พ.ย. ~16:54 น. |
| 3 | เอกสาร + commit | ต่ำ | เอกสารนี้ |

ผลตอนนี้: ผู้เล่น login ใน L9 ได้จริง (ทดสอบในเกม 16 พ.ย. ด้วยบัญชีใหม่ id 4) ก่อนเฟส 2 ตัวละครยังอยู่บัญชี `local`
หลังเฟส 2: c9-login ไม่รับ 0x2F04 ที่ไม่มี ticket ที่ถูกต้อง และ c9-lobby โหลดตัวละครของบัญชีที่ login

## 2. สถาปัตยกรรม (เฟส 1)

```
L9 (MAUI) --HTTP 127.0.0.1:14890--> c9-auth --EF Core--> c9-db (game.accounts)
   | register / login -> session token (12 ชม., เก็บใน memory ของ L9 เท่านั้น)
   | ticket -> 16 ไบต์สุ่ม ใช้ครั้งเดียว อายุ 2 นาที
   v
C9.exe  args: fromVLauncher::VALOFE:<ticket base64 %-escaped>:c9-gl:<account 13 หลัก>:<login>::127.0.0.1:14000:EN
   v
c9-login (0x2F04) --> POST c9-auth /internal/v1/tickets/consume (Bearer agent.token) -> {login, account_id, handoff_key}
   | ตอบ 0x2EE2 = [account u64][handoff_key u64][1 u32]
   v
C9.exe --> c9-lobby (0x1771 49 ไบต์ = [account u64][handoff_key u64]...)  (client ส่งค่าจาก 0x2EE2 กลับมาเอง)
c9-lobby --> POST c9-auth /internal/v1/handoffs/resolve -> {login, account_id} -> โหลดตัวละครของบัญชีนั้นจาก game DB
```

### 2.1 Layout ที่ยืนยันแล้ว (เฟส 2)

| Packet | Offset | ค่า | หลักฐาน |
|---|---|---|---|
| 0x2F04 (1,092 ไบต์, plain) | +0x000 (512 ไบต์) | UTF-16LE ปิดด้วย NUL: ticket ตาม argument ช่อง 3 (base64 แบบ %-escaped) | trace `login-ced44f-7` 16 พ.ย. 09:17 UTC (trace payload เต็ม) |
| | +0x200 (512 ไบต์) | UTF-16LE: `c9-gl:0000000000004` (ช่อง 4:ช่อง 5) | trace เดียวกัน; บัญชี 4 = บัญชีที่ผู้ใช้สมัคร |
| | +0x400 | UTF-16LE: `c9-gl` | trace เดียวกัน; ชื่อ login (ช่อง 6) **ไม่อยู่ใน packet** |
| 0x2EE2 (20 ไบต์) | 0 / 8 / 16 | account u64 / key u64 / u32 (=1 ตาม fixture เดิม) | reader 00A621F0 (packet+0x0D = payload 0) อ่าน +0x0D,+0x11 / +0x15,+0x19 / +0x1D |
| 0x1771 (49 ไบต์) | 0 / 8 | account / key ที่ client ส่งกลับ | fixture เดิม A=1,B=0 → 1771 ขึ้นต้น `01 00..` ; handler ของ server 2014 มีข้อความ "Login AccountNo 0 error" |
| 0x2EED | 8 / 13 / 17 | world id u32 / IPv4 / port | reader 00A621F0 +0x15 / +0x1A / +0x1E (ตรงกับ fixture) |

**ยืนยันแล้วในเกม (16 พ.ย.):** client ส่ง account/key ของ 0x2EE2 กลับใน 0x1771 ไบต์ 0–15 (ผู้ใช้เข้าเกมด้วยบัญชีตัวเองได้ ไม่เห็นตัวละครของ `local`)

| ไฟล์ | หน้าที่ |
|---|---|
| `src/C9.AuthService/` (ใหม่, อยู่ใน `C9.slnx`) | Minimal API: `POST /launcher/v1/register`, `/login`, `/ticket`, `/logout`, `/internal/v1/tickets/consume`, `GET /healthz` |
| `src/C9.AuthService/LauncherSessionStore.cs` | session/ticket ใน memory เก็บเฉพาะ SHA-256 ของ token; restart = ทุกคน login ใหม่ |
| `src/C9.Contracts/LauncherAuthContracts.cs` | DTO + routes ใช้ร่วม L9/c9-auth |
| `src/C9.ClientProfiles/LauncherAuthClient.cs` | HTTP client ของ L9 + `LaunchIdentity` |
| `src/C9.ClientProfiles/LocalLaunchPlan.cs` | ใส่ ticket/account/login ใน argument ช่อง 3/5/6 (ไม่ login = ค่า lab เดิม) |
| `src/C9.LocalLauncher.Maui/LauncherPage.cs` | การ์ด "บัญชีผู้เล่น" (login/register, ออกจากระบบ, ปุ่ม Local Lab), ช่อง URL เซิร์ฟเวอร์บัญชีในการ์ดเครื่องมือ |
| `src/C9.ClientProfiles/LauncherUserSettings.cs` | เพิ่ม `auth_url`, `last_login` (ไม่เก็บรหัสผ่าน/token) |
| `compose.yaml`, `docker/Dockerfile` | service `c9-auth` (profile `gamedata`, port 127.0.0.1:14890), publish `/c9/app/auth` |
| `tests/C9.Validation/LauncherAccountCases.cs` | 7 cases (args, URL policy, client flow, ticket store, layout 0x2F04/2EE2/1771, handoff store, login+lobby บังคับ login) |
| `src/C9.Server/LauncherAuthority.cs` (ใหม่, เฟส 2) | `LauncherLoginPacket` (parse 0x2F04, สร้าง 0x2EE2, อ่าน 0x1771), `ILauncherAuthority`, `HttpLauncherAuthority` |
| `src/C9.Server/SessionProcessor.cs` | login: ถ้าตั้ง `C9_AUTH_URL` ต้องมี ticket ที่ redeem ได้ ไม่งั้นปิด session; lobby: resolve handoff แล้วสร้าง `CharacterLobby` ต่อ session ตามบัญชี |
| `src/C9.Server/ServerOptions.cs` | `WithLauncherAuthFromEnvironment()` อ่าน `C9_AUTH_URL` + `C9_AUTH_TOKEN_FILE` (runtime เท่านั้น ไม่อ่านจาก JSON) |
| `src/C9.Server/CharacterStores.cs` / `CharacterLobby.cs` | `GameDataCharacterStore` รับ `fixedAccount` (ไม่สร้าง/ไม่ import บัญชี local) |
| `src/C9.AuthService/*` | `CreateHandoff`/`ResolveHandoff` (เก็บแค่ SHA-256, อายุ 12 ชม.), endpoint `/internal/v1/handoffs/resolve`, consume คืน `handoff_key` |
| `compose.yaml`, `Start-C9Platform.ps1`, `.env.example` | `C9_AUTH_URL` (ตั้งอัตโนมัติเมื่อใช้ `-GameData`) + `C9_AUTH_TOKEN_FILE` ให้ c9-login/c9-lobby |

## 3. ความปลอดภัย (ISO/IEC 27001)

- แยกบริการผู้เล่น (`c9-auth`) ออกจาก Admin Portal: ผู้เล่นไม่เข้าถึง endpoint ของผู้ดูแล
- รหัสผ่านเก็บเป็น PBKDF2 ผ่าน `AccountService` เดิม; L9 ล้างช่องรหัสผ่านหลังส่งทุกครั้ง ไม่บันทึกลงดิสก์
- L9 ยอม `http` เฉพาะ loopback; เซิร์ฟเวอร์นอกเครื่องต้อง `https`
- Rate limit 10 ครั้ง/นาที/IP สำหรับ register+login, 30 ครั้ง/นาที สำหรับ ticket
  (ข้อจำกัด: ผ่าน Docker port mapping ทุก client อาจเห็นเป็น IP เดียวกัน)
- ticket ใช้ครั้งเดียว อายุ 2 นาที; endpoint consume ต้องใช้ `agent.token`
- log บันทึก login name/account id/ผลลัพธ์ ไม่มีรหัสผ่านหรือ token (ตรวจแล้วใน smoke test)
- ข้อความ error ของ login ไม่แยก "ไม่มีผู้ใช้" กับ "รหัสผิด"
- เฟส 2: ไม่มี ticket / ticket ใช้ซ้ำ / หมดอายุ / เลขบัญชีไม่ตรง → c9-login ปิดการเชื่อมต่อ (ไม่ตอบ 0x2EE2)
- เฟส 2: lobby ไม่รับ 0x1771 ที่ไม่มี handoff ที่ c9-auth รู้จัก (ป้องกันการข้าม login ผ่าน reconnect 0x2EE8)
- handoff key 64 บิตสุ่ม เก็บแค่ digest; ใช้ซ้ำได้ใน 12 ชม. (client ใช้ค่าเดิมทุกครั้งที่กลับ lobby)
- ยังไม่ทำ: ตรวจ ban ตอนออก ticket/handoff (ตรวจแค่ตอน login), logout ใน L9 ยังไม่ยกเลิก handoff, session เก็บใน memory
- ถ้า c9-auth ล่ม ผู้เล่นเข้าเกมไม่ได้ (fail closed) — ตั้งใจ

## 4. การทวนสอบ (ISO 9001) — 16 พ.ย. 2026 ~06:57 น. บน Windows x64 (CNNB886)

| รายการ | ผล |
|---|---|
| `dotnet build src/C9.AuthService -c Release` | 0 warning, 0 error |
| `tests/C9.Validation` | **62/62 ผ่าน**, 6,691 assertions (รวม 4 cases ใหม่) |
| `scripts/Build-C9Launcher.ps1` | 0 warning, 0 error |
| `docker compose --profile gamedata up -d --build --no-deps c9-auth` | `/healthz` = `ok`, log "game database configured; registration open" |
| Smoke: register `l9smoke02` | 201, account_id 3, session token 43 ตัว |
| Smoke: login + ticket | 200 / 200, ticket base64 24 ตัว (= 16 ไบต์), หมดอายุ +2 นาที |
| L9.exe เปิดแบบไม่ใช่ Admin | หน้าต่างทำงาน (ยังไม่ได้ทดสอบกด login ผ่าน UI ด้วยตา) |

บัญชีทดสอบที่สร้างไว้ใน DB: `l9smoke01` (id 2), `l9smoke02` (id 3) ลบ/แบนได้ทาง Admin Portal

## 5. ข้อควรระวังในการรัน

- **บังคับ login เปิดเฉพาะเมื่อ `C9_AUTH_URL` ถูกตั้ง** (สคริปต์ `-GameData` ตั้งให้) ถ้ารัน `docker compose up` ตรงๆ โดยไม่ตั้ง ค่า login จะกลับเป็น fixture เดิมที่รับทุก 0x2F04
- ถ้า c9-auth restart ทุกคนต้อง login ใน L9 ใหม่ และคนที่อยู่ในเกมจะกลับเข้า lobby ไม่ได้ (handoff อยู่ใน memory)

- ต้องเริ่มด้วย `scripts\docker\Start-C9Platform.ps1 -GameData` (สคริปต์ตั้ง `C9_DB_CONNECTION`)
  ถ้ารัน `docker compose up c9-auth` ตรงๆ จะได้ `/healthz` = `accounts-disabled` และ L9 จะขึ้น "ระบบบัญชียังไม่ได้เปิด"
  (healthcheck ยังขึ้น healthy — ควรปรับให้ล้มในรอบถัดไป)
- image `c9-platform:local` build ใหม่แล้ว แต่ `c9-login`/`c9-lobby`/`c9-portal` อาจยังรัน image เก่าจนกว่าจะ recreate
- ตอนหยุดงาน อาจได้สั่ง recreate `c9-login` ด้วย `C9_TRACE_PAYLOADS` แบบเก็บ payload เต็มเพื่อเตรียม capture ของเฟส 2
  (16 พ.ย. ~16:40 น.: คืนเป็น `redact-credentials` แล้ว ตรวจด้วย docker inspect; trace payload เต็มของ session `login-ced44f-7`/`lobby-c53478-7`
  ยังอยู่ใน `artifacts/docker-state/trace/*/20261116/` มีเฉพาะ ticket ที่ใช้แล้วและหมดอายุ ไม่มีรหัสผ่าน จะถูกลบตาม retention 7 วัน)

## 6. ขั้นตอนเดิมของเฟส 2 (ทำแล้ว 16 พ.ย. ยกเว้นที่ระบุ)

1. ให้ `c9-login` เก็บ payload 0x2F04 เต็ม (ค่าใน `src/C9.Server/Tracing/TraceOptions.cs`) ชั่วคราว
2. ผู้ใช้ login ใน L9 ด้วยบัญชีใหม่ → เปิดเกม 1 รอบ (UAC)
3. หา login name, เลขบัญชี 13 หลัก และ ticket ใน payload 1,092 ไบต์ → บันทึก layout ใน `re/evidence/`
4. แก้ login service: ถอด ticket → เรียก consume → ผูก AccountId กับ session (ตอบ 0x2EE2 ตามบัญชี)
5. ส่งตัวตนต่อไป lobby (ผ่าน 0x2EED/0x1771) แล้วให้ `CharacterLobby` โหลดตัวละครของบัญชีนั้นแทน `local`
6. แจ้ง thread "2nd password" เมื่อผูกบัญชีเสร็จ (เขาจะต่อระบบ 2nd password หลังขั้นนี้)
7. คืน trace เป็น `redact-credentials`, ปรับ healthcheck, commit

## 7. สถานะล่าสุด 16 พ.ย. ~17:00 น. และงานค้าง

1. ✓ capture 0x2F04 (trace payload เต็มชั่วคราว) → layout ตารางข้อ 2.1
2. ✓ โค้ดเฟส 2 + 3 test cases ใหม่ (เขียนบน cloud, build/test บน Windows ผ่าน Remote Control)
3. ✓ restart ด้วย `Start-C9Platform.ps1 -GameData`: ตรวจแล้ว login/lobby ใช้ `--trace-payloads redact-credentials` และมี `C9_AUTH_URL=http://c9-auth:14890`, ทุก container healthy, `/healthz` = `ok`
4. ✓ ผู้ใช้ทดสอบในเกม: login ผ่าน L9 ใช้ได้ และออกจากเกม (Quit Game) ปิดได้ปกติ (fix 0x17D4 ของ thread 2nd password)
   ยังเป็น defect ของงานอื่น (ไม่เกี่ยวกับ login): NPC ไม่แสดง และเดินด้วย keyboard ไม่ได้ — thread "2nd password / exit" ดูอยู่ (ชุดคำตอบ 0x0BC5)
5. ○ ปัญหาที่พบระหว่างทดสอบ (ไม่ได้เกิดจากเฟส 2, มีมาตั้งแต่ 15 พ.ย.): กด "กลับหน้าเลือกตัวละคร" ในเมือง → client ส่ง 0x0EC4 →
   เราตอบ 0x2EEB → client ต่อ login ใหม่ส่ง 0x2EE8 (28 ไบต์) → fixture ตอบ 0x2EE9 ว่าง → เกมค้าง
   (handler 0x2EE9 ของ client เรียก FUN_009bf800(5) + FUN_00d52ab0; ยังไม่รู้ว่าต้องตอบอะไรต่อ) — thread launcher รับไปตรวจต่อ
6. ○ แจ้ง thread "2nd password" ว่าผูกบัญชีแล้ว, commit (เมื่อผู้ใช้สั่ง)

## 8. Current State: ปุ่ม "กลับหน้าเลือกตัวละคร" (RE 16 พ.ย. ~18:00 น., thread launcher เป็นเจ้าของ)

สถานะ: ○ ยังค้าง กำลัง RE รอบ 3 (Ghidra headless บน CNNB886) ยังไม่ได้แก้โค้ด

**ข้อควรระวังเรื่อง build:** export ของเฟส 2 เดิม (`phase2`) เป็น client build b2f23c8d ส่วน image ปัจจุบันคือ
`artifacts/claude-sync/world/analysis-2242` (project world2242, `/C9.exe.img`, SHA256 ed582d13…) ที่อยู่ฟังก์ชันไม่ตรงกัน
ห้ามใช้ address ข้ามกัน. รอบแรก `RecoverC9Functions` สร้างฟังก์ชันผิดไว้ใน world2242 (ยังไม่ลบ ตาม brief ห้ามลบ) อาจทำให้เจอ error "inside another function"

ลำดับที่ยืนยันแล้วใน world2242:
1. ในเมืองกดปุ่ม → client ส่ง 0x0EC4 (ว่าง) หรือ 0x2EEA (16 ไบต์) → server ตอบ 0x2EEB (`SessionProcessor` branch character_lobby/world_loading/world_ready)
2. 0x2EEB → `FUN_008e7ba0`: ตั้ง [this+0x6c]=1, ปิด connection เดิม (`FUN_004bb950`), ต่อ login ใหม่ (`func_0x008ed210`)
3. greeting type 4 ใน mode 0 เมื่อ +0x6c≠0 → สร้าง **0x2EE8** แทน 0x2EE1: ขนาดรวม 0x29 (header 13 + payload 28)
   copy 16 ไบต์จาก [0x017a00c8] (บล็อกเดียวกับที่ 0x1771 ใช้ = account/handoff key จาก 0x2EE2) ส่วนที่เหลือ memset 0
   offset ที่แน่นอนยังไม่ยืนยัน (trace fixture เก่า A=1,B=0 ได้ `00000000 00000000 01000000 00…` → มี "1" ที่ payload[8])
4. server (fixture) ตอบ 0x2EE9 ว่าง → `FUN_00c51640`: reset field และตั้ง [obj+0x238]=7
5. `FUN_00c51ed0` switch ตาม +0x238 (case 1,2,7): state 7 → stage = [[0x017a011c]+0x10] (state 1 ใช้ +0x8),
   เช็ค `func_0x0086cc20()` ก่อน, เรียก stage vf+4 (ต้องคืนค่า ≠0) แล้ว vf+0x18
6. อาการ: หลัง 0x2EE9 client เปิด connection login ค้างไว้เฉย ๆ 120 วินาที (server TimeoutException) ไม่ส่งอะไรเลย = รอบางอย่างจาก server

สมมติฐาน (ยังไม่ยืนยัน):
- H1: หลัง 0x2EE9 server ต้อง push 0x2EED (redirect ไป lobby) ให้ client ต่อ lobby แล้วส่ง 0x1771 เหมือนตอน login ครั้งแรก
- H2: stage เลือกตัวละครตอน enter ส่ง 0x2EEC เอง (ถ้าใช่ ปัญหาจะอยู่ที่ vf+4 คืน 0)

แผนแก้ (ต้องคง mandatory login):
- login fixture path เมื่อมี authority: ตรวจ handoff ใน 0x2EE8 (ลอง layout acct@8+key@16, acct@0+key@8) ผ่าน c9-auth
  `POST /internal/v1/handoffs/resolve`; resolve ไม่ผ่าน → ปิด connection (fail closed). key สุ่ม 64 บิต จึงปลอดภัย
- ตอบตามผล RE รอบ 3 (น่าจะ 0x2EE9 แล้วตามด้วย 0x2EED), เพิ่ม validation case, build/test ผ่าน RC, ให้ผู้ใช้ทดสอบในเกม

ไฟล์ RE: `artifacts/research/20261116/return-to-character/` (`return_flow2.c` = 00C51ED0, 008E7BA0, 004BB950, 008ED950; รอบ 3 = `return_flow3*`)

### 8.1 ผล RE รอบ 3 (16 พ.ย. ~18:30 น.) — สำคัญ
- stage ที่ client เข้าหลัง 0x2EE9 (state 7 → `[[0x017a011c]+0x10]`, vtable 010a2d6c) คือ **หน้าเลือก server** ไม่ใช่หน้าเลือกตัวละคร
  ทางที่ถูกต้องคือ server-select → 0x2EEC → 0x2EED → ต่อ lobby → 0x1771 → หน้าเลือกตัวละคร
- enter `FUN_00c9d860`: ถ้า flag `[[[0x0179e948]+0xd7f4]+0x184]+4` = 0 ส่ง **0x2EF6** (17 ไบต์, payload u32) ตั้ง flag=1 แล้วซ่อนหน้าต่างรอ 0x2EF7;
  ถ้า flag = 1 อยู่แล้ว (กรณีกลับมา) เปิดหน้าต่างรายชื่อ server (+0x74) ทันทีโดยไม่ส่งอะไร
- update tick `FUN_00c9dc90`: ส่ง **0x2EE5** (header อย่างเดียว, flag เข้ารหัส) tick แรกแล้วทุก 5 วินาที **เฉพาะเมื่อ** `[[[0x0179e948]+0xd7f4]+0xe0]+0x90 == 1`
  และ `[conn+0x7c] != 0` และ `func_0x004b58e0()` เป็นจริง → server ต้องตอบ 0x2EE6 (รายชื่อ server). timeout 180 วินาทีใช้เงื่อนไขเดียวกัน
- Enter/คลิก → `FUN_00a366b0` → **0x2EEC** (33 ไบต์): payload +0 = `[0x017a00c8]+8`, +4 = `+0xc`, +8 = world id ที่เลือก, ที่เหลือ 0
  (entry ต้องมี status byte +0x33 ≠ 0 และ ≠ 4)
- อาการ "ค้าง 120 วินาทีไม่ส่งอะไร" แปลว่าเงื่อนไข 0x2EE5 ไม่จริงหลัง reconnect (น่าจะเป็น `+0xe0→+0x90` ที่ handler ของ 0x2EE2 ตั้งตอน login ครั้งแรก แต่เส้นทาง 2EE8/2EE9 ไม่ได้ตั้ง)
- ยังไม่ทราบ: ใครเขียน `+0xe0→+0x90` และ `+0x7c`, handler 0x2EE6/0x2EF7/0x2EE2/0x2EE9 ใน dispatcher `FUN_008e64f0` (ต้องอ่าน asm)
- ไฟล์: `return_flow3.c/.asm.txt` (21 functions), `return_flow3b.*` (00A366B0)

### 8.2 ผล RE รอบ 4 + trace (16 พ.ย. ~22:00 น.)
- **layout 0x2EE8 (ยืนยันจาก handler 0x2EE2 + trace):** handler 0x2EE2 (008e6fb3) เก็บ payload+0 (account u64) ที่ [0x017a00c8]+8 และ payload+8 (key u64) ที่ [0x017a00c8]+0.
  0x2EE8 copy [0x017a00c8]+0..+0xF ตามลำดับ → **payload 2EE8 = [key u64 @0][account u64 @8][0 × 12]**.
  trace fixture (account 1, key 0): `0000000000000000 0100000000000000 000000000000000000000000` ตรงกัน
- trace login ครั้งแรก (ทั้ง account 1 และ 4): 2F04→2EE2, **2EF6(4)→2EF7(`00000000`)**, 2EE5(0)→2EE6(53), 2EEC(20)→2EED(27)
- trace รอบกลับ (09:22 UTC, account 1): 0EC4→2EEB, login ใหม่ greeting 0004 → 2EE8(28) → 2EE9(ว่าง) → เงียบ 120 วินาที
- account 4 ยังไม่เคยทดสอบรอบกลับเลย: รอบล่าสุด 14:44 UTC lobby ปิดที่ 0x0BBF ด้วย "Encoded frame exceeds configured maximum" (ฝั่ง server, งานของ thread world)
  → แก้แล้วราว 14:55 UTC 16 พ.ย. (thread world ยก max_frame ของ lobby เป็น 40960 และ restart) จึงไม่บล็อกการทดสอบรอบกลับ; ต้องดู trace ใหม่ของ account 4 เมื่อ PC ออนไลน์
- dispatcher `FUN_008e64f0` ไม่มี case 0x2EF7 (handler อยู่ที่อื่น); handler 0x2EE9 (008e7171) ไม่อ่าน payload; `[NetMgr+0xe0]` = object login session (this ของ dispatcher)
- flag `[NetMgr+0x184]+4` ถูกตั้งเป็น 1 โดย `FUN_008e6290` (ตัวส่ง 2EF6) และไม่พบจุด reset → รอบกลับ client จึงไม่ส่ง 2EF6 ซ้ำ จึงไม่ได้ 2EF7
- ยังไม่พบ writer ของ +0x90 (เงื่อนไขส่ง 2EE5) และ +0x7c
- **สมมติฐานที่จะทดสอบ (H3):** 0x2EF7 เป็นตัวที่ทำให้ +0x90=1 → รอบกลับไม่ได้ 2EF7 เลยไม่ส่ง 2EE5. แก้โดยหลัง 2EE9 ให้ server ส่ง 2EF7 (`00000000`) และ 2EE6 (รายชื่อ server) ตามทันที
  และเพิ่ม rule `server_list_sent` + 2EF6 → 2EF7 เผื่อ client ส่ง 2EF6
- ข้อสังเกตความปลอดภัย: `trace/login/20261116/ced44f-001.jsonl` frame 0x2F04 เวลา 09:17 UTC มี `redacted:false` (ช่วงเปิด payload เต็มชั่วคราว) ticket นั้นใช้ครั้งเดียว/หมดอายุ 2 นาทีและถูกใช้ไปแล้ว แต่ควรลบหรือ redact (รอผู้ใช้อนุมัติ)
- อัปเดต 19 พ.ย. (จาก thread 2nd password/world): หลังยก lobby max_frame=40960 (16 พ.ย. 14:55Z) account 4 เข้าโลกได้แล้ว
  (`trace/lobby/20261116/933830-001.jsonl` session lobby-933830-3: 0x0BC0 33,734 ไบต์ 14:57:11, world_ready 14:57:30).
  ข้อสังเกต: 14:59:18 client ส่ง 0x0BE8 (ว่าง, flag 1) ใน world_ready แล้ว server ไม่ตอบ ยังไม่รู้ความหมาย (บันทึกใน docs/WORLD_MOVEMENT_TH.md)
  ตอนทดสอบปุ่มกลับ ให้ดูว่ามี 0x0BE8 ก่อน 0x0EC4/0x2EEA หรือไม่
- 19 พ.ย. ~05:10Z: เริ่มแก้บน PC (Remote Control): fixture 2EE8 → [2EE9, 2EF7, 2EE6], rule 2EF6 ใน server_list_sent, ตรวจ handoff 2EE8 ผ่าน c9-auth (fail closed), test cases, build/deploy
- 19 พ.ย. 09:15Z: ยังไม่ได้แก้โค้ด: Remote Control เปิดใน worktree ที่แอปสร้าง (`.claude/worktrees/*`, มีแค่ README) และ PreToolUse:Edit hook ห้ามแก้ checkout หลัก p-server;
  ExitWorktree ใช้ไม่ได้, ย้าย working directory ไม่ได้ → ต้องใช้ session ที่เปิดตรงที่ C:\MyProjects\C9-Server แบบไม่ใช้ worktree (รอผู้ใช้เลือก)
  แผนแก้พร้อมแล้ว (ดูข้อ 8.2): overlay profile ใช้แทน base ทั้งไฟล์จึงต้องแก้ทั้งสอง, ServerOptions รับได้ ≤3 replies ต่อ rule, handoff ใช้ซ้ำได้,
  trace redaction เป็นแบบทั้ง packet (KnownPackets.Sensitive) → จะเพิ่ม 0x2EE2/0x2EE8; 0x1771/0x17D4 ต้องทำ byte-range ภายหลัง
- 19 พ.ย. 09:35Z: Remote Control เปิด session ใหม่ไม่ได้ → ให้ผู้ใช้เปิด session ใน Code tab ที่ C:\MyProjects\C9-Server (ปิด worktree) แล้ววาง brief
  `/mnt/project-files/c9-launcher/return-fix-brief-20261119.md` (แผน 7 ข้อ, ไฟล์ที่อนุมัติ, line ending, ข้อห้าม). รอผลจาก session นั้น

### 8.3 แก้โค้ด 19 พ.ย. ~10:30Z (แก้ผ่าน folder access ของแอป ไม่ใช่ Remote Control; ผู้ใช้อนุมัติ 05:16Z/09:09Z)
สถานะ: ✱ แก้โค้ดแล้ว รอ build/validation/deploy บน Windows แล้วรอผู้ใช้ทดสอบในเกม. ไม่ได้ commit
- `src/C9.Server/LauncherAuthority.cs`: `ReturnRequestBytes = 28`, `TryReadReturnIdentity` (key u64 @0, account i64 @8, ยาวต้อง 28, account > 0, key ≠ 0)
- `src/C9.Server/SessionProcessor.cs` (fixture path หลัง branch 0x2F04): เมื่อมี c9-auth และ state connected + 0x2EE8 → resolve handoff;
  ผ่าน → ส่ง replies ของ rule (2EE9, 2EF7, 2EE6) + event `launcher_return` + ผูก account; ไม่ผ่าน → event `launcher_return_rejected` แล้วปิด connection (fail closed)
- `config/lab-official-login-fixture.json` และ overlay `artifacts/docker-state/portal/profiles/lab-official-login-fixture.json`
  (overlay ใช้แทน base ทั้งไฟล์; สำรองเดิมไว้ที่ `.bak-20261119`): rule connected/12008 ตอบ [12009 flag1 ว่าง, 12023 flag0 `00000000`, 12006 flag0 รายชื่อ server]
  และเพิ่ม rule server_list_sent/12022 → 12023
- `src/C9.Contracts/KnownPackets.cs`: 0x2EE2 และ 0x2EE8 เป็น Sensitive → trace โหมด redact-credentials ไม่เก็บ payload/wire (key เป็น bearer credential ของ lobby)
- Tests: `LauncherReturnPacketLayout`, `LauncherReturnToCharacterSelect` (LauncherAccountCases.cs, ลงทะเบียนใน Program.cs), และ AgentCases.TracePayloadPolicy ตรวจ redaction ของ 2EE2/2EE8
- ความเสี่ยงที่เหลือ: ยังไม่รู้ writer ของ flag +0x90 ฝั่ง client (ถ้า client ยังค้าง ให้ดูว่าได้ 2EF7/2EE6 แล้วหรือยังใน trace);
  key ใน 0x1771/0x17D4 ยังไม่ซ่อนใน trace (ต้องทำ redaction แบบช่วงไบต์); `trace/login/20261116/ced44f-001.jsonl` มี 0x2F04 `redacted:false` รอผู้ใช้ตัดสินใจ;
  0x0BE8 ใน world_ready ยังไม่รู้ความหมาย
