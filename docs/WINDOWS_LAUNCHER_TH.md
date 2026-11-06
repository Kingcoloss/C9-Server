# Windows / Launcher debug — 3 พฤศจิกายน 2026

## แพตช์ C9Launcher.exe ให้อ่านไฟล์ในเครื่อง — 5 พฤศจิกายน 2026

แพตช์ launcher ที่ `F:\52C9GL\C9Launcher.exe` ให้ใช้ `version.ini` ข้างเกม
โดยตรง เวอร์ชันที่ติดตั้งคือ 1017 และ manifest ชี้เกมไปที่ `127.0.0.1:14000`
โปรแกรมยังใช้ตัวอ่าน INI และการเปรียบเทียบเวอร์ชันเดิม หากไฟล์หาย
จะเข้าสู่เส้นทางแจ้งข้อผิดพลาดเดิม ฝั่งข่าวของ launcher เปิด `about:blank`
และข้ามการดาวน์โหลด `maintenance.ini` สำหรับ client ในเครื่องชุดนี้

เปิด launcher ได้จากไฟล์ที่แพตช์แล้ว หลังรีสตาร์ตเครื่อง ใช้คำสั่งนี้เพื่อเปิด
บริการเกมในเครื่องและเปิด launcher พร้อมกัน

```powershell
Set-Location 'F:\C9-Server'
& .\scripts\Start-C9Lab.ps1 -OfflineLauncher -GameProfile fixture -OpenLauncher
```

โหมดนี้ตรวจ hash ของ launcher และพอร์ตใน `version.ini` ก่อนเปิดบริการเกม
ที่พอร์ต 14000 และ 14001 โดยไม่เปิด HTTP updater ที่ 18080
ทดสอบบน launcher จริงแล้วว่าแสดง `Patch success complete.` และกด PLAY
จนถึงหน้า Log In ขณะพอร์ต 18080 ไม่มี listener ไฟล์ `version.ini` ยังมี hash เดิม
การเข้าเกมยังต้องผ่านการตอบกลับฝั่ง 14001 ตาม [บันทึก observer](WINDOWS_RESUME_20261104_TH.md)

| จุดแก้ (VA) | ไบต์เดิม → ใหม่ | ผล |
|---|---|---|
| `00408965` | `e8969e0000` → `83c40831c0` | คืน stack arguments แล้วเข้าทางเดิมสำหรับ maintenance ที่ดาวน์โหลดไม่ได้ |
| `0041a9a6` | `7538` → `eb4f` | ไปที่จุดตรวจผล `_access` ของ `version.ini` ก่อนเรียกดาวน์โหลด |
| `0041aa02` | `7554` → `eb54` | ใช้ parser เดิมเมื่อไฟล์มีอยู่ |
| `004769f8` | UTF-16LE URL ข่าวเดิม → `about:blank` | แสดงแผงข่าวว่าง |

สคริปต์ `re/scripts/patch_launcher_offline.py` ตรวจ SHA-256 ทั้งไฟล์ก่อนแก้
และเก็บต้นฉบับใน `artifacts/client-backups/offline-launcher/C9Launcher.exe.original`
ก่อนแทนที่ไฟล์แบบ atomic ทดสอบ apply ซ้ำ คืนต้นฉบับ และ apply กลับแล้ว
launcher เดิมได้ SHA-256 `c3221361367f84721e92bf360d54be7490e3aa443ebce908ee6f3064218439bf`
ส่วนไฟล์แพตช์ได้ `a90bfc010cbf189e3795fb6ed6266c02721165274ebdfa7dad0c93c4b9e38834`

ปิด launcher ก่อนคืนต้นฉบับ แล้วเริ่ม lab โดยละ `-OfflineLauncher`
เพื่อใช้ HTTP updater ในเครื่องกับ launcher ต้นฉบับ

```powershell
python .\re\scripts\patch_launcher_offline.py --client-dir 'F:\52C9GL' --restore
```

## บันทึกก่อนแพตช์ executable — 3 พฤศจิกายน 2026

