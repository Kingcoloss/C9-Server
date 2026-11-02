/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00561950 FUN_00561950 */
/* Callers:
 * 00562460 005623d0 FUN_005623d0
 * 0056249f 005623d0 FUN_005623d0
 * 006663b1 00666260 FUN_00666260
 * Callees:
 */

undefined4 * __fastcall FUN_00561950(undefined4 *param_1)

{
  *param_1 = 0xd;
  param_1[1] = 0xd;
  *(undefined2 *)(param_1 + 2) = 0;
  param_1[0x3f06] = 0;
  return param_1;
}



/* 00561ac0 FUN_00561ac0 */
/* Callers:
 * 00667dd9 00667c90 FUN_00667c90
 * 00563efe 00563e40 FUN_00563e40
 * 005640ec 00563e40 FUN_00563e40
 * 006663f5 00666260 FUN_00666260
 * Callees:
 */

uint __thiscall FUN_00561ac0(uint param_1,undefined4 param_2,int param_3)

{
  int iVar1;
  
  if ((uint)*(ushort *)(param_1 + 8) + param_3 < 0xfc0d) {
    iVar1 = memcpy_s(param_1 + 10 + (uint)*(ushort *)(param_1 + 8),
                     0xfc0d - (uint)*(ushort *)(param_1 + 8),param_2,param_3);
    if (iVar1 == 0) {
      *(short *)(param_1 + 8) = *(short *)(param_1 + 8) + (short)param_3;
    }
    param_1 = (uint)(iVar1 == 0);
  }
  else {
    param_1 = param_1 & 0xffffff00;
  }
  return param_1;
}



/* 00561a20 FUN_00561a20 */
/* Callers:
 * 00561a8a 00561a80 FUN_00561a80
 * 00563f41 00563e40 FUN_00563e40
 * 00666400 00666260 FUN_00666260
 * Callees:
 */

uint __fastcall FUN_00561a20(uint param_1)

{
  if (*(ushort *)(param_1 + 8) < 0xd) {
    param_1 = param_1 & 0xffffff00;
  }
  else if (*(short *)(param_1 + 10) == 0) {
    param_1 = 0;
  }
  else {
    param_1 = CONCAT31((uint3)(byte)((ushort)*(short *)(param_1 + 10) >> 8),1);
  }
  return param_1;
}



/* 00561a50 FUN_00561a50 */
/* Callers:
 * 00563f55 00563e40 FUN_00563e40
 * 00666416 00666260 FUN_00666260
 * Callees:
 */

undefined2 __fastcall FUN_00561a50(int param_1)

{
  return CONCAT11((char)((ushort)*(short *)(param_1 + 8) >> 8),
                  *(short *)(param_1 + 10) == *(short *)(param_1 + 8));
}



/* 00561a80 FUN_00561a80 */
/* Callers:
 * 00563edc 00563e40 FUN_00563e40
 * Callees:
 * 00561a20 FUN_00561a20
 */

int __fastcall FUN_00561a80(int param_1)

{
  char cVar1;
  int iVar2;
  
  cVar1 = FUN_00561a20();
  if (cVar1 == '\0') {
    iVar2 = 0xd - (uint)*(ushort *)(param_1 + 8);
  }
  else {
    iVar2 = (uint)*(ushort *)(param_1 + 10) - (uint)*(ushort *)(param_1 + 8);
  }
  return iVar2;
}



/* 00561630 FUN_00561630 */
/* Callers:
 * 005640c9 00563e40 FUN_00563e40
 * 00666506 00666260 FUN_00666260
 * Callees:
 * 00561860 FUN_00561860
 * 00561890 FUN_00561890
 * 00561820 FUN_00561820
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00561630(undefined4 param_1,undefined1 param_2)

{
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00e8429e;
  pvStack_10 = ExceptionList;
  ExceptionList = &pvStack_10;
  FUN_00561860(param_1,_DAT_01302704 ^ (int)register0x00000010 - 4U);
  uStack_8 = 0;
  FUN_00561820(&param_2);
  uStack_8 = 0xffffffff;
  FUN_00561890();
  ExceptionList = pvStack_10;
  return;
}



/* 00563d00 FUN_00563d00 */
/* Callers:
 * 00564004 00563e40 FUN_00563e40
 * Callees:
 * 00564140 FUN_00564140
 * 00dcef50 FUN_00dcef50
 * 00dd5f60 memcpy
 * 00563600 FUN_00563600
 */

