# C9 Client — ผล Reverse Engineering รอบแรก

วันที่ 2 พฤศจิกายน 2026 (Asia/Bangkok) · วิเคราะห์บน macOS ARM64

ผลหลัก: ระบุเส้นทาง TCP และ BaseCat ของ Client ได้แล้ว พบโครงสร้าง frame พื้นฐาน 13 ไบต์ ฟิลด์ความยาว การตรวจ checksum และตำแหน่งโค้ดแปลง payload ที่ใช้เป็นจุดเริ่มต้นพัฒนาโปรโตคอลได้ ยังไม่ได้ยืนยัน login opcode หรือ handshake ด้วยการรัน Client และจับแพ็กเก็ตจริง

## 1. ขอบเขตและสถานะ

- เปิดและสแกน `52C9GL/C9.exe` ผ่าน Computer Use ใน Detect It Easy บน Mac แล้ว
- ใช้ Ghidra ที่ติดตั้งในโปรเจกต์ วิเคราะห์สำเนาฐานข้อมูลใน `re/evidence/` และ export โค้ดกับ assembly
- คอมไพล์ native decompiler ARM64 จากซอร์สที่มากับ Ghidra สำเร็จ แล้วติดตั้งไว้ใน `ghidra_12.2_PUBLIC/Ghidra/Features/Decompiler/build/os/mac_arm_64/decompile`
- Export ฟังก์ชันที่เกี่ยวข้อง 82 ฟังก์ชัน และ export กลุ่ม packet framing / transform เพิ่มพร้อม assembly
- รอบ auto-analysis แรกหยุดที่ขีดจำกัด 180 วินาทีสำหรับ EXE และ 90 วินาทีสำหรับ loader; รอบ export หลังแก้ native decompiler สำเร็จ ไม่ใช่การรับรองว่า auto-analysis ทุกส่วนของโปรแกรมเสร็จครบ
- ยังไม่ได้รัน Client, เปลี่ยนปลายทางเครือข่าย, เชื่อมต่อเซิร์ฟเวอร์ภายนอก หรือจับแพ็กเก็ตด้วย Wireshark ในรอบนี้
- ตรวจ SHA-256 ก่อนและหลังแล้ว ไบนารีต้นฉบับ 7 ไฟล์ไม่เปลี่ยนแปลง ดู `evidence/static/source_integrity.json`

ข้อจำกัดของ UI: Computer Use เข้าถึง Ghidra ที่เปิดผ่าน Java โดยตรงไม่ได้ แม้พบโปรแกรมในรายการ จึงใช้ Ghidra headless สำหรับการวิเคราะห์และ export ส่วนปุ่ม Import ของ DiE ทำให้หน้าต่างปิดในรอบที่ทดลอง จึงเก็บรายงาน DiE ผ่าน `diec` เพิ่มเติม

## 2. ระบุตัว Client ให้ถูกก่อนพัฒนา

| รายการ | สิ่งที่ตรวจพบ | ความหมาย |
|---|---|---|
| Executable | `C9.exe`, 15,844,864 ไบต์ | Client หลักที่วิเคราะห์ |
| SHA-256 | `b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25` | ใช้ผูกผล RE กับไบนารีรุ่นนี้ |
| PE / CPU | PE32, I386, little-endian | x86 32-bit แม้ไฟล์ BAT มี argument `64bit` |
| Compiler signature | Microsoft Visual C/C++ 15.00.30729 / Visual Studio 2008 | ผลจาก DiE; เป็น signature ของเครื่องมือ build |
| Graphics | Direct3D 9 | สอดคล้องกับ dependencies ของ Client |
| Image base | `0x00400000` | ที่อยู่ VA ในรายงานอิงฐานนี้; runtime อาจต่างหากถูก relocate |
| Entry point | `0x00DD66C9` | จุดเข้า executable |
| `version.dat` | uint32 little-endian = `1017` | เวอร์ชันข้อมูล/patch ที่บันทึกในไฟล์นี้ |
| `VerCli.C9` | ASCII `4 685` | อีกตัวระบุเวอร์ชัน ไม่ควรรวมเป็นเลขเดียวกับ `version.dat` |
| `Version.cfs` | ASCII `202406070002` | อีกตัวระบุชุดข้อมูล |
| Build string ภายใน EXE | `Build Version = 49482` | ข้อความที่ฝังใน executable; ยังไม่ได้ยืนยันว่าเป็น protocol version |
| PDB path ภายใน EXE | `d:\Work\C9\CN\SRC\Client\Bin\C9.pdb` | เป็นข้อความ path สำหรับ debug ไม่ใช่ source code หรือไฟล์ PDB ที่มีอยู่ |

