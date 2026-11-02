/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00dd3060 FUN_00dd3060 */
/* Callers:
 * Callees:
 */

undefined4 __thiscall FUN_00dd3060(int param_1,undefined4 param_2)

{
  (**(code **)(*(int *)(param_1 + 8) + 0x18))(param_2,0x10,0x130ae18);
  (**(code **)(*(int *)(param_1 + 0x28) + 0x18))(param_2,0x10,0x130ae18);
  *(undefined4 *)(param_1 + 4) = 0x100;
  return 1;
}



/* 00dd30a0 FUN_00dd30a0 */
/* Callers:
 * Callees:
 * 00dd5d9c memset
 * 00dd5fb0 __alloca_probe
 * 00dd5b90 FUN_00dd5b90
 * 00dd5f60 memcpy
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00dd30a0(int param_1,int param_2,int param_3,int param_4)

{
  uint uVar1;
  int iVar2;
  int iVar3;
  undefined1 auStack_20804 [66560];
  undefined1 auStack_10404 [66564];
  
  if ((((((byte)param_3 & 0xf) == 1) &&
       (uVar1 = (uint)*(byte *)(param_2 + -1 + param_3), uVar1 < 0x10)) &&
      (iVar3 = (param_3 - uVar1) + -1, -1 < iVar3)) && (param_4 != 0)) {
    memset(auStack_20804,0,param_3);
    memset(auStack_10404,0,param_3);
    memcpy(auStack_20804,param_2,param_3);
    iVar2 = 0;
    if (0 < iVar3) {
      do {
        (**(code **)(*(int *)(param_1 + 0x2c) + 0xc))(auStack_20804 + iVar2,0,auStack_10404 + iVar2)
        ;
        iVar2 = iVar2 + 0x10;
      } while (iVar2 < iVar3);
    }
    memcpy(param_4,auStack_10404,iVar3);
  }
  FUN_00dd5b90();
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



/* 00dd5470 FUN_00dd5470 */
/* Callers:
 * 00dced27 00dcecb0 FUN_00dcecb0
 * 00dced3d 00dcecb0 FUN_00dcecb0
 * 00dcee03 00dcedf0 FUN_00dcedf0
 * Callees:
 * 00dd5d9c memset
 * 00dd5070 FUN_00dd5070
 * 00dcfbd0 FUN_00dcfbd0
 * 00dd5b90 FUN_00dd5b90
 * 00e6d650 FUN_00e6d650
 * 00dcf800 FUN_00dcf800
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00dd5470(int param_1,byte *param_2,uint param_3)

{
  byte bVar1;
  undefined4 uVar2;
  int unaff_EBX;
  undefined1 auStack_a14 [2];
  undefined1 uStack_a12;
  byte bStack_a11;
  undefined *puStack_a10;
  int iStack_8d8;
  undefined *puStack_8d0;
  byte bStack_830;
  byte bStack_82f;
  byte bStack_82e;
  byte bStack_82d;
  byte bStack_82c;
  byte bStack_82b;
  byte bStack_82a;
  byte bStack_829;
  byte bStack_828;
  byte bStack_827;
  undefined1 auStack_820 [2032];
  void *pvStack_30;
  uint uStack_1c;
  void *pvStack_14;
  undefined *puStack_10;
  undefined4 uStack_c;
  
  uStack_c = 0xffffffff;
  puStack_10 = &DAT_00ee809e;
  pvStack_14 = ExceptionList;
  uStack_1c = _DAT_01302704 ^ (uint)auStack_a14;
  ExceptionList = &pvStack_14;
  memset(auStack_820,0,0x800,_DAT_01302704 ^ (uint)(auStack_a14 + -0xc));
  if (param_3 < 10) {
    bVar1 = *param_2;
  }
  else {
    bStack_a11 = param_2[1] ^ *param_2;
    bVar1 = (bStack_a11 << param_2[2] % 3 | param_2[3]) ^ param_2[4];
  }
  switch((uint)bVar1 % 0xb) {
  case 1:
    FUN_00dcf8c0();
    puStack_a10 = &UNK_011d91a4;
    uStack_c = 0;
    FUN_00e6d650(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_830 % 10;
    FUN_00dd5070((uint)bStack_830 % 10,&bStack_830,0x20);
    FUN_00dcf760();
    break;
  default:
    FUN_00dcfbd0();
    puStack_8d0 = &UNK_011d935c;
    uStack_c = 9;
    FUN_00e6d650(param_2,param_3);
    uVar2 = (**(code **)(iStack_8d8 + 0x1c))();
    (**(code **)(iStack_8d8 + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_827 % 10;
    FUN_00dd5070((uint)bStack_827 % 10,&bStack_830,0x10);
    goto code_r0x00dd59fe;
  case 3:
    FUN_00dcf950();
    puStack_a10 = &UNK_011d9214;
    uStack_c = 1;
    FUN_00e6d330(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_82f % 10;
    FUN_00dd5070((uint)bStack_82f % 10,&bStack_830,0x40);
    FUN_00dcf850();
    break;
  case 4:
    FUN_00dcfa00();
    puStack_a10 = &UNK_011d9284;
    uStack_c = 2;
    FUN_00e6d330(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_82e % 10;
    FUN_00dd5070((uint)bStack_82e % 10,&bStack_830,0x30);
    FUN_00dcf850();
    break;
  case 5:
    FUN_00dcfbd0();
    puStack_a10 = &UNK_011d92f4;
    uStack_c = 3;
    FUN_00e6d650(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_82d % 10;
    FUN_00dd5070((uint)bStack_82d % 10,&bStack_830,0x10);
    FUN_00dcf800();
    break;
  case 6:
    FUN_00dcfbd0();
    puStack_a10 = &UNK_011d935c;
    uStack_c = 4;
    FUN_00e6d650(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_82c % 10;
    FUN_00dd5070((uint)bStack_82c % 10,&bStack_830,0x10);
code_r0x00dd59fe:
    FUN_00dcf800();
    break;
  case 7:
    FUN_00dcfab0();
    puStack_a10 = &UNK_011d93c4;
    uStack_c = 5;
    FUN_00e6d650(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_82b % 10;
    FUN_00dd5070((uint)bStack_82b % 10,&bStack_830,0x14);
    FUN_00dcf800();
    break;
  case 8:
    FUN_00dcfb40();
    puStack_a10 = &UNK_011d9434;
    uStack_c = 6;
    FUN_00e6d650(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_82a % 10;
    FUN_00dd5070((uint)bStack_82a % 10,&bStack_830,0x28);
    FUN_00dcf800();
    break;
  case 9:
    FUN_00dcfbd0();
    puStack_a10 = &UNK_011d94a4;
    uStack_c = 7;
    FUN_00e6d650(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_829 % 10;
    FUN_00dd5070((uint)bStack_829 % 10,&bStack_830,0x10);
    FUN_00dcf800();
    break;
  case 10:
    FUN_00dcfc60();
    puStack_a10 = &UNK_011d9514;
    uStack_c = 8;
    FUN_00e6d650(param_2,param_3);
    uVar2 = (**(code **)(unaff_EBX + 0x1c))();
    (**(code **)(unaff_EBX + 0x38))(&bStack_828,uVar2);
    *(uint *)(param_1 + 4) = (uint)bStack_828 % 10;
    FUN_00dd5070((uint)bStack_828 % 10,&bStack_830,0x20);
    FUN_00dcf800();
  }
  ExceptionList = pvStack_30;
  FUN_00dd5b90();
  return;
}



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



/* 00e6d650 FUN_00e6d650 */
/* Callers:
 * 00dd59a3 00dd5470 FUN_00dd5470
 * Callees:
 * 00e6cf30 FUN_00e6cf30
 * 00dd5f60 memcpy
 * 00e6cea0 FUN_00e6cea0
 * 00dd5ecc _CxxThrowException
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00e6d650(int *param_1,int param_2,uint param_3)

{
  uint uVar1;
  uint uVar2;
  int iVar3;
  int iVar4;
  undefined4 uVar5;
  uint uVar6;
  undefined1 auStack_56 [2];
  undefined1 auStack_54 [20];
  undefined1 auStack_40 [52];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  uStack_4 = 0xffffffff;
  puStack_8 = &DAT_00ee8a79;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_56 + -0x12);
  ExceptionList = &pvStack_c;
  uVar6 = param_1[1];
  uVar2 = param_1[2];
  param_1[1] = uVar6 + param_3;
  if (uVar6 + param_3 < uVar6) {
    param_1[2] = uVar2 + 1;
  }
  if ((uint)param_1[2] < uVar2) {
    uVar5 = (**(code **)(*param_1 + 8))(auStack_54,uVar1);
    puStack_8 = (undefined *)0x0;
    FUN_00e6cea0(uVar5);
                    /* WARNING: Subroutine does not return */
    _CxxThrowException(auStack_40,&UNK_012d5248);
  }
  uVar2 = (**(code **)(*param_1 + 0x20))();
  uVar6 = uVar2 - 1 & uVar6;
  iVar3 = (**(code **)(*param_1 + 0x58))();
  if (uVar6 != 0) {
    iVar4 = iVar3 + uVar6;
    if (uVar6 + param_3 < uVar2) goto LAB_00e6d70d;
    memcpy(iVar4,param_2,uVar2 - uVar6);
    FUN_00e6cf30(iVar3);
    param_2 = param_2 + (uVar2 - uVar6);
    param_3 = param_3 + (uVar6 - uVar2);
  }
  uVar6 = param_3;
  if (uVar2 <= param_3) {
    if (param_2 == iVar3) {
      FUN_00e6cf30(iVar3);
      ExceptionList = pvStack_c;
      return;
    }
    uVar6 = (**(code **)(*param_1 + 0x54))(param_2,param_3);
    param_2 = param_2 + (param_3 - uVar6);
  }
  iVar4 = iVar3;
  param_3 = uVar6;
