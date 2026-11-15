# Design: Game Account Management (3.2) และ Game World Management (3.3)

สถานะ: **Draft สำหรับรีวิว** — 15 พฤศจิกายน 2026 (Asia/Bangkok)
ขอบเขต: งานข้อ 3.2 และ 3.3 ใน [CURRENT_STATE_20261115_TH.md](CURRENT_STATE_20261115_TH.md)
พึ่งพา: สถาปัตยกรรม Web App + Docker Compose ของงานข้อ 3/3.1 (ยังไม่เผยแพร่ ณ วันที่เขียน)
เอกสารนี้จะใช้ API, auth และ layout ของ Portal ตามที่งานข้อ 3 กำหนด ไม่สร้าง web stack แยก

---

## 1. ข้อค้นพบจากการสำรวจ (สิ่งที่มีอยู่จริงตอนนี้)

**ยังไม่มีฐานข้อมูลเกม** ข้อมูลทั้งหมดที่ server เก็บถาวรมีเพียง:

| ข้อมูล | ที่เก็บ | เนื้อหา |
|---|---|---|
| ตัวละคร | JSON ตาม `character_store` (เช่น `artifacts/official-55350-characters.local.json`) | `Slot`, `Job`, `Name`, `Appearance` (44 bytes, base64) เท่านั้น |
| พารามิเตอร์ Admin | `artifacts/admin-console/parameters.local.json` | HP/MP เริ่มต้น, จำนวน connection, timeout, rates 3 ชั้น (Server/Event/Account) |
| คิวส่งของ | `artifacts/admin-console/pending-grants/*.json` | Item/Gold/Exp ต่อ character — **ยังไม่มี handler ในเกมนำไปใช้** |

ข้อจำกัดที่กระทบการออกแบบ (อ้างอิงโค้ด):

- `CharacterLobby.cs:9` — store เป็นของ **บัญชีสังเคราะห์บัญชีเดียว** ไม่มีแนวคิด account เลย
- `CharacterLobby.cs` — Level ถูก hard-code เป็น 1, HP/MP มาจาก config, character Id = `Slot + 1`
  (Id ซ้ำกันได้ทันทีที่มีหลายบัญชี ต้องเปลี่ยนเป็น Id ที่ไม่ซ้ำทั้งระบบ)
- `SessionProcessor.cs` — login ผ่าน fixture rules; lobby รับ `0x1771` (49 bytes) แต่ยังไม่ decode
  ว่าข้อมูลบัญชี/token อยู่ตรงไหน → **ยังแยกผู้เล่นแต่ละบัญชีไม่ได้**
- หลังเข้าโลก (`world_loading`) server แค่ observe packet: **ยังไม่มี inventory, gold, exp, movement,
  chat, combat, drop** ดังนั้นการ "แก้ inventory" หรือ "Economic Dashboard" จะมีข้อมูลจริงก็ต่อเมื่อ
  ระบบเกมเหล่านั้นถูก implement ใน server
- `app/Accounts/AccountService.cs` (ต้นแบบจาก Harness อื่น) มี PBKDF2, roles, suspension, lockout
  ที่ยืมแนวคิดได้ แต่ slot=3, ไม่มี appearance และ rate cap ผิด (ดู CURRENT_STATE_20261114 ข้อ 3)
- ข้อมูลเกมฝั่ง Client มีอยู่แต่ยังไม่ถอดรูปแบบ: `GameData/GameData.cfs` บรรจุ `GameData.c9d` (~68 MB)
  และ `GameData.c9t` (~8.6 MB) ส่วน `GameData/Msg/ENG/ENG.cfs` มี `ENG_GameData.c9t` (~14 MB),
  `ENG_QuestStringTable.DAT`, `ENG_GameStringTable.DAT` ฯลฯ น่าจะเป็นแหล่ง **ชื่อไอเท็ม/มอนสเตอร์/เควสต์**
  (CFS ชุดนี้ใช้ header แบบ 24 bytes ที่ `re/scripts/c9_cfs.py` ยังไม่รองรับ — ตรวจแบบ read-only แล้ว อ่าน index ได้)

