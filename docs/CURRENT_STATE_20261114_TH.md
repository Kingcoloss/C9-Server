# ผลตรวจ Current State หลังย้ายจาก UTM ไป Windows

> **อัปเดตล่าสุด:** สถานะวันที่ 15 พ.ย. 2026 (เข้าเกมด้วย Client รุ่นใหม่ได้แล้ว และ roadmap ใหม่) อยู่ที่ [CURRENT_STATE_20261115_TH.md](CURRENT_STATE_20261115_TH.md)

หมายเหตุ: รายงานนี้เป็น snapshot ก่อนเริ่มแก้ implementation ต่อในวันที่ 14 พฤศจิกายน
ภายหลังเพิ่ม `Build-C9Release.ps1` และระบบสร้าง/ตรวจ config ของ Client Launcher แล้ว
สถานะการใช้งานล่าสุดอยู่ใน `docs/LOCAL_LAUNCHER_TH.md` และ `docs/WINDOWS_MIGRATION_TH.md`

ตรวจวันที่ 14 พฤศจิกายน 2026 (Asia/Bangkok) ที่ `C:\MyProjects\C9-Server`
ขอบเขต: ตรวจ Git, โครงสร้างและโค้ดปัจจุบัน, งานจาก Harness ใน `app/`,
สถานะการย้ายข้อมูลและ Launcher, build และ validation โดยไม่เปิดเกมจริง

## ข้อสรุปสำหรับเริ่มงานต่อ

ฐานพัฒนาหลักยังเป็น C#/.NET 10 ใน `src/` และ build ได้บน Windows x64 เครื่องนี้
โค้ดเดิมไม่ได้ถูก Harness รอบล่าสุดแก้หรือแทนที่ งานใน `app/` เป็นต้นแบบแยก
ที่ build ได้ แต่ยังไม่ได้เชื่อมกับ protocol server, Admin เดิม หรือข้อมูลตัวละครเดิม
จึงยังไม่นับเป็นความคืบหน้าของการเล่นเกมจริง

สิ่งที่ต้องแก้ก่อนทดสอบเกมบนเครื่องนี้คือการคืนข้อมูล Local และจัดทำ configuration
ของ Local Launcher ให้ตรงกับเครื่องใหม่ แล้วทดสอบ Client เดิมผ่าน backend ใน `src/`
ไม่จำเป็นต้องเริ่มสร้างเกมหรือหน้าจอบริหารใหม่เพื่อแก้ปัญหานี้

## 1. Git และขอบเขตการเปลี่ยนแปลง

| รายการ | ผลตรวจ |
|---|---|
| Branch | `p-server` |
| HEAD | `225083789b99eb55309b3640a2fe7ba1e33a264c` — `update`, 10 พ.ย. 2026 เวลา 00:26:37 +07:00 |
| Remote-tracking ref | HEAD ตรงกับ `origin/p-server` ที่มีอยู่ในเครื่อง; รอบนี้ไม่ได้ fetch จึงไม่ใช่การตรวจ remote ล่าสุด |
| โค้ดเดิม | `git diff HEAD -- src tests config docs re scripts C9.slnx` ว่าง ก่อนสร้างรายงานนี้ |
| Tracked file ที่ถูกแก้มาก่อนตรวจ | `.gitignore` เพิ่มการ ignore `app/**/bin/` และ `app/**/obj/` สองบรรทัด |
| งานใหม่ที่ยังไม่ tracked | `app/`, `capture/README.md`, `checks/`, `craft.json`, `feed.json`, `questions.json`, `transcript.json`, `usage.json`, `inbox/`, `investigation.cany/` |

ไม่มี commit ใหม่ของ Harness ใน branch นี้ เอกสารแผนเดิมก็ยังไม่มี diff
ข้อความในประวัติที่กล่าวว่าแก้แผนหรือเชื่อมโมดูลแล้วจึงต้องตรวจเทียบไฟล์จริง
ไม่ใช้คำตอบของ Harness เป็นหลักฐานการ implement