int __thiscall
FUN_00563d00(undefined4 param_1,ushort *param_2,ushort *param_3,int param_4,undefined4 *param_5)

{
  int iVar1;
  int iVar2;
  int iVar3;
  
  memcpy(param_3,param_2,3,param_1);
  iVar2 = FUN_00dcef50(1,(int)param_3 + 3,(int)param_2 + 3,*param_2 - 3);
  if (iVar2 < 1) {
    *param_5 = 0xfffffffc;
    iVar2 = -1;
  }
  else {
    *param_3 = (short)iVar2 + 3;
    FUN_00563600(*(undefined4 *)((int)param_3 + 5),(int)param_3 + 0xd,*param_3 - 0xd);
    iVar1 = *(int *)((int)param_3 + 5);
    *(undefined4 *)((int)param_3 + 5) = 100;
    iVar3 = FUN_00564140((int)param_3 + 3,(*param_3 - 3) + (int)param_3 + 3);
    if (iVar3 == iVar1) {
      *(int *)((int)param_3 + 5) = iVar1;
      if ((param_4 == 0) || (param_4 == *(int *)((int)param_3 + 9))) {
        *param_5 = 0;
        iVar2 = iVar2 + 3;
      }
      else {
        *param_5 = 0xfffffffd;
        iVar2 = -1;
      }
    }
    else {
      *param_5 = 0xfffffffe;
      iVar2 = -1;
    }
  }
  return iVar2;
}



/* 00666890 FUN_00666890 */
/* Callers:
 * 0066649c 00666260 FUN_00666260
 * Callees:
 * 006669d0 FUN_006669d0
 * 00dcef50 FUN_00dcef50
 * 00412230 FUN_00412230
 * 00dd5f60 memcpy
 */

int __thiscall
FUN_00666890(undefined4 param_1,ushort *param_2,ushort *param_3,int param_4,undefined4 *param_5)

{
  int iVar1;
  int iVar2;
  int iVar3;
  
  memcpy(param_3,param_2,3,param_1);
  iVar2 = FUN_00dcef50(1,(int)param_3 + 3,(int)param_2 + 3,*param_2 - 3);
  if (iVar2 < 1) {
    *param_5 = 0xfffffffc;
    iVar2 = -1;
  }
  else {
    *param_3 = (short)iVar2 + 3;
    FUN_00412230(*(undefined4 *)((int)param_3 + 5),(int)param_3 + 0xd,*param_3 - 0xd);
    iVar1 = *(int *)((int)param_3 + 5);
    *(undefined4 *)((int)param_3 + 5) = 100;
    iVar3 = FUN_006669d0((int)param_3 + 3,(*param_3 - 3) + (int)param_3 + 3);
    if (iVar3 == iVar1) {
      *(int *)((int)param_3 + 5) = iVar1;
      if ((param_4 == 0) || (param_4 == *(int *)((int)param_3 + 9))) {
        *param_5 = 0;
        iVar2 = iVar2 + 3;
      }
      else {
        *param_5 = 0xfffffffd;
        iVar2 = -1;
      }
    }
    else {
      *param_5 = 0xfffffffe;
      iVar2 = -1;
    }
  }
  return iVar2;
}



/* 00666260 FUN_00666260 */
/* Callers:
 * Callees:
 * 00dd5fb0 __alloca_probe
 * 00561950 FUN_00561950
 * 00666890 FUN_00666890
 * 00dd5b90 FUN_00dd5b90
 * 00564200 FUN_00564200
 * 00561a50 FUN_00561a50
 * 00666570 FUN_00666570
 * 00408750 FUN_00408750
 * 00561ac0 FUN_00561ac0
 * 006665e0 FUN_006665e0
 * 00486400 FUN_00486400
 * 00dd5f60 memcpy
 * 00564220 FUN_00564220
 * 00561a20 FUN_00561a20
 * 00666590 FUN_00666590
 * 00561630 FUN_00561630
 * 00564240 FUN_00564240
 * 006661a0 FUN_006661a0
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Removing unreachable block (ram,0x0066653a) */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_00666260(undefined4 param_1,undefined4 param_2,int param_3)

