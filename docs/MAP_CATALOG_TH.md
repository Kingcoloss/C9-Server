# ทะเบียนแผนที่ C9 และผลตรวจไฟล์

รายงานสร้างจาก MapData ที่ client โหลดจริง และตารางไฟล์ CFS ใน MapData ทั้งหมด
ตัวสร้างรายงาน: `re/scripts/build_map_manifest.py` พร้อม `--markdown`

- รหัสแผนที่: 257; โฟลเดอร์ฉาก: 136; ส่วนย่อย: 516
- ตรวจแพ็ก 1,887 ไฟล์ / 22,466 รายการ; ตารางไฟล์ผิดพลาด 0 รายการ
- พบ geometry และ marker ที่อ่านส่วนต้นได้ 450/516 คู่
- มีไฟล์ตรงครบทุกส่วนย่อย 207 แผนที่; มีอย่างน้อยหนึ่งส่วนย่อยที่ตรง 231 แผนที่
- พบพิกัดตัวอย่างจาก marker แรก 420 คู่

คำว่าไฟล์พร้อมในรายงานหมายถึงพบชื่อไฟล์และอ่านส่วนต้นที่บีบอัดได้
การเล่นจริงยังต้องทดสอบการโหลดฉากและระบบ world ของแต่ละคู่
คู่ที่ยังไม่ครบต้องตรวจข้อมูลเก่า ชื่อเฉพาะภาษา และไฟล์ที่ขาดต่อไป
พิกัดตัวอย่างเป็น marker แรกของไฟล์ ยังไม่ใช่จุดเกิดผู้เล่นที่ยืนยันแล้ว

## ใช้กับบริการในเครื่อง

ตั้ง `initial_map_id` และ `initial_submap` ใน `config/lab-character-lobby.json`
ระบบตรวจคู่รหัสและส่วนย่อยจาก catalog ก่อนเปิดบริการ ส่วนย่อยเริ่มจาก 0
ถ้าไม่มีพิกัดตัวอย่าง ต้องระบุ `initial_position` จากข้อมูลที่ตรวจสอบแล้ว
ที่หน้า Choose World ใช้ `scripts/Update-C9Handoff.ps1` เพื่ออัปเดตบริการตัวละคร

## รายการครบทุก Map ID

