# ข้อมูลอ้างอิงจาก Server C9 รุ่นเก่า (ชุด LouLx, build 2014) สำหรับ Server ของเรา (patch ล่าสุด)

อัปเดต 15 พฤศจิกายน 2026 (Asia/Bangkok) — สถานะ: **ร่างแรก รอผู้ใช้ตรวจ**

เอกสารนี้สรุปสิ่งที่ใช้ได้จาก backup VM ของ server C9 รุ่นเก่าที่ผู้ใช้พบ โดยใช้เป็น
**ข้อมูลอ้างอิงแบบอ่านอย่างเดียว** เท่านั้น ไม่ได้คัดลอกโค้ด/ไบนารี/DB เข้า repo และไม่ได้รันโปรแกรมใดจาก backup

## 1. สรุปสั้น (สำหรับคนที่ไม่มีเวลา)

1. Backup คือชุด repack เวียดนาม "C9 5 Class Offline (LouLx)" มี VM Windows 7 x64 + SQL Server 2008 R2
   ข้างในเป็น **server ต้นฉบับครบชุด** (Auth/AuthBroker/World/Game×10/Relay/STUN/Community/Auction/DataCache/Log/Agent/LMS)
   build วันที่ **20 พ.ย. 2014**, `Client Version[1495] ServiceType[China]` และ `C9.exe` ในโฟลเดอร์ Client เป็น build เดียวกัน (ไม่ถูก pack)
2. **Opcode ไม่ได้เปลี่ยนเลขระหว่าง build 2014 กับ Client ปัจจุบัน** (หลักฐานข้อ 4.2) จึงใช้ตาราง handler ของ server เก่า
   เป็นแผนที่ได้เลย: packet ในโลกที่ Client ใหม่ส่งแล้วเรายังไม่ตอบ 10 ตัว มี handler เดิม 8 ตัว (ข้อ 5)
3. Layout ของ packet เคลื่อนที่ `0x0BCE` จาก server เก่า (46 bytes, tick อยู่ที่ +42) **ตรงกับ trace ของ Client ใหม่ทุกไบต์ที่ตรวจได้**
4. ได้กฎ cipher selector ฝั่ง C2S ที่ชัดเจน: `twister = (checksum + typeHash[type]) mod N` (ฝั่ง S2C ไม่บวก typeHash)
   และอธิบายได้ว่าทำไม adjustment ที่สังเกตใน trace ของเราจึงแกว่ง ±16 (ข้อ 4.4)
5. DB เดิมมี 9 ฐาน (C9World/C9Unity/…) และ server เรียก stored procedure 297 ตัว (เก็บรายชื่อไว้แล้ว) ใช้เป็นต้นแบบออกแบบ
   EF Core GameData ในส่วนที่เรายังไม่มี (inventory/quest/skill/mail/guild ฯลฯ) — ยังไม่ได้อ่านตาราง/คอลัมน์จากไฟล์ .mdf (ข้อ 7.3)

## 2. แหล่งข้อมูลและวิธีจัดการ (ISO/IEC 27001, ISO 9001)

