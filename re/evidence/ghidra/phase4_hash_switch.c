/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00dd5470 FUN_00dd5470 */
/* Callers:
 * 00dced27 00dcecb0 FUN_00dcecb0
 * 00dced3d 00dcecb0 FUN_00dcecb0
 * 00dcee03 00dcedf0 FUN_00dcedf0
 * Callees:
 * 00dcf850 FUN_00dcf850
 * 00dcfc60 FUN_00dcfc60
 * 00e6d330 FUN_00e6d330
 * 00dcf800 FUN_00dcf800
 * 00dcfb40 FUN_00dcfb40
 * 00dcfab0 FUN_00dcfab0
 * 00dd5d9c memset
 * 00dcf760 FUN_00dcf760
 * 00dcf950 FUN_00dcf950
 * 00dcf8c0 FUN_00dcf8c0
 * 00dd5070 FUN_00dd5070
 * 00e6d650 FUN_00e6d650
 * 00dcfa00 FUN_00dcfa00
 * 00dcfbd0 FUN_00dcfbd0
 * 00dd5b90 FUN_00dd5b90
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
    goto LAB_00dd59fe;
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
LAB_00dd59fe:
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


