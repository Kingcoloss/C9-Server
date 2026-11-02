
/* 1011e656 FUN_1011e656 */

/* WARNING: Function: __SEH_prolog4 replaced with injection: SEH_prolog4 */
/* WARNING: Function: __SEH_epilog4 replaced with injection: EH_epilog3 */

undefined4 FUN_1011e656(undefined4 param_1,int param_2,int param_3)

{
  int iVar1;
  undefined4 uVar2;
  DWORD *_Memory;
  DWORD DVar3;
  
  if (param_2 == 1) {
    iVar1 = FUN_1012889d();
    if (iVar1 != 0) {
      iVar1 = FUN_10128cde();
      if (iVar1 != 0) {
        FUN_1012987b();
        DAT_101a163c = GetCommandLineA();
        DAT_1019f44c = FUN_10129423();
        iVar1 = FUN_10128d71();
        if (-1 < iVar1) {
          iVar1 = FUN_10129077();
          if (-1 < iVar1) {
            iVar1 = FUN_101292a6();
            if (-1 < iVar1) {
              iVar1 = FUN_1011f9d2(0);
              if (iVar1 == 0) {
                DAT_1019f448 = DAT_1019f448 + 1;
                goto LAB_1011e7bd;
              }
            }
          }
          __ioterm();
        }
        __mtterm();
      }
      FUN_101288b2();
    }
LAB_1011e673:
    uVar2 = 0;
  }
  else {
    if (param_2 == 0) {
      if (DAT_1019f448 < 1) goto LAB_1011e673;
      DAT_1019f448 = DAT_1019f448 + -1;
      if (DAT_1019f47c == 0) {
        __cexit();
      }
      FUN_1011f89a();
      if (param_3 == 0) {
        __ioterm();
        __mtterm();
        FUN_101288b2();
      }
      FUN_1011e738();
    }
    else if (param_2 == 2) {
      iVar1 = FUN_101294ed(DAT_101954a0);
      if (iVar1 == 0) {
        _Memory = (DWORD *)FUN_10127857(1,0x3bc);
        if (_Memory != (DWORD *)0x0) {
          iVar1 = FUN_1012950c(DAT_101954a0,_Memory);
          if (iVar1 != 0) {
            FUN_10128c2b(_Memory,0);
            DVar3 = GetCurrentThreadId();
            *_Memory = DVar3;
            _Memory[1] = 0xffffffff;
            goto LAB_1011e7bd;
          }
          FID_conflict__free(_Memory);
        }
        goto LAB_1011e673;
      }
    }
    else if (param_2 == 3) {
      __freeptd((_ptiddata)0x0);
    }
LAB_1011e7bd:
    uVar2 = 1;
  }
  return uVar2;
}