**สรุป:** งาน 3.2/3.3 ต้องเริ่มจาก **ชั้น Persistence (PostgreSQL)** ที่ทั้ง game server และ Portal ใช้ร่วมกัน
แล้วค่อยเปิดฟีเจอร์ตามที่ระบบเกมรองรับจริง แบ่งเป็นเฟสด้านล่าง

---

## 2. หลักการออกแบบ

1. **Game server เป็นเจ้าของข้อมูลของตัวละครที่ออนไลน์** — Portal ห้ามเขียน DB ทับตัวละครที่กำลังเล่นตรง ๆ
   (ไม่งั้น server save ทับ) ใช้ **Admin Command** ที่ server รับไป apply แทน; ถ้าออฟไลน์ เขียน DB ได้
   ภายใต้ row lock + `version` (optimistic concurrency)
2. **ทุกการเปลี่ยนแปลงทรัพย์สินผ่าน Ledger** — gold/item/exp ที่เพิ่มหรือลด ต้องมีแถวใน ledger
   พร้อม `reason` (drop, npc_shop, trade, mail, admin_grant, ...) Economic Dashboard คำนวณจาก ledger
   จึงตรวจย้อนกลับ/rollback ได้
3. **Audit ทุกการกระทำของ GM/Admin** — ใคร ทำอะไร กับใคร ก่อน/หลังเป็นอะไร เหตุผล (บังคับกรอก)
4. **Idempotent** — ทุก command มี `request_id` (UUID) เก็บถาวรใน DB (ไม่ใช่ cache 10,000 รายการแบบ `app/`)
5. **RBAC** — role อย่างน้อย `viewer`, `gm`, `admin`, `owner`; การกระทำอันตราย (ลบตัวละคร, rollback,
   แก้ทรัพย์สินจำนวนมาก, bulk) ต้องยืนยันซ้ำ
6. **ตรวจกับ Client จริงก่อนเปิดฟีเจอร์** — ฟีเจอร์ที่ต้องส่ง packet ใหม่ (ประกาศ, kick, เพิ่มของสด)
   ต้องยืนยัน layout ด้วย packet trace (งาน 3.1) ก่อน ไม่เดา reply

---

## 3. Game Account Management (3.2)

### 3.1 ฟีเจอร์หลัก (ตามที่ขอ)

| # | ฟีเจอร์ | รายละเอียด | เฟส |
|---|---|---|---|
| A1 | บัญชีหลายบัญชี | สร้าง/ค้นหา/ดูบัญชี, รหัสผ่าน (PBKDF2/Argon2), สถานะ active/suspended/banned, จำนวน slot (ค่าเริ่ม 17) | 1 |
| A2 | ตัวละครทั้งหมดในบัญชี | รายการ 17 slots: ชื่อ, อาชีพ (Job/Template), level, map ล่าสุด, เวลาเล่นล่าสุด, appearance (decode 44 bytes ให้อ่านง่าย) | 1 |
| A3 | แก้ข้อมูลตัวละคร | เปลี่ยนชื่อ (ตรวจซ้ำ/คำต้องห้าม), ย้าย slot, แก้ appearance, ย้ายตำแหน่ง (map/sub-map/พิกัดจาก `WorldMapCatalog`) | 1 |
| A4 | ลบ/กู้คืนตัวละคร | soft delete (`deleted_at`) + กู้คืนภายใน N วัน (client รองรับสถานะ "pending deletion" ที่ `entry+0x3C`) | 1 |
| A5 | Ban / Suspend | ban ถาวรหรือมีวันหมดอายุ, เหตุผล, ผู้สั่ง; ปลดอัตโนมัติเมื่อหมดเวลา; ตรวจตอน login | 1 |
| A6 | Inventory / Equipment / Warehouse | ดูและแก้ไอเท็ม (เพิ่ม/ลบ/เปลี่ยนจำนวน/enchant/option) | 3 (ต้องมีระบบ inventory ใน server) |
| A7 | Gold / Level / Exp / Stats | ดูและแก้ (ผ่าน ledger) | 2–3 |
| A8 | ส่งของ (Grant) | ต่อยอดคิว `pending-grants` เดิม → ย้ายเข้า DB, ส่งผ่าน mail/อัตโนมัติตอน login | 3 |

