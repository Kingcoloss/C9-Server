# ย้าย C9 Server ไปเครื่อง Windows ใหม่

## ดึงโค้ดและ build

ใช้ Git, .NET 10 SDK ตาม `global.json`, WebView2 Runtime และ Python 3.10 ขึ้นไป
สำหรับตัวติดตั้ง Local Launcher ให้ `dotnet` และ `python` เรียกได้จาก PowerShell

```powershell
git clone --branch p-server https://github.com/Kingcoloss/C9-Server.git
Set-Location C9-Server
dotnet build C9.slnx -c Release
dotnet tests/C9.Validation/bin/Release/net10.0/C9.Validation.dll
dotnet workload install maui-windows --source https://api.nuget.org/v3/index.json
.\scripts\Build-C9AdminDesktop.ps1
```

ผล build อยู่ที่ `artifacts/admin-desktop/win-x64` หรือ `artifacts/admin-desktop/win-arm64`
ตามสถาปัตยกรรมเครื่อง เปิด `C9.AdminDesktop.exe` จากโฟลเดอร์นั้นได้โดยตรง
ถ้าจะทดสอบ backend อย่างเดียว ใช้สองคำสั่ง `dotnet build` และ `C9.Validation.dll`
ได้ก่อนติดตั้ง MAUI workload

## สร้าง Release แยก Server และ Client

สคริปต์ `Build-C9Release.ps1` สร้าง Desktop Manager และ game backend ใน `server/`
และสร้าง Local Launcher แบบ single-file ใน `client/` โดยเลือก target และสถาปัตยกรรมได้:

```powershell
.\scripts\Build-C9Release.ps1 -Target All -RuntimeIdentifier win-x64
.\scripts\Build-C9Release.ps1 -Target Server -RuntimeIdentifier win-x64 -Launch
.\scripts\Build-C9Release.ps1 -Target Client -RuntimeIdentifier win-x64 -NoZip
```

ผลอยู่ใน `artifacts/release/<RID>/server/` และ `client/` พร้อม ZIP แยกแต่ละ target
เว้นแต่ใช้ `-NoZip` หากมี release เดิม สคริปต์เก็บไว้ใน `.previous/` ก่อนแทนที่
ส่วนไฟล์ที่ publish ไม่สำเร็จจะค้างใน `.staging-*` เพื่อใช้ตรวจสาเหตุ

Manager ยังต้องเปิดจาก workspace ที่มี `C9.slnx` และ `config/` และใช้ `dotnet.exe`
ในการเปิด backend; ZIP จึงยังไม่ใช่ชุดติดตั้ง server อิสระ สำหรับ Client ให้ใช้
installer ด้านล่างโดยเพิ่ม `--ui-dir artifacts/release/win-x64/client` และเปลี่ยน
`--backend` ให้ชี้ `artifacts/release/win-x64/server/game/C9.Server.dll` หากใช้
server release ชุดนี้ Launcher ต้องติดตั้งข้าง `C9.exe` และจะสร้าง `L9.local.json`
เมื่อยังไม่มีจาก `config/local-launcher.json` ใน workspace ที่ build โปรแกรมนี้
ใช้ `L9.exe --configure --workspace '<C9-Server>' --client-dir '<game folder>'`
เพื่อสร้างและตรวจ config ได้โดยไม่เปิดเกม ดู [คู่มือ Launcher](LOCAL_LAUNCHER_TH.md)

## ย้ายข้อมูล Local

เซฟตัวละครทดสอบ Lab ทั้งสองรุ่นและค่าตั้ง Server Manager อยู่ใน
`config/migration/local-state` หลัง clone ให้รันจากโฟลเดอร์โครงการ
ก่อนเปิดเซิร์ฟเวอร์ครั้งแรก

```powershell
.\scripts\Restore-C9LocalState.ps1
```

สคริปต์คัดลอก snapshot ลง `artifacts/` และเก็บไฟล์ที่มีอยู่แล้วบนเครื่องปลายทาง
เซฟและค่าตั้งที่เปลี่ยนหลังเริ่มเล่นจะบันทึกใน `artifacts/` ตามปกติ
Web Console จะสร้างกุญแจใหม่บนเครื่องปลายทาง

## เตรียมเกมและตัวเปิด Local

ติดตั้งเกมและอัปเดตผ่าน VFUN แยกจากโฟลเดอร์ Git เปิดเกมผ่าน **GAME START**
เพื่อให้ VFUN เตรียม GameGuard แล้วปิดเกมก่อนทดสอบ Local
โปรไฟล์ที่ทดสอบถึง Waterford ใช้ Build 55350 หากตัวเปิดแจ้งว่า hash ของเกมเปลี่ยน
ให้ตรวจ compatibility profile ของแพตช์นั้นก่อน

แก้ตัวแปรสองตัวแรกให้ตรงกับเครื่องใหม่ แล้วรันจากโฟลเดอร์โครงการ

```powershell
$c9Client = 'D:/Games/C9'
$vfunLauncher = 'C:/VFUN/VLauncher/VFUNLauncher.exe'
$c9Dotnet = (Get-Command dotnet.exe).Source
$c9Runtime = if ([Runtime.InteropServices.RuntimeInformation]::OSArchitecture -eq 'Arm64') { 'win-arm64' } else { 'win-x64' }

dotnet publish src/C9.LocalLauncher/C9.LocalLauncher.csproj -c Release -r $c9Runtime --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o artifacts/local-launcher-standalone --source https://api.nuget.org/v3/index.json

$launcherArgs = @(
    '--client-dir', $c9Client,
    '--backend', "artifacts/admin-desktop/$c9Runtime/game/C9.Server.dll",
    '--backend-config', 'config/lab-official-login-fixture.json',
    '--handoff-config', 'config/lab-official-character-lobby.json',
    '--dotnet', $c9Dotnet
)
if (Test-Path -LiteralPath $vfunLauncher -PathType Leaf) {
    $launcherArgs += @('--official-launcher', $vfunLauncher)
}
python re/scripts/install_local_launcher.py @launcherArgs
# เมื่อ preview ผ่าน ให้ติดตั้งตัวเปิดและสร้าง config สำหรับเครื่องใหม่
python re/scripts/install_local_launcher.py @launcherArgs --apply
```

เปิด **C9 Server Manager** กด **Start all** รอทั้งสองบริการขึ้น **Running**
แล้วเปิด `C9LocalLauncher.exe` ในโฟลเดอร์เกม ขั้นตอนนี้ใช้ backend ที่ Manager เปิดไว้
และใช้ค่าที่บันทึกใน Manager

## จุดที่พัฒนาต่อ

- Build 55350 ใช้ 17 ช่อง สร้าง Fighter ชื่อ Lab และเข้า Waterford ได้
  ยืนยัน HP 100/100 บนหน้าจอเกมแล้ว ทดสอบค่า MP ที่แก้แล้วในเกมจริงเป็นขั้นถัดไป
- แอป Windows ทดสอบบน ARM64 แล้ว ใช้ Manager เปิดและติดตามบริการที่พอร์ต 14000/14001
  ค่าเริ่มต้น HP/MP มีผลหลัง Restart all คิวรางวัลและตัวคูณเรตรอระบบเกมนำไปใช้
- โปรไฟล์ปัจจุบันใช้บัญชีทดสอบในเครื่อง การเข้าเกมด้วยหลายบัญชีและ gameplay
  หลังโหลดฉากอยู่ใน [แผนพัฒนา](DEVELOPMENT_PLAN_TH.md)
