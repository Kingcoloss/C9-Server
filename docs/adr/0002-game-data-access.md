# ADR 0002: Data access library สำหรับ C9.GameData (PostgreSQL)

วันที่: 2026-11-15 (Asia/Bangkok)
สถานะ: **Proposed** — รอเจ้าของโปรเจกต์ยืนยัน
เกี่ยวข้อง: [ADR 0001](0001-server-stack.md) (PostgreSQL 18), [GAME_ACCOUNT_WORLD_MANAGEMENT_TH.md](../GAME_ACCOUNT_WORLD_MANAGEMENT_TH.md)

## โจทย์

`src/C9.GameData` ต้องเก็บบัญชี ตัวละคร ไอเท็ม ledger เศรษฐกิจ event และ audit ใช้ร่วมกันระหว่าง
game server (`C9.Server`) กับ Admin Portal (`C9.Portal.*`) เจ้าของโปรเจกต์ขอให้ใช้ ORM ที่ป้องกัน SQL injection
และให้เปรียบเทียบตัวเลือกก่อนเลือก

ลักษณะงานที่มีผลต่อการเลือก:

- **Game server**: โหลดตัวละครตอนเลือกตัวละคร, save เป็นระยะหรือตอนออก, เขียน ledger ทุกครั้งที่ทรัพย์สินเปลี่ยน
  ไม่ได้ query DB ทุก packet (state อยู่ในหน่วยความจำ) จึงไม่ใช่ hot path ระดับไมโครวินาที
- **Admin Portal**: ค้นหา/กรอง/เรียง/แบ่งหน้าแบบ dynamic (ชื่อบัญชี สถานะ ช่วงเวลา) ซึ่งเป็นจุดที่ SQL injection
  เกิดบ่อยที่สุดเมื่อประกอบ `WHERE`/`ORDER BY` ด้วย string
- **ความถูกต้อง**: Portal กับ server แก้ตัวละครเดียวกันได้ → ต้องมี optimistic concurrency และ transaction ที่รวม
  การแก้ข้อมูล + audit + ledger
- **Schema จะเปลี่ยนบ่อย** ตามการ RE ระบบเกม → ต้องมี migration ที่เชื่อถือได้
- นโยบายเดิมของ repo คือไม่ใช้ external package (`NuGet.Config` ปิดทุก source) → ควรเพิ่ม package ให้น้อย

## Option matrix

คะแนน 1–5 (5 = ดีที่สุดสำหรับโปรเจกต์นี้) ประสิทธิภาพเป็นการประเมินจากลักษณะ library ไม่ได้ benchmark ในเครื่องนี้

| เกณฑ์ (น้ำหนัก) | Npgsql ตรง (ADO.NET) | Dapper | **EF Core + Npgsql provider** | linq2db | RepoDb |
|---|---|---|---|---|---|
| **ป้องกัน SQL injection โดยโครงสร้าง** (×3) | 2 — ปลอดภัยถ้าใช้ parameter ทุกครั้ง แต่ SQL เป็น string ล้วน พลาดได้ง่ายตอนทำ filter/sort แบบ dynamic | 3 — parameter ผ่าน object สะดวก แต่ยังเขียน SQL string เอง dynamic `WHERE`/`ORDER BY` ต้องระวังเอง | **5** — LINQ สร้าง parameter เสมอ; `FromSql($"...")` แปลง interpolation เป็น parameter; มี analyzer EF1002 เตือนเมื่อใช้ raw SQL กับ string interpolation (repo ตั้ง `TreatWarningsAsErrors` จึงกลายเป็น error) | 5 — LINQ สร้าง parameter | 3 — มี expression API แต่หลายส่วนรับ string |
| **Migration ในตัว** (×2) | 1 — ไม่มี ต้องเขียน runner เอง | 1 — ไม่มี (ต้องเพิ่ม DbUp/FluentMigrator) | **5** — EF Migrations: สร้างจาก model, review เป็นโค้ด, export เป็น `.sql` idempotent script ได้ | 2 — ไม่มีในตัว ต้องใช้ FluentMigrator | 1 — ไม่มี |
| **Concurrency + Transaction + Audit** (×2) | 2 — ทำเองทั้งหมด | 2 — ทำเอง | **5** — concurrency token (`version`/`xmin`) ในตัว, `SaveChanges` เป็น transaction เดียว, interceptor ทำ audit อัตโนมัติได้ | 3 — มี transaction, concurrency ทำเอง | 3 |
| **ประสิทธิภาพ** (×2) | 5 | 5 — ใกล้ ADO.NET | 4 — ช้ากว่า Dapper เล็กน้อย; ใช้ `AsNoTracking`, compiled queries, `ExecuteUpdate` และ DbContext pooling ลดส่วนต่าง; เพียงพอเพราะไม่ใช่ per-packet | 5 | 4 |
| **รองรับ PostgreSQL** (bytea, jsonb, citext, inet, LISTEN/NOTIFY, COPY) (×1) | 5 | 3 — jsonb ต้องเขียน type handler | 5 — provider ของทีม Npgsql เอง map jsonb/bytea/citext/inet ได้; LISTEN/NOTIFY และ COPY ใช้ `NpgsqlDataSource` เดียวกัน | 4 | 3 |
| **จำนวน runtime package** (×1) | 5 — 1 ตัว | 4 — 2 ตัว | 3 — 6 ตัว (EF Core 4 + Npgsql 2); ชุด design-time ~20 ตัวแยกไว้ใน tool project ไม่ ship | 4 — 2 ตัว | 4 |
| **ความยั่งยืน / community / เอกสาร** (×1) | 5 | 5 | 5 — Microsoft ดูแล, LTS ตาม .NET 10 | 3 | 2 — พัฒนาช้าลง |
| **Fit กับ Admin Portal (Blazor, query dynamic)** (×1) | 2 | 3 | 5 — `IQueryable` ต่อ filter/paging ได้ตรง ๆ | 4 | 3 |
| **คะแนนรวมถ่วงน้ำหนัก** (เต็ม 65) | 39 | 40 | **61** | 50 | 37 |