## 2. ฐานระบบเดิมที่ควรใช้ต่อ

| ส่วน | สภาพที่ตรวจพบ |
|---|---|
| `src/C9.Protocol` | Binary framing, checksum, payload transforms และ RIPEMD160/RC6 สำหรับ initial cipher epoch |
| `src/C9.Server` | TCP host, session processor, fixture login, character lobby, world-entry packet และ initial vitals |
| `src/C9.ClientProfiles` + `src/C9.LocalLauncher` | ตรวจ hash Client และสร้าง launch arguments ตาม compatibility profile |
| `src/C9.Admin.Core` + `src/C9.Admin` | จัดการ start/stop/restart, monitor, พารามิเตอร์ Local และ pending grants ผ่าน Web Console |
| `src/C9.AdminDesktop` | Windows MAUI Blazor Hybrid Manager เดิม; build แยกผ่าน `scripts/Build-C9AdminDesktop.ps1` |
| `tests/C9.Validation` | Validation executable ปัจจุบันมี 48 cases; Offline เลือก 31 และข้าม TCP 17 |
| `c9server/` + `re/` | Python reference, เครื่องมือ RE และหลักฐานเดิม |

`C9.slnx` มี 7 projects รวม validation และ Local Launcher ไม่รวม
`C9.AdminDesktop` ซึ่งมีสคริปต์ build แยก และไม่รวม `app/C9.Continuation`
การ build solution ผ่านจึงไม่ได้ยืนยันว่า Desktop Manager หรือ `app/` ถูก build ด้วย

เอกสารและหลักฐานเดิมระบุว่า Build 55350 เคยสร้าง Fighter ชื่อ Lab และเข้า Waterford
พร้อมเห็น HP 100/100 บน UTM/Windows ARM64 แล้ว ส่วน MP หลังแก้ packet รอตรวจในเกม
รอบนี้ไม่ได้เปิด Client จึงยังไม่ยืนยันผลดังกล่าวซ้ำบนเครื่อง Windows x64 นี้

### ขอบเขตเกมที่ยังไม่เสร็จ

- `src/C9.Server/SessionProcessor.cs:89` เข้าสถานะ `world_loading`
  และส่ง extended vitals เมื่อรับ `0x0BC5` ตามเงื่อนไขที่บรรทัด 92
- `SessionProcessor.cs:107` รับ world requests อื่นโดยยังไม่สร้าง gameplay replies
  ยังไม่พบ state/handler สำหรับโลกที่เล่นต่อได้ครบ, movement, combat หรือ inventory จริง
- ระบบปัจจุบันยังใช้ synthetic local account และ JSON character store
  ยังไม่ใช่ระบบ authentication หลายบัญชีหรือ persistence หลักของเกม
- Admin บันทึกคิวส่งของและ rates ได้ แต่ยังไม่มี reward/inventory handlers นำไปใช้ในเกม
- Cipher หลัง initial epoch 256 transformed frames, multiplayer ข้ามเครื่อง และ UDP
  ที่จำเป็นต่อเกมยังไม่ได้ยืนยันครบ
- Catalog 257 maps / 516 sub-maps เป็นข้อมูลจาก Client เดิมตามแผนปัจจุบัน
  ยังไม่ควรถือว่าเป็นผลสำรวจ content ของ Build 55350 ครบถ้วน

## 3. ตรวจงานจาก Harness ใน `app/`

### งานที่มีอยู่จริง

`app/C9.Continuation.csproj` เป็น WinForms executable, target `net10.0-windows`
และไม่มี `ProjectReference` ไปยัง project ใดใน `src/`
คอมเมนต์ที่กล่าวถึง `src/C9.Admin.Core` จึงเป็นคำอธิบายแนวคิด ไม่ใช่การอ้างอิง type จริง
การค้นชื่อ `C9.Continuation`, `AccountService` และ `RatePolicy` ไม่พบการใช้งานจาก `src/`
หรือชุดทดสอบเดิม