/* 10002520 FUN_10002520 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined4 FUN_10002520(void)

{
  char cVar1;
  HWND pHVar2;
  HMODULE pHVar3;
  undefined4 uVar4;
  char *pcVar5;
  
  pHVar2 = FindWindowA("Cloud9",&DAT_10177ff0);
  pcVar5 = "GetModuleFileNameA";
  if (pHVar2 != (HWND)0x0) {
    DAT_101a0362 = 1;
  }
  pHVar3 = GetModuleHandleA("kernel32.dll");
  _DAT_101a03e0 = GetProcAddress(pHVar3,pcVar5);
  pHVar3 = LoadLibraryA("xp.dll");
  if (pHVar3 == (HMODULE)0x0) {
    AfxMessageBox("init xp error!",0,0);
    uVar4 = FUN_10004ce5();
    return uVar4;
  }
  DAT_101a03c0 = GetProcAddress(pHVar3,"NxCreatePhysicsSDK");
  DAT_101a03bc = GetProcAddress(pHVar3,"NxGetCookingLib");
  DAT_101a03b8 = GetProcAddress(pHVar3,"NxGetUtilLib");
  DAT_101a03b4 = GetProcAddress(pHVar3,"NxReleasePhysicsSDK");
  pcVar5 = "CreateProcessW";
  pHVar3 = GetModuleHandleA("kernel32.dll");
  _DAT_101a03c8 = GetProcAddress(pHVar3,pcVar5);
  pcVar5 = "ExitProcess";
  pHVar3 = GetModuleHandleA("kernel32.dll");
  _DAT_101a03cc = GetProcAddress(pHVar3,pcVar5);
  pcVar5 = "OpenProcess";
  pHVar3 = GetModuleHandleA("kernel32.dll");
  _DAT_101a03d0 = GetProcAddress(pHVar3,pcVar5);
  pcVar5 = "TerminateProcess";
  pHVar3 = GetModuleHandleA("kernel32.dll");
  _DAT_101a03d4 = GetProcAddress(pHVar3,pcVar5);
  pcVar5 = "NtOpenProcess";
  pHVar3 = LoadLibraryA("ntdll.dll");
  _DAT_101a03d8 = GetProcAddress(pHVar3,pcVar5);
  pcVar5 = "CreateToolhelp32Snapshot";
  pHVar3 = GetModuleHandleA("kernel32.dll");
  _DAT_101a03dc = GetProcAddress(pHVar3,pcVar5);
  cVar1 = FUN_10002760();
  if (cVar1 == '\0') {
    AfxMessageBox(&DAT_101780fc,0,0);
  }
  cVar1 = FUN_10001cd0();
  if (cVar1 == '\0') {
    AfxMessageBox(&DAT_101780fc,0,0);
  }
  if ((((DAT_101a03c0 == (FARPROC)0x0) || (DAT_101a03bc == (FARPROC)0x0)) ||
      (DAT_101a03b8 == (FARPROC)0x0)) || (DAT_101a03b4 == (FARPROC)0x0)) {
    AfxMessageBox(&DAT_1017810c,0,0);
    return 0;
  }
  MessageBoxW((HWND)0x0,
              L"该玩意是由BKS团队开发，支持任何Windows系统版本，福利版本不收费，祝您玩的愉快"
              ,L"提示",0x30);
  uVar4 = FUN_10004ce5();
  return uVar4;
}



/* 1011e7eb ___DllMainCRTStartup */

/* WARNING: Function: __SEH_prolog4 replaced with injection: SEH_prolog4 */
/* WARNING: Function: __SEH_epilog4 replaced with injection: EH_epilog3 */
/* Library Function - Single Match
    ___DllMainCRTStartup
   
   Library: Visual Studio 2012 Release */

uint ___DllMainCRTStartup(undefined4 param_1,int param_2,undefined4 param_3)

{
  int iVar1;
  uint uVar2;
  
  iVar1 = 1;
  if ((param_2 == 0) && (DAT_1019f448 == 0)) {
LAB_1011e8f1:
    uVar2 = 0;
  }
  else {
    if ((param_2 == 1) || (param_2 == 2)) {
      if (PTR_1017120c != (undefined *)0x0) {
        iVar1 = (*(code *)PTR_1017120c)(param_1,param_2,param_3);
      }
      if ((iVar1 == 0) || (iVar1 = FUN_1011e656(param_1,param_2,param_3), iVar1 == 0))
      goto LAB_1011e8f1;
    }
    uVar2 = _DllMain_12(param_1,param_2,param_3);
    if ((param_2 == 1) && (uVar2 == 0)) {
      _DllMain_12(param_1,0,param_3);
      FUN_1011e656(param_1,0,param_3);
      if (PTR_1017120c != (undefined *)0x0) {
        (*(code *)PTR_1017120c)(param_1,0,param_3);
      }
    }
    if ((param_2 == 0) || (param_2 == 3)) {
      iVar1 = FUN_1011e656(param_1,param_2,param_3);
      uVar2 = uVar2 & -(uint)(iVar1 != 0);
      if ((uVar2 != 0) && (PTR_1017120c != (undefined *)0x0)) {
        uVar2 = (*(code *)PTR_1017120c)(param_1,param_2,param_3);
      }
    }
  }
  return uVar2;
}



/* 1011e8f9 ___DllXcptFilter */

/* Library Function - Single Match
    ___DllXcptFilter
   
   Library: Visual Studio 2012 Release */

void ___DllXcptFilter(undefined4 param_1,int param_2,undefined4 param_3,ulong param_4,
                     _EXCEPTION_POINTERS *param_5)

{
  if (param_2 == 1) {
    FUN_1011e656(param_1,0,param_3);
  }
  ___CppXcptFilter(param_4,param_5);
  return;
}


