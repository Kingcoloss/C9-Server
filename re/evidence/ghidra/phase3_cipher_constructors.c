/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00dd3680 FUN_00dd3680 */
/* Callers:
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd3680(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_ESI;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7e23;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -10);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9e68;
  FUN_00e622e0(1,uVar1);
  param_1[0xc] = 4;
  *(undefined1 *)((int)param_1 + 0x29) = 1;
  param_1[0xd] = param_1 + 6;
  param_1[2] = &UNK_011d9b44;
  param_1[3] = &UNK_011d9b84;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x1a] = 4;
  *(undefined1 *)((int)param_1 + 0x61) = 1;
  param_1[0x1b] = param_1 + 0x14;
  param_1[0x10] = &UNK_011d9bb4;
  param_1[0x11] = &UNK_011d9bf4;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd2880 FUN_00dd2880 */
/* Callers:
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd2880(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_ESI;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7d33;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -10);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9df0;
  FUN_00e622e0(1,uVar1);
  param_1[0xc] = 3;
  *(undefined1 *)((int)param_1 + 0x2d) = 1;
  param_1[0xd] = param_1 + 8;
  param_1[2] = &UNK_011d95ec;
  param_1[3] = &UNK_011d962c;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x18] = 3;
  *(undefined1 *)((int)param_1 + 0x5d) = 1;
  param_1[0x19] = param_1 + 0x14;
  param_1[0xe] = &UNK_011d9664;
  param_1[0xf] = &UNK_011d96a4;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd2c10 FUN_00dd2c10 */
/* Callers:
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd2c10(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_ESI;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7d63;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -10);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9e04;
  FUN_00e622e0(1,uVar1);
  param_1[0x2a] = 0x20;
  *(undefined1 *)((int)param_1 + 0xa1) = 1;
  param_1[0x2b] = param_1 + 8;
  param_1[2] = &UNK_011d96d4;
  param_1[3] = &UNK_011d9714;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x54] = 0x20;
  *(undefined1 *)((int)param_1 + 0x149) = 1;
  param_1[0x55] = param_1 + 0x32;
  param_1[0x2c] = &UNK_011d974c;
  param_1[0x2d] = &UNK_011d978c;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd2ea0 FUN_00dd2ea0 */
/* Callers:
 * Callees:
 * 00dd1d30 FUN_00dd1d30
 * 00dd1dd0 FUN_00dd1dd0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd2ea0(undefined4 *param_1)

{
  uint uVar1;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7d93;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -6);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9e18;
  FUN_00dd1d30(uVar1);
  uStack_4 = CONCAT31(uStack_4._1_3_,1);
  FUN_00dd1dd0();
  ExceptionList = pvStack_c;
  return param_1;
}



/* 00dd3000 FUN_00dd3000 */
/* Callers:
 * Callees:
 * 00dd1fa0 FUN_00dd1fa0
 * 00dd1e30 FUN_00dd1e30
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd3000(undefined4 *param_1)

{
  uint uVar1;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7d93;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -6);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9e2c;
  FUN_00dd1e30(uVar1);
  uStack_4 = CONCAT31(uStack_4._1_3_,1);
  FUN_00dd1fa0();
  ExceptionList = pvStack_c;
  return param_1;
}



/* 00dd31a0 FUN_00dd31a0 */
/* Callers:
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd31a0(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_ESI;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7dc3;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -10);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9e40;
  FUN_00e622e0(1,uVar1);
  param_1[0x30] = 0x28;
  *(undefined1 *)((int)param_1 + 0xb9) = 1;
  param_1[0x31] = param_1 + 6;
  param_1[2] = &UNK_011d9974;
  param_1[3] = &UNK_011d99b4;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x60] = 0x28;
  *(undefined1 *)((int)param_1 + 0x179) = 1;
  param_1[0x61] = param_1 + 0x36;
  param_1[0x32] = &UNK_011d99ec;
  param_1[0x33] = &UNK_011d9a2c;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd0c30 FUN_00dd0c30 */