| ไฟล์ | สิ่งที่ implement |
|---|---|
| `Program.cs` | วาดตาราง Rate 4 แถว, animation และบันทึก screenshot หลัง 120 frames |
| `Accounts/AccountService.cs` | บัญชีใน JSON, PBKDF2, roles, suspension, login lockout และ character/inventory แบบง่าย |
| `Economy/RatePolicy.cs` | คำนวณ rates จาก server/event/account |
| `Economy/EventScheduler.cs` | เก็บ events ใน JSON และเลือก events ตามช่วงเวลา |
| `Storage/JsonFiles.cs` | อ่าน JSON และเขียนผ่าน temporary file ก่อน replace |

หน้าจอเรียก `Preview(null)` (`Program.cs:160`) ไม่เลือกบัญชี และเก็บ state
ใต้ `AppContext.BaseDirectory/state` (`Program.cs:28`) ซึ่งเป็นคนละที่กับ
`artifacts/admin-console` และ character store ของ backend เดิม
ไม่มีการเปิด TCP server, เรียก ServerOperations หรือเชื่อม Client จากโปรแกรมนี้
ตรวจภาพ `checks/round-2/screenshot.png` แล้วเป็นตาราง rates เท่านั้น

### จุดที่ทำให้งานออกนอกเป้าหมาย

| ระดับ | หลักฐาน | ผลกระทบ |
|---|---|---|
| สูง | `capture/README.md:3` และ `:76` ระบุ target ที่วิเคราะห์เป็น `PhysX_10.01.29_9.10.0129_SystemSoftware.exe` | เป็นไฟล์ติดตั้ง PhysX; ผลวิเคราะห์นี้ใช้แทนการ RE `C9.exe` ไม่ได้ |
| สูง | `craft.json:9` ระบุ `Rust + macroquad` ทั้งที่ request ในไฟล์เดียวกันต้องการ .NET 10 และใช้เกมเดิม | Metadata ของ Harness ไม่ตรงกับงาน; โค้ด `app/` ที่เกิดขึ้นจริงยังเป็น C# |
| สูง | `craft.json` บันทึก check แบบ `--screenshot` และ `--self-test` สำหรับ move/act/hud ของแอปที่สร้าง | โปรแกรมใหม่ถูกปรับให้ผ่านเกณฑ์หน้าจอของ Harness แต่ไม่ได้เชื่อม original Client; screenshot ไม่พิสูจน์ว่าเล่นเกมได้ |
| สูง | `app` ไม่มี project reference หรือจุดเรียกจาก server | ระบบบัญชีและ Rate ใหม่ยังไม่ถูกนำไปใช้ในการเข้าเกมหรือคำนวณผลเกม |

### ข้อจำกัดที่ต้องแก้ก่อนนำโค้ด `app` กลับมาใช้

1. **Character model ไม่ตรงกับ Client ปัจจุบัน** — `AccountService.cs:148`
   จำกัด 3 slots แต่ official lobby profile ใช้ 17 slots; `CharacterRecord` ไม่มี
   appearance 44 bytes ที่ `CharacterLobby` ใช้สร้างและโหลดตัวละคร
   ต้องออกแบบ migration และใช้โมเดลร่วมก่อนแทน store เดิม
2. **Rate ถูก cap ระหว่างทาง** — `RatePolicy.cs:32` cap ทุกครั้งที่ `Times` และ
   `:107` เรียกต่อสองครั้ง ตัวอย่างจากการอ่านโค้ด: server=100, event=2, account=0.1
   ได้ 10 ทั้งที่ `min(100 × 2 × 0.1, 100)` ได้ 20
   ต้องกำหนด semantics ของ cap ให้ชัดและทดสอบ rates ที่ต่ำกว่า 1 รวม events ซ้อน
3. **Exactly-once มีขอบเขตจำกัด** — `AccountService.cs:317` เก็บเพียง 10,000
   request IDs ล่าสุด การ retry ID ที่ถูกตัดทิ้งแล้วอาจ apply ซ้ำได้
   จึงยังอ้างว่าไม่ส่งซ้ำตลอดอายุข้อมูลไม่ได้
