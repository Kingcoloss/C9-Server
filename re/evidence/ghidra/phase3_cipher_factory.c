/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00dd5070 FUN_00dd5070 */
/* Callers:
 * 00dd59f2 00dd5470 FUN_00dd5470
 * Callees:
 * 00dd5ce4 ??2@YAPAXI@Z
 * 00dd3430 FUN_00dd3430
 */

/* WARNING: Removing unreachable block (ram,0x00dd5239) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00dd5070(int *param_1,uint param_2,byte *param_3,undefined4 param_4)

{
  int iVar1;
  int *piVar2;
  void *unaff_EDI;
  undefined4 uStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  uStack_4 = 0xffffffff;
  puStack_8 = &DAT_00ee8006;
  uStack_c = ExceptionList;
  ExceptionList = &uStack_c;
  if ((undefined4 *)*param_1 != (undefined4 *)0x0) {
    (*(code *)**(undefined4 **)*param_1)(1,_DAT_01302704 ^ (int)register0x00000010 - 0x14U);
    *param_1 = 0;
  }
  switch(param_2 % 10) {
  case 0:
    iVar1 = __2_YAPAXI_Z(0x78);
    uStack_4 = 7;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd3680();
      goto LAB_00dd53f0;
    }
    break;
  case 1:
    iVar1 = __2_YAPAXI_Z(0x68);
    uStack_4 = 0;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd2880();
      goto LAB_00dd53f0;
    }
    break;
  case 2:
    iVar1 = __2_YAPAXI_Z(0x158);
    uStack_4 = 1;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd2c10();
      goto LAB_00dd53f0;
    }
    break;
  default:
    iVar1 = __2_YAPAXI_Z(0x88);
    uStack_4 = 0x11;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd3430();
      goto LAB_00dd53f0;
    }
    break;
  case 4:
    iVar1 = __2_YAPAXI_Z(0x48);
    uStack_4 = 2;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd2ea0();
      goto LAB_00dd53f0;
    }
    break;
  case 5:
    iVar1 = __2_YAPAXI_Z(0x48);
    uStack_4 = 3;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd3000();
      goto LAB_00dd53f0;
    }
    break;
  case 6:
    iVar1 = __2_YAPAXI_Z(0x188);
    uStack_4 = 4;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd31a0();
      goto LAB_00dd53f0;
    }
    break;
  case 7:
    iVar1 = __2_YAPAXI_Z(0x1e8);
    uStack_4 = 5;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd0c30();
      goto LAB_00dd53f0;
    }
    break;
  case 8:
    iVar1 = __2_YAPAXI_Z(0x88);
    uStack_4 = 6;
    if (iVar1 != 0) {
      piVar2 = (int *)FUN_00dd3430();
      goto LAB_00dd53f0;
    }
    break;
  case 9:
    switch(*param_3 & 7) {
    case 0:
      iVar1 = __2_YAPAXI_Z(0x21a8);
      uStack_4 = 8;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd38d0();
        goto LAB_00dd53f0;
      }
      break;
    case 1:
      iVar1 = __2_YAPAXI_Z(0x148);
      uStack_4 = 9;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd0ec0();
        goto LAB_00dd53f0;
      }
      break;
    case 2:
      iVar1 = __2_YAPAXI_Z(0x268);
      uStack_4 = 10;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd1150();
        goto LAB_00dd53f0;
      }
      break;
    case 3:
      iVar1 = __2_YAPAXI_Z(0x388);
      uStack_4 = 0xb;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd1320();
        goto LAB_00dd53f0;
      }
      break;
    case 4:
      iVar1 = __2_YAPAXI_Z(0x98);
      uStack_4 = 0xc;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd1630();
        goto LAB_00dd53f0;
      }
      break;
    case 5:
      iVar1 = __2_YAPAXI_Z(0x1448);
      uStack_4 = 0xd;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd3bc0();
        goto LAB_00dd53f0;
      }
      break;
    case 6:
      iVar1 = __2_YAPAXI_Z(0x40);
      uStack_4 = 0xe;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd4bb0();
        goto LAB_00dd53f0;
      }
      break;
    case 7:
      iVar1 = __2_YAPAXI_Z(0x40);
      uStack_4 = 0xf;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd4e90();
        goto LAB_00dd53f0;
      }
      break;
    default:
      iVar1 = __2_YAPAXI_Z(0x21a8);
      uStack_4 = 0x10;
      if (iVar1 != 0) {
        piVar2 = (int *)FUN_00dd38d0();
        goto LAB_00dd53f0;
      }
    }
  }
  piVar2 = (int *)0x0;
LAB_00dd53f0:
  *param_1 = (int)piVar2;
  uStack_4 = 0xffffffff;
  (**(code **)(*piVar2 + 4))(param_3,param_4);
  ExceptionList = unaff_EDI;
  return;
}



/* 00dcf760 FUN_00dcf760 */
/* Callers:
 * 00dcf7f0 00dcf7f0 FUN_00dcf7f0
 * 00dd06f3 00dd06f0 FUN_00dd06f0
 * Callees:
 * 00dd5d9c memset
 */