| Map ID | ชื่อในข้อมูลเกม | โฟลเดอร์ | ไฟล์พร้อม / ส่วนย่อย | ลำดับที่ยังไม่ครบ |
|---:|---|---|---:|---|
| 1 | [튜토리얼]메슈리안 산맥 | C9_0th_Stage_MeshurianMT | 1/1 | — |
| 4 | [1대륙 1-1]고블린 숲 | C9_1th_Stage_GoblinForest | 5/6 | 4 |
| 5 | [1대륙 3-3]임프의 숲 | C9_1th_Stage_ImpForest | 5/6 | 4 |
| 8 | [1대륙 2-2]고블린 지하제단 | C9_1th_Stage_GoblinUA | 5/6 | 4 |
| 9 | [1대륙 4-4]임프의 빼앗긴 광산 | C9_1th_Stage_ImpCave | 5/6 | 4 |
| 10 | [1대륙 마을]워터포드 마을 | C9_1th_Town_Waterford | 1/1 | — |
| 11 | [1대륙 5-5]바닷가 폐허 마을 | C9_1th_Stage_SeaRuinsVill | 5/6 | 4 |
| 13 | [1대륙 6-6]메슈리안 고원 | C9_1th_Stage_MeshurianMT | 5/6 | 4 |
| 14 | [1대륙 9-8]망자의 사원 | C9_1th_Stage_DeadTemple | 5/6 | 4 |
| 15 | [1대륙 10-9]델파스트 북쪽 경계지역 | C9_1th_Stage_DelpastNorth | 5/6 | 4 |
| 16 | [1대륙 8-7]매혹의 숲 | C9_1th_Stage_CharmForest | 5/6 | 4 |
| 19 | [2대륙 마을]빔펠리 성 | C9_2th_Town_Vimpeli | 1/1 | — |
| 21 | [PVP Zone]빔펠리 | PvP_Vimpeli | 1/1 | — |
| 22 | [2대륙 1-1]불타는 중부 평야 | C9_2th_Stage_DistrickPlain | 5/5 | — |
| 23 | [2대륙 4-4]붉은 안개 늪지대 | C9_2th_Stage_RedFogMarsh | 4/4 | — |
| 24 | 길드하우스 여관급 | C9_In_GuildHouse_a | 1/1 | — |
| 25 | [2대륙 3-3]픽시의 황금 광산 | C9_2th_Stage_PixyMine | 4/4 | — |
| 26 | [2대륙 2-2]서부 국경 | C9_2th_Stage_WestFrontier | 4/4 | — |
| 27 | [2대륙 6-5]고통의 구덩이 | C9_2th_Stage_PainHole | 4/4 | — |
| 28 | [2대륙 7-6]오베론의 안식처 | C9_2th_Stage_OberonShelter | 4/5 | 4 |
| 29 | [사용안함]잊혀진 해안가 | HiddenStage_1th_ForgSeaForest | 0/1 | 0 |
| 30 | [2대륙 9-7]오레아스의 신전 | C9_2th_Stage_OrearsAlter | 4/4 | — |
| 31 | [2대륙 10-8]화산섬 지하통로 | C9_2th_Stage_VolslandUT | 5/5 | — |
| 32 | [2대륙 11-9]헨코 분화구 | C9_2th_Stage_HenSlandCrater | 4/4 | — |
| 33 | [3대륙 마을]러너스빌 | C9_3th_Town_RunnersVill | 1/1 | — |
| 35 | [2대륙 숨던]북부 국경지대 | HiddenStage_2th_NorthFrontier | 1/1 | — |
| 36 | [사용안함]고블린 창고 | HiddenDepot_1th_a | 0/1 | 0 |
| 39 | [2대륙 숨창]오크의 창고 | HiddenDepot_2th_a | 0/1 | 0 |
| 41 | [3대륙 숨창]바베가지 창고 | HiddenDepot_3th_a | 0/1 | 0 |
| 42 | [3대륙 1-1]강제 노역소 | C9_3th_stage_LaborArea | 4/4 | — |
| 43 | [3대륙 2-2]웬디고 캠프 | C9_3th_Stage_WendigoCamp | 4/4 | — |
| 44 | [3대륙 4-3]영혼의 계곡 | C9_3th_Stage_SoulValley | 4/4 | — |
| 45 | [3대륙 5-4]극한의 고원 | C9_3th_Stage_ExtrmeHL | 4/4 | — |
| 46 | [3대륙 7-5]얼음의 협곡 | C9_3th_Stage_IceValley | 4/4 | — |
| 52 | [3대륙 8-6]타락한 루나드의 성 | C9_3th_Stage_HiredBase | 4/5 | 4 |
| 53 | [3대륙 10-7]녹아 내리는 얼음 섬 | C9_3th_Stage_MeltIceIsland | 4/4 | — |
| 54 | [PVP 연습장]녹아 내린 협곡 | PvP_Training | 1/1 | — |
| 55 | GVG 경기장 | GvG_Stage_a | 0/1 | 0 |
| 56 | 테스트용 더미맵 | C9_DummyMap | 0/1 | 0 |
| 57 | 길드하우스 회관급 | C9_In_GuildHouse_b | 1/1 | — |
| 58 | [3대륙 숨던]이단자의 섬 | HiddenStage_3th_HereticIsland | 1/1 | — |
| 59 | [PVP Zone]워터포드 | PvP_Waterford | 1/1 | — |
| 60 | [5대륙]서바이벌 | SUV_ArenaStage | 1/1 | — |
| 61 | 길드 난입 테스트 | C9_In_GuildHouse_c_intrusion | 0/1 | 0 |
| 62 | [PVP Zone]러너스빌 | PvP_RunnersVill | 1/1 | — |
| 63 | [4대륙 1-1] 암흑 결계 지역 | C9_4th_Stage_DarkFogArea | 4/4 | — |
| 64 | [4대륙 2-2] 참혹한 유적지 | C9_4th_Stage_MiseryRuins | 4/4 | — |
| 65 | [공성] 1성 벤투스 성 | Siege_1th_Ventus | 0/1 | 0 |
| 66 | [튜토리얼]잊혀진 해안가 | C9_0th_ForgSeaForest | 1/1 | — |
| 67 | 녹아 내린 협곡(퀘스트용) | PvP_Training_Quest | 1/1 | — |
| 68 | [데스매치] 투사의 전당 | DeathMatch_Aria | 1/1 | — |
| 69 | [데스매치] 크로스로드 | Deathmatch_Crossroad | 0/1 | 0 |
| 70 | [데스매치] 천공의 결투장 | Deathmatch_Stadium | 0/1 | 0 |
| 71 | [팀매치] 운명의 결투장 | DeathMatch_Rounding | 1/1 | — |
| 72 | [데스매치] Q3_Test | DeathMatch_Q3_Test | 0/1 | 0 |
| 73 | [데스매치] 공성테스트_팀 | DeathMatch_Test | 0/1 | 0 |
| 74 | [데스매치] 아케네의 대전당 | DeathMatch_CenterStrike | 0/1 | 0 |
| 75 | [데스매치] 혈전의 전당 | DeathMatch_Iris | 1/1 | — |
| 76 | [공성] 2성 아르테르 성 | Siege_2th_Arteree | 0/1 | 0 |
| 77 | [공성] 3성 칸디둠 성 | Siege_3th_Candidum | 0/1 | 0 |
| 78 | [4대륙 3-3] 타락한 신의 정원 | C9_4th_Stage_CorruptGarden | 4/4 | — |
| 79 | [4대륙 4-4] 비밀의 신전 | C9_4th_Stage_SecretTemple | 5/5 | — |
| 80 | 아레나 더미맵 | C9_ArenaLobby | 1/1 | — |
| 81 | [데스매치] 잊혀진 유적 | DeathMatch_Hideout | 1/1 | — |
| 82 | [데스매치] 로스트메이즈 | DeathMatch_Lostmaze | 0/1 | 0 |
| 83 | [4대륙 8-5] 크나토스 양성소 | C9_4th_Stage_CnatoCenter | 4/4 | — |
| 84 | [4대륙 9-6] 아쿠바의 둥지 | C9_4th_Stage_FearHole | 4/4 | — |
| 85 | [4대륙 10-7] 신의 부름터 | C9_4th_Stage_AncientsCave | 5/5 | — |
| 86 | [데스매치] 웨어하우스_테스트 | DeathMatch_WareHouse_test | 0/1 | 0 |
| 87 | [4대륙 숨던] 고대인의 유적지 | HiddenStage_4th | 1/1 | — |
| 88 | [4대륙 숨창] 크나토스 창고 | HiddenDepot_4th_a | 0/1 | 0 |
| 89 | [릴레이매치] 거상의 무덤 | PvP_okapia | 1/1 | — |
| 90 | [1대륙 숨창]고블린 창고 | HiddenDepot_1th_a | 0/1 | 0 |
| 91 | [1대륙 7-10]델파스트 전선 | C9_1th_Stage_DelpastBattleField | 5/6 | 4 |
| 92 | [2대륙 5-10]바루카 요새 | C9_2th_Stage_Barooka | 4/4 | — |
| 93 | [2대륙 8-11]스팀팔리아 망루 | C9_2th_Stage_Truekia-ElonBorder | 4/4 | — |
| 94 | [3대륙 3-8]노르간트 저항선 | C9_3th_Stage_NorgantBattlefront | 4/4 | — |
| 95 | [3대륙 6-9]타하라 고원 | C9_3th_Stage_TaharaHL | 4/4 | — |
| 96 | [3대륙 9-10]엘퀴네스의 쉼터 | C9_3th_Stage_ElquinesShelter | 5/5 | — |
| 97 | [4대륙 5-8] 스카라브 번식지 | C9_4th_Stage_ScaravPlace | 4/4 | — |
| 98 | [4대륙 6-9] 마쉬아 왕조의 무덤 | C9_4th_Stage_TreasureHouse | 4/4 | — |
| 99 | [4대륙 7-10] 크나토스 지하 감옥 | C9_4th_Stage_CnatosDungeon | 4/4 | — |
| 100 | [데스매치] 상인들의 거리 | DeathMatch_Market | 1/1 | — |
| 101 | [5대륙-1] 부서지는 파도 해안 | C9_5th_Stage_SurfCoast | 4/4 | — |
| 102 | [5대륙-2] 홀리스의 전진기지 | C9_5th_Stage_HollisCenter | 4/4 | — |
| 103 | [5대륙-3] 신성한 대지 코데스 | C9_5th_stage_HolyLand | 4/4 | — |
| 104 | [5대륙-4] 불의 신전 유피아 | C9_5th_Stage_suntemple | 4/4 | — |
| 105 | [5대륙-5] 대사막 마노아 | C9_5th_Stage_DesertManoa | 5/5 | — |
| 106 | [5대륙 숨던] 숨겨진 최후의 보루 | HiddenStage_5th_HiddenFortress | 1/1 | — |
| 107 | [5대륙 숨창] 말라버린 신의 창고 | HiddenDepot_5th_DriedDepot | 0/1 | 0 |
| 108 | [5대륙-6] 다브엘 성전 | C9_5th_Stage_DabealTemple | 4/4 | — |
| 109 | [5대륙-7] 타오르는 붉은 전장 | C9_5th_Stage_RedBattle | 4/4 | — |
| 110 | [5대륙-8] 화염 장막 카미노스 | C9_5th_Stage_FlameCurtain | 4/4 | — |
| 111 | [5대륙-9] 달의 사막 | C9_5th_Stage_DesertMoon | 4/4 | — |
| 112 | [5대륙-10] 성도 바실루스 | C9_5th_Stage_SaintsBasileus | 5/5 | — |
| 114 | [길드대전] 1 상인들의 거리 | Guild_2th_Stage01 | 1/1 | — |
| 115 | [길드대전] 2 성벽 전투 | Guild_2th_Stage02 | 1/1 | — |
| 116 | [길드대전] 3 내성 전투 | Guild_2th_Stage03 | 1/1 | — |
| 117 | [길드대전] 4 왕좌 전투 | Guild_2th_Stage04 | 1/1 | — |
| 118 | [튜토리얼]워터포드 마을 | C9_1th_Town_Waterford | 1/1 | — |
| 120 | [5대륙 익스트림]바르디엘 봉인터 | Raid_5th_sarad | 1/1 | — |
| 121 | [3대륙 전승 최종]극한의 고원 | C9_3th_Stage_ExtrmeHL | 1/1 | — |
| 122 | [3대륙 전승] 노르간트 저항선 | C9_3th_Stage_NorgantBattlefront | 1/1 | — |
| 123 | [1대륙 2차 전직]망자의 사원 | C9_1th_Stage_DeadTemple | 1/1 | — |
| 127 | [전직퀘스트]잊혀진 해안가 | C9_0th_ForgSeaForest | 1/1 | — |
| 128 | [전직퀘스트]임프의 빼앗긴 광산 | C9_1th_Stage_ImpCave | 1/1 | — |
| 129 | [전직퀘스트]녹아 내린 협곡-전투력 | PvP_Training_Quest | 1/1 | — |
| 130 | [전직퀘스트]녹아 내린 협곡-순발력 | PvP_Training_Quest | 1/1 | — |
| 131 | [전직퀘스트]녹아 내린 협곡-결투 | PvP_Training_Quest | 1/1 | — |
| 132 | [전직퀘스트]녹아 내린 협곡-기민함 | PvP_Training_Quest | 1/1 | — |
| 133 | [전직퀘스트]매혹의 숲 | C9_1th_Stage_CharmForest | 1/1 | — |
| 134 | [팀매치]빔펠리 | PvP_Vimpeli | 1/1 | — |
| 135 | [팀매치]워터포드 | PvP_Waterford | 1/1 | — |
| 136 | [팀매치]러너스빌 | PvP_RunnersVill | 1/1 | — |
| 137 | [4대륙]서바이벌 | SUV_ArenaStage | 1/1 | — |
| 138 | [3대륙]서바이벌 | SUV_ArenaStage | 1/1 | — |
| 139 | [2대륙 Hell] 오베론의 안식처 | C9_2th_Stage_OberonShelter | 1/1 | — |
| 140 | [3대륙 Hell] 타락한 루나드의 성 | C9_3th_Stage_HiredBase | 1/1 | — |
| 141 | [4대륙 Hell] 신의 부름터 | C9_4th_Stage_AncientsCave | 1/1 | — |
| 142 | [2대륙-퀘스트]고통의 구덩이 | C9_2th_Stage_PainHole | 1/1 | — |
| 143 | 길드대전 로비맵 | C9_GuildwarLobby | 1/1 | — |
| 144 | [2대륙-퀘스트]서부 국경 지대 | C9_2th_Stage_WestFrontier | 1/1 | — |
| 145 | [3대륙-퀘스트]강제 노역소 | C9_3th_stage_LaborArea | 1/1 | — |
| 146 | [3대륙-퀘스트]극한의 고원 | C9_3th_Stage_ExtrmeHL | 1/1 | — |
| 147 | [3대륙-퀘스트]엘퀴네스의 쉼터 | C9_3th_Stage_ElquinesShelter | 1/1 | — |
| 148 | [튜토리얼]잊혀진 해안가 | C9_0th_ForgSeaForest | 1/1 | — |
| 149 | 녹아내린 협곡 | PvP_Training_Quest | 1/1 | — |
| 150 | 녹아내린 협곡 | PvP_Training_Quest | 1/1 | — |
| 151 | [6대륙-1] 감춰진 바람의 계곡 | C9_6th_Stage_WindValley | 5/5 | — |
| 152 | [6대륙-2] 갈라진 코데스의 파편 | C9_6th_Stage_TornFragments | 5/5 | — |
| 153 | [6대륙-3] 시들어가는 영원의 숲 | C9_6th_Stage_EternalForest | 5/5 | — |
| 154 | [6대륙-4] 슈테르넨 숲 벌목지 | C9_6th_Stage_Syutereunenforest | 5/5 | — |
| 155 | [6대륙-5] 심연의 지하통로 | C9_6th_Stage_AbyssUnderPass | 5/5 | — |
| 156 | [6대륙-6] 뒤틀린 미로의 숲 | C9_6th_Stage_GodShelter | 4/5 | 0 |
| 157 | [6대륙-7] 무너진 수룡의 신전 | C9_6th_Stage_DragonTemple | 5/5 | — |
| 158 | [6대륙-8] 마르지 않는 샘 엔도르 | C9_6thcom_ChaosEndor | 4/5 | 0 |
| 159 | [6대륙 익스트림] 혼돈의 첨탑 | C9_6th_Stage_ChaosSteeple | 1/1 | — |
| 160 | [6대륙 숨던] 가시 그늘의 숲 | HiddenStage_6th_a | 1/1 | — |
| 161 | [6대륙 숨창] 요정의 쉼터 | HiddenDepot_6th_a | 1/1 | — |
| 162 | [팀매치]중국 | PVP_Chi | 1/1 | — |
| 163 | [PVP Zone]중국 | PVP_Chi | 1/1 | — |
| 164 | [PVP Zone]일본 | PvP_Japan | 1/1 | — |
| 165 | [팀매치]일본 | PvP_Japan | 1/1 | — |
| 166 | <C9Color0xFF56e353>튜토리얼<C9OldColor> | C9_0th_Stage_MeshurianMT | 1/1 | — |
| 167 | [팀매치] 테스트맵 | PVP_GOA | 0/1 | 0 |
| 168 | [PVP Zone]한국 | PvP_Kor | 1/1 | — |
| 169 | [팀매치]한국 | PvP_Kor | 1/1 | — |
| 170 | [3대륙-퀘스트]노르간트 저항선_파이터 | C9_3th_Stage_NorgantBattlefront | 1/1 | — |
| 171 | [3대륙]서바이벌_CHN | SUV_ArenaStage | 1/1 | — |
| 172 | [3대륙-퀘스트]영혼의 계곡 | C9_3th_Stage_SoulValley | 1/1 | — |
| 173 | [3대륙-퀘스트]러너스 빌_파이터 | C9_3th_Town_RunnersVill | 1/1 | — |
| 174 | [3대륙-퀘스트]얼음의 협곡 | C9_3th_Stage_IceValley | 1/1 | — |
| 175 | [포츈 서바이벌] | SUV_ArenaStage | 1/1 | — |
| 176 | [4대륙 퀘스트]실종 사건 발생지 | C9_2th_Stage_RedFogMarsh | 1/1 | — |
| 177 | 길드하우스 저택급 | C9_In_GuildHouse_a_new | 1/1 | — |
| 178 | 길드하우스 대회관급 | C9_In_GuildHouse_b_new | 1/1 | — |
| 179 | [전직]잊혀진 해안가 - 파이터 | C9_0th_ForgSeaForest | 1/1 | — |
| 180 | [전직]잊혀진 해안가 - 헌터 | C9_0th_ForgSeaForest | 1/1 | — |
| 181 | [전직]잊혀진 해안가 - 샤먼 | C9_0th_ForgSeaForest | 1/1 | — |
| 182 | [전직]잊혀진 해안가 - 위치블레이드 | C9_0th_ForgSeaForest | 1/1 | — |
| 183 | 이벤트 표시용 더미맵 | C9_0th_Stage_MeshurianMT | 1/1 | — |
| 184 | [데스매치] 데스매치_테스트 | Deathmatch_strasbourg | 0/1 | 0 |
| 185 | 녹아 내린 협곡(이벤트용) | PvP_Training | 1/1 | — |
| 186 | [6대륙 마을]엔실롯 | C9_6th_Town_Ensilrot | 1/1 | — |
| 187 | [6대륙-1] 시들어가는 영원의 숲(사용안함) | C9_6thcom_DisorderlyForest | 0/1 | 0 |
| 188 | [6대륙-2] 마르지 않는 샘 엔도르(사용안함) | C9_6thcom_ChaosEndor | 1/1 | — |
| 189 | [PVP Zone]태국 | PvP_Thai | 1/1 | — |
| 190 | [팀매치]태국 | PvP_Thai | 1/1 | — |
| 191 | 총명의 수련장 | PvP_Training | 1/1 | — |
| 192 | [4대륙 숨창] 크나토스 창고_유료 | HiddenDepot_4th_a | 1/1 | — |
| 193 | [5대륙 숨창] 말라버린 신의 창고_유료 | HiddenDepot_5th_DriedDepot | 1/1 | — |
| 194 | [AOS]전장 모드 | LOCL | 0/1 | 0 |
| 195 | 전장모드 캐릭터 선택 더미맵 | LOC | 0/1 | 0 |
| 196 | [1대륙 숨창]고블린 창고_유료 | HiddenDepot_1th_a | 1/1 | — |
| 197 | [6대륙] 숨겨진 탑 | C9_6thcom_SecretTower_new | 1/1 | — |
| 198 | [6대륙]서바이벌 | SUV_ArenaStage | 1/1 | — |
| 199 | [전직]잊혀진 해안가 - 노블레스 | C9_0th_ForgSeaForest | 1/1 | — |
| 200 | [6대륙] 숨겨진 탑 - 중국용 | C9_6thcom_SecretTower_new | 1/1 | — |
| 201 | 버그테스트용맵 | C9_1th_Stage_DelpastNorth | 1/1 | — |
| 202 | [좀비모드] 투사의 전당 | DeathMatch_Aria | 1/1 | — |
| 203 | [데스매치] 빔펠리 시가지 | Guild_2th_Stage01 | 1/1 | — |
| 204 | [데스매치] 빔펠리 성벽 | Guild_2th_Stage02 | 1/1 | — |
| 205 | [데스매치] 빔펠리 내성 | Guild_2th_Stage03 | 1/1 | — |
| 206 | [데스매치] 클라우스의 왕좌 | Guild_2th_Stage04 | 1/1 | — |
| 207 | [팀매치] 빔펠리 내성 | Guild_2th_Stage03 | 1/1 | — |
| 208 | [팀매치] 빔펠리 성벽 | Guild_2th_Stage02 | 1/1 | — |
| 209 | [팀매치] 빔펠리 시가지 | Guild_2th_Stage01 | 1/1 | — |
| 210 | [팀매치] 클라우스의 왕좌 | Guild_2th_Stage04 | 1/1 | — |
| 211 | [데스매치] 아케네의 대전당 | DeathMatch_CenterStrike | 1/1 | — |
| 212 | [좀비모드] 상인들의 거리 | Guild_2th_Stage01 | 1/1 | — |
| 213 | [좀비모드] 아케네의 대전당 | DeathMatch_CenterStrike | 1/1 | — |
| 214 | [이벤트]실드보스 미션 | PvP_Training | 1/1 | — |
| 215 | [이벤트]오브젝트 파괴 | C9_1th_Stage_DeadTemple | 1/1 | — |
| 216 | [이벤트]악동 소탕 대작전 | C9_1th_Town_Waterford | 1/1 | — |
| 217 | [이벤트]내일은 X10 | DeathMatch_Iris | 1/1 | — |
| 218 | [모드] 몬스터 소환대전 | Mode_SummonersWar | 0/1 | 0 |
| 219 | [종말모드]운명의 결투장 | DeathMatch_Rounding | 1/1 | — |
| 220 | [3대륙-퀘스트]러너스 빌_샤먼 | C9_3th_Town_RunnersVill | 1/1 | — |
| 221 | [3대륙-퀘스트]러너스 빌_헌터 | C9_3th_Town_RunnersVill | 1/1 | — |
| 222 | [3대륙-퀘스트]러너스 빌_위치 | C9_3th_Town_RunnersVill | 1/1 | — |
| 223 | [3대륙-퀘스트]러너스 빌_미스틱 | C9_3th_Town_RunnersVill | 1/1 | — |
| 224 | [종말모드]지하 감옥 | C9_4th_Stage_CnatosDungeon | 1/1 | — |
| 225 | [릴레이매치치]러너스빌 | PvP_RunnersVill | 1/1 | — |
| 226 | [릴레이매치] 천공의 결투장 | Deathmatch_Stadium | 1/1 | — |
| 227 | [3대륙-퀘스트]노르간트 저항선_샤먼 | C9_3th_Stage_NorgantBattlefront | 1/1 | — |
| 228 | [3대륙-퀘스트]노르간트 저항선_헌터 | C9_3th_Stage_NorgantBattlefront | 1/1 | — |
| 229 | [3대륙-퀘스트]노르간트 저항선_위치 | C9_3th_Stage_NorgantBattlefront | 1/1 | — |
| 230 | [3대륙-퀘스트]노르간트 저항선_미스틱 | C9_3th_Stage_NorgantBattlefront | 1/1 | — |
| 231 | 고대의 경기장 | PVP_Russia | 1/1 | — |
| 232 | [6대륙-퀘스트]각성의 장 | C9_0th_ForgSeaForest | 1/1 | — |
| 233 | [요일던전]스카라브전 알 섬멸전 | C9_4th_Stage_AncientsCave | 1/1 | — |
| 234 | [요일던전]오크들의 습격 | C9_2th_Stage_Barooka | 1/1 | — |
| 235 | [요일던전]얼음 동굴 파수꾼 | C9_3th_Stage_MeltIceIsland | 1/1 | — |
| 236 | [요일던전]러너스빌의 악몽 | C9_3th_Town_RunnersVill | 1/1 | — |
| 237 | [요일던전]성장의 증거 | PvP_Training | 1/1 | — |
| 238 | [요일던전]운을 시험해 보세요 | C9_4th_Stage_AncientsCave | 1/1 | — |
| 239 | [6대륙-어나더] 감춰진 바람의 계곡 | C9_6th_Stage_WindValley | 4/4 | — |
| 240 | [6대륙-어나더] 갈라진 코데스의 파편 | C9_6th_Stage_TornFragments | 4/4 | — |
| 241 | [6대륙-어나더] 시들어가는 영원의 숲 | C9_6th_Stage_EternalForest | 4/4 | — |
| 242 | [6대륙-어나더] 슈테르넨 숲 벌목지 | C9_6th_Stage_Syutereunenforest | 4/4 | — |
| 243 | [6대륙-어나더] 심연의 지하통로 | C9_6th_Stage_AbyssUnderPass | 4/4 | — |
| 244 | [6대륙 익스트림] 혼돈의 첨탑_2 | C9_6th_Stage_ChaosSteeple | 1/1 | — |
| 245 | 시간의 균열 | C9_6th_Stage_Syutereunenforest | 1/1 | — |
| 246 | [크래시] 운명의 결투장 | PvP_Thai | 1/1 | — |
| 247 | [6대륙 더미]쉐도우 | C9_6th_Town_Ensilrot | 1/1 | — |
| 248 | [경쟁전] 끝없는 전쟁 | SUV_ArenaStage | 1/1 | — |
| 249 | [7대륙-1] 사념의 듀라한 기지 | C9_7th_Stage_prisoncastle | 2/4 | 0, 1 |
| 250 | [7대륙-2] 문드러진 정령의 숲 | C9_7th_Stage_elfvillage | 2/4 | 0, 1 |
| 251 | [7대륙-3] 빛바랜 신전 | C9_7th_Stage_temple | 2/4 | 0, 1 |
| 252 | [7대륙 마을]엔노르 | C9_7th_Town_Ennor | 1/1 | — |
| 253 | [이벤트]실드보스 미션_7대륙 | PvP_Training | 1/1 | — |
| 254 | [이벤트]오브젝트 파괴_7대륙 | C9_1th_Stage_DeadTemple | 1/1 | — |
| 255 | [이벤트]무차별 총공격 | PvP_Training | 1/1 | — |
| 261 | [랭킹아레나듀얼]일본 | PvP_Japan | 1/1 | — |
| 262 | [랭킹아레나듀얼]중국 | PVP_Chi | 1/1 | — |
| 263 | [랭킹아레나듀얼]한국 | PvP_Kor | 1/1 | — |
| 264 | [랭킹아레나릴레이]빔펠리 | PvP_Vimpeli | 1/1 | — |
| 265 | [랭킹아레나릴레이]워터포드 | PvP_Waterford | 1/1 | — |
| 266 | [랭킹아레나릴레이]러너스빌 | PvP_RunnersVill | 1/1 | — |
| 267 | [8대륙-1]잊혀진 숲의 신전 | C9_8th_Stage_A_Vinetemple | 1/4 | 0, 1, 2 |
| 268 | [8대륙-2] 더러운 지하수로 | C9_8th_Stage_B_Ancientregion | 1/4 | 0, 1, 2 |
| 269 | [8대륙-3] 지배자의 성 | C9_8th_Stage_C_Sapientia | 1/4 | 0, 1, 2 |
| 270 | [深?]???安息? | C9_2th_Stage_OberonShelter | 1/1 | — |
| 271 | 赫?那德生存 | SUV_ArenaStage | 1/1 | — |
| 272 | [深?]?落的??德城 | C9_3th_Stage_HiredBase | 1/1 | — |
| 273 | [8대륙 Hell]8-4-地? | C9_8th_Hell_A | 1/1 | — |
| 274 | 8-5-?巢 | C9_8th_Stage_DragonNest | 1/3 | 0, 1 |
| 275 | [7대륙-1] 사념의 듀라한 기지[?界] | C9_7th_Stage_prisoncastle | 1/1 | — |
| 276 | [7대륙-2] 문드러진 정령의 숲[?界] | C9_7th_Stage_elfvillage | 1/1 | — |
| 277 | [7대륙-3] 빛바랜 신전[?界] | C9_7th_Stage_temple | 1/1 | — |
| 278 | [8대륙-1]잊혀진 숲의 신전[?界] | C9_8th_Stage_A_Vinetemple | 1/4 | 0, 1, 2 |
| 279 | [8대륙-2] 더러운 지하수로[?界] | C9_8th_Stage_B_Ancientregion | 1/4 | 0, 1, 2 |
| 280 | [8대륙-3] 지배자의 성[?界] | C9_8th_Stage_C_Sapientia | 1/4 | 0, 1, 2 |
| 281 | 被腐?的守?者之塔 | C9_6thcom_SecretTower_new | 1/1 | — |
| 282 | [深?]神之召?地 | C9_4th_Stage_AncientsCave | 1/1 | — |
| 283 | Boss Arena | C9_8th_Stage_DragonNest | 2/2 | — |
| 284 | [9th Continent] Judge Factory | C9_9th_Stage_JudgeFactory | 1/1 | — |

รายละเอียด suffix ชื่อไฟล์ hash และพิกัดตัวอย่างอยู่ใน
[map manifest](../re/evidence/static/windows_map_manifest.json) และ
[catalog จาก client](../re/evidence/static/windows_map_catalog.json)