4. **ยังไม่มี integration tests กับ protocol และเซฟเดิม** — validation ของ solution
   ไม่อ้าง project นี้ การ build `app` ผ่านยืนยันการ compile เท่านั้น

โค้ดส่วนบัญชี, events และ atomic JSON write อาจเก็บไว้เป็นต้นแบบได้
แต่ควรเลือกย้ายเข้าบริการร่วมของ `src/` หลังตรวจ schema/transactions
ไม่ยก WinForms dashboard ตัวนี้มาแทน Admin Manager เดิม

## 4. สภาพแวดล้อม Windows และข้อมูลที่ยังย้ายไม่ครบ

| รายการ | ผลตรวจ |
|---|---|
| Runtime | Windows x64; .NET SDK 10.0.404; .NET/ASP.NET Core/WindowsDesktop runtime 10.0.15 |
| SDK policy | `global.json` ขอ 10.0.302 พร้อม `latestFeature`; build ใช้ 10.0.404 ได้ |
| Git | พบ Git ที่ bundled กับ GitHub Desktop แต่คำสั่ง `git` ใน PATH ของ shell ตรวจสอบนี้เรียกไม่ได้; ใช้ absolute path ตรวจแทน |
| Python | พบ Python 3.14.5 ใน pyenv; เรียก absolute path ได้ |
| Client | `C:\VFUN\C9-GL\C9.exe`, ขนาด 6,791,064 bytes |
| Client SHA-256 | `9c6816ab93a6de55e93a9c98cb3837879f0d8e9682e94932d122cf7995845529` ตรงกับ profile Build 55350 ใน `LocalLaunchPlan.cs:10` |
| Local Launcher | พบ `L9.exe`/DLLs แต่ไม่พบ `L9.local.json` และ `C9LocalLauncher.exe` ที่ตำแหน่ง Client |
| Error เดิม | `L9.local.log` ยังบันทึก FileNotFoundException ของ `L9.local.json` วันที่ 10 พ.ย. |
| Character save ที่ runtime ใช้ | ยังไม่พบ `artifacts/official-55350-characters.local.json` |
| Admin settings ที่ runtime ใช้ | ยังไม่พบ `artifacts/admin-console/parameters.local.json` |
| Snapshot สำหรับ restore | มี Lab และ appearance ใน `config/migration/local-state/official-55350-characters.json` พร้อมเซฟ Client เดิมและ `parameters.json` |
| RE location config | ยังไม่พบ `config/research.local.json` |
| Wireshark/Npcap | พบ `dumpcap.exe`; `dumpcap -D` แสดง `\Device\NPF_Loopback` สำเร็จ |
| บริการที่กำลังทำงาน | ไม่พบ TCP endpoints 14000/14001/14880/18080 หรือ processes ของ C9/Launcher/backend ใน snapshot ที่ตรวจ |

ข้อมูล snapshot ที่มีอยู่ทำให้ยังคืน Lab ได้ ไม่ใช่หลักฐานว่าเซฟสูญหาย
รอบนี้ยังไม่ได้รัน restore หรือเขียน config ลงโฟลเดอร์เกม
การพบ capture interface เป็นการตรวจความพร้อมเท่านั้น ยังไม่ได้จับแพ็กเก็ตเกมรอบใหม่

## 5. ผลตรวจที่รันจริงในรอบนี้

