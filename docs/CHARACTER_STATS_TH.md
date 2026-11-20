# Base Status ต่ออาชีพ และ Status Upgrade Point (Current State)

อัปเดต: 20 พฤศจิกายน 2026 ~20:45 น. (Asia/Bangkok) — **เฟส 1–2 deploy แล้ว (13:06 UTC โดยเธรด NPC); เฟส 3 (stat เพิ่มตาม level) commit แล้ว รอ deploy**
มาตรฐานอ้างอิง: ISO/IEC 29110 (บันทึกงาน/ส่งต่อ), ISO 9001 (ทวนสอบตรวจย้อนได้), ISO/IEC 27001 (สิทธิ์และ audit)
เกี่ยวข้อง: [GAME_DATA_TH.md](GAME_DATA_TH.md), [LAUNCHER_ACCOUNTS_TH.md](LAUNCHER_ACCOUNTS_TH.md), [GAME_PORTAL_MODULES_TH.md](GAME_PORTAL_MODULES_TH.md)

## 1. ความต้องการและการตัดสินใจของผู้ใช้ (19 พ.ย. 2026)

| # | คำถาม | คำตอบผู้ใช้ |
|---|---|---|
| 1 | status ไหนอัปได้ | **ทุก status** ไม่ใช่แค่ 4 ตัวหลัก (ตีความ: ทุกค่าที่เกมมี และตั้งได้ว่า 1 point เพิ่มเท่าไร/เปิดปิดรายตัว) |
| 2 | ค่า base | **แก้ได้ต่ออาชีพ** (ค่าเริ่มต้นตอนนี้เป็น placeholder ยังไม่ใช่ค่าต้นฉบับของเกม) |
| 3 | point ต่อ level | ใช่: ค่าเริ่มต้น 5 point ทุก level 1–95, แก้รายตัวได้ |
| 4 | เวลาที่มีผล | ตามที่เสนอ: มีผลตอนเข้าโลกครั้งถัดไป |
| 5 (20 พ.ย.) | ใช้ stat เพิ่มตาม level แบบเกมต้นฉบับด้วยไหม | **ใช้ร่วมกับ point** |

กติกา: **status = base (level 1) + เพิ่มต่อ level × (level − 1) + point ที่ใช้ × ค่าต่อ point**; point ที่ได้ = ผลรวมของตารางตั้งแต่ level 1 ถึง level ปัจจุบัน − point ที่ใช้ไปแล้ว (ใช้แล้วคืนไม่ได้ ยังไม่มีระบบรีเซ็ต)
ถ้าผู้ดูแลลดตารางจนได้ point น้อยกว่าที่ใช้ไป ระบบไม่ลบ point เดิม แต่ผู้เล่นจะมี point เหลือ 0 จน level ถึง

## 2. ข้อมูลอ้างอิงจากเกม

- Status หลัก 4 ตัว: Strength (力量), Intelligence (智力), Health (体力), Wisdom (智慧) — จากข้อความ "X is increased."
  ใน `ENG_GameData.c9t` ของ Ref Server 2014 และรายการ item ของ GM tool
- รายการ status ย่อย 26 ตัว (Max HP/MP, Physical/Magic Attack/Defense, Critical, Accuracy, Evasion, ความเร็ว, Guard, ธาตุ) มาจากชุดข้อความเดียวกัน
- ค่า base ต้นฉบับต่ออาชีพอยู่ในตาราง `C9DataInfoClass` ภายใน `GameData.c9d` ของ Client (ถอดได้แล้ว ดูข้อ 2.1)
- DB ของ Ref Server มี `Game.UspUpdatePcAddStat` (11 พารามิเตอร์) = stat ถาวรจาก item ไม่ใช่ระบบแจก point (เกมเดิมไม่มีระบบแจก point)
- ~~อาชีพ 0 Fighter, 1 Hunter, 2 Shaman, 3 WitchBlade, 4 Mistic~~ **แก้แล้ว 20 พ.ย.** (ตารางอาชีพจริง ข้อ 2.1): template = key ของคลาส novice
  0 Fighter, 1 Shaman, 2 Valcan, 3 Hunter, 4 WitchBlade → job code 1–8 Fighter, 9–16 Shaman, 17–24 Hunter, 25–32 Valcan, 33–40 WitchBlade
  (Mistic = key 70/71 ไม่อยู่ใน TemplateIds ของ lobby)

