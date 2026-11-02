# C9 RE Phase 3 — Cipher catalog และเครื่องมือตรวจข้อมูล offline

วันที่ 2 พฤศจิกายน 2026 · ต่อจาก Phase 2 · ทำบน macOS แบบ headless

## ผลที่ได้

ผูก factory ของ outer cipher กับ constructor, vtable และ RTTI ได้ 16 คลาส; แยกจาก PacketTwister ซึ่งมีการลงทะเบียน 100 ตัว แต่ wrapper ที่ตรวจอยู่เลือกเพียง 80 ค่าแรก เพิ่มหลักฐานโครงสร้างรายการ server และสร้างตัวอ่าน frame/checksum แบบ offline พร้อมการทดสอบขอบเขตข้อมูล 8 กรณี

ทั้งหมดเป็น static analysis และการรันสคริปต์ที่เขียนใหม่ ไม่ได้เริ่ม C9.exe, Launcher, Patcher หรือ VM ไม่ได้เชื่อมต่อเซิร์ฟเวอร์เกม และยังไม่มี runtime trace หรือหลักฐานว่าเซิร์ฟเวอร์ใหม่สื่อสารกับ Client ได้

## ขอบเขตและการใช้คอมพิวเตอร์ร่วมกัน

รอบนี้ใช้ Ghidra headless และ Python อ่านไฟล์ ไม่ใช้เมาส์ คีย์บอร์ด หรือเปลี่ยนหน้าต่างของผู้ใช้ ผู้ใช้ทำงานบนคอมพิวเตอร์พร้อมกันได้ งานวิเคราะห์อาจใช้ CPU/RAM เพิ่มขึ้น โดยตั้ง Ghidra สูงสุด 2 CPU และ Java heap 2 GiB หากต้องใช้ Computer Use กับหน้าต่างในรอบถัดไป ให้แจ้งผู้ใช้ก่อน

ข้อจำกัดเดิมยังมีผล: ห้ามเริ่มเกม, Launcher หรือ Patcher แม้ในเบื้องหลัง จนกว่าผู้ใช้จะแจ้งว่าเปิดเพื่อ debug ได้

ชุดอ้างอิงคือ `52C9GL/C9.exe` SHA-256:

`b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25`

ตรวจไฟล์ baseline ที่เลือกไว้ 38 ไฟล์กับ manifest ของ Phase 2 แล้ว hash ตรงทั้งหมด รายการนี้ไม่ครอบคลุม asset ทุกไฟล์ หลักฐานอยู่ใน `re/evidence/static/phase3_source_integrity.json`

## 1. Outer cipher: factory ที่ผูกชื่อคลาสได้แล้ว

`0x00DD5070` เลือกด้วย `selector % 10` จากนั้นเรียก constructor และ virtual method offset `+4` พร้อม pointer/length ของข้อมูลตั้งค่า ตารางนี้ผูกชื่อจาก RTTI ของ vtable ที่ constructor เขียนลง object โดยตรง จึงมีหลักฐานมากกว่าการพบชื่อ algorithm ใน strings อย่างเดียว

| selector % 10 | เงื่อนไขย่อย | คลาส CryptoPP ใน RTTI | Constructor VA |
|---:|---|---|---|
| 0 | — | TEA | 00DD3680 |
| 1 | — | ThreeWay | 00DD2880 |
| 2 | — | CAST128 | 00DD2C10 |
| 3 | default branch | GOST | 00DD3430 |
| 4 | — | RC5 | 00DD2EA0 |
| 5 | — | RC6 | 00DD3000 |
| 6 | — | MARS | 00DD31A0 |
| 7 | — | IDEA | 00DD0C30 |
| 8 | — | GOST | 00DD3430 |
| 9 | key[0] & 7 = 0 | Twofish | 00DD38D0 |
| 9 | key[0] & 7 = 1 | DES | 00DD0EC0 |
| 9 | key[0] & 7 = 2 | DES_EDE2 | 00DD1150 |
| 9 | key[0] & 7 = 3 | DES_EDE3 | 00DD1320 |
| 9 | key[0] & 7 = 4 | DES_XEX3 | 00DD1630 |
| 9 | key[0] & 7 = 5 | SKIPJACK | 00DD3BC0 |
| 9 | key[0] & 7 = 6 | SAFER_K | 00DD4BB0 |
| 9 | key[0] & 7 = 7 | SAFER_SK | 00DD4E90 |