### 3.2 ไอเดียเพิ่มเติม (เสนอ)

| # | ไอเดีย | ประโยชน์ | เฟส |
|---|---|---|---|
| A9 | **ประวัติ Login / Session** | เวลาเข้า-ออก, IP, Client hash, ตัวละครที่เลือก, เหตุผลที่หลุด (ผูกกับ trace ของ 3.1 ได้ → กดจาก session ไปดู packet) | 1 |
| A10 | **ผู้เล่นออนไลน์ + Kick** | ดู session ที่ออนไลน์ตอนนี้, state (lobby/world), map, ปุ่ม kick | 1 (kick = ปิด socket ก่อน; packet แจ้งเตือนรอยืนยัน) |
| A11 | **Audit Log & Timeline ต่อบัญชี** | หน้าเดียวเห็นทุกอย่างที่เกิดกับบัญชี: login, GM action, ทรัพย์สินเปลี่ยน | 1 |
| A12 | **โน้ต/แท็กบัญชี** | GM จดบันทึก, แท็ก เช่น `tester`, `vip`, `suspicious` | 1 |
| A13 | **Snapshot & Rollback ตัวละคร** | snapshot อัตโนมัติก่อนทุก GM edit และรายวัน; ย้อนกลับได้ทั้งตัวละคร (เหมาะมากช่วงพัฒนา เพราะ bug ของ server อาจทำของหาย) | 2 |
| A14 | **Export / Import ตัวละคร (JSON)** | สำรอง, ย้ายระหว่างเครื่อง, ใช้แทน `config/migration/local-state` ปัจจุบัน | 1 |
| A15 | **Character Templates สำหรับทดสอบ** | สร้างตัวละครทดสอบ "Lv.X + ชุดอุปกรณ์ Y ที่ map Z" ในคลิกเดียว — ช่วยทดสอบ packet ของ RE | 2 |
| A16 | **GM Flag / โหมดล่องหน** | กำหนดบัญชีเป็น GM, สิทธิ์คำสั่ง in-game (`/item`, `/warp`) | 3 |
| A17 | **ตรวจจับความผิดปกติ** | multi-account จาก IP เดียวกัน, ไอเท็มซ้ำ (item serial ซ้ำ = dupe), gold โตผิดปกติ | 3 |
| A18 | **ย้ายตัวละครข้ามบัญชี / จองชื่อ** | ย้าย character ไปบัญชีอื่น, รายชื่อที่สงวน/ห้ามใช้ | 2 |

### 3.3 การผูกบัญชีกับ Client (ปัญหาที่ต้องแก้ก่อน)

ตอนนี้ไม่รู้ว่าผู้เล่นคนไหนคือบัญชีไหน มี 2 ทางเลือก:

1. **Decode login packet** (`0x1771` 49 bytes และ packet login ฝั่ง 14000) ด้วย trace ของงาน 3.1 เพื่อหา
   account/token — ถูกต้องที่สุด แต่ต้องทำ RE
2. **ผ่าน Launcher (L9)** — ให้ผู้เล่นล็อกอินใน L9 → L9 ขอ one-time token จาก API → ส่ง token ใน launch
   argument ที่ Client ส่งต่อมาใน login packet (ต้องยืนยันว่า field ไหนถูกส่งผ่าน)

