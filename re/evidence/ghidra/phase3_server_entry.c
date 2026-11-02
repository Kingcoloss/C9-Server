/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00614cd0 FUN_00614cd0 */
/* Callers:
 * 00614b0c 00614ab0 FUN_00614ab0
 * 00614e3e 00614de0 FUN_00614de0
 * 00a6563c 00a655e0 FUN_00a655e0
 * 00a658ae 00a65850 FUN_00a65850
 * Callees:
 * 00614d40 FUN_00614d40
 */

void __thiscall FUN_00614cd0(undefined4 param_1,undefined4 param_2,undefined4 param_3)

{
  FUN_00614d40(param_2,param_3,param_1);
  return;
}



/* 00a631b0 FUN_00a631b0 */
/* Callers:
 * 00b1d850 00b1d770 FUN_00b1d770
 * Callees:
 * 00a68dd0 FUN_00a68dd0
 * 00a64340 FUN_00a64340
 * 00a68fe0 FUN_00a68fe0
 * 009b6d90 FUN_009b6d90
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



/* 00563820 FUN_00563820 */
/* Callers:
 * 0056380a 005637f0 FUN_005637f0
 * 00a68d56 00a68d40 FUN_00a68d40
 * Callees:
 * 00562f40 FUN_00562f40
 * 009af830 FUN_009af830
 * 00563770 FUN_00563770
 * 00dcecb0 FUN_00dcecb0
 * 00563120 FUN_00563120
 */

undefined4 __thiscall FUN_00563820(int param_1,undefined4 param_2,undefined2 param_3)

{
  undefined2 uVar1;
  undefined4 uVar2;
  int iVar3;
  
  FUN_00dcecb0();
  FUN_00563770();
  uVar2 = Ordinal_23(2,1,6);
  *(undefined4 *)(param_1 + 4) = uVar2;
  if (*(int *)(param_1 + 4) == -1) {
    uVar2 = 0;
  }
  else {
    *(undefined2 *)(param_1 + 8) = 2;
    uVar1 = Ordinal_9(param_3);
    *(undefined2 *)(param_1 + 10) = uVar1;
    *(undefined4 *)(param_1 + 0xc) = param_2;
    FUN_009af830(param_1);
    FUN_00562f40();
    iVar3 = Ordinal_4(*(undefined4 *)(param_1 + 4),param_1 + 8,0x10);
    if (((iVar3 == -1) && (iVar3 = Ordinal_111(), iVar3 != 0x3e5)) && (iVar3 != 0x2733)) {
      Ordinal_3(*(undefined4 *)(param_1 + 4));
      *(undefined4 *)(param_1 + 4) = 0xffffffff;
      FUN_009af830(param_1);
      FUN_00563120();
      return 0;
    }
    *(undefined4 *)(param_1 + 0x20) = 0;
    *(undefined4 *)(param_1 + 0x24) = 0;
    *(undefined2 *)(param_1 + 0x28) = 0;
    uVar2 = 1;
  }
  return uVar2;
}



/* 00a68d40 FUN_00a68d40 */
/* Callers:
 * 00a62e58 00a621f0 FUN_00a621f0
 * 00a68d2a 00a68d10 FUN_00a68d10
 * 00a6e0ee 00a6d940 FUN_00a6d940
 * Callees:
 * 00563820 FUN_00563820
 * 005f4ba0 FUN_005f4ba0
 * 0046a0d0 FUN_0046a0d0
 */

bool __thiscall FUN_00a68d40(int param_1,undefined4 param_2,undefined2 param_3)

{
  char cVar1;
  
  cVar1 = FUN_00563820(param_2,param_3);
  if (cVar1 != '\0') {
    FUN_005f4ba0();
    *(undefined1 *)(param_1 + 0x1f1) = 0;
  }
  else {
    FUN_0046a0d0(0,&UNK_0112dd0c);
  }
  return cVar1 != '\0';
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



/* 00563600 FUN_00563600 */
/* Callers:
 * 00563dae 00563d00 FUN_00563d00
 * Callees:
 * 00412130 FUN_00412130
 */

void FUN_00563600(undefined4 param_1,undefined4 param_2,undefined4 param_3)

{
  int *piVar1;
  undefined4 extraout_var;
  
  piVar1 = (int *)FUN_00412130(param_1);
  if (piVar1 != (int *)0x0) {
    (**(code **)(*piVar1 + 8))(param_2,param_3,extraout_var);
  }
  return;
}


