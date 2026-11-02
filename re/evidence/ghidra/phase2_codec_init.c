/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00dceec0 FUN_00dceec0 */
/* Callers:
 * 00563cd5 00563c10 FUN_00563c10
 * 0066685b 006667b0 FUN_006667b0
 * Callees:
 * 00dcf0e0 FUN_00dcf0e0
 * 00dcedf0 FUN_00dcedf0
 * 00dcf140 FUN_00dcf140
 * 00dced50 FUN_00dced50
 */

undefined4 __thiscall
FUN_00dceec0(int param_1,int param_2,undefined4 param_3,undefined4 param_4,undefined4 param_5)

{
  undefined4 uVar1;
  int iVar2;
  int unaff_EBX;
  
  if (param_2 == 1) {
    param_1 = param_1 + 0x98;
  }
  uVar1 = FUN_00dcf0e0(param_3);
  if (unaff_EBX != 0) {
    iVar2 = FUN_00dcf140();
    if (*(int *)(param_1 + 4) == iVar2) {
      FUN_00dced50(param_1,param_4,param_5);
      FUN_00dcedf0(param_1);
      *(undefined4 *)(param_1 + 4) = 1;
      return param_4;
    }
    FUN_00dced50(param_1,param_4,param_5);
    *(int *)(param_1 + 4) = *(int *)(param_1 + 4) + 1;
    uVar1 = param_5;
  }
  return uVar1;
}



/* 00dcf0c0 FUN_00dcf0c0 */
/* Callers:
 * 00dcec86 00dcec50 FUN_00dcec50
 * 00dcec99 00dcec50 FUN_00dcec50
 * 00dcec46 00dcec40 FUN_00dcec40
 * Callees:
 */

void __fastcall FUN_00dcf0c0(int *param_1)

{
  if ((undefined4 *)*param_1 != (undefined4 *)0x0) {
    (*(code *)**(undefined4 **)*param_1)(1);
    *param_1 = 0;
  }
  return;
}



/* 00dcecb0 FUN_00dcecb0 */
/* Callers:
 * 005636e5 00563640 FUN_00563640
 * 00563a76 00563a30 FUN_00563a30
 * 0056382f 00563820 FUN_00563820
 * 00666a97 00666a70 FUN_00666a70
 * Callees:
 * 00dd5470 FUN_00dd5470
 * 00dd5d9c memset
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00dcecb0(undefined4 *param_1)

{
  *param_1 = 0;
  param_1[1] = 1;
  param_1[2] = 1;
  param_1[0x23] = 0;
  memset(param_1 + 3,99,0x80);
  param_1[0x26] = 0;
  param_1[0x27] = 1;
  param_1[0x28] = 1;
  param_1[0x49] = 0;
  memset(param_1 + 0x29,99,0x80);
  FUN_00dd5470(0x12fc7f4,_DAT_012fc818);
  FUN_00dd5470(0x12fc7f4,_DAT_012fc818);
  return;
}


/* No defined function at 00dcec00 */

/* 00dcec40 FUN_00dcec40 */
/* Callers:
 * 00ee79e3 00ee79e0 Unwind@00ee79e0
 * 00ee7a13 00ee7a10 Unwind@00ee7a10
 * Callees:
 * 00dcf0c0 FUN_00dcf0c0
 */

void FUN_00dcec40(void)