แนะนำ: เริ่มด้วยข้อ 2 ถ้า trace ยืนยันว่า argument ไหลไปถึง login packet; ระหว่างนี้ใช้ **บัญชี default บัญชีเดียว**
(เท่ากับสภาพปัจจุบัน) โดย schema รองรับหลายบัญชีตั้งแต่ต้น

---

## 4. Game World Management (3.3)

### 4.1 ฟีเจอร์หลัก (ตามที่ขอ)

| # | ฟีเจอร์ | รายละเอียด | เฟส |
|---|---|---|---|
| W1 | **Event Scheduler** | สร้าง event มีช่วงเวลา (เริ่ม/จบ, ทำซ้ำรายวัน/สัปดาห์), ผลกระทบ: ตัวคูณ Exp/Drop/RareDrop/Upgrade, เปิด map/บอส, ของรางวัล login; ปฏิทินดู event ซ้อนกัน | 1 (ส่วน rates) / 3 (ส่วนที่ต้องมีระบบเกม) |
| W2 | **Rate Policy** | ต่อยอด `ServerRates/EventRates/AccountRates` เดิม; สูตรเดียวชัดเจน `effective = min(server × Πevent × account, cap)` cap ครั้งเดียวตอนท้าย (แก้ bug ของ `app/RatePolicy.cs`); มีหน้า preview | 1 |
| W3 | **Economic Dashboard** | gold ในระบบทั้งหมด, faucet vs sink ต่อวัน (แยก reason), กราฟเงินเฟ้อ, top 20 คนรวย, การกระจายไอเท็มหายาก, อัตรา enchant สำเร็จ | 2 (โครง) / 3 (มีข้อมูลจริงเมื่อมี drop/shop) |

### 4.2 ไอเดียเพิ่มเติม (เสนอ)

| # | ไอเดีย | ประโยชน์ | เฟส |
|---|---|---|---|
| W4 | **ประกาศระบบ / MOTD** | ประกาศทันทีหรือตั้งเวลา, ข้อความหน้า Launcher (ทำได้ทันทีผ่าน L9), ข้อความในเกม (ต้องยืนยัน chat/notice packet) | 1 (Launcher) / 3 (in-game) |
| W5 | **Maintenance Mode** | ปิดรับ login พร้อม countdown, whitelist บัญชี GM/tester ให้เข้าได้ | 1 |
| W6 | **World Map Browser** | ดู catalog 257 maps / 516 sub-maps (`WorldMapCatalog`), สถานะไฟล์พร้อม, ตั้ง map เริ่มต้นของตัวละครใหม่ (แทนการแก้ `initial_map_id` ใน JSON) | 1 |
| W7 | **ผู้เล่นออนไลน์ต่อ Map + Heatmap** | นับต่อ map ตอนนี้ได้จาก state ของ session; heatmap ตำแหน่งเมื่อมี movement | 1 / 3 |
| W8 | **Drop Table & NPC Shop Editor** | แก้ drop rate รายมอนสเตอร์ และราคาร้าน NPC (ต้องมีระบบ drop/shop และ item catalog) | 3 |
| W9 | **Item / Monster Catalog** | ถอด `GameData.c9d/.c9t` + `ENG_GameData.c9t` เป็นตาราง reference ใน DB → ทุกหน้าที่เลือกไอเท็มใช้ชื่อจริงแทนรหัสตัวเลข | 2 (งาน RE แยก) |
| W10 | **World Boss / Spawn Control** | สั่ง spawn บอส, ดูเวลาเกิด | 4 |
| W11 | **Leaderboards** | level, gold, PvP (`PvP_Vimpeli` มีใน catalog) | 3 |
| W12 | **Economy Alerts** | แจ้งเตือนเข้า Grafana/Portal เมื่อ gold เข้าระบบเกินเกณฑ์ หรือมีไอเท็มหายากเกิดมากผิดปกติ (จับ bug/dupe) | 3 |
| W13 | **World Snapshot** | สำรอง DB ทั้งโลกก่อนอัปเดต server/patch และคืนค่าได้ | 1 |

