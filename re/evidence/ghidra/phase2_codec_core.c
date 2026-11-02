/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00563c10 FUN_00563c10 */
/* Callers:
 * 00a3892e 00a38850 FUN_00a38850
 * Callees:
 * 00564140 FUN_00564140
 * 005635c0 FUN_005635c0
 * 00dd5f60 memcpy
 * 005642d0 FUN_005642d0
 * 00564380 FUN_00564380
 * 00dceec0 FUN_00dceec0
 */

int __thiscall FUN_00563c10(undefined4 param_1,ushort *param_2,short *param_3,undefined4 param_4)

{
  undefined4 uVar1;
  int iVar2;
  int iVar3;
  
  iVar3 = (int)param_2 + 3;
  memcpy(param_3,param_2,3,param_1);
  *(undefined4 *)((int)param_2 + 9) = param_4;
  *(undefined4 *)((int)param_2 + 5) = 100;
  uVar1 = FUN_00564140(iVar3,(*param_2 - 3) + iVar3);
  *(undefined4 *)((int)param_2 + 5) = uVar1;
  FUN_005642d0(*(undefined2 *)((int)param_2 + 3));
  iVar2 = FUN_00564380();
  FUN_005635c0(*(int *)((int)param_2 + 5) + iVar2,(int)param_2 + 0xd,*param_2 - 0xd);
  iVar2 = FUN_00dceec0(0,(int)param_3 + 3,iVar3,*param_2 - 3);
  if (iVar2 < 1) {
    iVar2 = -1;
  }
  else {
    *param_3 = (short)iVar2 + 3;
    iVar2 = iVar2 + 3;
  }
  return iVar2;
}



/* 006667b0 FUN_006667b0 */
/* Callers:
 * 006666dc 00666600 FUN_00666600
 * Callees:
 * 004121d0 FUN_004121d0
 * 00dd5f60 memcpy
 * 006669d0 FUN_006669d0
 * 00dceec0 FUN_00dceec0
 */

int __thiscall FUN_006667b0(undefined4 param_1,ushort *param_2,short *param_3,undefined4 param_4)

{
  undefined4 uVar1;
  int iVar2;
  
  iVar2 = (int)param_2 + 3;
  memcpy(param_3,param_2,3,param_1);
  *(undefined4 *)((int)param_2 + 9) = param_4;
  *(undefined4 *)((int)param_2 + 5) = 100;
  uVar1 = FUN_006669d0(iVar2,(*param_2 - 3) + iVar2);
  *(undefined4 *)((int)param_2 + 5) = uVar1;
  FUN_004121d0(*(undefined4 *)((int)param_2 + 5),(int)param_2 + 0xd,*param_2 - 0xd);
  iVar2 = FUN_00dceec0(0,(int)param_3 + 3,iVar2,*param_2 - 3);
  if (iVar2 < 1) {
    iVar2 = -1;
  }
  else {
    *param_3 = (short)iVar2 + 3;
    iVar2 = iVar2 + 3;
  }
  return iVar2;
}