พบข้อมูลทั้ง CN, Thai และ Webzen ใน strings จึงยังสรุปไม่ได้ว่าชุดนี้เป็น Thailand legacy build ต้นฉบับจากชื่อโฟลเดอร์หรือชื่อโครงการเพียงอย่างเดียว PE timestamp สามารถถูกแก้ไขได้ จึงไม่ใช้เป็นหลักฐานยืนยันวัน build

## 3. เส้นทางเครือข่ายที่ตรวจพบ

Client import DLL 31 โมดูล ในจำนวนนั้นมี Winsock (`WS2_32.dll`) และ `BaseCat.Network.dll` รวมถึง WinINet / WinHTTP เส้นทาง HTTP อาจใช้กับบริการประกอบ จึงยังไม่ถือว่าเป็น game protocol

| VA ใน `C9.exe` | หน้าที่จากโค้ดที่อ่าน | หลักฐาน |
|---|---|---|
| `0x00563820` | สร้างและเริ่ม TCP connection | เรียก Winsock socket ด้วย `(2, 1, 6)`, เตรียม sockaddr และเรียก connect |
| `0x00563930` | ส่ง buffer จนครบ | loop เรียก `WSASend`, ชดเชยจำนวนไบต์ที่ส่งไปแล้ว |
| `0x00563AC0` | รับข้อมูลจาก Winsock | เรียก `WSARecv` แล้วส่งต่อไป `0x00563E40` |
| `0x00563E40` | ประกอบ frame จาก TCP chunks | สะสมข้อมูลที่ยังรับไม่ครบ; ใช้ฟังก์ชันตรวจ header และความยาว |
| `0x00665410` | เตรียม BaseCat connector และ callbacks | register on-connected / on-disconnected / on-received |
| `0x00665650` | เรียก BaseCat Connect พร้อม retry count | ส่ง host และ port เข้า `BaseCat::Network::Connector::Connect` |
| `0x00665BA0` | ส่งผ่าน BaseCat Link | เรียก `BaseCat::Network::Link::Send` |
| `0x00666260` | รับและอ่าน frame จาก BaseCat stream | อ่าน prefix ความยาว รอข้อมูลครบ แล้วตรวจ/แปลง frame |
| `0x00666890` | แปลงและตรวจ frame ในเส้นทาง BaseCat | transform, payload processing, checksum และฟิลด์ลำดับ |
| `0x00563D00` | แปลงและตรวจ frame ในเส้นทาง TCP | โครงสร้างคล้าย BaseCat แต่เรียก payload processor อีกเส้นทาง |
| `0x00561630` | ส่ง frame เข้าโครงสร้าง queue | เป็นขั้นถัดจากรับและตรวจ frame; ยังไม่ใช่ handler ของ login opcode |

Winsock หลายรายการถูก import ด้วย ordinal เช่น `Ordinal_4`, `Ordinal_16`, `Ordinal_19` และ `Ordinal_23` ใน output ของ Ghidra เก็บชื่อ ordinal เดิมไว้เพื่อหลีกเลี่ยงการตั้งชื่อที่กลบหลักฐานต้นฉบับ

Callback ที่พบใน `0x00665410`: on-connected = `0x006661D0`, on-disconnected = `0x00666220`, on-received = `0x00666260`

## 4. Frame header ที่อ่านได้จาก static analysis