### 2.1 แหล่งข้อมูลใหม่จาก Client ปัจจุบัน (20 พ.ย. 2026)

- **`C:\VFUN\C9-GL\C9.pdb`** (166 MB, MSF 7.00) มากับเกม: GUID `b8ec96a9…` age 2 (build ส.ค. 2026) ไม่ตรงกับ C9.exe (`196d4829…` age 22)
  จึงใช้ address ไม่ได้ แต่ **ข้อมูลชนิด (TPI 44 MB) มีชื่อ struct/ฟิลด์** ใช้ยืนยัน layout ได้ (ตัวอ่าน TPI อยู่ใน scratchpad ของ session ยังไม่ย้ายเข้า repo)
- `C9PacketDeclare<PS_CharacterStatus_Ack,3102,1>` = **0x0C1E**: `m_BattlePower` u64 + `m_CharacterStatus` + `m_CharacterAbnormalStatus` (C9CharacterStatus ×2)
  + `m_MagicResistanceCount` + list. `C9CharacterStatus` (ส.ค. = 176 ไบต์; C9.exe พ.ย. = 192 ไบต์ที่ payload +0x0C):
  `+0x00 m_nMaxHP i64, +0x08 m_nMaxMP, +0x0C m_nSTR, +0x10 m_nINT, +0x14 m_nCON, +0x18 m_nWIS, +0x1C m_nHitRate i16, +0x1E m_nEvadeRate,
  +0x20 m_nGlancingArmorRate (Valiance), +0x22 m_nConcentrationRate, +0x24 m_nCRIRate, +0x26 m_nCRIResistanceRate, +0x28 m_nMoveSpeedRate,
  +0x2A m_nAttackSpeedRate, +0x2C/+0x2E m_nFinalMove/AttackSpeedRate` (ตรงกับ speed factor ที่ RE จาก 008CFF30 เดิม จึงเชื่อถือช่วงนี้ได้)
  ต่อด้วย HPR/MPR, ธาตุ (FDD/WDD/EDD/SDD, FDV…), Guard, MagicResistance, Damage (Min/MaxDD ×2, Min/MaxMD), Defence (DDV/MDV) — ใน build พ.ย.
  มี 16 ไบต์เพิ่มที่ยังไม่รู้ตำแหน่ง จึงยังไม่ส่งช่องหลัง +0x2E
- `C9DataInfoClass` (PDB) → record ใน `GameData.c9d`: `m_iCreateLevel, m_iMaxLevel, m_iGuildSkillSlotNum, m_iDefaultSkillPoint, m_iStr, m_iInt,
  m_iCon, m_iWis, m_iStr/Int/Con/WisIncrease, m_fMoveSpeedEfficiency, m_fAttackSpeedEfficiency` พบ 59 คลาส (novice/elite/advanced) ที่ c9d +0xA0000–0xC1000
- ค่า novice (ใช้เป็นค่าเริ่มต้นของแต่ละสาย): Fighter 10/5/9/1 (+4/+4/+5/+2 ต่อ level), Shaman 6/5/5/9 (+4/+3/+3/+5), Valcan 5/1/4/2 (+4/+1/+4/+1),
  Hunter 8/7/7/3 (+4/+5/+4/+2), WitchBlade 8/4/5/8 (+5/+2/+3/+5) — เกมเดิมเพิ่ม stat ตาม level ด้วย (ระบบเรายังไม่ใช้ค่า Increase)

## 3. สิ่งที่ทำแล้ว