อัปเดตช่วงบ่าย: หลังเครื่องเริ่มใหม่ เปิดบริการคืนแล้ว แก้ startup ของ Client
และเพิ่ม `[SERVER]` ใน manifest แล้ว ดูสถานะล่าสุดที่
[WINDOWS_CLIENT_STARTUP_TH.md](WINDOWS_CLIENT_STARTUP_TH.md)
รายละเอียดที่กล่าวถึงหน้าต่าง Java และ baseline 36/38 ด้านล่างเป็นผลช่วงเช้า

ผู้ใช้อนุญาตให้ทำ implementation ต่อและเปิดเกมเพื่อทดสอบแล้วในแชตครั้งนี้
จึงยกเลิกเงื่อนไขรออนุมัติเปิดเกมจากรายงาน Mac เดิมสำหรับ session นี้

## Baseline และผลทดสอบ server

- Client: `F:\52C9GL` ตั้งค่าไว้ใน `config/research.local.json` ซึ่ง Git ignore
- `C9.exe` SHA-256: `b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25`
- `C9Launcher.exe` SHA-256: `c3221361367f84721e92bf360d54be7490e3aa443ebce908ee6f3064218439bf`
- ติดตั้ง .NET SDK 10.0.403 ARM64 แบบ user-local ที่ `%USERPROFILE%\.dotnet`
  ตาม [Microsoft dotnet-install](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-install-script)
- Release build ผ่าน ไม่มี warning/error
- .NET full validation ล่าสุด: 36/36 กรณี, 2,836 assertions, ไม่ข้าม TCP
- Python protocol/server unittest: 17/17 กรณีผ่าน
- Client path tests: 7/7 รวม config UTF-8 ที่มีชื่อโฟลเดอร์ภาษาไทย
- `--check-runtime` ผ่านบน Windows

พบปัญหา Git checkout เปลี่ยน newline ท้าย `c9server/data/twisters.json` เป็น CRLF
ทำให้ SHA-256 ไม่ตรงและ runtime ปฏิเสธเริ่ม codec คืน LF แล้วได้ hash เดิม
`5b380265f92ff3b2c900e505d322934ea400658591095f93c1bd45b66e741fc3`
โดยไม่เปลี่ยน expected hash หรือข้าม provenance check เพิ่ม `.gitattributes`
ให้รักษาไฟล์นี้เป็น LF สำหรับ Windows checkout ถัดไป

การทดสอบ truncated TCP frame เดิมเรียก `TcpClient.GetStream()` หลัง half-close
บน Windows จะรายงาน non-connected socket จึงเก็บ stream ก่อน `Shutdown(Send)`
และตรวจ EOF บน stream นั้นต่อ ข้อทดสอบยังตรวจ event `truncated_frame` ตามเดิม

## บริการเวอร์ชันในเครื่อง

`localinfo.dat` เดิมอ้าง `https://c9ol.net/C9Update/EN/version.ini` และไฟล์
`version.ini` ในเครื่องว่าง 0 ไบต์ การลอง endpoint เดิมล้มเหลวที่ SSL;
launcher log เดิมบันทึก WinINet error 12007

เพิ่มโหมด `--launcher-update --client-dir DIR --update-port 18080` ใน C9.Server
เปิดเฉพาะ `127.0.0.1` และอ่านตัวเลขจาก `version.dat` แบบ DWORD little-endian
ใน Client ชุดนี้ได้ 1017 บริการประกาศเวอร์ชันเดียวกับ Client ที่ติดตั้งอยู่
เพื่อเปิดการทดสอบต่อ ไม่มีการดาวน์โหลด patch หรือแก้ไฟล์ executable

สำรองก่อนแก้ไว้ที่ `artifacts/client-backups/20261103-launcher/`
ประกอบด้วย `localinfo.dat`, `version.ini`, `version.dat`, `C9.cfg`
เปลี่ยน URL ใน `localinfo.dat` ให้เรียกบริการในเครื่อง

โหมด HTTP รองรับ GET/HEAD; ปิด cache; path อื่นคืน 404 และ method อื่นคืน 405
ไม่เปิด directory ของ Client เป็น file server

## เปิดและหยุด lab