| การตรวจ | ผล |
|---|---|
| Build `C9.slnx`, Release | ผ่าน — 0 warnings, 0 errors |
| `C9.Server.dll --check-runtime` | ผ่าน — initial crypto/tables ใช้ได้โดยไม่ต้องอ่าน Client/Python |
| `C9.Validation.dll --offline` | ผ่าน 31/31, 6,099 assertions; ข้าม TCP 17 cases |
| `C9.Validation.dll` แบบเต็ม | ผ่าน 31 cases ก่อนหยุดที่ TCP case แรกด้วย SocketException 10013 ระหว่าง loopback ConnectAsync; cases ถัดไปยังไม่ได้รันจนจบ |
| Python `test_protocol.py` | ผ่าน 10/10 |
| Build `app/C9.Continuation.csproj`, Release | ผ่าน — 0 warnings, 0 errors |
| Build `C9.AdminDesktop.csproj --no-restore` | ไม่ผ่าน NETSDK1004: ยังไม่มี `obj/project.assets.json`; ยังไม่ได้ restore/publish Desktop Manager บนเครื่องนี้ |
| Original Client / Waterford / HUD | ไม่ได้ทดสอบในรอบนี้ |

ข้อจำกัด TCP เป็นผลจากสิทธิ์เครือข่ายของ process ในเซสชันตรวจสอบนี้
ยังแยกไม่ได้จากผลนี้เพียงอย่างเดียวว่า PowerShell ปกติของผู้ใช้จะถูกบล็อกด้วยหรือไม่
จึงไม่เปลี่ยน firewall หรือสรุปว่า protocol server เสีย
การ build MAUI แบบไม่ restore ล้มเพราะ assets หาย ไม่ใช่หลักฐานว่าไม่มี MAUI workload

จำนวน 43 cases ใน README เป็นผลเก่า ไม่ใช่จำนวน suite ปัจจุบัน 48 cases
และผล Windows ARM64 ในเอกสารเก่าไม่ใช้แทนผล TCP บนเครื่องนี้

## 6. ลำดับงานที่แนะนำหลังจบการตรวจ

1. ใช้ `src/` และ `docs/DEVELOPMENT_PLAN_TH.md` เป็นฐานต่อ เก็บ `app/` ไว้เป็น
   ต้นแบบสำหรับเลือกส่วนที่นำกลับมาใช้ ไม่ต้องลบหรือเขียนทับงานเดิม
2. คืน Local state ด้วย `scripts/Restore-C9LocalState.ps1` ก่อนเริ่ม backend ครั้งแรก
   แล้วจัดทำ Launcher/config สำหรับ `C:\VFUN\C9-GL` ให้ชี้ backend และ profiles
   ใน workspace ใหม่นี้ตาม `docs/WINDOWS_MIGRATION_TH.md`
3. สร้าง Manager เดิมสำหรับ win-x64 หรือใช้ Web Console เดิมเป็นทางเริ่มทดสอบ
   โดยต้อง restore dependencies ของ MAUI หากเลือก Desktop
4. รัน TCP validation ใน PowerShell ที่เชื่อม loopback ได้ และเปิด original Client
   ทดสอบ login → 17-slot lobby → Lab → Waterford พร้อมจับ 14000/14001
   แยกผลที่เห็นจริงบน HP/MP, disconnect และ packet sequence
5. ทำ P0 ต่อจาก trace จริง: world initialization/session lifecycle, movement,
   inventory/persistence และ cipher rotation แล้วค่อยเชื่อม accounts/rates/grants
   ผ่านบริการร่วม ให้ Admin และ game handlers ใช้ข้อมูลชุดเดียวกัน
6. หารือและบันทึกขอบเขต host/module ตาม ADR เมื่อเริ่มงาน integration
   ไม่ถือว่าจำเป็นต้อง rewrite ทุก process ก่อนคืน baseline ให้เชื่อมเกมได้

## 7. ตรวจเพิ่มเติม: สคริปต์ PowerShell สำหรับ Release และโครงสร้าง output

ตรวจเพิ่มตามที่ผู้ใช้แจ้ง พบสคริปต์หลัก 7 ไฟล์ใน `scripts/` ซึ่งอยู่ใน Git เดิม
และ `app/shots/verify.ps1` ของ Harness อีก 1 ไฟล์ การค้นรอบนี้รวมไฟล์ที่ถูก ignore แล้ว
ไม่พบสคริปต์ Release ตัวอื่นใน workspace ปัจจุบัน

### โครงสร้าง Release ที่มีอยู่แล้ว

