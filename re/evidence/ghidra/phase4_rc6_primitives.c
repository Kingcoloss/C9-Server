/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00e625e0 FUN_00e625e0 */
/* Callers:
 * Callees:
 * 00e62400 FUN_00e62400
 */

void __thiscall FUN_00e625e0(int *param_1,undefined4 param_2,undefined4 param_3)

{
  undefined4 unaff_retaddr;
  
  FUN_00e62400(param_3);
  (**(code **)(*param_1 + 0x38))(unaff_retaddr,param_3,param_3);
  return;
}



/* 00e6fb30 FUN_00e6fb30 */
/* Callers:
 * Callees:
 */

void __thiscall FUN_00e6fb30(int param_1,uint *param_2,uint *param_3,uint *param_4)

{
  int iVar1;
  byte bVar2;
  byte bVar3;
  uint uVar4;
  uint uVar5;
  uint uVar6;
  uint uVar7;
  uint uVar8;
  int *piVar9;
  uint uVar10;
  int iStack_8;
  
  uVar5 = *param_2;
  piVar9 = *(int **)(param_1 + 0x18);
  iStack_8 = *(int *)(param_1 + 0xc);
  uVar7 = param_2[2];
  uVar10 = *piVar9 + param_2[1];
  uVar8 = param_2[3] + piVar9[1];
  piVar9 = piVar9 + 2;
  if (iStack_8 != 0) {
    uVar6 = uVar5;
    param_2 = (uint *)uVar7;
    do {
      uVar5 = uVar10;
      uVar7 = uVar8;
      uVar10 = (uVar5 * 2 + 1) * uVar5;
      uVar8 = (uVar7 * 2 + 1) * uVar7;
      uVar4 = uVar10 * 0x20 | uVar10 >> 0x1b;
      uVar8 = uVar8 * 0x20 | uVar8 >> 0x1b;
      uVar6 = uVar4 ^ uVar6;
      uVar10 = uVar8 ^ (uint)param_2;
      bVar2 = (byte)uVar8 & 0x1f;
      bVar3 = (byte)uVar4 & 0x1f;
      uVar10 = (uVar10 << bVar3 | uVar10 >> 0x20 - bVar3) + piVar9[1];
      uVar8 = *piVar9 + (uVar6 << bVar2 | uVar6 >> 0x20 - bVar2);
      piVar9 = piVar9 + 2;
      iStack_8 = iStack_8 + -1;
      uVar6 = uVar5;
      param_2 = (uint *)uVar7;
    } while (iStack_8 != 0);
  }
  iVar1 = piVar9[1];
  if (param_3 == (uint *)0x0) {
    uVar6 = 0;
  }
  else {
    uVar6 = *param_3;
  }
  *param_4 = uVar6 ^ *piVar9 + uVar5;
  if ((param_3 == (uint *)0x0) || (param_3 = param_3 + 1, param_3 == (uint *)0x0)) {
    uVar5 = 0;
  }
  else {
    uVar5 = *param_3;
  }
  param_4[1] = uVar5 ^ uVar10;
  if ((param_3 == (uint *)0x0) || (param_3 = param_3 + 1, param_3 == (uint *)0x0)) {
    uVar5 = 0;
  }
  else {
    uVar5 = *param_3;
  }
  param_4[2] = uVar5 ^ uVar7 + iVar1;
  if ((param_3 == (uint *)0x0) || (param_3 + 1 == (uint *)0x0)) {
    param_4[3] = uVar8;
    return;
  }
  param_4[3] = param_3[1] ^ uVar8;
  return;
}



/* 00e6fc30 FUN_00e6fc30 */
/* Callers:
 * Callees:
 */

void __thiscall FUN_00e6fc30(int param_1,int *param_2,uint *param_3,uint *param_4)

{
  byte bVar1;
  int *piVar2;
  uint uVar3;
  uint uVar4;
  uint uVar5;
  int iVar6;
  uint uVar7;
  uint uVar8;
  uint uVar9;
  uint uVar10;
  
  uVar4 = param_2[1];
  uVar5 = param_2[3];
  uVar7 = *param_2 - *(int *)(*(int *)(param_1 + 0x18) + -8 + *(int *)(param_1 + 0x14) * 4);
  piVar2 = param_2 + 2;
  iVar6 = *(int *)(param_1 + 0x18) + -8 + *(int *)(param_1 + 0x14) * 4;
  param_2 = *(int **)(param_1 + 0xc);
  uVar9 = *piVar2 - *(int *)(iVar6 + 4);
  if (param_2 != (int *)0x0) {
    uVar10 = uVar4;
    uVar8 = uVar5;
    do {
      uVar5 = uVar9;
      uVar4 = uVar7;
      uVar8 = uVar8 - *(int *)(iVar6 + -8);
      uVar10 = uVar10 - *(int *)(iVar6 + -4);
      uVar9 = (uVar4 * 2 + 1) * uVar4;
      uVar7 = (uVar5 * 2 + 1) * uVar5;
      iVar6 = iVar6 + -8;
      uVar3 = uVar7 * 0x20 | uVar7 >> 0x1b;
      uVar7 = uVar9 * 0x20 | uVar9 >> 0x1b;
      bVar1 = (byte)uVar7 & 0x1f;
      uVar9 = (uVar10 >> bVar1 | uVar10 << 0x20 - bVar1) ^ uVar3;
      bVar1 = (byte)uVar3 & 0x1f;
      uVar7 = (uVar8 >> bVar1 | uVar8 << 0x20 - bVar1) ^ uVar7;
      param_2 = (int *)((int)param_2 + -1);
      uVar10 = uVar4;
      uVar8 = uVar5;
    } while (param_2 != (int *)0x0);
  }
  uVar5 = uVar5 - (*(int **)(param_1 + 0x18))[1];
  iVar6 = **(int **)(param_1 + 0x18);
  if (param_3 == (uint *)0x0) {
    uVar10 = 0;
  }
  else {
    uVar10 = *param_3;
  }
  *param_4 = uVar10 ^ uVar7;
  if ((param_3 == (uint *)0x0) || (param_3 = param_3 + 1, param_3 == (uint *)0x0)) {
    uVar7 = 0;
  }
  else {
    uVar7 = *param_3;
  }
  param_4[1] = uVar7 ^ uVar4 - iVar6;
  if ((param_3 == (uint *)0x0) || (param_3 = param_3 + 1, param_3 == (uint *)0x0)) {
    uVar4 = 0;
  }
  else {
    uVar4 = *param_3;
  }
  param_4[2] = uVar4 ^ uVar9;
  if ((param_3 == (uint *)0x0) || (param_3 + 1 == (uint *)0x0)) {
    param_4[3] = uVar5;
    return;
  }
  param_4[3] = param_3[1] ^ uVar5;
  return;
}



/* 00dd23d0 FUN_00dd23d0 */
/* Callers:
 * Callees:
 */

undefined4 FUN_00dd23d0(void)

{
  return 4;
}


