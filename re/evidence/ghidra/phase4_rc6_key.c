/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00e2a3f0 FUN_00e2a3f0 */
/* Callers:
 * Callees:
 */

undefined4 FUN_00e2a3f0(void)

{
  return 1;
}



/* 00e62400 FUN_00e62400 */
/* Callers:
 * 00e625e9 00e625e0 FUN_00e625e0
 * Callees:
 * 00dd5ecc _CxxThrowException
 * 00e621c0 FUN_00e621c0
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00e62400(int *param_1,undefined4 param_2)

{
  char cVar1;
  int *piVar2;
  undefined4 uVar3;
  undefined1 auStack_52 [10];
  undefined1 auStack_48 [56];
  void *pvStack_10;
  void *pvStack_c;
  undefined *puStack_8;
  undefined4 uStack_4;
  
  uStack_4 = 0xffffffff;
  puStack_8 = &DAT_00ee8929;
  pvStack_c = ExceptionList;
  ExceptionList = &pvStack_c;
  cVar1 = (**(code **)(*param_1 + 0x14))(param_2,_DAT_01302704 ^ (uint)(auStack_52 + -10));
  if (cVar1 == '\0') {
    piVar2 = (int *)(**(code **)(*param_1 + 0x34))();
    uVar3 = (**(code **)(*piVar2 + 8))(auStack_52 + -6);
    pvStack_c = (void *)0x0;
    FUN_00e621c0(uVar3,param_2);
                    /* WARNING: Subroutine does not return */
    _CxxThrowException(auStack_48,&UNK_012d50d8);
  }
  ExceptionList = pvStack_10;
  return;
}


