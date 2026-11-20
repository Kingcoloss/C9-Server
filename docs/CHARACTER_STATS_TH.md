# Base Status ต่ออาชีพ และ Status Upgrade Point (Current State)

อัปเดต: 20 พฤศจิกายน 2026 (Asia/Bangkok) — **เฟส 1 เสร็จ deploy แล้ว รอผู้ใช้ทดสอบในเกม**
มาตรฐานอ้างอิง: ISO/IEC 29110 (บันทึกงาน/ส่งต่อ), ISO 9001 (ทวนสอบตรวจย้อนได้), ISO/IEC 27001 (สิทธิ์และ audit)
เกี่ยวข้อง: [GAME_DATA_TH.md](GAME_DATA_TH.md), [LAUNCHER_ACCOUNTS_TH.md](LAUNCHER_ACCOUNTS_TH.md), [GAME_PORTAL_MODULES_TH.md](GAME_PORTAL_MODULES_TH.md)

## 1. ความต้องการและการตัดสินใจของผู้ใช้ (19 พ.ย. 2026)

| # | คำถาม | คำตอบผู้ใช้ |
|---|---|---|
| 1 | status ไหนอัปได้ | **ทุก status** ไม่ใช่แค่ 4 ตัวหลัก (ตีความ: ทุกค่าที่เกมมี และตั้งได้ว่า 1 point เพิ่มเท่าไร/เปิดปิดรายตัว) |
| 2 | ค่า base | **แก้ได้ต่ออาชีพ** (ค่าเริ่มต้นตอนนี้เป็น placeholder ยังไม่ใช่ค่าต้นฉบับของเกม) |
| 3 | point ต่อ level | ใช่: ค่าเริ่มต้น 5 point ทุก level 1–95, แก้รายตัวได้ |
| 4 | เวลาที่มีผล | ตามที่เสนอ: มีผลตอนเข้าโลกครั้งถัดไป |

กติกา: point ที่ได้ = ผลรวมของตารางตั้งแต่ level 1 ถึง level ปัจจุบัน − point ที่ใช้ไปแล้ว (ใช้แล้วคืนไม่ได้ ยังไม่มีระบบรีเซ็ต)
ถ้าผู้ดูแลลดตารางจนได้ point น้อยกว่าที่ใช้ไป ระบบไม่ลบ point เดิม แต่ผู้เล่นจะมี point เหลือ 0 จน level ถึง

## 2. ข้อมูลอ้างอิงจากเกม

- Status หลัก 4 ตัว: Strength (力量), Intelligence (智力), Health (体力), Wisdom (智慧) — จากข้อความ "X is increased."
  ใน `ENG_GameData.c9t` ของ Ref Server 2014 และรายการ item ของ GM tool
- รายการ status ย่อย 26 ตัว (Max HP/MP, Physical/Magic Attack/Defense, Critical, Accuracy, Evasion, ความเร็ว, Guard, ธาตุ) มาจากชุดข้อความเดียวกัน
- ค่า base ต้นฉบับต่ออาชีพอยู่ในตาราง `C9DataInfoClass`/`C9DataInfoCharacter` ภายใน `GameData.c9d` (binary แบบ columnar, ยังไม่มีตัวอ่าน)
- DB ของ Ref Server มี `Game.UspUpdatePcAddStat` (11 พารามิเตอร์) = stat ถาวรจาก item ไม่ใช่ระบบแจก point (เกมเดิมไม่มีระบบแจก point)
- อาชีพ (job code 1–40 เป็นชุดละ 8 ตามลำดับ template 0,1,3,2,4): 0 Fighter, 1 Hunter, 2 Shaman, 3 WitchBlade, 4 Mistic
  — Fighter ยืนยันแล้ว อีก 4 ชื่อยึดลำดับ template ของ Client **ยังไม่ยืนยันในเกม**

## 3. สิ่งที่ทำแล้ว