```powershell
# ใช้ SDK ที่ติดตั้งแบบ user-local หรือ dotnet.exe ใน PATH
& .\scripts\Start-C9Lab.ps1 -OpenLauncher

# ใช้เมื่อยังไม่ได้ตั้ง config/research.local.json
& .\scripts\Start-C9Lab.ps1 -ClientDirectory 'F:\52C9GL' -OpenLauncher

# หยุดเฉพาะสอง service ที่ script บันทึกไว้ ไม่ปิดเกมหรือ launcher
& .\scripts\Stop-C9Lab.ps1
```

Start script publish เมื่อยังไม่มี `artifacts/server/C9.Server.dll`, เปิด
update HTTP และ passive TCP probe, ตรวจ HTTP ก่อนเปลี่ยน Client config,
สำรอง config เมื่อมีการเปลี่ยน และเก็บ logs ที่ `artifacts/logs/`
หากแก้ source แล้วให้หยุด lab และ publish ใหม่ก่อนเริ่มรอบถัดไป
Stop script ตรวจทั้ง PID, เวลาเริ่ม process และ path ของ server ก่อนหยุด

## หลักฐาน launcher จริง

`re/evidence/ghidra/launcher_ini_reader.c` และ `.asm.txt` ผูกกับ SHA-256
ของ launcher ด้านบน ฟังก์ชัน `00413570` ใช้ `wsprintfA("%s%d", prefix, index)`
เริ่ม index ที่ 1 แล้วอ่าน `GetPrivateProfileStringA` จนพบค่าว่าง
ผู้เรียก `0041a800` ส่ง prefix `server`; จึงต้องใช้ `[DOWNLOAD] server1=...`
ไม่ใช่ `server=...` ค่า VERSION และ DOWNLOAD จาก `00407ce0`
เส้นทาง maintenance ใช้ `00408830`: `[MAINTENANCE] IsUsed` และ `Msg`

Import Ghidra ครั้งที่สำเร็จปิดทั้ง Load Libraries และ Ordinal Lookup,
จำกัด 2 CPU และ heap 2 GiB; auto-analysis ถึง deadline 120 วินาที
จากนั้น export ฟังก์ชันที่ต้องการแยกด้วย `-process -noanalysis`
จึงไม่ถือว่า auto-analysis ทั้งโปรแกรมสมบูรณ์

หลังแก้ server1 ทดสอบ launcher จริงแล้ว:

- ดาวน์โหลด `version.ini` ผ่าน HTTP 200
- progress 100 และข้อความ `Patch success complete.`
- หน้าต่าง launcher `v17` แสดงปุ่ม PLAY
- Client files ใน baseline 38 ไฟล์ยังตรงเดิม 36 ไฟล์
  อีกสองไฟล์คือ `localinfo.dat` และ `version.ini` ที่ตั้งใจเปลี่ยนในงานนี้
  ไฟล์ executable/DLL และ `C9.cfg` ไม่เปลี่ยน
  ก่อนแก้ baseline ตรง 37/38: version.ini ถูก launcher เดิมทำให้ว่างอยู่แล้ว
  ขณะที่ manifest จาก Mac เก็บไฟล์ขนาด 5,788 ไบต์
- ตัวตรวจ integrity และตัวอ่าน research.local.json ระบุ UTF-8 ชัดเจนแล้ว
  เพื่อไม่ให้ Windows code page อ่าน manifest ที่มีชื่อไฟล์ภาษาจีนผิด

เริ่ม `C9.exe 64bit` ตาม Start Game.bat แล้ว ได้ process และหน้าต่าง
ข้อความเริ่มต้น `提示` จาก wrapper ยังไม่ยืนยันการเข้า login screen
มีหน้าต่าง Windows Security ของ Java บังการควบคุมหน้าจอ จึงขอให้ผู้ใช้
จัดการหน้าต่างนี้เองก่อนทดสอบเกมต่อ ไม่ได้เปลี่ยน firewall/security settings

การเปิด update endpoint หรือ TCP listener สำเร็จยังไม่ใช่หลักฐานว่าเกม
ล็อกอินหรือเข้าโลกได้ โปรโตคอล auth/server list กับ Client จริงยังต้องตรวจต่อ
