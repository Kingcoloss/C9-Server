/* SHA256 b2f23c8d00b50f0909f8cfbe059ec893deb2de266a3ab557301daa3c36cf8a25 */

/* 00564cc0 FUN_00564cc0 */
/* Callers:
 * Callees:
 */

void FUN_00564cc0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      *param_1 = *param_1 ^ 0xfd;
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      param_1[2] = param_1[2] ^ 0xc5;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 2;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 7;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xb];
      if ((char)((int)(uint)param_1[0xb] >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[0xb];
      param_1[0xb] = bVar1;
      param_1[5] = param_1[5] ^ 0xa8;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 2 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xbf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfb;
      }
      else {
        param_1[0xf] = param_1[0xf] | 4;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 6;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 2);
    }
    else {
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 5;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 7;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 1);
      param_1[2] = param_1[2] ^ 0x5f;
      param_1[0x17] = param_1[0x17] ^ 0xf8;
      bVar1 = param_1[5];
      param_1[5] = param_1[2];
      param_1[2] = bVar1;
      param_1[0x1b] = param_1[0x1b] ^ 8;
    }
  }
  return;
}



/* 00565310 FUN_00565310 */
/* Callers:
 * Callees:
 */

void FUN_00565310(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      *param_1 = *param_1 ^ 0xfd;
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x10) {
      param_1[2] = param_1[2] ^ 0xc5;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 2;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 2 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xbf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfb;
      }
      else {
        param_1[0xf] = param_1[0xf] | 4;
      }
      param_1[5] = param_1[5] ^ 0xa8;
      bVar1 = param_1[7];
      param_1[7] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0xb];
      if ((char)((int)(uint)param_1[0xb] >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 1;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 6;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 2);
    }
    else {
      param_1[0x1b] = param_1[0x1b] ^ 8;
      bVar1 = param_1[5];
      param_1[5] = param_1[2];
      param_1[2] = bVar1;
      param_1[0x17] = param_1[0x17] ^ 0xf8;
      param_1[2] = param_1[2] ^ 0x5f;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 1;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 3;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 5);
    }
  }
  return;
}



/* 00565960 FUN_00565960 */
/* Callers:
 * Callees:
 */

void FUN_00565960(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      *param_1 = *param_1 ^ 0xe7;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      param_1[2] = param_1[2] ^ 0x24;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
    }
    else if (param_2 < 0x10) {
      param_1[5] = param_1[5] ^ 0xa1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      param_1[3] = param_1[3] ^ 0x2d;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0xb] = param_1[0xb] ^ 0x4d;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      param_1[3] = param_1[3] ^ 0xce;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 1;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 6 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xbf;
      }
      else {
        param_1[9] = param_1[9] | 0x40;
      }
    }
    else {
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 5;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 3);
      param_1[0x1c] = param_1[0x1c] ^ 0xb2;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xbf;
      }
      else {
        param_1[9] = param_1[9] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x10] << 7;
      param_1[0x10] = param_1[0x10] | (byte)((int)(uint)bVar1 >> 1);
      param_1[0x1c] = param_1[0x1c] ^ 0xd;
    }
  }
  return;
}



/* 005660f0 FUN_005660f0 */
/* Callers:
 * Callees:
 */

void FUN_005660f0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      param_1[2] = param_1[2] ^ 0x24;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      *param_1 = *param_1 ^ 0xe7;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      param_1[3] = param_1[3] ^ 0x2d;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
      param_1[5] = param_1[5] ^ 0xa1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 6 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xbf;
      }
      else {
        param_1[9] = param_1[9] | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 7;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 1);
      param_1[3] = param_1[3] ^ 0xce;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0xb] = param_1[0xb] ^ 0x4d;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
    }
    else {
      param_1[0x1c] = param_1[0x1c] ^ 0xd;
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x10] << 1;
      param_1[0x10] = param_1[0x10] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xbf;
      }
      else {
        param_1[9] = param_1[9] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      param_1[0x1c] = param_1[0x1c] ^ 0xb2;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 3;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 5);
    }
  }
  return;
}



