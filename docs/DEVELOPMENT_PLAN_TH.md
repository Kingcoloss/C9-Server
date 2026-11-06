# แผนพัฒนา C9 Server และ Admin Portal

อัปเดต 6 พฤศจิกายน 2026 ตามขอบเขตที่ผู้ใช้เพิ่มเรื่องทุกแผนที่และ Admin Portal
เอกสารนี้เป็นแผนพัฒนาปัจจุบัน ส่วนประวัติ RE และหลักฐานแต่ละเฟสอยู่ใน
[แผน RE](../re/C9_RE_PLAN_TH.md) และ [Protocol status](PROTOCOL_STATUS.md)

## สถานะตั้งต้นและเป้าหมายถัดไป

| ส่วน | สถานะที่ตรวจพบ |
|---|---|
| Launcher | มีแพตช์ใช้ version.ini ในเครื่อง พร้อมสำรองและวิธีคืนค่า |
| ตัวละคร | สร้าง Fighter ชื่อ Lab และโหลดชื่อ คลาส รูปลักษณ์จากไฟล์หลังเริ่มบริการใหม่ได้ |
| เข้าโลก | แก้ sub-map index และรหัสคลาสแล้ว เข้า Waterford Village และเปิด Inventory ได้; ค่าสถานะ ช่องกระเป๋า สกิล และวงจรการเชื่อมต่อยังต้องทำต่อ |
| ทะเบียนแผนที่ | พบ 257 รหัส, 136 โฟลเดอร์ฉาก, 516 ส่วนย่อย; ตรวจตารางแพ็ก 1,887 ไฟล์ / 22,466 รายการ |
| ไฟล์ฉากและ marker | จับคู่และอ่านส่วนต้นได้ 450 คู่ map/sub-map; อีก 66 คู่ต้องตรวจไฟล์ที่ขาด ชื่อเฉพาะภาษา หรือข้อมูลเก่า |
| ระบบเกม | ยังต้องทำ world object/player initialization, movement, inventory, combat, persistence และ cipher epoch ถัดไป |
| Admin Portal | เพิ่มเป็นขอบเขตพัฒนาตามเอกสารนี้; ยังไม่มีหน้าจอหรือ API ของ portal |

เป้าหมายงานเกมลำดับแรกเป็นการเติมข้อมูลตัวละครและระบบหลังเข้าเมือง และทดสอบระบบ
ในฉากจริง ข้อมูลไฟล์ของทุกแผนที่จะใช้ร่วมกับทางเข้าแผนที่เดียวกัน โดยติดตามผล
ทดสอบจริงแยกตาม map/sub-map เพื่อระบุได้ว่าคู่ใดยังติดข้อมูลหรือโปรโตคอลส่วนใด

## ลำดับพัฒนา

| ระยะ | งาน | เงื่อนไขส่งมอบ |
|---|---|---|
| P0: โลกเกมและข้อมูลหลัก | แก้จุด client ปิดตัว, สร้าง player/world objects, เดินและย้ายฉาก, บัญชีจริง, inventory และ persistence พื้นฐาน, cipher rotation | เข้าเมืองและกลับเข้าใหม่ได้ ข้อมูลตัวละครและไอเท็มคงอยู่ มี trace ของ client จริง |
| P1: Content catalog | อ่าน GameData และ MapData ครบทุกตารางที่ค้นพบ จัดรุ่นข้อมูลและความสัมพันธ์ | จำนวนที่นำเข้าตรงกับแหล่งข้อมูล มีรายการที่ยังแปลไม่ได้และหลักฐานอ้างอิง |
| P2: Portal พื้นฐานและการติดตาม | บัญชี admin/สิทธิ์, audit, dashboard, ค้นบัญชี/ตัวละคร, ดู content และสถานะบริการ | ใช้ข้อมูลจาก backend จริงและทดสอบสิทธิ์แต่ละบทบาทได้ |
| P3: การจัดส่งไอเท็ม | Item catalog, ค้นตัวละคร, delivery queue, online/offline delivery, ประวัติผล | ส่งครั้งเดียวได้จริง ตรวจ inventory เต็มและ retry โดยไม่สร้างไอเท็มซ้ำ |
| P4: Rate policies และ Events | ตัวคูณ server/event/account, ตารางตีบวก, กำหนดเวลา, preview และ rollback | ผลในเกมตรงกับ policy และทดสอบผลแยกระหว่างบัญชีได้ |
| P5: Content customization | สกิลเฉพาะบัญชี/ตัวละคร, monster/NPC overrides, client content profiles ที่จำเป็น | ค่าฝั่ง server และ client สอดคล้องกัน บัญชีที่อยู่นอก scope ใช้ค่าของตนเองได้ตามเดิม |
| P6: Operations | ควบคุมบริการ, maintenance, backup/restore, จัดรุ่นและย้อน config/patch | ตรวจผล start/stop/restart และกู้ข้อมูลในสภาพแวดล้อมทดสอบได้ |

