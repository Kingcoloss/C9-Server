/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00a63340 FUN_00a63340 */
/* Callers:
 * 00a62392 00a621f0 FUN_00a621f0
 * 00a623b4 00a621f0 FUN_00a621f0
 * 00ab54a3 00ab52e0 FUN_00ab52e0
 * 00ab5670 00ab55a0 FUN_00ab55a0
 * 00ab59e2 00ab5950 FUN_00ab5950
 * 00ab5b08 00ab5a70 FUN_00ab5a70
 * 00ab5c78 00ab5be0 FUN_00ab5be0
 * 00b087f6 00b086d0 FUN_00b086d0
 * Callees:
 * 00469d80 FUN_00469d80
 * 0045df90 FUN_0045df90
 * 00469920 FUN_00469920
 * 00469910 FUN_00469910
 * 0045dfd0 FUN_0045dfd0
 * 0045df00 FUN_0045df00
 * 00dd5b90 FUN_00dd5b90
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_00a63340(undefined4 param_1)

{
  undefined4 uVar1;
  undefined1 auStack_50 [28];
  undefined1 auStack_34 [28];
  uint uStack_18;
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00ebcc76;
  pvStack_10 = ExceptionList;
  uStack_18 = _DAT_01302704 ^ (uint)(auStack_34 + 0x30);
  ExceptionList = &pvStack_10;
  FUN_00469910(uStack_18);
  uStack_8 = 0;
  __0__basic_string_DU__char_traits_D_std__V__allocator_D_2__std__QAE_XZ();
  uStack_8._0_1_ = 1;
  __0__basic_string_DU__char_traits_D_std__V__allocator_D_2__std__QAE_PBD_Z(param_1);
  uStack_8._0_1_ = 2;
  FUN_00469d80(auStack_50,auStack_34);
  uStack_8._0_1_ = 1;
  __1__basic_string_DU__char_traits_D_std__V__allocator_D_2__std__QAE_XZ();
  uVar1 = _c_str___basic_string_DU__char_traits_D_std__V__allocator_D_2__std__QBEPBDXZ();
  FUN_0045df00(uVar1);
  uStack_8._0_1_ = 3;
  uVar1 = FUN_0045dfd0();
  __0__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAE_PB_W_Z(uVar1);
  uStack_8._0_1_ = 4;
  uVar1 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ();
  __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar1);
  uStack_8._0_1_ = 3;
  __1__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAE_XZ();
  uStack_8._0_1_ = 1;
  FUN_0045df90();
  uStack_8 = (uint)uStack_8._1_3_ << 8;
  __1__basic_string_DU__char_traits_D_std__V__allocator_D_2__std__QAE_XZ();
  uStack_8 = 0xffffffff;
  FUN_00469920();
  ExceptionList = pvStack_10;
  FUN_00dd5b90();
  return;
}



/* 00a63560 FUN_00a63560 */
/* Callers:
 * 00a628ae 00a621f0 FUN_00a621f0
 * 00a62d9d 00a621f0 FUN_00a621f0
 * Callees:
 * 00a65ab0 FUN_00a65ab0
 * 009d3e80 FUN_009d3e80
 * 00a68da0 FUN_00a68da0
 * 009d4390 FUN_009d4390
 * 00a64260 FUN_00a64260
 * 00a68d10 FUN_00a68d10
 * 009b6d90 FUN_009b6d90
 * 004fbff0 FUN_004fbff0
 * 009d3cb0 FUN_009d3cb0
 * 00a65b10 FUN_00a65b10
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00a63560(int param_1,undefined1 param_2)

{
  char cVar1;
  undefined4 uVar2;
  
  *(undefined1 *)(param_1 + 0x6c) = param_2;
  FUN_009b6d90(0);
  FUN_00a64260();
  FUN_009b6d90();
  FUN_00a68da0();
  uVar2 = _c_str___basic_string_DU__char_traits_D_std__V__allocator_D_2__std__QBEPBDXZ
                    (*(undefined2 *)(param_1 + 0x58));
  FUN_009b6d90(uVar2);
  cVar1 = FUN_00a68d10();
  if (cVar1 == '\0') {
    uVar2 = FUN_009d3e80(_DAT_0191a450);
    FUN_009d3cb0(_DAT_01913b88,uVar2);
  }
  else {
    FUN_00a65ab0(1);
    FUN_009d4390(1);
    FUN_009b6d90(0);
    FUN_004fbff0();
    FUN_00a65b10();
  }
  return;
}



/* 00a63630 FUN_00a63630 */
/* Callers:
 * 00a62de0 00a621f0 FUN_00a621f0
 * 00b1d7b8 00b1d770 FUN_00b1d770
 * Callees:
 * 00d39f00 FUN_00d39f00
 * 008fdec0 FUN_008fdec0
 */

