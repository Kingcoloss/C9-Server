# C9 Local Launcher (MAUI) — Current State

อัปเดต: 26 พฤศจิกายน 2026 (Asia/Bangkok) — เพิ่มหัวข้อ 8 release layout แบบกะทัดรัด + ตัวติดตั้ง
ขอบเขต: Task 1 (UI ของ Client Launcher เป็น MAUI แบบ Responsive) และ Task 2 (กำหนด Game Directory ได้)
มาตรฐานอ้างอิง: ISO/IEC 29110 (บันทึกงาน/ส่งต่อ), ISO 9001 (การทวนสอบที่ตรวจย้อนได้), ISO/IEC 27001 (ความปลอดภัย)

## 1. สถานะสรุป

| หัวข้อ | สถานะ |
|---|---|
| โปรเจกต์ MAUI ใหม่ `src/C9.LocalLauncher.Maui` → `L9.exe` | เสร็จ (build บน Windows x64) |
| UI Responsive (กว้าง ≥ 760px = 2 คอลัมน์, แคบ = 1 คอลัมน์ + scroll) | เสร็จ |
| เลือก/พิมพ์ Game Directory, ตรวจว่ามี `C9.exe`, จำค่าไว้ | เสร็จ |
| โหมด "ทดสอบ Client รุ่นใหม่ (Local)" และจำค่าที่เลือก | เอาออกแล้ว 26 พ.ย. (Client 14 พ.ย. เป็น profile ปกติ ดูหัวข้อ 8.4) |
| ขอสิทธิ์ Administrator (UAC) เองตอนเปิด `C9.exe` | เสร็จ; ทดสอบจริงจนเข้าเมือง Waterford แล้ว (15 พ.ย. 17:01 น.) |
| WinForms L9 เดิม (`src/C9.LocalLauncher`) | คงไว้เป็น legacy และ CLI `--check` / `--configure` |
| Commit เข้า git | ยังไม่ commit (ทั้ง repo มีงานที่ยังไม่ commit จากหลาย session) |

## 2. โครงสร้างโค้ด