/* 00412290 FUN_00412290 */
/* Callers:
 * 00410b6c 00410ae0 FUN_00410ae0
 * 00410ba1 00410ae0 FUN_00410ae0
 * 00410bd6 00410ae0 FUN_00410ae0
 * 00410c0b 00410ae0 FUN_00410ae0
 * 00410c40 00410ae0 FUN_00410ae0
 * 00410c75 00410ae0 FUN_00410ae0
 * 00410caa 00410ae0 FUN_00410ae0
 * 00410cdf 00410ae0 FUN_00410ae0
 * 00410d14 00410ae0 FUN_00410ae0
 * 00410d49 00410ae0 FUN_00410ae0
 * 00410d7e 00410ae0 FUN_00410ae0
 * 00410db3 00410ae0 FUN_00410ae0
 * 00410de8 00410ae0 FUN_00410ae0
 * 00410e1d 00410ae0 FUN_00410ae0
 * 00410e52 00410ae0 FUN_00410ae0
 * 00410e87 00410ae0 FUN_00410ae0
 * 00410ebc 00410ae0 FUN_00410ae0
 * 00410ef1 00410ae0 FUN_00410ae0
 * 00410f26 00410ae0 FUN_00410ae0
 * 00410f5b 00410ae0 FUN_00410ae0
 * 00410f90 00410ae0 FUN_00410ae0
 * 00410fc5 00410ae0 FUN_00410ae0
 * 00410ffa 00410ae0 FUN_00410ae0
 * 0041102f 00410ae0 FUN_00410ae0
 * 00411064 00410ae0 FUN_00410ae0
 * 00411099 00410ae0 FUN_00410ae0
 * 004110ce 00410ae0 FUN_00410ae0
 * 00411103 00410ae0 FUN_00410ae0
 * 00411138 00410ae0 FUN_00410ae0
 * 0041116d 00410ae0 FUN_00410ae0
 * 004111a2 00410ae0 FUN_00410ae0
 * 004111d7 00410ae0 FUN_00410ae0
 * 0041120c 00410ae0 FUN_00410ae0
 * 00411241 00410ae0 FUN_00410ae0
 * 00411276 00410ae0 FUN_00410ae0
 * 004112ab 00410ae0 FUN_00410ae0
 * 004112e0 00410ae0 FUN_00410ae0
 * 00411315 00410ae0 FUN_00410ae0
 * 0041134a 00410ae0 FUN_00410ae0
 * 0041137f 00410ae0 FUN_00410ae0
 * 004113b4 00410ae0 FUN_00410ae0
 * 004113e9 00410ae0 FUN_00410ae0
 * 0041141e 00410ae0 FUN_00410ae0
 * 00411453 00410ae0 FUN_00410ae0
 * 00411488 00410ae0 FUN_00410ae0
 * 004114bd 00410ae0 FUN_00410ae0
 * 004114f2 00410ae0 FUN_00410ae0
 * 00411527 00410ae0 FUN_00410ae0
 * 0041155c 00410ae0 FUN_00410ae0
 * 00411591 00410ae0 FUN_00410ae0
 * 004115c6 00410ae0 FUN_00410ae0
 * 004115fb 00410ae0 FUN_00410ae0
 * 00411630 00410ae0 FUN_00410ae0
 * 00411665 00410ae0 FUN_00410ae0
 * 0041169a 00410ae0 FUN_00410ae0
 * 004116cf 00410ae0 FUN_00410ae0
 * 00411704 00410ae0 FUN_00410ae0
 * 00411739 00410ae0 FUN_00410ae0
 * 0041176e 00410ae0 FUN_00410ae0
 * 004117a3 00410ae0 FUN_00410ae0
 * 004117d8 00410ae0 FUN_00410ae0
 * 0041180d 00410ae0 FUN_00410ae0
 * 00411842 00410ae0 FUN_00410ae0
 * 00411877 00410ae0 FUN_00410ae0
 * 004118ac 00410ae0 FUN_00410ae0
 * 004118e1 00410ae0 FUN_00410ae0
 * 00411916 00410ae0 FUN_00410ae0
 * 0041194b 00410ae0 FUN_00410ae0
 * 00411980 00410ae0 FUN_00410ae0
 * 004119b5 00410ae0 FUN_00410ae0
 * 004119ea 00410ae0 FUN_00410ae0
 * 00411a1f 00410ae0 FUN_00410ae0
 * 00411a54 00410ae0 FUN_00410ae0
 * 00411a89 00410ae0 FUN_00410ae0
 * 00411abe 00410ae0 FUN_00410ae0
 * 00411af3 00410ae0 FUN_00410ae0
 * 00411b28 00410ae0 FUN_00410ae0
 * 00411b5d 00410ae0 FUN_00410ae0
 * 00411b92 00410ae0 FUN_00410ae0
 * 00411bc7 00410ae0 FUN_00410ae0
 * 00411bfc 00410ae0 FUN_00410ae0
 * 00411c31 00410ae0 FUN_00410ae0
 * 00411c66 00410ae0 FUN_00410ae0
 * 00411c9b 00410ae0 FUN_00410ae0
 * 00411cd0 00410ae0 FUN_00410ae0
 * 00411d05 00410ae0 FUN_00410ae0
 * 00411d3a 00410ae0 FUN_00410ae0
 * 00411d6f 00410ae0 FUN_00410ae0
 * 00411da4 00410ae0 FUN_00410ae0
 * 00411dd9 00410ae0 FUN_00410ae0
 * 00411e0e 00410ae0 FUN_00410ae0
 * 00411e43 00410ae0 FUN_00410ae0
 * 00411e78 00410ae0 FUN_00410ae0
 * 00411ead 00410ae0 FUN_00410ae0
 * 00411ee2 00410ae0 FUN_00410ae0
 * 00411f17 00410ae0 FUN_00410ae0
 * 00411f4c 00410ae0 FUN_00410ae0
 * 00411f81 00410ae0 FUN_00410ae0
 * 00411fb6 00410ae0 FUN_00410ae0
 * 00411feb 00410ae0 FUN_00410ae0
 * Callees:
 * 00412410 FUN_00412410
 * 00412b90 FUN_00412b90
 */