งานดู catalog และ monitor ใน P2 ใช้ข้อมูลที่มีแล้วได้ ส่วนคำสั่งที่เปลี่ยนผลเกม
ต้องเชื่อมกับระบบเกมเจ้าของข้อมูลใน P0/P3/P4/P5 และมีสถานะรองรับที่ตรวจสอบได้

## ขอบเขต Admin Portal

### 0. ฟีเจอร์พื้นฐานสำหรับ admin

- บัญชีผู้ดูแล แบ่งบทบาท Owner, Administrator, GM, Content Editor และ Read-only
  พร้อมสิทธิ์อ่าน/แก้/ส่งของ/ควบคุมบริการแยกกัน
- Dashboard สรุปสถานะ server, ผู้เล่นออนไลน์, sessions, errors และงานที่รอทำ
- ค้นและดูบัญชี ตัวละคร สถานะออนไลน์ ไอเท็ม เงิน EXP และประวัติการเปลี่ยนแปลง
- จัดการสถานะบัญชี การระงับ/ยกเลิกระงับ เหตุผล ระยะเวลา และตัด session ตามสิทธิ์
- Audit log ระบุผู้ทำ เวลา เป้าหมาย เหตุผล และค่าก่อน/หลัง พร้อมค้นและส่งออกรายงาน
- ประวัติ config/content versions, การดูความต่าง และการคืนค่ารุ่นก่อน
- ใช้เวลาเดียวกันใน backend และแสดงเขตเวลาในหน้าจอสำหรับ events/maintenance
- ตรวจข้อมูลนำเข้าและแสดงผลกระทบก่อนใช้คำสั่ง; งานใช้เวลานานแสดงสถานะและผลรายรายการ

### 1. ปรับ rate ระดับ Server, Event และบัญชี

รองรับ EXP multiplier, drop chance multiplier, rare drop chance multiplier และ
อัตราสำเร็จของการตีบวก โดยเลือก scope ได้ทั้ง server, event และบัญชีที่ระบุ

กติกาเริ่มต้นที่ต้องทำให้ตรวจสอบได้:

- EXP/drop/rare ใช้ค่าฐานของระบบคูณ Server × Event ที่มีผล × Account; ค่าเริ่มต้นแต่ละ scope คือ 1
- ระบุชัดว่า drop chance ปรับโอกาสเกิด drop ส่วนจำนวนครั้งที่ roll และขนาด stack
  เป็นคนละค่าในข้อมูลเกม; rare chance เป็นโอกาส rare ในขั้นที่ระบบ drop กำหนด
- ตารางโอกาสตีบวกมีค่าฐานแยกตามระดับ/ชนิดไอเท็ม และรวมตัวคูณตาม scope เดียวกัน
  รักษากฎ guarantee/pity ของระบบที่กู้ได้ พร้อมจำกัดผล probability อยู่ในช่วง 0–100%
- ค่าตัวคูณต้องเป็นจำนวนจำกัดและไม่ติดลบ มีคำอธิบายการปัด EXP และเพดานค่าที่ใช้จริง
- Event มีชื่อ ช่วงเริ่ม/สิ้นสุด เขตเวลา สถานะ draft/active/ended และรายการ rates
  หากหลาย event มีผลพร้อมกัน ให้ preview แสดงทุกตัวที่นำมาคูณ
- หน้าตัวอย่างแสดงบัญชี/ตัวละคร ค่า base, server, event, account และค่าผลลัพธ์
  รวมถึงเหตุผลที่ event มีผลหรือหมดอายุแล้ว
