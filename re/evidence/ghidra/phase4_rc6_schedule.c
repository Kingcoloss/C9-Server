/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00e6f950 FUN_00e6f950 */
/* Callers:
 * Callees:
 * 00dcf3f0 FUN_00dcf3f0
 * 00e6e210 FUN_00e6e210
 * 00dd5d9c memset
 * 00e71c20 FUN_00e71c20
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00e6f950(int param_1,undefined4 param_2,undefined4 param_3,int *param_4)

{
  undefined *puVar1;
  int iVar2;
  uint *puVar3;
  int iVar4;
  byte bVar5;
  undefined4 uVar6;
  int iVar7;
  uint uVar8;
  undefined **ppuVar9;
  uint uVar10;
  uint uVar11;
  undefined *puVar12;
  int unaff_ESI;
  undefined *puVar13;
  undefined4 uStack_1c;
  undefined *puStack_18;
  undefined *puStack_14;
  uint uStack_10;
  undefined *puStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  uStack_4 = 0xffffffff;
  puStack_8 = &DAT_00ee8b28;
  puStack_c = ExceptionList;
  ExceptionList = &puStack_c;
  if (param_1 == 0) {
    iVar7 = 0;
  }
  else {
    iVar7 = param_1 + 4;
  }
  uStack_1c = 0x14;
  (**(code **)(*param_4 + 4))
            (&UNK_012241c0,0x12ebf1c,&uStack_1c,_DAT_01302704 ^ (int)register0x00000010 - 0x2cU);
  FUN_00e71c20(unaff_ESI,iVar7);
  *(int *)(param_1 + 0x10) = unaff_ESI;
  uVar6 = *(undefined4 *)(param_1 + 0x1c);
  iVar7 = unaff_ESI * 2 + 4;
  if (*(int *)(param_1 + 0x18) != iVar7) {
    memset(uVar6,0,*(int *)(param_1 + 0x18) * 4);
    free(uVar6);
    uVar6 = FUN_00dcf3f0(iVar7,0);
  }
  puVar13 = puStack_c;
  *(undefined4 *)(param_1 + 0x1c) = uVar6;
  *(int *)(param_1 + 0x18) = iVar7;
  puVar1 = puStack_c + 3;
  puStack_c = (undefined *)((uint)puVar1 >> 2);
  puStack_8 = (undefined *)0x1;
  ppuVar9 = &puStack_8;
  if ((undefined *)((uint)puVar1 >> 2) != (undefined *)0x0) {
    ppuVar9 = &puStack_c;
  }
  puVar1 = *ppuVar9;
  puStack_c = puVar1;
  iVar7 = FUN_00dcf3f0(puVar1,0);
  FUN_00e6e210(0,iVar7,puVar1,puStack_18,puVar13);
  **(undefined4 **)(param_1 + 0x1c) = 0xb7e15163;
  uVar8 = 1;
  if (1 < *(uint *)(param_1 + 0x18)) {
    do {
      iVar4 = uVar8 * 4;
      iVar2 = uVar8 * 4;
      uVar8 = uVar8 + 1;
      *(int *)(*(int *)(param_1 + 0x1c) + iVar2) =
           *(int *)(*(int *)(param_1 + 0x1c) + -4 + iVar4) + -0x61c88647;
    } while (uVar8 < *(uint *)(param_1 + 0x18));
  }
  puVar13 = (undefined *)0x0;
  uVar8 = 0;
  puStack_18 = *(undefined **)(param_1 + 0x18);
  uStack_10 = 0;
  ppuVar9 = &puStack_14;
  if (puVar1 <= *(undefined **)(param_1 + 0x18)) {
    ppuVar9 = &puStack_18;
  }
  puStack_18 = (undefined *)((int)*ppuVar9 * 3);
  puVar12 = puVar1;
  if (puStack_18 != (undefined *)0x0) {
    do {
      puVar3 = (uint *)(*(int *)(param_1 + 0x1c) + ((uint)puVar13 % *(uint *)(param_1 + 0x18)) * 4);
      uVar8 = *puVar3 + uStack_10 + uVar8;
      uVar8 = uVar8 * 8 | uVar8 >> 0x1d;
      *puVar3 = uVar8;
      uVar11 = (uint)puVar13 % (uint)puVar12;
      puVar13 = puVar13 + 1;
      uVar10 = *(int *)(iVar7 + uVar11 * 4) + uStack_10 + uVar8;
      bVar5 = (char)uVar8 + (char)uStack_10 & 0x1f;
      uStack_10 = uVar10 << bVar5 | uVar10 >> 0x20 - bVar5;
      *(uint *)(iVar7 + uVar11 * 4) = uStack_10;
      puVar12 = puStack_14;
    } while (puVar13 < puStack_18);
  }
  memset(iVar7,0,(int)puVar12 * 4);
  free(iVar7);
  ExceptionList = puVar1;
  return;
}


