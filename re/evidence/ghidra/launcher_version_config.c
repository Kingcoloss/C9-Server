// String 0046e1a4: Failed to make download urls from version.ini
// Reference 0041b4e1 function FUN_0041a800
// Reference 0041b4eb function FUN_0041a800
// String 0046e1e0: Failed to obtain download urls from version.ini
// Reference 0041af50 function FUN_0041a800
// Reference 0041af5a function FUN_0041a800
// String 0046e210: server
// Reference 0041aebe function FUN_0041a800
// String 00477884: VERSION
// Reference 0040814f function FUN_00407ce0
// String 0047788c: DOWNLOAD
// Reference 00408165 function FUN_00407ce0
// String 00477920: .\localinfo.dat
// Reference 004088b6 function FUN_00408830
// Reference 004087bd function FUN_00408790

// Function 0041a800

void __thiscall
FUN_0041a800(LPCSTR param_1,undefined4 param_2,undefined4 param_3,undefined4 param_4)

{
  char cVar1;
  void *pvVar2;
  void *pvVar3;
  void *pvVar4;
  wchar_t wVar5;
  undefined1 *hFindFile;
  LPCSTR pCVar6;
  uint *puVar7;
  void *pvVar8;
  char *pcVar9;
  int iVar10;
  BOOL BVar11;
  int iVar12;
  wchar_t *pwVar13;
  uint uVar14;
  uint uVar15;
  int *piVar16;
  undefined4 *puVar17;
  rsize_t rVar18;
  CSimpleStringT<char,0> *pCVar19;
  long lVar20;
  void *extraout_ECX;
  void *pvVar21;
  wchar_t *pwVar22;
  int *piVar23;
  LPCSTR pCVar24;
  void *pvVar25;
  char cStack00000114;
  undefined1 uStack00000115;
  uint in_stack_00000420;
  int in_stack_00000428;
  uint *in_stack_0000042c;
  char acStackY_7ffffffe [2147482794];
  int iStackY_354;
  undefined1 uStackY_350;
  undefined4 uStackY_34c;
  LPCSTR pCStackY_348;
  undefined *puStackY_344;
  char *pcStackY_340;
  char *pcStackY_33c;
  LPCSTR pCStackY_338;
  undefined *puStackY_334;
  undefined1 *apuStack_304 [2];
  void *pvStack_2fc;
  void *pvStack_2f8;
  void *pvStack_2f4;
  int *piStack_2f0;
  void *pvStack_2e8;
  void *pvStack_2e4;
  void *pvStack_2e0;
  void *pvStack_2d8;
  void *pvStack_2d4;
  void *pvStack_2d0;
  wchar_t *pwStack_2cc;
  LPCSTR local_2c8;
  int iStack_2c4;
  uint *local_2c0;
  void *pvStack_2b8;
  void *pvStack_2a8;
  uint uStack_2a4;
  void *pvStack_29c;
  void *pvStack_28c;
  uint uStack_288;
  char *pcStack_284;
  void *pvStack_280;
  CHAR aCStack_27c [8];
  void *pvStack_274;
  _WIN32_FIND_DATAA _Stack_250;
  uint local_10;
  void *local_c;
  undefined1 *puStack_8;
  uint uStack_4;

  uStack_4._0_1_ = 0xff;
  uStack_4._1_3_ = 0xffffff;
  puStack_8 = &LAB_00467ffc;
  local_c = ExceptionList;
  local_10 = DAT_0048790c ^ (uint)apuStack_304;
  ExceptionList = &local_c;
  local_2c8 = param_1;
  local_2c0 = in_stack_0000042c;
  GetTickCount();
  FUN_0044a093();
  FUN_00420370();
  *(undefined4 *)(param_1 + 0x4c) = param_4;
  pcVar9 = &cStack00000114;
  *(undefined4 *)(param_1 + 0x48) = param_3;
  *(undefined4 *)(param_1 + 0x44) = param_2;
  do {
    cVar1 = *pcVar9;
    pcVar9 = pcVar9 + 1;
  } while (cVar1 != '\0');
  pCVar19 = (CSimpleStringT<char,0> *)(param_1 + 0x50);
  ATL::CSimpleStringT<char,0>::SetString(pCVar19,&cStack00000114,(int)pcVar9 - (int)&uStack00000115)
  ;
  *(uint *)(param_1 + 0x70) = in_stack_00000420;
  iVar10 = *(int *)(*(int *)pCVar19 + -0xc) + -1;
  if ((iVar10 < 0) || (iVar12 = *(int *)pCVar19, *(int *)(iVar12 + -0xc) < iVar10)) {
LAB_0041aa4e:
                    /* WARNING: Subroutine does not return */
    FUN_004038a0();
  }
  if (*(char *)(iVar12 + iVar10) != '/') {
    iVar10 = *(int *)(*(int *)pCVar19 + -0xc) + -1;
    if ((iVar10 < 0) || (*(int *)(iVar12 + -0xc) < iVar10)) goto LAB_0041aa4e;
    if (*(char *)(iVar12 + iVar10) != '\\') {
      FUN_004151c0();
    }
  }
  FUN_00415690();
  pCStackY_338 = param_1 + 0x58;
  puStackY_334 = &DAT_0046dc30;
  pcStackY_33c = (char *)0x41a933;
  FUN_00415690();
  pcStackY_340 = *(char **)pCVar19;
  pcStackY_33c = "backup\\output\\";
  pCStackY_348 = param_1 + 0x5c;
  puStackY_344 = &DAT_0046dc30;
  uStackY_34c = 0x41a949;
  FUN_00415690();
  FUN_004198b0();
  iVar10 = (**(code **)(DAT_0048e2e4 + 0xc))();
  pwStack_2cc = (wchar_t *)(iVar10 + 0x10);
  uStack_4 = 0;
  FUN_00415690();
  pwVar22 = pwStack_2cc;
  puStackY_334 = (undefined *)0x41a992;
  iVar10 = FID_conflict___access(pwStack_2cc,0);
  if ((in_stack_00000428 == 0) && (BVar11 = SetFileAttributesA((LPCSTR)pwVar22,0x80), BVar11 == 0))
  {
    GetLastError();
    FUN_004205f0();
    GetLastError();
    FUN_00420370();
  }
  iVar12 = FUN_00421720();
  if ((iVar12 == 0) || ((iVar10 != -1 && (in_stack_00000428 != 0)))) {
    FUN_00412910();
    piStack_2f0 = (int *)0x0;
    uStack_4._0_1_ = 1;
    if ((int)(*(uint *)(pwVar22 + -4) | 1U - *(int *)(pwVar22 + -2)) < 0) {
      ATL::CSimpleStringT<char,0>::PrepareWrite2((CSimpleStringT<char,0> *)&pwStack_2cc,0);
      pwVar22 = pwStack_2cc;
    }
    uStack_2a4 = 0xf;
    pvStack_2a8 = (void *)0x0;
    pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
    pwVar13 = pwVar22;
    do {
      wVar5 = *pwVar13;
      pwVar13 = (wchar_t *)((int)pwVar13 + 1);
    } while ((char)wVar5 != '\0');
    FUN_00401dc0();
    uStack_4._0_1_ = 2;
    FUN_00412bc0();
    uStack_4._0_1_ = 1;
    if (0xf < uStack_2a4) {
      _free(pvStack_2b8);
    }
    uStack_288 = 0xf;
    pvStack_28c = (void *)0x0;
    pvStack_29c = (void *)((uint)pvStack_29c & 0xffffff00);
    FUN_00401dc0();
    pcVar9 = (char *)((int)register0x00000010 + 0x218);
    uStack_4._0_1_ = 3;
    uStack_2a4 = 0xf;
    pvStack_2a8 = (void *)0x0;
    pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
    do {
      cVar1 = *pcVar9;
      pcVar9 = pcVar9 + 1;
    } while (cVar1 != '\0');
    FUN_00401dc0();
    uStack_4 = CONCAT31(uStack_4._1_3_,4);
    uVar14 = FUN_004128c0();
    *local_2c0 = uVar14;
    if (0xf < uStack_2a4) {
      _free(pvStack_2b8);
    }
    uStack_2a4 = 0xf;
    pvStack_2a8 = (void *)0x0;
    pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
    uStack_4._0_1_ = 1;
    if (0xf < uStack_288) {
      _free(pvStack_29c);
    }
    uStack_288 = 0xf;
    pvStack_28c = (void *)0x0;
    pvStack_29c = (void *)((uint)pvStack_29c & 0xffffff00);
    FUN_00401dc0();
    pcVar9 = (char *)((int)register0x00000010 + 0x218);
    uStack_4._0_1_ = 5;
    uStack_2a4 = 0xf;
    pvStack_2a8 = (void *)0x0;
    pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
    do {
      cVar1 = *pcVar9;
      pcVar9 = pcVar9 + 1;
    } while (cVar1 != '\0');
    FUN_00401dc0();
    uStack_4 = CONCAT31(uStack_4._1_3_,6);
    uVar14 = FUN_004128c0();
    local_2c0[1] = uVar14;
    if (0xf < uStack_2a4) {
      _free(pvStack_2b8);
    }
    uStack_2a4 = 0xf;
    pvStack_2a8 = (void *)0x0;
    pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
    uStack_4._0_1_ = 1;
    if (0xf < uStack_288) {
      _free(pvStack_29c);
    }
    uStack_288 = 0xf;
    pvStack_28c = (void *)0x0;
    pvStack_29c = (void *)((uint)pvStack_29c & 0xffffff00);
    FUN_00401dc0();
    pcVar9 = (char *)((int)register0x00000010 + 0x218);
    uStack_4._0_1_ = 7;
    uStack_2a4 = 0xf;
    pvStack_2a8 = (void *)0x0;
    pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
    do {
      cVar1 = *pcVar9;
      pcVar9 = pcVar9 + 1;
    } while (cVar1 != '\0');
    FUN_00401dc0();
    uStack_4 = CONCAT31(uStack_4._1_3_,8);
    uVar14 = FUN_004128c0();
    local_2c0[2] = uVar14;
    if (0xf < uStack_2a4) {
      _free(pvStack_2b8);
    }
    uStack_2a4 = 0xf;
    pvStack_2a8 = (void *)0x0;
    pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
    uStack_4._0_1_ = 1;
    if (0xf < uStack_288) {
      _free(pvStack_29c);
    }
    if (local_2c0[2] == 0) {
      uStack_288 = 0xf;
      pvStack_28c = (void *)0x0;
      pvStack_29c = (void *)((uint)pvStack_29c & 0xffffff00);
      FUN_00401dc0();
      pcVar9 = (char *)((int)register0x00000010 + 0x218);
      uStack_4._0_1_ = 9;
      uStack_2a4 = 0xf;
      pvStack_2a8 = (void *)0x0;
      pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
      do {
        cVar1 = *pcVar9;
        pcVar9 = pcVar9 + 1;
      } while (cVar1 != '\0');
      FUN_00401dc0();
      uStack_4 = CONCAT31(uStack_4._1_3_,10);
      uVar14 = FUN_004128c0();
      local_2c0[2] = uVar14;
      if (0xf < uStack_2a4) {
        _free(pvStack_2b8);
      }
      uStack_2a4 = 0xf;
      pvStack_2a8 = (void *)0x0;
      pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
      uStack_4._0_1_ = 1;
      if (0xf < uStack_288) {
        _free(pvStack_29c);
      }
    }
    if (((*local_2c0 != 0) || (local_2c0[1] != 0)) || (local_2c0[2] != 0)) {
      pvStack_2fc = (void *)0x0;
      pvStack_2f8 = (void *)0x0;
      pvStack_2f4 = (void *)0x0;
      pvStack_2d8 = (void *)0x0;
      pvStack_2d4 = (void *)0x0;
      pvStack_2d0 = (void *)0x0;
      pvStack_2e8 = (void *)0x0;
      pvStack_2e4 = (void *)0x0;
      pvStack_2e0 = (void *)0x0;
      apuStack_304[0] = (undefined1 *)&pCStackY_338;
      pcStackY_33c = (char *)0x6;
      pcStackY_340 = "server";
      uStack_4._0_1_ = 0xd;
      puStackY_334 = (undefined *)((uint)puStackY_334 & 0xffffff00);
      puStackY_344 = (undefined *)0x41aed4;
      FUN_00401dc0();
      pcVar9 = (char *)((int)register0x00000010 + 0x31c);
      pcStackY_33c = (char *)0xf;
      pcStackY_340 = (undefined *)0x0;
      uStack_4._0_1_ = 0xe;
      piStack_2f0 = &iStackY_354;
      uStackY_350 = 0;
      do {
        cVar1 = *pcVar9;
        pcVar9 = pcVar9 + 1;
      } while (cVar1 != '\0');
      FUN_00401dc0();
      uStack_4._0_1_ = 0xd;
      FUN_00413570();
      if ((pvStack_2fc == (void *)0x0) ||
         (iVar10 = (int)pvStack_2f8 - (int)pvStack_2fc >> 0x1f,
         ((int)pvStack_2f8 - (int)pvStack_2fc) / 0x1c + iVar10 == iVar10)) {
        FUN_004205f0();
        FUN_00420370();
        if (pvStack_2e8 != (void *)0x0) {
          _free(pvStack_2e8);
        }
        pvStack_2e8 = (void *)0x0;
        pvStack_2e4 = (void *)0x0;
        pvStack_2e0 = (void *)0x0;
        FUN_00417e80();
        FUN_00417e80();
        uStack_4 = (uint)uStack_4._1_3_ << 8;
        FUN_00412930();
        goto LAB_0041aa1d;
      }
      iStack_2c4 = (**(code **)(DAT_0048e2e4 + 0xc))();
      iStack_2c4 = iStack_2c4 + 0x10;
      uStack_4._0_1_ = 0xf;
      FUN_00415690();
      apuStack_304[0] = (undefined1 *)&pCStackY_338;
      pcStackY_33c = (char *)0x3;
      pcStackY_340 = "POD";
      puStackY_334 = (undefined *)((uint)puStackY_334 & 0xffffff00);
      puStackY_344 = (undefined *)0x41b00e;
      FUN_00401dc0();
      piStack_2f0 = &iStackY_354;
      uStack_4._0_1_ = 0x10;
      FUN_00407550();
      uStack_4._0_1_ = 0xf;
      FUN_00413570();
      if ((pvStack_2d8 == (void *)0x0) ||
         (iVar10 = (int)pvStack_2d4 - (int)pvStack_2d8 >> 0x1f,
         ((int)pvStack_2d4 - (int)pvStack_2d8) / 0x1c + iVar10 == iVar10)) {
        apuStack_304[0] = (undefined1 *)&pCStackY_338;
        pcStackY_33c = "POD";
        pcStackY_340 = (undefined *)0x41b07a;
        FUN_00407550();
        piStack_2f0 = &iStackY_354;
        uStack_4._0_1_ = 0x11;
        FUN_00407550();
        uStack_4._0_1_ = 0xf;
        FUN_00413570();
      }
      if ((pvStack_2d8 == (void *)0x0) ||
         (iVar10 = (int)pvStack_2d4 - (int)pvStack_2d8 >> 0x1f,
         ((int)pvStack_2d4 - (int)pvStack_2d8) / 0x1c + iVar10 == iVar10)) {
        uVar14 = 0;
        iVar10 = FUN_00412820();
        if (iVar10 != 0) {
          piStack_2f0 = (int *)0x1;
          do {
            FUN_00416ee0();
            uVar14 = uVar14 + 1;
            uVar15 = FUN_00412820();
          } while (uVar14 < uVar15);
        }
      }
      else {
        for (uVar14 = 0;
            (pvStack_2fc != (void *)0x0 &&
            (uVar14 < (uint)(((int)pvStack_2f8 - (int)pvStack_2fc) / 0x1c))); uVar14 = uVar14 + 1) {
          if ((pvStack_2d8 == (void *)0x0) ||
             ((uint)(((int)pvStack_2d4 - (int)pvStack_2d8) / 0x1c) <= uVar14)) {
            piStack_2f0 = (int *)0x0;
          }
          else {
            iVar10 = FUN_004140e0();
            if (*(uint *)(iVar10 + 0x18) < 0x10) {
              pcVar9 = (char *)(iVar10 + 4);
            }
            else {
              pcVar9 = *(char **)(iVar10 + 4);
            }
            piStack_2f0 = (int *)_atol(pcVar9);
          }
          FUN_00416ee0();
        }
      }
      pCVar24 = local_2c8;
      uVar14 = *(uint *)(local_2c8 + 0x7c);
      if (uVar14 < *(uint *)(local_2c8 + 0x78)) {
        FUN_004495e8();
      }
      uVar15 = *(uint *)(pCVar24 + 0x78);
      if (*(uint *)(pCVar24 + 0x7c) < uVar15) {
        FUN_004495e8();
      }
      if (uVar15 != uVar14) {
        uVar14 = FUN_004166d0();
        pCStackY_338 = (CHAR *)0x41b150;
        puStackY_334 = (undefined *)uVar14;
        FUN_00415a40();
        *(uint *)(pCVar24 + 0x7c) = uVar14;
      }
      while( true ) {
        pvVar21 = pvStack_2e8;
        if (((((*(int *)(local_2c8 + 0x78) != 0) &&
              (2 < (uint)(*(int *)(local_2c8 + 0x7c) - *(int *)(local_2c8 + 0x78) >> 2))) ||
             (pvStack_2fc == (void *)0x0)) ||
            ((iVar10 = (int)pvStack_2f8 - (int)pvStack_2fc >> 0x1f,
             ((int)pvStack_2f8 - (int)pvStack_2fc) / 0x1c + iVar10 == iVar10 ||
             (pvStack_2e8 == (void *)0x0)))) || ((int)pvStack_2e4 - (int)pvStack_2e8 >> 2 == 0))
        break;
        uVar14 = 0;
        iVar10 = FUN_00414020();
        if (iVar10 != 0) {
          do {
            if ((pvVar21 == (void *)0x0) || ((uint)((int)pvStack_2e4 - (int)pvVar21 >> 2) <= uVar14)
               ) {
              FUN_004495e8();
              pvVar21 = pvStack_2e8;
            }
            uVar14 = uVar14 + 1;
            uVar15 = FUN_00414020();
          } while (uVar14 < uVar15);
        }
        __time64((__time64_t *)0x0);
        iVar10 = __allrem();
        iVar10 = iVar10 + 1;
        uVar14 = 0;
        iVar12 = FUN_00414020();
        pvVar21 = pvStack_2e4;
        pvVar25 = pvStack_2e8;
        if (iVar12 != 0) {
          do {
            if (pvVar25 == (void *)0x0) {
LAB_0041b242:
              FUN_004495e8();
              pvVar21 = pvStack_2e4;
              pvVar25 = pvStack_2e8;
            }
            else if ((uint)((int)pvVar21 - (int)pvVar25 >> 2) <= uVar14) goto LAB_0041b242;
            iVar12 = *(int *)((int)pvVar25 + uVar14 * 4);
            if (iVar10 <= iVar12) break;
            iVar10 = iVar10 - iVar12;
            uVar14 = uVar14 + 1;
            uVar15 = FUN_00414020();
          } while (uVar14 < uVar15);
        }
        if (pvVar25 == (void *)0x0) {
          uVar15 = 0;
        }
        else {
          uVar15 = (int)pvVar21 - (int)pvVar25 >> 2;
        }
        if (uVar14 == uVar15) {
          uVar14 = 0;
        }
        if ((pvStack_2fc == (void *)0x0) ||
           ((uint)(((int)pvStack_2f8 - (int)pvStack_2fc) / 0x1c) <= uVar14)) {
          FUN_004495e8();
        }
        FUN_00415b50();
        uStack_4._0_1_ = 0x12;
        FUN_004189f0();
        piVar16 = piStack_2f0 + -4;
        uStack_4._0_1_ = 0xf;
        piVar23 = piStack_2f0 + -1;
        LOCK();
        iVar10 = *piVar23;
        *piVar23 = *piVar23 + -1;
        UNLOCK();
        if (iVar10 + -1 < 1) {
          (**(code **)(*(int *)*piVar16 + 4))();
        }
        pvVar21 = pvStack_2fc;
        if (pvStack_2f8 < pvStack_2fc) {
          FUN_004495e8();
        }
        pvVar25 = pvStack_2e8;
        pvStack_280 = pvVar21;
        if (pvStack_2e4 < pvStack_2e8) {
          FUN_004495e8();
        }
        pvStack_274 = pvVar25;
        pvVar4 = pvStack_2f8;
        pvVar2 = pvStack_280;
        while( true ) {
          pvVar3 = pvVar4;
          pvVar8 = pvVar2;
          if (uVar14 == 0) break;
          pvStack_2f8 = pvVar4;
          if (pvVar4 <= pvVar21) {
            FUN_004495e8();
          }
          pvVar21 = (void *)((int)pvVar21 + 0x1c);
          if (pvStack_2e4 <= pvVar25) {
            FUN_004495e8();
          }
          pvVar25 = (void *)((int)pvVar25 + 4);
          uVar14 = uVar14 - 1;
          pvVar4 = pvStack_2f8;
          pvVar2 = pvVar21;
        }
        while( true ) {
          pvStack_274 = pvVar25;
          pvStack_280 = pvVar8;
          pvVar2 = (void *)((int)pvVar2 + 0x1c);
          pvStack_2f8 = pvVar3;
          if (pvVar2 == pvVar4) break;
          FUN_00401ce0();
          pvVar3 = pvStack_2f8;
          pvVar8 = pvStack_280;
          pvVar25 = pvStack_274;
        }
        for (pvVar21 = (void *)((int)pvVar3 + -0x1c); pvVar21 != pvVar3;
            pvVar21 = (void *)((int)pvVar21 + 0x1c)) {
          if (0xf < *(uint *)((int)pvVar21 + 0x18)) {
            _free(*(void **)((int)pvVar21 + 4));
          }
          *(undefined4 *)((int)pvVar21 + 0x18) = 0xf;
          *(undefined4 *)((int)pvVar21 + 0x14) = 0;
          *(undefined1 *)((int)pvVar21 + 4) = 0;
        }
        pvStack_2f8 = (void *)((int)pvStack_2f8 + -0x1c);
        iVar10 = (int)pvStack_2e4 - ((int)pvStack_274 + 4) >> 2;
        if (0 < iVar10) {
          rVar18 = iVar10 * 4;
          _memmove_s(pvStack_274,rVar18,(void *)((int)pvStack_274 + 4),rVar18);
        }
        pvStack_2e4 = (void *)((int)pvStack_2e4 + -4);
      }
      if ((*(int *)(local_2c8 + 0x78) != 0) &&
         (*(int *)(local_2c8 + 0x7c) - *(int *)(local_2c8 + 0x78) >> 2 != 0)) {
        uVar14 = 0;
LAB_0041b5d0:
        pCVar24 = local_2c8;
        pvVar21 = (void *)0x0;
        pvVar25 = *(void **)(local_2c8 + 0x78);
        if ((pvVar25 == (void *)0x0) ||
           ((uint)(*(int *)(local_2c8 + 0x7c) - (int)pvVar25 >> 2) <= uVar14)) goto LAB_0041b749;
        if ((pvVar25 == (void *)0x0) ||
           ((uint)(*(int *)(local_2c8 + 0x7c) - (int)pvVar25 >> 2) <= uVar14)) {
          FUN_004495e8();
        }
        pvVar21 = (void *)(uVar14 * 4);
        pvVar25 = *(void **)(*(int *)(pCVar24 + 0x78) + (int)pvVar21);
        piVar23 = (int *)((int)pvVar25 + -0x10);
        puVar17 = (undefined4 *)(**(code **)(**(int **)((int)pvVar25 + -0x10) + 0x10))();
        if ((-1 < *(int *)((int)pvVar25 + -4)) && (puVar17 == (undefined4 *)*piVar23)) {
          LOCK();
          *(int *)((int)pvVar25 + -4) = *(int *)((int)pvVar25 + -4) + 1;
          UNLOCK();
LAB_0041b670:
          piStack_2f0 = piVar23 + 4;
          iVar10 = piVar23[1] + -1;
          uStack_4._0_1_ = 0x13;
          if ((iVar10 < 0) || (piVar23[1] < iVar10)) goto LAB_0041aa4e;
          if (*(char *)(iVar10 + (int)piStack_2f0) == '/') {
            pCVar19 = (CSimpleStringT<char,0> *)FUN_00414c80();
            uStack_4._0_1_ = 0x14;
            if ((*(int *)(local_2c8 + 0x78) == 0) ||
               ((uint)(*(int *)(local_2c8 + 0x7c) - *(int *)(local_2c8 + 0x78) >> 2) <= uVar14)) {
              FUN_004495e8();
            }
            ATL::CSimpleStringT<char,0>::operator=
                      ((CSimpleStringT<char,0> *)(*(int *)(local_2c8 + 0x78) + (int)pvVar21),pCVar19
                      );
            uStack_4._0_1_ = 0x13;
            piVar16 = (int *)(pcStack_284 + -4);
            LOCK();
            iVar10 = *piVar16;
            *piVar16 = *piVar16 + -1;
            UNLOCK();
            if (iVar10 + -1 < 1) {
              (**(code **)(**(int **)(pcStack_284 + -0x10) + 4))();
            }
          }
          uStack_4._0_1_ = 0xf;
          piVar16 = piVar23 + 3;
          LOCK();
          iVar10 = *piVar16;
          *piVar16 = *piVar16 + -1;
          UNLOCK();
          if (iVar10 + -1 < 1) {
            (**(code **)(*(int *)*piVar23 + 4))();
          }
          uVar14 = uVar14 + 1;
          goto LAB_0041b5d0;
        }
        piVar23 = (int *)(**(code **)*puVar17)();
        if (piVar23 != (int *)0x0) {
          piVar23[1] = *(int *)((int)pvVar25 + -0xc);
          rVar18 = *(int *)((int)pvVar25 + -0xc) + 1;
          _memcpy_s(piVar23 + 4,rVar18,pvVar25,rVar18);
          goto LAB_0041b670;
        }
        FUN_004039c0();
        pCVar24 = (LPCSTR)0x0;
        pvVar25 = extraout_ECX;
LAB_0041b749:
        if ((pvVar25 == pvVar21) || (*(int *)(pCVar24 + 0x7c) - (int)pvVar25 >> 2 == 0)) {
          FUN_004495e8();
        }
        pCVar19 = *(CSimpleStringT<char,0> **)(pCVar24 + 0x78);
        if ((int)(*(uint *)(*(int *)pCVar19 + -8) | 1U - *(int *)(*(int *)pCVar19 + -4)) < 0) {
          ATL::CSimpleStringT<char,0>::PrepareWrite2(pCVar19,(int)pvVar21);
        }
        puVar7 = local_2c0;
        _strcpy_s((char *)(local_2c0 + 4),0x104,*(char **)pCVar19);
        uStack_288 = 0xf;
        pvStack_29c = (void *)((uint)pvStack_29c & 0xffffff00);
        pvStack_28c = pvVar21;
        FUN_00401dc0();
        pcVar9 = (char *)((int)register0x00000010 + 0x31c);
        uStack_4._0_1_ = 0x15;
        uStack_2a4 = 0xf;
        pvStack_2a8 = pvVar21;
        pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
        do {
          cVar1 = *pcVar9;
          pcVar9 = pcVar9 + 1;
        } while (cVar1 != '\0');
        FUN_00401dc0();
        uStack_4 = CONCAT31(uStack_4._1_3_,0x16);
        FUN_004128c0();
        if (0xf < uStack_2a4) {
          _free(pvStack_2b8);
        }
        uStack_2a4 = 0xf;
        pvStack_2b8 = (void *)((uint)pvStack_2b8 & 0xffffff00);
        uStack_4._0_1_ = 0xf;
        pvStack_2a8 = pvVar21;
        if (0xf < uStack_288) {
          _free(pvStack_29c);
        }
        *(uint *)(pCVar24 + 0x60) = *puVar7;
        *(uint *)(pCVar24 + 100) = puVar7[1];
        *(uint *)(pCVar24 + 0x6c) = puVar7[2];
        iVar10 = FUN_00420b70();
        pCVar6 = local_2c8;
        if (iVar10 == 0) {
          FUN_004205f0();
          FUN_00420370();
          uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
          piVar23 = (int *)(iStack_2c4 + -4);
          LOCK();
          iVar10 = *piVar23;
          *piVar23 = *piVar23 + -1;
          UNLOCK();
          if (iVar10 + -1 < 1) {
            (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
          }
          if (pvStack_2e8 != pvVar21) {
            _free(pvStack_2e8);
          }
          pvStack_2e8 = pvVar21;
          pvStack_2e4 = pvVar21;
          pvStack_2e0 = pvVar21;
          if (pvStack_2d8 != pvVar21) {
            FUN_00412cb0();
            _free(pvStack_2d8);
          }
          pvStack_2d8 = pvVar21;
          pvStack_2d4 = pvVar21;
          pvStack_2d0 = pvVar21;
          if (pvStack_2fc != pvVar21) {
            FUN_00412cb0();
            _free(pvStack_2fc);
          }
          uStack_4 = uStack_4 & 0xffffff00;
          pvStack_2fc = pvVar21;
          pvStack_2f8 = pvVar21;
          pvStack_2f4 = pvVar21;
          FUN_00412930();
          uStack_4 = 0xffffffff;
          pwVar22 = pwStack_2cc + -2;
          LOCK();
          iVar10 = *(int *)pwVar22;
          *(int *)pwVar22 = *(int *)pwVar22 + -1;
          UNLOCK();
          if (iVar10 + -1 < 1) {
            (**(code **)(**(int **)(pwStack_2cc + -8) + 4))();
          }
        }
        else {
          uVar14 = *(uint *)(pCVar24 + 0x68);
          puVar7[3] = uVar14;
          if (uVar14 < puVar7[1]) {
            FUN_004205f0();
            FUN_00420370();
            uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
            piVar23 = (int *)(iStack_2c4 + -4);
            LOCK();
            iVar10 = *piVar23;
            *piVar23 = *piVar23 + -1;
            UNLOCK();
            if (iVar10 + -1 < 1) {
              (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
            }
            if (pvStack_2e8 != pvVar21) {
              _free(pvStack_2e8);
            }
            pvStack_2e8 = pvVar21;
            pvStack_2e4 = pvVar21;
            pvStack_2e0 = pvVar21;
            if (pvStack_2d8 != pvVar21) {
              FUN_00412cb0();
              _free(pvStack_2d8);
            }
            pvStack_2d8 = pvVar21;
            pvStack_2d4 = pvVar21;
            pvStack_2d0 = pvVar21;
            if (pvStack_2fc != pvVar21) {
              FUN_00412cb0();
              _free(pvStack_2fc);
            }
            uStack_4 = uStack_4 & 0xffffff00;
            pvStack_2fc = pvVar21;
            pvStack_2f8 = pvVar21;
            pvStack_2f4 = pvVar21;
            FUN_00412930();
            uStack_4 = 0xffffffff;
            pwVar22 = pwStack_2cc + -2;
            LOCK();
            iVar10 = *(int *)pwVar22;
            *(int *)pwVar22 = *(int *)pwVar22 + -1;
            UNLOCK();
            if (iVar10 + -1 < 1) {
              (**(code **)(**(int **)(pwStack_2cc + -8) + 4))();
            }
          }
          else if (*puVar7 < uVar14) {
            FUN_004205f0();
            FUN_00420370();
            uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
            piVar23 = (int *)(iStack_2c4 + -4);
            LOCK();
            iVar10 = *piVar23;
            *piVar23 = *piVar23 + -1;
            UNLOCK();
            if (iVar10 + -1 < 1) {
              (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
            }
            if (pvStack_2e8 != pvVar21) {
              _free(pvStack_2e8);
            }
            pvStack_2e8 = pvVar21;
            pvStack_2e4 = pvVar21;
            pvStack_2e0 = pvVar21;
            if (pvStack_2d8 != pvVar21) {
              FUN_00412cb0();
              _free(pvStack_2d8);
            }
            pvStack_2d8 = pvVar21;
            pvStack_2d4 = pvVar21;
            pvStack_2d0 = pvVar21;
            if (pvStack_2fc != pvVar21) {
              FUN_00412cb0();
              _free(pvStack_2fc);
            }
            uStack_4 = uStack_4 & 0xffffff00;
            pvStack_2fc = pvVar21;
            pvStack_2f8 = pvVar21;
            pvStack_2f4 = pvVar21;
            FUN_00412930();
            uStack_4 = 0xffffffff;
            pwVar22 = pwStack_2cc + -2;
            LOCK();
            iVar10 = *(int *)pwVar22;
            *(int *)pwVar22 = *(int *)pwVar22 + -1;
            UNLOCK();
            if (iVar10 + -1 < 1) {
              (**(code **)(**(int **)(pwStack_2cc + -8) + 4))();
            }
          }
          else {
            uVar14 = *(uint *)(pCVar24 + 0x68);
            *(uint *)(local_2c8 + 0xbf8) = *(int *)(local_2c8 + 0x60) - uVar14;
            *(uint *)(local_2c8 + 3000) = uVar14;
            *(uint *)(local_2c8 + 0xbbc) = uVar14;
            FUN_00413990();
            iVar10 = 0;
            do {
              apuStack_304[0] = (undefined1 *)FUN_00429112();
              uStack_4._0_1_ = 0x17;
              if (apuStack_304[0] == (undefined1 *)0x0) {
                piVar23 = (int *)0x0;
              }
              else {
                piVar23 = (int *)FUN_004228f0();
              }
              uStack_4._0_1_ = 0xf;
              piStack_2f0 = piVar23;
              if (piVar23 == (int *)0x0) {
                FUN_004205f0();
LAB_0041bd8f:
                FUN_00420370();
                piVar23 = (int *)(iStack_2c4 + -4);
                uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
                LOCK();
                iVar10 = *piVar23;
                *piVar23 = *piVar23 + -1;
                UNLOCK();
                if (iVar10 + -1 < 1) {
                  (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
                }
                if (pvStack_2e8 != (void *)0x0) {
                  _free(pvStack_2e8);
                }
                pvStack_2e0 = (void *)0x0;
                pvStack_2e4 = (void *)0x0;
                pvStack_2e8 = (void *)0x0;
                if (pvStack_2d8 != (void *)0x0) {
                  FUN_00412cb0();
                  _free(pvStack_2d8);
                }
                pvStack_2d0 = (void *)0x0;
                pvStack_2d4 = (void *)0x0;
                pvStack_2d8 = (void *)0x0;
                if (pvStack_2fc != (void *)0x0) {
                  FUN_00412cb0();
                  _free(pvStack_2fc);
                }
                pvStack_2fc = (void *)0x0;
                pvStack_2f8 = (void *)0x0;
                pvStack_2f4 = (void *)0x0;
                goto LAB_0041be4c;
              }
              iVar12 = FUN_00422ff0();
              if (iVar12 == 0) {
                (**(code **)*piVar23)();
                FUN_004205f0();
                goto LAB_0041bd8f;
              }
              FUN_00416ee0();
              iVar10 = iVar10 + 1;
            } while (iVar10 < 1);
            if (local_2c0[3] == *local_2c0) {
              FUN_00420370();
              uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
              piVar23 = (int *)(iStack_2c4 + -4);
              LOCK();
              iVar10 = *piVar23;
              *piVar23 = *piVar23 + -1;
              UNLOCK();
              if (iVar10 + -1 < 1) {
                (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
              }
              if (pvStack_2e8 != (void *)0x0) {
                _free(pvStack_2e8);
              }
              pvStack_2e8 = (void *)0x0;
              pvStack_2e4 = (void *)0x0;
              pvStack_2e0 = (void *)0x0;
              if (pvStack_2d8 != (void *)0x0) {
                FUN_00412cb0();
                _free(pvStack_2d8);
              }
              pvStack_2d8 = (void *)0x0;
              pvStack_2d4 = (void *)0x0;
              pvStack_2d0 = (void *)0x0;
              if (pvStack_2fc != (void *)0x0) {
                FUN_00412cb0();
                _free(pvStack_2fc);
              }
              pvStack_2fc = (void *)0x0;
              pvStack_2f8 = (void *)0x0;
              pvStack_2f4 = (void *)0x0;
              uStack_4 = uStack_4 & 0xffffff00;
              FUN_00412930();
              uStack_4 = 0xffffffff;
              pwVar22 = pwStack_2cc + -2;
              LOCK();
              iVar10 = *(int *)pwVar22;
              *(int *)pwVar22 = *(int *)pwVar22 + -1;
              UNLOCK();
              if (iVar10 + -1 < 1) {
                (**(code **)(**(int **)(pwStack_2cc + -8) + 4))();
              }
            }
            else {
              if ((in_stack_00000420 & 4) != 0) {
                iVar10 = (**(code **)(DAT_0048e2e4 + 0xc))();
                local_2c8 = (LPCSTR)(iVar10 + 0x10);
                uStack_4 = CONCAT31(uStack_4._1_3_,0x18);
                FUN_00415690();
                pCVar24 = local_2c8;
                apuStack_304[0] = FindFirstFileA(local_2c8,&_Stack_250);
                if (apuStack_304[0] != (undefined1 *)0xffffffff) {
                  puStackY_334 = (undefined *)0x3;
                  pCStackY_338 = aCStack_27c;
                  pcStackY_33c = pCVar24;
                  pcStackY_340 = (undefined *)0x41bf39;
                  FUN_0044b222();
                  do {
                    if (((((byte)_Stack_250.dwFileAttributes & 0x10) != 0) &&
                        (iVar10 = __mbsncmp((uchar *)_Stack_250.cFileName,".",0x104), iVar10 != 0))
                       && (iVar10 = __mbsncmp((uchar *)_Stack_250.cFileName,"..",0x104), iVar10 != 0
                          )) {
                      FUN_00415b50();
                      iVar10 = *(int *)(pcStack_284 + -0xc);
                      iVar12 = 0;
                      uStack_4._0_1_ = 0x19;
                      if (0 < iVar10) {
                        do {
                          if ((iVar12 < 0) || (iVar10 < iVar12)) goto LAB_0041aa4e;
                          if (pcStack_284[iVar12] < '0') goto LAB_0041c083;
                          if (iVar10 < iVar12) goto LAB_0041aa4e;
                          if ('9' < pcStack_284[iVar12]) goto LAB_0041c083;
                          iVar12 = iVar12 + 1;
                        } while (iVar12 < iVar10);
                      }
                      lVar20 = _atol(pcStack_284);
                      if (lVar20 < (int)local_2c0[3]) {
                        iVar10 = (**(code **)(DAT_0048e2e4 + 0xc))();
                        piStack_2f0 = (int *)(iVar10 + 0x10);
                        uStack_4._0_1_ = 0x1a;
                        FUN_00415690();
                        piVar16 = piStack_2f0;
                        puStackY_334 = (undefined *)0x41c056;
                        FUN_00421530();
                        uStack_4._0_1_ = 0x19;
                        piVar23 = piVar16 + -1;
                        LOCK();
                        iVar10 = *piVar23;
                        *piVar23 = *piVar23 + -1;
                        UNLOCK();
                        if (iVar10 + -1 < 1) {
                          (**(code **)(*(int *)piVar16[-4] + 4))();
                        }
                      }
LAB_0041c083:
                      uStack_4 = CONCAT31(uStack_4._1_3_,0x18);
                      piVar23 = (int *)(pcStack_284 + -4);
                      LOCK();
                      iVar10 = *piVar23;
                      *piVar23 = *piVar23 + -1;
                      UNLOCK();
                      if (iVar10 + -1 < 1) {
                        (**(code **)(**(int **)(pcStack_284 + -0x10) + 4))();
                      }
                    }
                    hFindFile = apuStack_304[0];
                    BVar11 = FindNextFileA(apuStack_304[0],&_Stack_250);
                  } while (BVar11 != 0);
                  FindClose(hFindFile);
                  pCVar24 = local_2c8;
                }
                uStack_4._0_1_ = 0xf;
                piVar23 = (int *)(pCVar24 + -4);
                LOCK();
                iVar10 = *piVar23;
                *piVar23 = *piVar23 + -1;
                UNLOCK();
                if (iVar10 + -1 < 1) {
                  (**(code **)(**(int **)(pCVar24 + -0x10) + 4))();
                }
              }
              piVar23 = (int *)FUN_004230d0();
              *(int **)(pCVar6 + 0xc08) = piVar23;
              if (piVar23 == (int *)0x0) {
                FUN_004205f0();
                FUN_00420370();
                uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
                piVar23 = (int *)(iStack_2c4 + -4);
                LOCK();
                iVar10 = *piVar23;
                *piVar23 = *piVar23 + -1;
                UNLOCK();
                if (iVar10 + -1 < 1) {
                  (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
                }
                if (pvStack_2e8 != (void *)0x0) {
                  _free(pvStack_2e8);
                }
                pvStack_2e8 = (void *)0x0;
                pvStack_2e4 = (void *)0x0;
                pvStack_2e0 = (void *)0x0;
                if (pvStack_2d8 != (void *)0x0) {
                  FUN_00412cb0();
                  _free(pvStack_2d8);
                }
                pvStack_2d8 = (void *)0x0;
                pvStack_2d4 = (void *)0x0;
                pvStack_2d0 = (void *)0x0;
                if (pvStack_2fc != (void *)0x0) {
                  FUN_00412cb0();
                  _free(pvStack_2fc);
                }
                pvStack_2fc = (void *)0x0;
                pvStack_2f8 = (void *)0x0;
                pvStack_2f4 = (void *)0x0;
              }
              else {
                iVar10 = (**(code **)(*piVar23 + 4))();
                if (-1 < iVar10) {
                  uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
                  piVar23 = (int *)(iStack_2c4 + -4);
                  LOCK();
                  iVar10 = *piVar23;
                  *piVar23 = *piVar23 + -1;
                  UNLOCK();
                  if (iVar10 + -1 < 1) {
                    (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
                  }
                  if (pvStack_2e8 != (void *)0x0) {
                    _free(pvStack_2e8);
                  }
                  pvStack_2e8 = (void *)0x0;
                  pvStack_2e4 = (void *)0x0;
                  pvStack_2e0 = (void *)0x0;
                  FUN_00417e80();
                  FUN_00417e80();
                  uStack_4 = uStack_4 & 0xffffff00;
                  FUN_00412930();
                  uStack_4 = 0xffffffff;
                  pwVar22 = pwStack_2cc + -2;
                  LOCK();
                  iVar10 = *(int *)pwVar22;
                  *(int *)pwVar22 = *(int *)pwVar22 + -1;
                  UNLOCK();
                  if (iVar10 + -1 < 1) {
                    (**(code **)(**(int **)(pwStack_2cc + -8) + 4))();
                  }
                  goto LAB_0041c342;
                }
                FUN_004205f0();
                puStackY_334 = (undefined *)0x8;
                pCStackY_338 = (CHAR *)0x41c24b;
                FUN_00420370();
                uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
                piVar23 = (int *)(iStack_2c4 + -4);
                LOCK();
                iVar10 = *piVar23;
                *piVar23 = *piVar23 + -1;
                UNLOCK();
                if (iVar10 + -1 < 1) {
                  (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
                }
                if (pvStack_2e8 != (void *)0x0) {
                  _free(pvStack_2e8);
                }
                pvStack_2e8 = (void *)0x0;
                pvStack_2e4 = (void *)0x0;
                pvStack_2e0 = (void *)0x0;
                FUN_00417e80();
                FUN_00417e80();
              }
LAB_0041be4c:
              uStack_4 = uStack_4 & 0xffffff00;
              FUN_00412930();
              uStack_4 = 0xffffffff;
              pwVar22 = pwStack_2cc + -2;
              LOCK();
              iVar10 = *(int *)pwVar22;
              *(int *)pwVar22 = *(int *)pwVar22 + -1;
              UNLOCK();
              if (iVar10 + -1 < 1) {
                (**(code **)(**(int **)(pwStack_2cc + -8) + 4))();
              }
            }
          }
        }
        goto LAB_0041c342;
      }
      FUN_004205f0();
      FUN_00420370();
      uStack_4 = CONCAT31(uStack_4._1_3_,0xd);
      piVar23 = (int *)(iStack_2c4 + -4);
      LOCK();
      iVar10 = *piVar23;
      *piVar23 = *piVar23 + -1;
      UNLOCK();
      if (iVar10 + -1 < 1) {
        (**(code **)(**(int **)(iStack_2c4 + -0x10) + 4))();
      }
      if (pvStack_2e8 != (void *)0x0) {
        _free(pvStack_2e8);
      }
      pvStack_2e8 = (void *)0x0;
      pvStack_2e4 = (void *)0x0;
      pvStack_2e0 = (void *)0x0;
      if (pvStack_2d8 != (void *)0x0) {
        FUN_00412cb0();
        _free(pvStack_2d8);
      }
      pvStack_2d8 = (void *)0x0;
      pvStack_2d4 = (void *)0x0;
      pvStack_2d0 = (void *)0x0;
      if (pvStack_2fc != (void *)0x0) {
        FUN_00412cb0();
        _free(pvStack_2fc);
      }
      pvStack_2fc = (void *)0x0;
      pvStack_2f8 = (void *)0x0;
      pvStack_2f4 = (void *)0x0;
      uStack_4 = uStack_4 & 0xffffff00;
      FUN_00412930();
      pwVar22 = pwStack_2cc;
      goto LAB_0041aa20;
    }
    FUN_004205f0();
    FUN_00420370();
    uStack_4 = (uint)uStack_4._1_3_ << 8;
    FUN_00412930();
    pwVar13 = pwVar22 + -2;
    LOCK();
    iVar10 = *(int *)pwVar13;
    *(int *)pwVar13 = *(int *)pwVar13 + -1;
    UNLOCK();
  }
  else {
    FUN_004205f0();
    FUN_00420370();
LAB_0041aa1d:
LAB_0041aa20:
    pwVar13 = pwVar22 + -2;
    LOCK();
    iVar10 = *(int *)pwVar13;
    *(int *)pwVar13 = *(int *)pwVar13 + -1;
    UNLOCK();
  }
  uStack_4 = 0xffffffff;
  if (iVar10 + -1 < 1) {
    (**(code **)(**(int **)(pwVar22 + -8) + 4))();
  }
LAB_0041c342:
  ExceptionList = local_c;
  __security_check_cookie(local_10 ^ (uint)apuStack_304);
  return;
}



// Function 00407ce0

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_00407ce0(int *param_1,int param_2)

{
  CSimpleStringT<char,0> *this;
  CStringData *pCVar1;
  CStringData *pCVar2;
  char cVar3;
  short sVar4;
  int iVar5;
  int iVar6;
  HANDLE pvVar7;
  LPWSTR pWVar8;
  short *psVar9;
  char *pcVar10;
  undefined4 *puVar11;
  int iVar12;
  CStringData *pCVar13;
  int *piVar14;
  int *piVar15;
  int aiStackY_6d0 [257];
  undefined4 uStackY_2cc;
  undefined1 auStack_290 [4];
  int local_28c;
  CStringData *pCStack_288;
  CStringData *local_284;
  uint uStack_280;
  int local_27c [2];
  void *pvStack_274;
  undefined4 uStack_264;
  uint uStack_260;
  void *pvStack_258;
  undefined4 uStack_248;
  uint uStack_244;
  char local_220 [264];
  CHAR CStack_118;
  undefined1 auStack_117 [263];
  uint local_10;
  void *local_c;
  undefined1 *puStack_8;
  undefined4 local_4;

  local_4 = 0xffffffff;
  puStack_8 = &LAB_0046bcff;
  local_c = ExceptionList;
  local_10 = DAT_0048790c ^ (uint)auStack_290;
  ExceptionList = &local_c;
  param_1[1] = param_2;
  pvVar7 = OpenEventW(0x1f0003,0,L"DFF954A9-35DE-4f82-862F-37B26F9F75C7");
  if (pvVar7 == (HANDLE)0x0) {
    CreateEventW((LPSECURITY_ATTRIBUTES)0x0,0,0,L"DFF954A9-35DE-4f82-862F-37B26F9F75C7");
    piVar14 = local_27c;
    local_27c[0] = 0;
    pWVar8 = GetCommandLineW();
    local_284 = (CStringData *)CommandLineToArgvW(pWVar8,piVar14);
    if (local_284 != (CStringData *)0x0) {
      local_28c = 1;
      if (1 < local_27c[0]) {
        this = (CSimpleStringT<char,0> *)(param_1 + 3);
        do {
          psVar9 = *(short **)(local_284 + local_28c * 4);
          if (psVar9 != (short *)0x0) {
            do {
              sVar4 = *psVar9;
              psVar9 = psVar9 + 1;
            } while (sVar4 != 0);
          }
          FUN_004092e0();
          iVar5 = *(int *)this;
          iVar6 = *(int *)(iVar5 + -0xc);
          iVar12 = iVar6 + 1;
          if ((int)(*(int *)(iVar5 + -8) - iVar12 | 1U - *(int *)(iVar5 + -4)) < 0) {
            ATL::CSimpleStringT<char,0>::PrepareWrite2(this,iVar12);
          }
          *(undefined2 *)(*(int *)this + iVar6 * 2) = 0x20;
          if ((iVar12 < 0) || (*(int *)(*(int *)this + -8) < iVar12)) {
                    /* WARNING: Subroutine does not return */
            FUN_004038a0();
          }
          *(int *)(*(int *)this + -0xc) = iVar12;
          *(undefined2 *)(*(int *)this + iVar12 * 2) = 0;
          local_28c = local_28c + 1;
        } while (local_28c < local_27c[0]);
      }
      if (0 < *(int *)(param_1[3] + -0xc)) {
        FUN_00408a20();
      }
    }
    if (param_1[0x15a] == 0) {
      FUN_00408830();
      local_4 = 0;
      FUN_00407860();
      local_4 = CONCAT31(local_4._1_3_,2);
      pCVar1 = local_284 + -4;
      LOCK();
      pWVar8 = *(LPWSTR *)pCVar1;
      *(LPWSTR *)pCVar1 = (LPWSTR)((int)*(LPWSTR *)pCVar1 + -1);
      UNLOCK();
      if ((int)pWVar8 + -1 < 1) {
        (**(code **)(*(int *)*(LPWSTR *)(local_284 + -0x10) + 4))();
      }
      if (0 < *(int *)(local_28c + -0xc)) {
        (**(code **)(*param_1 + 0x10))();
        (**(code **)(*param_1 + 0x14))();
        local_4 = 0xffffffff;
        piVar14 = (int *)(local_28c + -4);
        LOCK();
        iVar12 = *piVar14;
        *piVar14 = *piVar14 + -1;
        UNLOCK();
        if (iVar12 + -1 < 1) {
          (**(code **)(**(int **)(local_28c + -0x10) + 4))();
        }
        goto LAB_00408363;
      }
      local_4 = 0xffffffff;
      piVar14 = (int *)(local_28c + -4);
      LOCK();
      iVar12 = *piVar14;
      *piVar14 = *piVar14 + -1;
      UNLOCK();
      if (iVar12 + -1 < 1) {
        (**(code **)(**(int **)(local_28c + -0x10) + 4))();
      }
    }
    if (*(int *)(param_1[6] + -0xc) == 0) {
      local_220[0] = '\0';
      _memset(local_220 + 1,0,0x103);
      iVar12 = FUN_00408790();
      if (iVar12 == 0) {
        iVar12 = 2;
        goto LAB_00408313;
      }
      pcVar10 = local_220;
      do {
        cVar3 = *pcVar10;
        pcVar10 = pcVar10 + 1;
      } while (cVar3 != '\0');
      ATL::CSimpleStringT<char,0>::SetString
                ((CSimpleStringT<char,0> *)(param_1 + 6),local_220,
                 (int)pcVar10 - (int)(local_220 + 1));
    }
    if (*(int *)(param_1[3] + -0xc) < 0) {
      iVar12 = -1;
    }
    else {
      iVar12 = FUN_0044a27e();
      if (iVar12 == 0) {
        iVar12 = -1;
      }
      else {
        iVar12 = iVar12 - param_1[3] >> 1;
      }
    }
    uStack_280 = (uint)(-1 < iVar12);
    CStack_118 = '\0';
    _memset(auStack_117,0,0x103);
    GetCurrentDirectoryA(0x104,&CStack_118);
    FUN_00408b80();
    local_4 = 6;
    FUN_00408c40();
    __mbsrchr((uchar *)pCStack_288,0x5c);
    puVar11 = (undefined4 *)FUN_00408cf0();
    local_4._0_1_ = 7;
    pcVar10 = (char *)*puVar11;
    pCVar1 = (CStringData *)(pcVar10 + -0x10);
    pCVar13 = pCStack_288 + -0x10;
    if (pCVar1 != pCVar13) {
      pCVar2 = pCStack_288 + -4;
      if (((int)*(LPWSTR *)(pCStack_288 + -4) < 0) || (*(int *)pCVar1 != *(int *)pCVar13)) {
        ATL::CSimpleStringT<char,0>::SetString
                  ((CSimpleStringT<char,0> *)&pCStack_288,pcVar10,*(int *)(pcVar10 + -0xc));
      }
      else {
        local_284 = ATL::CSimpleStringT<char,0>::CloneData(pCVar1);
        LOCK();
        pWVar8 = *(LPWSTR *)pCVar2;
        *(LPWSTR *)pCVar2 = (LPWSTR)((int)*(LPWSTR *)pCVar2 + -1);
        UNLOCK();
        if ((int)pWVar8 + -1 < 1) {
          (**(code **)(**(int **)pCVar13 + 4))();
        }
        pCStack_288 = local_284 + 0x10;
      }
    }
    local_4 = CONCAT31(local_4._1_3_,6);
    piVar14 = (int *)(local_28c + -4);
    LOCK();
    iVar12 = *piVar14;
    *piVar14 = *piVar14 + -1;
    UNLOCK();
    if (iVar12 + -1 < 1) {
      (**(code **)(**(int **)(local_28c + -0x10) + 4))();
    }
    param_1[0x110] = 0;
    param_1[8] = param_2;
    param_1[9] = 0x7e8;
    param_1[10] = 0x7e9;
    param_1[0x10f] = 5;
    if (uStack_280 != 0) {
      param_1[0x10f] = 7;
    }
    param_1[0x111] = 0;
    _strcpy_s((char *)(param_1 + 0x4c),0x104,&CStack_118);
    if (1 < *(int *)(param_1[6] + -4)) {
      ATL::CSimpleStringT<char,0>::Fork
                ((CSimpleStringT<char,0> *)(param_1 + 6),*(int *)(param_1[6] + -0xc));
    }
    _strcpy_s((char *)(param_1 + 0xb),0x104,*(char **)(param_1 + 6));
    _strcpy_s((char *)(param_1 + 0x8d),0x104,"VERSION");
    uStackY_2cc = 0x40817b;
    _strcpy_s((char *)(param_1 + 0xce),0x104,"DOWNLOAD");
    piVar14 = param_1 + 8;
    piVar15 = aiStackY_6d0;
    for (iVar12 = 0x10a; iVar12 != 0; iVar12 = iVar12 + -1) {
      *piVar15 = *piVar14;
      piVar14 = piVar14 + 1;
      piVar15 = piVar15 + 1;
    }
    iVar12 = FUN_00412650();
    _DAT_0048f458 = 0;
    FUN_00412910();
    local_4._0_1_ = 9;
    uStack_280 = 1;
    uStack_260 = 0xf;
    uStack_264 = 0;
    pvStack_274 = (void *)((uint)pvStack_274 & 0xffffff00);
    FUN_00401dc0();
    local_4._0_1_ = 10;
    FUN_00412bc0();
    local_4._0_1_ = 9;
    if (0xf < uStack_260) {
      _free(pvStack_274);
    }
    uStack_260 = 0xf;
    uStack_264 = 0;
    pvStack_274 = (void *)((uint)pvStack_274 & 0xffffff00);
    uStack_244 = 0xf;
    uStack_248 = 0;
    pvStack_258 = (void *)((uint)pvStack_258 & 0xffffff00);
    FUN_00401dc0();
    local_4._0_1_ = 0xb;
    uStack_260 = 0xf;
    uStack_264 = 0;
    pvStack_274 = (void *)((uint)pvStack_274 & 0xffffff00);
    FUN_00401dc0();
    local_4._0_1_ = 0xc;
    _DAT_0048f458 = FUN_004128c0();
    local_4._0_1_ = 0xb;
    if (0xf < uStack_260) {
      _free(pvStack_274);
    }
    uStack_260 = 0xf;
    uStack_264 = 0;
    pvStack_274 = (void *)((uint)pvStack_274 & 0xffffff00);
    local_4._0_1_ = 9;
    if (0xf < uStack_244) {
      _free(pvStack_258);
    }
    uStack_244 = 0xf;
    uStack_248 = 0;
    pvStack_258 = (void *)((uint)pvStack_258 & 0xffffff00);
    local_4 = CONCAT31(local_4._1_3_,6);
    FUN_00412930();
    local_4 = 0xffffffff;
    pCVar1 = pCStack_288 + -4;
    LOCK();
    pWVar8 = *(LPWSTR *)pCVar1;
    *(LPWSTR *)pCVar1 = (LPWSTR)((int)*(LPWSTR *)pCVar1 + -1);
    UNLOCK();
    if ((int)pWVar8 + -1 < 1) {
      (**(code **)(*(int *)*(LPWSTR *)(pCStack_288 + -0x10) + 4))();
    }
    if ((iVar12 == 0) || (iVar12 == 7)) {
      param_1[0x157] = 1;
      param_1[0x158] = (uint)(iVar12 != 7);
      param_1[0x159] = (uint)(0x11 < (uint)param_1[0x114]);
      goto LAB_00408363;
    }
  }
  else {
    iVar12 = 1;
  }
LAB_00408313:
  param_1[0x157] = 0;
  if (iVar12 != 0) {
    FUN_004127b0();
    (**(code **)(*param_1 + 4))();
  }
LAB_00408363:
  ExceptionList = local_c;
  __security_check_cookie(local_10 ^ (uint)auStack_290);
  return;
}



// Function 00408830

void __fastcall FUN_00408830(int param_1)

{
  DWORD DVar1;
  int iVar2;
  UINT UVar3;
  CSimpleStringT<char,0> *this;
  undefined4 unaff_EDI;
  undefined1 local_328 [4];
  undefined4 local_324;
  CHAR local_31c;
  undefined1 local_31b [259];
  CHAR local_218;
  undefined1 local_217 [259];
  CHAR local_114;
  undefined1 local_113 [259];
  uint local_10;
  void *local_c;
  undefined1 *puStack_8;
  uint local_4;

  local_4 = 0xffffffff;
  puStack_8 = &LAB_0046af5d;
  local_c = ExceptionList;
  local_10 = DAT_0048790c ^ (uint)local_328;
  ExceptionList = &local_c;
  local_324 = 0;
  FUN_00406bc0(local_328);
  local_4 = 1;
  this = (CSimpleStringT<char,0> *)(param_1 + 0x1c);
  if (*(int *)(*(int *)(param_1 + 0x1c) + -0xc) == 0) {
    local_114 = '\0';
    _memset(local_113,0,0x103);
    DVar1 = GetPrivateProfileStringA("URL","MaintenanceUrl","",&local_114,0x104,".\\localinfo.dat");
    if (DVar1 == 0) {
      FUN_00408b20(unaff_EDI,local_328);
      goto LAB_004089d6;
    }
  }
  local_31c = '\0';
  _memset(local_31b,0,0x103);
  local_218 = '\0';
  _memset(local_217,0,0x103);
  GetTempPathA(0x104,&local_31c);
  wsprintfA(&local_31c,"%smaintenance.ini",&local_31c);
  if (1 < *(int *)(*(int *)this + -4)) {
    ATL::CSimpleStringT<char,0>::Fork(this,*(int *)(*(int *)this + -0xc));
  }
  iVar2 = FUN_00412800(*(undefined4 *)this,&local_31c);
  if (((iVar2 != 0) &&
      (UVar3 = GetPrivateProfileIntA("MAINTENANCE","IsUsed",0,&local_31c), UVar3 != 0)) &&
     (DVar1 = GetPrivateProfileStringA
                        ("MAINTENANCE","Msg","Maintenance",&local_218,0x104,&local_31c), DVar1 != 0)
     ) {
    FUN_00408ae0(&local_218);
  }
  FUN_00408b20(unaff_EDI,local_328);
LAB_004089d6:
  local_324 = 1;
  local_4 = local_4 & 0xffffff00;
  FUN_00406c30();
  ExceptionList = local_c;
  __security_check_cookie(local_10 ^ (uint)local_328);
  return;
}



// Function 00408790

void FUN_00408790(void)

{
  DWORD DVar1;
  char *unaff_ESI;
  CHAR local_108;
  undefined1 local_107 [259];
  uint local_4;

  local_4 = DAT_0048790c ^ (uint)&local_108;
  local_108 = '\0';
  _memset(local_107,0,0x103);
  DVar1 = GetPrivateProfileStringA("URL","VersionUrl","",&local_108,0x104,".\\localinfo.dat");
  if (DVar1 == 0) {
    __security_check_cookie(local_4 ^ (uint)&local_108);
    return;
  }
  _strcpy_s(unaff_ESI,0x104,&local_108);
  __security_check_cookie(local_4 ^ (uint)&local_108);
  return;
}