LAB_00e6d70d:
  memcpy(iVar4,param_2,param_3);
  ExceptionList = pvStack_c;
  return;
}



/* 00dd1e30 FUN_00dd1e30 */
/* Callers:
 * 00dd3039 00dd3000 FUN_00dd3000
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd1e30(undefined4 *param_1)

{
  void *unaff_ESI;
  undefined4 uStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  uStack_4 = 0xffffffff;
  puStack_8 = &DAT_00ee7d00;
  uStack_c = ExceptionList;
  ExceptionList = &uStack_c;
  FUN_00e622e0(1,_DAT_01302704 ^ (int)register0x00000010 - 0x14U);
  param_1[6] = 0;
  param_1[7] = 0;
  *param_1 = &UNK_011d98c4;
  param_1[1] = &UNK_011d9894;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd1fa0 FUN_00dd1fa0 */
/* Callers:
 * 00dd3046 00dd3000 FUN_00dd3000
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd1fa0(undefined4 *param_1)

{
  void *unaff_ESI;
  undefined4 uStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  uStack_4 = 0xffffffff;
  puStack_8 = &DAT_00ee7d00;
  uStack_c = ExceptionList;
  ExceptionList = &uStack_c;
  FUN_00e622e0(1,_DAT_01302704 ^ (int)register0x00000010 - 0x14U);
  param_1[6] = 0;
  param_1[7] = 0;
  *param_1 = &UNK_011d9934;
  param_1[1] = &UNK_011d9908;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd2970 FUN_00dd2970 */
