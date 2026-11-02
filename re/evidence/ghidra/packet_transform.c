/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00564140 FUN_00564140 */
/* Callers:
 * 00563c70 00563c10 FUN_00563c10
 * 00563dd7 00563d00 FUN_00563d00
 * Callees:
 */

uint FUN_00564140(char *param_1,char *param_2)

{
  uint uStack_8;
  
  uStack_8 = 0xbd136d48;
  for (; param_1 != param_2; param_1 = param_1 + 1) {
    uStack_8 = uStack_8 * 0x3faa1ff ^ (int)*param_1;
  }
  return uStack_8;
}



/* 006669d0 FUN_006669d0 */
/* Callers:
 * 00666810 006667b0 FUN_006667b0
 * 00666967 00666890 FUN_00666890
 * Callees:
 */

uint FUN_006669d0(char *param_1,char *param_2)

{
  uint uStack_8;
  
  uStack_8 = 0xbd136d48;
  for (; param_1 != param_2; param_1 = param_1 + 1) {
    uStack_8 = uStack_8 * 0x3faa1ff ^ (int)*param_1;
  }
  return uStack_8;
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



/* 00412230 FUN_00412230 */
/* Callers:
 * 00667daa 00667c90 FUN_00667c90
 * 00415303 00414f70 FUN_00414f70
 * 0066693b 00666890 FUN_00666890
 * Callees:
 * 00412360 FUN_00412360
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_00412230(uint param_1,undefined4 param_2,undefined4 param_3)

{
  int iStack_4;
  
  param_1 = param_1 % 0x50;
  FUN_00412360(&iStack_4,&param_1);
  if ((iStack_4 != _DAT_01925898) && (*(int **)(iStack_4 + 0x10) != (int *)0x0)) {
    (**(code **)(**(int **)(iStack_4 + 0x10) + 8))(param_2,param_3);
  }
  return;
}



/* 00dcef50 FUN_00dcef50 */
/* Callers:
 * 00563d59 00563d00 FUN_00563d00
 * 006668e9 00666890 FUN_00666890
 * Callees:
 * 00dcedf0 FUN_00dcedf0
 * 00dcf140 FUN_00dcf140
 * 00dcf110 FUN_00dcf110
 * 00dced50 FUN_00dced50
 */

int __thiscall
FUN_00dcef50(int param_1,int param_2,int param_3,undefined4 param_4,undefined4 param_5)

{
  int iVar1;
  int iVar2;
  
  if (param_3 == 0) {
    return -1;
  }
  if (param_2 == 1) {
    param_1 = param_1 + 0x98;
  }
  iVar1 = FUN_00dcf110(param_3,param_4,param_5);
  if (0 < iVar1) {
    iVar2 = FUN_00dcf140();
    if (*(int *)(param_1 + 8) == iVar2) {
      FUN_00dced50(param_1,param_3,iVar1);
      FUN_00dcedf0(param_1);
      *(undefined4 *)(param_1 + 8) = 1;
      return iVar1;
    }
    FUN_00dced50(param_1,param_3,iVar1);
    *(int *)(param_1 + 8) = *(int *)(param_1 + 8) + 1;
  }
  return iVar1;
}