/* 00667c90 FUN_00667c90 */
/* Callers:
 * 00667c67 00667c50 FUN_00667c50
 * Callees:
 * 00414c40 FUN_00414c40
 * 00667e50 FUN_00667e50
 * 00668d60 FUN_00668d60
 * 00415e40 FUN_00415e40
 * 00561ac0 FUN_00561ac0
 * 0040d120 FUN_0040d120
 * 00412230 FUN_00412230
 * 00668ec0 FUN_00668ec0
 * 00412cc0 FUN_00412cc0
 * 00668bb0 FUN_00668bb0
 * 00412ef0 FUN_00412ef0
 * 00dd5b90 FUN_00dd5b90
 * 0040f6a0 FUN_0040f6a0
 * 00562260 FUN_00562260
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00667c90(undefined4 param_1,undefined4 *param_2)

{
  char cVar1;
  undefined4 uVar2;
  int iVar3;
  int iVar4;
  undefined1 auStack_3c [8];
  undefined1 auStack_34 [28];
  uint uStack_18;
  int iStack_14;
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00e8ddce;
  pvStack_10 = ExceptionList;
  uStack_18 = _DAT_01302704 ^ (uint)(auStack_34 + 0x30);
  ExceptionList = &pvStack_10;
  iStack_14 = 0;
  FUN_00415e40(uStack_18,param_1);
  uStack_8 = 0;
  FUN_0040f6a0();
  FUN_0040d120(&iStack_14,auStack_3c,auStack_34);
  cVar1 = FUN_00668d60();
  if (cVar1 == '\0') {
    FUN_00668bb0(0x15f97);
    uStack_8 = 0xffffffff;
    FUN_00414c40();
  }
  else if (iStack_14 == 0x36b8) {
    uVar2 = GetTickCount();
    FUN_00668ec0(uVar2);
    FUN_0040d120();
    iVar3 = FUN_00412ef0();
    if (param_2 != (undefined4 *)0x0) {
      *param_2 = *(undefined4 *)(iVar3 + 0xd);
      param_2[1] = *(undefined4 *)(iVar3 + 0x11);
    }
    FUN_0040d120();
    iVar4 = FUN_00412cc0();
    FUN_00667e50(iVar3 + 0x26,iVar4 + -0x26,0x12ee580);
    FUN_00412230(*(undefined4 *)(iVar3 + 0x1e),iVar3 + 0x26,iVar4 + -0x26);
    FUN_00562260();
    FUN_00561ac0(iVar3 + 0x26,*(undefined2 *)(iVar3 + 0x26));
    FUN_00668bb0(0);
    uStack_8 = 0xffffffff;
    FUN_00414c40();
  }
  else {
    FUN_00668bb0(0x15f98);
    uStack_8 = 0xffffffff;
    FUN_00414c40();
  }
  ExceptionList = pvStack_10;
  FUN_00dd5b90();
  return;
}



/* 00dcedf0 FUN_00dcedf0 */
/* Callers:
 * 00dcef17 00dceec0 FUN_00dceec0
 * 00dcefae 00dcef50 FUN_00dcef50
 * Callees:
 * 00dd5470 FUN_00dd5470
 */

void FUN_00dcedf0(int param_1)

{
  FUN_00dd5470(param_1 + 0xc,0x80);
  return;
}



/* 00dced50 FUN_00dced50 */
/* Callers:
 * 00dcef0f 00dceec0 FUN_00dceec0
 * 00dcef2e 00dceec0 FUN_00dceec0
 * 00dcefa6 00dcef50 FUN_00dcef50
 * 00dcefc3 00dcef50 FUN_00dcef50
 * Callees:
 */

void FUN_00dced50(int param_1,byte *param_2,int param_3)

{
  if (param_3 != 0) {
    if (8 < param_3) {
      *(byte *)((*param_2 & 0x7f) + 0xc + param_1) = *param_2;
      *(byte *)((param_2[1] & 0x7f) + 0xc + param_1) = param_2[1];
      *(byte *)((param_2[2] & 0x7f) + 0xc + param_1) = param_2[2];
      *(byte *)((param_2[3] & 0x7f) + 0xc + param_1) = param_2[3];
      *(byte *)((param_2[4] & 0x7f) + 0xc + param_1) = param_2[4];
      *(byte *)((param_2[5] & 0x7f) + 0xc + param_1) = param_2[5];
      *(byte *)((param_2[6] & 0x7f) + 0xc + param_1) = param_2[6];
      *(byte *)((param_2[7] & 0x7f) + 0xc + param_1) = param_2[7];
      return;
    }
    *(byte *)((*param_2 & 0x7f) + 0xc + param_1) = *param_2;
  }
  return;
}



/* 00dcf110 FUN_00dcf110 */
/* Callers:
 * 00dcef86 00dcef50 FUN_00dcef50
 * Callees:
 */

undefined4 __thiscall
FUN_00dcf110(int *param_1,undefined4 param_2,undefined4 param_3,undefined4 param_4)

