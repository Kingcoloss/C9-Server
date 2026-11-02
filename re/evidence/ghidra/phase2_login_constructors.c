/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00a64300 FUN_00a64300 */
/* Callers:
 * 00a63117 00a630f0 FUN_00a630f0
 * Callees:
 * 00a64580 FUN_00a64580
 */

undefined4 __fastcall FUN_00a64300(undefined4 param_1)

{
  FUN_00a64580();
  return param_1;
}



/* 00a64320 FUN_00a64320 */
/* Callers:
 * 00a63187 00a63160 FUN_00a63160
 * Callees:
 * 00a64430 FUN_00a64430
 */

undefined4 __fastcall FUN_00a64320(undefined4 param_1)

{
  FUN_00a64430();
  return param_1;
}



/* 00a64340 FUN_00a64340 */
/* Callers:
 * 00a631d7 00a631b0 FUN_00a631b0
 * Callees:
 * 00a64480 FUN_00a64480
 */

undefined4 __fastcall FUN_00a64340(undefined4 param_1)

{
  FUN_00a64480();
  return param_1;
}



/* 00a64360 FUN_00a64360 */
/* Callers:
 * 00a63787 00a63760 FUN_00a63760
 * Callees:
 * 00a64530 FUN_00a64530
 */

undefined4 __fastcall FUN_00a64360(undefined4 param_1)

{
  FUN_00a64530();
  return param_1;
}



/* 00a64380 FUN_00a64380 */
/* Callers:
 * 00a642ea 00a642e0 FUN_00a642e0
 * Callees:
 * 00dd5d9c memset
 */

undefined2 * __fastcall FUN_00a64380(undefined2 *param_1)

{
  memset(param_1,0,0x3e);
  *param_1 = 0x3e;
  *(undefined1 *)(param_1 + 1) = 1;
  *(undefined2 *)((int)param_1 + 3) = 0x1771;
  *(undefined4 *)((int)param_1 + 5) = 0;
  *(undefined4 *)((int)param_1 + 9) = 0;
  return param_1;
}



/* 00a643d0 FUN_00a643d0 */
/* Callers:
 * 00a642aa 00a642a0 FUN_00a642a0
 * Callees:
 * 00dd5d9c memset
 */

undefined2 * __fastcall FUN_00a643d0(undefined2 *param_1)

{
  memset(param_1,0,0x87f);
  *param_1 = 0x87f;
  *(undefined1 *)(param_1 + 1) = 1;
  *(undefined2 *)((int)param_1 + 3) = 0x2ee1;
  *(undefined4 *)((int)param_1 + 5) = 0;
  *(undefined4 *)((int)param_1 + 9) = 0;
  return param_1;
}



/* 00a64430 FUN_00a64430 */
/* Callers:
 * 00a6432a 00a64320 FUN_00a64320
 * Callees:
 */

undefined4 * __fastcall FUN_00a64430(undefined4 *param_1)

{
  *param_1 = 0;
  param_1[1] = 0;
  param_1[2] = 0;
  *(undefined1 *)(param_1 + 3) = 0;
  *(undefined2 *)param_1 = 0xd;
  *(undefined1 *)((int)param_1 + 2) = 1;
  *(undefined2 *)((int)param_1 + 3) = 0x2ee5;
  *(undefined4 *)((int)param_1 + 5) = 0;
  *(undefined4 *)((int)param_1 + 9) = 0;
  return param_1;
}



/* 00a64480 FUN_00a64480 */
/* Callers:
 * 00a6434a 00a64340 FUN_00a64340
 * Callees:
 */

undefined4 * __fastcall FUN_00a64480(undefined4 *param_1)

{
  *param_1 = 0;
  param_1[1] = 0;
  param_1[2] = 0;
  param_1[3] = 0;
  param_1[4] = 0;
  param_1[5] = 0;
  param_1[6] = 0;
  param_1[7] = 0;
  *(undefined1 *)(param_1 + 8) = 0;
  *(undefined2 *)param_1 = 0x21;
  *(undefined1 *)((int)param_1 + 2) = 1;
  *(undefined2 *)((int)param_1 + 3) = 0x2eec;
  *(undefined4 *)((int)param_1 + 5) = 0;
  *(undefined4 *)((int)param_1 + 9) = 0;
  return param_1;
}



/* 00a644e0 FUN_00a644e0 */
/* Callers:
 * 00a642ca 00a642c0 FUN_00a642c0
 * Callees:
 * 00dd5d9c memset
 */

undefined2 * __fastcall FUN_00a644e0(undefined2 *param_1)

{
  memset(param_1,0,0x29);
  *param_1 = 0x29;
  *(undefined1 *)(param_1 + 1) = 1;
  *(undefined2 *)((int)param_1 + 3) = 0x2ee8;
  *(undefined4 *)((int)param_1 + 5) = 0;
  *(undefined4 *)((int)param_1 + 9) = 0;
  return param_1;
}



/* 00a64530 FUN_00a64530 */
/* Callers:
 * 00a6436a 00a64360 FUN_00a64360
 * Callees:
 */

undefined4 * __fastcall FUN_00a64530(undefined4 *param_1)

{
  *param_1 = 0;
  param_1[1] = 0;
  param_1[2] = 0;
  *(undefined1 *)(param_1 + 3) = 0;
  *(undefined2 *)param_1 = 0xd;
  *(undefined1 *)((int)param_1 + 2) = 1;
  *(undefined2 *)((int)param_1 + 3) = 0xec4;
  *(undefined4 *)((int)param_1 + 5) = 0;
  *(undefined4 *)((int)param_1 + 9) = 0;
  return param_1;
}



/* 00a64580 FUN_00a64580 */
/* Callers:
 * 00a6430a 00a64300 FUN_00a64300
 * Callees:
 */

undefined4 * __fastcall FUN_00a64580(undefined4 *param_1)

{
  *param_1 = 0;
  param_1[1] = 0;
  param_1[2] = 0;
  param_1[3] = 0;
  param_1[4] = 0;
  param_1[5] = 0;
  param_1[6] = 0;
  *(undefined1 *)(param_1 + 7) = 0;
  *(undefined2 *)param_1 = 0x1d;
  *(undefined1 *)((int)param_1 + 2) = 1;
  *(undefined2 *)((int)param_1 + 3) = 0x2455;
  *(undefined4 *)((int)param_1 + 5) = 0;
  *(undefined4 *)((int)param_1 + 9) = 0;
  return param_1;
}



/* 00a645e0 FUN_00a645e0 */
/* Callers:
 * 00ebcb01 00ebcafb Unwind@00ebcafb
 * 00ebccd1 00ebcccb Unwind@00ebcccb
 * 00a641a1 00a64140 FUN_00a64140
 * Callees:
 * 00a648f0 FUN_00a648f0
 * 0048d890 FUN_0048d890
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00a645e0(undefined4 param_1)

{
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  puStack_c = &DAT_00edf4a8;
  pvStack_10 = ExceptionList;
  ExceptionList = &pvStack_10;
  uStack_8 = 0;
  FUN_00a648f0(_DAT_01302704 ^ (int)register0x00000010 - 4U,param_1);
  uStack_8 = 0xffffffff;
  FUN_0048d890();
  ExceptionList = pvStack_10;
  return;
}


