/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00a391c0 FUN_00a391c0 */
/* Callers:
 * Callees:
 */
/* Unavailable: Exception while decompiling 00a391c0: process: timeout
 */

/* 00a4da10 FUN_00a4da10 */
/* Callers:
 * Callees:
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_00a4da10(undefined4 param_1,int param_2)

{
  ushort uVar1;
  undefined1 uVar2;
  undefined4 uVar3;
  int iVar4;
  int *unaff_FS_OFFSET;
  undefined8 uVar5;
  undefined *puStack_cf0;
  undefined1 auStack_cd8 [564];
  undefined1 auStack_aa4 [20];
  undefined1 auStack_a90 [12];
  undefined4 uStack_a84;
  undefined1 auStack_930 [28];
  undefined1 auStack_914 [1700];
  uint uStack_270;
  uint uStack_26c;
  int iStack_114;
  undefined1 auStack_64 [28];
  int iStack_48;
  undefined1 auStack_44 [28];
  uint uStack_28;
  int iStack_24;
  int iStack_20;
  undefined4 uStack_1c;
  int iStack_18;
  undefined4 uStack_14;
  int iStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;

  uStack_8 = 0xffffffff;
  puStack_c = &DAT_00ebbc1e;
  iStack_10 = *unaff_FS_OFFSET;
  uStack_28 = _DAT_01302704 ^ (uint)(auStack_44 + 0x40);
  *unaff_FS_OFFSET = (int)&iStack_10;
  uVar1 = *(ushort *)(param_2 + 3);
  if (uVar1 < 0x36b7) {
    if (uVar1 == 0x36b6) {
      iStack_18 = param_2;
      func_0x009b6d90(uStack_28);
      uStack_14 = func_0x004fbff0();
      func_0x005f6d30(*(undefined4 *)(iStack_18 + 0xd),*(undefined4 *)(iStack_18 + 0x11),
                      *(undefined4 *)(iStack_18 + 0x15),*(undefined4 *)(iStack_18 + 9),1);
    }
    else if (uVar1 == 1) {
      iStack_48 = param_2;
      uVar3 = func_0x009d3dc0(*(undefined4 *)(param_2 + 0x17));
      __0__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAE_PB_W_Z(uVar3);
      uStack_8 = 0;
      __0__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAE_ABV01__Z(auStack_64);
      uStack_8._0_1_ = 1;
      if ((*(int *)(iStack_48 + 0x17) == _DAT_0191a124) ||
         (*(int *)(iStack_48 + 0x17) == _DAT_0191a128)) {
        func_0x00492e90();
        uStack_8._0_1_ = 2;
        uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ();
        func_0x00490f00(uVar3);
        func_0x00a43890(&UNK_01127120,*(undefined4 *)(iStack_48 + 0x1d));
        uVar3 = func_0x00492f70();
        __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
        uStack_8._0_1_ = 1;
        func_0x004909f0();
      }
      else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a12c) {
        iStack_114 = func_0x00676940(*(undefined4 *)(iStack_48 + 0x1d));
        if (iStack_114 != 0) {
          if (iStack_114 == 0) {
            puStack_cf0 = &UNK_0112712c;
          }
          else {
            puStack_cf0 = (undefined *)
                          _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ
                                    ();
          }
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x0045de30(auStack_44,&UNK_01127130,puStack_cf0,uVar3);
        }
      }
      else if (((*(int *)(iStack_48 + 0x17) == _DAT_0191a130) ||
               (*(int *)(iStack_48 + 0x17) == _DAT_0191a134)) ||
              (*(int *)(iStack_48 + 0x17) == _DAT_0191a138)) {
        uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ
                          (*(undefined4 *)(iStack_48 + 0x1d));
        func_0x0045de30(auStack_44,&UNK_01127144,uVar3);
      }
      else if (*(int *)(iStack_48 + 0x17) != _DAT_0191a13c) {
        if (*(int *)(iStack_48 + 0x17) == _DAT_0191a140) {
          func_0x00492e90();
          uStack_8._0_1_ = 3;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          func_0x00a43890(&UNK_01127154,*(undefined4 *)(iStack_48 + 0x1d));
          func_0x009c11b0(&UNK_01127164,5);
          uVar3 = func_0x00491170();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_ABV01__Z
                    (uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a144) {
          func_0x00a0bc90(1,*(undefined4 *)(iStack_48 + 0x21),*(undefined4 *)(iStack_48 + 0x25));
        }
        else if ((*(int *)(iStack_48 + 0x17) == _DAT_0191a148) ||
                (*(int *)(iStack_48 + 0x17) == _DAT_0191a14c)) {
          uStack_270 = *(uint *)(iStack_48 + 0x1d) % 0x3c;
          uStack_26c = *(uint *)(iStack_48 + 0x1d) / 0x3c;
          func_0x00492e90();
          uStack_8._0_1_ = 4;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          func_0x00a43890(&UNK_0112717c,uStack_26c);
          func_0x00a43890(&UNK_01127188,uStack_270);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a150) {
          func_0x00492e90();
          uStack_8._0_1_ = 5;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          uVar3 = func_0x0066b7d0(0x33);
          func_0x009c11b0(&UNK_01127190,uVar3);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a154) {
          func_0x00492e90();
          uStack_8._0_1_ = 6;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          uVar3 = func_0x0066b7d0(0x32);
          func_0x009c11b0(&UNK_0112719c,uVar3);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a158) {
          func_0x00492e90();
          uStack_8._0_1_ = 7;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          uVar3 = func_0x0066b7d0(0x32);
          func_0x009c11b0(&UNK_011271a8,uVar3);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a15c) {
          func_0x00492e90();
          uStack_8._0_1_ = 8;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          uVar3 = func_0x0066b7d0(0x39);
          func_0x009c11b0(&UNK_011271b4,uVar3);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a160) {
          func_0x00492e90();
          uStack_8._0_1_ = 9;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          uVar3 = func_0x0066b7d0(0x3a);
          func_0x009c11b0(&UNK_011271c0,uVar3);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a164) {
          func_0x00492e90();
          uStack_8._0_1_ = 10;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          func_0x009c11b0(&UNK_011271d8,4);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a168) {
          func_0x00492e90();
          uStack_8._0_1_ = 0xb;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          func_0x009c11b0(&UNK_011271f0,4);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a16c) {
          func_0x00492e90();
          uStack_8._0_1_ = 0xc;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          uVar3 = func_0x0066b7d0(0x3b);
          func_0x009c11b0(&UNK_01127208,uVar3);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a170) {
          func_0x00492e90();
          uStack_8._0_1_ = 0xd;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          uVar3 = func_0x0066b7d0(0x3c);
          func_0x009c11b0(&UNK_01127220,uVar3);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a174) {
          func_0x00492e90();
          uStack_8._0_1_ = 0xe;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          func_0x009b6d90();
          iVar4 = func_0x00c2f6f0();
          func_0x00a18450(&UNK_0112723c,*(undefined4 *)(iVar4 + 0x15));
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (((*(short *)(iStack_48 + 0x15) == 0xe8d) &&
                 (*(int *)(iStack_48 + 0x17) == _DAT_0191a178)) && (*(int *)(iStack_48 + 0x1d) == 0)
                ) {
          func_0x00609a90();
          uStack_8._0_1_ = 0xf;
          func_0x00a4e840(0);
          uVar2 = func_0x00938130(0);
          func_0x008b7830(auStack_914,uVar2);
          uStack_8._0_1_ = 1;
          func_0x00743270();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a17c) {
          func_0x00609ad0();
          uStack_8._0_1_ = 0x10;
          func_0x00984a80(0x7e,0,0,1);
          func_0x008b7830(auStack_930,0,0);
          uStack_8._0_1_ = 1;
          func_0x00743270();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a180) {
          func_0x00492e90();
          uStack_8._0_1_ = 0x11;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          func_0x00a43890(&UNK_01127270,*(undefined4 *)(iStack_48 + 0x1d));
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
        else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a184) {
          uStack_a84 = func_0x00a28700();
          func_0x00492e90();
          uStack_8._0_1_ = 0x12;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x00490f00(uVar3);
          uVar3 = func_0x0087f180();
          func_0x009c11b0(&UNK_01127284,uVar3);
          uVar3 = func_0x0052f9e0();
          func_0x009c11b0(&UNK_0112728c,uVar3);
          uVar3 = func_0x00492f70();
          __4__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAEAAV01_PB_W_Z(uVar3);
          uStack_8._0_1_ = 1;
          func_0x004909f0();
        }
      }
      if ((((*(int *)(iStack_48 + 0x17) == _DAT_0191a188) ||
           (*(int *)(iStack_48 + 0x17) == _DAT_0191a18c)) ||
          (*(int *)(iStack_48 + 0x17) == _DAT_0191a190)) ||
         ((*(int *)(iStack_48 + 0x17) == _DAT_0191a194 ||
          (*(int *)(iStack_48 + 0x17) == _DAT_0191a198)))) {
        func_0x009b6d90(1);
        func_0x00a4e820();
      }
      else if (*(int *)(iStack_48 + 0x17) == _DAT_0191a19c) {
        func_0x009b6d90();
        func_0x00a3c2f0();
        func_0x00609a10();
        uStack_8._0_1_ = 0x13;
        func_0x004a19a0(0xba);
        uVar2 = func_0x00938130(0);
        func_0x008b7830(auStack_a90,uVar2);
        uStack_8._0_1_ = 1;
        func_0x00743270();
      }
      if (((*(int *)(iStack_48 + 0x17) == _DAT_0191a1a0) ||
          (*(int *)(iStack_48 + 0x17) == _DAT_0191a1a4)) ||
         (*(int *)(iStack_48 + 0x17) == _DAT_0191a1a8)) {
        func_0x009b6d90();
        func_0x00aaa250();
      }
      if (*(short *)(iStack_48 + 0x15) == 0xea7) {
        func_0x00a4e7f0(0);
      }
      else if (*(short *)(iStack_48 + 0x15) == 0x275f) {
        func_0x00609a90();
        uStack_8._0_1_ = 0x14;
        func_0x00a4e870(0,0);
        func_0x008b7830(auStack_aa4,0,0);
        uStack_8._0_1_ = 1;
        func_0x00743270();
      }
      if (*(char *)(iStack_48 + 0x1b) == '\0') {
        if (*(char *)(iStack_48 + 0x1c) == '\0') {
          func_0x008aa0c0();
          func_0x0060b1f0();
          uStack_8._0_1_ = 0x15;
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ()
          ;
          func_0x009d39b0(auStack_cd8,uVar3);
          func_0x008b7830(auStack_cd8,0,0);
          uStack_8._0_1_ = 1;
          func_0x00743270();
        }
        else {
          iVar4 = func_0x008a9e90();
          if (iVar4 == 1) {
            func_0x008a9e60();
          }
          uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ
                            (2,0xfffffc18,0,0,0,0,0);
          func_0x008a9cf0(uVar3);
        }
      }
      else {
        uVar3 = _c_str___basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QBEPB_WXZ();
        func_0x009d3cb0(_DAT_01913b88,uVar3);
      }
      uStack_8 = (uint)uStack_8._1_3_ << 8;
      __1__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAE_XZ();
      uStack_8 = 0xffffffff;
      __1__basic_string__WU__char_traits__W_std__V__allocator__W_2__std__QAE_XZ();
    }
    else if ((uVar1 == 0xbd6) && (iStack_24 = param_2, _DAT_013522f4 != 0)) {
      uVar5 = func_0x009a7930();
      if ((*(int *)(iStack_24 + 0xd) != (int)uVar5) ||
         (*(int *)(iStack_24 + 0x11) != (int)((ulonglong)uVar5 >> 0x20))) {
        uVar2 = func_0x00938130(0,0xffffffff);
        func_0x00609700(iStack_24 + 0x19,uVar2);
      }
    }
  }
  else if (uVar1 == 0x36b7) {
    iStack_20 = param_2;
    func_0x009b6d90(uStack_28);
    uStack_1c = func_0x004fbff0();
    func_0x005f6fa0(*(undefined4 *)(iStack_20 + 0xd),*(undefined4 *)(iStack_20 + 0x11),
                    *(undefined4 *)(iStack_20 + 0x15),*(undefined4 *)(iStack_20 + 9),1);
  }
  *unaff_FS_OFFSET = iStack_10;
  func_0x00dd5b90();
  return;
}



/* 00d52ab0 FUN_00d52ab0 */
/* Callers:
 * Callees:
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00d52ab0(int param_1)

{
  undefined4 uVar1;

  uVar1 = func_0x0050e320(0,0,0,0,_DAT_01018bd4,1);
  func_0x009d4690(uVar1);
  *(undefined4 *)(param_1 + 0x22c) = 7;
  return;
}



/* 00da1350 FUN_00da1350 */
/* Callers:
 * Callees:
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __fastcall FUN_00da1350(int param_1)

{
  undefined4 uVar1;

  uVar1 = func_0x0050e320(0,0,0,0,_DAT_01018bd4,1);
  func_0x009d4690(uVar1);
  *(undefined4 *)(param_1 + 0x4c) = 1;
  return;
}
