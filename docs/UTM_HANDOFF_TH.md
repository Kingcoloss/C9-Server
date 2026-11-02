# ย้ายการพัฒนา C9 Server ไป UTM

วันที่ 2 พฤศจิกายน 2026 · ผู้ใช้เลือกใช้ UTM เป็น environment พัฒนาระยะยาว

ผู้ใช้ทดลอง CrossOver Free Trial แล้วรันเกมไม่ได้ จึงใช้ UTM เป็นเส้นทางพัฒนาและทดสอบหลัก ยังไม่มี error log ที่ยืนยันสาเหตุของ CrossOver และไม่สรุปว่าเกิดจากข้อจำกัดของ Trial การเปิดเกมเพื่อ RE/debug ยังคงรอผู้ใช้อนุมัติ

## สิ่งที่ย้ายผ่าน Git

- `C9.slnx`, `global.json`, `Directory.Build.props`, `NuGet.Config`
- `src/` และ `tests/C9.Validation/` สำหรับ server/runtime และ validation ของ .NET 10
- `tests/fixtures/`, `c9server/data/twisters.json`, Python reference และสคริปต์ RE
- `config/lab-fixture.json`, `config/research.example.json`, เอกสารและรายงาน
- exported `.c`, `.asm.txt` และ `.json` ใน `re/evidence/` ที่ใช้ตรวจสอบย้อนหลัง
- VS Code config ที่ใช้ร่วมกัน เช่น tasks/launch; machine-local config ใช้ชื่อ `*.local.json`

## สิ่งที่ .gitignore แยกออก

| กลุ่ม | ตัวอย่าง |
|---|---|
| Client/archives | `52C9GL/`, `52C9GL*.rar`, `client-data/` |
| Tool installations | `ghidra_<version>/`, tool ZIPs, `installers/` |
| Ghidra working state | `.gpr`, `.rep/`, `.gar`, `re/evidence/ghidra*/projects/` |
| .NET/Visual Studio | `bin/`, `obj/`, `artifacts/`, `.vs/`, `.suo`, `.user`, TestResults |
| Python environment | `.venv/`, caches, bytecode และ coverage output |
| Local config | `config/*.local.json`, local appsettings และ `.env` โดยเก็บ `.env.example` ได้ |
| Runtime/debug data | logs, captures, memory dumps และ downloaded symbols |
| VM data | `.utm/`, `.qcow2`, `.vhdx`, `.vmdk`, `.vdi`, ISO และ `vm-images/` |

การ ignore ไม่ย้ายหรือลบไฟล์จริง และไม่ตัดไฟล์ออกจากการ copy ทั้ง directory ด้วย Finder/Explorer ต้องเลือกสิ่งที่จะ copy แยกจาก Git

ชุด Ghidra ที่ตรวจพบมีขนาดประมาณ 891 MiB ส่วนฐานวิเคราะห์ใน Mac project และ research projects รวมประมาณ 594 MiB ให้เก็บ/ย้ายฐาน Ghidra แยกจาก Git หากต้องการใช้ analysis database เดิมต่อ คงคู่ project `.gpr` และ directory `.rep` ไว้ครบ; exported evidence ใน Git เป็นคนละส่วนกับฐานข้อมูลนี้

## ข้อค้นพบเกี่ยวกับ Git history

การตรวจ object metadata พบ archived Client ที่ยัง reachable จาก history:

| Blob | ขนาด |
|---|---:|
| `52C9GL.part1.rar` | 5,368,709,120 bytes |
| `52C9GL.part2.rar` | 3,826,849,350 bytes |

local Git packfiles รวมประมาณ 8.70 GiB การ revert หรือเพิ่ม `.gitignore` ไม่ได้เอา blobs เหล่านี้ออกจากประวัติ จึงต้องแยกตัดสินใจวิธีย้าย เช่น history ที่สะอาดหรือ export เฉพาะ source ก่อนย้าย repository เต็มชุด ไม่มีการ rewrite history, stage, commit หรือ push จากงานปรับ `.gitignore` รอบนี้

ขณะตรวจครั้งนี้ Git tracked อยู่เพียง `.gitignore` และ `README.md`; source ใหม่ส่วนใหญ่ยัง untracked ต้องนำ source ที่ต้องการเข้า commit ก่อนใช้ Git clone เป็นวิธีย้าย code ทั้งหมด การ clone remote ปัจจุบันอย่างเดียวไม่รับประกันว่าจะมี source ที่ยังไม่ commit

## เริ่มพัฒนาใน VM

ติดตั้ง .NET 10 SDK ให้ตรงกับ `global.json` (baseline SDK 10.0.302 พร้อม roll-forward policy ที่ระบุในไฟล์) ใน VM แล้ว build จาก project root:

```powershell
dotnet build C9.slnx -c Release --disable-build-servers -m:1 -p:UseSharedCompilation=false
dotnet src/C9.Server/bin/Release/net10.0/C9.Server.dll --check-runtime
dotnet tests/C9.Validation/bin/Release/net10.0/C9.Validation.dll --offline
```

เมื่อ environment อนุญาต loopback sockets ให้รัน validation โดยไม่ใส่ `--offline` เพื่อทดสอบ TCP เพิ่ม การรัน server/validation ที่เขียนใหม่ไม่เปิดเกม:

```powershell
dotnet tests/C9.Validation/bin/Release/net10.0/C9.Validation.dll
dotnet src/C9.Server/bin/Release/net10.0/C9.Server.dll --config config/lab-fixture.json
```

ค่าเริ่มต้นของ host ยังเป็น loopback และ response profile เป็นข้อมูลจำลอง ระบบ auth จริง, subsequent key rotation และ Client compatibility ยังต้องทำต่อก่อนใช้งานจริง Runtime มี managed crypto และ embedded codec tables จึงไม่ต้องใช้ Client folder หรือ Python ตอนเริ่ม server

## Client และ RE tools ใน VM

Client directory เป็นข้อมูลเครื่อง ไม่ commit absolute path ลง shared config ตั้งค่า path จริงใน VM ผ่าน `C9_CLIENT_DIR` หรือ `config/research.local.json` โดยใช้ `config/research.example.json` เป็นต้นแบบ เช่นค่าตัวอย่าง `D:/Games/C9` ให้เปลี่ยนเป็นตำแหน่งจริง

หลังตั้ง path ใช้ `python re/scripts/check_client_integrity.py` เปรียบเทียบกับ manifest เดิมก่อนอ้าง addresses/trace ว่าเป็น Client ชุดเดียวกัน หากใช้ Ghidra ให้ตั้ง `--java` และ `--ghidra` ตามเครื่อง VM; ค่า default บางส่วนของสคริปต์ RE เดิมอ้างเครื่อง Mac

ผู้ใช้แจ้งว่าย้าย Client แล้ว แต่ค่า absolute path ใหม่ยังไม่ปรากฏในข้อมูล project/workspace ที่เครื่องมืออ่านได้ จึงยังไม่ได้บันทึก path ใหม่หรือยืนยัน hash ของตำแหน่งใหม่นั้น งาน server port ที่อิง frozen fixtures ทำต่อได้โดยไม่อ่าน Client ต้นฉบับ