| ไฟล์ | หน้าที่ |
|---|---|
| `src/C9.ClientProfiles/LocalLauncherRuntime.cs` | Logic ที่ไม่ผูกกับ UI: ตรวจ config + SHA-256 ของ Client, ตรวจ greeting ของ server 14000/14001, เริ่ม backend, เปิด `C9.exe`, เฝ้าดูการปิดเกม, เขียน `L9.local.log` |
| `src/C9.ClientProfiles/LauncherUserSettings.cs` | ค่าที่ผู้ใช้เลือก (`game_directory`, `compatibility_test`) บันทึกแบบ atomic |
| `src/C9.LocalLauncher.Maui/LauncherPage.cs` | หน้าจอ MAUI (C# UI) และ layout แบบ responsive |
| `src/C9.LocalLauncher.Maui/MauiProgram.cs` | สร้างหน้าต่าง (980×700, ขั้นต่ำ 420×560) และอ่าน argument `--client-dir`, `--workspace`, `--compatibility-test` |
| `scripts/Build-C9Launcher.ps1` | publish ไปที่ `artifacts/launcher/<rid>/L9.exe` (ต้องมี workload `maui-windows`) |
| `tests/C9.Validation/LauncherSettingsCases.cs` | test case "Launcher user settings persist game folder and mode" |

โปรเจกต์ MAUI ไม่ได้ใส่ใน `C9.slnx` เหมือน `C9.AdminDesktop` เพราะต้องใช้ workload เฉพาะ Windows

## 3. พฤติกรรมสำคัญ

- **ลำดับเลือก Game Directory:** ค่าที่ผู้ใช้เลือกไว้ → `C9.exe` ข้าง `L9.exe` → `client_directory` ใน `config/local-launcher.json` → `C:\VFUN\C9-GL`
  ปุ่ม "ค่าเริ่มต้น" ล้างค่าที่เลือกไว้ ส่วน `--client-dir` ใช้ชั่วคราวและไม่ถูกบันทึก
- **ที่เก็บค่า:** `%LOCALAPPDATA%\C9Local\L9\launcher-settings.json` (ไฟล์ของผู้ใช้แต่ละคน ไม่เขียนลงโฟลเดอร์เกม)
  ถ้าไฟล์เสียหรือ path ไม่ใช่ path เต็ม ระบบจะใช้ค่าเริ่มต้นแทนเพื่อให้ Launcher เปิดได้เสมอ
- **`L9.local.json` และ log** ยังอยู่ข้าง `L9.exe` เหมือนเดิม และสร้างอัตโนมัติจาก project defaults
- **สิทธิ์ Admin:** `C9.exe` มี manifest `requireAdministrator` L9 จึงเปิดด้วย ShellExecute verb `runas`
  ซึ่ง Windows จะแสดง UAC ทุกครั้งที่เปิดเกม ถ้ากด No จะขึ้นสถานะ "ยังไม่ได้รับสิทธิ์ผู้ดูแลระบบ" (ERROR_CANCELLED 1223)
  ถ้า L9 ถูกเปิดแบบ Admin อยู่แล้ว จะเปิด `C9.exe` ตรงๆ
- **Client 14 พ.ย. (`b4580ed4…`):** ตั้งแต่ 26 พ.ย. รองรับเป็นปกติ ไม่มีสวิตช์โหมดทดสอบแล้ว (หัวข้อ 8.4)
  Client ที่ hash ไม่รู้จักยังถูกปฏิเสธ ("ยังไม่มี profile ที่รองรับ")

## 4. การทวนสอบ (ISO 9001)

ทวนสอบบนเครื่อง Windows x64 (CNNB886) วันที่ 15 พ.ย. 2026

| รายการ | ผล | หลักฐาน |
|---|---|---|
| `dotnet build src/C9.ClientProfiles -c Release` | ผ่าน, 0 warning, 0 error | build log |
| `tests/C9.Validation` (Release) | ผ่าน 51/51 cases, 6482 assertions | รวม "Launcher user settings persist game folder and mode" และ launcher cases เดิมทั้งหมด |
| `scripts/Build-C9Launcher.ps1` | exit 0, 0 warning, 0 error | `artifacts/launcher/win-x64/L9.exe` |
| เปิด `L9.exe` แบบผู้ใช้ปกติ (ไม่ใช่ Admin) | หน้าต่าง "C9 Local Launcher" 980×700 ทำงานต่อเนื่อง > 8 วินาที | `L9.local.log` 05:51:24 UTC "created and verified local configuration from project defaults" |
| ตรวจอัตโนมัติตอนเปิดในโหมด LOCAL กับ Client `b4580ed4…` | แจ้ง error ตามที่ออกแบบ (ต้องเปิดโหมดทดสอบ) | `L9.local.log` 05:51:24 UTC "The updated Client needs a compatibility profile…" |
| กด "เปิดเกม Local" + UAC จนเข้าเกม (server ใน Docker) | ผ่าน 15 พ.ย. 17:01 น.: L9 ตรวจพบ server 14000/14001 และไม่เปิด server ซ้ำ, สวิตช์โหมดทดสอบจำค่าไว้, UAC → ล็อกอิน สร้างตัวละคร เข้าเมือง Waterford | ภาพหน้าจอใน project files `c9-platform/screenshots/windows-e2e/` |
| ดูหน้าจอแบบ visual ที่ขนาดเริ่มต้น | พบข้อความปุ่ม "ตรวจความพร้อม" ถูกตัด → แก้แล้วในโค้ด (ปุ่มตรวจใช้ความกว้างตามข้อความ, ปุ่มเปิดเกมใช้พื้นที่ที่เหลือ) **ต้อง build ใหม่** | ภาพหน้าจอเดียวกัน |
| ทดสอบย่อหน้าต่างให้เหลือ 1 คอลัมน์ | **ยังไม่ทดสอบ** | — |

การเปลี่ยนแปลงระหว่างทวนสอบ (อนุมัติโดยผู้ใช้ 15 พ.ย. 12:49 น.):
1. `LauncherPage.cs` บรรทัด 18 แก้ชื่อตัวแปรสี `Text` → `TextTone` (compile error CS0103)
2. ลบ `RestoreSources` ออกจาก `C9.LocalLauncher.Maui.csproj`
3. เพิ่ม `src/C9.LocalLauncher.Maui/NuGet.Config` ให้โปรเจกต์นี้ restore package MAUI จาก nuget.org ได้
   เพราะ `NuGet.Config` ที่ root ใช้ packageSourceMapping จำกัดเฉพาะ EF Core/Npgsql (ไม่แก้ไฟล์ root)

## 5. ความปลอดภัย (ISO/IEC 27001)

- ใช้หลัก least privilege: ตัว Launcher และ backend รันด้วยสิทธิ์ผู้ใช้ปกติ ยกระดับสิทธิ์เฉพาะ `C9.exe` ผ่าน UAC
- Endpoint ถูกบังคับให้เป็น IPv4 loopback (`127.0.0.1`) ตาม `LocalLauncherConfiguration.ValidateEndpoint`
- บัญชีและ ticket ใน argument เป็นข้อมูลสมมติของ lab (`test`, ticket ศูนย์ 16 ไบต์) ไม่มีรหัสผ่านหรือ token จริงใน repo/log
- `L9.local.log` บันทึกเฉพาะ pid, hash, exit code และข้อความ error ไม่มี payload
- ไม่แก้ไฟล์ Official (`C9.exe`, `C9Launcher.exe`, `C9Patcher.exe`, `C9Server.ini`, `VerCli.C9`)
- ไฟล์ settings เขียนแบบ temp + move เพื่อไม่ให้ไฟล์ครึ่งๆ กลางๆ

## 6. ความเสี่ยงและงานค้าง

1. การตรวจความพร้อมของ L9 (เปิด TCP อ่าน greeting แล้วปิด) จะเห็นเป็น session 1 frame (`peer_closed`) ใน Packet trace ของ Admin Portal ไม่ใช่บั๊ก
2. Folder picker ของ WinUI ใช้ไม่ได้เมื่อ L9 ถูกเปิดแบบ Administrator ระบบแจ้งให้พิมพ์/วาง path แทน
3. Build script หลัก `Build-C9Release.ps1 -Target Client` ยัง publish WinForms L9 อยู่ ใช้ `Build-C9Launcher.ps1` สำหรับรุ่น MAUI
4. Server กำลังย้ายไปเป็น Web App + Docker Compose (thread อื่น) ถ้า backend ไม่ได้รันผ่าน `dotnet C9.Server.dll` แล้ว
   ต้องปรับ `LocalLauncherRuntime.EnsureBackendAsync` ให้แค่ตรวจ endpoint แทนการเริ่ม process เอง
5. Test "Local launcher configuration bootstrap and preservation" (โค้ดเดิม) flaky บน Linux: `Ensure` ใช้ temp + `File.Move(overwrite:false)`
   ซึ่งบน Windows เป็น atomic แต่บน Linux filesystem ที่ไม่รองรับ hard link .NET จะ fallback แบบ check-then-rename
   จึงได้ Created มากกว่า 1 ครั้งเมื่อรันพร้อมกัน บน Windows ผ่านปกติ (51/51) ยังไม่ได้แก้

## 7. วิธีใช้งาน

```powershell
cd C:\MyProjects\C9-Server
powershell -ExecutionPolicy Bypass -File scripts\Build-C9Launcher.ps1 -Launch
# หรือเปิดไฟล์ที่ build แล้ว
.\artifacts\launcher\win-x64\L9.exe
```

1. ตรวจช่อง "โฟลเดอร์เกม" (ค่าเริ่มต้น `C:\VFUN\C9-GL`) กด "เลือกโฟลเดอร์…" หรือพิมพ์ path แล้วกด "ใช้ path นี้"
2. เข้าสู่ระบบ (หรือสมัครสมาชิก) ในการ์ด "บัญชีผู้เล่น"
3. กด "เปิดเกม Local" แล้วกด Yes ที่หน้าต่าง UAC

เครื่องผู้เล่นทั่วไปใช้ตัวติดตั้ง `L9-Setup-<version>-<rid>.exe` แทน (หัวข้อ 8.3)

## 8. Release layout แบบกะทัดรัด + ตัวติดตั้ง (26 พ.ย. 2026)

ผู้ใช้ขอ (ผ่าน coordinator, 26 พ.ย.): เก็บ MAUI launcher ไว้ แต่ให้โฟลเดอร์ที่ส่งออกเหลือแค่ `L9.exe` + config ด้านบน
ไฟล์ runtime ทั้งหมดย้ายไปโฟลเดอร์ย่อย และมีตัวติดตั้ง Windows
Branch: `claude/project-thread-3dpsvk` (แตกจาก `claude/npc-waterford` `e841266` ซึ่งเป็นโค้ดที่ deploy อยู่)

### 8.1 Layout ใหม่ (`artifacts/launcher/<rid>/`)

```
L9.exe            starter ขนาด ~7 KB (src/C9.LocalLauncher.Package/L9Starter.cs) เปิด bin\L9.exe แล้วจบ
L9.local.json     config เดิม (ไม่ถูกลบตอน build ใหม่)
L9.*.log          log ของ L9 (L9.local.log, L9.server.log, L9.handoff.log)
bin\              MAUI launcher จริง (bin\L9.exe) + .NET/Windows App SDK runtime ทั้งหมด (567 ไฟล์, ~273 MB)
```

| ไฟล์ | การเปลี่ยนแปลง |
|---|---|
| `src/C9.LocalLauncher.Package/L9Starter.cs` (ใหม่) | starter: หา `bin\L9.exe` ข้างตัวเอง เปิดด้วย working dir = โฟลเดอร์บน ส่ง argument ต่อ (quote ตามกฎ Windows) ถ้าไม่พบไฟล์แสดงกล่องข้อความภาษาไทย. compile ด้วย `csc.exe` ของ .NET Framework 4.x ที่มากับ Windows (ไม่ต้องติดตั้งอะไรเพิ่ม) และใช้ icon เดียวกับ L9 |
| `src/C9.ClientProfiles/LocalLauncherConfiguration.cs` | เพิ่ม `LauncherHome(runtimeDir)`: ถ้า runtime อยู่ในโฟลเดอร์ชื่อ `bin` และโฟลเดอร์แม่มี `L9.exe` ให้ใช้โฟลเดอร์แม่เป็นที่เก็บ `L9.local.json`/log/หา `C9.exe` ข้าง L9; ไม่เช่นนั้นใช้โฟลเดอร์ของ runtime เหมือนเดิม |
| `src/C9.LocalLauncher.Maui/MauiProgram.cs` | `LauncherDirectory` = `LauncherHome(AppContext.BaseDirectory)` |
| `scripts/Build-C9Launcher.ps1` | publish ไป `bin\`, ลบไฟล์ runtime ที่ค้างอยู่ด้านบนจาก build แบบเดิม (เฉพาะชื่อที่มีใน `bin\`; config/log ไม่ถูกแตะ), compile starter, สวิตช์ใหม่ `-Installer` |
| `src/C9.LocalLauncher.Package/L9Setup.iss` (ใหม่) | สคริปต์ Inno Setup 6 (ดู 8.3) |
| `tests/C9.Validation/LauncherSettingsCases.cs` | case ใหม่ "Launcher release layout keeps config beside the starter L9.exe" |

ทำไมใช้ starter แทนการให้ `L9.exe` ตัวจริงชี้เข้า `bin\` โดยตรง: ทดลองแก้ apphost ให้โหลด `bin\L9.dll` แล้ว (.NET โหลดได้)
แต่ WinUI 3 แบบ self-contained หา class ผ่าน manifest ของ exe ซึ่งอ้างไฟล์ DLL ข้าง exe จึง crash
`COMException 0x80040111 ClassFactory cannot supply requested class` ที่ `Microsoft.UI.Xaml.Application.Start`
starter จึงเป็นทางที่ง่ายและไม่ต้องแก้ไฟล์ของ Windows App SDK

### 8.2 การทวนสอบ layout (ISO 9001)

| รายการ | ผล |
|---|---|
| `scripts/Build-C9Launcher.ps1` | exit 0; ด้านบนเหลือ `bin`, `L9.exe`, `L9.local.json`, log |
| build ทับโฟลเดอร์ layout เดิม (จำลอง 360 ไฟล์ + 122 โฟลเดอร์ด้านบน) | ด้านบนเหลือ `bin`, `L9.exe`, `L9.local.json`, `L9.local.log` (config/log เดิมยังอยู่) |
| เปิด `L9.exe` ด้านบน | starter จบด้วย exit 0, process `bin\L9.exe` เปิดหน้าต่าง "C9 Local Launcher - Compatibility Test" |
| อ่าน config ด้านบน | สถานะ "พร้อมเปิดเกม local • ตรวจพบ client 2026-11-14 และเซิร์ฟเวอร์ในเครื่อง" (มี `L9.local.json` เฉพาะด้านบน) |
| สมัคร + ออกจากระบบ + ล็อกอิน ผ่าน UI (UI Automation) กับ c9-auth ใน Docker | ผ่าน: บัญชีทดสอบ `l9layout01` (account 5); `L9.local.log` ด้านบนบันทึก register ok / logout / login ok |
| `tests/C9.Validation` (Release) | 83/83 ผ่าน, 37,611 assertions |

หมายเหตุ: ครั้งแรกหลังเปิด L9 (cold start) การตรวจ greeting ของ 14000/14001 มี timeout 700 ms จึงเคยขึ้น
"เซิร์ฟเวอร์ยังไม่เปิด" ทั้งที่ port เปิดอยู่; กด "ตรวจความพร้อม" อีกครั้งแล้วปกติ เป็นพฤติกรรมเดิม ไม่เกี่ยวกับ layout

บัญชีทดสอบ `l9layout01` (account 5) ถูกสร้างใน DB จริงระหว่างทดสอบ ไม่ได้บันทึกรหัสผ่านไว้ใน repo; ลบ/แบนได้ทาง Admin Portal

### 8.3 ตัวติดตั้ง (Inno Setup 6)

เลือก Inno Setup 6 เพราะใช้สคริปต์ไฟล์เดียวได้ setup `.exe` ไฟล์เดียว ติดตั้งแบบรายผู้ใช้ได้โดยไม่ต้องใช้สิทธิ์ Admin
และมี uninstaller + shortcut ในตัว (MSIX ต้องมีใบรับรองสำหรับ sign, WiX ซับซ้อนเกินงานนี้)

- ติดตั้งที่ `%LOCALAPPDATA%\Programs\L9` (`PrivilegesRequired=lowest`) เพราะ L9 เขียน config/log ข้าง `L9.exe`
- shortcut ใน Start Menu "C9 Local Launcher" (desktop icon เป็นตัวเลือก)
- อัปเดตเวอร์ชัน: ลบ `{app}\bin` ทั้งโฟลเดอร์ก่อนลงไฟล์ใหม่ จึงไม่มี DLL เก่าค้าง; `L9.local.json` ที่มีอยู่ไม่ถูกเขียนทับ
- รวม `L9.local.json` ของเครื่องที่ build ถ้ามี (เป็น path ในเครื่อง ไม่มีรหัสผ่าน/token); ไม่รวมไฟล์ `.pdb`
- ถอนการติดตั้ง: ลบโปรแกรมและ `L9.*.log`, เก็บ `L9.local.json` ไว้; ค่าผู้ใช้ใน `%LOCALAPPDATA%\C9Local\L9` ไม่ถูกแตะ

สถานะ: **build และทดสอบแล้ว 26 พ.ย.** (Inno Setup 6 ติดตั้งแบบรายผู้ใช้ผ่าน winget ตามที่ผู้ใช้อนุญาต)

```powershell
winget install JRSoftware.InnoSetup
powershell -ExecutionPolicy Bypass -File scripts\Build-C9Launcher.ps1 -Installer
# ได้ artifacts\launcher\L9-Setup-2.0-win-x64.exe
```

| การทวนสอบตัวติดตั้ง (26 พ.ย.) | ผล |
|---|---|
| `Build-C9Launcher.ps1 -Installer` | exit 0, `artifacts/launcher/L9-Setup-2.0-win-x64.exe` 70.7 MB |
| ติดตั้งเงียบ `/VERYSILENT` | exit 0; `%LOCALAPPDATA%\Programs\L9` มี `L9.exe`, `L9.local.json`, `bin\` (564 ไฟล์, ไม่มี `.pdb`), uninstaller; shortcut Start Menu ชี้ `L9.exe`; มี uninstall entry ใน HKCU (ไม่ต้องใช้ UAC) |
| เปิดจาก shortcut | หน้าต่าง "C9 Local Launcher" badge LOCAL, "พร้อมเปิดเกม local • ตรวจพบ client 2026-11-14 และเซิร์ฟเวอร์ในเครื่อง" |
| ล็อกอินจากตัวที่ติดตั้ง (UI Automation, บัญชี `l9layout01`) | ผ่าน; `L9.local.log` ถูกเขียนข้าง `L9.exe` ในโฟลเดอร์ติดตั้ง |
| ติดตั้งทับ (อัปเกรด) โดยวางไฟล์ `bin\stale-old.dll` ไว้ก่อน | ไฟล์เก่าถูกลบ, `L9.local.json` ที่แก้ไว้ไม่ถูกเขียนทับ, log ยังอยู่ |
| ถอนการติดตั้ง | exit 0; เหลือเฉพาะ `L9.local.json`; shortcut และ uninstall entry หายไป |

หลังทดสอบได้ติดตั้งไว้ในเครื่องผู้ใช้แล้ว 1 ชุด (`%LOCALAPPDATA%\Programs\L9`, config = สำเนาของ `artifacts/launcher/win-x64/L9.local.json`)
และคืนค่า `last_login` ใน `%LOCALAPPDATA%\C9Local\L9\launcher-settings.json` เป็นค่าเดิมของผู้ใช้หลังการทดสอบ

### 8.4 เอาโหมด "Compatibility Test" ออก (26 พ.ย. 2026)

ผู้ใช้ตัดสินใจ (26 พ.ย. 13:33 น.): เอาออก เพราะเกมอัปเดตตามปกติผ่าน VFUN และ argument ไม่น่าเปลี่ยนแล้ว ไม่จำเป็นต้องกั้น
เดิมสวิตช์เป็นแค่ประตูกั้น (argument ที่ส่งให้ `C9.exe` เหมือนกันทุกตัวอักษรทั้งเปิดและปิด)

| ไฟล์ | การเปลี่ยนแปลง |
|---|---|
| `src/C9.ClientProfiles/LocalLaunchPlan.cs` | รับ hash `Observed20261114` เป็นปกติ, ลบ parameter `compatibilityTest` และ property `CompatibilityTest`; `DisplayName` = "2026-11-14"; hash ที่ไม่รู้จักยังถูกปฏิเสธ |
| `src/C9.ClientProfiles/LocalLauncherRuntime.cs` | `CheckAsync`/`StartGameAsync` ไม่รับ flag แล้ว, log ไม่มี `compatibility_test`, ข้อความ error สำหรับ client ที่ไม่รู้จักบอกให้อัปเดต L9 |
| `src/C9.ClientProfiles/LauncherUserSettings.cs` | ลบ `compatibility_test` (ไฟล์ settings เก่าที่มี key นี้ยังโหลดได้ ระบบไม่สนใจ key นี้) |
| `src/C9.LocalLauncher.Maui/*` | ลบการ์ด "โหมดการเปิดเกม" + สวิตช์, badge เป็น LOCAL ตลอด, ชื่อหน้าต่าง "C9 Local Launcher", ลบ argument `--compatibility-test` |
| `src/C9.LocalLauncher/*` (WinForms legacy) | ลบ checkbox และ `--compatibility-test` ให้ใช้ `LocalLaunchPlan` แบบใหม่ได้ (build ผ่าน, ไม่ได้ทดสอบ UI) |
| `tests/C9.Validation` | ปรับ case: Client 14 พ.ย. ผ่านโดยไม่มี flag และได้ argument เดียวกับ 55350, settings เก่าที่มี `compatibility_test` ยังโหลดได้ |

ผล: `tests/C9.Validation` 83/83 ผ่าน (37,609 assertions)

หมายเหตุ: ใน `launcher-settings.json` ของผู้ใช้ยังคง `"compatibility_test": true` ไว้ เพื่อให้ L9 รุ่นเก่าใน `artifacts/launcher/win-x64` (build 16 พ.ย.) ยังเปิดเกมได้ L9 รุ่นใหม่ไม่อ่านค่านี้
เมื่อเลิกใช้รุ่นเก่าแล้วจะลบ key นี้ทิ้งหรือไม่ก็ได้

### 8.5 งานที่ยังค้าง / ข้อเสนอ

1. Branch `claude/project-thread-3dpsvk` ยังไม่ merge เข้า `p-server` (ผู้ใช้ตัดสินใจ)
2. `bin\` ยังใหญ่ (~273 MB, 564 ไฟล์) เพราะ Windows App SDK 1.8 พา DLL ของ AI/ML (DirectML, onnxruntime ฯลฯ) และโฟลเดอร์ภาษา ~120 ภาษามาด้วย ลดขนาดได้ในงานแยก
3. `C9WorkspaceRoot` ถูกฝังตอน build (path ของ checkout ที่ build) ใช้เป็นที่หา project defaults เมื่อยังไม่มี `L9.local.json` เท่านั้น

### 8.6 ไอคอน Launcher (26 พ.ย. 2026)

ผู้ใช้ขอเพิ่มไอคอน (26 พ.ย. 13:40 น.) ออกแบบใหม่เป็นของโปรเจกต์เอง (ไม่ใช้ไอคอนของเกม): พื้นสี่เหลี่ยมมุมมน `#11171F` ขอบ `#2A3648`
วงแหวน + ปุ่มเล่นสีทอง `#E8BE68` (สีเดียวกับหน้าจอ L9)

| ไฟล์ | การเปลี่ยนแปลง |
|---|---|
| `src/C9.LocalLauncher.Maui/Resources/AppIcon/appicon.svg` (ใหม่) | ต้นฉบับไอคอน (456×456) |
| `src/C9.LocalLauncher.Maui/C9.LocalLauncher.Maui.csproj` | `<MauiIcon Include="Resources/AppIcon/appicon.svg" />` MAUI สร้าง `appicon.ico` + PNG หลายขนาดให้ `bin\L9.exe` และหน้าต่าง |
| `scripts/Build-C9Launcher.ps1` | สร้าง `.ico` หลายขนาด (16/24/32/48/256 px, PNG entries) จาก `bin\appiconLogo.targetsize-*.png` แล้วใส่ใน starter `L9.exe` (shortcut และ uninstaller ใช้ไอคอนนี้) |

ทวนสอบ: build + `-Installer` exit 0 (setup 71.1 MB); ไอคอนของ `L9.exe` และ `bin\L9.exe` เป็นไอคอนใหม่;
ติดตั้งทับแล้วเปิดจาก Start Menu หน้าต่าง "C9 Local Launcher" มีไอคอนใหม่ (WM_GETICON)

### 8.7 ภาษาเหลือ TH, EN, CN (26 พ.ย. 2026)

ผู้ใช้ขอ (26 พ.ย. 13:45 น.) ให้ไฟล์ภาษาเหลือแค่ไทย อังกฤษ จีน (ตีความ CN = จีนตัวย่อ zh-Hans/zh-CN)

| ไฟล์ | การเปลี่ยนแปลง |
|---|---|
| `src/C9.LocalLauncher.Maui/C9.LocalLauncher.Maui.csproj` | `SatelliteResourceLanguages` = th;th-TH;en;en-US;en-GB;zh-Hans;zh-CN (ไฟล์ภาษา `Microsoft.Maui.Controls.resources.dll`) + target `C9KeepWinUILanguages` ตัด `.mui` ของ WinUI ที่ไม่ใช่ th-TH/en-US/en-GB/zh-CN ออกจากรายการ publish |
| `scripts/Build-C9Launcher.ps1` | ลบ `bin\` ก่อน publish ทุกครั้ง (เป็น build output ล้วน) ไฟล์ที่เลิกใช้จึงไม่ค้าง |

ผล: โฟลเดอร์ใน `bin\` 121 → 8 (`en-GB`, `en-us`, `th`, `th-TH`, `zh-CN`, `zh-Hans`, `Microsoft.UI.Xaml`, `NpuDetect`), ไฟล์ 581 → 417;
ขนาดลดไม่มาก (~273 → ~270 MB, setup 70.7 MB) เพราะส่วนใหญ่เป็น DLL ของ Windows App SDK ไม่ใช่ไฟล์ภาษา
ทวนสอบ: build/`-Installer` exit 0, เปิด L9 จาก build และจากตัวที่ติดตั้งทับได้ปกติ (Windows UI ภาษา en-US)
ความเสี่ยง: เครื่องที่ตั้ง Windows เป็นภาษาอื่น (เช่น ญี่ปุ่น) จะได้ข้อความของระบบ WinUI (เช่นเมนูคลิกขวา Copy/Paste) จากภาษาสำรองแทน ยังไม่ได้ทดสอบบนเครื่องภาษาอื่น