| รายการ | ค่า |
|---|---|
| ตำแหน่ง backup (ห้ามแก้) | `C:\MyProjects\New folder\C9-5-Class-LouLx` (`LouLx-Game\Server\*.vmdk` 23 ไฟล์ ~45 GB, `Client\`, `GM\`, `Video\`) |
| VM | `guestOS=windows7-64`, 2 vCPU, 4 GB, ดิสก์ 90 GB (2 partition NTFS: C: ระบบ, D: `C9-Server` + SQL Server) |
| วิธีเปิดดิสก์ | อ่าน VMDK→NTFS ด้วย `dissect.hypervisor`/`dissect.ntfs` ใน venv แยก (เปิดไฟล์แบบ `rb` เท่านั้น ไม่ boot VM) |
| วิธีวิเคราะห์ | static เท่านั้น: `pefile` + `capstone` (linear sweep) ไม่รัน exe ใดจาก backup |
| สำเนาที่ดึงออก (นอก git) | `C:\MyProjects\C9-Legacy-Ref\patch65-loulx\vm-d\` (exe/dll ของ server, `ServerData`, log 19 ก.ย. 2024, GameData ฝั่ง server, ไฟล์ DB `mssql-data\C9*.mdf/.ldf` 5.7 GB) |
| หลักฐานที่เก็บใน repo | `re/evidence/legacy_patch65/packet_catalog_2014.json`, `re/evidence/legacy_patch65/stored_procedures_2014.txt` (เป็นข้อเท็จจริงที่ถอดได้ ไม่มีโค้ด/ไบนารี) |
| ข้อมูลลับที่พบ | `ServerBaseConfig.dat` มี connection string เข้ารหัส, `ServerSubConfig.DAT` มี ServerKey, `connect.udl` มีรหัส `sa` แบบ plain → **ไม่คัดลอกค่าลงเอกสาร/repo** ให้ถือเป็นข้อมูลไม่น่าเชื่อถือและห้ามนำไปใช้ |
| การควบคุมเวอร์ชันหลักฐาน | SHA-256 ด้านล่าง ใช้ยืนยันว่า address ในเอกสารนี้อ้างไฟล์ชุดเดียวกัน |

| ไฟล์ | SHA-256 |
|---|---|
| `C9GameServer.exe` | `567d2f38066a908d0276f3d44fbdb808ae5748b2adde57446d44d3cfac9ffb51` |
| `C9WorldServer.exe` | `660d73b0d0d2d8fc705abb60107269787340770563c4e4381e8a42300e1206f9` |
| `C9AuthServer.exe` | `b07511ab99a33093e304b29cac5b66cb2aa30cb5440511335f2d4529ab7870a5` |
| `C9CommunityServer.exe` | `70d0da498b6c9c79f67fdf79e78ce3d0280a60dcd7ddf346c62f7cb1fa3b6da2` |
| `C9RelayServer.exe` | `3e744879640a06c9398145e622b580e3c3875d6790ad47ec235ee4843ef056bf` |
| `Client\C9.exe` (build 2014, ไม่ pack) | `93041c5ed30ee680c1d507c013c65375ba556e7b4c102e2a50d3e11ce39100b5` |

สคริปต์วิเคราะห์ที่ใช้อยู่ใน `re/scripts/legacy/` (ดู README ในโฟลเดอร์นั้น) ทุกตัวอ่านอย่างเดียว ให้รันด้วย `python -I`
จาก venv ที่ติดตั้ง `dissect.hypervisor dissect.ntfs dissect.volume pefile capstone` และส่ง path เป็น argument
(ยังไม่มี test อัตโนมัติ — งานค้างข้อ 10)

## 3. สถาปัตยกรรม server เดิม (จาก `ServerData\ServerBaseConfig.dat`)

| บริการ | Port | หมายเหตุ / เทียบกับของเรา |
|---|---|---|
| Auth | TCP 10721 (PublicIP) | Client ต่อที่นี่ก่อน: login 0x2EE1, server list 0x2EE5/0x2EE6, เลือก world 0x2EEC → ของเรา = `c9-login` (14000) |
| AuthBroker | TCP 10741 | ภายใน (Auth↔World) |
| World | TCP 10731 | ภายใน, ตัวกลางของ Game server ทั้งหมด (`MaxUser=200`, Match/Ranking) |
| Game `LOB` (ServerNo 1) | TCP 10501, MapKey 56 | **Lobby** = หน้าเลือกตัวละคร (0x0BBB/0x0BBF/0x0EFB ฯลฯ) → ของเรา = `c9-lobby` (14001) |
| Game `NOR` ×6 | 10502–10508 (MapKey 10/186/19/10/33/80) | แต่ละ process ถือชุดแผนที่/ช่อง (ChannelNo) ย้ายแผนที่ข้าม process ด้วย `PS_MoveMap_Server_PC_Channel_Req` |
| Game `GHS`/`GBS`/`PVP` | 10509/10510/10512 | Guild house, Guild battle, PvP |
| Relay | TCP+UDP 10711 (`MaxUdpPort=8`) | P2P relay ในดันเจี้ยน (ยังไม่มีในของเรา) |
| STUN | UDP 10781/10782 | NAT traversal สำหรับ P2P |
| Community / Auction / DataCache / Log / MemCached | 10791 / 10751 / 10761 / 10771 / 11211 | แชท-เพื่อน-ปาร์ตี้ / ตลาด / cache / log |
| Agent / LMS | 11300 / 10900 | ระบบจัดการ server (เทียบได้กับ agent mode + Portal ของเรา) |

ค่าอื่นที่น่าใช้เป็นค่าเริ่มต้น: `KeepAliveInterval=3`, `KeepAliveCheckThreshold=60`, `MaxSkillCountPerSec=7`,
`SpeedHackCheckThreshold=60`, `DailyReset TimeHour=7`, `DefendDos SamplingTime=10 Threshold=100`,
ContentSwitches (`UseInGameShop`, `UsePcTrade`, `UseMail`, `UseAdventureBook`, `UseDailyWork` …) — แนะนำทำเป็น feature flag ใน `world_settings`

## 4. Protocol

### 4.1 Packet catalog และ handler

- ทุก binary ที่ใช้ protocol หลัก (Game/World/Auction) มีตาราง static ของ **1,304 packet type** แต่ละตัวมี
  `typeHash` 32-bit และ flag `encrypted` (819 ตัวเข้ารหัส, 485 ตัว plain) ค่าเหมือนกันทุก binary
- `C9GameServer.exe` ลงทะเบียน handler ฝั่ง C2S **521 ตัว** ผ่าน dispatcher singleton (`0x9510E4`, register `0x6FD1E0`)
  World 143 ตัว, Community 40 ตัว — ไฟล์ `packet_catalog_2014.json` มี handler VA, packet ที่ handler สร้าง/ส่ง (`*_sends`),
  ชื่อ DB query class (`PSQuery…`) และข้อความ log สั้น ๆ ที่ช่วยเดาความหมาย
- ช่วงเลข: 0x0001–0xEE76; กลุ่มใหญ่ 3xxx (487), 60xxx (195), 14xxx (112), 10xxx (106), 2xxx/4xxx
- ชื่อ PS_* จริงเหลือใน binary แค่ 12 ตัว (เช่น `PS_UseSkill_Req`, `PS_Character_FallDamage_Req`=0x0BDD,
  `PS_Chat_World_Req`=0x0C27, `PS_Summon_Pet_Req`=0x2759) ที่เหลือต้องตั้งชื่อจากพฤติกรรม

### 4.2 หลักฐานว่า opcode คงเดิมถึง Client ปัจจุบัน

| ตรวจ | ผล |
|---|---|
| ID ที่โปรเจกต์ยืนยันแล้ว (0x0BBB, 0x0BBF, 0x0BC3, 0x0BC5, 0x0C1E, 0x0C20, 0x0EFB, 0x1771, 0x26AD, 0x270F, 0x2EE1, 0x2EE5, 0x2EEA, 0x2EEC …) | อยู่ใน catalog 2014 ทั้งหมด และความหมายตรง (เช่น 0x0BC5 handler log `[ClientMapLoad] End`) |
| packet ในโลกที่ Client ใหม่ส่ง 10 ตัว | 8 ตัวมีใน 2014 (ข้อ 5); ไม่มี: **0xF74F, 0xEEB3** (เพิ่มหลัง 2014) และ login 0x2F04 (VFUN) |
| ขนาด payload จาก trace Client ใหม่ (`artifacts/docker-state/trace`) | 0x0BCE = 46 bytes ตรงกับ reader เดิม (tick ที่ +42), 0xED1D = 4 bytes (u32 key) ตรง, 0x270F = 8 bytes (tick+flag) ตรง |
| Client 2014 กับ Server 2014 | catalog ตรงกัน 1,302/1,304 (ขาด 0xEA6A/0xEA6B ฝั่ง client), hash และ flag ตรงทุกตัว |

ข้อจำกัด: Client ใหม่ถูก WinLicense pack จึงเทียบ catalog ทั้งชุดจากไฟล์ไม่ได้ ถ้ามี memory dump ที่ unpack แล้ว
ให้สแกน byte pattern `BF <hash32> B0 <enc> B9 <id16> 00 00 E8` (หรือ `BF <hash32> 32 C0 B9 …` สำหรับ plain)
จะได้ catalog + typeHash ของ Client ใหม่ทั้งชุดเพื่อ diff กับไฟล์ JSON (สคริปต์ `client_catalog.py` ทำเรื่องนี้)

### 4.3 Header และ encrypted flag

Header 13 bytes ตรงกับที่โปรเจกต์ใช้: `+0 len(u16) +2 flag(u8) +3 type(u16) +5 checksum(u32) +9 seq(u32)` payload เริ่ม +13
handler ฝั่ง server ตรวจ `len >= 13`, `type` ตรง, และ `len <= buffer` ก่อนอ่าน payload ทุกครั้ง

ข้อที่ควรตรวจกับของเรา: catalog 2014 ระบุว่า reply บางตัวเป็น **plain** เช่น `0x0BC6`, `0x0BCF`, `0x0E44`, `0x0EFE`, `0x0C20`, `0x0004`
ส่วน `0x0BC0`, `0x0BC3`, `0x0C1E`, `0x0E4C`, `0xED1E` เป็น encrypted — ถ้าเราส่งผิดโหมด Client อาจทิ้ง packet

### 4.4 Cipher selector (สำคัญต่อ ConnectionCodec)

จาก decrypt path `0x6FDB60` และ encrypt path `0x6FDA60` ของ GameServer 2014:

- **C2S (ฝั่งรับของ server):** `selector = (checksumField + typeHash[type]) mod 40` ยกเว้น 0x1771 และ 0x2EE1 ใช้ typeHash = 0
- **S2C (ฝั่งส่งของ server):** `selector = checksumField mod 40` ไม่บวก typeHash → ตรงกับที่ `ConnectionCodec.Encode` ของเราใช้ adjustment 0 แล้วใช้ได้
- ลำดับฝั่งส่ง: ใส่ seq → ตั้ง checksum = 100 → คำนวณ checksum → twist payload ด้วย selector → RC6 → คืน buffer เดิม
- Client ใหม่ใช้ modulus 80 (ของเราใช้ 80 อยู่แล้ว) และ **typeHash ของ Client ใหม่ไม่เท่ากับค่าปี 2014** (ทดสอบกับ trace 20 type: ไม่ตรงเลย)
  แต่ adjustment ต่อ type ใน trace คงที่ ยกเว้นเหลื่อมไป 16 เมื่อ `checksum + typeHash` ล้น 32 บิต (เพราะ 2^32 mod 80 = 16)
  เช่น 0x270F เห็น 33 กับ 17, 0x26AD เห็น 75 กับ 59 → การ brute-force 80 selector ในปัจจุบันถูกต้อง
  ถ้าได้ typeHash ของ Client ใหม่จาก memory dump (ข้อ 4.2) จะเลิก brute-force ได้และตรวจ packet ปลอมได้ดีขึ้น

## 5. Packet หลังเข้าโลกที่ Client ใหม่ส่งแล้วเรายังไม่ตอบ

ลำดับที่เห็นใน trace: `0x0BC3` (S2C) → Client ส่ง `0xED1D`×9, `0x0E4B`, `0x0BC4`, `0x0EFD`, `0x0EB1`, `0x2347`, `0x11A1`, `0xF74F`, `0xEEB3`, `0x0BCE` …

| ID | enc | handler 2014 | สิ่งที่ server เดิมทำ / ตอบ | ความมั่นใจ |
|---|---|---|---|---|
| 0x0BC4 (payload 0) | 1 | `0x6A3570` | ลงทะเบียน PC เข้า field/แผนที่: guild house, AOS, ดึง PvP rank (`PSQueryPvpRank`) แล้วส่งชุดข้อมูลเริ่มต้นหลายตัว (0x0F08, 0x0DF8, 0x2390, 0x0DBD, 0x32C8, 0x36BB, 0x36D2, 0x1FA7) | กลาง (รู้ว่าเป็นขั้น "เข้า field" แต่ยังไม่ถอดทุก reply) |
| 0x0BC5 (payload 0) | 1 | `0x6A3A90` | `[ClientMapLoad] End` → ตอบ **0x0BC6 (plain, 21 bytes) = `{u64 playerObjectId}`** แล้วเริ่ม action state, ส่ง 0x0E44 (position reset) และข้อมูล party/guild battle (0x2335, 0xEAxx) ตามสถานะ | สูง สำหรับ 0x0BC6 |
| 0xED1D (u32 key) | 1 | `0x66CB30` | ตอบ **0xED1E** = `{u32 key (echo), u8 0, u32 count, entry[count] × 72 bytes}` (ยาว 22 + 72×count) → ตอบ count=0 ได้อย่างปลอดภัย | สูงสำหรับ layout, ความหมายของ key ยังไม่รู้ (ค่าใน trace เช่น 0x20) |
| 0x0E4B (5 bytes) | 1 | `0x491B80` | ตอบ **0x0E4C** ขนาดคงที่ 8,180 bytes (payload 8,167) — server เดิมส่ง 0x0E4C หลัง CharacterSelect 0x0BBF ด้วย น่าจะเป็น blob ตั้งค่าของบัญชี/ตัวละคร | กลาง (ยังไม่ถอดเนื้อใน) |
| 0x0EFD (payload 0) | **0** | `0x66B830` | ขอเวลา server → ตอบ **0x0EFE (plain, 25 bytes)** จาก `_time64()` และ `_get_timezone()` = เวลา unix 64-bit + timezone | สูงสำหรับความหมาย, ลำดับ field ต้องยืนยัน |
| 0x0EB1 (payload 0) | 1 | `0x6AD5A0` | จดหมาย: query `GetMailExCount` + `GetPcMailExList` แบบ async แล้วตอบจาก callback ของ DB | สูงสำหรับความหมาย |
| 0x2347 (8 bytes) | 1 | `0x66B440` | ข้อมูล ladder/PvP rank: `PSQueryGetLadderInfoAsync` แบบ async | สูงสำหรับความหมาย |
| 0x11A1 (payload 0) | 1 | `0x69DC20` | ตอบ **0x11A2** (ยังไม่ทราบความหมาย) | ต่ำ |
| 0x0BCE (46 bytes) | 1 | `0x6BD530` | **movement/action หลัก** ดูข้อ 6 → server กระจาย **0x0BCF (plain)** ให้ผู้เล่นใน field | สูง |
| 0xF74F, 0xEEB3 | – | ไม่มีในปี 2014 | ระบบที่เพิ่มหลัง patch นี้ ต้องถอดจาก Client ใหม่เอง | – |

## 6. การเคลื่อนที่ (movement) ในโลก — สิ่งที่ server เดิมทำ

ผู้เล่นเดินในเมืองได้เองฝั่ง Client; server ใช้ packet ต่อไปนี้เพื่อตรวจและกระจายตำแหน่ง ไม่ต้องตอบ ack ทุก packet

| ID | ทิศ | Layout ที่ถอดได้ (offset ใน payload) | หมายเหตุ |
|---|---|---|---|
| 0x0BCE | C2S, 46 bytes | `+0 f32 x, +4 f32 y(ความสูง), +8 f32 z, +12 f32, +16 u32, +20 f32, +32 u32, +36 u32, +42 u32 clientTick` | ตรวจ y ต่ำกว่าพื้นแผนที่ → reset, ตรวจ speed hack ด้วย tick, ตรวจระยะ (เกณฑ์ 50 และ 500) แล้วบันทึกตำแหน่ง; ส่ง 0x0BCF ที่มี objectId + ตำแหน่ง + ค่าจาก +16/+32/+36 ให้ผู้เล่นอื่น |
| 0x0BCB | C2S, ~24 bytes | `+0 f32 x,y,z, +12 f32 ทิศ, +16 u32, +20 u32 clientTick` | อัปเดตตำแหน่งแบบเบา ไม่มี reply |
| 0x0BCC | C2S | – | ตอบ 0x0BCD (plain) ยังไม่ถอด |
| 0x0BCF | S2C, plain | objectId (u64) + ตำแหน่ง/ทิศ/สถานะ | สำเนาการเคลื่อนที่ให้คนอื่น — ผู้เล่นคนเดียวยังไม่จำเป็น |
| 0x0E44 | S2C, plain | – | "send player position reset information" ใช้ตอนตรวจพบตำแหน่งผิด/ตกแมพ |
| 0x270F | C2S, 8 bytes | `+0 u32 clientTick, +4 u32 flag` | heartbeat ที่ server ใช้นับ position/speed hack ด้วย ไม่มี reply |
| DB | – | `Game.UspUpdatePcPos(5 params)` | บันทึกตำแหน่งตัวละคร (ใช้ map ไปยัง `characters.map_id/pos_x/pos_y/pos_z` ของเรา) |

ข้อสังเกตของ trace Client ใหม่ (0x0BCE แรก): x≈38497, y≈3501, z≈35872 และ tick ≈ 20.8 ล้าน ms สอดคล้องกับ layout ข้างบน

**ข้อเสนอสำหรับ thread "Make the character move in game"** (เรียงลำดับ ทำทีละตัวแล้วจับ trace ยืนยัน):
1. ตอบ 0x0BC5 ด้วย 0x0BC6 plain `{u64 objectId}` (ใช้ id เดียวกับที่ส่งใน 0x0BC0/0x0BC3)
2. ตอบ 0x0EFD ด้วย 0x0EFE plain (เวลา server + timezone), 0xED1D ด้วย 0xED1E (count=0)
3. รับ 0x0BCE/0x0BCB/0x270F แบบไม่ตอบ แต่บันทึกตำแหน่งลง DB; ส่ง 0x0E44 เฉพาะเมื่อจะ reset
4. 0x0E4B → 0x0E4C (8,180 bytes) ต้องถอดเนื้อในก่อน ส่งแบบผิด layout อาจทำ Client ล่ม
5. 0x0BC4 → ชุด reply เริ่มต้น ควรถอดทีละตัวด้วย Ghidra (address อยู่ในตารางข้อ 5)

## 7. ฐานข้อมูล

### 7.1 ฐานข้อมูลเดิม (SQL Server 2008 R2, `MSSQL10_50.MSSQLSERVER`)

| DB | ขนาด .mdf | ใช้โดย (จาก config/log) |
|---|---|---|
| C9World | 333 MB | World/Game: ตัวละคร ไอเทม เควสต์ กิลด์ (schema `Game`) |
| C9Unity | 419 MB | Auth: บัญชี (schema `Auth`) — `connect.udl` ชี้ Initial Catalog นี้ |
| C9Service | 419 MB | บริการ/ร้านค้า in-game |
| C9Rank | 419 MB | ranking / ladder |
| C9Log, C9Audit, C9BillingLog | 429 / 3 / 3 MB | log, audit, billing |
| C9Resource, C9Web | 419 MB ต่อไฟล์ | ข้อมูลอ้างอิง/เว็บ |

### 7.2 API ของ DB ที่ server เรียก (297 stored procedure, รายชื่อ + จำนวน parameter ใน `stored_procedures_2014.txt`)

schema: `Game` 281, `Auth` 11, `Server` 14, `LMS` 2, `Admin` 1, `Statistics` 1; หมวดใหญ่: Pc* 123, Guild* 41, Tournament* 25, Item* 17, Survival* 11

| หมวดใน DB เดิม | ตัวอย่าง procedure | มีใน EF Core GameData ของเรา? |
|---|---|---|
| บัญชี/ล็อกอิน | `Auth.__UspLoginAccountEx`(33), `Auth.UspLogoutAccountEx`, `Game.UspCheckConnectableAccount` | มี `game.accounts`, `account_sanctions`, `ops.sessions` (ยังไม่มีประวัติ login แยก) |
| ตัวละคร | `Game.UspCreatePc`(57), `_UspGetPcList`(5), `UspLoginPc`(13), `UspGetPcBaseRawData`, `UspUpdatePcPos`(5), `UspUpdatePcStyle`(38), `UspUpdatePcAddStat`(11), `UspDailyResetPc` | มี `game.characters` (level/exp/gold/hp/mp/map/pos) — **ยังไม่มี** stat ที่แจก, style 38 ช่อง, daily reset, tutorial flag |
| ไอเทม/คลัง | `_UspGetPcItemInvenList`, `_UspGetPcItemEquipList`, `UspGetPcBagRawData`, `UspChangeItemPosition`(8), `UspAddItemEnfcRate`, `UspAddPcMainInvenSlots` | มี `game.character_items` + `item_catalog` (ยังไม่มี bag/slot expansion, ความทนทาน, enchant rate) |
| สกิล | `UspGetPcSkillList`, `UspAddPcSkill`, `UspAddPcSkillPoint`, `UspGetPcSkillBookEquipList` | ไม่มี |
| เควสต์/งานประจำวัน | `UspGetPcQuestList`, `UspCreatePcQuest`, `UspCompletePcQuest`, `UspUpdatePcWorkBookNormal`, AdventureBook | ไม่มี |
| เมล/เพื่อน/บล็อก | `UspGetPcMailExList`, `UspGetPcBuddyList`, `UspAddPcBlocker` | ไม่มี |
| กิลด์/guild house | `UspCreateGuild`, `UspCreateGuildHouse`, `UspAddPcGuildSkill` และอีก ~38 ตัว | ไม่มี |
| PvP/ranking/tournament/survival | `UspAddPcPvPScore`, `UspGetPcRank`, Tournament* 25 ตัว, Survival* 11 ตัว | ไม่มี |
| ร้าน/เงิน | `UspBuyShopItem`, `UspAddPcMoney`, `UspAddPcMoneyToStore` | มี `game.ledger` (บันทึกเงินเข้าออก) แต่ยังไม่มีร้าน |
| สัตว์เลี้ยง/อาชีพช่าง | `UspCreatePet`, `UspUpdatePcArtisan` | ไม่มี |

ข้อแนะนำการออกแบบ: ของเดิมเก็บหลายส่วนเป็น "RawData" (blob ต่อตัวละคร เช่น bag/inven/store/workbook) ส่วนของเราใช้ตารางแยก
ซึ่งตรวจสอบได้ดีกว่า (ISO 27001 audit) ให้คงแนวทางของเรา แต่ใช้รายการ procedure ข้างบนเป็น checklist ของ field ที่ต้องมี
และเพิ่มทีละโมดูลตามที่ระบบเกมต้องใช้จริง

### 7.3 ยังไม่ได้ทำ: อ่านตาราง/คอลัมน์จริงจากไฟล์ .mdf

ไฟล์ .mdf/.ldf ถูกคัดลอกออกมาแล้ว (นอก git) แต่ยังไม่ได้เปิดดู schema ระดับตาราง/คอลัมน์และเนื้อของ procedure
วิธีที่ตรงที่สุดคือ attach สำเนาไฟล์เข้า SQL Server (เช่น container `mcr.microsoft.com/mssql/server`) ซึ่งต้อง **ให้ผู้ใช้ยอมรับ
license ของ Microsoft SQL Server เอง** จึงยังไม่ได้ทำ — รอผู้ใช้ตัดสินใจ

## 8. ความต่างระหว่าง build เก่า (2014, ที่ผู้ใช้ระบุว่า level cap 65) กับ patch ที่เราทำ (95 + rebirth)

| หัวข้อ | Build 2014 (LouLx) | Patch ปัจจุบัน (`C:\VFUN\C9-GL`) | หลักฐาน |
|---|---|---|---|
| Level cap | ผู้ใช้ระบุ 65 (มี string `LevelLimit_Max`, `MaxLevel` แต่ยังไม่ได้อ่านค่าจาก GameData) | 95 | ผู้ใช้ / ยังไม่ยืนยันจากไฟล์ |
| Rebirth | **ไม่มี** (ไม่พบ string rebirth/reincarnation/transcend/awaken ใน server และ client 2014) | มี | strings |
| อาชีพ | 5 คลาสเล่นได้: Fighter, Hunter, Shaman, WitchBlade, Mistic (ชื่อจีน "玲珑"); มีโฟลเดอร์ Valcan แต่ยังไม่เปิด | โฟลเดอร์ Player ชุดเดียวกัน 6 ชื่อ | GameData/Player |
| ข้อมูลเกม | `GameData.cfs` (mode เดิม) มี `GameData.c9d` 37 MB เหมือนกันทั้ง client/server | `GameData.cfs` รูปแบบใหม่ (reader ปัจจุบันของเราแจ้ง unsupported mode) + โฟลเดอร์ใหม่ `Rider`, `TalismanMonster` | `cfs_list.py` |
| แผนที่ | 105 โฟลเดอร์ใน Client MapData (MapData ฝั่ง server 142 โฟลเดอร์ รวม PvP/test) | 121 โฟลเดอร์ | dir listing |
| Packet ใหม่ | – | 0xF74F, 0xEEB3, login 0x2F04 | catalog vs trace |
| Cipher | twister 40 ตัว (`PacketTwister_1..40`), mod 40 | registry 80 ตัว, mod 80, typeHash เปลี่ยน | ข้อ 4.4 |
| Login | 0x2EE1 (เข้ารหัส, typeHash 0) | 0x2F04 (plain, VFUN) | catalog/KnownPackets |
| การป้องกัน client | ไม่ pack | WinLicense | PE entropy |

## 9. ความเสี่ยงและข้อควรระวัง

1. **ลิขสิทธิ์:** ไบนารี/DB/GameData เป็นของผู้พัฒนาเกม ใช้เพื่อศึกษา interoperability เท่านั้น ห้ามนำไบนารีหรือ DB เข้า repo
   หรือแจกจ่าย (เก็บนอก git ตามข้อ 2)
2. **ข้อมูลไม่น่าเชื่อถือ:** backup มาจาก repack บุคคลที่สาม (มีไฟล์ `.php`/`.exe` ใน Recycle Bin, ไฟล์ GM tool) ห้ามรันไฟล์ใดจาก backup
3. **ความต่างของ patch:** offset ทั้งหมดมาจาก build 2014 ต้องยืนยันกับ trace ของ Client ใหม่ทุกครั้งก่อนเขียน handler (ทำแล้วสำหรับ 0x0BCE/0xED1D/0x270F)
4. **ความแม่นของการวิเคราะห์:** ใช้ linear-sweep disassembly + heuristic; ช่อง `*_sends`/`*_hints` ใน JSON อาจมี false positive
   ควรยืนยันด้วย Ghidra ก่อนเขียน handler ที่ซับซ้อน (0x0BC4, 0x0E4C)

## 10. Current State (15 พ.ย. 2026 ~21:45 น.)

เสร็จแล้ว
- ยืนยันตำแหน่ง backup และโครงสร้าง, อ่านดิสก์ VM แบบ read-only, คัดลอกไฟล์อ้างอิงไปไว้นอก git
- ดึง packet catalog 1,304 ตัว + handler 521/143/40 ตัว (Game/World/Community) → `re/evidence/legacy_patch65/packet_catalog_2014.json`
- ยืนยันความต่อเนื่องของ opcode กับ Client ใหม่ และถอด packet หลังเข้าโลก 8 ตัว + movement (ข้อ 5–6)
- ถอดกฎ cipher selector C2S/S2C และอธิบาย ±16 ใน trace (ข้อ 4.4)
- รวบรวม API ของ DB 297 procedure → `re/evidence/legacy_patch65/stored_procedures_2014.txt` และเทียบกับ EF Core GameData (ข้อ 7)

ยังไม่ทำ / งานถัดไป
1. ส่งข้อสรุปข้อ 5–6 ให้ thread "Make the character move in game" (ส่งผ่าน coordinator แล้วในรอบนี้)
2. ถอด layout ของ 0x0E4C, reply ของ 0x0BC4 และ 0x11A2 ด้วย Ghidra (ต้องติดตั้ง Ghidra บน Windows; สคริปต์ `re/scripts/run_ghidra.py` รองรับ)
3. สแกน memory dump ของ Client ใหม่เพื่อดึง catalog + typeHash ชุดใหม่ (ข้อ 4.2)
4. อ่าน schema ตาราง/คอลัมน์จาก .mdf (ต้องให้ผู้ใช้ยอมรับ license SQL Server ก่อน)
5. สคริปต์อยู่ใน `re/scripts/legacy/` แล้ว แต่ยังไม่มี test อัตโนมัติ — ควรเพิ่ม test กับ fixture เล็ก ๆ (ISO/IEC 29110)
6. อ่านค่า level cap จริงจาก `GameData.c9d` เพื่อยืนยันตัวเลข 65