โค้ดเริ่มรับ header ที่ 13 ไบต์ (`0x0D`) และตรวจว่าความยาวที่สะสมตรงกับ length ใน frame ข้อมูลหลายไบต์ถูกอ่านตรงด้วยคำสั่ง x86 จึงเป็น little-endian ในขั้นตอนที่โค้ดนี้ประมวลผล

| Offset | ขนาด | บทบาท | ระดับความมั่นใจ |
|---:|---:|---|---|
| `0` | 2 | ความยาว frame รวม | ยืนยันการอ่านและการเปรียบเทียบจาก assembly |
| `2` | 1 | flag ที่เลือกว่าจะเข้าขั้น transform หรือไม่ | ยืนยัน branch; ความหมายของแต่ละค่าต้องตรวจเพิ่ม |
| `3` | 2 | type / opcode candidate | โค้ด TCP คัดลอก ushort จากตำแหน่งนี้หลัง transform; ยังไม่ได้ map กับชื่อ packet |
| `5` | 4 | ค่าที่ใช้ตรวจ checksum และเลือก payload processor | ยืนยันการใช้งานจากทั้งสอง validation functions |
| `9` | 4 | ฟิลด์ที่เปรียบเทียบกับค่าลำดับที่คาดหวัง | โค้ดยืนยันการเปรียบเทียบ; ชื่อ sequence เป็นการตีความจาก counter ที่เพิ่มหลังสำเร็จ |
| `13` | แปรผัน | payload | ยืนยัน pointer `frame + 0x0D` และ length ลบ `0x0D` |

**ข้อจำกัดสำคัญ:** เมื่อ flag ไม่เป็นศูนย์ เส้นทาง decode เก็บ 3 ไบต์แรกไว้ แล้วส่งส่วนตั้งแต่ offset 3 เข้า transform ดังนั้น offset 3, 5 และ 9 ในตารางหมายถึงโครงสร้าง **หลังแปลงข้อมูล** สำหรับ frame ประเภทนี้ ห้ามถือว่าอ่านฟิลด์เหล่านี้ตรงจาก wire ได้เสมอ

เส้นทาง BaseCat ที่ `0x00666260` ปฏิเสธ length เป็น 0 หรือมากกว่า `0x1000` (4,096) และรอจน stream มีอย่างน้อย 13 ไบต์ก่อนอ่าน frame ค่านี้เป็นขอบเขตของเส้นทางที่ตรวจพบ ไม่ใช่ข้อสรุปว่าแพ็กเก็ตทุกประเภทในเกมจำกัด 4,096 ไบต์ เส้นทาง TCP ใช้ working buffer อีกขนาดหนึ่ง

ตัว object ที่ใช้สะสม frame มีจำนวนไบต์ที่สะสมอยู่ที่ object offset `+8` และตัว frame เริ่มที่ object offset `+10` ต้องแยก object offset เหล่านี้ออกจาก wire/header offset ในตาราง

### Checksum

ฟังก์ชัน `0x00564140` และ `0x006669D0` มีสูตรเดียวกัน:

```text
h = 0xBD136D48
for each byte b:
    h = ((h * 0x03FAA1FF) XOR sign_extend_int8_to_uint32(b)) mod 2^32
```

คำสั่ง `MOVSX` ใน assembly ยืนยันว่า byte ถูกขยายแบบ signed หาก port ไป C#, TypeScript หรือ Python ต้องจำลอง signed byte และการ wrap 32-bit ให้ตรง

ใน validation path ที่ตรวจพบ โค้ดเก็บค่าฟิลด์ offset 5 ไว้ ตั้งฟิลด์นั้นเป็น 100 (`0x64`) ชั่วคราว แล้วคำนวณตั้งแต่ frame offset 3 จนถึงท้าย frame จากนั้นเปรียบเทียบและคืนค่าเดิม เมื่อ checksum ผ่าน จึงตรวจฟิลด์ offset 9 ต่อ

ก่อนคำนวณ checksum มีขั้น transform และ payload processing อีกชั้นหนึ่ง ยังไม่สรุปชื่อ algorithm หรือ key schedule และยังไม่ได้ทดสอบกับแพ็กเก็ตจริง จึงยังไม่ควรสร้าง serializer ที่สมมติว่า checksum อย่างเดียวเพียงพอ