รวม 17 ทางเลือกและ 16 คลาส เพราะ selector 3 กับ 8 ใช้ GOST เหมือนกัน ยังไม่ใช่ข้อยืนยันว่า session จริงจะเรียกครบทุก algorithm

ในเส้นทางตั้งค่า `0x00DD5470` ผูก hash vtable กับ RTTI ได้ 9 คลาส: SHA256, SHA512, SHA384, MD4, MD5, RIPEMD160, RIPEMD320, RIPEMD128 และ RIPEMD256 มีรายละเอียด branch ใน `phase3_cipher_catalog.json` ยังต้องตรวจ stack layout และการส่ง digest ไปเป็น key จาก assembly ให้ครบก่อนเขียน key derivation ใหม่

virtual encode ที่ `0x00DD1400`, `0x00DD3990`, `0x00DD2970` มีการแบ่ง block 8, 16 และ 12 ไบต์ตามลำดับ พร้อมการเติมศูนย์และ byte ท้ายบอกจำนวนที่เติม จึงไม่ควรนำ padding สำเร็จรูปมาใช้โดยสมมติว่าเท่ากัน ต้องตรวจความยาวและคู่ decode ให้ครบก่อนกำหนดรูปแบบที่ใช้จริงบน wire รายงานนี้ยังไม่ตั้งชื่อ mode หรือสรุป IV/key schedule

หลักฐาน: `phase3_cipher_factory.c/.asm.txt`, `phase3_cipher_constructors.c/.asm.txt`, `phase3_payload_samples.c/.asm.txt`, `phase2_codec_state.c/.asm.txt` และ RTTI จาก PE

## 2. PacketTwister: ลงทะเบียน 100 ตัว แต่ wrapper ที่พบใช้ modulo 80

initializer `0x00410AE0` สร้าง object และลงทะเบียน key 0–99 จับคู่กับ RTTI `PacketTwister_1` ถึง `PacketTwister_100` ได้ครบ ไม่มี key ขาดหรือซ้ำใน extraction รอบนี้

ส่วน `0x00412130`, `0x004121D0` และ `0x00412230` ลด selector ด้วย `% 0x50` หรือ 80 ก่อน lookup ดังนั้นเส้นทางเหล่านี้เข้าถึง key 0–79 ส่วน key 80–99 ยังไม่ควรเรียกว่า dead code เพราะยังไม่ได้ตรวจ caller อื่นทั้งหมด

| รายการตัวอย่าง | vtable | encode (+4) | decode (+8) |
|---|---|---|---|
| PacketTwister_1 / key 0 | 0100B354 | 00564CC0 | 00565310 |
| PacketTwister_2 / key 1 | 0100B374 | 00565960 | 005660F0 |

ตัวอย่างที่ export มีการแบ่งตามความยาว payload เป็น ≤3, 4–7, 8–15, 16–31 และ ≥32 ไบต์ ใช้การหมุน bit, XOR, สลับ byte และสลับ bit ภายใน byte ไม่ใช่ตัวเดียวกับ outer cipher ขณะนี้ยังไม่ได้เขียน implementation ใหม่ครบ 80 ตัวหรือพิสูจน์ inverse ทุก branch

`phase3_payload_registry.json` เก็บ key, RTTI, locator, vtable, encode/decode addresses และขอบเขตการเข้าถึงผ่าน wrapper ที่พบ สคริปต์ `c9_phase3_catalog.py` สร้างตารางซ้ำจาก PE และ Ghidra exports พร้อมตรวจ hash ของ Client, ลำดับ key, ชื่อ RTTI และว่าที่อยู่ใน vtable ชี้เข้า code section

## 3. โครงสร้างรายการ server ที่ยืนยันเพิ่ม

เส้นทาง `C9NetworkLogin::handler` ที่ `0x00A621F0` รับ decoded type `0x2EE6` โดยใช้ count หนึ่งไบต์ที่ offset 13 และ entry แรกที่ offset 14, stride 52 ไบต์

ยืนยันการคัดลอก entry จาก `00A64680 → 00A64960 → 00A65430 → 00A655E0 → 00614CD0 → 00614D40` โดยปลายทางคัดลอก 13 DWORD หรือ 52 ไบต์ ทำให้มีหลักฐานรองรับขนาด entry นอกจากค่า stride ใน handler