---

## 5. Data Model ที่เสนอ (PostgreSQL 18 ตาม ADR 0001)

schema `game` (ข้อมูลเกมที่ server และ Portal ใช้ร่วม) และ `ops` (admin/audit):

```sql
-- ===== game: บัญชีและตัวละคร =====
CREATE TABLE game.accounts (
  id            bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  login_name    citext UNIQUE NOT NULL,
  password_hash text   NOT NULL,          -- PHC string (argon2id/pbkdf2)
  status        text   NOT NULL DEFAULT 'active' CHECK (status IN ('active','suspended','banned')),
  slot_count    smallint NOT NULL DEFAULT 17 CHECK (slot_count BETWEEN 1 AND 17),
  is_gm         boolean NOT NULL DEFAULT false,
  created_at    timestamptz NOT NULL DEFAULT now(),
  last_login_at timestamptz
);

CREATE TABLE game.account_sanctions (       -- ban/suspend พร้อมประวัติ
  id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  account_id bigint NOT NULL REFERENCES game.accounts,
  kind text NOT NULL CHECK (kind IN ('ban','suspend','mute')),
  reason text NOT NULL, starts_at timestamptz NOT NULL DEFAULT now(), ends_at timestamptz,  -- NULL = ถาวร
  issued_by bigint NOT NULL, revoked_at timestamptz, revoked_by bigint
);

CREATE TABLE game.characters (
  id          bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,   -- แทน Slot+1 (ส่งเป็น UInt64 ใน packet)
  account_id  bigint   NOT NULL REFERENCES game.accounts,
  slot        smallint NOT NULL,
  name        text     NOT NULL,
  job         smallint NOT NULL,            -- asset job code (TemplateIds ใน CharacterLobby)
  appearance  bytea    NOT NULL CHECK (length(appearance) = 44),
  level       smallint NOT NULL DEFAULT 1,
  exp         bigint   NOT NULL DEFAULT 0,
  gold        bigint   NOT NULL DEFAULT 0 CHECK (gold >= 0),
  hp int, mp int,
  map_id int, submap smallint, pos_x real, pos_y real, pos_z real,
  play_seconds bigint NOT NULL DEFAULT 0,
  created_at timestamptz NOT NULL DEFAULT now(), last_played_at timestamptz,
  deleted_at timestamptz,                   -- soft delete
  version     bigint NOT NULL DEFAULT 0     -- optimistic concurrency
);
CREATE UNIQUE INDEX ON game.characters (account_id, slot) WHERE deleted_at IS NULL;
CREATE UNIQUE INDEX ON game.characters (lower(name))     WHERE deleted_at IS NULL;

CREATE TABLE game.character_items (
  serial      uuid PRIMARY KEY,             -- serial ต่อชิ้น → ตรวจ dupe ได้
  character_id bigint NOT NULL REFERENCES game.characters,
  location    text NOT NULL CHECK (location IN ('inventory','equipment','warehouse','mail')),
  slot        smallint NOT NULL,
  item_id     int NOT NULL,                 -- อ้าง game.item_catalog เมื่อถอดได้
  quantity    int NOT NULL CHECK (quantity > 0),
  enchant     smallint NOT NULL DEFAULT 0,
  options     jsonb NOT NULL DEFAULT '{}',
  bound       boolean NOT NULL DEFAULT false,
  expires_at  timestamptz,
  UNIQUE (character_id, location, slot)
);

CREATE TABLE game.item_catalog (            -- จาก RE ของ GameData (W9)
  item_id int PRIMARY KEY, name_en text, name_ko text, category text, rarity smallint,
  max_stack int, raw jsonb
);

-- ===== game: เศรษฐกิจ (append-only) =====
CREATE TABLE game.ledger (
  id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  at timestamptz NOT NULL DEFAULT now(),
  character_id bigint NOT NULL,
  asset text NOT NULL CHECK (asset IN ('gold','item','exp')),
  item_id int, item_serial uuid,
  delta bigint NOT NULL,                    -- + เข้า / - ออก
  balance_after bigint,
  reason text NOT NULL,                     -- drop|quest|npc_shop_buy|npc_shop_sell|trade|mail|enchant|admin_grant|rollback|...
  ref text, map_id int,
  request_id uuid UNIQUE                    -- idempotency
);
CREATE INDEX ON game.ledger (at); CREATE INDEX ON game.ledger (character_id, at);

-- ===== game: โลก =====
CREATE TABLE game.world_events (
  id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name text NOT NULL, description text,
  starts_at timestamptz NOT NULL, ends_at timestamptz NOT NULL,
  recurrence text,                          -- RRULE หรือ NULL
  effects jsonb NOT NULL,                   -- {"rates":{"Exp":2},"login_reward":{...},"open_maps":[...]}
  enabled boolean NOT NULL DEFAULT true, created_by bigint NOT NULL
);
CREATE TABLE game.rate_policy (scope text PRIMARY KEY, drop numeric, rare_drop numeric, upgrade numeric, exp numeric, cap numeric);
CREATE TABLE game.announcements (id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY, channel text, -- launcher|ingame
  message text NOT NULL, starts_at timestamptz, ends_at timestamptz, repeat_minutes int);
CREATE TABLE game.world_settings (key text PRIMARY KEY, value jsonb NOT NULL);  -- maintenance, default map, ...

-- ===== ops: session, audit, command =====
CREATE TABLE ops.sessions (id uuid PRIMARY KEY, account_id bigint, character_id bigint, remote_ip inet,
  client_hash text, started_at timestamptz, ended_at timestamptz, end_reason text, last_state text, map_id int);
CREATE TABLE ops.audit_log (id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY, at timestamptz DEFAULT now(),
  operator text NOT NULL, action text NOT NULL, target_type text, target_id text,
  before jsonb, after jsonb, reason text NOT NULL, request_id uuid UNIQUE);
CREATE TABLE ops.admin_commands (            -- Portal → game server (ตัวละครออนไลน์)
  request_id uuid PRIMARY KEY, created_at timestamptz DEFAULT now(), operator text NOT NULL,
  kind text NOT NULL, payload jsonb NOT NULL,
  status text NOT NULL DEFAULT 'pending' CHECK (status IN ('pending','applied','rejected','expired')),
  applied_at timestamptz, result jsonb);
CREATE TABLE ops.character_snapshots (id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  character_id bigint NOT NULL, taken_at timestamptz DEFAULT now(), cause text, data jsonb NOT NULL);
```