## 5. จุดเริ่มต้นของ login protocol

สกัด strings ที่มี ` -> PS_` ได้ 616 ตำแหน่ง และแยกชื่อ `PS_*` ที่ไม่ซ้ำได้ 459 ชื่อ ตัวเลขนี้เป็น **ชื่อที่อยู่ใน debug strings** ไม่ใช่จำนวน opcode ที่ยืนยันแล้ว

| ชื่อใน string | String VA | สิ่งที่ยังต้องทำ |
|---|---|---|
| `PS_Certify_Req` | `0x0112CB2C` | หา serializer และ numeric opcode |
| `PS_LoginData_Packet_REQ` | `0x0112CB58` | หาโครงสร้างและลำดับการใช้งาน |
| `PS_Certify_Nak` | `0x0112CBC8`, `0x0112CC1C` | หา error/result payload |
| `PS_Certify_Ack` | `0x0112CC40` | หา success payload |
| `PS_ServerList_Ack` | `0x0112CCF4` | หา schema รายการ server |
| `PS_Move_Auth_Server_Ack` | `0x0112CD54` | ยืนยันการเปลี่ยนปลายทางและ session state |
| `PS_Select_Server_Ack` | `0x0112CD78` | หา response และเงื่อนไข |
| `PS_ServerList_Req` | `0x0112CDC8` | หา request opcode |
| `PS_Select_Server_Req` | `0x0112CDF8` | หา request schema |

การค้น pointer แบบ absolute 32-bit ภายใน `.text` ยังไม่พบ direct literal references ของ debug strings กลุ่มนี้ จึงยังไม่ได้ผูกชื่อเหล่านี้กับฟังก์ชันที่กำลังทำงานจริง อาจต้องไล่ตาราง dispatch, relative references หรือ code path อื่นต่อไป ห้ามตั้งเลข opcode หรือสรุปลำดับ handshake จากลำดับ strings ในไฟล์

## 6. จุดที่มีผลต่อการทดสอบ runtime

`PhysXLoader.dll` ในชุดนี้มีลักษณะเป็น wrapper เพิ่มเติม:

- `0x10002520` เรียก `LoadLibraryA("xp.dll")` แล้วหา `NxCreatePhysicsSDK`, `NxGetCookingLib`, `NxGetUtilLib`, `NxReleasePhysicsSDK`
- พบข้อความเกี่ยวกับทีม BKS ใน initialization function และข้อความ `C9.exe`, `ipmana.exe`, `C9_DATA.exm`
- PDB path ของ wrapper ต่างจาก PDB path ที่พบใน `xp.dll`

หลักฐานนี้ทำให้ต้องรวม wrapper ไว้ในการวิเคราะห์ startup และ runtime; ไม่ควรอธิบายพฤติกรรมทั้งชุดจาก `C9.exe` เพียงตัวเดียว ยังไม่ได้พิสูจน์หน้าที่ของ hooks ทุกตัว หรือสรุปว่าเป็น malware

พบ literal `202.14.103.223` และ `202.14.103.223:10080` ใน EXE และยืนยัน code literal use ที่ `0x009B6CCB` / `0x009B6CD0` ภายใน `0x009B6BF0` ได้แล้ว อย่างไรก็ตามยังไม่ยืนยันว่าเป็น login/game endpoint ฟังก์ชันนี้อยู่ใกล้โค้ดข้อความ build และการรายงาน จึงต้องตามผู้เรียกและทดสอบ runtime ก่อน ห้ามนำค่า IP นี้ไปตั้งเป็นเป้าหมาย capture หรือ server emulator โดยอาศัย strings อย่างเดียว

`localinfo.dat` มี URL version update และ launcher มี URL news patch ข้อมูลเหล่านี้เป็นบริการ update/news ไม่ใช่หลักฐานของ auth/game endpoint

## 7. งานถัดไปที่ควรทำ