/* Callers:
 * Callees:
 * 00dd5fb0 __alloca_probe
 * 00dd5d9c memset
 * 00dd5f60 memcpy
 * 00dd5b90 FUN_00dd5b90
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00dd2970(int param_1,undefined4 param_2,uint param_3,int param_4)

{
  int iVar1;
  int iVar2;
  int iVar3;
  undefined1 auStack_20844 [64];
  undefined1 auStack_20804 [66560];
  undefined1 auStack_10404 [66564];
  
  iVar2 = 0;
  if (param_3 % 0xc != 0) {
    iVar2 = 0xc - param_3 % 0xc;
  }
  iVar1 = param_3 + 1 + iVar2;
  if (param_4 != 0) {
    memset(auStack_20844,0,0x40);
    memset(auStack_20804,0,iVar1);
    memset(auStack_10404,0,iVar1);
    memcpy(auStack_20804,param_2,param_3);
    memcpy(auStack_20804 + param_3,auStack_20844,iVar2);
    iVar3 = 0;
    if (0 < iVar1) {
      do {
        (**(code **)(*(int *)(param_1 + 0xc) + 0xc))(auStack_20804 + iVar3,0,auStack_10404 + iVar3);
        iVar3 = iVar3 + 0xc;
      } while (iVar3 < iVar1);
    }
    auStack_10404[iVar1 + -1] = (char)iVar2;
    memcpy(param_4,auStack_10404,iVar1);
  }
  FUN_00dd5b90();
  return;
}