{
  FUN_00dcf0c0();
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



/* 005635c0 FUN_005635c0 */
/* Callers:
 * 00563cb3 00563c10 FUN_00563c10
 * Callees:
 * 00412130 FUN_00412130
 */

void FUN_005635c0(undefined4 param_1,undefined4 param_2,undefined4 param_3)

{
  int *piVar1;
  undefined4 extraout_var;
  
  piVar1 = (int *)FUN_00412130(param_1);
  if (piVar1 != (int *)0x0) {
    (**(code **)(*piVar1 + 4))(param_2,param_3,extraout_var);
  }
  return;
}



/* 004121d0 FUN_004121d0 */
/* Callers:
 * 00415624 004154c0 FUN_004154c0
 * 00665a41 00665950 FUN_00665950
 * 0066790b 00667810 FUN_00667810
 * 00666836 006667b0 FUN_006667b0
 * Callees:
 * 00412360 FUN_00412360
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_004121d0(uint param_1,undefined4 param_2,undefined4 param_3)

{
  int iStack_4;
  
  param_1 = param_1 % 0x50;
  FUN_00412360(&iStack_4,&param_1);
  if ((iStack_4 != _DAT_01925898) && (*(int **)(iStack_4 + 0x10) != (int *)0x0)) {
    (**(code **)(**(int **)(iStack_4 + 0x10) + 4))(param_2,param_3);
  }
  return;
}



/* 00412010 FUN_00412010 */
/* Callers:
 * 00f88de8 00f88de0 FUN_00f88de0
 * Callees:
 * 004123d0 FUN_004123d0
 * 00dd5bac ??3@YAXPAX@Z
 * 00412660 FUN_00412660
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00412010(int param_1)

{
  char cVar1;
  int *piVar2;
  int *piVar3;
  uint uVar4;
  int *piVar5;
  undefined4 uStack_10;
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee9dd8;
  pvStack_c = ExceptionList;
  uVar4 = _DAT_01302704 ^ (int)register0x00000010 - 0x20U;
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  piVar5 = (int *)**(int **)(param_1 + 4);
  uStack_10 = param_1;
  if (piVar5 != *(int **)(param_1 + 4)) {
    do {
      if ((undefined4 *)piVar5[4] != (undefined4 *)0x0) {
        (*(code *)**(undefined4 **)piVar5[4])(1,uVar4);
      }
      if (*(char *)((int)piVar5 + 0x15) == '\0') {
        piVar2 = (int *)piVar5[2];
        if (*(char *)((int)piVar2 + 0x15) == '\0') {
          cVar1 = *(char *)(*piVar2 + 0x15);
          piVar5 = piVar2;
          piVar2 = (int *)*piVar2;
          while (cVar1 == '\0') {
            cVar1 = *(char *)(*piVar2 + 0x15);
            piVar5 = piVar2;
            piVar2 = (int *)*piVar2;
          }
        }
        else {
          cVar1 = *(char *)(piVar5[1] + 0x15);
          piVar3 = (int *)piVar5[1];
          piVar2 = piVar5;
          while ((piVar5 = piVar3, cVar1 == '\0' && (piVar2 == (int *)piVar5[2]))) {
            cVar1 = *(char *)(piVar5[1] + 0x15);
            piVar3 = (int *)piVar5[1];
            piVar2 = piVar5;
          }
        }
      }
    } while (piVar5 != *(int **)(param_1 + 4));
  }
  piVar5 = *(int **)(*(int *)(param_1 + 4) + 4);
  cVar1 = *(char *)((int)piVar5 + 0x15);
  while (cVar1 == '\0') {
    FUN_004123d0(piVar5[2]);
    piVar2 = (int *)*piVar5;
    __3_YAXPAX_Z(piVar5);
    piVar5 = piVar2;
    cVar1 = *(char *)((int)piVar2 + 0x15);
  }
  *(int *)(*(int *)(param_1 + 4) + 4) = *(int *)(param_1 + 4);
  *(undefined4 *)(param_1 + 8) = 0;
  *(undefined4 *)*(undefined4 *)(param_1 + 4) = *(undefined4 *)(param_1 + 4);
  *(int *)(*(int *)(param_1 + 4) + 8) = *(int *)(param_1 + 4);
  uStack_4 = 0xffffffff;
  FUN_00412660(&uStack_10,**(undefined4 **)(param_1 + 4),*(undefined4 **)(param_1 + 4));
  __3_YAXPAX_Z(*(undefined4 *)(param_1 + 4));
  *(undefined4 *)(param_1 + 4) = 0;
  *(undefined4 *)(param_1 + 8) = 0;
  ExceptionList = pvStack_c;
  return;
}



/* 009bd620 FUN_009bd620 */
/* Callers:
 * 009bd810 009bd800 FUN_009bd800
 * Callees:
 * 00412130 FUN_00412130
 * 00a68740 FUN_00a68740
 * 0041fd60 FUN_0041fd60
 * 009b6d90 FUN_009b6d90
 */

void FUN_009bd620(undefined4 param_1)

{
  int iVar1;
  undefined4 *puVar2;
  
  FUN_009b6d90(param_1);
  FUN_00a68740();
  FUN_0041fd60();
  iVar1 = GetTickCount();
  puVar2 = (undefined4 *)FUN_00412130(iVar1);
  if ((puVar2 != (undefined4 *)0x0) && (puVar2 != (undefined4 *)0x0)) {
    (**(code **)*puVar2)(1);
  }
  puVar2 = (undefined4 *)FUN_00412130(iVar1 + 7);
  if ((puVar2 != (undefined4 *)0x0) && (puVar2 != (undefined4 *)0x0)) {
    (**(code **)*puVar2)(1);
  }
  return;
}



/* 00666a50 FUN_00666a50 */
/* Callers:
 * 00665989 00665950 FUN_00665950
 * 00667849 00667810 FUN_00667810
 * Callees:
 * 00666ac0 FUN_00666ac0
 */

undefined4 __fastcall FUN_00666a50(undefined4 param_1)

{
  FUN_00666ac0();
  return param_1;
}



/* 00666aa0 FUN_00666aa0 */
/* Callers:
 * 0066663b 00666600 FUN_00666600
 * Callees:
 * 00666b20 FUN_00666b20
 */

undefined4 __fastcall FUN_00666aa0(undefined4 param_1)

{
  FUN_00666b20();
  return param_1;
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
 * 00561890 FUN_00561890
 * 00a13d50 FUN_00a13d50
 * 00561950 FUN_00561950
 * 00562600 FUN_00562600
 * 00dd60c6 FUN_00dd60c6
 * 00561860 FUN_00561860
 * 00408750 FUN_00408750
 * 005625e0 FUN_005625e0
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



/* 00563c10 FUN_00563c10 */
/* Callers:
 * 00a3892e 00a38850 FUN_00a38850
 * Callees:
 * 00dd5f60 memcpy
 * 00564140 FUN_00564140
 * 00dceec0 FUN_00dceec0
 * 005642d0 FUN_005642d0
 * 00564380 FUN_00564380
 * 005635c0 FUN_005635c0
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



/* 00665950 FUN_00665950 */
/* Callers:
 * 00664bdf 00664b80 FUN_00664b80
 * 00664c63 00664b80 FUN_00664b80
 * 00664d32 00664cd0 FUN_00664cd0
 * 00664dbc 00664cd0 FUN_00664cd0
 * Callees:
 * 00dd5f60 memcpy
 * 00666a10 FUN_00666a10
 * 00665aa0 FUN_00665aa0
 * 00665ba0 FUN_00665ba0
 * 00666a50 FUN_00666a50
 * 00dd5fb0 __alloca_probe
 * 004121d0 FUN_004121d0
 * 00dd5b90 FUN_00dd5b90
 * 00665870 FUN_00665870
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