Migration: ใช้ EF Core migrations หรือ SQL migration ตามที่งานข้อ 3 เลือก; มีสคริปต์ import
`config/migration/local-state/*.json` + `artifacts/*characters.local.json` + `pending-grants` เข้าบัญชี default

---

## 6. การเชื่อมกับ Game Server

```
 Admin Portal (Web, งาน 3/3.1)
   └─ Account / World pages (งานนี้) ──HTTP──► Admin API (C9.Admin)
                                                  │  เขียน ops.admin_commands / อ่าน-เขียน game.*
                                                  ▼
                                            PostgreSQL ◄──── C9.Server (lobby/world)
                                                  ▲              │ poll หรือ LISTEN/NOTIFY รับ command
                                                  └──────────────┘ apply → ledger + audit + status
```

- **ตัวละครออฟไลน์**: API เขียน `game.*` ตรงใน transaction เดียวกับ audit + snapshot
- **ตัวละครออนไลน์**: API เขียน `ops.admin_commands` → server รับผ่าน `LISTEN/NOTIFY` → apply ในหน่วยความจำ
  และ DB → อัปเดต status; Portal แสดงผลลัพธ์ (ถ้า server ยังไม่รองรับ command นั้น = `rejected` พร้อมเหตุผล)
- **Event/Rates**: server อ่าน `game.world_events` + `rate_policy` ทุก N วินาทีหรือเมื่อมี NOTIFY
- **Metrics** สำหรับ Grafana (งาน 3.1): `c9_online_players{map}`, `c9_gold_total`, `c9_gold_faucet_total{reason}`,
  `c9_gold_sink_total{reason}`, `c9_items_created_total{rarity}`, `c9_logins_total{result}`
  และ Grafana ใช้ PostgreSQL datasource อ่าน `game.ledger` ทำกราฟเศรษฐกิจได้ตรง ๆ
