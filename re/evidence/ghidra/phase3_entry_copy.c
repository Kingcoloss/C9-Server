/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00614d40 FUN_00614d40 */
/* Callers:
 * 00614cdf 00614cd0 FUN_00614cd0
 * Callees:
 * 00408750 FUN_00408750
 */

void FUN_00614d40(undefined4 param_1,undefined4 *param_2)

{
  undefined4 *puVar1;
  int iVar2;
  
  puVar1 = (undefined4 *)FUN_00408750(0x34,param_1);
  if (puVar1 != (undefined4 *)0x0) {
    for (iVar2 = 0xd; iVar2 != 0; iVar2 = iVar2 + -1) {
      *puVar1 = *param_2;
      param_2 = param_2 + 1;
      puVar1 = puVar1 + 1;
    }
  }
  return;
}