{
  undefined4 uVar1;
  
  if ((int *)*param_1 == (int *)0x0) {
    return 0xffffffff;
  }
  uVar1 = (**(code **)(*(int *)*param_1 + 0xc))(param_3,param_4,param_2);
  return uVar1;
}



/* 00dcf140 FUN_00dcf140 */
/* Callers:
 * 00dcef00 00dceec0 FUN_00dceec0
 * 00dcef97 00dcef50 FUN_00dcef50
 * Callees:
 */

undefined4 __fastcall FUN_00dcf140(int *param_1)

{
  return *(undefined4 *)(*param_1 + 4);
}



/* 00412130 FUN_00412130 */
/* Callers:
 * 005635d2 005635c0 FUN_005635c0
 * 00563612 00563600 FUN_00563600
 * 009bd653 009bd620 FUN_009bd620
 * 009bd699 009bd620 FUN_009bd620
 * Callees:
 * 00412360 FUN_00412360
 */

undefined4 __thiscall FUN_00412130(int param_1,uint param_2)

{
  int iStack_4;
  
  param_2 = param_2 % 0x50;
  iStack_4 = param_1;
  FUN_00412360(&iStack_4,&param_2);
  if (iStack_4 == *(int *)(param_1 + 4)) {
    return 0;
  }
  return *(undefined4 *)(iStack_4 + 0x10);
}



/* 00412360 FUN_00412360 */
/* Callers:
 * 004121fa 004121d0 FUN_004121d0
 * 0041225a 00412230 FUN_00412230
 * 0041215a 00412130 FUN_00412130
 * Callees:
 */

void __thiscall FUN_00412360(int param_1,undefined4 *param_2,int *param_3)

{
  undefined4 *puVar1;
  undefined4 *puVar2;
  undefined4 *puVar3;
  
  puVar3 = *(undefined4 **)(param_1 + 4);
  if (*(char *)((int)puVar3[1] + 0x15) == '\0') {
    puVar1 = (undefined4 *)puVar3[1];
    do {
      if ((int)puVar1[3] < *param_3) {
        puVar2 = (undefined4 *)puVar1[2];
      }
      else {
        puVar2 = (undefined4 *)*puVar1;
        puVar3 = puVar1;
      }
      puVar1 = puVar2;
    } while (*(char *)((int)puVar2 + 0x15) == '\0');
  }
  if ((puVar3 == *(undefined4 **)(param_1 + 4)) || (*param_3 < (int)puVar3[3])) {
    *param_2 = *(undefined4 **)(param_1 + 4);
    return;
  }
  *param_2 = puVar3;
  return;
}



/* 00561820 FUN_00561820 */
/* Callers:
 * 0056167f 00561630 FUN_00561630
 * Callees:
 * 009970c0 FUN_009970c0
 */

void __thiscall FUN_00561820(undefined4 param_1,undefined4 param_2)

{
  FUN_009970c0(param_2,param_1);
  return;
}