| ส่วน | ไฟล์ | รายละเอียด |
|---|---|---|
| DB (EF Core + Npgsql) | `src/C9.GameData/Entities/Stats.cs`, `GameDbContext.cs`, migration `20261119162447_StatUpgradePoints`, `db/migrations/game-data.sql` | ตาราง `game.stat_definitions` (30 status: ชื่อ, อัปได้, ค่าต่อ point), `class_stat_bases` (5 อาชีพ × 30), `stat_point_levels` (level 1–95 = 5), `character_stats` (point ที่ใช้ต่อตัวละคร) |
| Service | `src/C9.GameData/Services/StatService.cs` | `ConfigurationAsync`, `UpdateDefinitionsAsync`, `SetClassBaseAsync`, `SetLevelPointsAsync`, `ForAccountAsync`, `ForCharacterAsync`, `SpendAsync` (advisory lock ต่อตัวละคร กันใช้ point ซ้ำ, audit, idempotent ด้วย RequestId) |
| c9-auth (ผู้เล่น) | `src/C9.AuthService/Program.cs`, `LauncherSessionStore.FindSession`, `src/C9.Contracts/LauncherAuthContracts.cs` | `GET /launcher/v1/characters/stats`, `POST /launcher/v1/characters/{id}/stats` เฉพาะตัวละครของบัญชีที่ login, rate limit 30/นาที, audit operator `launcher:<login>` |
| L9 Launcher | `src/C9.LocalLauncher.Maui/LauncherPage.cs`, `src/C9.ClientProfiles/LauncherAuthClient.cs` | การ์ด "Status ตัวละคร": เลือกตัวละคร, กด +/− ต่อ status, "บันทึก (ใช้ N point)" ส่งครั้งเดียว; retry ใช้ request id เดิม |
| Admin Portal | `src/C9.Portal.World/Pages/CharacterStats.razor`, `WorldModule.cs` | เมนู Game World → "Status ตัวละคร" (`/world/stats`): base ต่ออาชีพ, ชื่อ/อัปได้/ค่าต่อ point, ตาราง point ต่อ level (ปรับ max level, ใส่ค่าเป็นช่วง) ต้องมีสิทธิ์ WorldWrite และใส่เหตุผล |
| Game server | `src/C9.Server/CharacterStores.cs`, `CharacterLobby.cs`, `compose.yaml` | ตอนเลือกตัวละคร lobby โหลดผลรวม status จาก DB แล้วใช้ `max_hp`/`max_mp` ใน 0BC0/0C1C/0C1E/0C20 (HP ต้อง > 0 ไม่งั้นใช้ค่าเดิม 100); `C9_STAT_PROBE=1` ใส่เลขกำกับใน 0x0C1E |