| ตำแหน่งใน entry | สิ่งที่ทราบจากโค้ด | ข้อจำกัด |
|---|---|---|
| +0 | ถูกคัดลอกเก็บไว้ | ความหมายยังไม่ทราบ |
| +1..+4 | `00A63630` อ่านเป็น DWORD เพื่อเทียบ lookup key | ใช้ชื่อเชิงโครงสร้าง ยังไม่ยืนยันชื่อ field ต้นฉบับ |
| +5 | handler ส่ง pointer ตำแหน่งนี้เข้าเส้นทางแปลงข้อความ | encoding, ความจุ และจุดจบ field ยังไม่ยืนยัน |
| ทั้ง entry | คัดลอก 52 ไบต์ | ยังไม่ได้แยกทุก field ภายใน |

decoded type `0x2EED` ใช้ DWORD ที่ packet +21 ในการค้น entry ผ่าน `00A63630` และมีเส้นทางส่ง host/port จาก packet +26/+30 ไป `00A68D40 → 00563820` สอดคล้องกับบทบาท handoff จากหลักฐาน static แต่ลำดับ handshake จริงและค่าที่ต้องตอบยังรอการตรวจ runtime

## 4. เครื่องมือตรวจข้อมูล offline ที่เพิ่ม

`re/scripts/c9_protocol_offline.py` มีสองโหมดที่ผู้เรียกต้องระบุชัดเจน:

- `wire`: แบ่ง frame ตาม length สองไบต์แรก เก็บ raw bytes และ flag โดยไม่ตีความ type/checksum/sequence ของข้อมูลที่ยังเข้ารหัส
- `decoded`: อ่าน header 13 ไบต์จากข้อมูลที่ถอด transform มาแล้ว ตรวจ length และโครงสร้าง `0x2EE6` เก็บ raw entry/ข้อมูลท้ายไว้ครบ ไม่เดา encoding ของชื่อ

checksum ใช้ signed byte และ uint32 wrap ตาม assembly รวมการตั้ง field +5 เป็น 100 ก่อนคำนวณช่วง +3 ถึงจบ frame การตรวจ checksum เป็นตัวเลือกเฉพาะ decoded frame ที่ flag ไม่เป็นศูนย์ ไม่บังคับใช้กับ flag=0 ซึ่งมีเส้นทางต่างกัน

ตัวอย่างการใช้จาก root ของ repository:

```bash
/opt/homebrew/bin/python3 re/scripts/c9_protocol_offline.py capture.bin --input-kind wire
/opt/homebrew/bin/python3 re/scripts/c9_protocol_offline.py decoded.bin --input-kind decoded --verify-checksum
/opt/homebrew/bin/python3 re/scripts/c9_phase3_catalog.py
/opt/homebrew/bin/python3 -m unittest discover -s re/scripts -p test_c9_protocol_offline.py -v
```

ชื่อ `capture.bin` และ `decoded.bin` ในตัวอย่างเป็น placeholder ยังไม่มี capture จริง เครื่องมือนี้ไม่ทำ outer cipher, PacketTwister, state/sequence tracking หรือ network communication

## 5. การตรวจสอบและสิ่งที่ยังขาด

การทดสอบ synthetic 8 กรณีผ่านทั้งหมด: checksum arithmetic ที่มี high bit, ทุกตำแหน่งแบ่ง stream ของสอง frame, หลาย frame ติดกัน, length ผิด/ไม่ครบ, การเก็บ wire bytes, decoded length, ขอบเขต checksum และ count/entry ของ server list รวมการเก็บ trailing bytes

literal checksum vectors มาจากสูตรที่กู้ได้ เป็นการตรวจ implementation ตามข้อกำหนด static ยังไม่ใช่ golden vectors ที่เก็บจาก Client การผ่านการทดสอบเหล่านี้จึงไม่ทำให้ M2 หรือ M4 ผ่าน

ยังขาด:

1. key derivation และ state update ที่ตรวจ assembly ครบ พร้อมชุด input/output อ้างอิงอิสระ
2. PacketTwister ทั้ง 80 ตัวที่เส้นทางปัจจุบันเข้าถึง และข้อแตกต่าง selector ฝั่งส่ง/รับ
3. field layout/encoding ของ server entry และเงื่อนไข handshake ที่จำเป็นครบ
4. runtime ของ Client ชุดเดียวกัน เพื่อยืนยัน transport, codec, sequence และลำดับข้อความจริง

ขั้นถัดไปที่ทำได้โดยไม่เปิดเกมคือทำตาราง operation ของ PacketTwister ทีละ length branch, ตรวจ key derivation จาก assembly และไล่ consumer ของ server entry ต่อ เมื่อผู้ใช้อนุญาต debug จึงเก็บ runtime fixtures แล้วนำมาตรวจเครื่องมือ offline และ codec ที่เขียนใหม่
