# C9 RE Phase 4 — Server implementation บน macOS

วันที่ 2 พฤศจิกายน 2026 · ต่อจาก Static Phase 3

## ผลที่ส่งมอบ

มี server ต้นแบบที่รันบน Mac ได้แล้ว พร้อม TCP transport, codec ช่วงเริ่มต้น, PacketTwister 80 selector และ fixture response rules ทดสอบการรับ login fixture ที่เข้ารหัสแล้วตอบ server-list fixture ผ่าน TCP loopback สำเร็จ ทั้งใน integration tests และการเปิด server CLI เป็น process แยก

ยังไม่ได้ยืนยันว่าเกมจริงล็อกอินหรือแสดง server list ได้ การตอบใน fixture profile เป็นข้อมูลทดสอบที่สร้างขึ้นเพื่อทดสอบ transport/schema ไม่ใช่ authentication ที่พิสูจน์กับเกมแล้ว และยังไม่มีระบบตัวละคร โลกเกม ฐานข้อมูล หรือ admin panel

ผู้ใช้อนุญาตให้ทำ RE และ implementation ต่อจนถึง server ได้แล้ว ส่วนการเปิด C9.exe, Launcher, Patcher หรือเปิดเกมเพื่อ debug ยังต้องรออนุมัติ รอบนี้ไม่ได้เปิดโปรแกรมเหล่านั้น ใช้ headless analysis และรันเฉพาะโค้ดที่เขียนขึ้นใหม่บน Mac

## วิธีรัน

เปิด terminal ที่ project directory `C9 Server` แล้วใช้ Python ที่ติดตั้งไว้:

```bash
# Passive endpoint: ตรวจ/รับ packet และบันทึก metadata
/opt/homebrew/bin/python3 -m c9server

# Scenario ที่ตอบด้วยข้อมูลทดสอบอย่างชัดเจน
/opt/homebrew/bin/python3 -m c9server --config config/lab-fixture.json

# เปลี่ยน port หรือให้ระบบเลือก port ว่าง
/opt/homebrew/bin/python3 -m c9server --config config/lab-fixture.json --port 0
```

ค่าเริ่มต้นคือ `127.0.0.1:14000` และรับเฉพาะ loopback bind address หยุดด้วย Ctrl-C ไม่ต้องติดตั้ง Python package เพิ่มบน Mac ที่ทดสอบ โดย Python/OpenSSL ต้องมี RIPEMD160 ซึ่ง server ตรวจให้ก่อนเปิด socket

หลังทดสอบ server ถูกหยุดเรียบร้อยแล้ว ไม่มี listener ที่ตั้งใจเปิดค้างไว้จากงานรอบนี้ ผู้ใช้ใช้เมาส์/คีย์บอร์ดทำงานร่วมกับการวิเคราะห์รอบนี้ได้ตามปกติ

## สิ่งที่ implement แล้ว

| ส่วน | พฤติกรรมและขอบเขต |
|---|---|
| TCP | อ่าน length ก่อน body, จำกัดขนาด/connection/packet, timeout และ per-connection state |
| Header/checksum | decoded header 13 ไบต์, signed-byte checksum, field +5 เป็น 100 ระหว่างคำนวณ |
| PacketTwister | encode/decode ครบ 80 selector ที่ wrapper ปัจจุบันเข้าถึง แยก length branches |
| Initial outer cipher | seed คงที่จาก PE → RIPEMD160 → factory branch 5 → RC6 key 16 ไบต์ |
| Outer wrapper | block ขนาด 16, zero fill และ byte ท้ายระบุจำนวน padding |
| Sequence | แยกส่ง/รับ; flag=0 ไม่เพิ่มลำดับของ encrypted frames |
| Client payload adjustment | ทดลอง selector ภายใน 80 ค่าและรับเฉพาะ plaintext ที่ checksum ผ่านแบบไม่กำกวม |
| Server list | builder/parser ของ count และ opaque entry 52 ไบต์ โดยไม่เดา encoding ของชื่อ |
| Response rules | ตรวจ state, numeric type และ payload length ก่อนส่ง fixture replies |
| Log | บันทึก type/length/state/sequence โดยไม่บันทึก payload หรือ credentials |

ไฟล์หลักอยู่ใน `c9server/`, config ที่ `config/lab-fixture.json`, tests ที่ `tests/` และคู่มือเริ่มงานที่ `README.md` โค้ด runtime ไม่ต้องอ่านหรือโหลด Windows PE

## ผล RE ที่ทำให้เขียน codec เริ่มต้นได้

`00DCECB0` ส่ง seed ที่ VA `012FC7F4` ความยาว 32 ไบต์จาก `012FC818` เข้า key factory ของ state ทั้งสองทิศทาง เมื่อไล่ hash switch และแก้ขอบเขตฟังก์ชันในฐานวิเคราะห์ พบ branch 7 ซึ่งผูกกับ RIPEMD160 ผ่าน vtable/RTTI

digest ที่คำนวณได้คือ `6f749a998ed7373b8413712b6b3a201381850d31` ใช้ digest byte 5 modulo 10 ได้ค่า 5 จึงเลือก RC6 ตาม `00DD5070` ส่วน `00DD3060` ใช้ key 16 ไบต์แรกและตั้ง property เป็น `0x100` หรือ 256