void __thiscall FUN_00412290(int param_1,undefined4 *param_2,int *param_3)

{
  undefined4 uVar1;
  bool bVar2;
  undefined4 *puVar3;
  undefined4 *puVar4;
  undefined4 auStack_c [2];
  
  puVar4 = *(undefined4 **)(param_1 + 4);
  bVar2 = true;
  auStack_c[0] = CONCAT31(auStack_c[0]._1_3_,1);
  if (*(char *)((int)puVar4[1] + 0x15) == '\0') {
    puVar3 = (undefined4 *)puVar4[1];
    do {
      puVar4 = puVar3;
      bVar2 = *param_3 < (int)puVar4[3];
      auStack_c[0] = CONCAT31(auStack_c[0]._1_3_,bVar2);
      if (bVar2) {
        puVar3 = (undefined4 *)*puVar4;
      }
      else {
        puVar3 = (undefined4 *)puVar4[2];
      }
    } while (*(char *)((int)puVar3 + 0x15) == '\0');
  }
  if (bVar2) {
    if (puVar4 == (undefined4 *)**(int **)(param_1 + 4)) {
      puVar4 = (undefined4 *)FUN_00412410(auStack_c,1,puVar4,param_3);
      uVar1 = *puVar4;
      *(undefined1 *)(param_2 + 1) = 1;
      *param_2 = uVar1;
      return;
    }
    FUN_00412b90();
  }
  if ((int)puVar4[3] < *param_3) {
    puVar4 = (undefined4 *)FUN_00412410(auStack_c,auStack_c[0],puVar4,param_3);
    *param_2 = *puVar4;
    *(undefined1 *)(param_2 + 1) = 1;
    return;
  }
  *(undefined1 *)(param_2 + 1) = 0;
  *param_2 = puVar4;
  return;
}



/* 00a64680 FUN_00a64680 */
/* Callers:
 * 00a62b8c 00a621f0 FUN_00a621f0
 * Callees:
 * 00a64870 FUN_00a64870
 * 008ff050 FUN_008ff050
 * 00745010 FUN_00745010
 * 00d39f00 FUN_00d39f00
 * 00a64960 FUN_00a64960
 */

void __thiscall FUN_00a64680(int param_1,undefined4 param_2)