void __fastcall FUN_00dcf760(int param_1)

{
  if (*(int *)(param_1 + 0xb4) == (-(param_1 + 0x60) & 0xfU) + param_1 + 0x60) {
    *(undefined1 *)(param_1 + 0xa9) = 0;
    memset(*(int *)(param_1 + 0xb4),0,*(int *)(param_1 + 0xb0) * 4);
  }
  if (*(int *)(param_1 + 0x5c) == param_1 + 0x10) {
    *(undefined1 *)(param_1 + 0x51) = 0;
    memset(*(int *)(param_1 + 0x5c),0,*(int *)(param_1 + 0x58) * 4);
  }
  return;
}



/* 00dcf800 FUN_00dcf800 */
/* Callers:
 * 00dd5a00 00dd5470 FUN_00dd5470
 * 00ee8057 00ee8051 Unwind@00ee8051
 * 00ee8062 00ee805c Unwind@00ee805c
 * 00ee806d 00ee8067 Unwind@00ee8067
 * 00ee8078 00ee8072 Unwind@00ee8072
 * 00ee8083 00ee807d Unwind@00ee807d
 * 00ee808e 00ee8088 Unwind@00ee8088
 * 00ee8099 00ee8093 Unwind@00ee8093
 * Callees:
 * 00dd5d9c memset
 */

void __fastcall FUN_00dcf800(int param_1)

{
  if (*(int *)(param_1 + 0xac) == param_1 + 0x60) {
    *(undefined1 *)(param_1 + 0xa1) = 0;
    memset(*(int *)(param_1 + 0xac),0,*(int *)(param_1 + 0xa8) * 4);
  }
  if (*(int *)(param_1 + 0x5c) == param_1 + 0x10) {
    *(undefined1 *)(param_1 + 0x51) = 0;
    memset(*(int *)(param_1 + 0x5c),0,*(int *)(param_1 + 0x58) * 4);
  }
  return;
}



/* 00dcf850 FUN_00dcf850 */
/* Callers:
 * 00dd0920 00dd0920 FUN_00dd0920
 * 00dd0c13 00dd0c10 FUN_00dd0c10
 * Callees:
 * 00dd5d9c memset
 */

void __fastcall FUN_00dcf850(int param_1)

{
  if (*(int *)(param_1 + 0x13c) == (-(param_1 + 0xa8) & 0xfU) + param_1 + 0xa8) {
    *(undefined1 *)(param_1 + 0x131) = 0;
    memset(*(int *)(param_1 + 0x13c),0,*(int *)(param_1 + 0x138) * 8);
  }
  if (*(int *)(param_1 + 0xa4) == param_1 + 0x18) {
    *(undefined1 *)(param_1 + 0x99) = 0;
    memset(*(int *)(param_1 + 0xa4),0,*(int *)(param_1 + 0xa0) * 8);
  }
  return;
}