ตรวจการส่ง digest จาก assembly ที่ `00DD57BD..00DD57EC` โดยนับผลของ PUSH ก่อน LEA แล้ว ไม่อาศัยชื่อ stack variables จาก decompiler อย่างเดียว ตรวจ key schedule/round functions ที่ `00E6F950`, `00E6FB30`, `00E6FC30` และ wrapper ที่ `00DD3990`/`00DD30A0` เพิ่มด้วย

implementation รองรับ 256 encrypted frames ต่อทิศทางใน initial epoch หลังจากนั้นจะปฏิเสธการใช้ codec ต่อ เพราะยังไม่ implement key rotation และ cipher อื่นครบ server ตั้ง packet/reply budget ให้อยู่ภายในขอบเขตนี้

รายละเอียด provenance อยู่ใน `docs/PROTOCOL_STATUS.md` และ `re/evidence/static/phase4_initial_cipher.json`

## การกู้ PacketTwister และการตรวจสอบ

export encode/decode รวม 160 ฟังก์ชันของ 80 selector จาก static analysis แล้วแปลงเป็นตาราง permutation ของ bit และ XOR ที่ใช้ได้โดยไม่พึ่ง native library ตอนรัน server

ขั้นตอนสร้างตาราง compile เฉพาะ C ที่ถอดจาก pseudocode โดยปรับ pointer width และ unsigned byte shifts ให้ตรง host จากนั้นตรวจ basis vectors, ความเป็น bijection, คู่ inverse และ random input ที่ length boundaries หากไม่ตรงจะหยุดสร้างตาราง ไม่ได้โหลดหรือ execute routine จาก C9.exe ต้นฉบับ

ผลที่บันทึก: 80 selector, 160 ทิศทาง, 640 length branches และ 67,520 differential/inverse basis checks ผ่านทั้งหมด นอกจากนี้ยังมี basis vectors ที่ใช้ระหว่าง extraction ตัวเลขนี้เป็นการตรวจ recovered implementation ไม่ใช่จำนวน packet จริงจากเกม

ไฟล์ที่เกี่ยวข้อง: `build_twister_tables.py`, `phase4_all_twisters.c/.asm.txt`, `phase4_twisters_reference.c`, `phase4_twister_validation.json` และ runtime table `c9server/data/twisters.json`

## ผลทดสอบ

| ชุดตรวจ | ผล |
|---|---|
| Protocol/server unittest | 17/17 ผ่าน |
| Offline unittest เดิม หลังใช้ protocol ร่วมกับ server | 8/8 ผ่าน |
| RC6 primitive known-answer vectors | 6 ชุดผ่าน อยู่ภายใน unittest ข้างต้น |
| CLI เป็น process แยกและ TCP client จำลอง | ผ่าน login fixture → server-list fixture, exit code 0 |
| Hash baseline ที่เลือกไว้ | 38/38 ไฟล์ตรงกับ manifest Phase 2 |

integration tests ครอบคลุม length prefix/body ที่แบ่งหลายครั้ง, สอง request ติดกัน, encrypted replies, reconnect, state ผิด, sequence ผิด, ciphertext เสีย, footer/length ผิด, connection limit, truncation และ timeout รวมถึงไม่ใส่ข้อมูล credentials ใน event log

RC6 known-answer vectors มาจาก [Crypto++ TestData/rc6val.dat](https://github.com/weidai11/cryptopp/blob/master/TestData/rc6val.dat) จึงช่วยตรวจ primitive กับหลักฐานอิสระจาก implementation ใหม่นี้ ส่วน C9 wire compatibility ยังไม่มี captured fixture จาก Client มายืนยัน

```bash
/opt/homebrew/bin/python3 -m unittest discover -s tests -v
/opt/homebrew/bin/python3 -m unittest discover -s re/scripts -p test_c9_protocol_offline.py -v
```

ผล smoke test และขอบเขตการทดสอบอยู่ใน `re/evidence/static/phase4_server_validation.json` ส่วน hash ต้นฉบับอยู่ใน `phase4_source_integrity.json` ขอบเขต baseline 38 ไฟล์ไม่รวม asset ทั้งหมด

## ช่องว่างที่ต้องทำต่อ

1. Implement subsequent key rotation และ cipher/hash paths ที่เหลือ พร้อม fixture อ้างอิงอิสระ
2. ตรวจความหมาย auth fields, ค่า response ที่ถูกต้อง และลำดับ handshake; fixture profile ยังไม่พิสูจน์ส่วนนี้
3. ตรวจ encoding/field layout ของ server entry และ session handoff; ยังไม่มี character/world service
4. ตรวจ BaseCat/envelope path แยกจาก TCP ที่ implement รอบนี้
5. เมื่อผู้ใช้อนุมัติเปิดเกม ให้เก็บ initial plaintext/wire traces และตรวจ server กับ Client ชุด hash เดียวกัน จึงประเมิน M4 acceptance ว่าเกมแสดง server list ได้จริง

งาน implementation/test ที่ทำได้โดยไม่เปิดเกมได้รับอนุญาตแล้ว การเปิดเกมจริงยังคงรอผู้ใช้อนุมัติ และ M4 acceptance ยังไม่ถือว่าผ่านด้วย client จำลองแทน