| สคริปต์/ขั้นตอน | Input | Output/ข้อมูลที่เกี่ยวข้อง |
|---|---|---|
| `scripts/Build-C9AdminDesktop.ps1` | `src/C9.AdminDesktop` และ `src/C9.Server` | `artifacts/admin-desktop/<RID>/C9.AdminDesktop.exe` และ dependencies; backend แยกที่ `<RID>/game/C9.Server.dll` |
| `scripts/Start-C9Admin.ps1` | `src/C9.Admin` และ `src/C9.Server` | Web UI ที่ `artifacts/admin-console/app/`; backend ที่ `artifacts/admin-console/game/`; publish เสร็จแล้วเปิด Web Console |
| Shared Local state | Snapshot จาก `config/migration/local-state/` ผ่าน `Restore-C9LocalState.ps1` | เซฟที่ `artifacts/*.local.json`; พารามิเตอร์, grants และ operation logs ที่ `artifacts/admin-console/` |
| Local Launcher ตาม migration guide | `src/C9.LocalLauncher` ผ่านคำสั่ง publish ในเอกสาร | `artifacts/local-launcher-standalone/` แล้วใช้ Python installer สร้าง `C9LocalLauncher.exe`/`L9.local.json` ในโฟลเดอร์เกม |
| `scripts/Start-C9Lab.ps1` | Legacy launcher lab / `src/C9.Server` | `artifacts/server/`, logs และ process record; มี flow ของ `version.dat`, `localinfo.dat` และ legacy login fixture |
| `scripts/Update-C9Handoff.ps1` | Character service ที่บันทึกโดย lab | backend ใหม่ที่ `artifacts/handoff-builds/<timestamp>/` แล้วสลับเฉพาะ handoff process |
| `scripts/Start-C9Capture.ps1` | Npcap loopback | ไฟล์ capture ที่ `artifacts/captures/` |
| `app/shots/verify.ps1` | `app/shots/now.png` | ตรวจ PNG signature/สี/variance ของ screenshot; ไม่มีขั้นตอน publish server |

โครงสร้างของ Desktop และ Web Console สอดคล้องกับโค้ดฝั่ง runtime:
`DesktopWorkspace` โหลด backend จาก `game/C9.Server.dll` ข้าง Manager
และทั้ง Desktop/Web ใช้ Local state ใน `artifacts/admin-console` ชุดเดียวกัน
การเริ่มงานต่อควรใช้ release flow นี้ของ `src/` ได้เลยหลังเตรียม dependencies/config

**ต้องแยก path ให้ชัด:** `artifacts/admin-console/app/` เป็นผล publish ของ
`src/C9.Admin` ส่วน `C9-Server/app/` เป็น source ของ `C9.Continuation`
ที่ Harness สร้างแยกไว้ ข้อสรุปเรื่องงานที่ไม่เชื่อม server ในหัวข้อ 3 หมายถึง path หลัง

### ผลตรวจสคริปต์และ Release เพิ่มเติม

- PowerShell parser ตรวจครบ 8 ไฟล์: ไม่มี syntax errors
- ตรวจลำดับ publish ของ Server และ Web Admin โดยใช้ Release configuration
  และ assets ที่ restore แล้ว (`--no-restore`) ลงโฟลเดอร์ตรวจสอบ
  `artifacts/state-audit/release-review/game/` และ `app/`: ผ่านทั้งสอง projects
- ตรวจพบ DLL, `.deps.json`, `.runtimeconfig.json` ของ backend และไฟล์ Web UI
  `app/wwwroot/console.css` ตามโครงสร้างที่คาดไว้
- รัน `--check-runtime` จาก **published backend**: ผ่าน
- ไม่ได้เรียก Start/Stop/Update scripts ทั้งชุด เพราะมีผลเปิดบริการ แก้ launcher config
  หรือสลับ process; รอบนี้ตรวจโค้ดและขั้นตอน publish โดยไม่เปลี่ยนสภาพ runtime ของเกม