1. ไล่ caller ของ `0x00563820` และ `0x00665650` เพื่อยืนยันที่มาของ host / port / session state
2. ไล่ codec และ payload processor จาก `0x00DCEF50`, `0x00412230` และ `0x00563600` พร้อมเส้นทางส่งที่คู่กัน
3. ไล่จาก receive queue ไปตัว dispatch และผูก numeric type กับ `PS_Certify_*`, `PS_ServerList_*`, `PS_Select_Server_*`
4. เก็บ runtime trace ของ Client ใน Windows VM โดยจำกัดเฉพาะ process/connection ของเกม แล้วตรวจว่า framing และ field offsets ตรงกับข้อมูลจริง
5. หลังมีหลักฐาน handshake จึงสร้าง server skeleton และ fixture ของ protocol ตาม Client SHA-256 นี้ หลีกเลี่ยงการเดา login response จากชื่อ packet

## 8. แฟ้มหลักฐานและวิธีทำซ้ำ

| ไฟล์/โฟลเดอร์ | เนื้อหา |
|---|---|
| `evidence/static/pe_inventory.json` | PE headers, SHA-256, imports, exports และ PDB paths ของ 7 ไฟล์ |
| `evidence/static/imports.tsv` | DLL, import name / ordinal และ IAT VA |
| `evidence/static/packet_names.tsv` | ชื่อ PS packet candidates 459 ชื่อและตำแหน่ง strings |
| `evidence/static/string_literal_hits.json` | ผลค้น absolute code literals; เป็นผลค้นไบต์ ไม่ใช่ xref ที่ยืนยันทุกตำแหน่ง |
| `evidence/die/` | รายงาน JSON จาก Detect It Easy |
| `evidence/ghidra/network_xrefs.tsv` | Import references และจุดโค้ดที่เลือกตรวจเพิ่มเติม |
| `evidence/ghidra/network_decompiled.c` | Pseudocode 82 ฟังก์ชัน; ใช้สำหรับอ่าน ไม่ใช่ source ที่ compile ได้ |
| `evidence/ghidra/packet_framing.c` / `.asm.txt` | ฟังก์ชันประกอบและตรวจ frame พร้อม assembly |
| `evidence/ghidra/packet_transform.c` / `.asm.txt` | Checksum และ transform dispatch ที่เจาะเพิ่ม |
| `evidence/ghidra/projects/C9_RE_Phase1.gpr` | Ghidra research project ของ EXE |
| `evidence/ghidra_shim/` | รายงานและ Ghidra research project ของ wrapper DLL |
| `scripts/` | สคริปต์ inventory และ Ghidra export สำหรับทำซ้ำ |

เรียกจาก project directory:

```bash
python3 re/scripts/c9_static_inventory.py 52C9GL re/evidence/static
python3 re/scripts/run_ghidra.py --process --no-analysis
python3 re/scripts/run_ghidra.py --process --no-analysis \
  --functions 00561950 00561ac0 00561a20 00561a50 00561a80 00561630 \
  00563d00 00666890 00666260 00563e40
python3 re/scripts/run_ghidra.py --process --no-analysis \
  --export-prefix packet_transform \
  --functions 00564140 006669d0 00563600 00412230 00dcef50
```

หากยังไม่มี research project ให้เรียก runner ครั้งแรกโดยไม่ใส่ `--process` สคริปต์ default อ้าง JDK 25 และ Ghidra 12.2 ที่ติดตั้งอยู่ในเครื่องนี้ สามารถระบุ `--java` และ `--ghidra` เมื่อย้ายเครื่อง

การ build native decompiler ที่ใช้ในรอบนี้คือ target `ghidra_opt` จาก Makefile ที่มากับ Ghidra โดยกำหนด `ARCH_TYPE=-arch arm64` และ `ADDITIONAL_FLAGS=-mmacosx-version-min=11.0` ไม่ได้ดาวน์โหลด native binary จากแหล่งอื่น

โค้ดที่ decompile มี type/prototype ที่เครื่องมืออนุมาน และมี warning เรื่อง calling convention / exception handling ต้องดู assembly หรือ trace เพิ่มก่อน port เป็น implementation จริง