{
  uint uVar1;
  uint uVar2;
  undefined4 uVar3;
  undefined4 *puVar4;
  undefined1 auStack_10 [4];
  undefined1 auStack_c [4];
  undefined4 uStack_8;
  
  uVar1 = FUN_00d39f00();
  uVar2 = FUN_008ff050();
  if (uVar1 < uVar2) {
    uVar3 = FUN_00a64960(*(undefined4 *)(param_1 + 8),1,param_2);
    *(undefined4 *)(param_1 + 8) = uVar3;
  }
  else {
    puVar4 = (undefined4 *)FUN_00745010(auStack_c);
    uStack_8 = *puVar4;
    FUN_00a64870(auStack_10,uStack_8,param_2);
  }
  return;
}



/* 00a655e0 FUN_00a655e0 */
/* Callers:
 * 00a65466 00a65430 FUN_00a65430
 * Callees:
 * 00614cd0 FUN_00614cd0
 * 00a6567c FUN_00a6567c
 */

/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_00a655e0(int param_1,int param_2,undefined4 param_3)

{
  void *pvStack_10;
  undefined *puStack_c;
  undefined4 uStack_8;
  
  puStack_c = &DAT_00ebcd60;
  pvStack_10 = ExceptionList;
  ExceptionList = &pvStack_10;
  uStack_8 = 0;
  for (; param_2 != 0; param_2 = param_2 + -1) {
    FUN_00614cd0(param_1,param_3);
    param_1 = param_1 + 0x34;
  }
  FUN_00a6567c();
  return;
}



/* 00a63630 FUN_00a63630 */
/* Callers:
 * 00a62de0 00a621f0 FUN_00a621f0
 * 00b1d7b8 00b1d770 FUN_00b1d770
 * Callees:
 * 008fdec0 FUN_008fdec0
 * 00d39f00 FUN_00d39f00
 */

undefined4 __thiscall FUN_00a63630(undefined4 param_1,int param_2)

{
  uint uVar1;
  int iVar2;
  undefined4 uVar3;
  uint uStack_8;
  
  uStack_8 = 0;
  while( true ) {
    uVar1 = FUN_00d39f00(param_1);
    if (uVar1 <= uStack_8) {
      return 0;
    }
    iVar2 = FUN_008fdec0(uStack_8);
    if (*(int *)(iVar2 + 1) == param_2) break;
    uStack_8 = uStack_8 + 1;
  }
  uVar3 = FUN_008fdec0(uStack_8);
  return uVar3;
}