/* Callers:
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd0c30(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_ESI;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7bf3;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -10);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9584;
  FUN_00e622e0(1,uVar1);
  param_1[0x3c] = 0x34;
  *(undefined1 *)((int)param_1 + 0xe9) = 1;
  param_1[0x3d] = param_1 + 6;
  param_1[2] = &UNK_011d8cf4;
  param_1[3] = &UNK_011d8d34;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x78] = 0x34;
  *(undefined1 *)((int)param_1 + 0x1d9) = 1;
  param_1[0x79] = param_1 + 0x42;
  param_1[0x3e] = &UNK_011d8da4;
  param_1[0x3f] = &UNK_011d8de4;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd3430 FUN_00dd3430 */
/* Callers:
 * 00dd53e7 00dd5070 FUN_00dd5070
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd3430(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_ESI;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7df3;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -10);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9e54;
  FUN_00e622e0(1,uVar1);
  param_1[0x10] = 8;
  *(undefined1 *)((int)param_1 + 0x39) = 1;
  param_1[0x11] = param_1 + 6;
  param_1[2] = &UNK_011d9a5c;
  param_1[3] = &UNK_011d9a9c;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x20] = 8;
  *(undefined1 *)((int)param_1 + 0x79) = 1;
  param_1[0x21] = param_1 + 0x16;
  param_1[0x12] = &UNK_011d9ad4;
  param_1[0x13] = &UNK_011d9b14;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd38d0 FUN_00dd38d0 */
/* Callers:
 * Callees:
 * 00dd24d0 FUN_00dd24d0
 * 00dd2340 FUN_00dd2340
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd38d0(undefined4 *param_1)

{
  uint uVar1;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7e53;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -6);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9e7c;
  FUN_00dd2340(uVar1);
  uStack_4 = CONCAT31(uStack_4._1_3_,1);
  FUN_00dd24d0();
  ExceptionList = pvStack_c;
  return param_1;
}



/* 00dd0ec0 FUN_00dd0ec0 */
/* Callers:
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd0ec0(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_ESI;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7c23;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -10);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9598;
  FUN_00e622e0(1,uVar1);
  param_1[0x28] = 0x20;
  *(undefined1 *)((int)param_1 + 0x99) = 1;
  param_1[0x29] = param_1 + 6;
  param_1[2] = &UNK_011d8e14;
  param_1[3] = &UNK_011d8e54;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x50] = 0x20;
  *(undefined1 *)((int)param_1 + 0x139) = 1;
  param_1[0x51] = param_1 + 0x2e;
  param_1[0x2a] = &UNK_011d8e84;
  param_1[0x2b] = &UNK_011d8ec4;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd1150 FUN_00dd1150 */
