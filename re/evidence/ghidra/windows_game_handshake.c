/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00a6b3f0 FUN_00a6b3f0 */
/* Callers:
 * 00a6b21a data reference
 * Callees:
 */

undefined4 * __fastcall FUN_00a6b3f0(undefined4 *param_1)

{
  *param_1 = 0;
  param_1[1] = 0;
  param_1[2] = 0;
  param_1[3] = 0;
  param_1[4] = 0;
  *(undefined1 *)(param_1 + 5) = 0;
  *(undefined2 *)param_1 = 0x15;
  *(undefined1 *)((int)param_1 + 2) = 1;
  *(undefined2 *)((int)param_1 + 3) = 9999;
  *(undefined4 *)((int)param_1 + 5) = 0;
  *(undefined4 *)((int)param_1 + 9) = 0;
  return param_1;
}



/* 00a68740 FUN_00a68740 */
/* Callers:
 * Callees:
 */

void __thiscall FUN_00a68740(int param_1,int param_2)

{
  char cVar1;
  uint uVar2;
  undefined1 auStack_1c [13];
  undefined4 uStack_f;
  int iStack_b;

  func_0x005f7760();
  cVar1 = func_0x00a68dd0();
  if ((cVar1 != '\0') && (*(char *)(param_1 + 8) != '\0')) {
    uVar2 = func_0x009a7360();
    if ((3000 < uVar2) || (param_2 != 0)) {
      func_0x005f4ba0();
      func_0x00a6b210();
      uStack_f = GetTickCount();
      iStack_b = param_2;
      func_0x00a38850(auStack_1c,0x15);
    }
  }
  return;
}



/* 00a694d0 FUN_00a694d0 */
/* Callers:
 * Callees:
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00a694d0(int param_1)

{
  char cVar1;
  undefined4 uVar2;
  int iVar3;
  undefined1 auStack_44 [4];
  undefined1 auStack_40 [4];
  undefined1 auStack_3c [13];
  undefined4 uStack_2f;
  undefined4 uStack_2b;
  undefined4 uStack_27;
  int iStack_23;
  int iStack_1f;
  uint uStack_1b;
  undefined4 uStack_17;
  undefined4 uStack_13;
  undefined4 uStack_f;
  undefined4 uStack_b;

  cVar1 = func_0x00a68dd0();
  if (cVar1 != '\0') {
    func_0x00a6b290();
    uStack_2f = *(undefined4 *)(param_1 + 0x78);
    func_0x005c7800();
    func_0x0052f580();
    uStack_2b = func_0x00dd5cf0();
    func_0x009378b0();
    uStack_27 = func_0x00a6d560();
    iStack_23 = 0;
    iStack_1f = 0;
    func_0x009378b0(auStack_40);
    func_0x00b7aea0();
    func_0x009a2180();
    while( true ) {
      func_0x009378b0(auStack_44);
      func_0x00b7aea0();
      uVar2 = func_0x00a88050();
      cVar1 = func_0x00882b10(uVar2);
      if (cVar1 == '\0') break;
      func_0x00475510();
      iVar3 = func_0x008b6460();
      if (iVar3 != 0) {
        func_0x00475510();
        iVar3 = func_0x008b6460();
        if (*(char *)(iVar3 + 0x14) != '\0') {
          iVar3 = func_0x00475510();
          iVar3 = (**(code **)(**(int **)(iVar3 + 8) + 0x1c))();
          if (iVar3 == 2) {
            iStack_1f = iStack_1f + 1;
          }
          else {
            iStack_23 = iStack_23 + 1;
          }
        }
      }
      func_0x006bfc70();
    }
    func_0x005c7800();
    uStack_13 = func_0x0052f2b0();
    uStack_17 = func_0x00479f20();
    uStack_1b = (uint)(_DAT_0130f8ac == 0);
    if (uStack_1b == 0) {
      uStack_f = _DAT_012e9300;
      uStack_b = _DAT_012e9304;
    }
    else {
      uStack_f = _DAT_012e9308;
      uStack_b = _DAT_012e930c;
    }
    func_0x00a38850(auStack_3c,0x35);
  }
  return;
}



/* 00a69420 FUN_00a69420 */
/* Callers:
 * Callees:
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00a69420(int param_1)

{
  uint uVar1;
  char cVar2;
  float10 fVar3;
  undefined1 auStack_2c [13];
  undefined4 uStack_1f;
  undefined4 uStack_1b;
  undefined4 uStack_17;
  undefined4 uStack_13;

  func_0x006033d0();
  cVar2 = func_0x00a68dd0();
  if ((cVar2 != '\0') && (*(char *)(param_1 + 0x30) != '\0')) {
    fVar3 = (float10)func_0x0093eb90();
    if ((float10)_DAT_0122ceb8 < fVar3) {
      func_0x00a6b270();
      uVar1 = *(uint *)(param_1 + 0x70);
      *(uint *)(param_1 + 0x70) = uVar1 + 1;
      *(uint *)(param_1 + 0x74) = *(int *)(param_1 + 0x74) + (uint)(0xfffffffe < uVar1);
      uStack_1f = *(undefined4 *)(param_1 + 0x70);
      uStack_1b = *(undefined4 *)(param_1 + 0x74);
      uStack_17 = timeGetTime();
      uStack_13 = 0;
      func_0x00a38850(auStack_2c,0x25);
      func_0x00603560();
    }
  }
  return;
}