- `CharacterLobby` เปลี่ยนจาก JSON → `ICharacterRepository` (มี JSON implementation เดิมไว้สำหรับ offline test)

### API (ร่าง — prefix จริงคือ `/api/v1/accounts/...` และ `/api/v1/world/...`)

```
GET    /api/accounts?q=&status=            GET  /api/accounts/{id}
POST   /api/accounts                       PATCH /api/accounts/{id}
POST   /api/accounts/{id}/sanctions        DELETE /api/accounts/{id}/sanctions/{sid}
GET    /api/accounts/{id}/timeline         GET  /api/accounts/{id}/sessions
GET    /api/characters/{id}                PATCH /api/characters/{id}         (If-Match: version)
POST   /api/characters/{id}/delete|restore|move|export   POST /api/characters/import
GET    /api/characters/{id}/items          POST /api/characters/{id}/items    (grant/remove ผ่าน ledger)
GET    /api/characters/{id}/snapshots      POST /api/characters/{id}/rollback
GET    /api/sessions/online                POST /api/sessions/{id}/kick
GET|POST|PATCH|DELETE /api/world/events    GET  /api/world/rates/effective?at=
GET|PUT /api/world/rates                   GET|POST /api/world/announcements
GET|PUT /api/world/maintenance             GET  /api/world/maps
GET    /api/economy/summary?from=&to=      GET  /api/economy/flows?group=reason
GET    /api/economy/top-holders            GET  /api/audit?target=&operator=
```

ทุก endpoint ที่เขียนข้อมูลต้องมี `Idempotency-Key` (UUID) และ `reason`

---

## 7. แผนงานเป็นเฟส + ประเมิน Severity

| เฟส | งาน | Severity | เหตุผล | ทำได้เมื่อ |
|---|---|---|---|---|
| **1** | Persistence layer (PostgreSQL schema, migration, import JSON เดิม), `ICharacterRepository` ใน server, Account CRUD, ตัวละคร 17 slots (ดู/แก้ชื่อ/ย้าย slot/ลบ-กู้คืน/export-import), Ban/Suspend, Sessions + kick (ปิด socket), Audit log, Rates + Event scheduler (rates), Maintenance, Map browser, ประกาศหน้า Launcher | **สูง** (แตะ server lobby + data migration; ผิดแล้วตัวละคร Lab เสีย/เข้าเกมไม่ได้) | ต้องรักษา byte layout ของ lobby เดิมและเปลี่ยน character Id | หลัง architecture/API ของงานข้อ 3 ออก |
| **2** | Snapshot/rollback, Character templates, ย้ายข้ามบัญชี, โครง Economic Dashboard (ledger + Grafana panels), Item catalog (RE `GameData.c9d/.c9t`) | กลาง (dashboard) / **สูง** (RE catalog) | RE format ไฟล์ข้อมูลที่ยังไม่รู้ | หลังเฟส 1 |
| **3** | Inventory/Equipment/Gold/Exp edit, Grant ส่งจริง, ประกาศในเกม, GM commands, Drop/Shop editor, ตรวจ dupe/anomaly, leaderboards | **สูงมาก** (ต้อง RE packet ของ inventory/chat/notice กับ Client ใหม่ `b4580ed4` ที่ป้องกันด้วย WinLicense) | ขึ้นกับการ implement gameplay ใน server | หลัง server มี inventory/drop/shop |
| **4** | World boss/spawn control, heatmap ตำแหน่ง | สูง | ต้องมี movement/AI | ระยะยาว |