| ส่วน | ไฟล์ | รายละเอียด |
|---|---|---|
| DB (EF Core + Npgsql) | `src/C9.GameData/Entities/Stats.cs`, `GameDbContext.cs`, migration `20261119162447_StatUpgradePoints`, `db/migrations/game-data.sql` | ตาราง `game.stat_definitions` (30 status: ชื่อ, อัปได้, ค่าต่อ point), `class_stat_bases` (5 อาชีพ × 30), `stat_point_levels` (level 1–95 = 5), `character_stats` (point ที่ใช้ต่อตัวละคร) |
| Service | `src/C9.GameData/Services/StatService.cs` | `ConfigurationAsync`, `UpdateDefinitionsAsync`, `SetClassBaseAsync`, `SetLevelPointsAsync`, `ForAccountAsync`, `ForCharacterAsync`, `SpendAsync` (advisory lock ต่อตัวละคร กันใช้ point ซ้ำ, audit, idempotent ด้วย RequestId) |
| c9-auth (ผู้เล่น) | `src/C9.AuthService/Program.cs`, `LauncherSessionStore.FindSession`, `src/C9.Contracts/LauncherAuthContracts.cs` | `GET /launcher/v1/characters/stats`, `POST /launcher/v1/characters/{id}/stats` เฉพาะตัวละครของบัญชีที่ login, rate limit 30/นาที, audit operator `launcher:<login>` |
| L9 Launcher | `src/C9.LocalLauncher.Maui/LauncherPage.cs`, `src/C9.ClientProfiles/LauncherAuthClient.cs` | การ์ด "Status ตัวละคร": เลือกตัวละคร, กด +/− ต่อ status, "บันทึก (ใช้ N point)" ส่งครั้งเดียว; retry ใช้ request id เดิม |
| Admin Portal | `src/C9.Portal.World/Pages/CharacterStats.razor`, `WorldModule.cs` | เมนู Game World → "Status ตัวละคร" (`/world/stats`): base ต่ออาชีพ, ชื่อ/อัปได้/ค่าต่อ point, ตาราง point ต่อ level (ปรับ max level, ใส่ค่าเป็นช่วง) ต้องมีสิทธิ์ WorldWrite และใส่เหตุผล |
| Game server | `src/C9.Server/CharacterStores.cs`, `CharacterLobby.cs`, `compose.yaml` | ตอนเลือกตัวละคร lobby โหลดผลรวม status จาก DB แล้วใช้ `max_hp`/`max_mp` ใน 0BC0/0C1C/0C1E/0C20 (HP ต้อง > 0 ไม่งั้นใช้ค่าเดิม 100); `C9_STAT_PROBE=1` ใส่เลขกำกับใน 0x0C1E |

### 3.1 โหมดหา layout ของหน้าต่าง Status (`C9_STAT_PROBE=1`)

ตอนนี้รู้เฉพาะ HP/MP และ speed factor (+0x2C/+0x2E ของ block A) ใน 0x0C1E (398 ไบต์: header 12 ไบต์, block A 192 ไบต์ที่ +0x0C,
block B 192 ไบต์ที่ +0xCC, WORD count ที่ +0x18C) เมื่อเปิด probe แต่ละ DWORD i ของ block A = **100 + i** และของ block B = **300 + i**
(speed factor ยังเป็น 1000/1000) ผู้ใช้เปิดหน้าต่าง Status ในเกมแล้วแจ้งว่าเลขไหนอยู่ช่องไหน → นำมาทำตาราง key → offset
แล้วส่งค่า status จริงทุกตัว (ยังไม่ได้ทำ จนกว่าจะได้ผล probe)

- offset ของ DWORD i ใน payload: block A = `0x0C + 4i`, block B = `0xCC + 4i`
- ถ้าช่องไหนขึ้น 0 หรือเลขแปลก อาจเป็น WORD/float ต้อง probe รอบสอง
- ปิด probe: deploy ใหม่โดยไม่ตั้ง `C9_STAT_PROBE`

## 4. การทวนสอบ (19–20 พ.ย. 2026, Windows x64 CNNB886)