undefined4 __thiscall FUN_00a63630(undefined4 param_1,int param_2)

{
  uint uVar1;
  int iVar2;
  undefined4 uVar3;
  uint uStack_8;
  
  uStack_8 = 0;
  while( true ) {
    uVar1 = FUN_00d39f00(param_1);
    if (uVar1 <= uStack_8) {
      return 0;
    }
    iVar2 = FUN_008fdec0(uStack_8);
    if (*(int *)(iVar2 + 1) == param_2) break;
    uStack_8 = uStack_8 + 1;
  }
  uVar3 = FUN_008fdec0(uStack_8);
  return uVar3;
}



/* 00a637b0 FUN_00a637b0 */
/* Callers:
 * 00a62918 00a621f0 FUN_00a621f0
 * Callees:
 * 00a64760 FUN_00a64760
 * 008eed10 FUN_008eed10
 * 009d3e80 FUN_009d3e80
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00a637b0(undefined4 param_1)

{
  char cVar1;
  undefined4 uStack_bc;
  undefined4 uStack_b8;
  undefined4 uStack_b4;
  undefined4 uStack_b0;
  undefined4 uStack_ac;
  undefined4 uStack_a8;
  undefined4 uStack_a4;
  undefined4 uStack_a0;
  undefined4 uStack_9c;
  undefined4 uStack_98;
  undefined4 uStack_94;
  undefined4 uStack_90;
  undefined4 uStack_8c;
  undefined4 uStack_88;
  undefined4 uStack_84;
  undefined4 uStack_80;
  undefined4 uStack_7c;
  undefined4 uStack_78;
  undefined4 uStack_74;
  undefined4 uStack_70;
  undefined4 uStack_6c;
  undefined4 uStack_68;
  undefined4 uStack_64;
  undefined4 uStack_60;
  undefined4 uStack_5c;
  undefined4 uStack_58;
  undefined4 uStack_54;
  undefined4 uStack_50;
  undefined4 uStack_4c;
  undefined4 uStack_48;
  undefined4 uStack_44;
  undefined4 uStack_40;
  undefined4 uStack_3c;
  undefined4 uStack_38;
  undefined4 uStack_34;
  undefined4 uStack_30;
  undefined4 uStack_2c;
  undefined4 uStack_28;
  undefined4 uStack_24;
  undefined4 uStack_20;
  undefined4 uStack_1c;
  undefined4 uStack_18;
  undefined4 uStack_14;
  undefined4 uStack_10;
  undefined4 uStack_c;
  undefined4 uStack_8;
  
  cVar1 = FUN_008eed10(param_1);
  if (cVar1 != '\0') {
    uStack_8 = FUN_009d3e80(_DAT_0191a458);
    FUN_00a64760(&uStack_8);
    uStack_c = FUN_009d3e80(_DAT_0191a45c);
    FUN_00a64760(&uStack_c);
    uStack_10 = FUN_009d3e80(_DAT_0191a460);
    FUN_00a64760(&uStack_10);
    uStack_14 = FUN_009d3e80(_DAT_0191a464);
    FUN_00a64760(&uStack_14);
    uStack_18 = FUN_009d3e80(_DAT_0191a468);
    FUN_00a64760(&uStack_18);
    uStack_1c = FUN_009d3e80(_DAT_0191a46c);
    FUN_00a64760(&uStack_1c);
    uStack_20 = FUN_009d3e80(_DAT_0191a470);
    FUN_00a64760(&uStack_20);
    uStack_24 = FUN_009d3e80(_DAT_0191a474);
    FUN_00a64760(&uStack_24);
    uStack_28 = FUN_009d3e80(_DAT_0191a478);
    FUN_00a64760(&uStack_28);
    uStack_2c = FUN_009d3e80(_DAT_0191a47c);
    FUN_00a64760(&uStack_2c);
    uStack_30 = FUN_009d3e80(_DAT_0191a480);
    FUN_00a64760(&uStack_30);
    uStack_34 = FUN_009d3e80(_DAT_0191a484);
    FUN_00a64760(&uStack_34);
    uStack_38 = FUN_009d3e80(_DAT_0191a488);
    FUN_00a64760(&uStack_38);
    uStack_3c = FUN_009d3e80(_DAT_0191a48c);
    FUN_00a64760(&uStack_3c);
    uStack_40 = FUN_009d3e80(_DAT_0191a490);
    FUN_00a64760(&uStack_40);
    uStack_44 = FUN_009d3e80(_DAT_0191a494);
    FUN_00a64760(&uStack_44);
    uStack_48 = FUN_009d3e80(_DAT_0191a498);
    FUN_00a64760(&uStack_48);
    uStack_4c = FUN_009d3e80(_DAT_0191a49c);
    FUN_00a64760(&uStack_4c);
    uStack_50 = FUN_009d3e80(_DAT_0191a4a0);
    FUN_00a64760(&uStack_50);
    uStack_54 = FUN_009d3e80(_DAT_0191a4a4);
    FUN_00a64760(&uStack_54);
    uStack_58 = FUN_009d3e80(_DAT_0191a4a8);
    FUN_00a64760(&uStack_58);
    uStack_5c = FUN_009d3e80(_DAT_0191a4ac);
    FUN_00a64760(&uStack_5c);
    uStack_60 = FUN_009d3e80(_DAT_0191a4b0);
    FUN_00a64760(&uStack_60);
    uStack_64 = FUN_009d3e80(_DAT_0191a4b4);
    FUN_00a64760(&uStack_64);
  }
  cVar1 = FUN_008eed10(param_1);
  if (cVar1 != '\0') {
    uStack_68 = FUN_009d3e80(_DAT_0191a4b8);
    FUN_00a64760(&uStack_68);
    uStack_6c = FUN_009d3e80(_DAT_0191a4bc);
    FUN_00a64760(&uStack_6c);
    uStack_70 = FUN_009d3e80(_DAT_0191a4c0);
    FUN_00a64760(&uStack_70);
    uStack_74 = FUN_009d3e80(_DAT_0191a4c4);
    FUN_00a64760(&uStack_74);
    uStack_78 = FUN_009d3e80(_DAT_0191a4c8);
    FUN_00a64760(&uStack_78);
    uStack_7c = FUN_009d3e80(_DAT_0191a4cc);
    FUN_00a64760(&uStack_7c);
    uStack_80 = FUN_009d3e80(_DAT_0191a4d0);
    FUN_00a64760(&uStack_80);
    uStack_84 = FUN_009d3e80(_DAT_0191a4d4);
    FUN_00a64760(&uStack_84);
    uStack_88 = FUN_009d3e80(_DAT_0191a4d8);
    FUN_00a64760(&uStack_88);
    uStack_8c = FUN_009d3e80(_DAT_0191a4dc);
    FUN_00a64760(&uStack_8c);
    uStack_90 = FUN_009d3e80(_DAT_0191a4e0);
    FUN_00a64760(&uStack_90);
    uStack_94 = FUN_009d3e80(_DAT_0191a4e4);
    FUN_00a64760(&uStack_94);
    uStack_98 = FUN_009d3e80(_DAT_0191a4e8);
    FUN_00a64760(&uStack_98);
    uStack_9c = FUN_009d3e80(_DAT_0191a4ec);
    FUN_00a64760(&uStack_9c);
    uStack_a0 = FUN_009d3e80(_DAT_0191a4f0);
    FUN_00a64760(&uStack_a0);
    uStack_a4 = FUN_009d3e80(_DAT_0191a4f4);
    FUN_00a64760(&uStack_a4);
    uStack_a8 = FUN_009d3e80(_DAT_0191a4f8);
    FUN_00a64760(&uStack_a8);
    uStack_ac = FUN_009d3e80(_DAT_0191a4fc);
    FUN_00a64760(&uStack_ac);
    uStack_b0 = FUN_009d3e80(_DAT_0191a500);
    FUN_00a64760(&uStack_b0);
    uStack_b4 = FUN_009d3e80(_DAT_0191a504);
    FUN_00a64760(&uStack_b4);
    uStack_b8 = FUN_009d3e80(_DAT_0191a508);
    FUN_00a64760(&uStack_b8);
    uStack_bc = FUN_009d3e80(_DAT_0191a50c);
    FUN_00a64760(&uStack_bc);
  }
  return;
}



/* 00a642a0 FUN_00a642a0 */
/* Callers:
 * 00a623bf 00a621f0 FUN_00a621f0
 * Callees:
 * 00a643d0 FUN_00a643d0
 */