/* 00dceec0 FUN_00dceec0 */
/* Callers:
 * 00563cd5 00563c10 FUN_00563c10
 * 0066685b 006667b0 FUN_006667b0
 * Callees:
 * 00dcf140 FUN_00dcf140
 * 00dced50 FUN_00dced50
 * 00dcf0e0 FUN_00dcf0e0
 * 00dcedf0 FUN_00dcedf0
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



/* 00dcef50 FUN_00dcef50 */
/* Callers:
 * 00563d59 00563d00 FUN_00563d00
 * 006668e9 00666890 FUN_00666890
 * Callees:
 * 00dcf110 FUN_00dcf110
 * 00dcf140 FUN_00dcf140
 * 00dced50 FUN_00dced50
 * 00dcedf0 FUN_00dcedf0
 */

int __thiscall
FUN_00dcef50(int param_1,int param_2,int param_3,undefined4 param_4,undefined4 param_5)

{
  int iVar1;
  int iVar2;
  
  if (param_3 == 0) {
    return -1;
  }
  if (param_2 == 1) {
    param_1 = param_1 + 0x98;
  }
  iVar1 = FUN_00dcf110(param_3,param_4,param_5);
  if (0 < iVar1) {
    iVar2 = FUN_00dcf140();
    if (*(int *)(param_1 + 8) == iVar2) {
      FUN_00dced50(param_1,param_3,iVar1);
      FUN_00dcedf0(param_1);
      *(undefined4 *)(param_1 + 8) = 1;
      return iVar1;
    }
    FUN_00dced50(param_1,param_3,iVar1);
    *(int *)(param_1 + 8) = *(int *)(param_1 + 8) + 1;
  }
  return iVar1;
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



/* 00412230 FUN_00412230 */
/* Callers:
 * 00667daa 00667c90 FUN_00667c90
 * 00415303 00414f70 FUN_00414f70
 * 0066693b 00666890 FUN_00666890
 * Callees:
 * 00412360 FUN_00412360
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_00412230(uint param_1,undefined4 param_2,undefined4 param_3)

{
  int iStack_4;
  
  param_1 = param_1 % 0x50;
  FUN_00412360(&iStack_4,&param_1);
  if ((iStack_4 != _DAT_01925898) && (*(int **)(iStack_4 + 0x10) != (int *)0x0)) {
    (**(code **)(**(int **)(iStack_4 + 0x10) + 8))(param_2,param_3);
  }
  return;
}



/* 00a64960 FUN_00a64960 */
/* Callers:
 * 00a646b0 00a64680 FUN_00a64680
 * 00a64cee 00a64bb0 FUN_00a64bb0
 * 00a64ee9 data reference
 * Callees:
 * 00a65430 FUN_00a65430
 */

int __thiscall FUN_00a64960(undefined4 param_1,int param_2,int param_3,undefined4 param_4)

{
  FUN_00a65430(param_2,param_3,param_4,param_1);
  return param_3 * 0x34 + param_2;
}



/* 00a64870 FUN_00a64870 */
/* Callers:
 * 00a646dd 00a64680 FUN_00a64680
 * Callees:
 * 008b68f0 FUN_008b68f0
 * 00a65400 FUN_00a65400
 * 00d39f00 FUN_00d39f00
 * 00614900 FUN_00614900
 * 00a64bb0 FUN_00a64bb0
 */

undefined4 FUN_00a64870(undefined4 param_1,undefined4 param_2,undefined4 param_3)

{
  int iVar1;
  undefined4 uVar2;
  undefined4 extraout_var;
  undefined4 uVar3;
  undefined1 auStack_10 [4];
  undefined1 auStack_c [4];
  undefined4 uStack_8;
  
  iVar1 = FUN_00d39f00();
  uVar3 = extraout_var;
  if (iVar1 == 0) {
    uStack_8 = 0;
  }
  else {
    uVar2 = FUN_008b68f0(auStack_c);
    uStack_8 = FUN_00614900(uVar2);
  }
  FUN_00a64bb0(param_2,1,param_3,uStack_8,uVar3);
  FUN_008b68f0(auStack_10,param_1,uStack_8);
  FUN_00a65400();
  return param_1;
}