/* Callers:
 * Callees:
 * 00dd02e0 FUN_00dd02e0
 * 00dd0150 FUN_00dd0150
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd1150(undefined4 *param_1)

{
  uint uVar1;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7c53;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -6);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d95ac;
  FUN_00dd0150(uVar1);
  uStack_4 = CONCAT31(uStack_4._1_3_,1);
  FUN_00dd02e0();
  ExceptionList = pvStack_c;
  return param_1;
}



/* 00dd1320 FUN_00dd1320 */
/* Callers:
 * Callees:
 * 00dcf700 FUN_00dcf700
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd1320(undefined4 *param_1)

{
  uint uVar1;
  undefined1 auStackY_9c [128];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7c83;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStackY_9c + 0x84);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d95c0;
  FUN_00dcf700(uVar1);
  param_1[2] = &UNK_011d9004;
  param_1[3] = &UNK_011d8fd8;
  uStack_4 = CONCAT31(uStack_4._1_3_,1);
  FUN_00dcf700();
  param_1[0x72] = &UNK_011d907c;
  param_1[0x73] = &UNK_011d9050;
  ExceptionList = pvStack_c;
  return param_1;
}



/* 00dd1630 FUN_00dd1630 */
/* Callers:
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd1630(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_EBX;
  undefined1 auStackY_ac [128];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7cb3;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStackY_ac + 0x8c);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d95d4;
  FUN_00e622e0(1,uVar1);
  *(undefined1 *)((int)param_1 + 0x21) = 1;
  param_1[0xb] = param_1 + 6;
  param_1[10] = 8;
  param_1[0x10] = 8;
  *(undefined1 *)((int)param_1 + 0x39) = 1;
  param_1[0x11] = param_1 + 0xc;
  param_1[0x12] = 0;
  param_1[2] = &UNK_011d90bc;
  param_1[3] = &UNK_011d90fc;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x1c] = 8;
  *(undefined1 *)((int)param_1 + 0x69) = 1;
  param_1[0x1d] = param_1 + 0x18;
  param_1[0x22] = 8;
  *(undefined1 *)((int)param_1 + 0x81) = 1;
  param_1[0x23] = param_1 + 0x1e;
  param_1[0x24] = 0;
  param_1[0x14] = &UNK_011d9134;
  param_1[0x15] = &UNK_011d9174;
  ExceptionList = unaff_EBX;
  return param_1;
}



/* 00dd3bc0 FUN_00dd3bc0 */
/* Callers:
 * Callees:
 * 00e622e0 FUN_00e622e0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd3bc0(undefined4 *param_1)

{
  uint uVar1;
  void *unaff_ESI;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7e83;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -10);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011d9e90;
  FUN_00e622e0(1,uVar1);
  param_1[0x288] = 10;
  *(undefined1 *)((int)param_1 + 0xa19) = 1;
  param_1[0x289] = param_1 + 6;
  param_1[2] = &UNK_011d9d0c;
  param_1[3] = &UNK_011d9d4c;
  puStack_8 = (undefined *)CONCAT31(puStack_8._1_3_,1);
  FUN_00e622e0(1);
  param_1[0x510] = 10;
  *(undefined1 *)((int)param_1 + 0x1439) = 1;
  param_1[0x511] = param_1 + 0x28e;
  param_1[0x28a] = &UNK_011d9d84;
  param_1[0x28b] = &UNK_011d9dc4;
  ExceptionList = unaff_ESI;
  return param_1;
}



/* 00dd4bb0 FUN_00dd4bb0 */
/* Callers:
 * Callees:
 * 00dd4650 FUN_00dd4650
 * 00dd4580 FUN_00dd4580
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd4bb0(undefined4 *param_1)

{
  uint uVar1;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7ef3;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -6);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011da080;
  FUN_00dd4580(uVar1);
  uStack_4 = CONCAT31(uStack_4._1_3_,1);
  FUN_00dd4650();
  ExceptionList = pvStack_c;
  return param_1;
}



/* 00dd4e90 FUN_00dd4e90 */
/* Callers:
 * Callees:
 * 00dd47e0 FUN_00dd47e0
 * 00dd4740 FUN_00dd4740
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 * __fastcall FUN_00dd4e90(undefined4 *param_1)

{
  uint uVar1;
  undefined1 auStack_e [2];
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  puStack_8 = &DAT_00ee7ef3;
  pvStack_c = ExceptionList;
  uVar1 = _DAT_01302704 ^ (uint)(auStack_e + -6);
  ExceptionList = &pvStack_c;
  uStack_4 = 0;
  *param_1 = &UNK_011da094;
  FUN_00dd4740(uVar1);
  uStack_4 = CONCAT31(uStack_4._1_3_,1);
  FUN_00dd47e0();
  ExceptionList = pvStack_c;
  return param_1;
}



/* 00a65430 FUN_00a65430 */
/* Callers:
 * 00a64977 00a64960 FUN_00a64960
 * Callees:
 * 00a655e0 FUN_00a655e0
 * 00dc1570 FUN_00dc1570
 */

void __thiscall
FUN_00a65430(uint3 param_1,undefined4 param_2,undefined4 param_3,undefined4 param_4,
            undefined4 param_5)

{
  undefined1 uVar1;
  uint uVar2;
  
  uVar2 = (uint)param_1;
  uVar1 = FUN_00dc1570(&param_2,&param_2,uVar2);
  FUN_00a655e0(param_2,param_3,param_4,param_5,uVar1,uVar2 >> 0x18);
  return;
}