- Desktop Manager ยังไม่ได้ publish สำเร็จบนเครื่องนี้ตามผลตรวจในหัวข้อ 5

### ข้อค้นพบที่ต้องจัดการก่อนใช้ flow ปัจจุบันกับ Build 55350

1. **สูง — Handoff updater เลือก legacy profile แบบตายตัว.**
   `scripts/Update-C9Handoff.ps1:39` ใช้ `config/lab-character-lobby.json`
   ไม่รักษา configuration ที่ส่งให้ `Start-C9Lab.ps1 -HandoffConfig ...`
   หากเริ่มด้วย official profile แล้วเรียก updater นี้ บริการใหม่จะเปลี่ยนกลับไปใช้
   legacy character store, 3 slots, list header 42 bytes และ legacy vitals
   แทน official store, 17 slots, header 47 bytes และ extended vitals
   ต้องรักษา config เดิมหรือรับ parameter ที่ตรวจสอบได้ก่อนใช้ update กับ Client ใหม่นี้

2. **กลาง — Start lab อาจใช้ backend เก่า.**
   `Start-C9Lab.ps1:44` publish เฉพาะเมื่อ `artifacts/server/C9.Server.dll` ยังไม่มี
   การแก้ source แล้วเรียก Start lab อีกครั้งจึงไม่ได้รับประกันว่าจะใช้โค้ดล่าสุด
   ในทางกลับกัน Desktop/Web release scripts publish ทุกครั้ง

3. **ข้อจำกัดของแพ็กเกจ — ยังผูกกับ workspace และ runtime ที่ติดตั้ง.**
   `DesktopWorkspace.Locate()` ใน `src/C9.AdminDesktop/MauiProgram.cs:54`
   เดินขึ้นไปหา `C9.slnx` และยังอ่าน profiles จาก root `config/`
   แม้ UI ตั้ง self-contained ไว้ แต่ backend ถูกเรียกผ่าน `dotnet.exe`
   และ published runtime config ยังต้องใช้ .NET 10 + ASP.NET Core 10
   โครงสร้างปัจจุบันจึงใช้งานภายใน workspace ที่ clone และเตรียมเครื่องแล้ว
   การคัดลอกเฉพาะโฟลเดอร์ Manager ไปตำแหน่งอื่นยังไม่พอสำหรับการแจกติดตั้ง

4. **ข้อจำกัดของ release process — publish/restore/install ยังแยกขั้นตอน.**
   Desktop script ไม่สร้าง Local Launcher, ไม่คืน Local state, ไม่ติดตั้ง config ในเกม
   และไม่รัน validation ให้อัตโนมัติ ต้องทำตาม migration guide เพิ่ม
   output ของ Desktop/Web ถูกเขียนตรงไปยังโฟลเดอร์เดิม ไม่มี staging/version manifest
   หรือ rollback ครบชุด หาก publish ล้มกลางทางต้องตรวจความครบของ output ก่อนใช้

ข้อค้นพบข้างต้นมาจาก code review และการ publish ที่ระบุ ไม่ใช่ผลทดสอบ
การสลับบริการจริงหรือการเปิด Client รอบใหม่ ยังไม่ได้แก้ script ในรอบตรวจนี้

## การเปลี่ยนแปลงจากรอบตรวจนี้

เพิ่มรายงานไฟล์นี้เท่านั้นใน source ที่ผู้ใช้ควร review ไม่มีการแก้ implementation,
แผนเดิม, Client หรือข้อมูลเซฟ และไม่มีการ stage/commit/push
Build/validation สร้าง outputs และ temporary state ในตำแหน่งที่ ignore ตามปกติ

เอกสารอ้างอิงใน repo: `README.md`, `docs/WINDOWS_MIGRATION_TH.md`,
`docs/DEVELOPMENT_PLAN_TH.md`, `docs/ADMIN_CONSOLE_TH.md`,
`docs/adr/0001-server-stack.md`, `docs/PROTOCOL_STATUS.md`,
`re/evidence/static/windows_official_55350_startup.json`