undefined4 __fastcall FUN_00a642a0(undefined4 param_1)

{
  FUN_00a643d0();
  return param_1;
}



/* 00a642c0 FUN_00a642c0 */
/* Callers:
 * 00a62471 00a621f0 FUN_00a621f0
 * Callees:
 * 00a644e0 FUN_00a644e0
 */

undefined4 __fastcall FUN_00a642c0(undefined4 param_1)

{
  FUN_00a644e0();
  return param_1;
}



/* 00a642e0 FUN_00a642e0 */
/* Callers:
 * 00a62509 00a621f0 FUN_00a621f0
 * Callees:
 * 00a64380 FUN_00a64380
 */

undefined4 __fastcall FUN_00a642e0(undefined4 param_1)

{
  FUN_00a64380();
  return param_1;
}



/* 00a65ad0 FUN_00a65ad0 */
/* Callers:
 * 00a6243a 00a621f0 FUN_00a621f0
 * 00a624b6 00a621f0 FUN_00a621f0
 * 00a62628 00a621f0 FUN_00a621f0
 * Callees:
 */

undefined4 __fastcall FUN_00a65ad0(int param_1)

{
  return *(undefined4 *)(param_1 + 0xfc18);
}



/* 00a64220 FUN_00a64220 */
/* Callers:
 * 00a62a18 00a621f0 FUN_00a621f0
 * Callees:
 */