## การตัดสินใจ

เลือก **EF Core 10 + `Npgsql.EntityFrameworkCore.PostgreSQL`** สำหรับ `C9.GameData`

เหตุผลหลัก:

1. **Injection safety by construction** — query ทั้งหมดเป็น LINQ จึงเป็น parameterized โดยไม่ต้องพึ่งวินัยของผู้เขียน
   เหมาะกับ Admin Portal ที่มี filter/sort แบบ dynamic มากที่สุด
2. **Migration, concurrency token และ transaction อยู่ในตัว** — ตรงกับความต้องการ "แก้ตัวละครพร้อม audit และ ledger
   ใน transaction เดียว" และ "Portal ห้ามเขียนทับ server" โดยไม่ต้องเขียนกลไกเอง
3. **ประสิทธิภาพพอสำหรับ game server** เพราะ DB ไม่อยู่บน per-packet path; จุดที่ต้องเร็ว (ledger จำนวนมาก, LISTEN/NOTIFY)
   ใช้ Npgsql ตัวเดียวกันโดยตรงได้
4. **Dapper** ชนะเรื่องความเร็วและขนาด แต่ไม่ช่วยป้องกัน injection เกินกว่าการ bind parameter และไม่มี migration
   ต้องเพิ่ม library อื่นอีก → ไม่ตรงคำขอ "ORM ที่มี security เรื่อง injection"
5. **linq2db** เป็นตัวสำรองที่ดี (LINQ + เร็ว) แต่ไม่มี migration ในตัวและ community เล็กกว่า

## กฎการใช้งาน (บังคับใน code review)

- ห้ามใช้ `FromSqlRaw` / `ExecuteSqlRaw` / `SqlQueryRaw` กับ string ที่ประกอบจาก input; ใช้ `FromSql($"...")` /
  `ExecuteSql($"...")` (interpolation → parameter) เท่านั้น; ไม่ปิด analyzer EF1002
- การเรียง/กรองแบบ dynamic จาก UI ต้อง map ผ่าน whitelist (enum → expression) ห้ามส่งชื่อ column จาก client
- `Npgsql` ตรงใช้เฉพาะ LISTEN/NOTIFY และ COPY (binary import) ซึ่งไม่มี SQL จาก input
- Query ฝั่ง game server ใช้ `AsNoTracking` และ compiled query; ใช้ `PooledDbContextFactory`
- Migration ทุกตัวต้อง export เป็น idempotent `.sql` (`dotnet ef migrations script --idempotent`) เก็บใน repo
  เพื่อ review และใช้ deploy ใน Docker ได้โดยไม่ต้องมี SDK

## Packages ที่เพิ่ม (ผ่าน `packageSourceMapping` จำกัดเฉพาะชื่อเหล่านี้)

Runtime (`src/C9.GameData`):
`Npgsql.EntityFrameworkCore.PostgreSQL`, `Npgsql`, `Microsoft.EntityFrameworkCore`,
`Microsoft.EntityFrameworkCore.Relational`, `Microsoft.EntityFrameworkCore.Abstractions`, `Microsoft.EntityFrameworkCore.Analyzers`
(ส่วน `Microsoft.Extensions.*` มาจาก shared framework `Microsoft.AspNetCore.App`)

Design-time เท่านั้น (`tools/C9.GameData.Design`, ไม่ถูก reference จาก server/portal):
`Microsoft.EntityFrameworkCore.Design` และ dependency ของมัน (Roslyn, Humanizer, Mono.TextTemplating ฯลฯ)
ใช้เฉพาะตอนสร้าง migration

## ผลที่ตามมา

- `NuGet.Config` ไม่ได้เป็น "no external packages" อีกต่อไป แต่จำกัดด้วย source mapping
- Docker image ของ server/portal ใหญ่ขึ้นประมาณขนาด EF Core + Npgsql (ไม่กี่ MB)
- ถ้าวัดผลภายหลังพบว่า query ใดของ game server ช้า สามารถเขียนเฉพาะจุดนั้นด้วย `FromSql($"...")` หรือ Npgsql
  ตรงได้ โดยไม่ต้องเปลี่ยน library