ข้อควรระวัง:
- **Account binding** (ข้อ 3.3) เป็นตัวกั้นของ "หลายบัญชีจริง" — ระหว่างนี้ทุกคนเข้าบัญชี default
- อย่าเปิดหน้าแก้ inventory/gold ก่อน server มีระบบนั้น เพราะแก้ DB แล้ว Client ไม่เห็นผล (สร้างความสับสน)
  หน้าเหล่านี้จะแสดงสถานะ "รอระบบเกม" แทน
- ข้อมูลตัวละคร `Lab` ปัจจุบันต้อง import และทดสอบ login → lobby → Waterford ซ้ำหลังเปลี่ยนเป็น DB

---

## 8. ขอบเขตโค้ดของงานนี้ (ตกลงกับงานข้อ 3 แล้ว 15 พ.ย. 2026)

Portal หลักคือ `src/C9.Portal` (Blazor Web App, REST `/api/v1`) ของงานข้อ 3 โดยมี contract ใน
`src/C9.Portal.Abstractions` งานนี้เป็นเจ้าของ:

- `src/C9.GameData` — Npgsql + SQL เขียนเอง + migration เป็นไฟล์ `.sql` ลำดับเลข (schema `game`/`ops`)
  ใช้ร่วม server และ Portal; DB คือ service `c9-db` (postgres:18-alpine) ใน `compose.yaml` profile `gamedata`,
  connection string จาก `C9_DB_CONNECTION` (รอเจ้าของอนุญาตเพิ่ม NuGet package Npgsql)
- `src/C9.Portal.Accounts` และ `src/C9.Portal.World` — Razor class library ที่ implement `IPortalModule`
  หน้า `/accounts`, `/characters`, `/world`, `/economy`; API ที่ `/api/v1/{module}` (ไม่แก้ host ของ Portal)
- สิทธิ์ใช้ `PortalPolicies` (Owner, Administrator, GM, ContentEditor, ReadOnly); audit ผ่าน `IAuditLog`
  (เพิ่ม implementation บน Postgres ได้)
- ข้อมูล session ออนไลน์รับจาก `ISessionFeed` ของ Portal (session key `{service}-{bootId}-{n}`,
  lifecycle: opened, state, character id, map id, closed) เก็บลง `ops.sessions` และใช้ key เดียวกับ packet trace
- ใน `src/C9.Server`: เฉพาะ `CharacterLobby` → repository และ command consumer (ประสานกับงานข้อ 3 ก่อนแก้)

**Account binding:** Client มีหน้า Log In (Username/Password) และส่ง login request type `12036`
payload 1092 bytes ไปที่ 14000 ซึ่ง server ถอดรหัสได้เองอยู่แล้ว จึงจะใช้ decoded packet trace ของงาน 3.1
หา field username แทนการดักด้วย Wireshark (Wireshark เห็นแค่ข้อมูลที่เข้ารหัส RC6)

## 9. คำถามที่ต้องการคำตอบจากเจ้าของโปรเจกต์

1. อยากให้ผู้เล่นล็อกอินผ่าน L9 (มี username/password ของเราเอง) หรือพยายามใช้บัญชี/flow ของ VFUN ที่ Client ส่งมา?
2. เซิร์ฟเวอร์นี้สำหรับเล่นคนเดียว/เพื่อนไม่กี่คน หรือเปิดสาธารณะ? (กระทบระดับความปลอดภัย, anti-cheat, 2FA ของ admin)
3. ไอเดียข้อไหนในตาราง A9–A18 และ W4–W13 ที่ไม่ต้องการ หรืออยากให้ทำก่อน?
