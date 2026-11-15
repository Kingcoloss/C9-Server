# ADR 0003: Server ใหม่เป็น Web Admin Portal + game agent และรันด้วย Docker Compose

วันที่: 2026-11-15 (Asia/Bangkok)
สถานะ: **Accepted (implemented)** — รอผู้ใช้ทดสอบบน Windows + Docker Desktop
เกี่ยวข้อง: [SERVER_PLATFORM_ARCHITECTURE_TH.md](../SERVER_PLATFORM_ARCHITECTURE_TH.md), [ADR 0001](0001-server-stack.md), [ADR 0002](0002-game-data-access.md)

## โจทย์

ผู้ใช้ต้องการเลิกใช้ server manager แบบ WinForms/MAUI แล้วใช้ Web Application ที่รันด้วย Docker Compose พร้อม
Server Admin Portal สำหรับจัดการและเฝ้าดู server (dashboard ใช้ Grafana ฟรีได้) และ trace packet ระหว่าง client กับ server
เพื่อวิเคราะห์ error และต้องเป็นฐานให้โมดูล Game Account / Game World (งาน 3.2/3.3) ต่อยอดได้

ข้อจำกัด: client (C9.exe) ต่อ 127.0.0.1:14000 และ handoff ชี้ 127.0.0.1:14001 แบบตายตัว; repo ไม่ใช้ package ภายนอก
ยกเว้นที่ ADR 0002 อนุญาต; ต้องไม่ทำให้วิธีรันเดิม (L9, Admin Console) พัง

## ทางเลือกที่พิจารณา

| ทางเลือก | ข้อดี | ข้อเสีย | ผล |
|---|---|---|---|
| A. Portal ฝัง game listener ไว้ใน process เดียว | ง่าย | Portal ล่ม/อัปเดตแล้วเกมหลุด, scale/แยกสิทธิ์ไม่ได้ | ไม่เลือก |
| **B. game service แยก process + HTTP agent ภายใน, Portal คุยผ่าน agent** | แยกความรับผิดชอบ, Portal restart ได้โดยเกมไม่หลุด, สัญญา API ชัด, ทดสอบแยกได้ | ต้องดูแล token และสัญญา 2 ชั้น | **เลือก** |
| C. ใช้ Docker API สั่ง start/stop container จาก Portal | ไม่ต้องมี agent | Portal ต้องได้ docker.sock (สิทธิ์ระดับ root ของเครื่อง), ไม่ได้ข้อมูล session/packet | ไม่เลือก |
| UI: Razor Pages + JS | ไม่ต้องใช้ package | live view (trace tail, dashboard) ต้องเขียน JS เอง | ไม่เลือก |
| **UI: Blazor Web App (interactive server)** | live update ผ่าน SignalR ในตัว, C# ทั้งระบบ, โมดูลเพิ่มหน้าเองได้ | .NET 10 ต้องใช้ package `Microsoft.AspNetCore.App.Internal.Assets` (ไฟล์ JS ของ framework) | **เลือก** |
| Logs: Promtail | คุ้นเคย | EOL แล้ว | ไม่เลือก |
| **Logs: Grafana Alloy → Loki** | ตัวที่ Grafana แนะนำแทน Promtail | ต้องใช้ docker.sock อ่าน log | **เลือก** (บันทึกเป็นความเสี่ยง R-09) |

## การตัดสินใจ

1. `C9.Server --agent-url` เปิด agent HTTP (Kestrel) ข้าง game listener: health, metrics (Prometheus text เขียนเอง ไม่ใช้ package),
   control, sessions, events, packet trace, export และ SSE stream ทุก path ใต้ `/agent/v1` ต้องใช้ Bearer token จากไฟล์
2. Admin Portal (`src/C9.Portal`) = Blazor Web App + REST `/api/v1` + RBAC + audit; โมดูลเสียบผ่าน `IPortalModule`
   ใน `C9.Portal.Abstractions` (ไม่ต้องแก้ host) และ host ลงทะเบียน `C9.GameData` เมื่อมี `C9_DB_CONNECTION`
3. Docker Compose: image เดียว (`c9-platform:local`) ใช้ทั้ง game service และ Portal; ทุก port publish ที่ 127.0.0.1;
   state ทั้งหมดอยู่ใน bind mount `artifacts/docker-state`; observability ใช้ Prometheus v3.5.0, Loki 3.5.5, Alloy v1.10.2,
   Grafana OSS 12.1.1 (ตรึงเวอร์ชัน); PostgreSQL 18 อยู่ใน profile `gamedata`
4. อนุญาต package `Microsoft.AspNetCore.App.Internal.Assets` ใน `NuGet.Config` (packageSourceMapping) — เป็น package
   ของ Microsoft ที่ SDK อ้างอิงเองเมื่อใช้ Razor components และมีแต่ไฟล์ JS ของ Blazor
5. การ bind 0.0.0.0 (ทั้ง game listener และ agent) ทำได้เฉพาะเมื่อสั่ง `--allow-container-bind` ซึ่งใช้ใน compose เท่านั้น

## ผลที่ตามมา

- วิธีรันเดิมยังใช้ได้ (ไม่ใส่ `--agent-url` = พฤติกรรมเดิมทุกอย่าง) จึงย้ายแบบค่อยเป็นค่อยไปได้
- ต้องมี Docker Desktop บน Windows; ถ้าไม่มี ยังรัน Portal + agent ตรงบนเครื่องได้ (ดู R-07)
- IP ของ client ที่ server เห็นเป็น gateway ของ Docker (ข้อจำกัด R-06)
- WinForms/MAUI server manager และ `C9.Admin` เดิมจะเลิกใช้หลังผู้ใช้ยืนยันว่า Portal ใช้แทนได้ครบ
