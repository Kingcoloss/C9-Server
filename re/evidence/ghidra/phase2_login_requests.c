/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00a630f0 FUN_00a630f0 */
/* Callers:
 * 00da6139 00da6050 FUN_00da6050
 * Callees:
 * 00a68dd0 FUN_00a68dd0
 * 005fe840 FUN_005fe840
 * 009b6d90 FUN_009b6d90
 * 00a64300 FUN_00a64300
 * 009a7930 FUN_009a7930
 * 00a68fe0 FUN_00a68fe0
 */

void __fastcall FUN_00a630f0(undefined4 param_1)

{
  char cVar1;
  undefined2 auStack_24 [6];
  undefined8 uStack_17;
  undefined8 uStack_f;
  
  FUN_009b6d90(param_1);
  cVar1 = FUN_00a68dd0();
  if (cVar1 != '\0') {
    FUN_00a64300();
    uStack_17 = FUN_009a7930();
    uStack_f = FUN_005fe840();
    FUN_009b6d90(auStack_24,auStack_24[0]);
    FUN_00a68fe0();
  }
  return;
}



/* 00a63160 FUN_00a63160 */
/* Callers:
 * 00da6038 00da5ee0 FUN_00da5ee0
 * Callees:
 * 00a68dd0 FUN_00a68dd0
 * 00a64320 FUN_00a64320
 * 009b6d90 FUN_009b6d90
 * 00a68fe0 FUN_00a68fe0
 */

void __fastcall FUN_00a63160(undefined4 param_1)

{
  char cVar1;
  undefined2 auStack_14 [8];
  
  FUN_009b6d90(param_1);
  cVar1 = FUN_00a68dd0();
  if (cVar1 != '\0') {
    FUN_00a64320();
    FUN_009b6d90(auStack_14,auStack_14[0]);
    FUN_00a68fe0();
  }
  return;
}



/* 00a631b0 FUN_00a631b0 */
/* Callers:
 * 00b1d850 00b1d770 FUN_00b1d770
 * Callees:
 * 00a68dd0 FUN_00a68dd0
 * 009b6d90 FUN_009b6d90
 * 00a64340 FUN_00a64340
 * 00a68fe0 FUN_00a68fe0
 */

void __thiscall
FUN_00a631b0(undefined4 param_1,undefined4 param_2,undefined4 param_3,undefined4 param_4)

{
  char cVar1;
  undefined2 auStack_28 [6];
  undefined4 uStack_1b;
  undefined4 uStack_17;
  undefined4 uStack_13;
  
  FUN_009b6d90(param_1);
  cVar1 = FUN_00a68dd0();
  if (cVar1 != '\0') {
    FUN_00a64340();
    uStack_1b = param_2;
    uStack_17 = param_3;
    uStack_13 = param_4;
    FUN_009b6d90(auStack_28,auStack_28[0]);
    FUN_00a68fe0();
  }
  return;
}



/* 00a63760 FUN_00a63760 */
/* Callers:
 * 009bd059 009bcfd0 FUN_009bcfd0
 * Callees:
 * 00a68dd0 FUN_00a68dd0
 * 009b6d90 FUN_009b6d90
 * 00a64360 FUN_00a64360
 * 00a68fe0 FUN_00a68fe0
 */

void __fastcall FUN_00a63760(undefined4 param_1)

{
  char cVar1;
  undefined2 auStack_14 [8];
  
  FUN_009b6d90(param_1);
  cVar1 = FUN_00a68dd0();
  if (cVar1 != '\0') {
    FUN_00a64360();
    FUN_009b6d90(auStack_14,auStack_14[0]);
    FUN_00a68fe0();
  }
  return;
}



