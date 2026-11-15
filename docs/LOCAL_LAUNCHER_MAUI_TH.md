# C9 Local Launcher (MAUI) — Current State

อัปเดต: 15 พฤศจิกายน 2026 (Asia/Bangkok)
ขอบเขต: Task 1 (UI ของ Client Launcher เป็น MAUI แบบ Responsive) และ Task 2 (กำหนด Game Directory ได้)
มาตรฐานอ้างอิง: ISO/IEC 29110 (บันทึกงาน/ส่งต่อ), ISO 9001 (การทวนสอบที่ตรวจย้อนได้), ISO/IEC 27001 (ความปลอดภัย)

## 1. สถานะสรุป

| หัวข้อ | สถานะ |
|---|---|
| โปรเจกต์ MAUI ใหม่ `src/C9.LocalLauncher.Maui` → `L9.exe` | เสร็จ (build บน Windows x64) |
| UI Responsive (กว้าง ≥ 760px = 2 คอลัมน์, แคบ = 1 คอลัมน์ + scroll) | เสร็จ |
| เลือก/พิมพ์ Game Directory, ตรวจว่ามี `C9.exe`, จำค่าไว้ | เสร็จ |
| โหมด "ทดสอบ Client รุ่นใหม่ (Local)" และจำค่าที่เลือก | เสร็จ |
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
- **โหมดปกติ (LOCAL) กับ Client 14 พ.ย.:** การตรวจตอนเปิดจะแจ้งว่ายังไม่มี profile และแนะนำให้เปิดสวิตช์ "ทดสอบ Client รุ่นใหม่ (Local)"
  เป็นพฤติกรรมที่ตั้งใจ (hash `b4580ed4…` รองรับเฉพาะในโหมดทดสอบ)

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
2. ถ้าเป็น Client รุ่น 14 พ.ย. ให้เปิดสวิตช์ "ทดสอบ Client รุ่นใหม่ (Local)"
3. กด "เปิดเกม Local" แล้วกด Yes ที่หน้าต่าง UAC