/* 00dd1400 FUN_00dd1400 */
/* Callers:
 * Callees:
 * 00dd5d9c memset
 * 00dd5b90 FUN_00dd5b90
 * 00dd5f60 memcpy
 * 00dd5fb0 __alloca_probe
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00dd1400(int param_1,undefined4 param_2,uint param_3,int param_4)

{
  int iVar1;
  int iVar2;
  int iVar3;
  undefined1 auStack_20844 [64];
  undefined1 auStack_20804 [66560];
  undefined1 auStack_10404 [66564];
  
  iVar2 = 0;
  if ((param_3 & 7) != 0) {
    iVar2 = 8 - (param_3 & 7);
  }
  iVar1 = param_3 + 1 + iVar2;
  if (param_4 != 0) {
    memset(auStack_20844,0,0x40);
    memset(auStack_20804,0,iVar1);
    memset(auStack_10404,0,iVar1);
    memcpy(auStack_20804,param_2,param_3);
    memcpy(auStack_20804 + param_3,auStack_20844,iVar2);
    iVar3 = 0;
    if (0 < iVar1) {
      do {
        (**(code **)(*(int *)(param_1 + 0xc) + 0xc))(auStack_20804 + iVar3,0,auStack_10404 + iVar3);
        iVar3 = iVar3 + 8;
      } while (iVar3 < iVar1);
    }
    auStack_10404[iVar1 + -1] = (char)iVar2;
    memcpy(param_4,auStack_10404,iVar1);
  }
  FUN_00dd5b90();
  return;
}



/* 00dd3990 FUN_00dd3990 */
/* Callers:
 * Callees:
 * 00dd5d9c memset
 * 00dd5b90 FUN_00dd5b90
 * 00dd5f60 memcpy
 * 00dd5fb0 __alloca_probe
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00dd3990(int param_1,undefined4 param_2,uint param_3,int param_4)

{
  int iVar1;
  int iVar2;
  int iVar3;
  undefined1 auStack_20844 [64];
  undefined1 auStack_20804 [66560];
  undefined1 auStack_10404 [66564];
  
  iVar2 = 0;
  if ((param_3 & 0xf) != 0) {
    iVar2 = 0x10 - (param_3 & 0xf);
  }
  iVar1 = param_3 + 1 + iVar2;
  if (param_4 != 0) {
    memset(auStack_20844,0,0x40);
    memset(auStack_20804,0,iVar1);
    memset(auStack_10404,0,iVar1);
    memcpy(auStack_20804,param_2,param_3);
    memcpy(auStack_20804 + param_3,auStack_20844,iVar2);
    iVar3 = 0;
    if (0 < iVar1) {
      do {
        (**(code **)(*(int *)(param_1 + 0xc) + 0xc))(auStack_20804 + iVar3,0,auStack_10404 + iVar3);
        iVar3 = iVar3 + 0x10;
      } while (iVar3 < iVar1);
    }
    auStack_10404[iVar1 + -1] = (char)iVar2;
    memcpy(param_4,auStack_10404,iVar1);
  }
  FUN_00dd5b90();
  return;
}



/* 00dd2970 FUN_00dd2970 */
/* Callers:
 * Callees:
 * 00dd5d9c memset
 * 00dd5b90 FUN_00dd5b90
 * 00dd5f60 memcpy
 * 00dd5fb0 __alloca_probe
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00dd2970(int param_1,undefined4 param_2,uint param_3,int param_4)

{
  int iVar1;
  int iVar2;
  int iVar3;
  undefined1 auStack_20844 [64];
  undefined1 auStack_20804 [66560];
  undefined1 auStack_10404 [66564];
  
  iVar2 = 0;
  if (param_3 % 0xc != 0) {
    iVar2 = 0xc - param_3 % 0xc;
  }
  iVar1 = param_3 + 1 + iVar2;
  if (param_4 != 0) {
    memset(auStack_20844,0,0x40);
    memset(auStack_20804,0,iVar1);
    memset(auStack_10404,0,iVar1);
    memcpy(auStack_20804,param_2,param_3);
    memcpy(auStack_20804 + param_3,auStack_20844,iVar2);
    iVar3 = 0;
    if (0 < iVar1) {
      do {
        (**(code **)(*(int *)(param_1 + 0xc) + 0xc))(auStack_20804 + iVar3,0,auStack_10404 + iVar3);
        iVar3 = iVar3 + 0xc;
      } while (iVar3 < iVar1);
    }
    auStack_10404[iVar1 + -1] = (char)iVar2;
    memcpy(param_4,auStack_10404,iVar1);
  }
  FUN_00dd5b90();
  return;
}


/* No defined function at 00dd3730 */

/* 00dd3770 FUN_00dd3770 */
/* Callers:
 * Callees:
 * 00dd5d9c memset
 * 00dd5b90 FUN_00dd5b90
 * 00dd5f60 memcpy
 * 00dd5fb0 __alloca_probe
 */

/* WARNING: Function: __alloca_probe replaced with injection: alloca_probe */
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void __thiscall FUN_00dd3770(int param_1,int param_2,int param_3,int param_4)

{
  uint uVar1;
  int iVar2;
  int iVar3;
  undefined1 auStack_20804 [66560];
  undefined1 auStack_10404 [66564];
  
  if ((((((byte)param_3 & 7) == 1) && (uVar1 = (uint)*(byte *)(param_2 + -1 + param_3), uVar1 < 8))
      && (iVar3 = (param_3 - uVar1) + -1, -1 < iVar3)) && (param_4 != 0)) {
    memset(auStack_20804,0,param_3);
    memset(auStack_10404,0,param_3);
    memcpy(auStack_20804,param_2,param_3);
    iVar2 = 0;
    if (0 < iVar3) {
      do {
        (**(code **)(*(int *)(param_1 + 0x44) + 0xc))(auStack_20804 + iVar2,0,auStack_10404 + iVar2)
        ;
        iVar2 = iVar2 + 8;
      } while (iVar2 < iVar3);
    }
    memcpy(param_4,auStack_10404,iVar3);
  }
  FUN_00dd5b90();
  return;
}