- จัดรุ่น policy และเปลี่ยน snapshot ของเกมอย่างสอดคล้องกัน; แสดงว่าบริการใดรับรุ่นใหม่แล้ว

เกณฑ์ผ่าน: ตั้ง EXP ×2 ทั้ง server, เพิ่ม event ×1.5 และบัญชี A ×2 แล้วบัญชี A
ได้รับ ×6 ขณะที่บัญชี B ได้ ×3 ทดสอบ event หมดอายุ การคืนค่า และโอกาสที่ชนเพดาน
พร้อมตรวจผล EXP/drop/ตีบวกกับระบบเกมที่เกี่ยวข้อง

### 2. ดู Game Content Data ทั้งหมด

นำข้อมูลเข้าจาก client build ที่เลือก พร้อม hash/รุ่นและสถานะการแปลแต่ละตาราง
รองรับการค้น กรอง ดูรายละเอียด และความสัมพันธ์ระหว่าง:

- Item catalog ทั้งหมด: ID, ชื่อหลายภาษา, ประเภท, rarity, icon, stack, binding,
  expiry, ค่าสถานะ, การตีบวก และข้อจำกัดการใช้
- Skills, skill levels, effects, cooldowns, costs และความสัมพันธ์กับ class
- Monsters, NPCs, spawn definitions, shops, dialogues และ loot tables
- Maps/sub-maps, instances, portals, quests, rewards, EXP curves และตารางโอกาสต่าง ๆ

แสดงค่าต้นฉบับ ค่าที่ override และค่าที่ใช้งานจริง พร้อม reference ไปยังข้อมูลต้นทาง
มี diff ระหว่าง content versions และรายงาน missing references/แถวที่ยังไม่ทราบ schema
หน้าแผนที่แสดงสถานะ Catalogued, Assets ready, Scene tested และ Playable แยกกัน

เกณฑ์ผ่าน: จำนวนข้อมูลใน catalog ตรงกับจำนวนใน source ของแต่ละตารางที่รองรับ
และทุกแถวที่นำเข้าไม่ได้มีเหตุผลในรายงาน สามารถตาม item → loot table → monster
หรือ map → sub-map → spawn/portal ได้

### 3. ส่งไอเท็มจาก catalog ให้ตัวละครที่ระบุ

- ค้น item จาก catalog ทั้งหมด เลือกบัญชีและตัวละครเป้าหมาย แล้วกำหนดจำนวน
  binding, expiry, enhancement และ attributes ตาม schema ของไอเท็มนั้น
- แสดงข้อมูลตัวละครและรายละเอียดไอเท็มก่อนสร้าง delivery job พร้อมเหตุผลจาก GM
- รองรับตัวละคร online/offline ด้วยคิวที่บันทึกถาวร แสดง queued/delivered/failed
  และสถานะรอพื้นที่เมื่อ inventory เต็ม
- ใช้ transaction และ idempotency key เพื่อให้การกดซ้ำหรือ retry ส่งไอเท็มครั้งเดียว
- ยืนยันผลจาก inventory persistence และการแสดงใน client; ใช้ mail เป็นช่องทางเสริม
  เมื่อกู้และทดสอบ mail protocol แล้ว

เกณฑ์ผ่าน: ตัวละคร offline รับของหลังเข้าเกม ตัวละคร online เห็นของตาม protocol
ที่รองรับ ทดสอบกระเป๋าเต็ม ไอเท็มที่มีข้อจำกัด การขาดการเชื่อมต่อ และ retry
โดยจำนวนของใน persistence ตรงกับประวัติ delivery

### 4. ปรับแต่งสกิลเฉพาะบัญชี และ monster/NPC

สกิลใช้ override profile ที่ผูกกับ Account ID และเลือก Character ID เพิ่มได้
กำหนดค่าที่รองรับ เช่น damage/scaling, cooldown, cost, range, effects และ skill level
โดยให้ server เป็นผู้ใช้กฎคำนวณผลจริง

แยกค่าพื้นฐานและ override รายบัญชีออกจากกัน ผูก profile version เข้ากับ session
และตรวจว่าบัญชีอื่นยังใช้ profile ของตนเอง หลังออกและเข้าใหม่ต้องโหลดรุ่นเดิมได้