| รายการ | ผล |
|---|---|
| `dotnet build C9.slnx -c Release` และ MAUI launcher | 0 warning, 0 error |
| `tests/C9.GameData.Validation` (PostgreSQL 18 ใน Docker) | **20/20**, 153 assertions (ใหม่ 3: key/อาชีพ, config+base+ตาราง level, ใช้ point: เกินไม่ได้, ของคนอื่นไม่ได้, retry ไม่ซ้ำ, กดพร้อมกันได้ครั้งเดียว) |
| `tests/C9.Validation` (full + DB integration) | **68/68**, 6,874 assertions (ใหม่: client/session ของ stat API; lobby ส่ง Max HP/MP จาก base อาชีพ + point) |
| `tests/C9.Portal.GameData.Validation` | **11/11**, 109 assertions (หน้า `/world/stats` แสดงผล, ReadOnly ไม่มีฟอร์มแก้, Player ได้ 403) |
| Deploy | image `c9-platform:local` ใหม่, `docker compose --profile gamedata up -d` จาก worktree โดย `C9_STATE_DIR` = state เดิมของ main checkout; ทุก service healthy, migration ลง DB แล้ว, บัญชี 4 และตัวละคร 2 ตัวอยู่ครบ, API ไม่มี token ตอบ 401 |
| ทดสอบในเกม | **ยังไม่ทำ** (ผู้ใช้เป็นคนทดสอบ) |

## 5. ตำแหน่งโค้ดและการรวมงาน (สำคัญสำหรับคนทำต่อ)

- งานนี้อยู่ใน worktree `C:\MyProjects\C9-Server\.claude\worktrees\agitated-zhukovsky-7fe587` branch **`claude/project-thread-wk3q2i`**
  (สร้างจาก `p-server` + commit snapshot ของ uncommitted working tree ของ main checkout เมื่อ 19 พ.ย. 16:20Z = `883ee82`)
  เพราะเครื่องมือของเธรดนี้ห้ามแก้ main checkout โดยตรง
- L9 ตัวใหม่: `...\agitated-zhukovsky-7fe587\artifacts\launcher\win-x64\L9.exe` (คัดลอก `L9.local.json` เดิมมาแล้ว)
- **ความเสี่ยงตอนรวม:** เธรด 2nd password กำลังเพิ่ม migration ของตัวเองใน main checkout จาก snapshot เดียวกัน
  เมื่อรวมให้คง migration ทั้งสองไฟล์ แล้วสร้าง `GameDbContextModelSnapshot.cs` และ `db/migrations/game-data.sql` ใหม่
  (หรือลบ migration `StatUpgradePoints` แล้ว `dotnet ef migrations add StatUpgradePoints` ใหม่บน model ที่รวมแล้ว) ไฟล์ที่ชนแน่:
  `GameDbContext.cs`, `CharacterLobby.cs`, `SessionProcessor.cs` (ของเขา), `compose.yaml`
- ถ้ามีการ deploy จาก main checkout ก่อนรวม ระบบ status จะหายจาก container (ตารางใน DB ยังอยู่)

## 6. งานถัดไป

1. ผู้ใช้ทดสอบ: แจก point ใน L9 → เข้าเกม → ดู Max HP และเปิดหน้าต่าง Status ส่งภาพเลข probe
2. ทำตาราง key → offset ของ 0x0C1E จากผล probe แล้วส่งค่า status จริงทุกตัว (ปิด probe)
3. ถอดค่า base ต้นฉบับต่ออาชีพจาก `GameData.c9d` (ต้องเขียนตัวอ่าน columnar format) แล้วใส่เป็นค่าเริ่มต้น
4. ยืนยันชื่ออาชีพของ template 1–4 ในเกม
5. รวม branch นี้เข้า `p-server` (รอผู้ใช้ตัดสินใจ) แล้วทำชุดถัดไป: Item Catalog, ส่ง item ผ่าน mail/กระเป๋ากลาง, starter items