/* 005623d0 FUN_005623d0 */
/* Callers:
 * 0056226d 00562260 FUN_00562260
 * Callees:
 * 00561860 FUN_00561860
 * 00408750 FUN_00408750
 * 00dd60c6 FUN_00dd60c6
 * 00a13d50 FUN_00a13d50
 * 00561890 FUN_00561890
 * 005625e0 FUN_005625e0
 * 00561950 FUN_00561950
 * 00562600 FUN_00562600
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 __fastcall FUN_005623d0(undefined4 param_1)

{
  char cVar1;
  undefined4 *puVar2;
  int iVar3;
  undefined4 uStack_38;
  undefined4 uStack_18;
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00e8442b;
  pvStack_10 = ExceptionList;
  ExceptionList = &pvStack_10;
  FUN_00561860(param_1,_DAT_01302704 ^ (int)register0x00000010 - 4U);
  uStack_8 = 0;
  cVar1 = FUN_005625e0();
  if (cVar1 == '\0') {
    puVar2 = (undefined4 *)FUN_00562600();
    uStack_18 = *puVar2;
    FUN_00a13d50();
    iVar3 = FUN_00408750(0xfc2c,uStack_18);
    uStack_8 = CONCAT31(uStack_8._1_3_,1);
    if (iVar3 != 0) {
      FUN_00561950();
    }
  }
  else {
    iVar3 = FUN_00dd60c6(0xfc2c,0x19214a8);
    uStack_8 = CONCAT31(uStack_8._1_3_,2);
    if (iVar3 == 0) {
      uStack_38 = 0;
    }
    else {
      uStack_38 = FUN_00561950();
    }
    uStack_18 = uStack_38;
  }
  uStack_8 = 0xffffffff;
  FUN_00561890();
  ExceptionList = pvStack_10;
  return uStack_18;
}



/* 00665950 FUN_00665950 */
/* Callers:
 * 00664bdf 00664b80 FUN_00664b80
 * 00664c63 00664b80 FUN_00664b80
 * 00664d32 00664cd0 FUN_00664cd0
 * 00664dbc 00664cd0 FUN_00664cd0
 * Callees:
 * 00dd5fb0 __alloca_probe
 * 004121d0 FUN_004121d0
 * 00666a10 FUN_00666a10
 * 00dd5b90 FUN_00dd5b90
 * 00666a50 FUN_00666a50
 * 00dd5f60 memcpy
 * 00665aa0 FUN_00665aa0
 * 00665870 FUN_00665870
 * 00665ba0 FUN_00665ba0
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall
FUN_00665950(int param_1,undefined4 param_2,undefined4 param_3,byte param_4,undefined4 param_5,
            undefined4 param_6,byte param_7)

{
  char cVar1;
  short asStack_1034 [6];
  undefined4 uStack_1027;
  undefined4 uStack_1023;
  undefined4 uStack_101f;
  undefined4 uStack_101b;
  byte bStack_1017;
  undefined4 uStack_1016;
  undefined4 uStack_1012;
  undefined1 auStack_100e [4102];
  undefined4 uStack_8;
  
  uStack_8 = _DAT_01302704 ^ (uint)(auStack_100e + 0x100a);
  cVar1 = FUN_00665870();
  if (cVar1 == '\0') {
    FUN_00666a10(0x15f92);
  }
  else {
    FUN_00666a50();
    uStack_101f = param_2;
    uStack_101b = param_3;
    memcpy(auStack_100e,param_5,param_6);
    uStack_1027 = *(undefined4 *)(param_1 + 0x18);
    uStack_1023 = *(undefined4 *)(param_1 + 0x1c);
    asStack_1034[0] = (short)param_6 + 0x26;
    bStack_1017 = bStack_1017 & 0xf0 | param_4 & 1 | (param_7 & 1) << 3;
    uStack_1016 = GetTickCount();
    uStack_1012 = 0;
    FUN_004121d0(uStack_1016,auStack_100e,param_6);
    FUN_00665aa0(auStack_100e,param_6,0x12ee540);
    FUN_00665ba0(asStack_1034,asStack_1034[0]);
  }
  FUN_00dd5b90();
  return;
}



/* 00666600 FUN_00666600 */
/* Callers:
 * 006661f8 006661d0 FUN_006661d0
 * Callees:
 * 006667b0 FUN_006667b0
 * 00dd5fb0 __alloca_probe
 * 00666b90 FUN_00666b90
 * 00dd5b90 FUN_00dd5b90
 * 00666aa0 FUN_00666aa0
 * 00666c10 FUN_00666c10
 * 00665ba0 FUN_00665ba0
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00666600(int param_1)

{
  char cVar1;
  ushort auStack_1064 [6];
  undefined4 uStack_1057;
  undefined4 uStack_1053;
  undefined4 uStack_104f;
  undefined4 uStack_104b;
  undefined4 uStack_1047;
  undefined4 uStack_1043;
  ushort *puStack_103c;
  undefined1 auStack_1038 [4116];
  uint uStack_24;
  uint uStack_14;
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00e8dc40;
  pvStack_10 = ExceptionList;
  uStack_14 = 0x66661b;
  uStack_24 = _DAT_01302704 ^ (uint)(auStack_1038 + 0x1034);
  ExceptionList = &pvStack_10;
  FUN_00666aa0(uStack_24);
  uStack_1057 = *(undefined4 *)(param_1 + 0x18);
  uStack_1053 = *(undefined4 *)(param_1 + 0x1c);
  uStack_1047 = *(undefined4 *)(param_1 + 0x28);
  uStack_1043 = *(undefined4 *)(param_1 + 0x2c);
  uStack_104f = *(undefined4 *)(param_1 + 0x20);
  uStack_104b = *(undefined4 *)(param_1 + 0x24);
  puStack_103c = auStack_1064;
  uStack_14 = (uint)auStack_1064[0];
  FUN_00666b90(param_1 + 0xc,1);
  uStack_8 = 0;
  uStack_14 = FUN_006667b0(puStack_103c,auStack_1038,*(undefined4 *)(param_1 + 0x184));
  if (uStack_14 == 0) {
    uStack_8 = 0xffffffff;
    FUN_00666c10();
  }
  else {
    cVar1 = FUN_00665ba0(auStack_1038,uStack_14);
    if (cVar1 == '\0') {
      uStack_8 = 0xffffffff;
      FUN_00666c10();
    }
    else {
      *(int *)(param_1 + 0x184) = *(int *)(param_1 + 0x184) + 1;
      uStack_8 = 0xffffffff;
      FUN_00666c10();
    }
  }
  ExceptionList = pvStack_10;
  FUN_00dd5b90();
  return;
}



/* 006656b0 FUN_006656b0 */
/* Callers:
 * 00664700 006646b0 FUN_006646b0
 * 00664761 006646b0 FUN_006646b0
 * Callees:
 * 00666b90 FUN_00666b90
 * 00665650 FUN_00665650
 * 00666a30 FUN_00666a30
 * 00666a10 FUN_00666a10
 * 00666130 FUN_00666130
 * 00666c10 FUN_00666c10
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 __thiscall FUN_006656b0(int param_1,undefined4 param_2,undefined2 param_3)

{
  char cVar1;
  undefined4 uVar2;
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00e8db67;
  pvStack_10 = ExceptionList;
  ExceptionList = &pvStack_10;
  FUN_00666b90(param_1 + 0xc,1,_DAT_01302704 ^ (int)register0x00000010 - 4U);
  uStack_8 = 0;
  if (*(int *)(param_1 + 0x10) == 1) {
    *(undefined4 *)(param_1 + 0x10) = 2;
    uStack_8 = 0xffffffff;
    FUN_00666c10();
    cVar1 = FUN_00665650(param_2,param_3,3);
    if (cVar1 == '\0') {
      uVar2 = _GetLastError_Diagnostics_System_BaseCat__YGIXZ();
      FUN_00666a10(uVar2);
      FUN_00666130();
      FUN_00666b90(param_1 + 0xc,1);
      uStack_8 = 1;
      *(undefined4 *)(param_1 + 0x10) = 1;
      FUN_00666a30();
      uStack_8 = 0xffffffff;
      FUN_00666c10();
      uVar2 = 0;
    }
    else {
      FUN_00666a10(0);
      uVar2 = 1;
    }
  }
  else {
    FUN_00666a10(0x15f93);
    uStack_8 = 0xffffffff;
    FUN_00666c10();
    uVar2 = 0;
  }
  ExceptionList = pvStack_10;
  return uVar2;
}



/* 005637f0 FUN_005637f0 */
/* Callers:
 * Callees:
 * 00563820 FUN_00563820
 */

void __thiscall FUN_005637f0(undefined4 param_1,undefined4 param_2,undefined2 param_3)

{
  undefined4 uVar1;
  
  uVar1 = Ordinal_11(param_2,param_3,param_1);
  FUN_00563820(uVar1);
  return;
}