void __thiscall FUN_00a64220(undefined4 *param_1,undefined4 param_2,undefined4 param_3)

{
  *param_1 = param_2;
  param_1[1] = param_3;
  return;
}



/* 00a64680 FUN_00a64680 */
/* Callers:
 * 00a62b8c 00a621f0 FUN_00a621f0
 * Callees:
 * 00a64960 FUN_00a64960
 * 00a64870 FUN_00a64870
 * 00d39f00 FUN_00d39f00
 * 008ff050 FUN_008ff050
 * 00745010 FUN_00745010
 */

void __thiscall FUN_00a64680(int param_1,undefined4 param_2)

{
  uint uVar1;
  uint uVar2;
  undefined4 uVar3;
  undefined4 *puVar4;
  undefined1 auStack_10 [4];
  undefined1 auStack_c [4];
  undefined4 uStack_8;
  
  uVar1 = FUN_00d39f00();
  uVar2 = FUN_008ff050();
  if (uVar1 < uVar2) {
    uVar3 = FUN_00a64960(*(undefined4 *)(param_1 + 8),1,param_2);
    *(undefined4 *)(param_1 + 8) = uVar3;
  }
  else {
    puVar4 = (undefined4 *)FUN_00745010(auStack_c);
    uStack_8 = *puVar4;
    FUN_00a64870(auStack_10,uStack_8,param_2);
  }
  return;
}



/* 00666000 FUN_00666000 */
/* Callers:
 * Callees:
 * 005616b0 FUN_005616b0
 */

void __fastcall FUN_00666000(undefined4 param_1)

{
  FUN_005616b0(param_1);
  return;
}



/* 00dcf0b0 FUN_00dcf0b0 */
/* Callers:
 * 00dcee55 00dcee10 FUN_00dcee10
 * 00dcee8f 00dcee10 FUN_00dcee10
 * Callees:
 */

void __fastcall FUN_00dcf0b0(undefined4 *param_1)

{
  *param_1 = 0;
  param_1[1] = 0;
  return;
}