/* 00a38850 FUN_00a38850 */
/* Callers:
 * 00a687b4 00a68740 FUN_00a68740
 * 00a69004 00a68fe0 FUN_00a68fe0
 * 00a694af 00a69420 FUN_00a69420
 * 00a34b71 00a349c0 FUN_00a349c0
 * 00a34db5 00a34ba0 FUN_00a34ba0
 * 00a34f95 00a34ba0 FUN_00a34ba0
 * 00a350c1 00a34fc0 FUN_00a34fc0
 * 00a62441 00a621f0 FUN_00a621f0
 * 00a624bd 00a621f0 FUN_00a621f0
 * 00a6262f 00a621f0 FUN_00a621f0
 * 00a69649 00a694d0 FUN_00a694d0
 * 00cf6967 00cf6380 FUN_00cf6380
 * Callees:
 * 005620b0 FUN_005620b0
 * 00dd5f60 memcpy
 * 00563c10 FUN_00563c10
 * 00415db0 FUN_00415db0
 * 00dd5b90 FUN_00dd5b90
 * 00563930 FUN_00563930
 * 005641c0 FUN_005641c0
 * 00415dd0 FUN_00415dd0
 * 00dd5fb0 __alloca_probe
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00a38850(int param_1,undefined4 param_2,undefined4 param_3)

{
  char cVar1;
  uint uVar2;
  ushort uStack_1f858;
  char cStack_1f856;
  undefined4 uStack_fc44;
  uint uStack_fc40;
  ushort *puStack_fc3c;
  ushort auStack_fc38 [32266];
  uint uStack_24;
  uint uStack_18;
  ushort *puStack_14;
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00ebaa88;
  pvStack_10 = ExceptionList;
  puStack_14 = (ushort *)0xa3886b;
  uVar2 = _DAT_01302704 ^ (uint)(auStack_fc38 + 0x7e1a);
  ExceptionList = &pvStack_10;
  uStack_24 = uVar2;
  FUN_00415db0(param_1 + 0x164);
  uStack_8 = 0;
  cVar1 = FUN_005641c0(uVar2);
  if (cVar1 == '\0') {
    uStack_8 = 0xffffffff;
    FUN_00415dd0();
  }
  else {
    memcpy(&uStack_1f858,param_2,param_3);
    puStack_fc3c = &uStack_1f858;
    uStack_18 = (uint)uStack_1f858;
    if (cStack_1f856 != '\0') {
      uStack_18 = FUN_00563c10(puStack_fc3c,auStack_fc38,*(undefined4 *)(param_1 + 0x20));
      if (uStack_18 == 0) {
        uStack_8 = 0xffffffff;
        FUN_00415dd0();
        goto LAB_00a38a00;
      }
      *(int *)(param_1 + 0x20) = *(int *)(param_1 + 0x20) + 1;
    }
    puStack_14 = &uStack_1f858;
    uStack_fc40 = uStack_18;
    if ((char)puStack_fc3c[1] != '\0') {
      puStack_14 = auStack_fc38;
    }
    uStack_fc44 = 0;
    cVar1 = FUN_00563930(puStack_14,uStack_18,&uStack_fc44);
    if (cVar1 == '\0') {
      FUN_005620b0(param_1,uStack_fc44);
    }
    uStack_8 = 0xffffffff;
    FUN_00415dd0();
  }
LAB_00a38a00:
  ExceptionList = pvStack_10;
  FUN_00dd5b90();
  return;
}



/* 00a68d10 FUN_00a68d10 */
/* Callers:
 * 00a635bc 00a63560 FUN_00a63560
 * 00a634a8 00a63460 FUN_00a63460
 * Callees:
 * 00a68d40 FUN_00a68d40
 */

void __thiscall FUN_00a68d10(undefined4 param_1,undefined4 param_2,undefined2 param_3)

{
  undefined4 uVar1;
  
  uVar1 = Ordinal_11(param_2,param_3,param_1);
  FUN_00a68d40(uVar1);
  return;
}



/* 00563a10 FUN_00563a10 */
/* Callers:
 * 0056350d 00563220 FUN_00563220
 * Callees:
 */

undefined4 __fastcall FUN_00563a10(int param_1)

{
  *(undefined1 *)(param_1 + 0x30) = 1;
  return 0;
}



/* 006661d0 FUN_006661d0 */
/* Callers:
 * Callees:
 * 00666a70 FUN_00666a70
 * 00666600 FUN_00666600
 */

void FUN_006661d0(undefined4 param_1,undefined4 param_2,int param_3)

{
  _AddRef_Link_Network_BaseCat__YGHPAU_Handle_123__Z(param_2);
  *(undefined4 *)(param_3 + 8) = param_2;
  FUN_00666a70();
  FUN_00666600();
  _Recv_Link_Network_BaseCat__YG_NPAU_Handle_123_I_Z(param_2,0x400);
  return;
}