{
  char cVar1;
  uint uVar2;
  int iVar3;
  undefined4 uVar4;
  undefined4 uStack_2068;
  int iStack_205c;
  int iStack_2058;
  undefined1 *puStack_2054;
  undefined1 auStack_2050 [2];
  char cStack_204e;
  ushort *puStack_1040;
  uint uStack_103c;
  undefined4 uStack_1038;
  undefined4 uStack_1034;
  undefined1 auStack_1030 [4100];
  uint uStack_2c;
  uint uStack_28;
  undefined4 uStack_24;
  int iStack_20;
  undefined4 uStack_1c;
  undefined4 uStack_14;
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00e8dc10;
  pvStack_10 = ExceptionList;
  uStack_14 = 0x66627b;
  uVar2 = _DAT_01302704 ^ (uint)(auStack_1030 + 0x102c);
  ExceptionList = &pvStack_10;
  iStack_20 = param_3;
  uStack_2c = uVar2;
  uStack_1c = _LockReadStream_Link_Network_BaseCat__YGPAU_Handle_Stream_System_3_PAU4123__Z(param_2)
  ;
  FUN_00666570(param_2,uVar2);
  uStack_8 = 0;
  do {
    uStack_103c = _GetLength_Stream_System_BaseCat__YGIPAU_Handle_123__Z(uStack_1c);
    if (uStack_103c < 0xd) {
      uStack_8 = 0xffffffff;
      FUN_00666590();
LAB_00666550:
      ExceptionList = pvStack_10;
      FUN_00dd5b90();
      return;
    }
    uStack_28 = 0;
    puStack_1040 = (ushort *)
                   _GetRawPtr_Stream_System_BaseCat__YGPAU_Block_SmartHeap_23_PAU_Handle_123__Z
                             (uStack_1c);
    uStack_28 = (uint)*puStack_1040;
    if ((uStack_28 == 0) || (0x1000 < uStack_28)) {
      FUN_006665e0();
      uStack_8 = 0xffffffff;
      FUN_00666590();
      goto LAB_00666550;
    }
    if (uStack_103c < uStack_28) {
      uStack_8 = 0xffffffff;
      FUN_00666590();
      goto LAB_00666550;
    }
    uStack_1034 = _Read_Stream_System_BaseCat__YGIPAU_Handle_123_PAXI_Z
                            (uStack_1c,auStack_1030,uStack_28);
    uStack_24 = FUN_00486400(0xfc2c);
    iVar3 = FUN_00408750(0xfc2c,uStack_24);
    uStack_8._0_1_ = 1;
    if (iVar3 == 0) {
      uStack_2068 = 0;
    }
    else {
      uStack_2068 = FUN_00561950();
    }
    uStack_8 = (uint)uStack_8._1_3_ << 8;
    uStack_1038 = uStack_2068;
    FUN_00561ac0(auStack_1030,uStack_28);
    cVar1 = FUN_00561a20();
    if ((cVar1 == '\0') || (cVar1 = FUN_00561a50(), cVar1 == '\0')) {
      FUN_006661a0(uStack_1038);
      FUN_006665e0();
      uStack_8 = 0xffffffff;
      FUN_00666590();
      goto LAB_00666550;
    }
    uVar4 = FUN_00564200();
    uVar4 = FUN_00564240(uVar4);
    memcpy(auStack_2050,uVar4);
    puStack_2054 = auStack_2050;
    if (cStack_204e != '\0') {
      iStack_205c = 0;
      uVar4 = FUN_00564240(*(undefined4 *)(iStack_20 + 0x188),&iStack_205c);
      iStack_2058 = FUN_00666890(auStack_2050,uVar4);
      if ((iStack_2058 < 1) || (iStack_205c != 0)) {
        FUN_006665e0();
        uStack_8 = 0xffffffff;
        FUN_00666590();
        goto LAB_00666550;
      }
      FUN_00564220(iStack_2058);
      *(int *)(iStack_20 + 0x188) = *(int *)(iStack_20 + 0x188) + 1;
    }
    FUN_00561630(uStack_1038);
  } while( true );
}