| เฟส 4 (21 พ.ย.) | `CharacterLobby.cs` | ผลภาพ probe ในเกม (20 พ.ย.): build พ.ย. ขยาย FDV/WDV/EDV/SDV เป็น DWORD ทำให้ช่องหลัง +0x3C เลื่อน 8 ไบต์ ยืนยันแล้ว: HP/MP Recovery (+0x3C/+0x3E), ธาตุโจมตี +0x40–0x4C, ธาตุป้องกัน +0x50–0x5C, Max DEF (guard_limit) +0x60, guard_recovery +0x64, MagicResistance +0x68 (แสดงเป็น Judar Exorcism/Inhibition), Physical Atk R/L min +0x70/+0x74 max +0x78/+0x7C, Magical Atk +0x80/+0x84, Physical/Magic Def +0xA8/+0xAC (payload offset). move_speed/attack_speed เขียน rate +0x34/+0x36 และ final factor = 1000 + rate (cap 1.7/1.3). แก้ CP เลขขยะ (0x0C1E +4 = battle power u64) และ EXP เลขขยะ (0BC0 +24 = m_nExp i64) ที่เดิมเขียน Max HP/MP ลงไป |
| เฟส 3 (20 พ.ย.) | `Entities/Stats.cs` (`ClassStatBase.PerLevel`), `StatService.cs` (`SetClassGrowthAsync`, `LevelBonus`), migration `StatClassGrowth`, `LauncherAuthContracts.cs` (`level_bonus`), `LauncherPage.cs`, `CharacterStats.razor` | ค่าเพิ่มต่อ level ต่อสาย × status; ค่าเริ่มต้นจาก novice ของ Client: Fighter +4/+4/+5/+2, Shaman +4/+3/+3/+5, Valcan +4/+1/+4/+1, Hunter +4/+5/+4/+2, WitchBlade +5/+2/+3/+5 (STR/INT/CON/WIS) อื่น 0; Portal แก้ได้ในคอลัมน์ "เพิ่มต่อ level"; Launcher แสดง base + level + point. หมายเหตุ: เกมเดิมใช้ค่าเพิ่มของคลาสปัจจุบัน (elite/advanced ต่างกัน) เรายังใช้ค่าของ novice ทั้งสาย |
| เฟส 2 (20 พ.ย.) | `StatService.cs`, migration `20261120125157_StatClientClassBase`, `CharacterLobby.cs` | ชื่อสายอาชีพถูกต้อง; ค่า STR/INT/CON/WIS ต้นฉบับแทน placeholder 10 (อัปเดตเฉพาะแถวที่ยังเป็น 10); เพิ่ม status `valiance`, `hp_recovery`, `mp_recovery` (รวม 33); 0x0C1E ส่ง MaxHP (i64), MaxMP, STR, INT, CON, WIS, Hit, Evade, Valiance, Concentration, Critical, Critical Resist ตาม C9CharacterStatus |

### 3.1 โหมดหา layout ของหน้าต่าง Status (`C9_STAT_PROBE=1`)

(เฟส 2: ช่วง block A +0x00–+0x2F รู้แล้วจาก PDB probe จึงเหลือเฉพาะ DWORD i ≥ 12 ของ block A และ block B ทั้งหมด) เดิมรู้เฉพาะ HP/MP และ speed factor (+0x2C/+0x2E ของ block A) ใน 0x0C1E (398 ไบต์: header 12 ไบต์, block A 192 ไบต์ที่ +0x0C,
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
| เฟส 3 (20 พ.ย., ต่อจาก deploy รวม `97c7af5`) | GameData **22/22** (197 assertions: growth ×(level−1), แก้ growth ได้, ค่าติดลบถูกปฏิเสธ, audit), C9.Validation **78/78**, Portal **12/12**, MAUI build 0 warning |
| เฟส 2 (20 พ.ย., branch รวม Sprint 1 `55d7205`) | build 0 warning; GameData **22/22** (188), C9.Validation **77/77** (37,539; ตรวจ 0x0C1E MaxHP/MaxMP/STR/WIS จาก DB), Portal **12/12** |

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

1. หลัง deploy รวม: ผู้ใช้ดูหน้าต่าง Status ในเกม ว่า STR/INT/CON(Health)/WIS, Hit, Evade, Critical ตรงกับ base ของสาย + point ที่ใช้ และส่งภาพเลข probe ของช่องที่เหลือ
2. ทำ offset ของช่องหลัง +0x2E (ความเร็ว, HPR/MPR, ธาตุ, Guard, Damage/Defence) จากผล probe แล้วปิด probe
3. ~~ใช้ค่า Increase ต่อ level~~ ทำแล้ว (เฟส 3) — ถ้าต้องการให้ตรงเกมเดิมมากขึ้น อาจแยกค่าเพิ่มต่อ level ตาม job (elite/advanced) ภายหลัง
4. ~~ถอดค่า base ต้นฉบับ~~ เสร็จ; ~~ยืนยันชื่ออาชีพ~~ ได้จากตารางคลาส
5. รวม branch นี้เข้า `p-server` (รอผู้ใช้ตัดสินใจ) แล้วทำชุดถัดไป: Item Catalog, ส่ง item ผ่าน mail/กระเป๋ากลาง, starter items