สำหรับส่วนที่ client อ่านจากไฟล์ เช่น animation, VFX, icon หรือค่าบางชนิดของ skill
ต้องตรวจความสามารถของ client และ protocol ราย field หากต้องใช้ไฟล์เฉพาะ ให้ทำ
content profile/patch manifest ที่แยกรุ่นและผูกกับบัญชี พร้อม backup และ rollback
การเปิดใช้ field แต่ละตัวต้องมีผลทดสอบว่าค่าใน client และ server ตรงกัน

Monster/NPC editor รองรับค่าสถานะ ระดับ AI/skills, loot, spawn location/count/time,
dialogue และ shop ตามชนิดข้อมูลที่กู้ได้ เลือกขอบเขต server/map/instance/event ได้
การเปลี่ยน model/visual ใช้ content profile เมื่อจำเป็น

เกณฑ์ผ่าน: เปลี่ยนสกิลของบัญชี A แล้วตรวจผลจริงและ persistence ขณะที่บัญชี B
ใช้ค่าของตนเองได้ ทดสอบ monster/NPC ที่แก้ในแผนที่เป้าหมาย รวมการคืนค่ารุ่นก่อน

### 5. Manage and Monitor Server

- ติดตาม login/lobby/world/update/admin services: สถานะ รุ่น build, uptime,
  online players, sessions, map instances, queues และอัตรา disconnect/error
- CPU, memory, disk, network, tick/update latency, packet/connection counters,
  database latency และ delivery/content import jobs
- ดู log และ metrics แยกบริการ ช่วงเวลา บัญชี ตัวละคร session และ instance
  โดยเก็บ metadata ที่จำเป็นและมี correlation ID
- Start/stop/restart บริการตามสิทธิ์ มี maintenance mode, drain sessions,
  config reload และผลยืนยันว่าบริการกลับมาพร้อมใช้งาน
- Backup/restore ข้อมูลและ config, ตรวจอายุ/ผล backup และทดสอบการกู้คืน
- แสดง alerts และเกณฑ์สุขภาพของบริการ รวมประวัติ incident และการดำเนินการ

เกณฑ์ผ่าน: ทำให้บริการทดสอบล้มแล้ว portal ตรวจพบ ควบคุมบริการเป้าหมายได้
และยืนยันสถานะหลัง restart ทดสอบ backup/restore แล้วข้อมูลตัวละครและไอเท็มตรงกัน

## โครงสร้างที่วางไว้

ใช้ C#/.NET ที่มีอยู่ แยก Admin Web/API ออกจาก game packet handlers และเรียก
application services ร่วมกัน ได้แก่ ContentCatalog, Account/Character,
InventoryDelivery, RatePolicy, ContentOverride และ ServerOperations

ข้อมูลที่ต้องมี: admin users/roles/audit, accounts/characters/inventory,
content snapshots/import jobs, scoped rate policies/events,
account/character content profiles, delivery jobs/outbox และ operation history
เริ่มจากการออกแบบ schema/transactions และ migration ของข้อมูลตัวละครปัจจุบัน
เมื่อระบบ persistence หลักพร้อม แผน PostgreSQL ใน ADR เดิมยังเป็นฐานการประเมิน

Portal แสดงความพร้อมของ backend ราย feature และผลส่งมอบแต่ละเฟส การคำนวณตัวอย่าง
และสถานะคิวต้องแยกจากผลที่บันทึกและเกิดขึ้นในเกมแล้วอย่างชัดเจน

## งานถัดไปที่ต้องทำก่อนปิด P0

1. วิเคราะห์ client crash หลังเข้า Waterford และข้อความหลังโหลดฉาก
2. เติม player/world initialization แล้วทดสอบ movement, inventory และกลับเข้าเกม
3. ยืนยัน map/sub-map routing ที่ใช้ร่วมกัน พร้อมบัญชีรายการไฟล์ที่ขาด/ข้อมูลเก่า
4. ทำบัญชีจริงและ persistence/transactions ที่รองรับการจัดส่งไอเท็มและ rate รายบัญชี
5. กำหนด schema และ API ของ catalog/admin ตามขอบเขตข้างต้น แล้วเปิด P1/P2