/* 00563e40 FUN_00563e40 */
/* Callers:
 * 00563bc8 00563ac0 FUN_00563ac0
 * Callees:
 * 00dd5fb0 __alloca_probe
 * 00dd5b90 FUN_00dd5b90
 * 00564200 FUN_00564200
 * 006634f0 FUN_006634f0
 * 00561a50 FUN_00561a50
 * 00563d00 FUN_00563d00
 * 009a7810 FUN_009a7810
 * 00561ac0 FUN_00561ac0
 * 00562280 FUN_00562280
 * 00562260 FUN_00562260
 * 00dd5f60 memcpy
 * 00561a80 FUN_00561a80
 * 00564220 FUN_00564220
 * 00561a20 FUN_00561a20
 * 00564260 FUN_00564260
 * 00561630 FUN_00561630
 * 00564240 FUN_00564240
 * 00415db0 FUN_00415db0
 * 00415dd0 FUN_00415dd0
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00563e40(int param_1,int param_2,uint param_3)

{
  char cVar1;
  uint uVar2;
  undefined4 uVar3;
  int iVar4;
  uint uVar5;
  int iStack_fc44;
  int iStack_fc40;
  undefined1 *puStack_fc34;
  undefined1 auStack_fc30 [2];
  char cStack_fc2e;
  uint uStack_20;
  uint uStack_1c;
  int iStack_18;
  int iStack_14;
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00e845f2;
  pvStack_10 = ExceptionList;
  uVar2 = _DAT_01302704 ^ (int)register0x00000010 - 4U;
  ExceptionList = &pvStack_10;
  iStack_18 = 0;
  iStack_14 = 0;
  uStack_20 = uVar2;
LAB_00563e83:
  do {
    if (param_3 == 0) {
LAB_00564119:
      ExceptionList = pvStack_10;
      FUN_00dd5b90();
      return;
    }
    if (*(int *)(param_1 + 0x2c) == 0) {
      FUN_009a7810(uVar2);
      iStack_18 = FUN_00562260();
      if (iStack_18 == 0) goto LAB_00564119;
    }
    else {
      iStack_18 = *(int *)(param_1 + 0x2c);
      *(undefined4 *)(param_1 + 0x2c) = 0;
    }
    uStack_1c = FUN_00561a80();
    if (uStack_1c <= param_3) {
      cVar1 = FUN_00561ac0(param_2 + iStack_14,uStack_1c);
      if (cVar1 == '\0') {
        FUN_009a7810(iStack_18);
        FUN_00562280();
        goto LAB_00564119;
      }
      iStack_14 = iStack_14 + uStack_1c;
      param_3 = param_3 - uStack_1c;
      cVar1 = FUN_00561a20();
      if ((cVar1 == '\x01') && (cVar1 = FUN_00561a50(), cVar1 == '\x01')) {
        FUN_00564260(param_1);
        uVar3 = FUN_00564200();
        uVar3 = FUN_00564240(uVar3);
        memcpy(auStack_fc30,uVar3);
        puStack_fc34 = auStack_fc30;
        if (cStack_fc2e != '\0') {
          FUN_00415db0(param_1 + 0x164);
          uStack_8 = 0;
          iStack_fc44 = 0;
          uVar3 = FUN_00564240(*(undefined4 *)(param_1 + 0x24),&iStack_fc44);
          iStack_fc40 = FUN_00563d00(auStack_fc30,uVar3);
          if ((iStack_fc40 < 1) || (iStack_fc44 != 0)) {
            uStack_8 = 0xffffffff;
            FUN_00415dd0(uVar2,param_1,0xfffffffc);
            goto LAB_00564119;
          }
          FUN_00564220(iStack_fc40);
          *(int *)(param_1 + 0x24) = *(int *)(param_1 + 0x24) + 1;
          uStack_8 = 0xffffffff;
          FUN_00415dd0();
        }
        iVar4 = FUN_00564240();
        if ((iVar4 != 0) && (uVar5 = FUN_00564200(), 0xc < uVar5)) {
          *(undefined2 *)(param_1 + 0x28) = *(undefined2 *)(iVar4 + 3);
        }
        FUN_006634f0(iStack_18);
        FUN_00561630();
      }
      else {
        *(int *)(param_1 + 0x2c) = iStack_18;
      }
      goto LAB_00563e83;
    }
    cVar1 = FUN_00561ac0(param_2 + iStack_14,param_3);
    if (cVar1 == '\0') goto LAB_00564119;
    param_3 = 0;
    *(int *)(param_1 + 0x2c) = iStack_18;
  } while( true );
}


