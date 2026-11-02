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



/* 00566880 FUN_00566880 */
/* Callers:
 * Callees:
 */

void FUN_00566880(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      if (((int)(uint)(byte)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      uVar2 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = uVar2;
      param_1[3] = param_1[3] ^ 0xa2;
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
    }
    else if (param_2 < 0x10) {
      param_1[1] = param_1[1] ^ 0x1e;
      uVar2 = param_1[5];
      param_1[5] = param_1[7];
      param_1[7] = uVar2;
      uVar2 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = uVar2;
      param_1[7] = param_1[7] ^ 0xae;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = uVar2;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 3;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 5);
      param_1[9] = param_1[9] ^ 0x9c;
      bVar1 = param_1[6];
      if (((int)(uint)(byte)param_1[6] >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      bVar1 = param_1[0xe];
      if (((int)(uint)(byte)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      param_1[2] = param_1[2] ^ 0x49;
      bVar1 = param_1[7];
      if (((int)(uint)(byte)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      param_1[0xe] = param_1[0xe] ^ 0xce;
      param_1[8] = param_1[8] ^ 0x61;
      param_1[4] = param_1[4] ^ 0xa1;
      param_1[5] = param_1[5] ^ 0xfd;
      uVar2 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = uVar2;
      bVar1 = param_1[5];
      if (((int)(uint)(byte)param_1[5] >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      bVar1 = param_1[0xe];
      if (((int)(uint)(byte)param_1[0xe] >> 6 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xf7;
      }
      else {
        param_1[0xe] = param_1[0xe] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xbf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x40;
      }
    }
    else {
      param_1[0x14] = param_1[0x14] ^ 0x71;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 7;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 7;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 1);
      uVar2 = param_1[10];
      param_1[10] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[0x17];
      if (((int)(uint)(byte)param_1[0x17] >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xef;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      param_1[0x1b] = param_1[0x1b] ^ 0x21;
      param_1[3] = param_1[3] ^ 0xcb;
      param_1[1] = param_1[1] ^ 0x8e;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 6;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 2);
    }
  }
  return;
}



/* 00566fa0 FUN_00566fa0 */
/* Callers:
 * Callees:
 */

void FUN_00566fa0(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      param_1[3] = param_1[3] ^ 0xa2;
      uVar2 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = uVar2;
      bVar1 = param_1[3];
      if (((int)(uint)(byte)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
    }
    else if (param_2 < 0x10) {
      uVar2 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = uVar2;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
      param_1[7] = param_1[7] ^ 0xae;
      uVar2 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = uVar2;
      uVar2 = param_1[5];
      param_1[5] = param_1[7];
      param_1[7] = uVar2;
      param_1[1] = param_1[1] ^ 0x1e;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      if (((int)(uint)(byte)param_1[0xe] >> 6 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xf7;
      }
      else {
        param_1[0xe] = param_1[0xe] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xbf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x40;
      }
      bVar1 = param_1[5];
      if (((int)(uint)(byte)param_1[5] >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      uVar2 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = uVar2;
      param_1[5] = param_1[5] ^ 0xfd;
      param_1[4] = param_1[4] ^ 0xa1;
      param_1[8] = param_1[8] ^ 0x61;
      param_1[0xe] = param_1[0xe] ^ 0xce;
      bVar1 = param_1[7];
      if (((int)(uint)(byte)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      param_1[2] = param_1[2] ^ 0x49;
      bVar1 = param_1[0xe];
      if (((int)(uint)(byte)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      bVar1 = param_1[6];
      if (((int)(uint)(byte)param_1[6] >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      param_1[9] = param_1[9] ^ 0x9c;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 5;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 3);
    }
    else {
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 2;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 6);
      param_1[1] = param_1[1] ^ 0x8e;
      param_1[3] = param_1[3] ^ 0xcb;
      param_1[0x1b] = param_1[0x1b] ^ 0x21;
      bVar1 = param_1[0x17];
      if (((int)(uint)(byte)param_1[0x17] >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xef;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      uVar2 = param_1[10];
      param_1[10] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 1;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 1;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 7);
      param_1[0x14] = param_1[0x14] ^ 0x71;
    }
  }
  return;
}



/* 005676b0 FUN_005676b0 */
/* Callers:
 * Callees:
 */

void FUN_005676b0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
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
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      param_1[3] = param_1[3] ^ 0x53;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 6;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      param_1[0xc] = param_1[0xc] ^ 1;
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 1 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xf7;
      }
      else {
        param_1[9] = param_1[9] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfd;
      }
      else {
        param_1[9] = param_1[9] | 2;
      }
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[3];
      param_1[3] = bVar1;
    }
    else {
      bVar1 = param_1[0x10];
      if (((int)(uint)param_1[0x10] >> 4 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xfd;
      }
      else {
        param_1[0x10] = param_1[0x10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xef;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x10;
      }
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 7;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 5 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xef;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xdf;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x20;
      }
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      param_1[5] = param_1[5] ^ 0x12;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 2;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xdf;
      }
      else {
        param_1[8] = param_1[8] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
    }
  }
  return;
}



/* 00567e90 FUN_00567e90 */
/* Callers:
 * Callees:
 */

void FUN_00567e90(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      param_1[3] = param_1[3] ^ 0x53;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
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
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 1 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xf7;
      }
      else {
        param_1[9] = param_1[9] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfd;
      }
      else {
        param_1[9] = param_1[9] | 2;
      }
      param_1[0xc] = param_1[0xc] ^ 1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 2;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 6);
    }
    else {
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xdf;
      }
      else {
        param_1[8] = param_1[8] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 6;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 2);
      param_1[5] = param_1[5] ^ 0x12;
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 5 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xef;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xdf;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x20;
      }
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 1;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x10];
      if (((int)(uint)param_1[0x10] >> 4 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xfd;
      }
      else {
        param_1[0x10] = param_1[0x10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xef;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x10;
      }
    }
  }
  return;
}



/* 00568680 FUN_00568680 */
/* Callers:
 * Callees:
 */

void FUN_00568680(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      param_1[1] = param_1[1] ^ 0xd0;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      param_1[3] = param_1[3] ^ 0xcb;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      param_1[1] = param_1[1] ^ 0x7f;
      param_1[2] = param_1[2] ^ 0xa7;
    }
    else if (param_2 < 0x10) {
      param_1[1] = param_1[1] ^ 0xd4;
      param_1[3] = param_1[3] ^ 0x42;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 5;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 5 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xdf;
      }
      else {
        param_1[5] = param_1[5] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xdf;
      }
      else {
        param_1[5] = param_1[5] | 0x20;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0x7f;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 3;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 3;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[8];
      param_1[8] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0xe] = param_1[0xe] ^ 0xbe;
      param_1[4] = param_1[4] ^ 0xd3;
    }
    else {
      param_1[0x10] = param_1[0x10] ^ 0xdf;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 7;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      param_1[0x1d] = param_1[0x1d] ^ 0x92;
      bVar1 = param_1[7];
      param_1[7] = param_1[0x1e];
      param_1[0x1e] = bVar1;
    }
  }
  return;
}



/* 00568e90 FUN_00568e90 */
/* Callers:
 * Callees:
 */

void FUN_00568e90(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0xa7;
      param_1[1] = param_1[1] ^ 0x7f;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      param_1[3] = param_1[3] ^ 0xcb;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      param_1[1] = param_1[1] ^ 0xd0;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 5 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xdf;
      }
      else {
        param_1[5] = param_1[5] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xdf;
      }
      else {
        param_1[5] = param_1[5] | 0x20;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 3;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      param_1[3] = param_1[3] ^ 0x42;
      param_1[1] = param_1[1] ^ 0xd4;
    }
    else if (param_2 < 0x20) {
      param_1[4] = param_1[4] ^ 0xd3;
      param_1[0xe] = param_1[0xe] ^ 0xbe;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[8];
      param_1[8] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 5;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 5;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0x7f;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
    }
    else {
      bVar1 = param_1[7];
      param_1[7] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      param_1[0x1d] = param_1[0x1d] ^ 0x92;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 1;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      param_1[0x10] = param_1[0x10] ^ 0xdf;
    }
  }
  return;
}



/* 005696a0 FUN_005696a0 */
/* Callers:
 * Callees:
 */

void FUN_005696a0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      param_1[1] = param_1[1] ^ 0x3b;
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
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
    }
    else if (param_2 < 0x10) {
      param_1[7] = param_1[7] ^ 0x2e;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
    }
    else if (param_2 < 0x20) {
      param_1[0xe] = param_1[0xe] ^ 0xca;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 7;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 7;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
    }
    else {
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 2;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      param_1[1] = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 4 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xdf;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xef;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x10;
      }
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 5;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 1;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 3;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x13] << 3;
      param_1[0x13] = param_1[0x13] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x1d];
      if ((char)((int)(uint)param_1[0x1d] >> 7) == '\0') {
        param_1[0x1d] = param_1[0x1d] & 0xfb;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0x7f;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 0x80;
      }
      bVar1 = param_1[0x13];
      if ((char)((int)(uint)param_1[0x13] >> 7) == '\0') {
        param_1[0x13] = param_1[0x13] & 0xdf;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0x7f;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x80;
      }
    }
  }
  return;
}



/* 00569e00 FUN_00569e00 */
/* Callers:
 * Callees:
 */

void FUN_00569e00(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
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
      param_1[1] = param_1[1] ^ 0x3b;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      param_1[7] = param_1[7] ^ 0x2e;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 1;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 1;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 7);
      param_1[0xe] = param_1[0xe] ^ 0xca;
    }
    else {
      bVar1 = param_1[0x13];
      if ((char)((int)(uint)param_1[0x13] >> 7) == '\0') {
        param_1[0x13] = param_1[0x13] & 0xdf;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0x7f;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x80;
      }
      bVar1 = param_1[0x1d];
      if ((char)((int)(uint)param_1[0x1d] >> 7) == '\0') {
        param_1[0x1d] = param_1[0x1d] & 0xfb;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0x7f;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 0x80;
      }
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x13] << 5;
      param_1[0x13] = param_1[0x13] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 5;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 7;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 3;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 4 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xdf;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xef;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 6;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
    }
  }
  return;
}



/* 0056a550 FUN_0056a550 */
/* Callers:
 * Callees:
 */

void FUN_0056a550(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      param_1[2] = param_1[2] ^ 4;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 1;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[6];
      param_1[6] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 7;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 3;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 4;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 1;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 1;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 7);
      param_1[7] = param_1[7] ^ 0xe1;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 1;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      *param_1 = *param_1 ^ 0x87;
      bVar1 = param_1[0xc];
      param_1[0xc] = *param_1;
      *param_1 = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0x3b;
    }
    else {
      param_1[0x12] = param_1[0x12] ^ 4;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[0x16];
      if (((int)(uint)param_1[0x16] >> 4 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xef;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xef;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x10;
      }
      param_1[0x17] = param_1[0x17] ^ 0xd4;
      bVar1 = param_1[0x1f];
      if (((int)(uint)param_1[0x1f] >> 4 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xfd;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xef;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 0x10;
      }
      bVar1 = param_1[0x1f];
      if (((int)(uint)param_1[0x1f] >> 3 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xdf;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xf7;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 8;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 1 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfd;
      }
      else {
        param_1[9] = param_1[9] | 2;
      }
    }
  }
  return;
}



/* 0056ae30 FUN_0056ae30 */
/* Callers:
 * Callees:
 */

void FUN_0056ae30(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 4;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 5;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 1;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[6];
      param_1[6] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 7;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x20) {
      param_1[0xf] = param_1[0xf] ^ 0x3b;
      bVar1 = param_1[0xc];
      param_1[0xc] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0x87;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 7;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 1);
      param_1[7] = param_1[7] ^ 0xe1;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 7;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 7;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 4;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 4);
    }
    else {
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 1 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfd;
      }
      else {
        param_1[9] = param_1[9] | 2;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0x1f];
      if (((int)(uint)param_1[0x1f] >> 3 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xdf;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xf7;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 8;
      }
      bVar1 = param_1[0x1f];
      if (((int)(uint)param_1[0x1f] >> 4 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xfd;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xef;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 0x10;
      }
      param_1[0x17] = param_1[0x17] ^ 0xd4;
      bVar1 = param_1[0x16];
      if (((int)(uint)param_1[0x16] >> 4 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xef;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xef;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x10;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0x17];
      param_1[0x17] = bVar1;
      param_1[0x12] = param_1[0x12] ^ 4;
    }
  }
  return;
}



/* 0056b710 FUN_0056b710 */
/* Callers:
 * Callees:
 */

void FUN_0056b710(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = uVar2;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      param_1[1] = param_1[1] ^ 0xfe;
      uVar2 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = uVar2;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      uVar2 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = uVar2;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 7;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 1);
      uVar2 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = uVar2;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      uVar2 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = uVar2;
      bVar1 = param_1[2];
      if (((int)(uint)(byte)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      if (((int)(uint)(byte)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[3];
      if ((char)((int)(uint)(byte)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      param_1[0xd] = param_1[0xd] ^ 0xd3;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
    }
    else {
      uVar2 = param_1[0x1b];
      param_1[0x1b] = param_1[6];
      param_1[6] = uVar2;
      param_1[0x1c] = param_1[0x1c] ^ 0xce;
      bVar1 = param_1[0x12];
      if (((int)(uint)(byte)param_1[0x12] >> 3 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xf7;
      }
      else {
        param_1[0x12] = param_1[0x12] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xf7;
      }
      else {
        param_1[0x12] = param_1[0x12] | 8;
      }
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)(byte)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0xfb;
      }
      else {
        param_1[0xf] = param_1[0xf] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      uVar2 = param_1[0x10];
      param_1[0x10] = param_1[10];
      param_1[10] = uVar2;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 2;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 2;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0xb] = param_1[0xb] ^ 0xbf;
      param_1[0xe] = param_1[0xe] ^ 0x66;
    }
  }
  return;
}



/* 0056bd70 FUN_0056bd70 */
/* Callers:
 * Callees:
 */

void FUN_0056bd70(undefined1 *param_1,int param_2)

{
  undefined1 uVar1;
  byte bVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      uVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = uVar1;
      bVar2 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar2 >> 2);
      uVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = uVar1;
      param_1[1] = param_1[1] ^ 0xfe;
      bVar2 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar2 >> 7);
      uVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = uVar1;
      bVar2 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar2 >> 6);
    }
    else if (param_2 < 0x10) {
      bVar2 = param_1[3];
      if (((int)(uint)(byte)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar2 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      bVar2 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar2 >> 2);
      bVar2 = param_1[2];
      if (((int)(uint)(byte)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar2 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      uVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = uVar1;
      bVar2 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar2 >> 4);
      bVar2 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar2 >> 5);
      uVar1 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = uVar1;
      bVar2 = param_1[4];
      param_1[4] = param_1[4] << 1;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar2 >> 7);
    }
    else if (param_2 < 0x20) {
      bVar2 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar2 >> 5);
      param_1[0xd] = param_1[0xd] ^ 0xd3;
      bVar2 = param_1[3];
      if ((char)((int)(uint)(byte)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar2 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
    }
    else {
      param_1[0xe] = param_1[0xe] ^ 0x66;
      param_1[0xb] = param_1[0xb] ^ 0xbf;
      bVar2 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 6;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar2 >> 2);
      bVar2 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 6;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar2 >> 2);
      uVar1 = param_1[0x10];
      param_1[0x10] = param_1[10];
      param_1[10] = uVar1;
      bVar2 = param_1[0xf];
      if ((char)((int)(uint)(byte)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0xfb;
      }
      else {
        param_1[0xf] = param_1[0xf] | 4;
      }
      if (((int)(uint)bVar2 >> 2 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      bVar2 = param_1[0x12];
      if (((int)(uint)(byte)param_1[0x12] >> 3 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xf7;
      }
      else {
        param_1[0x12] = param_1[0x12] | 8;
      }
      if (((int)(uint)bVar2 >> 3 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xf7;
      }
      else {
        param_1[0x12] = param_1[0x12] | 8;
      }
      param_1[0x1c] = param_1[0x1c] ^ 0xce;
      uVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[6];
      param_1[6] = uVar1;
    }
  }
  return;
}



/* 0056c3c0 FUN_0056c3c0 */
/* Callers:
 * Callees:
 */

void FUN_0056c3c0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      param_1[1] = param_1[1] ^ 0x74;
      param_1[2] = param_1[2] ^ 0xcb;
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 0x67;
      *param_1 = *param_1 ^ 0x5f;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      param_1[4] = param_1[4] ^ 0x12;
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[7];
      param_1[7] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[10];
      param_1[10] = bVar1;
      *param_1 = *param_1 ^ 0xbf;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[0xe];
      param_1[0xe] = bVar1;
      param_1[1] = param_1[1] ^ 0xc1;
      bVar1 = param_1[2];
      param_1[2] = param_1[7];
      param_1[7] = bVar1;
      param_1[0xd] = param_1[0xd] ^ 0xe;
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
    }
    else {
      bVar1 = param_1[0x15];
      if (((int)(uint)param_1[0x15] >> 6 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0xfb;
      }
      else {
        param_1[0x15] = param_1[0x15] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0xbf;
      }
      else {
        param_1[0x15] = param_1[0x15] | 0x40;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 3;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 1;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 7);
      param_1[0xe] = param_1[0xe] ^ 0xd7;
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x10] << 5;
      param_1[0x10] = param_1[0x10] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      param_1[0xb] = param_1[0xb] ^ 0xe;
      bVar1 = param_1[0x1e];
      if (((int)(uint)param_1[0x1e] >> 5 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfd;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xdf;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = param_1[0x19];
      param_1[0x19] = bVar1;
      param_1[0x15] = param_1[0x15] ^ 0xe0;
      bVar1 = param_1[0x16];
      if (((int)(uint)param_1[0x16] >> 4 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xef;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xef;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x10;
      }
    }
  }
  return;
}



/* 0056cb30 FUN_0056cb30 */
/* Callers:
 * Callees:
 */

void FUN_0056cb30(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0x5f;
      param_1[3] = param_1[3] ^ 0x67;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      param_1[2] = param_1[2] ^ 0xcb;
      param_1[1] = param_1[1] ^ 0x74;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
      param_1[4] = param_1[4] ^ 0x12;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      param_1[0xd] = param_1[0xd] ^ 0xe;
      bVar1 = param_1[2];
      param_1[2] = param_1[7];
      param_1[7] = bVar1;
      param_1[1] = param_1[1] ^ 0xc1;
      bVar1 = param_1[7];
      param_1[7] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      *param_1 = *param_1 ^ 0xbf;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[7];
      param_1[7] = bVar1;
    }
    else {
      bVar1 = param_1[0x16];
      if (((int)(uint)param_1[0x16] >> 4 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xef;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xef;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x10;
      }
      param_1[0x15] = param_1[0x15] ^ 0xe0;
      bVar1 = *param_1;
      *param_1 = param_1[0x19];
      param_1[0x19] = bVar1;
      bVar1 = param_1[0x1e];
      if (((int)(uint)param_1[0x1e] >> 5 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfd;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xdf;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 0x20;
      }
      param_1[0xb] = param_1[0xb] ^ 0xe;
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x10] << 3;
      param_1[0x10] = param_1[0x10] | (byte)((int)(uint)bVar1 >> 5);
      param_1[0xe] = param_1[0xe] ^ 0xd7;
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 7;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 5;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x15];
      if (((int)(uint)param_1[0x15] >> 6 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0xfb;
      }
      else {
        param_1[0x15] = param_1[0x15] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0xbf;
      }
      else {
        param_1[0x15] = param_1[0x15] | 0x40;
      }
    }
  }
  return;
}



/* 0056d290 FUN_0056d290 */
/* Callers:
 * Callees:
 */

void FUN_0056d290(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 0xcd;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
    }
    else if (param_2 < 0x10) {
      param_1[7] = param_1[7] ^ 0x51;
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 1;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 7);
      param_1[5] = param_1[5] ^ 0x8b;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xf7;
      }
      else {
        param_1[0xc] = param_1[0xc] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 1;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0x8b;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[0xc];
      param_1[0xc] = bVar1;
    }
    else {
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 3 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfb;
      }
      else {
        param_1[0xc] = param_1[0xc] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xf7;
      }
      else {
        param_1[0xc] = param_1[0xc] | 8;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[0x12];
      param_1[0x12] = bVar1;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfb;
      }
      else {
        param_1[0xc] = param_1[0xc] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      bVar1 = param_1[0x15];
      if (((int)(uint)param_1[0x15] >> 4 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0xf7;
      }
      else {
        param_1[0x15] = param_1[0x15] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0xef;
      }
      else {
        param_1[0x15] = param_1[0x15] | 0x10;
      }
      *param_1 = *param_1 ^ 0x82;
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 1 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfd;
      }
      else {
        param_1[0x17] = param_1[0x17] | 2;
      }
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x13];
      param_1[0x13] = bVar1;
      param_1[0x11] = param_1[0x11] ^ 0xb;
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 3;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      param_1[4] = param_1[4] ^ 0x35;
    }
  }
  return;
}



/* 0056d950 FUN_0056d950 */
/* Callers:
 * Callees:
 */

void FUN_0056d950(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      param_1[1] = param_1[1] ^ 0xcd;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      param_1[5] = param_1[5] ^ 0x8b;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 7;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      param_1[7] = param_1[7] ^ 0x51;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      param_1[2] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0x8b;
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 7;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xf7;
      }
      else {
        param_1[0xc] = param_1[0xc] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
    }
    else {
      param_1[4] = param_1[4] ^ 0x35;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 5;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 3);
      param_1[0x11] = param_1[0x11] ^ 0xb;
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x13];
      param_1[0x13] = bVar1;
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 1 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfd;
      }
      else {
        param_1[0x17] = param_1[0x17] | 2;
      }
      *param_1 = *param_1 ^ 0x82;
      bVar1 = param_1[0x15];
      if (((int)(uint)param_1[0x15] >> 4 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0xf7;
      }
      else {
        param_1[0x15] = param_1[0x15] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0xef;
      }
      else {
        param_1[0x15] = param_1[0x15] | 0x10;
      }
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfb;
      }
      else {
        param_1[0xc] = param_1[0xc] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[0x12];
      param_1[0x12] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 3 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfb;
      }
      else {
        param_1[0xc] = param_1[0xc] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xf7;
      }
      else {
        param_1[0xc] = param_1[0xc] | 8;
      }
    }
  }
  return;
}



/* 0056e010 FUN_0056e010 */
/* Callers:
 * Callees:
 */

void FUN_0056e010(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      param_1[3] = param_1[3] ^ 0xf9;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 0x2f;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      param_1[4] = param_1[4] ^ 0xb3;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 4 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xbf;
      }
      else {
        param_1[9] = param_1[9] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xef;
      }
      else {
        param_1[9] = param_1[9] | 0x10;
      }
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 4 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xdf;
      }
      else {
        param_1[9] = param_1[9] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xef;
      }
      else {
        param_1[9] = param_1[9] | 0x10;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0xf] = param_1[0xf] ^ 0xdd;
      param_1[0xc] = param_1[0xc] ^ 0xe;
      param_1[10] = param_1[10] ^ 0xb6;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 7;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 1);
    }
    else {
      param_1[0x12] = param_1[0x12] ^ 0xa6;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      param_1[0x1a] = param_1[0x1a] ^ 0xb1;
      param_1[5] = param_1[5] ^ 0x45;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 4;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 1;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 7);
      param_1[6] = param_1[6] ^ 0xd1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 7;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 4;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 4);
      param_1[0x14] = param_1[0x14] ^ 0xc4;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      *param_1 = *param_1 ^ 0xa6;
    }
  }
  return;
}



/* 0056e760 FUN_0056e760 */
/* Callers:
 * Callees:
 */

void FUN_0056e760(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      param_1[1] = param_1[1] ^ 0x2f;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 0xf9;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      param_1[4] = param_1[4] ^ 0xb3;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 1;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[1];
      param_1[1] = bVar1;
      param_1[10] = param_1[10] ^ 0xb6;
      param_1[0xc] = param_1[0xc] ^ 0xe;
      param_1[0xf] = param_1[0xf] ^ 0xdd;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 4 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xdf;
      }
      else {
        param_1[9] = param_1[9] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xef;
      }
      else {
        param_1[9] = param_1[9] | 0x10;
      }
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 4 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xbf;
      }
      else {
        param_1[9] = param_1[9] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xef;
      }
      else {
        param_1[9] = param_1[9] | 0x10;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[10];
      param_1[10] = bVar1;
    }
    else {
      *param_1 = *param_1 ^ 0xa6;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      param_1[0x14] = param_1[0x14] ^ 0xc4;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 4;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 1;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 7);
      param_1[6] = param_1[6] ^ 0xd1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 7;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 4;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 4);
      param_1[5] = param_1[5] ^ 0x45;
      param_1[0x1a] = param_1[0x1a] ^ 0xb1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      param_1[0x12] = param_1[0x12] ^ 0xa6;
    }
  }
  return;
}



/* 0056eeb0 FUN_0056eeb0 */
/* Callers:
 * Callees:
 */

void FUN_0056eeb0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0x6e;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      param_1[2] = param_1[2] ^ 0x23;
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      param_1[2] = param_1[2] ^ 0x69;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      *param_1 = *param_1 ^ 0xe4;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      param_1[1] = param_1[1] ^ 0xa3;
      param_1[7] = param_1[7] ^ 0xf9;
    }
    else if (param_2 < 0x20) {
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      param_1[0xc] = param_1[0xc] ^ 0x9f;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
    }
    else {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 2;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 5 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xfd;
      }
      else {
        param_1[0x19] = param_1[0x19] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xdf;
      }
      else {
        param_1[0x19] = param_1[0x19] | 0x20;
      }
      param_1[5] = param_1[5] ^ 0x13;
      bVar1 = param_1[7];
      param_1[7] = param_1[0x10];
      param_1[0x10] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 5 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xdf;
      }
      else {
        param_1[4] = param_1[4] | 0x20;
      }
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 2;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 6);
    }
  }
  return;
}



/* 0056f650 FUN_0056f650 */
/* Callers:
 * Callees:
 */

void FUN_0056f650(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0x69;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      param_1[2] = param_1[2] ^ 0x23;
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      param_1[3] = param_1[3] ^ 0x6e;
    }
    else if (param_2 < 0x10) {
      param_1[7] = param_1[7] ^ 0xf9;
      param_1[1] = param_1[1] ^ 0xa3;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      *param_1 = *param_1 ^ 0xe4;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      param_1[0xc] = param_1[0xc] ^ 0x9f;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
    }
    else {
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 6;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 5 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xdf;
      }
      else {
        param_1[4] = param_1[4] | 0x20;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[0x10];
      param_1[0x10] = bVar1;
      param_1[5] = param_1[5] ^ 0x13;
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 5 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xfd;
      }
      else {
        param_1[0x19] = param_1[0x19] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xdf;
      }
      else {
        param_1[0x19] = param_1[0x19] | 0x20;
      }
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 6;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
    }
  }
  return;
}



/* 0056fdf0 FUN_0056fdf0 */
/* Callers:
 * Callees:
 */

void FUN_0056fdf0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[3] = param_1[3] ^ 0x7c;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      param_1[4] = param_1[4] ^ 0xfd;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 0xdb;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 3;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 7;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 1);
    }
    else {
      param_1[0xb] = param_1[0xb] ^ 0x97;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x10] << 5;
      param_1[0x10] = param_1[0x10] | (byte)((int)(uint)bVar1 >> 3);
      param_1[0x1c] = param_1[0x1c] ^ 0xd6;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0xe] = param_1[0xe] ^ 0x6f;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 3;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 5);
    }
  }
  return;
}



/* 005704d0 FUN_005704d0 */
/* Callers:
 * Callees:
 */

void FUN_005704d0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0x7c;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 5;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      param_1[2] = param_1[2] ^ 0xdb;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      param_1[4] = param_1[4] ^ 0xfd;
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 1;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
    }
    else {
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 5;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 3);
      param_1[0xe] = param_1[0xe] ^ 0x6f;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0x1c] = param_1[0x1c] ^ 0xd6;
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x10] << 3;
      param_1[0x10] = param_1[0x10] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0xe];
      param_1[0xe] = bVar1;
      param_1[0xb] = param_1[0xb] ^ 0x97;
    }
  }
  return;
}



/* 00570bb0 FUN_00570bb0 */
/* Callers:
 * Callees:
 */

void FUN_00570bb0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      param_1[2] = param_1[2] ^ 0xf7;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      param_1[5] = param_1[5] ^ 0x8e;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x20) {
      param_1[0xb] = param_1[0xb] ^ 0x75;
      bVar1 = param_1[10];
      param_1[10] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 3 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xf7;
      }
      else {
        param_1[8] = param_1[8] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xf7;
      }
      else {
        param_1[8] = param_1[8] | 8;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[5];
      param_1[5] = bVar1;
      *param_1 = *param_1 ^ 0x6b;
    }
    else {
      bVar1 = param_1[5];
      param_1[5] = param_1[0x1f];
      param_1[0x1f] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[0xc];
      param_1[0xc] = bVar1;
      *param_1 = *param_1 ^ 0xef;
      param_1[0x1d] = param_1[0x1d] ^ 0xaa;
    }
  }
  return;
}



/* 00571050 FUN_00571050 */
/* Callers:
 * Callees:
 */

void FUN_00571050(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      param_1[2] = param_1[2] ^ 0xf7;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      param_1[3] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      param_1[5] = param_1[5] ^ 0x8e;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
    }
    else if (param_2 < 0x20) {
      *param_1 = *param_1 ^ 0x6b;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 3 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xf7;
      }
      else {
        param_1[8] = param_1[8] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xf7;
      }
      else {
        param_1[8] = param_1[8] | 8;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[4];
      param_1[4] = bVar1;
      param_1[0xb] = param_1[0xb] ^ 0x75;
    }
    else {
      param_1[0x1d] = param_1[0x1d] ^ 0xaa;
      *param_1 = *param_1 ^ 0xef;
      bVar1 = param_1[9];
      param_1[9] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[0x1f];
      param_1[0x1f] = bVar1;
    }
  }
  return;
}



/* 005714f0 FUN_005714f0 */
/* Callers:
 * Callees:
 */

void FUN_005714f0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      *param_1 = *param_1 ^ 0xf7;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      *param_1 = *param_1 ^ 0xd5;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[8];
      param_1[8] = bVar1;
      param_1[0xd] = param_1[0xd] ^ 0x1a;
      param_1[0xf] = param_1[0xf] ^ 0xc0;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 4;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 4);
    }
    else {
      bVar1 = param_1[9];
      param_1[9] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[8];
      param_1[8] = bVar1;
      param_1[0x10] = param_1[0x10] ^ 0xd9;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 2;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 6);
    }
  }
  return;
}



/* 00571ab0 FUN_00571ab0 */
/* Callers:
 * Callees:
 */

void FUN_00571ab0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      *param_1 = *param_1 ^ 0xf7;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[5];
      param_1[5] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[4];
      param_1[4] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 4;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 4);
      param_1[0xf] = param_1[0xf] ^ 0xc0;
      param_1[0xd] = param_1[0xd] ^ 0x1a;
      bVar1 = param_1[2];
      param_1[2] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      *param_1 = *param_1 ^ 0xd5;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
    }
    else {
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 6;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1f];
      param_1[0x1f] = *param_1;
      *param_1 = bVar1;
      param_1[0x10] = param_1[0x10] ^ 0xd9;
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[0x18];
      param_1[0x18] = bVar1;
    }
  }
  return;
}



/* 00572070 FUN_00572070 */
/* Callers:
 * Callees:
 */

void FUN_00572070(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      param_1[4] = param_1[4] ^ 0x7e;
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      param_1[0xb] = param_1[0xb] ^ 0x7b;
      param_1[0xc] = param_1[0xc] ^ 0xba;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[9];
      param_1[9] = param_1[2];
      param_1[2] = bVar1;
      param_1[9] = param_1[9] ^ 4;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 6 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xbf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x40;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      param_1[7] = param_1[7] ^ 0x45;
      bVar1 = param_1[10];
      param_1[10] = param_1[3];
      param_1[3] = bVar1;
    }
    else {
      bVar1 = param_1[4];
      param_1[4] = param_1[0x14];
      param_1[0x14] = bVar1;
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 2 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfb;
      }
      else {
        param_1[0x18] = param_1[0x18] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfb;
      }
      else {
        param_1[0x18] = param_1[0x18] | 4;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[0x12];
      param_1[0x12] = bVar1;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 3 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xef;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xf7;
      }
      else {
        param_1[0x17] = param_1[0x17] | 8;
      }
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 3 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xef;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xf7;
      }
      else {
        param_1[0x18] = param_1[0x18] | 8;
      }
      param_1[5] = param_1[5] ^ 0x22;
      param_1[0x19] = param_1[0x19] ^ 0x6b;
    }
  }
  return;
}



/* 00572800 FUN_00572800 */
/* Callers:
 * Callees:
 */

void FUN_00572800(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      param_1[4] = param_1[4] ^ 0x7e;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[10];
      param_1[10] = param_1[3];
      param_1[3] = bVar1;
      param_1[7] = param_1[7] ^ 0x45;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 6 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xbf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x40;
      }
      param_1[9] = param_1[9] ^ 4;
      bVar1 = param_1[9];
      param_1[9] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      param_1[0xc] = param_1[0xc] ^ 0xba;
      param_1[0xb] = param_1[0xb] ^ 0x7b;
    }
    else {
      param_1[0x19] = param_1[0x19] ^ 0x6b;
      param_1[5] = param_1[5] ^ 0x22;
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 3 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xef;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xf7;
      }
      else {
        param_1[0x18] = param_1[0x18] | 8;
      }
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 3 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xef;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xf7;
      }
      else {
        param_1[0x17] = param_1[0x17] | 8;
      }
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      bVar1 = param_1[0x1e];
      param_1[0x1e] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[0x12];
      param_1[0x12] = bVar1;
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 2 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfb;
      }
      else {
        param_1[0x18] = param_1[0x18] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfb;
      }
      else {
        param_1[0x18] = param_1[0x18] | 4;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[0x14];
      param_1[0x14] = bVar1;
    }
  }
  return;
}



/* 00572f90 FUN_00572f90 */
/* Callers:
 * Callees:
 */

void FUN_00572f90(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = uVar2;
      bVar1 = param_1[2];
      if (((int)(uint)(byte)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      param_1[3] = param_1[3] ^ 0x1d;
      uVar2 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = uVar2;
      uVar2 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 4;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 4);
      param_1[2] = param_1[2] ^ 0xd0;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      param_1[2] = param_1[2] ^ 0xfe;
      uVar2 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = uVar2;
      uVar2 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = uVar2;
      param_1[7] = param_1[7] ^ 0xac;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 5;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 3);
      uVar2 = param_1[3];
      param_1[3] = param_1[0xd];
      param_1[0xd] = uVar2;
      param_1[3] = param_1[3] ^ 0x1c;
      uVar2 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = uVar2;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 7;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 1);
      param_1[0xf] = param_1[0xf] ^ 0xd7;
      param_1[0xb] = param_1[0xb] ^ 0x70;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
    }
    else {
      param_1[0x19] = param_1[0x19] ^ 0xe8;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      uVar2 = param_1[0xc];
      param_1[0xc] = param_1[0xe];
      param_1[0xe] = uVar2;
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      param_1[0x1e] = param_1[0x1e] ^ 0x7a;
      param_1[0xc] = param_1[0xc] ^ 0xc3;
    }
  }
  return;
}



/* 00573530 FUN_00573530 */
/* Callers:
 * Callees:
 */

void FUN_00573530(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      uVar2 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = uVar2;
      uVar2 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = uVar2;
      param_1[3] = param_1[3] ^ 0x1d;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      if (((int)(uint)(byte)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      uVar2 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = uVar2;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      param_1[7] = param_1[7] ^ 0xac;
      uVar2 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = uVar2;
      uVar2 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = uVar2;
      param_1[2] = param_1[2] ^ 0xfe;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      param_1[2] = param_1[2] ^ 0xd0;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 4;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      param_1[0xb] = param_1[0xb] ^ 0x70;
      param_1[0xf] = param_1[0xf] ^ 0xd7;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 1;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 7);
      uVar2 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = uVar2;
      param_1[3] = param_1[3] ^ 0x1c;
      uVar2 = param_1[3];
      param_1[3] = param_1[0xd];
      param_1[0xd] = uVar2;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 3;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
    }
    else {
      param_1[0xc] = param_1[0xc] ^ 0xc3;
      param_1[0x1e] = param_1[0x1e] ^ 0x7a;
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      uVar2 = param_1[0xc];
      param_1[0xc] = param_1[0xe];
      param_1[0xe] = uVar2;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      param_1[0x19] = param_1[0x19] ^ 0xe8;
    }
  }
  return;
}



/* 00573ad0 FUN_00573ad0 */
/* Callers:
 * Callees:
 */

void FUN_00573ad0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0x55;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 4;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      param_1[1] = param_1[1] ^ 0xc;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 1;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0x7f;
      }
      else {
        param_1[8] = param_1[8] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
    }
    else {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 7;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0x1a];
      param_1[0x1a] = bVar1;
      param_1[0x12] = param_1[0x12] ^ 0x8b;
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfb;
      }
      else {
        param_1[0x12] = param_1[0x12] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x1c];
      param_1[0x1c] = bVar1;
    }
  }
  return;
}



/* 00574050 FUN_00574050 */
/* Callers:
 * Callees:
 */

void FUN_00574050(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[2] = param_1[2] ^ 0x55;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 4;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0x7f;
      }
      else {
        param_1[8] = param_1[8] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 7;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 1);
      param_1[1] = param_1[1] ^ 0xc;
    }
    else {
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfb;
      }
      else {
        param_1[0x12] = param_1[0x12] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      param_1[0x12] = param_1[0x12] ^ 0x8b;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0x1a];
      param_1[0x1a] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 1;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 7);
    }
  }
  return;
}



/* 005745d0 FUN_005745d0 */
/* Callers:
 * Callees:
 */

void FUN_005745d0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      param_1[5] = param_1[5] ^ 0x40;
      bVar1 = param_1[5];
      param_1[5] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xdf;
      }
      else {
        param_1[6] = param_1[6] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
    }
    else if (param_2 < 0x20) {
      param_1[2] = param_1[2] ^ 0xe8;
      bVar1 = param_1[4];
      param_1[4] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 1;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 2;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 6 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xef;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xbf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x40;
      }
      param_1[9] = param_1[9] ^ 0x68;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
    }
    else {
      param_1[0x15] = param_1[0x15] ^ 0xd0;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x15];
      param_1[0x15] = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0x2a;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 6;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[8];
      param_1[8] = *param_1;
      *param_1 = bVar1;
      param_1[5] = param_1[5] ^ 0x7e;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 5;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 3);
      param_1[2] = param_1[2] ^ 0x37;
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 6 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xbf;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x40;
      }
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 5 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xdf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x20;
      }
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 7;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[5];
      param_1[5] = bVar1;
    }
  }
  return;
}



/* 00574e30 FUN_00574e30 */
/* Callers:
 * Callees:
 */

void FUN_00574e30(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xdf;
      }
      else {
        param_1[6] = param_1[6] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[6];
      param_1[6] = bVar1;
      param_1[5] = param_1[5] ^ 0x40;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      param_1[9] = param_1[9] ^ 0x68;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 6 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xef;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xbf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x40;
      }
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 6;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 7;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[10];
      param_1[10] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[1];
      param_1[1] = bVar1;
      param_1[2] = param_1[2] ^ 0xe8;
    }
    else {
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 1;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 5 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xdf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x20;
      }
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 6 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xbf;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x40;
      }
      param_1[2] = param_1[2] ^ 0x37;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 3;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 5);
      param_1[5] = param_1[5] ^ 0x7e;
      bVar1 = param_1[8];
      param_1[8] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 2;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0xf] = param_1[0xf] ^ 0x2a;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x15];
      param_1[0x15] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      param_1[0x15] = param_1[0x15] ^ 0xd0;
    }
  }
  return;
}



/* 00575690 FUN_00575690 */
/* Callers:
 * Callees:
 */

void FUN_00575690(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0x87;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      param_1[6] = param_1[6] ^ 0xd8;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 0x95;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      param_1[7] = param_1[7] ^ 0x52;
      param_1[0xe] = param_1[0xe] ^ 6;
      param_1[0xe] = param_1[0xe] ^ 0xa4;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[4];
      param_1[4] = bVar1;
    }
    else {
      bVar1 = *param_1;
      *param_1 = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 4;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      param_1[3] = param_1[7];
      param_1[7] = bVar1;
    }
  }
  return;
}



/* 00575b40 FUN_00575b40 */
/* Callers:
 * Callees:
 */

void FUN_00575b40(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 0x87;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      param_1[6] = param_1[6] ^ 0xd8;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[9];
      param_1[9] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[0xe] = param_1[0xe] ^ 0xa4;
      param_1[0xe] = param_1[0xe] ^ 6;
      param_1[7] = param_1[7] ^ 0x52;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      param_1[1] = param_1[1] ^ 0x95;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
    }
    else {
      bVar1 = param_1[3];
      param_1[3] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 4;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      *param_1 = param_1[0x11];
      param_1[0x11] = bVar1;
    }
  }
  return;
}



/* 00575ff0 FUN_00575ff0 */
/* Callers:
 * Callees:
 */

void FUN_00575ff0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      param_1[8] = param_1[8] ^ 10;
      param_1[0xb] = param_1[0xb] ^ 8;
      bVar1 = param_1[3];
      param_1[3] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[8];
      param_1[8] = bVar1;
      param_1[0xe] = param_1[0xe] ^ 0xdd;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      param_1[0xf] = param_1[0xf] ^ 0x69;
    }
    else {
      bVar1 = param_1[9];
      param_1[9] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 4;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 3;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 5);
    }
  }
  return;
}



/* 00576690 FUN_00576690 */
/* Callers:
 * Callees:
 */

void FUN_00576690(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x20) {
      param_1[0xf] = param_1[0xf] ^ 0x69;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      param_1[0xe] = param_1[0xe] ^ 0xdd;
      bVar1 = *param_1;
      *param_1 = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[7];
      param_1[7] = bVar1;
      param_1[0xb] = param_1[0xb] ^ 8;
      param_1[8] = param_1[8] ^ 10;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
    }
    else {
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 5;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 4;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[0x1d];
      param_1[0x1d] = bVar1;
    }
  }
  return;
}



/* 00576d30 FUN_00576d30 */
/* Callers:
 * Callees:
 */

void FUN_00576d30(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      *param_1 = *param_1 ^ 0xd1;
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
    }
    else if (param_2 < 0x20) {
      param_1[1] = param_1[1] ^ 0x32;
      bVar1 = param_1[7];
      param_1[7] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[8];
      param_1[8] = bVar1;
    }
    else {
      bVar1 = param_1[8];
      param_1[8] = param_1[0x13];
      param_1[0x13] = bVar1;
      param_1[1] = param_1[1] ^ 0x7d;
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 4 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xdf;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xef;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x10;
      }
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 6 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0x7f;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x11] = param_1[0x11] & 0xbf;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x40;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x17];
      param_1[0x17] = bVar1;
      param_1[8] = param_1[8] ^ 0x54;
      param_1[0xc] = param_1[0xc] ^ 0x89;
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[3];
      param_1[3] = bVar1;
    }
  }
  return;
}



/* 00577390 FUN_00577390 */
/* Callers:
 * Callees:
 */

void FUN_00577390(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
    }
    else if (param_2 < 0x10) {
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
      *param_1 = *param_1 ^ 0xd1;
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[0xe];
      param_1[0xe] = bVar1;
      param_1[1] = param_1[1] ^ 0x32;
    }
    else {
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      param_1[0xc] = param_1[0xc] ^ 0x89;
      param_1[8] = param_1[8] ^ 0x54;
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 6 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0x7f;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x11] = param_1[0x11] & 0xbf;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x40;
      }
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 4 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xdf;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xef;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x10;
      }
      param_1[1] = param_1[1] ^ 0x7d;
      bVar1 = param_1[8];
      param_1[8] = param_1[0x13];
      param_1[0x13] = bVar1;
    }
  }
  return;
}



/* 005779f0 FUN_005779f0 */
/* Callers:
 * Callees:
 */

void FUN_005779f0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0xf;
      param_1[1] = param_1[1] ^ 0xf6;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      param_1[3] = param_1[3] ^ 0xdd;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      param_1[3] = param_1[3] ^ 0x8d;
      param_1[3] = param_1[3] ^ 0x29;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      param_1[2] = param_1[2] ^ 0x85;
      bVar1 = param_1[5];
      param_1[5] = param_1[1];
      param_1[1] = bVar1;
      param_1[3] = param_1[3] ^ 0xb6;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      param_1[3] = param_1[3] ^ 0x89;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 6;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0xc] = param_1[0xc] ^ 0xfc;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 7;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0xb] = param_1[0xb] ^ 0xec;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      param_1[0xd] = param_1[0xd] ^ 0x74;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 2;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
    }
    else {
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 6 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfb;
      }
      else {
        param_1[0x12] = param_1[0x12] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xbf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x40;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 5;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[0x11] = param_1[0x11] ^ 0xbf;
      bVar1 = param_1[0x17];
      if ((char)((int)(uint)param_1[0x17] >> 7) == '\0') {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0x7f;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x80;
      }
    }
  }
  return;
}



/* 00578360 FUN_00578360 */
/* Callers:
 * Callees:
 */

void FUN_00578360(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      param_1[3] = param_1[3] ^ 0x29;
      param_1[3] = param_1[3] ^ 0x8d;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 0xdd;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      param_1[1] = param_1[1] ^ 0xf6;
      param_1[3] = param_1[3] ^ 0xf;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      param_1[3] = param_1[3] ^ 0x89;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      param_1[3] = param_1[3] ^ 0xb6;
      bVar1 = param_1[5];
      param_1[5] = param_1[1];
      param_1[1] = bVar1;
      param_1[2] = param_1[2] ^ 0x85;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 6;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0xd] = param_1[0xd] ^ 0x74;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      param_1[0xb] = param_1[0xb] ^ 0xec;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 1;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      param_1[0xc] = param_1[0xc] ^ 0xfc;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 2;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xb];
      param_1[0xb] = bVar1;
    }
    else {
      bVar1 = param_1[0x17];
      if ((char)((int)(uint)param_1[0x17] >> 7) == '\0') {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0x7f;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x80;
      }
      param_1[0x11] = param_1[0x11] ^ 0xbf;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 3;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 6 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfb;
      }
      else {
        param_1[0x12] = param_1[0x12] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xbf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x40;
      }
    }
  }
  return;
}



/* 00578ce0 FUN_00578ce0 */
/* Callers:
 * Callees:
 */

void FUN_00578ce0(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      uVar2 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = uVar2;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      if (((int)(uint)(byte)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      param_1[7] = param_1[7] ^ 0xca;
      param_1[3] = param_1[3] ^ 0xe;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      if (((int)(uint)(byte)param_1[0xe] >> 6 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xbf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x40;
      }
      bVar1 = param_1[4];
      if (((int)(uint)(byte)param_1[4] >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      param_1[10] = param_1[10] ^ 0x19;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 2;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 6);
      param_1[9] = param_1[9] ^ 0x8c;
      uVar2 = param_1[2];
      param_1[2] = param_1[0xf];
      param_1[0xf] = uVar2;
    }
    else {
      uVar2 = *param_1;
      *param_1 = param_1[0x17];
      param_1[0x17] = uVar2;
      uVar2 = param_1[10];
      param_1[10] = param_1[0xd];
      param_1[0xd] = uVar2;
      uVar2 = param_1[0x16];
      param_1[0x16] = param_1[3];
      param_1[3] = uVar2;
      bVar1 = param_1[0xb];
      if ((char)((int)(uint)(byte)param_1[0xb] >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xf7;
      }
      else {
        param_1[0xb] = param_1[0xb] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
    }
  }
  return;
}



/* 005790a0 FUN_005790a0 */
/* Callers:
 * Callees:
 */

void FUN_005790a0(undefined1 *param_1,int param_2)

{
  undefined1 uVar1;
  byte bVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      uVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = uVar1;
      bVar2 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar2 >> 5);
      bVar2 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar2 >> 2);
    }
    else if (param_2 < 0x10) {
      param_1[3] = param_1[3] ^ 0xe;
      param_1[7] = param_1[7] ^ 0xca;
      bVar2 = param_1[4];
      if (((int)(uint)(byte)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar2 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
    }
    else if (param_2 < 0x20) {
      uVar1 = param_1[2];
      param_1[2] = param_1[0xf];
      param_1[0xf] = uVar1;
      param_1[9] = param_1[9] ^ 0x8c;
      bVar2 = param_1[9];
      param_1[9] = param_1[9] << 6;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar2 >> 2);
      param_1[10] = param_1[10] ^ 0x19;
      bVar2 = param_1[4];
      if (((int)(uint)(byte)param_1[4] >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar2 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      bVar2 = param_1[0xe];
      if (((int)(uint)(byte)param_1[0xe] >> 6 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      if (((int)(uint)bVar2 >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xbf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x40;
      }
    }
    else {
      bVar2 = param_1[0xb];
      if ((char)((int)(uint)(byte)param_1[0xb] >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xf7;
      }
      else {
        param_1[0xb] = param_1[0xb] | 8;
      }
      if (((int)(uint)bVar2 >> 3 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      uVar1 = param_1[0x16];
      param_1[0x16] = param_1[3];
      param_1[3] = uVar1;
      uVar1 = param_1[10];
      param_1[10] = param_1[0xd];
      param_1[0xd] = uVar1;
      uVar1 = *param_1;
      *param_1 = param_1[0x17];
      param_1[0x17] = uVar1;
    }
  }
  return;
}



/* 00579460 FUN_00579460 */
/* Callers:
 * Callees:
 */

void FUN_00579460(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0xf4;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      *param_1 = *param_1 ^ 0x42;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      param_1[3] = param_1[3] ^ 0x4a;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      param_1[2] = param_1[2] ^ 0x4c;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      *param_1 = *param_1 ^ 0x72;
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      param_1[5] = param_1[5] ^ 0x2f;
      param_1[1] = param_1[1] ^ 0xbe;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      param_1[2] = param_1[2] ^ 0x1c;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 6 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xf7;
      }
      else {
        param_1[0xe] = param_1[0xe] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xbf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x40;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = *param_1;
      *param_1 = bVar1;
      param_1[2] = param_1[2] ^ 0xfa;
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xef;
      }
      else {
        param_1[8] = param_1[8] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
      param_1[6] = param_1[6] ^ 0x46;
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = param_1[0xb];
      param_1[0xb] = bVar1;
      param_1[5] = param_1[5] ^ 0x50;
      param_1[4] = param_1[4] ^ 0xf2;
      bVar1 = param_1[2];
      param_1[2] = param_1[10];
      param_1[10] = bVar1;
    }
    else {
      param_1[0xc] = param_1[0xc] ^ 0x23;
      param_1[0x1d] = param_1[0x1d] ^ 0x41;
      bVar1 = param_1[5];
      param_1[5] = param_1[7];
      param_1[7] = bVar1;
      param_1[0xc] = param_1[0xc] ^ 0xad;
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 3 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0x7f;
      }
      else {
        param_1[0x19] = param_1[0x19] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x19] = param_1[0x19] & 0xf7;
      }
      else {
        param_1[0x19] = param_1[0x19] | 8;
      }
      param_1[8] = param_1[8] ^ 0xd7;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      param_1[4] = param_1[4] ^ 0xf0;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 7;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[7];
      param_1[7] = param_1[0x16];
      param_1[0x16] = bVar1;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 2;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 6);
    }
  }
  return;
}



/* 00579c70 FUN_00579c70 */
/* Callers:
 * Callees:
 */

void FUN_00579c70(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      *param_1 = *param_1 ^ 0x72;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      param_1[2] = param_1[2] ^ 0x4c;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      param_1[3] = param_1[3] ^ 0x4a;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      *param_1 = *param_1 ^ 0x42;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      param_1[3] = param_1[3] ^ 0xf4;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      param_1[2] = param_1[2] ^ 0x1c;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      param_1[1] = param_1[1] ^ 0xbe;
      param_1[5] = param_1[5] ^ 0x2f;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      param_1[2] = param_1[10];
      param_1[10] = bVar1;
      param_1[4] = param_1[4] ^ 0xf2;
      param_1[5] = param_1[5] ^ 0x50;
      bVar1 = *param_1;
      *param_1 = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      param_1[6] = param_1[6] ^ 0x46;
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xef;
      }
      else {
        param_1[8] = param_1[8] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
      param_1[2] = param_1[2] ^ 0xfa;
      bVar1 = param_1[0xb];
      param_1[0xb] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 6 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xf7;
      }
      else {
        param_1[0xe] = param_1[0xe] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xbf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x40;
      }
    }
    else {
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 6;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = param_1[0x16];
      param_1[0x16] = bVar1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 1;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 7);
      param_1[4] = param_1[4] ^ 0xf0;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      param_1[8] = param_1[8] ^ 0xd7;
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 3 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0x7f;
      }
      else {
        param_1[0x19] = param_1[0x19] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x19] = param_1[0x19] & 0xf7;
      }
      else {
        param_1[0x19] = param_1[0x19] | 8;
      }
      param_1[0xc] = param_1[0xc] ^ 0xad;
      bVar1 = param_1[5];
      param_1[5] = param_1[7];
      param_1[7] = bVar1;
      param_1[0x1d] = param_1[0x1d] ^ 0x41;
      param_1[0xc] = param_1[0xc] ^ 0x23;
    }
  }
  return;
}



/* 0057a490 FUN_0057a490 */
/* Callers:
 * Callees:
 */

void FUN_0057a490(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      *param_1 = *param_1 ^ 0xd6;
    }
    else if (param_2 < 0x10) {
      param_1[3] = param_1[3] ^ 0x8a;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      param_1[7] = param_1[7] ^ 0x4f;
      param_1[4] = param_1[4] ^ 99;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 3;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 2;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      param_1[4] = param_1[4] ^ 0x98;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      param_1[8] = param_1[8] ^ 0xa6;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
    }
    else {
      param_1[0x1d] = param_1[0x1d] ^ 0xfa;
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 2;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 1;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 7);
      param_1[8] = param_1[8] ^ 0x59;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 2;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      param_1[5] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x1b];
      param_1[0x1b] = bVar1;
    }
  }
  return;
}



/* 0057ac00 FUN_0057ac00 */
/* Callers:
 * Callees:
 */

void FUN_0057ac00(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0xd6;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      param_1[4] = param_1[4] ^ 99;
      param_1[7] = param_1[7] ^ 0x4f;
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      param_1[3] = param_1[3] ^ 0x8a;
    }
    else if (param_2 < 0x20) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[8] = param_1[8] ^ 0xa6;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      param_1[4] = param_1[4] ^ 0x98;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 6;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 5;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 3);
    }
    else {
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 6;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      param_1[8] = param_1[8] ^ 0x59;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 7;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 6;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0x1d] = param_1[0x1d] ^ 0xfa;
    }
  }
  return;
}



/* 0057b380 FUN_0057b380 */
/* Callers:
 * Callees:
 */

void FUN_0057b380(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0xad;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 4;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 1;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 7);
      param_1[5] = param_1[5] ^ 0x11;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      param_1[3] = param_1[3] ^ 3;
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
    }
    else {
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      param_1[0xd] = param_1[0xd] ^ 0x91;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 6 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xef;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xbf;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 2;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 5;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 3);
    }
  }
  return;
}



/* 0057b9c0 FUN_0057b9c0 */
/* Callers:
 * Callees:
 */

void FUN_0057b9c0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      *param_1 = *param_1 ^ 0xad;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      param_1[5] = param_1[5] ^ 0x11;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 7;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 4;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      param_1[3] = param_1[3] ^ 3;
    }
    else {
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 3;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 6;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 6 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xef;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xbf;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x40;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[2];
      param_1[2] = bVar1;
      param_1[0xd] = param_1[0xd] ^ 0x91;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
    }
  }
  return;
}



/* 0057c000 FUN_0057c000 */
/* Callers:
 * Callees:
 */

void FUN_0057c000(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      *param_1 = *param_1 ^ 0x8b;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[5];
      param_1[5] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      param_1[3] = param_1[3] ^ 0xc9;
      *param_1 = *param_1 ^ 0xcd;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 3;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 5);
      param_1[0xc] = param_1[0xc] ^ 0xe3;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[4] = param_1[4] ^ 0x49;
    }
    else {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      param_1[4] = param_1[4] ^ 0x5f;
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0x19];
      if ((char)((int)(uint)param_1[0x19] >> 7) == '\0') {
        param_1[0x19] = param_1[0x19] & 0xfb;
      }
      else {
        param_1[0x19] = param_1[0x19] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0x7f;
      }
      else {
        param_1[0x19] = param_1[0x19] | 0x80;
      }
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 6;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 3;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x16];
      if (((int)(uint)param_1[0x16] >> 2 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xfb;
      }
      else {
        param_1[0x16] = param_1[0x16] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xfb;
      }
      else {
        param_1[0x16] = param_1[0x16] | 4;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[0x18];
      param_1[0x18] = bVar1;
      param_1[0x1c] = param_1[0x1c] ^ 0xa0;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 1;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 7);
      param_1[0x18] = param_1[0x18] ^ 0x52;
    }
  }
  return;
}



/* 0057c890 FUN_0057c890 */
/* Callers:
 * Callees:
 */

void FUN_0057c890(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0x8b;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x10) {
      *param_1 = *param_1 ^ 0xcd;
      param_1[3] = param_1[3] ^ 0xc9;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4];
      param_1[4] = bVar1;
    }
    else if (param_2 < 0x20) {
      param_1[4] = param_1[4] ^ 0x49;
      bVar1 = *param_1;
      *param_1 = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0xc] = param_1[0xc] ^ 0xe3;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 5;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
    }
    else {
      param_1[0x18] = param_1[0x18] ^ 0x52;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 7;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 1);
      param_1[0x1c] = param_1[0x1c] ^ 0xa0;
      bVar1 = param_1[2];
      param_1[2] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[0x16];
      if (((int)(uint)param_1[0x16] >> 2 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xfb;
      }
      else {
        param_1[0x16] = param_1[0x16] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xfb;
      }
      else {
        param_1[0x16] = param_1[0x16] | 4;
      }
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 5;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 2;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x19];
      if ((char)((int)(uint)param_1[0x19] >> 7) == '\0') {
        param_1[0x19] = param_1[0x19] & 0xfb;
      }
      else {
        param_1[0x19] = param_1[0x19] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0x7f;
      }
      else {
        param_1[0x19] = param_1[0x19] | 0x80;
      }
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[9];
      param_1[9] = bVar1;
      param_1[4] = param_1[4] ^ 0x5f;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
    }
  }
  return;
}



/* 0057d130 FUN_0057d130 */
/* Callers:
 * Callees:
 */

void FUN_0057d130(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      param_1[2] = param_1[2] ^ 0x13;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      param_1[3] = param_1[3] ^ 0x3e;
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      param_1[2] = param_1[2] ^ 0x18;
      param_1[4] = param_1[4] ^ 199;
      *param_1 = *param_1 ^ 200;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x20) {
      param_1[0xf] = param_1[0xf] ^ 0x6f;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xe];
      param_1[0xe] = bVar1;
      param_1[0xe] = param_1[0xe] ^ 0xb;
      *param_1 = *param_1 ^ 0x81;
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xdf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      bVar1 = param_1[8];
      param_1[8] = *param_1;
      *param_1 = bVar1;
    }
    else {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 7;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 1);
      param_1[2] = param_1[2] ^ 0x4e;
      param_1[0x19] = param_1[0x19] ^ 0x61;
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 2;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 6 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfb;
      }
      else {
        param_1[0x18] = param_1[0x18] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xbf;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x40;
      }
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 6 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfb;
      }
      else {
        param_1[0x12] = param_1[0x12] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xbf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x40;
      }
      bVar1 = param_1[0x15];
      if (((int)(uint)param_1[0x15] >> 3 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0x7f;
      }
      else {
        param_1[0x15] = param_1[0x15] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x15] = param_1[0x15] & 0xf7;
      }
      else {
        param_1[0x15] = param_1[0x15] | 8;
      }
      param_1[0xe] = param_1[0xe] ^ 0xb5;
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xdf;
      }
      else {
        param_1[6] = param_1[6] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      bVar1 = param_1[0x1a];
      if (((int)(uint)param_1[0x1a] >> 5 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0x7f;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x1a] = param_1[0x1a] & 0xdf;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x20;
      }
      *param_1 = *param_1 ^ 0x80;
      param_1[0x1d] = param_1[0x1d] ^ 0x1c;
      param_1[0x14] = param_1[0x14] ^ 0x86;
    }
  }
  return;
}



/* 0057d8c0 FUN_0057d8c0 */
/* Callers:
 * Callees:
 */

void FUN_0057d8c0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0x13;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      *param_1 = *param_1 ^ 200;
      param_1[4] = param_1[4] ^ 199;
      param_1[2] = param_1[2] ^ 0x18;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      param_1[3] = param_1[3] ^ 0x3e;
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[8];
      param_1[8] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xdf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      *param_1 = *param_1 ^ 0x81;
      param_1[0xe] = param_1[0xe] ^ 0xb;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      param_1[0xf] = param_1[0xf] ^ 0x6f;
    }
    else {
      param_1[0x14] = param_1[0x14] ^ 0x86;
      param_1[0x1d] = param_1[0x1d] ^ 0x1c;
      *param_1 = *param_1 ^ 0x80;
      bVar1 = param_1[0x1a];
      if (((int)(uint)param_1[0x1a] >> 5 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0x7f;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x1a] = param_1[0x1a] & 0xdf;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x20;
      }
      bVar1 = param_1[6];
      if ((char)((int)(uint)param_1[6] >> 7) == '\0') {
        param_1[6] = param_1[6] & 0xdf;
      }
      else {
        param_1[6] = param_1[6] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[6] = param_1[6] & 0x7f;
      }
      else {
        param_1[6] = param_1[6] | 0x80;
      }
      param_1[0xe] = param_1[0xe] ^ 0xb5;
      bVar1 = param_1[0x15];
      if (((int)(uint)param_1[0x15] >> 3 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0x7f;
      }
      else {
        param_1[0x15] = param_1[0x15] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x15] = param_1[0x15] & 0xf7;
      }
      else {
        param_1[0x15] = param_1[0x15] | 8;
      }
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 6 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfb;
      }
      else {
        param_1[0x12] = param_1[0x12] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xbf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x40;
      }
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 6 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfb;
      }
      else {
        param_1[0x18] = param_1[0x18] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xbf;
      }
      else {
        param_1[0x18] = param_1[0x18] | 0x40;
      }
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 6;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0x19] = param_1[0x19] ^ 0x61;
      param_1[2] = param_1[2] ^ 0x4e;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 1;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 7);
    }
  }
  return;
}



/* 0057e050 FUN_0057e050 */
/* Callers:
 * Callees:
 */

void FUN_0057e050(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 0xc6;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[6];
      param_1[6] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      param_1[4] = param_1[4] ^ 0x84;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
    }
    else if (param_2 < 0x20) {
      param_1[3] = param_1[3] ^ 0x37;
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 1 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfd;
      }
      else {
        param_1[0xb] = param_1[0xb] | 2;
      }
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 6;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 2);
      param_1[1] = param_1[1] ^ 0x44;
      bVar1 = param_1[10];
      if ((char)((int)(uint)param_1[10] >> 7) == '\0') {
        param_1[10] = param_1[10] & 0xbf;
      }
      else {
        param_1[10] = param_1[10] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[10] = param_1[10] & 0x7f;
      }
      else {
        param_1[10] = param_1[10] | 0x80;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 6 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xf7;
      }
      else {
        param_1[0xe] = param_1[0xe] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xbf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x40;
      }
    }
    else {
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 4 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xfb;
      }
      else {
        param_1[0x19] = param_1[0x19] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xef;
      }
      else {
        param_1[0x19] = param_1[0x19] | 0x10;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0x1f] = param_1[0x1f] ^ 0xab;
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[0x1e];
      param_1[0x1e] = bVar1;
    }
  }
  return;
}



/* 0057e820 FUN_0057e820 */
/* Callers:
 * Callees:
 */

void FUN_0057e820(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      param_1[2] = param_1[2] ^ 0xc6;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      param_1[4] = param_1[4] ^ 0x84;
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[5];
      param_1[5] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 6 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xf7;
      }
      else {
        param_1[0xe] = param_1[0xe] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xbf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x40;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[10];
      if ((char)((int)(uint)param_1[10] >> 7) == '\0') {
        param_1[10] = param_1[10] & 0xbf;
      }
      else {
        param_1[10] = param_1[10] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[10] = param_1[10] & 0x7f;
      }
      else {
        param_1[10] = param_1[10] | 0x80;
      }
      param_1[1] = param_1[1] ^ 0x44;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 2;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 1 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfd;
      }
      else {
        param_1[0xb] = param_1[0xb] | 2;
      }
      param_1[3] = param_1[3] ^ 0x37;
    }
    else {
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      param_1[0x1f] = param_1[0x1f] ^ 0xab;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 4 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xfb;
      }
      else {
        param_1[0x19] = param_1[0x19] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xef;
      }
      else {
        param_1[0x19] = param_1[0x19] | 0x10;
      }
    }
  }
  return;
}



/* 0057efe0 FUN_0057efe0 */
/* Callers:
 * Callees:
 */

void FUN_0057efe0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[1] = param_1[1] ^ 0x4e;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[5] = param_1[5] ^ 0xc0;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      param_1[7] = param_1[7] ^ 0x66;
      *param_1 = *param_1 ^ 0xd;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
    }
    else if (param_2 < 0x20) {
      param_1[2] = param_1[2] ^ 0x78;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 5 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xdf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xdf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x20;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[0xb];
      param_1[0xb] = bVar1;
      param_1[8] = param_1[8] ^ 0xb;
    }
    else {
      bVar1 = param_1[10];
      param_1[10] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x1f];
      param_1[0x1f] = bVar1;
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xbf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
    }
  }
  return;
}



/* 0057f7c0 FUN_0057f7c0 */
/* Callers:
 * Callees:
 */

void FUN_0057f7c0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      param_1[1] = param_1[1] ^ 0x4e;
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      *param_1 = *param_1 ^ 0xd;
      param_1[7] = param_1[7] ^ 0x66;
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      param_1[5] = param_1[5] ^ 0xc0;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x20) {
      param_1[8] = param_1[8] ^ 0xb;
      bVar1 = param_1[7];
      param_1[7] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 5 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xdf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xdf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x20;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      param_1[2] = param_1[2] ^ 0x78;
    }
    else {
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xbf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x1f];
      param_1[0x1f] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[0xc];
      param_1[0xc] = bVar1;
    }
  }
  return;
}



/* 0057ff90 FUN_0057ff90 */
/* Callers:
 * Callees:
 */

void FUN_0057ff90(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      *param_1 = *param_1 ^ 4;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      param_1[1] = param_1[1] ^ 0x22;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      param_1[3] = param_1[3] ^ 0x6c;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      param_1[7] = param_1[7] ^ 0x98;
      *param_1 = *param_1 ^ 0xe9;
      param_1[3] = param_1[3] ^ 0x6b;
      param_1[6] = param_1[6] ^ 0xd9;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      param_1[7] = param_1[7] ^ 0x17;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      param_1[7] = param_1[7] ^ 0x6c;
      param_1[3] = param_1[3] ^ 0xac;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0x7f;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      bVar1 = param_1[0xd];
      if ((char)((int)(uint)param_1[0xd] >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      param_1[8] = param_1[8] ^ 0xae;
      bVar1 = param_1[7];
      param_1[7] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 2;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[0xc];
      param_1[0xc] = bVar1;
    }
    else {
      param_1[0x18] = param_1[0x18] ^ 0x6e;
      param_1[8] = param_1[8] ^ 0x89;
      bVar1 = param_1[0x16];
      if (((int)(uint)param_1[0x16] >> 3 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xfd;
      }
      else {
        param_1[0x16] = param_1[0x16] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xf7;
      }
      else {
        param_1[0x16] = param_1[0x16] | 8;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
    }
  }
  return;
}



/* 00580910 FUN_00580910 */
/* Callers:
 * Callees:
 */

void FUN_00580910(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      param_1[3] = param_1[3] ^ 0x6c;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      param_1[1] = param_1[1] ^ 0x22;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      *param_1 = *param_1 ^ 4;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
    }
    else if (param_2 < 0x10) {
      param_1[7] = param_1[7] ^ 0x17;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      param_1[6] = param_1[6] ^ 0xd9;
      param_1[3] = param_1[3] ^ 0x6b;
      *param_1 = *param_1 ^ 0xe9;
      param_1[7] = param_1[7] ^ 0x98;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      param_1[2] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 6;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = param_1[1];
      param_1[1] = bVar1;
      param_1[8] = param_1[8] ^ 0xae;
      bVar1 = param_1[0xd];
      if ((char)((int)(uint)param_1[0xd] >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0x7f;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      param_1[3] = param_1[3] ^ 0xac;
      param_1[7] = param_1[7] ^ 0x6c;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
    }
    else {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x16];
      if (((int)(uint)param_1[0x16] >> 3 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xfd;
      }
      else {
        param_1[0x16] = param_1[0x16] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xf7;
      }
      else {
        param_1[0x16] = param_1[0x16] | 8;
      }
      param_1[8] = param_1[8] ^ 0x89;
      param_1[0x18] = param_1[0x18] ^ 0x6e;
    }
  }
  return;
}



/* 00581290 FUN_00581290 */
/* Callers:
 * Callees:
 */

void FUN_00581290(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 0x13;
      param_1[2] = param_1[2] ^ 0xc6;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      *param_1 = *param_1 ^ 0xc4;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      param_1[1] = param_1[1] ^ 0x21;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[2];
      param_1[2] = bVar1;
      param_1[3] = param_1[3] ^ 0x68;
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      param_1[5] = param_1[5] ^ 0x18;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      param_1[3] = param_1[3] ^ 0xd2;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 5;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
      param_1[7] = param_1[7] ^ 0x77;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 4 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfb;
      }
      else {
        param_1[0xe] = param_1[0xe] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xef;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x10;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 5 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xdf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x20;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
    }
    else {
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 2;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0x17] = param_1[0x17] ^ 0xa3;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 7;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 1);
    }
  }
  return;
}



/* 00581b40 FUN_00581b40 */
/* Callers:
 * Callees:
 */

void FUN_00581b40(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      param_1[1] = param_1[1] ^ 0x21;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      *param_1 = *param_1 ^ 0xc4;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      param_1[2] = param_1[2] ^ 0xc6;
      param_1[1] = param_1[1] ^ 0x13;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      param_1[7] = param_1[7] ^ 0x77;
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 3;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 5);
      param_1[3] = param_1[3] ^ 0xd2;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      param_1[5] = param_1[5] ^ 0x18;
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      param_1[3] = param_1[3] ^ 0x68;
      bVar1 = param_1[6];
      param_1[6] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 5 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xdf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 4 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfb;
      }
      else {
        param_1[0xe] = param_1[0xe] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xef;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x10;
      }
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
    }
    else {
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 1;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 7);
      param_1[0x17] = param_1[0x17] ^ 0xa3;
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 6;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 2);
    }
  }
  return;
}



/* 005823f0 FUN_005823f0 */
/* Callers:
 * Callees:
 */

void FUN_005823f0(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0x91;
      bVar1 = param_1[2];
      if (((int)(uint)(byte)param_1[2] >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      param_1[3] = param_1[3] ^ 0xb7;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      uVar2 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 7;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[6];
      if (((int)(uint)(byte)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      uVar2 = param_1[3];
      param_1[3] = param_1[4];
      param_1[4] = uVar2;
      uVar2 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      uVar2 = param_1[5];
      param_1[5] = *param_1;
      *param_1 = uVar2;
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = param_1[5];
      param_1[5] = param_1[4];
      param_1[4] = uVar2;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 4;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      uVar2 = param_1[0xc];
      param_1[0xc] = param_1[10];
      param_1[10] = uVar2;
    }
    else {
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 2;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 6);
      param_1[8] = param_1[8] ^ 0xdf;
      param_1[8] = param_1[8] ^ 0x20;
      bVar1 = param_1[0x1e];
      if (((int)(uint)(byte)param_1[0x1e] >> 1 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xbf;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfd;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 2;
      }
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 1;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 7);
      uVar2 = param_1[0x13];
      param_1[0x13] = param_1[0xe];
      param_1[0xe] = uVar2;
      bVar1 = param_1[0x14];
      if ((char)((int)(uint)(byte)param_1[0x14] >> 7) == '\0') {
        param_1[0x14] = param_1[0x14] & 0xdf;
      }
      else {
        param_1[0x14] = param_1[0x14] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x14] = param_1[0x14] & 0x7f;
      }
      else {
        param_1[0x14] = param_1[0x14] | 0x80;
      }
      uVar2 = param_1[0x16];
      param_1[0x16] = param_1[0x1b];
      param_1[0x1b] = uVar2;
      bVar1 = param_1[4];
      if (((int)(uint)(byte)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      uVar2 = param_1[9];
      param_1[9] = param_1[0xc];
      param_1[0xc] = uVar2;
      param_1[8] = param_1[8] ^ 0x57;
      bVar1 = param_1[0x1c];
      if (((int)(uint)(byte)param_1[0x1c] >> 4 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0x7f;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x1c] = param_1[0x1c] & 0xef;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 0x10;
      }
    }
  }
  return;
}



/* 00582b90 FUN_00582b90 */
/* Callers:
 * Callees:
 */

void FUN_00582b90(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      param_1[3] = param_1[3] ^ 0xb7;
      bVar1 = param_1[2];
      if (((int)(uint)(byte)param_1[2] >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      param_1[3] = param_1[3] ^ 0x91;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      uVar2 = param_1[5];
      param_1[5] = *param_1;
      *param_1 = uVar2;
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      uVar2 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = uVar2;
      uVar2 = param_1[3];
      param_1[3] = param_1[4];
      param_1[4] = uVar2;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[6];
      if (((int)(uint)(byte)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 1;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 7);
      uVar2 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = uVar2;
    }
    else if (param_2 < 0x20) {
      uVar2 = param_1[0xc];
      param_1[0xc] = param_1[10];
      param_1[10] = uVar2;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 4;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      uVar2 = param_1[5];
      param_1[5] = param_1[4];
      param_1[4] = uVar2;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
    }
    else {
      bVar1 = param_1[0x1c];
      if (((int)(uint)(byte)param_1[0x1c] >> 4 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0x7f;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x1c] = param_1[0x1c] & 0xef;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 0x10;
      }
      param_1[8] = param_1[8] ^ 0x57;
      uVar2 = param_1[9];
      param_1[9] = param_1[0xc];
      param_1[0xc] = uVar2;
      bVar1 = param_1[4];
      if (((int)(uint)(byte)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      uVar2 = param_1[0x16];
      param_1[0x16] = param_1[0x1b];
      param_1[0x1b] = uVar2;
      bVar1 = param_1[0x14];
      if ((char)((int)(uint)(byte)param_1[0x14] >> 7) == '\0') {
        param_1[0x14] = param_1[0x14] & 0xdf;
      }
      else {
        param_1[0x14] = param_1[0x14] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x14] = param_1[0x14] & 0x7f;
      }
      else {
        param_1[0x14] = param_1[0x14] | 0x80;
      }
      uVar2 = param_1[0x13];
      param_1[0x13] = param_1[0xe];
      param_1[0xe] = uVar2;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 7;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x1e];
      if (((int)(uint)(byte)param_1[0x1e] >> 1 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xbf;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfd;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 2;
      }
      param_1[8] = param_1[8] ^ 0x20;
      param_1[8] = param_1[8] ^ 0xdf;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 6;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 2);
    }
  }
  return;
}



/* 00583320 FUN_00583320 */
/* Callers:
 * Callees:
 */

void FUN_00583320(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      param_1[3] = param_1[3] ^ 0x24;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 7;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      param_1[3] = param_1[3] ^ 0x8d;
      bVar1 = *param_1;
      *param_1 = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 5 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xdf;
      }
      else {
        param_1[5] = param_1[5] | 0x20;
      }
      param_1[5] = param_1[5] ^ 0x84;
    }
    else if (param_2 < 0x20) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 1;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[8];
      param_1[8] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
    }
    else {
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 1;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x1c];
      if (((int)(uint)param_1[0x1c] >> 1 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0xfb;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0xfd;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 2;
      }
      param_1[0x18] = param_1[0x18] ^ 0x5c;
      param_1[0x11] = param_1[0x11] ^ 0x1e;
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 3 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xfd;
      }
      else {
        param_1[0x11] = param_1[0x11] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xf7;
      }
      else {
        param_1[0x11] = param_1[0x11] | 8;
      }
      param_1[0x1d] = param_1[0x1d] ^ 0xad;
      bVar1 = param_1[0x13];
      if (((int)(uint)param_1[0x13] >> 1 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xfb;
      }
      else {
        param_1[0x13] = param_1[0x13] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xfd;
      }
      else {
        param_1[0x13] = param_1[0x13] | 2;
      }
      param_1[5] = param_1[5] ^ 0x90;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 4;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x1c];
      param_1[0x1c] = bVar1;
    }
  }
  return;
}



/* 00583a50 FUN_00583a50 */
/* Callers:
 * Callees:
 */

void FUN_00583a50(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      param_1[3] = param_1[3] ^ 0x24;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x10) {
      param_1[5] = param_1[5] ^ 0x84;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 5 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xdf;
      }
      else {
        param_1[5] = param_1[5] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[4];
      param_1[4] = bVar1;
      param_1[3] = param_1[3] ^ 0x8d;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 1;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 4;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[8];
      param_1[8] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 7;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
    }
    else {
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 4;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 4);
      param_1[5] = param_1[5] ^ 0x90;
      bVar1 = param_1[0x13];
      if (((int)(uint)param_1[0x13] >> 1 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xfb;
      }
      else {
        param_1[0x13] = param_1[0x13] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xfd;
      }
      else {
        param_1[0x13] = param_1[0x13] | 2;
      }
      param_1[0x1d] = param_1[0x1d] ^ 0xad;
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 3 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xfd;
      }
      else {
        param_1[0x11] = param_1[0x11] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xf7;
      }
      else {
        param_1[0x11] = param_1[0x11] | 8;
      }
      param_1[0x11] = param_1[0x11] ^ 0x1e;
      param_1[0x18] = param_1[0x18] ^ 0x5c;
      bVar1 = param_1[0x1c];
      if (((int)(uint)param_1[0x1c] >> 1 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0xfb;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0xfd;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 7;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xf];
      param_1[0xf] = bVar1;
    }
  }
  return;
}



/* 00584170 FUN_00584170 */
/* Callers:
 * Callees:
 */

void FUN_00584170(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      param_1[1] = param_1[1] ^ 0xe0;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      param_1[3] = param_1[3] ^ 0x5c;
      *param_1 = *param_1 ^ 0x48;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      param_1[1] = param_1[1] ^ 0x26;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      param_1[1] = param_1[1] ^ 0x72;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[8];
      param_1[8] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 5;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 3);
    }
    else {
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
    }
  }
  return;
}



/* 00584940 FUN_00584940 */
/* Callers:
 * Callees:
 */

void FUN_00584940(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[1] = param_1[1] ^ 0x26;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      *param_1 = *param_1 ^ 0x48;
      param_1[3] = param_1[3] ^ 0x5c;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      param_1[1] = param_1[1] ^ 0xe0;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      param_1[1] = param_1[1] ^ 0x72;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 3;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[0xd];
      param_1[0xd] = bVar1;
    }
    else {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
    }
  }
  return;
}



/* 00585100 FUN_00585100 */
/* Callers:
 * Callees:
 */

void FUN_00585100(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      *param_1 = *param_1 ^ 0x42;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      param_1[2] = param_1[2] ^ 0xf1;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0xd6;
    }
    else if (param_2 < 0x10) {
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
      param_1[3] = param_1[3] ^ 0x8a;
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      param_1[7] = param_1[7] ^ 0xd7;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[4];
      param_1[4] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
    }
    else if (param_2 < 0x20) {
      param_1[8] = param_1[8] ^ 0xec;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
    }
    else {
      param_1[0xc] = param_1[0xc] ^ 0x4c;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[8];
      if ((char)((int)(uint)param_1[8] >> 7) == '\0') {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0x7f;
      }
      else {
        param_1[8] = param_1[8] | 0x80;
      }
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14] << 2;
      param_1[0x14] = param_1[0x14] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x10] << 5;
      param_1[0x10] = param_1[0x10] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x1b];
      if (((int)(uint)param_1[0x1b] >> 1 & 1U) == 0) {
        param_1[0x1b] = param_1[0x1b] & 0xf7;
      }
      else {
        param_1[0x1b] = param_1[0x1b] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0x1b] = param_1[0x1b] & 0xfd;
      }
      else {
        param_1[0x1b] = param_1[0x1b] | 2;
      }
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 2;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0x1b];
      if (((int)(uint)param_1[0x1b] >> 4 & 1U) == 0) {
        param_1[0x1b] = param_1[0x1b] & 0xfd;
      }
      else {
        param_1[0x1b] = param_1[0x1b] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x1b] = param_1[0x1b] & 0xef;
      }
      else {
        param_1[0x1b] = param_1[0x1b] | 0x10;
      }
    }
  }
  return;
}



/* 00585960 FUN_00585960 */
/* Callers:
 * Callees:
 */

void FUN_00585960(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0xd6;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      param_1[2] = param_1[2] ^ 0xf1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0x42;
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      param_1[7] = param_1[7] ^ 0xd7;
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      param_1[3] = param_1[3] ^ 0x8a;
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
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      param_1[8] = param_1[8] ^ 0xec;
    }
    else {
      bVar1 = param_1[0x1b];
      if (((int)(uint)param_1[0x1b] >> 4 & 1U) == 0) {
        param_1[0x1b] = param_1[0x1b] & 0xfd;
      }
      else {
        param_1[0x1b] = param_1[0x1b] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x1b] = param_1[0x1b] & 0xef;
      }
      else {
        param_1[0x1b] = param_1[0x1b] | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 6;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1b];
      if (((int)(uint)param_1[0x1b] >> 1 & 1U) == 0) {
        param_1[0x1b] = param_1[0x1b] & 0xf7;
      }
      else {
        param_1[0x1b] = param_1[0x1b] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0x1b] = param_1[0x1b] & 0xfd;
      }
      else {
        param_1[0x1b] = param_1[0x1b] | 2;
      }
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0x10] << 3;
      param_1[0x10] = param_1[0x10] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14] << 6;
      param_1[0x14] = param_1[0x14] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[8];
      if ((char)((int)(uint)param_1[8] >> 7) == '\0') {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0x7f;
      }
      else {
        param_1[8] = param_1[8] | 0x80;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      param_1[0xc] = param_1[0xc] ^ 0x4c;
    }
  }
  return;
}



/* 005861c0 FUN_005861c0 */
/* Callers:
 * Callees:
 */

void FUN_005861c0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 0xe8;
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      param_1[4] = param_1[4] ^ 0x6e;
      param_1[2] = param_1[2] ^ 0x87;
      param_1[2] = param_1[2] ^ 0x9e;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[4];
      param_1[4] = bVar1;
      param_1[0xd] = param_1[0xd] ^ 0x81;
      param_1[6] = param_1[6] ^ 0x4c;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0x7f;
      }
      else {
        param_1[10] = param_1[10] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      *param_1 = *param_1 ^ 0x4c;
    }
    else {
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 3;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 5);
      param_1[5] = param_1[5] ^ 0x3c;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 2;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      param_1[1] = param_1[0x19];
      param_1[0x19] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[0x1f];
      param_1[0x1f] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 2;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 2;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0xf];
      param_1[0xf] = bVar1;
    }
  }
  return;
}



/* 00586820 FUN_00586820 */
/* Callers:
 * Callees:
 */

void FUN_00586820(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      param_1[1] = param_1[1] ^ 0xe8;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
    }
    else if (param_2 < 0x10) {
      param_1[2] = param_1[2] ^ 0x9e;
      param_1[2] = param_1[2] ^ 0x87;
      param_1[4] = param_1[4] ^ 0x6e;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[4];
      param_1[4] = bVar1;
    }
    else if (param_2 < 0x20) {
      *param_1 = *param_1 ^ 0x4c;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0x7f;
      }
      else {
        param_1[10] = param_1[10] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      param_1[6] = param_1[6] ^ 0x4c;
      param_1[0xd] = param_1[0xd] ^ 0x81;
      bVar1 = param_1[9];
      param_1[9] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
    }
    else {
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 6;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 6;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[9];
      param_1[9] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[0x19];
      param_1[0x19] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 6;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 2);
      param_1[5] = param_1[5] ^ 0x3c;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 5;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 3);
    }
  }
  return;
}



/* 00586e80 FUN_00586e80 */
/* Callers:
 * Callees:
 */

void FUN_00586e80(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      param_1[3] = param_1[3] ^ 0x5b;
      param_1[2] = param_1[2] ^ 0xd3;
    }
    else if (param_2 < 0x10) {
      param_1[5] = param_1[5] ^ 0x4a;
      *param_1 = *param_1 ^ 0xe3;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 7;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 5;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      *param_1 = *param_1 ^ 0xe0;
      bVar1 = param_1[5];
      param_1[5] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      param_1[2] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 3;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 5);
    }
    else {
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 3;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 2;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 6);
      param_1[1] = param_1[1] ^ 0x92;
      bVar1 = param_1[0xe];
      if ((char)((int)(uint)param_1[0xe] >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      param_1[0x1f] = param_1[0x1f] ^ 0x71;
      bVar1 = param_1[7];
      param_1[7] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      bVar1 = param_1[0x1a];
      if (((int)(uint)param_1[0x1a] >> 2 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0x7f;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x1a] = param_1[0x1a] & 0xfb;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 4;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
    }
  }
  return;
}



/* 00587500 FUN_00587500 */
/* Callers:
 * Callees:
 */

void FUN_00587500(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0xd3;
      param_1[3] = param_1[3] ^ 0x5b;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0xe0;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 3;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 1;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      *param_1 = *param_1 ^ 0xe3;
      param_1[5] = param_1[5] ^ 0x4a;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 5;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[8];
      param_1[8] = bVar1;
    }
    else {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1a];
      if (((int)(uint)param_1[0x1a] >> 2 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0x7f;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x1a] = param_1[0x1a] & 0xfb;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 4;
      }
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[7];
      param_1[7] = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[0x1f] = param_1[0x1f] ^ 0x71;
      bVar1 = param_1[0xe];
      if ((char)((int)(uint)param_1[0xe] >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      param_1[1] = param_1[1] ^ 0x92;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 6;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1b];
      param_1[0x1b] = param_1[0x1b] << 5;
      param_1[0x1b] = param_1[0x1b] | (byte)((int)(uint)bVar1 >> 3);
    }
  }
  return;
}



/* 00587b80 FUN_00587b80 */
/* Callers:
 * Callees:
 */

void FUN_00587b80(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      param_1[1] = param_1[1] ^ 0xd6;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      param_1[2] = param_1[2] ^ 0xfc;
    }
    else if (param_2 < 0x10) {
      param_1[6] = param_1[6] ^ 0xf6;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      param_1[6] = param_1[6] ^ 0x9d;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[9];
      param_1[9] = bVar1;
    }
    else {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      param_1[0xb] = param_1[0xb] ^ 0xb;
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 3 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xfd;
      }
      else {
        param_1[0x19] = param_1[0x19] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xf7;
      }
      else {
        param_1[0x19] = param_1[0x19] | 8;
      }
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      bVar1 = param_1[0x1e];
      if (((int)(uint)param_1[0x1e] >> 2 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xf7;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfb;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 4;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 1;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[9];
      if ((char)((int)(uint)param_1[9] >> 7) == '\0') {
        param_1[9] = param_1[9] & 0xfd;
      }
      else {
        param_1[9] = param_1[9] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[9] = param_1[9] & 0x7f;
      }
      else {
        param_1[9] = param_1[9] | 0x80;
      }
    }
  }
  return;
}



/* 00588240 FUN_00588240 */
/* Callers:
 * Callees:
 */

void FUN_00588240(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0xfc;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 0xd6;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      param_1[6] = param_1[6] ^ 0x9d;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      param_1[6] = param_1[6] ^ 0xf6;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[8];
      param_1[8] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
    }
    else {
      bVar1 = param_1[9];
      if ((char)((int)(uint)param_1[9] >> 7) == '\0') {
        param_1[9] = param_1[9] & 0xfd;
      }
      else {
        param_1[9] = param_1[9] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[9] = param_1[9] & 0x7f;
      }
      else {
        param_1[9] = param_1[9] | 0x80;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 7;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x1e];
      if (((int)(uint)param_1[0x1e] >> 2 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xf7;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfb;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 4;
      }
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 3 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xfd;
      }
      else {
        param_1[0x19] = param_1[0x19] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xf7;
      }
      else {
        param_1[0x19] = param_1[0x19] | 8;
      }
      param_1[0xb] = param_1[0xb] ^ 0xb;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xdf;
      }
      else {
        param_1[7] = param_1[7] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
    }
  }
  return;
}



/* 00588900 FUN_00588900 */
/* Callers:
 * Callees:
 */

void FUN_00588900(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[1] = param_1[1] ^ 0xc0;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      param_1[2] = param_1[2] ^ 0x30;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      *param_1 = *param_1 ^ 0x6a;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 5 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xdf;
      }
      else {
        param_1[4] = param_1[4] | 0x20;
      }
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
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      param_1[4] = param_1[4] ^ 0x36;
      param_1[4] = param_1[4] ^ 0x3f;
      param_1[1] = param_1[1] ^ 0x2f;
      param_1[6] = param_1[6] ^ 0x4f;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 5 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xdf;
      }
      else {
        param_1[6] = param_1[6] | 0x20;
      }
    }
    else if (param_2 < 0x20) {
      *param_1 = *param_1 ^ 0x2a;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 4;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      param_1[9] = param_1[9] ^ 2;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 1;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0xfb;
      }
      else {
        param_1[0xf] = param_1[0xf] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      param_1[10] = param_1[10] ^ 0xa1;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[9];
      param_1[9] = bVar1;
    }
    else {
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 3 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xf7;
      }
      else {
        param_1[8] = param_1[8] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xf7;
      }
      else {
        param_1[8] = param_1[8] | 8;
      }
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = *param_1;
      *param_1 = bVar1;
    }
  }
  return;
}



/* 00589260 FUN_00589260 */
/* Callers:
 * Callees:
 */

void FUN_00589260(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0x6a;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      param_1[2] = param_1[2] ^ 0x30;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      param_1[1] = param_1[1] ^ 0xc0;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 5 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xdf;
      }
      else {
        param_1[6] = param_1[6] | 0x20;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      param_1[6] = param_1[6] ^ 0x4f;
      param_1[1] = param_1[1] ^ 0x2f;
      param_1[4] = param_1[4] ^ 0x3f;
      param_1[4] = param_1[4] ^ 0x36;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
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
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 5 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xdf;
      }
      else {
        param_1[4] = param_1[4] | 0x20;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[6];
      param_1[6] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      param_1[10] = param_1[10] ^ 0xa1;
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0xfb;
      }
      else {
        param_1[0xf] = param_1[0xf] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 7;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 1);
      param_1[9] = param_1[9] ^ 2;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 4;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 4);
      *param_1 = *param_1 ^ 0x2a;
    }
    else {
      bVar1 = param_1[0x1f];
      param_1[0x1f] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 3 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xf7;
      }
      else {
        param_1[8] = param_1[8] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xf7;
      }
      else {
        param_1[8] = param_1[8] | 8;
      }
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18];
      param_1[0x18] = bVar1;
    }
  }
  return;
}



/* 00589bc0 FUN_00589bc0 */
/* Callers:
 * Callees:
 */

void FUN_00589bc0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0xe9;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      param_1[1] = param_1[1] ^ 0xf7;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 0xab;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      param_1[4] = param_1[4] ^ 0xa6;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      param_1[8] = param_1[8] ^ 0xc9;
      param_1[6] = param_1[6] ^ 0x4b;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      param_1[10] = param_1[10] ^ 0xaf;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 5;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 3);
      param_1[10] = param_1[10] ^ 0x67;
      bVar1 = param_1[8];
      param_1[8] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[0xb];
      param_1[0xb] = bVar1;
    }
    else {
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[0x13];
      if (((int)(uint)param_1[0x13] >> 3 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xdf;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xf7;
      }
      else {
        param_1[0x13] = param_1[0x13] | 8;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[10];
      param_1[10] = bVar1;
      param_1[0x12] = param_1[0x12] ^ 0xef;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = param_1[0x1d];
      if (((int)(uint)param_1[0x1d] >> 2 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xef;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xfb;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 4;
      }
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      param_1[0x14] = param_1[0x14] ^ 0xcf;
      param_1[0x19] = param_1[0x19] ^ 0x6d;
    }
  }
  return;
}



/* 0058a320 FUN_0058a320 */
/* Callers:
 * Callees:
 */

void FUN_0058a320(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      param_1[2] = param_1[2] ^ 0xab;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      param_1[1] = param_1[1] ^ 0xf7;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      param_1[3] = param_1[3] ^ 0xe9;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      param_1[4] = param_1[4] ^ 0xa6;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[8];
      param_1[8] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[3];
      param_1[3] = bVar1;
      param_1[10] = param_1[10] ^ 0x67;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 3;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 5);
      param_1[10] = param_1[10] ^ 0xaf;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      param_1[6] = param_1[6] ^ 0x4b;
      param_1[8] = param_1[8] ^ 0xc9;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xd];
      param_1[0xd] = bVar1;
    }
    else {
      param_1[0x19] = param_1[0x19] ^ 0x6d;
      param_1[0x14] = param_1[0x14] ^ 0xcf;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[0x1d];
      if (((int)(uint)param_1[0x1d] >> 2 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xef;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xfb;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 4;
      }
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      param_1[0x12] = param_1[0x12] ^ 0xef;
      bVar1 = param_1[1];
      param_1[1] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[0x13];
      if (((int)(uint)param_1[0x13] >> 3 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xdf;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xf7;
      }
      else {
        param_1[0x13] = param_1[0x13] | 8;
      }
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
    }
  }
  return;
}



/* 0058aa80 FUN_0058aa80 */
/* Callers:
 * Callees:
 */

void FUN_0058aa80(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      param_1[1] = param_1[1] ^ 2;
      param_1[1] = param_1[1] ^ 0x60;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      param_1[3] = param_1[3] ^ 0xd7;
    }
    else if (param_2 < 0x10) {
      param_1[7] = param_1[7] ^ 0x3c;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      param_1[1] = param_1[1] ^ 0xfa;
      param_1[4] = param_1[4] ^ 0x1c;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xd];
      if ((char)((int)(uint)param_1[0xd] >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[0xe];
      param_1[0xe] = bVar1;
      param_1[3] = param_1[3] ^ 0x69;
      param_1[6] = param_1[6] ^ 0xf1;
      param_1[10] = param_1[10] ^ 0x91;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 4;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      param_1[2] = param_1[2] ^ 0x85;
      param_1[0xf] = param_1[0xf] ^ 0x4f;
    }
    else {
      param_1[8] = param_1[8] ^ 0xbb;
      param_1[6] = param_1[6] ^ 0xe;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 1;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[6];
      param_1[6] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 5;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[10];
      param_1[10] = bVar1;
    }
  }
  return;
}



/* 0058b130 FUN_0058b130 */
/* Callers:
 * Callees:
 */

void FUN_0058b130(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0xd7;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      param_1[1] = param_1[1] ^ 0x60;
      param_1[1] = param_1[1] ^ 2;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      param_1[4] = param_1[4] ^ 0x1c;
      param_1[1] = param_1[1] ^ 0xfa;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      param_1[7] = param_1[7] ^ 0x3c;
    }
    else if (param_2 < 0x20) {
      param_1[0xf] = param_1[0xf] ^ 0x4f;
      param_1[2] = param_1[2] ^ 0x85;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 4;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 4);
      param_1[10] = param_1[10] ^ 0x91;
      param_1[6] = param_1[6] ^ 0xf1;
      param_1[3] = param_1[3] ^ 0x69;
      bVar1 = param_1[4];
      param_1[4] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0xd];
      if ((char)((int)(uint)param_1[0xd] >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
    }
    else {
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 3;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x11] << 7;
      param_1[0x11] = param_1[0x11] | (byte)((int)(uint)bVar1 >> 1);
      param_1[6] = param_1[6] ^ 0xe;
      param_1[8] = param_1[8] ^ 0xbb;
    }
  }
  return;
}



/* 0058b7e0 FUN_0058b7e0 */
/* Callers:
 * Callees:
 */

void FUN_0058b7e0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0xf;
      param_1[1] = param_1[1] ^ 0x28;
      *param_1 = *param_1 ^ 0x55;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      param_1[5] = param_1[5] ^ 0x2e;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xdf;
      }
      else {
        param_1[6] = param_1[6] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[5];
      param_1[5] = bVar1;
    }
    else if (param_2 < 0x20) {
      *param_1 = *param_1 ^ 0xa3;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      param_1[7] = param_1[7] ^ 0x2d;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 5;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      param_1[3] = param_1[3] ^ 0x55;
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 3 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xef;
      }
      else {
        param_1[9] = param_1[9] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xf7;
      }
      else {
        param_1[9] = param_1[9] | 8;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
    }
    else {
      param_1[0x1a] = param_1[0x1a] ^ 0x39;
      param_1[3] = param_1[3] ^ 0xda;
      bVar1 = param_1[10];
      param_1[10] = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[0xe] = param_1[0xe] ^ 0x5e;
    }
  }
  return;
}



/* 0058bcf0 FUN_0058bcf0 */
/* Callers:
 * Callees:
 */

void FUN_0058bcf0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      *param_1 = *param_1 ^ 0x55;
      param_1[1] = param_1[1] ^ 0x28;
      param_1[3] = param_1[3] ^ 0xf;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xdf;
      }
      else {
        param_1[6] = param_1[6] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      param_1[5] = param_1[5] ^ 0x2e;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 3 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xef;
      }
      else {
        param_1[9] = param_1[9] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xf7;
      }
      else {
        param_1[9] = param_1[9] | 8;
      }
      param_1[3] = param_1[3] ^ 0x55;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 3;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 5);
      param_1[7] = param_1[7] ^ 0x2d;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      *param_1 = *param_1 ^ 0xa3;
    }
    else {
      param_1[0xe] = param_1[0xe] ^ 0x5e;
      bVar1 = param_1[10];
      param_1[10] = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[3] = param_1[3] ^ 0xda;
      param_1[0x1a] = param_1[0x1a] ^ 0x39;
    }
  }
  return;
}



/* 0058c200 FUN_0058c200 */
/* Callers:
 * Callees:
 */

void FUN_0058c200(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      param_1[2] = param_1[2] ^ 0xe6;
      param_1[1] = param_1[1] ^ 0x1d;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      *param_1 = *param_1 ^ 0x48;
      param_1[3] = param_1[3] ^ 0x75;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      param_1[6] = param_1[6] ^ 0xcb;
    }
    else if (param_2 < 0x20) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      param_1[9] = param_1[9] ^ 0xf5;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[3];
      param_1[3] = bVar1;
    }
    else {
      param_1[6] = param_1[6] ^ 0xb7;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 3 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xfb;
      }
      else {
        param_1[0x19] = param_1[0x19] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xf7;
      }
      else {
        param_1[0x19] = param_1[0x19] | 8;
      }
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 3 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xf7;
      }
      else {
        param_1[0xb] = param_1[0xb] | 8;
      }
    }
  }
  return;
}



/* 0058c7d0 FUN_0058c7d0 */
/* Callers:
 * Callees:
 */

void FUN_0058c7d0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0x75;
      *param_1 = *param_1 ^ 0x48;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      param_1[1] = param_1[1] ^ 0x1d;
      param_1[2] = param_1[2] ^ 0xe6;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x10) {
      param_1[6] = param_1[6] ^ 0xcb;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[3];
      param_1[3] = bVar1;
      param_1[9] = param_1[9] ^ 0xf5;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
    }
    else {
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 3 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xf7;
      }
      else {
        param_1[0xb] = param_1[0xb] | 8;
      }
      bVar1 = param_1[0x19];
      if (((int)(uint)param_1[0x19] >> 3 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xfb;
      }
      else {
        param_1[0x19] = param_1[0x19] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x19] = param_1[0x19] & 0xf7;
      }
      else {
        param_1[0x19] = param_1[0x19] | 8;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xd];
      param_1[0xd] = bVar1;
      param_1[6] = param_1[6] ^ 0xb7;
    }
  }
  return;
}



/* 0058cda0 FUN_0058cda0 */
/* Callers:
 * Callees:
 */

void FUN_0058cda0(int param_1,int param_2)

{
  undefined1 uVar1;
  byte bVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) ^ 0x96;
      uVar1 = *(undefined1 *)(param_1 + 1);
      *(undefined1 *)(param_1 + 1) = *(undefined1 *)(param_1 + 2);
      *(undefined1 *)(param_1 + 2) = uVar1;
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) ^ 0xc5;
      bVar2 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 5;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar2 >> 3);
      bVar2 = *(byte *)(param_1 + 1);
      if (((int)(uint)*(byte *)(param_1 + 1) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | 4;
      }
      if (((int)(uint)bVar2 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) & 0xef;
      }
      else {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | 0x10;
      }
    }
    else if (param_2 < 0x10) {
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) ^ 0x9c;
      bVar2 = *(byte *)(param_1 + 5);
      if (((int)(uint)*(byte *)(param_1 + 5) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) & 0xf7;
      }
      else {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) | 8;
      }
      if (((int)(uint)bVar2 >> 3 & 1U) == 0) {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) & 0xef;
      }
      else {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) | 0x10;
      }
      bVar2 = *(byte *)(param_1 + 4);
      *(char *)(param_1 + 4) = *(char *)(param_1 + 4) << 2;
      *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) | (byte)((int)(uint)bVar2 >> 6);
    }
    else if (param_2 < 0x20) {
      bVar2 = *(byte *)(param_1 + 10);
      if (((int)(uint)*(byte *)(param_1 + 10) >> 1 & 1U) == 0) {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) | 4;
      }
      if (((int)(uint)bVar2 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) & 0xfd;
      }
      else {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) | 2;
      }
      *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) ^ 0x97;
      bVar2 = *(byte *)(param_1 + 8);
      *(char *)(param_1 + 8) = *(char *)(param_1 + 8) << 7;
      *(byte *)(param_1 + 8) = *(byte *)(param_1 + 8) | (byte)((int)(uint)bVar2 >> 1);
      bVar2 = *(byte *)(param_1 + 6);
      if (((int)(uint)*(byte *)(param_1 + 6) >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | 0x40;
      }
      if (((int)(uint)bVar2 >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | 0x40;
      }
      bVar2 = *(byte *)(param_1 + 4);
      if (((int)(uint)*(byte *)(param_1 + 4) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) & 0xef;
      }
      else {
        *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) | 0x10;
      }
      if (((int)(uint)bVar2 >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) & 0xef;
      }
      else {
        *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) | 0x10;
      }
      uVar1 = *(undefined1 *)(param_1 + 0xe);
      *(undefined1 *)(param_1 + 0xe) = *(undefined1 *)(param_1 + 5);
      *(undefined1 *)(param_1 + 5) = uVar1;
      bVar2 = *(byte *)(param_1 + 3);
      *(char *)(param_1 + 3) = *(char *)(param_1 + 3) << 3;
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | (byte)((int)(uint)bVar2 >> 5);
      bVar2 = *(byte *)(param_1 + 10);
      if (((int)(uint)*(byte *)(param_1 + 10) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) | 4;
      }
      if (((int)(uint)bVar2 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) & 0xef;
      }
      else {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) | 0x10;
      }
      uVar1 = *(undefined1 *)(param_1 + 5);
      *(undefined1 *)(param_1 + 5) = *(undefined1 *)(param_1 + 0xb);
      *(undefined1 *)(param_1 + 0xb) = uVar1;
      bVar2 = *(byte *)(param_1 + 2);
      *(char *)(param_1 + 2) = *(char *)(param_1 + 2) << 3;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | (byte)((int)(uint)bVar2 >> 5);
      *(byte *)(param_1 + 0xc) = *(byte *)(param_1 + 0xc) ^ 0x41;
    }
    else {
      bVar2 = *(byte *)(param_1 + 0xd);
      if (((int)(uint)*(byte *)(param_1 + 0xd) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) | 4;
      }
      if (((int)(uint)bVar2 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) & 0xef;
      }
      else {
        *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) | 0x10;
      }
      uVar1 = *(undefined1 *)(param_1 + 0x18);
      *(undefined1 *)(param_1 + 0x18) = *(undefined1 *)(param_1 + 0xd);
      *(undefined1 *)(param_1 + 0xd) = uVar1;
      *(byte *)(param_1 + 0x12) = *(byte *)(param_1 + 0x12) ^ 0x9a;
    }
  }
  return;
}



/* 0058d340 FUN_0058d340 */
/* Callers:
 * Callees:
 */

void FUN_0058d340(int param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *(byte *)(param_1 + 1);
      if (((int)(uint)*(byte *)(param_1 + 1) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) & 0xef;
      }
      else {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | 0x10;
      }
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 3;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 5);
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) ^ 0xc5;
      uVar2 = *(undefined1 *)(param_1 + 1);
      *(undefined1 *)(param_1 + 1) = *(undefined1 *)(param_1 + 2);
      *(undefined1 *)(param_1 + 2) = uVar2;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) ^ 0x96;
    }
    else if (param_2 < 0x10) {
      bVar1 = *(byte *)(param_1 + 4);
      *(char *)(param_1 + 4) = *(char *)(param_1 + 4) << 6;
      *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *(byte *)(param_1 + 5);
      if (((int)(uint)*(byte *)(param_1 + 5) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) & 0xf7;
      }
      else {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) & 0xef;
      }
      else {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) | 0x10;
      }
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) ^ 0x9c;
    }
    else if (param_2 < 0x20) {
      *(byte *)(param_1 + 0xc) = *(byte *)(param_1 + 0xc) ^ 0x41;
      bVar1 = *(byte *)(param_1 + 2);
      *(char *)(param_1 + 2) = *(char *)(param_1 + 2) << 5;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | (byte)((int)(uint)bVar1 >> 3);
      uVar2 = *(undefined1 *)(param_1 + 5);
      *(undefined1 *)(param_1 + 5) = *(undefined1 *)(param_1 + 0xb);
      *(undefined1 *)(param_1 + 0xb) = uVar2;
      bVar1 = *(byte *)(param_1 + 10);
      if (((int)(uint)*(byte *)(param_1 + 10) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) & 0xef;
      }
      else {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) | 0x10;
      }
      bVar1 = *(byte *)(param_1 + 3);
      *(char *)(param_1 + 3) = *(char *)(param_1 + 3) << 5;
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | (byte)((int)(uint)bVar1 >> 3);
      uVar2 = *(undefined1 *)(param_1 + 0xe);
      *(undefined1 *)(param_1 + 0xe) = *(undefined1 *)(param_1 + 5);
      *(undefined1 *)(param_1 + 5) = uVar2;
      bVar1 = *(byte *)(param_1 + 4);
      if (((int)(uint)*(byte *)(param_1 + 4) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) & 0xef;
      }
      else {
        *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) & 0xef;
      }
      else {
        *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) | 0x10;
      }
      bVar1 = *(byte *)(param_1 + 6);
      if (((int)(uint)*(byte *)(param_1 + 6) >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | 0x40;
      }
      bVar1 = *(byte *)(param_1 + 8);
      *(char *)(param_1 + 8) = *(char *)(param_1 + 8) << 1;
      *(byte *)(param_1 + 8) = *(byte *)(param_1 + 8) | (byte)((int)(uint)bVar1 >> 7);
      *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) ^ 0x97;
      bVar1 = *(byte *)(param_1 + 10);
      if (((int)(uint)*(byte *)(param_1 + 10) >> 1 & 1U) == 0) {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) & 0xfd;
      }
      else {
        *(byte *)(param_1 + 10) = *(byte *)(param_1 + 10) | 2;
      }
    }
    else {
      *(byte *)(param_1 + 0x12) = *(byte *)(param_1 + 0x12) ^ 0x9a;
      uVar2 = *(undefined1 *)(param_1 + 0x18);
      *(undefined1 *)(param_1 + 0x18) = *(undefined1 *)(param_1 + 0xd);
      *(undefined1 *)(param_1 + 0xd) = uVar2;
      bVar1 = *(byte *)(param_1 + 0xd);
      if (((int)(uint)*(byte *)(param_1 + 0xd) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) & 0xef;
      }
      else {
        *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) | 0x10;
      }
    }
  }
  return;
}



/* 0058d8e0 FUN_0058d8e0 */
/* Callers:
 * Callees:
 */

void FUN_0058d8e0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      param_1[4] = param_1[4] ^ 0xfc;
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      param_1[1] = param_1[1] ^ 0xcc;
      bVar1 = param_1[7];
      param_1[7] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 5;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 100;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      param_1[6] = param_1[6] ^ 6;
      bVar1 = param_1[2];
      param_1[2] = param_1[6];
      param_1[6] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 4;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 4);
      param_1[0xb] = param_1[0xb] ^ 0xcf;
      param_1[3] = param_1[3] ^ 0x94;
      bVar1 = param_1[2];
      param_1[2] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 7;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 1);
      param_1[0xf] = param_1[0xf] ^ 0x8c;
      bVar1 = param_1[8];
      param_1[8] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 6;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[8];
      param_1[8] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xf7;
      }
      else {
        param_1[0xf] = param_1[0xf] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 1;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 7);
    }
    else {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 7;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = param_1[0x19];
      param_1[0x19] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0x10];
      if (((int)(uint)param_1[0x10] >> 1 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xfd;
      }
      else {
        param_1[0x10] = param_1[0x10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xfd;
      }
      else {
        param_1[0x10] = param_1[0x10] | 2;
      }
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 6;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[0x1a] << 2;
      param_1[0x1a] = param_1[0x1a] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 4;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 4);
      param_1[7] = param_1[7] ^ 0xd6;
      param_1[0xd] = param_1[0xd] ^ 0xb7;
      *param_1 = *param_1 ^ 0xc5;
      param_1[0x16] = param_1[0x16] ^ 0x3b;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x18];
      param_1[0x18] = bVar1;
    }
  }
  return;
}



/* 0058e120 FUN_0058e120 */
/* Callers:
 * Callees:
 */

void FUN_0058e120(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[6];
      param_1[6] = bVar1;
      param_1[6] = param_1[6] ^ 6;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      param_1[3] = param_1[3] ^ 100;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 3;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[6];
      param_1[6] = bVar1;
      param_1[1] = param_1[1] ^ 0xcc;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      param_1[4] = param_1[4] ^ 0xfc;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 7;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xf7;
      }
      else {
        param_1[0xf] = param_1[0xf] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 2;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[6];
      param_1[6] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[8];
      param_1[8] = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0x8c;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 1;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      param_1[2] = param_1[10];
      param_1[10] = bVar1;
      param_1[3] = param_1[3] ^ 0x94;
      param_1[0xb] = param_1[0xb] ^ 0xcf;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 4;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 4);
    }
    else {
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0xf];
      param_1[0xf] = bVar1;
      param_1[0x16] = param_1[0x16] ^ 0x3b;
      *param_1 = *param_1 ^ 0xc5;
      param_1[0xd] = param_1[0xd] ^ 0xb7;
      param_1[7] = param_1[7] ^ 0xd6;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 4;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[0x1a] << 6;
      param_1[0x1a] = param_1[0x1a] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 2;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x10];
      if (((int)(uint)param_1[0x10] >> 1 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xfd;
      }
      else {
        param_1[0x10] = param_1[0x10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xfd;
      }
      else {
        param_1[0x10] = param_1[0x10] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[0x19];
      param_1[0x19] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 1;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 7);
    }
  }
  return;
}



/* 0058e960 FUN_0058e960 */
/* Callers:
 * Callees:
 */

void FUN_0058e960(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0x68;
      *param_1 = *param_1 ^ 0xbe;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      *param_1 = *param_1 ^ 0x7c;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
    }
    else if (param_2 < 0x10) {
      param_1[5] = param_1[5] ^ 0xda;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      param_1[7] = param_1[7] ^ 0x2a;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 5;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      param_1[6] = param_1[6] ^ 0x3b;
      param_1[1] = param_1[1] ^ 1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 1;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      bVar1 = param_1[0xd];
      if ((char)((int)(uint)param_1[0xd] >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xbf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      param_1[8] = param_1[8] ^ 0x4b;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 2;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xd];
      param_1[0xd] = bVar1;
    }
    else {
      param_1[3] = param_1[3] ^ 0x25;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 7;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0x15];
      if (((int)(uint)param_1[0x15] >> 3 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0x7f;
      }
      else {
        param_1[0x15] = param_1[0x15] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x15] = param_1[0x15] & 0xf7;
      }
      else {
        param_1[0x15] = param_1[0x15] | 8;
      }
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 7;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 1);
    }
  }
  return;
}



/* 0058f140 FUN_0058f140 */
/* Callers:
 * Callees:
 */

void FUN_0058f140(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      *param_1 = *param_1 ^ 0x7c;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      *param_1 = *param_1 ^ 0xbe;
      param_1[2] = param_1[2] ^ 0x68;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 7;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 1);
      param_1[1] = param_1[1] ^ 1;
      param_1[6] = param_1[6] ^ 0x3b;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 3;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      param_1[7] = param_1[7] ^ 0x2a;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      param_1[5] = param_1[5] ^ 0xda;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 6;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 2);
      param_1[8] = param_1[8] ^ 0x4b;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xd];
      if ((char)((int)(uint)param_1[0xd] >> 7) == '\0') {
        param_1[0xd] = param_1[0xd] & 0xbf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0x7f;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x80;
      }
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
    }
    else {
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 1;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      bVar1 = param_1[0x15];
      if (((int)(uint)param_1[0x15] >> 3 & 1U) == 0) {
        param_1[0x15] = param_1[0x15] & 0x7f;
      }
      else {
        param_1[0x15] = param_1[0x15] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x15] = param_1[0x15] & 0xf7;
      }
      else {
        param_1[0x15] = param_1[0x15] | 8;
      }
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0x10];
      param_1[0x10] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 1;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 7);
      param_1[3] = param_1[3] ^ 0x25;
    }
  }
  return;
}



/* 0058f920 FUN_0058f920 */
/* Callers:
 * Callees:
 */

void FUN_0058f920(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      param_1[2] = param_1[2] ^ 0xa9;
      bVar1 = param_1[2];
      if (((int)(uint)(byte)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
    }
    else if (param_2 < 0x10) {
      param_1[4] = param_1[4] ^ 0x74;
      uVar2 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = uVar2;
      uVar2 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = uVar2;
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = uVar2;
      param_1[6] = param_1[6] ^ 0xc5;
      uVar2 = param_1[7];
      param_1[7] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[7];
      if (((int)(uint)(byte)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 1;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 7);
      param_1[4] = param_1[4] ^ 0xc;
      uVar2 = param_1[9];
      param_1[9] = param_1[7];
      param_1[7] = uVar2;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 7;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 1);
      uVar2 = param_1[0xf];
      param_1[0xf] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 1;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 2;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 6);
    }
    else {
      bVar1 = param_1[0xf];
      if (((int)(uint)(byte)param_1[0xf] >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      bVar1 = param_1[0x1c];
      if (((int)(uint)(byte)param_1[0x1c] >> 1 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0xef;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0xfd;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 2;
      }
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 7;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 1);
    }
  }
  return;
}



/* 0058feb0 FUN_0058feb0 */
/* Callers:
 * Callees:
 */

void FUN_0058feb0(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      if (((int)(uint)(byte)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      param_1[2] = param_1[2] ^ 0xa9;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = param_1[7];
      param_1[7] = param_1[2];
      param_1[2] = uVar2;
      param_1[6] = param_1[6] ^ 0xc5;
      uVar2 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = uVar2;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      uVar2 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = uVar2;
      uVar2 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = uVar2;
      param_1[4] = param_1[4] ^ 0x74;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 6;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 7;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 1);
      uVar2 = param_1[0xf];
      param_1[0xf] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 1;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 7);
      uVar2 = param_1[9];
      param_1[9] = param_1[7];
      param_1[7] = uVar2;
      param_1[4] = param_1[4] ^ 0xc;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 7;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[7];
      if (((int)(uint)(byte)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
    }
    else {
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 1;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x1c];
      if (((int)(uint)(byte)param_1[0x1c] >> 1 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0xef;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1c] = param_1[0x1c] & 0xfd;
      }
      else {
        param_1[0x1c] = param_1[0x1c] | 2;
      }
      bVar1 = param_1[0xf];
      if (((int)(uint)(byte)param_1[0xf] >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
    }
  }
  return;
}



/* 00590440 FUN_00590440 */
/* Callers:
 * Callees:
 */

void FUN_00590440(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      param_1[2] = param_1[2] ^ 0xea;
      param_1[1] = param_1[1] ^ 0xe4;
      param_1[2] = param_1[2] ^ 0x6c;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      param_1[2] = param_1[2] ^ 0x8a;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 0x5f;
      *param_1 = *param_1 ^ 0xfa;
    }
    else if (param_2 < 0x10) {
      param_1[6] = param_1[6] ^ 0x8f;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      param_1[3] = param_1[3] ^ 0x6b;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 3;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[2];
      param_1[2] = bVar1;
    }
    else {
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      param_1[0x10] = param_1[0x10] ^ 0x93;
      param_1[0x12] = param_1[0x12] ^ 0xac;
    }
  }
  return;
}



/* 00590c40 FUN_00590c40 */
/* Callers:
 * Callees:
 */

void FUN_00590c40(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0xfa;
      param_1[2] = param_1[2] ^ 0x5f;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 0x8a;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      param_1[2] = param_1[2] ^ 0x6c;
      param_1[1] = param_1[1] ^ 0xe4;
      param_1[2] = param_1[2] ^ 0xea;
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 5;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 3);
      param_1[3] = param_1[3] ^ 0x6b;
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      param_1[6] = param_1[6] ^ 0x8f;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
    }
    else {
      param_1[0x12] = param_1[0x12] ^ 0xac;
      param_1[0x10] = param_1[0x10] ^ 0x93;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
    }
  }
  return;
}



/* 00591440 FUN_00591440 */
/* Callers:
 * Callees:
 */

void FUN_00591440(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      *param_1 = *param_1 ^ 0x1f;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      *param_1 = *param_1 ^ 0x37;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      param_1[2] = param_1[2] ^ 0xdc;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[4] = param_1[4] ^ 0xd;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[4];
      param_1[4] = bVar1;
      param_1[2] = param_1[2] ^ 0x57;
      bVar1 = param_1[1];
      param_1[1] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      param_1[6] = param_1[6] ^ 0x7c;
      bVar1 = param_1[8];
      param_1[8] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 2;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
    }
    else {
      param_1[0x18] = param_1[0x18] ^ 0x5a;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      param_1[6] = param_1[6] ^ 0xc4;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[0x12];
      if ((char)((int)(uint)param_1[0x12] >> 7) == '\0') {
        param_1[0x12] = param_1[0x12] & 0xbf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0x7f;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x80;
      }
      param_1[0xc] = param_1[0xc] ^ 0x85;
    }
  }
  return;
}



/* 00591b50 FUN_00591b50 */
/* Callers:
 * Callees:
 */

void FUN_00591b50(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0x37;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0x1f;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = param_1[5];
      param_1[5] = bVar1;
      param_1[2] = param_1[2] ^ 0x57;
      bVar1 = param_1[2];
      param_1[2] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      param_1[4] = param_1[4] ^ 0xd;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 0xdc;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 6;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[8];
      param_1[8] = param_1[7];
      param_1[7] = bVar1;
      param_1[6] = param_1[6] ^ 0x7c;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
    }
    else {
      param_1[0xc] = param_1[0xc] ^ 0x85;
      bVar1 = param_1[0x12];
      if ((char)((int)(uint)param_1[0x12] >> 7) == '\0') {
        param_1[0x12] = param_1[0x12] & 0xbf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0x7f;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x80;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      param_1[6] = param_1[6] ^ 0xc4;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      param_1[0x18] = param_1[0x18] ^ 0x5a;
    }
  }
  return;
}



/* 00592260 FUN_00592260 */
/* Callers:
 * Callees:
 */

void FUN_00592260(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      *param_1 = *param_1 ^ 0x14;
      param_1[3] = param_1[3] ^ 0x85;
      *param_1 = *param_1 ^ 5;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      param_1[6] = param_1[6] ^ 0xd5;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      param_1[3] = param_1[3] ^ 2;
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      param_1[1] = param_1[1] ^ 0xd1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      *param_1 = *param_1 ^ 0xd7;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 200;
    }
    else if (param_2 < 0x20) {
      param_1[4] = param_1[4] ^ 99;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      *param_1 = *param_1 ^ 0x58;
      bVar1 = param_1[6];
      param_1[6] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 7;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xe];
      param_1[0xe] = bVar1;
      param_1[1] = param_1[1] ^ 0xc5;
      param_1[1] = param_1[1] ^ 0x24;
    }
    else {
      bVar1 = param_1[0x1a];
      if (((int)(uint)param_1[0x1a] >> 3 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0x7f;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x1a] = param_1[0x1a] & 0xf7;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 8;
      }
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 4;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 4);
      param_1[0x10] = param_1[0x10] ^ 0xcd;
      bVar1 = param_1[8];
      param_1[8] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0x1e];
      if (((int)(uint)param_1[0x1e] >> 1 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfb;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfd;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 2;
      }
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x10];
      param_1[0x10] = bVar1;
    }
  }
  return;
}



/* 00592a30 FUN_00592a30 */
/* Callers:
 * Callees:
 */

void FUN_00592a30(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 5;
      param_1[3] = param_1[3] ^ 0x85;
      *param_1 = *param_1 ^ 0x14;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
    }
    else if (param_2 < 0x10) {
      param_1[1] = param_1[1] ^ 200;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      *param_1 = *param_1 ^ 0xd7;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      param_1[1] = param_1[1] ^ 0xd1;
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      param_1[3] = param_1[3] ^ 2;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      param_1[6] = param_1[6] ^ 0xd5;
    }
    else if (param_2 < 0x20) {
      param_1[1] = param_1[1] ^ 0x24;
      param_1[1] = param_1[1] ^ 0xc5;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 1;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[6];
      param_1[6] = param_1[0xd];
      param_1[0xd] = bVar1;
      *param_1 = *param_1 ^ 0x58;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      param_1[4] = param_1[4] ^ 99;
    }
    else {
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x10];
      param_1[0x10] = bVar1;
      bVar1 = param_1[0x1e];
      if (((int)(uint)param_1[0x1e] >> 1 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfb;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1e] = param_1[0x1e] & 0xfd;
      }
      else {
        param_1[0x1e] = param_1[0x1e] | 2;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[0x10] = param_1[0x10] ^ 0xcd;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 4;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x1a];
      if (((int)(uint)param_1[0x1a] >> 3 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0x7f;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x1a] = param_1[0x1a] & 0xf7;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 8;
      }
    }
  }
  return;
}



/* 00593200 FUN_00593200 */
/* Callers:
 * Callees:
 */

void FUN_00593200(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      param_1[1] = param_1[1] ^ 0x3a;
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      param_1[2] = param_1[2] ^ 0xaf;
      param_1[2] = param_1[2] ^ 0xc9;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      param_1[1] = param_1[1] ^ 99;
      param_1[1] = param_1[1] ^ 0xf1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 2;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[4];
      param_1[4] = bVar1;
      param_1[9] = param_1[9] ^ 0x66;
      bVar1 = param_1[6];
      param_1[6] = *param_1;
      *param_1 = bVar1;
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
    }
    else {
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xdf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      bVar1 = param_1[0x1f];
      if (((int)(uint)param_1[0x1f] >> 3 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xfb;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xf7;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 8;
      }
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 6;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1c];
      param_1[0x1c] = bVar1;
    }
  }
  return;
}



/* 005938d0 FUN_005938d0 */
/* Callers:
 * Callees:
 */

void FUN_005938d0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0xc9;
      param_1[2] = param_1[2] ^ 0xaf;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      param_1[1] = param_1[1] ^ 0x3a;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      param_1[1] = param_1[1] ^ 0xf1;
      param_1[1] = param_1[1] ^ 99;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x20) {
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
      bVar1 = param_1[6];
      param_1[6] = *param_1;
      *param_1 = bVar1;
      param_1[9] = param_1[9] ^ 0x66;
      bVar1 = param_1[6];
      param_1[6] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 6;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
    }
    else {
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 2;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x1f];
      if (((int)(uint)param_1[0x1f] >> 3 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xfb;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xf7;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 8;
      }
      bVar1 = param_1[0x12];
      if (((int)(uint)param_1[0x12] >> 1 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xdf;
      }
      else {
        param_1[0x12] = param_1[0x12] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x12] = param_1[0x12] & 0xfd;
      }
      else {
        param_1[0x12] = param_1[0x12] | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
    }
  }
  return;
}



/* 00593fa0 FUN_00593fa0 */
/* Callers:
 * Callees:
 */

void FUN_00593fa0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      param_1[3] = param_1[3] ^ 0x80;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[5];
      param_1[5] = bVar1;
      param_1[6] = param_1[6] ^ 0x4d;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfb;
      }
      else {
        param_1[0xc] = param_1[0xc] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      param_1[5] = param_1[5] ^ 0xad;
      param_1[0xf] = param_1[0xf] ^ 1;
      param_1[0xf] = param_1[0xf] ^ 0xb;
      param_1[7] = param_1[7] ^ 0xab;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 5;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
    }
    else {
      bVar1 = param_1[9];
      param_1[9] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[0x12];
      param_1[0x12] = bVar1;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 5;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 5;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 3);
      param_1[0x11] = param_1[0x11] ^ 0x4f;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
    }
  }
  return;
}



/* 00594690 FUN_00594690 */
/* Callers:
 * Callees:
 */

void FUN_00594690(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      param_1[3] = param_1[3] ^ 0x80;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      param_1[3] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      param_1[6] = param_1[6] ^ 0x4d;
      bVar1 = param_1[4];
      param_1[4] = param_1[5];
      param_1[5] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xe];
      param_1[0xe] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 3;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 5);
      param_1[7] = param_1[7] ^ 0xab;
      param_1[0xf] = param_1[0xf] ^ 0xb;
      param_1[0xf] = param_1[0xf] ^ 1;
      param_1[5] = param_1[5] ^ 0xad;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfb;
      }
      else {
        param_1[0xc] = param_1[0xc] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
    }
    else {
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0xf];
      param_1[0xf] = bVar1;
      param_1[0x11] = param_1[0x11] ^ 0x4f;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 3;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x11];
      param_1[0x11] = bVar1;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 3;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[0x12];
      param_1[0x12] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[6];
      param_1[6] = bVar1;
    }
  }
  return;
}



/* 00594d80 FUN_00594d80 */
/* Callers:
 * Callees:
 */

void FUN_00594d80(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0xdd;
      param_1[3] = param_1[3] ^ 0xe3;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      param_1[4] = param_1[4] ^ 0xe;
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      *param_1 = *param_1 ^ 0x66;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 4;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 4);
      param_1[2] = param_1[2] ^ 0x1e;
      bVar1 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = bVar1;
    }
    else if (param_2 < 0x20) {
      param_1[8] = param_1[8] ^ 0xcb;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 2;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xf7;
      }
      else {
        param_1[10] = param_1[10] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[0xf];
      param_1[0xf] = bVar1;
    }
    else {
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xe];
      param_1[0xe] = bVar1;
    }
  }
  return;
}



/* 005954d0 FUN_005954d0 */
/* Callers:
 * Callees:
 */

void FUN_005954d0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      param_1[3] = param_1[3] ^ 0xe3;
      *param_1 = *param_1 ^ 0xdd;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = bVar1;
      param_1[2] = param_1[2] ^ 0x1e;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 4;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 3 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xf7;
      }
      else {
        param_1[5] = param_1[5] | 8;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0x66;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      param_1[4] = param_1[4] ^ 0xe;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[5];
      param_1[5] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xf7;
      }
      else {
        param_1[10] = param_1[10] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 6;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 2);
      param_1[8] = param_1[8] ^ 0xcb;
    }
    else {
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[10];
      param_1[10] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
    }
  }
  return;
}



/* 00595c20 FUN_00595c20 */
/* Callers:
 * Callees:
 */

void FUN_00595c20(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 3;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 5);
      param_1[3] = param_1[3] ^ 0xed;
      bVar1 = param_1[7];
      param_1[7] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      param_1[4] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      param_1[3] = param_1[3] ^ 0x79;
      bVar1 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 5;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xf];
      param_1[0xf] = *param_1;
      *param_1 = bVar1;
      param_1[3] = param_1[3] ^ 0xe8;
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
    }
    else {
      param_1[3] = param_1[3] ^ 0x75;
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 1;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 5;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 3);
    }
  }
  return;
}



/* 005962f0 FUN_005962f0 */
/* Callers:
 * Callees:
 */

void FUN_005962f0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = bVar1;
      param_1[3] = param_1[3] ^ 0x79;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[4];
      param_1[4] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7];
      param_1[7] = bVar1;
      param_1[3] = param_1[3] ^ 0xed;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 5;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      param_1[3] = param_1[3] ^ 0xe8;
      bVar1 = param_1[0xf];
      param_1[0xf] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 3;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 2 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfb;
      }
      else {
        param_1[8] = param_1[8] | 4;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[8];
      param_1[8] = bVar1;
    }
    else {
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 3;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 7;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      param_1[3] = param_1[3] ^ 0x75;
    }
  }
  return;
}



/* 005969c0 FUN_005969c0 */
/* Callers:
 * Callees:
 */

void FUN_005969c0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      *param_1 = *param_1 ^ 0x1c;
      *param_1 = *param_1 ^ 0x8d;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xdf;
      }
      else {
        param_1[5] = param_1[5] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[2];
      param_1[2] = param_1[5];
      param_1[5] = bVar1;
      param_1[3] = param_1[3] ^ 0xb0;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[6];
      param_1[6] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0x7f;
      }
      else {
        param_1[10] = param_1[10] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 6 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xbf;
      }
      else {
        param_1[10] = param_1[10] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xbf;
      }
      else {
        param_1[10] = param_1[10] | 0x40;
      }
      param_1[0xe] = param_1[0xe] ^ 0x5e;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[5];
      param_1[5] = bVar1;
      param_1[8] = param_1[8] ^ 0x29;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
    }
    else {
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 7;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 7;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
    }
  }
  return;
}



/* 00597180 FUN_00597180 */
/* Callers:
 * Callees:
 */

void FUN_00597180(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0x8d;
      *param_1 = *param_1 ^ 0x1c;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      param_1[3] = param_1[3] ^ 0xb0;
      bVar1 = param_1[2];
      param_1[2] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xdf;
      }
      else {
        param_1[5] = param_1[5] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 4;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      *param_1 = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      param_1[8] = param_1[8] ^ 0x29;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[5];
      param_1[5] = bVar1;
      param_1[0xe] = param_1[0xe] ^ 0x5e;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 6 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xbf;
      }
      else {
        param_1[10] = param_1[10] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xbf;
      }
      else {
        param_1[10] = param_1[10] | 0x40;
      }
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0x7f;
      }
      else {
        param_1[10] = param_1[10] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
    }
    else {
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 1;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c] << 1;
      param_1[0x1c] = param_1[0x1c] | (byte)((int)(uint)bVar1 >> 7);
    }
  }
  return;
}



/* 00597940 FUN_00597940 */
/* Callers:
 * Callees:
 */

void FUN_00597940(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      param_1[3] = param_1[3] ^ 0x22;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0x93;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[4];
      if ((char)((int)(uint)param_1[4] >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = *param_1;
      *param_1 = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 7;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 1;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 7);
      param_1[10] = param_1[10] ^ 3;
      param_1[0xb] = param_1[0xb] ^ 0x4c;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[10] = param_1[10] ^ 0xa3;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 7;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 3;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[0xe];
      param_1[0xe] = bVar1;
    }
    else {
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 1 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfb;
      }
      else {
        param_1[0x18] = param_1[0x18] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfd;
      }
      else {
        param_1[0x18] = param_1[0x18] | 2;
      }
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x10];
      param_1[0x10] = bVar1;
    }
  }
  return;
}



/* 005980d0 FUN_005980d0 */
/* Callers:
 * Callees:
 */

void FUN_005980d0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      *param_1 = *param_1 ^ 0x93;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      param_1[3] = param_1[3] ^ 0x22;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      if ((char)((int)(uint)param_1[4] >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      param_1[1] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 5;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 1;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 7);
      param_1[10] = param_1[10] ^ 0xa3;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[0xb] = param_1[0xb] ^ 0x4c;
      param_1[10] = param_1[10] ^ 3;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 7;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 1;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[0xb];
      param_1[0xb] = bVar1;
    }
    else {
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x10];
      param_1[0x10] = bVar1;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xef;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      bVar1 = param_1[0x18];
      if (((int)(uint)param_1[0x18] >> 1 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfb;
      }
      else {
        param_1[0x18] = param_1[0x18] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x18] = param_1[0x18] & 0xfd;
      }
      else {
        param_1[0x18] = param_1[0x18] | 2;
      }
    }
  }
  return;
}



/* 00598870 FUN_00598870 */
/* Callers:
 * Callees:
 */

void FUN_00598870(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[2];
      param_1[2] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      param_1[2] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      param_1[5] = param_1[5] ^ 0xa1;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 2;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 5;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[5];
      param_1[5] = bVar1;
      param_1[5] = param_1[5] ^ 0xa4;
    }
    else {
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 5 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xdf;
      }
      else {
        param_1[10] = param_1[10] | 0x20;
      }
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x13] << 2;
      param_1[0x13] = param_1[0x13] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[0x18];
      param_1[0x18] = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0xbf;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 4;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 4);
    }
  }
  return;
}



/* 00598f60 FUN_00598f60 */
/* Callers:
 * Callees:
 */

void FUN_00598f60(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
    }
    else if (param_2 < 0x20) {
      param_1[5] = param_1[5] ^ 0xa4;
      bVar1 = param_1[6];
      param_1[6] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 3;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 6;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xf7;
      }
      else {
        param_1[0xd] = param_1[0xd] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      param_1[5] = param_1[5] ^ 0xa1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[0xb];
      param_1[0xb] = bVar1;
    }
    else {
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 4;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0xd];
      param_1[0xd] = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0xbf;
      bVar1 = param_1[6];
      param_1[6] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x13] << 6;
      param_1[0x13] = param_1[0x13] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 5 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xdf;
      }
      else {
        param_1[10] = param_1[10] | 0x20;
      }
    }
  }
  return;
}



/* 00599650 FUN_00599650 */
/* Callers:
 * Callees:
 */

void FUN_00599650(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 0xb6;
      param_1[2] = param_1[2] ^ 0xdf;
      *param_1 = *param_1 ^ 0xce;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      param_1[3] = param_1[3] ^ 0x1e;
    }
    else if (param_2 < 0x10) {
      param_1[3] = param_1[3] ^ 0xa1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 2;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 2;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 1;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[3];
      param_1[3] = param_1[0xf];
      param_1[0xf] = bVar1;
    }
    else {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 4;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 4);
      *param_1 = *param_1 ^ 0x4c;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 2;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0xd] = param_1[0xd] ^ 0x69;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 6;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0xe] = param_1[0xe] ^ 0x91;
      param_1[0x10] = param_1[0x10] ^ 0x39;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x10];
      param_1[0x10] = bVar1;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 4;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 4);
    }
  }
  return;
}



/* 0059a100 FUN_0059a100 */
/* Callers:
 * Callees:
 */

void FUN_0059a100(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0x1e;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      *param_1 = *param_1 ^ 0xce;
      param_1[2] = param_1[2] ^ 0xdf;
      param_1[3] = param_1[3] ^ 0xb6;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 2 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfb;
      }
      else {
        param_1[5] = param_1[5] | 4;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      param_1[3] = param_1[3] ^ 0xa1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[3];
      param_1[3] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 7;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 6;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xfd;
      }
      else {
        param_1[0xf] = param_1[0xf] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 6;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 2);
    }
    else {
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 4;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x10];
      param_1[0x10] = bVar1;
      param_1[0x10] = param_1[0x10] ^ 0x39;
      param_1[0xe] = param_1[0xe] ^ 0x91;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x15] << 2;
      param_1[0x15] = param_1[0x15] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      param_1[0xd] = param_1[0xd] ^ 0x69;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 6;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      *param_1 = *param_1 ^ 0x4c;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 4;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 4);
    }
  }
  return;
}



/* 0059aba0 FUN_0059aba0 */
/* Callers:
 * Callees:
 */

void FUN_0059aba0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      *param_1 = *param_1 ^ 3;
      param_1[1] = param_1[1] ^ 0x50;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0xc5;
      param_1[3] = param_1[3] ^ 0xfe;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      *param_1 = *param_1 ^ 0x69;
      bVar1 = param_1[6];
      param_1[6] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      param_1[6] = param_1[6] ^ 0x2e;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      *param_1 = *param_1 ^ 0xcd;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 5 & 1U) == 0) {
        param_1[10] = param_1[10] & 0x7f;
      }
      else {
        param_1[10] = param_1[10] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[10] = param_1[10] & 0xdf;
      }
      else {
        param_1[10] = param_1[10] | 0x20;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      param_1[1] = param_1[1] ^ 0xe4;
      param_1[0xb] = param_1[0xb] ^ 0x38;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 6 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xbf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x40;
      }
      param_1[1] = param_1[1] ^ 0x9a;
      param_1[8] = param_1[8] ^ 0x9b;
      param_1[0xd] = param_1[0xd] ^ 0x44;
      bVar1 = param_1[5];
      param_1[5] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
    }
    else {
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 1 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfd;
      }
      else {
        param_1[0x17] = param_1[0x17] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfd;
      }
      else {
        param_1[0x17] = param_1[0x17] | 2;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x19];
      param_1[0x19] = bVar1;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0xb];
      param_1[0xb] = bVar1;
    }
  }
  return;
}



/* 0059b560 FUN_0059b560 */
/* Callers:
 * Callees:
 */

void FUN_0059b560(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      param_1[3] = param_1[3] ^ 0xfe;
      *param_1 = *param_1 ^ 0xc5;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      param_1[1] = param_1[1] ^ 0x50;
      *param_1 = *param_1 ^ 3;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      *param_1 = *param_1 ^ 0xcd;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[6] = param_1[6] ^ 0x2e;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[6];
      param_1[6] = param_1[5];
      param_1[5] = bVar1;
      *param_1 = *param_1 ^ 0x69;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[8];
      param_1[8] = bVar1;
      param_1[0xd] = param_1[0xd] ^ 0x44;
      param_1[8] = param_1[8] ^ 0x9b;
      param_1[1] = param_1[1] ^ 0x9a;
      bVar1 = param_1[0xf];
      if (((int)(uint)param_1[0xf] >> 6 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xef;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0xbf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x40;
      }
      param_1[0xb] = param_1[0xb] ^ 0x38;
      param_1[1] = param_1[1] ^ 0xe4;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 5 & 1U) == 0) {
        param_1[10] = param_1[10] & 0x7f;
      }
      else {
        param_1[10] = param_1[10] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[10] = param_1[10] & 0xdf;
      }
      else {
        param_1[10] = param_1[10] | 0x20;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 5 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xdf;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x20;
      }
    }
    else {
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0xb];
      param_1[0xb] = bVar1;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x19];
      param_1[0x19] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 1 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfd;
      }
      else {
        param_1[0x17] = param_1[0x17] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfd;
      }
      else {
        param_1[0x17] = param_1[0x17] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[8];
      param_1[8] = bVar1;
    }
  }
  return;
}



/* 0059bf20 FUN_0059bf20 */
/* Callers:
 * Callees:
 */

void FUN_0059bf20(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[1] = param_1[1] ^ 0x68;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      *param_1 = *param_1 ^ 0x89;
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[5];
      if ((char)((int)(uint)param_1[5] >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[10];
      param_1[10] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      param_1[0xe] = param_1[0xe] ^ 0x8b;
    }
    else {
      param_1[0x1c] = param_1[0x1c] ^ 4;
      param_1[0x1a] = param_1[0x1a] ^ 9;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
    }
  }
  return;
}



/* 0059c440 FUN_0059c440 */
/* Callers:
 * Callees:
 */

void FUN_0059c440(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      *param_1 = *param_1 ^ 0x89;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      param_1[1] = param_1[1] ^ 0x68;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[5];
      if ((char)((int)(uint)param_1[5] >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
    }
    else if (param_2 < 0x20) {
      param_1[0xe] = param_1[0xe] ^ 0x8b;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[6];
      param_1[6] = bVar1;
    }
    else {
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0x15];
      param_1[0x15] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      param_1[0x1a] = param_1[0x1a] ^ 9;
      param_1[0x1c] = param_1[0x1c] ^ 4;
    }
  }
  return;
}



/* 0059c960 FUN_0059c960 */
/* Callers:
 * Callees:
 */

void FUN_0059c960(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      param_1[1] = param_1[1] ^ 0xd8;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
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
      param_1[1] = param_1[1] ^ 0xc0;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 7;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[7];
      param_1[7] = bVar1;
      param_1[1] = param_1[1] ^ 0xf0;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      param_1[7] = param_1[7] ^ 0x76;
      param_1[1] = param_1[1] ^ 0x82;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4];
      param_1[4] = bVar1;
      *param_1 = *param_1 ^ 0xce;
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[10];
      param_1[10] = bVar1;
      param_1[4] = param_1[4] ^ 0x2c;
      param_1[0xf] = param_1[0xf] ^ 0xfe;
      param_1[0xc] = param_1[0xc] ^ 0x9c;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 4 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xef;
      }
      else {
        param_1[10] = param_1[10] | 0x10;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      param_1[4] = param_1[4] ^ 0xd8;
      param_1[0xb] = param_1[0xb] ^ 0x84;
      param_1[4] = param_1[4] ^ 0xc2;
    }
    else {
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 2;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      param_1[5] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 1 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xdf;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xfd;
      }
      else {
        param_1[0x11] = param_1[0x11] | 2;
      }
      param_1[0xc] = param_1[0xc] ^ 0x74;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
    }
  }
  return;
}



/* 0059d220 FUN_0059d220 */
/* Callers:
 * Callees:
 */

void FUN_0059d220(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      param_1[1] = param_1[1] ^ 0xc0;
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
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      param_1[1] = param_1[1] ^ 0xd8;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      *param_1 = *param_1 ^ 0xce;
      bVar1 = param_1[4];
      param_1[4] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 0x82;
      param_1[7] = param_1[7] ^ 0x76;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      param_1[1] = param_1[1] ^ 0xf0;
      bVar1 = *param_1;
      *param_1 = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 1;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x20) {
      param_1[4] = param_1[4] ^ 0xc2;
      param_1[0xb] = param_1[0xb] ^ 0x84;
      param_1[4] = param_1[4] ^ 0xd8;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0xfd;
      }
      else {
        param_1[0xc] = param_1[0xc] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 4 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xef;
      }
      else {
        param_1[10] = param_1[10] | 0x10;
      }
      param_1[0xc] = param_1[0xc] ^ 0x9c;
      param_1[0xf] = param_1[0xf] ^ 0xfe;
      param_1[4] = param_1[4] ^ 0x2c;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[10];
      param_1[10] = bVar1;
    }
    else {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[10];
      param_1[10] = bVar1;
      param_1[0xc] = param_1[0xc] ^ 0x74;
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 1 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xdf;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xfd;
      }
      else {
        param_1[0x11] = param_1[0x11] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[0x17];
      param_1[0x17] = bVar1;
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 6;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 2);
    }
  }
  return;
}



/* 0059dae0 FUN_0059dae0 */
/* Callers:
 * Callees:
 */

void FUN_0059dae0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0x62;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      param_1[1] = param_1[1] ^ 0x35;
      param_1[3] = param_1[3] ^ 0x37;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      param_1[4] = param_1[4] ^ 0x7a;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      *param_1 = *param_1 ^ 0x65;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      param_1[7] = param_1[7] ^ 0x82;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      param_1[6] = param_1[6] ^ 0x72;
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x20) {
      param_1[4] = param_1[4] ^ 0x82;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[1];
      param_1[1] = bVar1;
      param_1[10] = param_1[10] ^ 8;
      param_1[4] = param_1[4] ^ 0x15;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 3;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 5);
      param_1[7] = param_1[7] ^ 0x41;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 2;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
    }
    else {
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xdf;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 1;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 6;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 2);
      param_1[10] = param_1[10] ^ 0x88;
      bVar1 = param_1[9];
      param_1[9] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x1a];
      if (((int)(uint)param_1[0x1a] >> 2 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0xef;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0xfb;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 4;
      }
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      bVar1 = param_1[0x1f];
      if (((int)(uint)param_1[0x1f] >> 6 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xbf;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xbf;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 0x40;
      }
      param_1[5] = param_1[5] ^ 0x98;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 2;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 2;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 6);
    }
  }
  return;
}



/* 0059e4a0 FUN_0059e4a0 */
/* Callers:
 * Callees:
 */

void FUN_0059e4a0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      param_1[3] = param_1[3] ^ 0x37;
      param_1[1] = param_1[1] ^ 0x35;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      param_1[3] = param_1[3] ^ 0x62;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      param_1[6] = param_1[6] ^ 0x72;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      param_1[7] = param_1[7] ^ 0x82;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      *param_1 = *param_1 ^ 0x65;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      param_1[4] = param_1[4] ^ 0x7a;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 6;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 2);
      param_1[7] = param_1[7] ^ 0x41;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 5;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      param_1[4] = param_1[4] ^ 0x15;
      param_1[10] = param_1[10] ^ 8;
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xf7;
      }
      else {
        param_1[4] = param_1[4] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xe];
      param_1[0xe] = bVar1;
      param_1[4] = param_1[4] ^ 0x82;
    }
    else {
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x17] << 6;
      param_1[0x17] = param_1[0x17] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 6;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 2);
      param_1[5] = param_1[5] ^ 0x98;
      bVar1 = param_1[0x1f];
      if (((int)(uint)param_1[0x1f] >> 6 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xbf;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x1f] = param_1[0x1f] & 0xbf;
      }
      else {
        param_1[0x1f] = param_1[0x1f] | 0x40;
      }
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x18];
      param_1[0x18] = bVar1;
      bVar1 = param_1[0x1a];
      if (((int)(uint)param_1[0x1a] >> 2 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0xef;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0xfb;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 4;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[3];
      param_1[3] = bVar1;
      param_1[10] = param_1[10] ^ 0x88;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 2;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 7;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 2 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xdf;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xfb;
      }
      else {
        param_1[0x17] = param_1[0x17] | 4;
      }
    }
  }
  return;
}



/* 0059ee70 FUN_0059ee70 */
/* Callers:
 * Callees:
 */

void FUN_0059ee70(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      param_1[1] = param_1[1] ^ 0x98;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      param_1[3] = param_1[3] ^ 0xda;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x10) {
      param_1[3] = param_1[3] ^ 0x90;
      bVar1 = param_1[4];
      if ((char)((int)(uint)param_1[4] >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[8];
      param_1[8] = param_1[0xb];
      param_1[0xb] = bVar1;
      param_1[6] = param_1[6] ^ 0xdf;
      param_1[3] = param_1[3] ^ 0x78;
      bVar1 = param_1[5];
      param_1[5] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[0xe];
      param_1[0xe] = bVar1;
      param_1[1] = param_1[1] ^ 0xe5;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 2;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 6);
    }
    else {
      param_1[1] = param_1[1] ^ 0xdd;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 1;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 7);
      param_1[7] = param_1[7] ^ 9;
      param_1[0x1c] = param_1[0x1c] ^ 0x15;
      bVar1 = param_1[3];
      param_1[3] = param_1[0x19];
      param_1[0x19] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[0x14];
      param_1[0x14] = bVar1;
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0xf];
      param_1[0xf] = bVar1;
    }
  }
  return;
}



/* 0059f590 FUN_0059f590 */
/* Callers:
 * Callees:
 */

void FUN_0059f590(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      param_1[3] = param_1[3] ^ 0xda;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      param_1[1] = param_1[1] ^ 0x98;
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      if ((char)((int)(uint)param_1[4] >> 7) == '\0') {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0x7f;
      }
      else {
        param_1[4] = param_1[4] | 0x80;
      }
      param_1[3] = param_1[3] ^ 0x90;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 6;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      param_1[1] = param_1[1] ^ 0xe5;
      bVar1 = param_1[7];
      param_1[7] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[0xc];
      param_1[0xc] = bVar1;
      param_1[3] = param_1[3] ^ 0x78;
      param_1[6] = param_1[6] ^ 0xdf;
      bVar1 = param_1[8];
      param_1[8] = param_1[0xb];
      param_1[0xb] = bVar1;
    }
    else {
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0xf];
      param_1[0xf] = bVar1;
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = param_1[0x14];
      param_1[0x14] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[0x19];
      param_1[0x19] = bVar1;
      param_1[0x1c] = param_1[0x1c] ^ 0x15;
      param_1[7] = param_1[7] ^ 9;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 7;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 1);
      param_1[1] = param_1[1] ^ 0xdd;
    }
  }
  return;
}



/* 0059fcb0 FUN_0059fcb0 */
/* Callers:
 * Callees:
 */

void FUN_0059fcb0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x10) {
      param_1[7] = param_1[7] ^ 0x9e;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 5 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xdf;
      }
      else {
        param_1[4] = param_1[4] | 0x20;
      }
      bVar1 = param_1[6];
      param_1[6] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[5];
      param_1[5] = bVar1;
      param_1[1] = param_1[1] ^ 199;
      param_1[7] = param_1[7] ^ 0x38;
      bVar1 = param_1[7];
      param_1[7] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[7];
      if ((char)((int)(uint)param_1[7] >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      param_1[1] = param_1[1] ^ 0xfb;
      bVar1 = *param_1;
      *param_1 = param_1[4];
      param_1[4] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[8];
      param_1[8] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      bVar1 = *param_1;
      *param_1 = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[0xe];
      param_1[0xe] = bVar1;
    }
    else {
      param_1[5] = param_1[5] ^ 0xb2;
      bVar1 = param_1[2];
      param_1[2] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 6;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 2);
      *param_1 = *param_1 ^ 0xb7;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      param_1[0x19] = param_1[0x19] ^ 0x69;
      param_1[1] = param_1[1] ^ 0x6e;
      param_1[0x12] = param_1[0x12] ^ 1;
    }
  }
  return;
}



/* 005a0620 FUN_005a0620 */
/* Callers:
 * Callees:
 */

void FUN_005a0620(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[2];
      if ((char)((int)(uint)param_1[2] >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      *param_1 = param_1[4];
      param_1[4] = bVar1;
      param_1[1] = param_1[1] ^ 0xfb;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[7];
      if ((char)((int)(uint)param_1[7] >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7];
      param_1[7] = bVar1;
      param_1[7] = param_1[7] ^ 0x38;
      param_1[1] = param_1[1] ^ 199;
      bVar1 = param_1[7];
      param_1[7] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 5 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfb;
      }
      else {
        param_1[4] = param_1[4] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xdf;
      }
      else {
        param_1[4] = param_1[4] | 0x20;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      param_1[7] = param_1[7] ^ 0x9e;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      param_1[2] = param_1[0xe];
      param_1[0xe] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xf7;
      }
      else {
        param_1[6] = param_1[6] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
    }
    else {
      param_1[0x12] = param_1[0x12] ^ 1;
      param_1[1] = param_1[1] ^ 0x6e;
      param_1[0x19] = param_1[0x19] ^ 0x69;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      *param_1 = *param_1 ^ 0xb7;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 2;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[5];
      param_1[5] = bVar1;
      param_1[5] = param_1[5] ^ 0xb2;
    }
  }
  return;
}



/* 005a0f80 FUN_005a0f80 */
/* Callers:
 * Callees:
 */

void FUN_005a0f80(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0x61;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      param_1[3] = param_1[3] ^ 0xe1;
      param_1[3] = param_1[3] ^ 0x33;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      param_1[1] = param_1[1] ^ 0xb2;
      param_1[4] = param_1[4] ^ 0xbd;
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7];
      param_1[7] = bVar1;
      *param_1 = *param_1 ^ 0xf4;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      param_1[1] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
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
    }
    else if (param_2 < 0x20) {
      *param_1 = *param_1 ^ 0xbe;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 5;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xe];
      if ((char)((int)(uint)param_1[0xe] >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 1;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xef;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
    }
    else {
      param_1[0x19] = param_1[0x19] ^ 0xda;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1f];
      param_1[0x1f] = bVar1;
    }
  }
  return;
}



/* 005a1530 FUN_005a1530 */
/* Callers:
 * Callees:
 */

void FUN_005a1530(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 1 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfd;
      }
      else {
        param_1[3] = param_1[3] | 2;
      }
      param_1[3] = param_1[3] ^ 0x33;
      param_1[3] = param_1[3] ^ 0xe1;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 0x61;
    }
    else if (param_2 < 0x10) {
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
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      *param_1 = *param_1 ^ 0xf4;
      bVar1 = param_1[7];
      param_1[7] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      param_1[4] = param_1[4] ^ 0xbd;
      param_1[1] = param_1[1] ^ 0xb2;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[4];
      param_1[4] = param_1[5];
      param_1[5] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 1 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xef;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfd;
      }
      else {
        param_1[0xd] = param_1[0xd] | 2;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 7;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0xe];
      if ((char)((int)(uint)param_1[0xe] >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 3;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[9];
      param_1[9] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[5];
      param_1[5] = bVar1;
      *param_1 = *param_1 ^ 0xbe;
    }
    else {
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1f];
      param_1[0x1f] = bVar1;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[3];
      param_1[3] = bVar1;
      param_1[0x19] = param_1[0x19] ^ 0xda;
    }
  }
  return;
}



/* 005a1ae0 FUN_005a1ae0 */
/* Callers:
 * Callees:
 */

void FUN_005a1ae0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      *param_1 = *param_1 ^ 0x7f;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      param_1[1] = param_1[1] ^ 0x6e;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[6];
      param_1[6] = bVar1;
      param_1[6] = param_1[6] ^ 0xb4;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      param_1[3] = param_1[3] ^ 0x41;
    }
    else if (param_2 < 0x20) {
      param_1[0xf] = param_1[0xf] ^ 0x65;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 6;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xe];
      if ((char)((int)(uint)param_1[0xe] >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 2;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 1 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xfd;
      }
      else {
        param_1[0xb] = param_1[0xb] | 2;
      }
    }
    else {
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 2;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 2;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 6);
      param_1[3] = param_1[3] ^ 0x47;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[3];
      param_1[3] = bVar1;
      param_1[10] = param_1[10] ^ 0xe0;
      bVar1 = param_1[0xe];
      if ((char)((int)(uint)param_1[0xe] >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      param_1[0x13] = param_1[0x13] ^ 0x3f;
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 3;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 5);
    }
  }
  return;
}



/* 005a25a0 FUN_005a25a0 */
/* Callers:
 * Callees:
 */

void FUN_005a25a0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      param_1[1] = param_1[1] ^ 0x6e;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      *param_1 = *param_1 ^ 0x7f;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x10) {
      param_1[3] = param_1[3] ^ 0x41;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      param_1[6] = param_1[6] ^ 0xb4;
      bVar1 = param_1[3];
      param_1[3] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfd;
      }
      else {
        param_1[6] = param_1[6] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 1 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0x7f;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xb] = param_1[0xb] & 0xfd;
      }
      else {
        param_1[0xb] = param_1[0xb] | 2;
      }
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 6;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0xe];
      if ((char)((int)(uint)param_1[0xe] >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[0xd] << 2;
      param_1[0xd] = param_1[0xd] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[10];
      param_1[10] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[8];
      param_1[8] = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0x65;
    }
    else {
      bVar1 = param_1[0x18];
      param_1[0x18] = param_1[0x18] << 5;
      param_1[0x18] = param_1[0x18] | (byte)((int)(uint)bVar1 >> 3);
      param_1[0x13] = param_1[0x13] ^ 0x3f;
      bVar1 = param_1[0xe];
      if ((char)((int)(uint)param_1[0xe] >> 7) == '\0') {
        param_1[0xe] = param_1[0xe] & 0xdf;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0x7f;
      }
      else {
        param_1[0xe] = param_1[0xe] | 0x80;
      }
      param_1[10] = param_1[10] ^ 0xe0;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[0x1d];
      param_1[0x1d] = bVar1;
      param_1[3] = param_1[3] ^ 0x47;
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 6;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 6;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 2);
    }
  }
  return;
}



/* 005a3070 FUN_005a3070 */
/* Callers:
 * Callees:
 */

void FUN_005a3070(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[3] = param_1[3] ^ 0xb3;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      param_1[4] = param_1[4] ^ 0xd5;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[7];
      param_1[7] = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 1;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xef;
      }
      else {
        param_1[8] = param_1[8] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      bVar1 = param_1[7];
      if ((char)((int)(uint)param_1[7] >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      param_1[9] = param_1[9] ^ 0x26;
      param_1[0xd] = param_1[0xd] ^ 0xda;
      *param_1 = *param_1 ^ 0xb2;
    }
    else {
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 7;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[2];
      param_1[2] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      param_1[0x1a] = param_1[0x1a] ^ 0xfd;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 2;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x13];
      if (((int)(uint)param_1[0x13] >> 1 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xbf;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xfd;
      }
      else {
        param_1[0x13] = param_1[0x13] | 2;
      }
    }
  }
  return;
}



/* 005a3760 FUN_005a3760 */
/* Callers:
 * Callees:
 */

void FUN_005a3760(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xbf;
      }
      else {
        *param_1 = *param_1 | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      param_1[3] = param_1[3] ^ 0xb3;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xef;
      }
      else {
        *param_1 = *param_1 | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      param_1[4] = param_1[4] ^ 0xd5;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x20) {
      *param_1 = *param_1 ^ 0xb2;
      param_1[0xd] = param_1[0xd] ^ 0xda;
      param_1[9] = param_1[9] ^ 0x26;
      bVar1 = param_1[7];
      if ((char)((int)(uint)param_1[7] >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      bVar1 = param_1[8];
      if (((int)(uint)param_1[8] >> 6 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xef;
      }
      else {
        param_1[8] = param_1[8] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xbf;
      }
      else {
        param_1[8] = param_1[8] | 0x40;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 7;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xf7;
      }
      else {
        param_1[1] = param_1[1] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
    }
    else {
      bVar1 = param_1[0x13];
      if (((int)(uint)param_1[0x13] >> 1 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xbf;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0xfd;
      }
      else {
        param_1[0x13] = param_1[0x13] | 2;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[0x16] << 6;
      param_1[0x16] = param_1[0x16] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0x1a] = param_1[0x1a] ^ 0xfd;
      bVar1 = param_1[2];
      param_1[2] = param_1[0x1e];
      param_1[0x1e] = bVar1;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 1;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
    }
  }
  return;
}



/* 005a3e60 FUN_005a3e60 */
/* Callers:
 * Callees:
 */

void FUN_005a3e60(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
      param_1[1] = param_1[1] ^ 0x26;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      param_1[2] = param_1[2] ^ 0xe2;
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      param_1[4] = param_1[4] ^ 0x32;
      bVar1 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      param_1[4] = param_1[4] ^ 0xb;
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      param_1[2] = param_1[2] ^ 0x23;
      bVar1 = param_1[7];
      param_1[7] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 4 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xef;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x10;
      }
    }
    else {
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 4;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 2;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 6);
      param_1[0x17] = param_1[0x17] ^ 0x66;
      param_1[0x18] = param_1[0x18] ^ 0xe8;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[0x1a] << 6;
      param_1[0x1a] = param_1[0x1a] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x10];
      param_1[0x10] = bVar1;
      bVar1 = param_1[0x1d];
      if (((int)(uint)param_1[0x1d] >> 3 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xef;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xf7;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 8;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 6;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 2);
    }
  }
  return;
}



/* 005a45f0 FUN_005a45f0 */
/* Callers:
 * Callees:
 */

void FUN_005a45f0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      param_1[1] = param_1[1] ^ 0x26;
      bVar1 = param_1[2];
      param_1[2] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[4];
      param_1[4] = bVar1;
      param_1[4] = param_1[4] ^ 0x32;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6];
      param_1[6] = bVar1;
      param_1[2] = param_1[2] ^ 0xe2;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xd];
      if (((int)(uint)param_1[0xd] >> 4 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xfb;
      }
      else {
        param_1[0xd] = param_1[0xd] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xd] = param_1[0xd] & 0xef;
      }
      else {
        param_1[0xd] = param_1[0xd] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7];
      param_1[7] = bVar1;
      param_1[2] = param_1[2] ^ 0x23;
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      param_1[4] = param_1[4] ^ 0xb;
    }
    else {
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 2;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x1d];
      if (((int)(uint)param_1[0x1d] >> 3 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xef;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xf7;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 8;
      }
      bVar1 = param_1[0x11];
      param_1[0x11] = param_1[0x10];
      param_1[0x10] = bVar1;
      bVar1 = param_1[0x16];
      param_1[0x16] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[0x1a] << 2;
      param_1[0x1a] = param_1[0x1a] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfb;
      }
      else {
        param_1[2] = param_1[2] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      param_1[0x18] = param_1[0x18] ^ 0xe8;
      param_1[0x17] = param_1[0x17] ^ 0x66;
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 6;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 4;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
    }
  }
  return;
}



/* 005a4d80 FUN_005a4d80 */
/* Callers:
 * Callees:
 */

void FUN_005a4d80(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 0x48;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      param_1[3] = param_1[3] ^ 0x65;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      param_1[3] = param_1[3] ^ 0xc;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
    }
    else if (param_2 < 0x20) {
      param_1[1] = param_1[1] ^ 0x7a;
      bVar1 = param_1[8];
      param_1[8] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xf7;
      }
      else {
        param_1[0xe] = param_1[0xe] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 4 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xef;
      }
      else {
        param_1[10] = param_1[10] | 0x10;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8];
      param_1[8] = bVar1;
      param_1[4] = param_1[4] ^ 0x53;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xf7;
      }
      else {
        param_1[10] = param_1[10] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      param_1[1] = param_1[1] ^ 0x74;
    }
    else {
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 3 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xef;
      }
      else {
        param_1[9] = param_1[9] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xf7;
      }
      else {
        param_1[9] = param_1[9] | 8;
      }
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 5;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14] << 2;
      param_1[0x14] = param_1[0x14] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 3 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xdf;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xf7;
      }
      else {
        param_1[0x11] = param_1[0x11] | 8;
      }
      param_1[0xb] = param_1[0xb] ^ 0xbd;
    }
  }
  return;
}



/* 005a54e0 FUN_005a54e0 */
/* Callers:
 * Callees:
 */

void FUN_005a54e0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xfb;
      }
      else {
        param_1[3] = param_1[3] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      param_1[3] = param_1[3] ^ 0x65;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      param_1[3] = param_1[3] ^ 0x48;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[2];
      param_1[2] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      param_1[3] = param_1[3] ^ 0xc;
    }
    else if (param_2 < 0x20) {
      param_1[1] = param_1[1] ^ 0x74;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xf7;
      }
      else {
        param_1[10] = param_1[10] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      param_1[4] = param_1[4] ^ 0x53;
      bVar1 = param_1[8];
      param_1[8] = param_1[8];
      param_1[8] = bVar1;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 4 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xef;
      }
      else {
        param_1[10] = param_1[10] | 0x10;
      }
      bVar1 = param_1[0xe];
      if (((int)(uint)param_1[0xe] >> 1 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xf7;
      }
      else {
        param_1[0xe] = param_1[0xe] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xe] = param_1[0xe] & 0xfd;
      }
      else {
        param_1[0xe] = param_1[0xe] | 2;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[5];
      param_1[5] = bVar1;
      param_1[1] = param_1[1] ^ 0x7a;
    }
    else {
      param_1[0xb] = param_1[0xb] ^ 0xbd;
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 3 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xdf;
      }
      else {
        param_1[0x11] = param_1[0x11] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xf7;
      }
      else {
        param_1[0x11] = param_1[0x11] | 8;
      }
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14] << 6;
      param_1[0x14] = param_1[0x14] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 3;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[9];
      if (((int)(uint)param_1[9] >> 3 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xef;
      }
      else {
        param_1[9] = param_1[9] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xf7;
      }
      else {
        param_1[9] = param_1[9] | 8;
      }
    }
  }
  return;
}



/* 005a5c50 FUN_005a5c50 */
/* Callers:
 * Callees:
 */

void FUN_005a5c50(int param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *(byte *)(param_1 + 3);
      *(char *)(param_1 + 3) = *(char *)(param_1 + 3) << 6;
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *(byte *)(param_1 + 1);
      if ((char)((int)(uint)*(byte *)(param_1 + 1) >> 7) == '\0') {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) & 0xdf;
      }
      else {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) & 0x7f;
      }
      else {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | 0x80;
      }
      bVar1 = *(byte *)(param_1 + 2);
      *(char *)(param_1 + 2) = *(char *)(param_1 + 2) << 4;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 5;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *(byte *)(param_1 + 2);
      if (((int)(uint)*(byte *)(param_1 + 2) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) & 0xfd;
      }
      else {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) & 0xef;
      }
      else {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | 0x10;
      }
      bVar1 = *(byte *)(param_1 + 2);
      *(char *)(param_1 + 2) = *(char *)(param_1 + 2) << 7;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x10) {
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 1;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *(byte *)(param_1 + 3);
      *(char *)(param_1 + 3) = *(char *)(param_1 + 3) << 2;
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 2;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 6);
      *(byte *)(param_1 + 7) = *(byte *)(param_1 + 7) ^ 199;
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 4;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *(byte *)(param_1 + 3);
      if (((int)(uint)*(byte *)(param_1 + 3) >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | 4;
      }
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 3;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *(byte *)(param_1 + 3);
      if (((int)(uint)*(byte *)(param_1 + 3) >> 3 & 1U) == 0) {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) & 0xf7;
      }
      else {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | 8;
      }
      uVar2 = *(undefined1 *)(param_1 + 5);
      *(undefined1 *)(param_1 + 5) = *(undefined1 *)(param_1 + 5);
      *(undefined1 *)(param_1 + 5) = uVar2;
      bVar1 = *(byte *)(param_1 + 2);
      *(char *)(param_1 + 2) = *(char *)(param_1 + 2) << 7;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | (byte)((int)(uint)bVar1 >> 1);
      uVar2 = *(undefined1 *)(param_1 + 1);
      *(undefined1 *)(param_1 + 1) = *(undefined1 *)(param_1 + 3);
      *(undefined1 *)(param_1 + 3) = uVar2;
      *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) ^ 0x5f;
      *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) ^ 0xd9;
    }
    else if (param_2 < 0x20) {
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 7;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *(byte *)(param_1 + 5);
      if (((int)(uint)*(byte *)(param_1 + 5) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) & 0xef;
      }
      else {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) | 0x10;
      }
      bVar1 = *(byte *)(param_1 + 8);
      *(char *)(param_1 + 8) = *(char *)(param_1 + 8) << 3;
      *(byte *)(param_1 + 8) = *(byte *)(param_1 + 8) | (byte)((int)(uint)bVar1 >> 5);
      uVar2 = *(undefined1 *)(param_1 + 0xf);
      *(undefined1 *)(param_1 + 0xf) = *(undefined1 *)(param_1 + 9);
      *(undefined1 *)(param_1 + 9) = uVar2;
      *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) ^ 0x58;
      *(byte *)(param_1 + 8) = *(byte *)(param_1 + 8) ^ 0xe4;
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) ^ 0x21;
      bVar1 = *(byte *)(param_1 + 2);
      if ((char)((int)(uint)*(byte *)(param_1 + 2) >> 7) == '\0') {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) & 0xfd;
      }
      else {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) & 0x7f;
      }
      else {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | 0x80;
      }
      uVar2 = *(undefined1 *)(param_1 + 3);
      *(undefined1 *)(param_1 + 3) = *(undefined1 *)(param_1 + 8);
      *(undefined1 *)(param_1 + 8) = uVar2;
      bVar1 = *(byte *)(param_1 + 6);
      *(char *)(param_1 + 6) = *(char *)(param_1 + 6) << 2;
      *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | (byte)((int)(uint)bVar1 >> 6);
    }
    else {
      bVar1 = *(byte *)(param_1 + 9);
      *(char *)(param_1 + 9) = *(char *)(param_1 + 9) << 6;
      *(byte *)(param_1 + 9) = *(byte *)(param_1 + 9) | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = *(undefined1 *)(param_1 + 0xb);
      *(undefined1 *)(param_1 + 0xb) = *(undefined1 *)(param_1 + 6);
      *(undefined1 *)(param_1 + 6) = uVar2;
      *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) ^ 0xf9;
      *(byte *)(param_1 + 0x1a) = *(byte *)(param_1 + 0x1a) ^ 0xb2;
      uVar2 = *(undefined1 *)(param_1 + 0x1f);
      *(undefined1 *)(param_1 + 0x1f) = *(undefined1 *)(param_1 + 6);
      *(undefined1 *)(param_1 + 6) = uVar2;
      uVar2 = *(undefined1 *)(param_1 + 1);
      *(undefined1 *)(param_1 + 1) = *(undefined1 *)(param_1 + 0x1b);
      *(undefined1 *)(param_1 + 0x1b) = uVar2;
      bVar1 = *(byte *)(param_1 + 0x11);
      if (((int)(uint)*(byte *)(param_1 + 0x11) >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) & 0x7f;
      }
      else {
        *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) | 0x40;
      }
      bVar1 = *(byte *)(param_1 + 6);
      if (((int)(uint)*(byte *)(param_1 + 6) >> 3 & 1U) == 0) {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) & 0xf7;
      }
      else {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) & 0xf7;
      }
      else {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | 8;
      }
      uVar2 = *(undefined1 *)(param_1 + 0x10);
      *(undefined1 *)(param_1 + 0x10) = *(undefined1 *)(param_1 + 9);
      *(undefined1 *)(param_1 + 9) = uVar2;
      *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) ^ 0x4b;
    }
  }
  return;
}



/* 005a6560 FUN_005a6560 */
/* Callers:
 * Callees:
 */

void FUN_005a6560(int param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *(byte *)(param_1 + 2);
      *(char *)(param_1 + 2) = *(char *)(param_1 + 2) << 1;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *(byte *)(param_1 + 2);
      if (((int)(uint)*(byte *)(param_1 + 2) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) & 0xfd;
      }
      else {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) & 0xef;
      }
      else {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | 0x10;
      }
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 3;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = *(byte *)(param_1 + 2);
      *(char *)(param_1 + 2) = *(char *)(param_1 + 2) << 4;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *(byte *)(param_1 + 1);
      if ((char)((int)(uint)*(byte *)(param_1 + 1) >> 7) == '\0') {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) & 0xdf;
      }
      else {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) & 0x7f;
      }
      else {
        *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | 0x80;
      }
      bVar1 = *(byte *)(param_1 + 3);
      *(char *)(param_1 + 3) = *(char *)(param_1 + 3) << 2;
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) ^ 0xd9;
      *(byte *)(param_1 + 4) = *(byte *)(param_1 + 4) ^ 0x5f;
      uVar2 = *(undefined1 *)(param_1 + 1);
      *(undefined1 *)(param_1 + 1) = *(undefined1 *)(param_1 + 3);
      *(undefined1 *)(param_1 + 3) = uVar2;
      bVar1 = *(byte *)(param_1 + 2);
      *(char *)(param_1 + 2) = *(char *)(param_1 + 2) << 1;
      *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | (byte)((int)(uint)bVar1 >> 7);
      uVar2 = *(undefined1 *)(param_1 + 5);
      *(undefined1 *)(param_1 + 5) = *(undefined1 *)(param_1 + 5);
      *(undefined1 *)(param_1 + 5) = uVar2;
      bVar1 = *(byte *)(param_1 + 3);
      if (((int)(uint)*(byte *)(param_1 + 3) >> 3 & 1U) == 0) {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) & 0xf7;
      }
      else {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | 8;
      }
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 5;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *(byte *)(param_1 + 3);
      if (((int)(uint)*(byte *)(param_1 + 3) >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) & 0xfb;
      }
      else {
        *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | 4;
      }
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 4;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 4);
      *(byte *)(param_1 + 7) = *(byte *)(param_1 + 7) ^ 199;
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 6;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *(byte *)(param_1 + 3);
      *(char *)(param_1 + 3) = *(char *)(param_1 + 3) << 6;
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 7;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x20) {
      bVar1 = *(byte *)(param_1 + 6);
      *(char *)(param_1 + 6) = *(char *)(param_1 + 6) << 6;
      *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = *(undefined1 *)(param_1 + 3);
      *(undefined1 *)(param_1 + 3) = *(undefined1 *)(param_1 + 8);
      *(undefined1 *)(param_1 + 8) = uVar2;
      bVar1 = *(byte *)(param_1 + 2);
      if ((char)((int)(uint)*(byte *)(param_1 + 2) >> 7) == '\0') {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) & 0xfd;
      }
      else {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) & 0x7f;
      }
      else {
        *(byte *)(param_1 + 2) = *(byte *)(param_1 + 2) | 0x80;
      }
      *(byte *)(param_1 + 3) = *(byte *)(param_1 + 3) ^ 0x21;
      *(byte *)(param_1 + 8) = *(byte *)(param_1 + 8) ^ 0xe4;
      *(byte *)(param_1 + 0xd) = *(byte *)(param_1 + 0xd) ^ 0x58;
      uVar2 = *(undefined1 *)(param_1 + 0xf);
      *(undefined1 *)(param_1 + 0xf) = *(undefined1 *)(param_1 + 9);
      *(undefined1 *)(param_1 + 9) = uVar2;
      bVar1 = *(byte *)(param_1 + 8);
      *(char *)(param_1 + 8) = *(char *)(param_1 + 8) << 5;
      *(byte *)(param_1 + 8) = *(byte *)(param_1 + 8) | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *(byte *)(param_1 + 5);
      if (((int)(uint)*(byte *)(param_1 + 5) >> 4 & 1U) == 0) {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) & 0xef;
      }
      else {
        *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) | 0x10;
      }
      bVar1 = *(byte *)(param_1 + 1);
      *(char *)(param_1 + 1) = *(char *)(param_1 + 1) << 1;
      *(byte *)(param_1 + 1) = *(byte *)(param_1 + 1) | (byte)((int)(uint)bVar1 >> 7);
    }
    else {
      *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) ^ 0x4b;
      uVar2 = *(undefined1 *)(param_1 + 0x10);
      *(undefined1 *)(param_1 + 0x10) = *(undefined1 *)(param_1 + 9);
      *(undefined1 *)(param_1 + 9) = uVar2;
      bVar1 = *(byte *)(param_1 + 6);
      if (((int)(uint)*(byte *)(param_1 + 6) >> 3 & 1U) == 0) {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) & 0xf7;
      }
      else {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) & 0xf7;
      }
      else {
        *(byte *)(param_1 + 6) = *(byte *)(param_1 + 6) | 8;
      }
      bVar1 = *(byte *)(param_1 + 0x11);
      if (((int)(uint)*(byte *)(param_1 + 0x11) >> 6 & 1U) == 0) {
        *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) & 0x7f;
      }
      else {
        *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) & 0xbf;
      }
      else {
        *(byte *)(param_1 + 0x11) = *(byte *)(param_1 + 0x11) | 0x40;
      }
      uVar2 = *(undefined1 *)(param_1 + 1);
      *(undefined1 *)(param_1 + 1) = *(undefined1 *)(param_1 + 0x1b);
      *(undefined1 *)(param_1 + 0x1b) = uVar2;
      uVar2 = *(undefined1 *)(param_1 + 0x1f);
      *(undefined1 *)(param_1 + 0x1f) = *(undefined1 *)(param_1 + 6);
      *(undefined1 *)(param_1 + 6) = uVar2;
      *(byte *)(param_1 + 0x1a) = *(byte *)(param_1 + 0x1a) ^ 0xb2;
      *(byte *)(param_1 + 5) = *(byte *)(param_1 + 5) ^ 0xf9;
      uVar2 = *(undefined1 *)(param_1 + 0xb);
      *(undefined1 *)(param_1 + 0xb) = *(undefined1 *)(param_1 + 6);
      *(undefined1 *)(param_1 + 6) = uVar2;
      bVar1 = *(byte *)(param_1 + 9);
      *(char *)(param_1 + 9) = *(char *)(param_1 + 9) << 2;
      *(byte *)(param_1 + 9) = *(byte *)(param_1 + 9) | (byte)((int)(uint)bVar1 >> 6);
    }
  }
  return;
}



/* 005a6e70 FUN_005a6e70 */
/* Callers:
 * Callees:
 */

void FUN_005a6e70(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      param_1[2] = param_1[2] ^ 0x36;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      *param_1 = *param_1 ^ 0xc3;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      param_1[2] = param_1[2] ^ 0x46;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      param_1[3] = param_1[3] ^ 0x1b;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      param_1[6] = param_1[6] ^ 0xaf;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 1;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 7);
      param_1[4] = param_1[4] ^ 0x17;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 4;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
    }
    else {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 3;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 2;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 5;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x14];
      param_1[0x14] = bVar1;
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = param_1[0x1a];
      if ((char)((int)(uint)param_1[0x1a] >> 7) == '\0') {
        param_1[0x1a] = param_1[0x1a] & 0xef;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0x7f;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x80;
      }
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 1;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 7);
      param_1[0x1b] = param_1[0x1b] ^ 0xea;
      bVar1 = param_1[9];
      param_1[9] = param_1[0x14];
      param_1[0x14] = bVar1;
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14];
      param_1[0x14] = bVar1;
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 5;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14] << 3;
      param_1[0x14] = param_1[0x14] | (byte)((int)(uint)bVar1 >> 5);
    }
  }
  return;
}



/* 005a7710 FUN_005a7710 */
/* Callers:
 * Callees:
 */

void FUN_005a7710(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 3 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xf7;
      }
      else {
        *param_1 = *param_1 | 8;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      param_1[2] = param_1[2] ^ 0x36;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      param_1[6] = param_1[6] ^ 0xaf;
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      param_1[3] = param_1[3] ^ 0x1b;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[6];
      param_1[6] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      param_1[2] = param_1[2] ^ 0x46;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      *param_1 = *param_1 ^ 0xc3;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xbf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[0xf] << 4;
      param_1[0xf] = param_1[0xf] | (byte)((int)(uint)bVar1 >> 4);
      param_1[4] = param_1[4] ^ 0x17;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 7;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 1);
    }
    else {
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14] << 5;
      param_1[0x14] = param_1[0x14] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 3;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14];
      param_1[0x14] = bVar1;
      bVar1 = param_1[9];
      param_1[9] = param_1[0x14];
      param_1[0x14] = bVar1;
      param_1[0x1b] = param_1[0x1b] ^ 0xea;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 7;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x1a];
      if ((char)((int)(uint)param_1[0x1a] >> 7) == '\0') {
        param_1[0x1a] = param_1[0x1a] & 0xef;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x1a] = param_1[0x1a] & 0x7f;
      }
      else {
        param_1[0x1a] = param_1[0x1a] | 0x80;
      }
      bVar1 = param_1[0x1c];
      param_1[0x1c] = param_1[0x1c];
      param_1[0x1c] = bVar1;
      bVar1 = param_1[0x13];
      param_1[0x13] = param_1[0x14];
      param_1[0x14] = bVar1;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 3;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x12];
      param_1[0x12] = param_1[0x12] << 6;
      param_1[0x12] = param_1[0x12] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 5;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 6 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xbf;
      }
      else {
        param_1[2] = param_1[2] | 0x40;
      }
    }
  }
  return;
}



/* 005a7fb0 FUN_005a7fb0 */
/* Callers:
 * Callees:
 */

void FUN_005a7fb0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 100;
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      *param_1 = *param_1 ^ 0x61;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 4;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 4);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = bVar1;
      param_1[7] = param_1[7] ^ 0xaa;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 6;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      param_1[3] = param_1[3] ^ 0x47;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
    }
    else if (param_2 < 0x20) {
      param_1[5] = param_1[5] ^ 0x45;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 7;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = *param_1;
      *param_1 = *param_1 << 5;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 3);
      *param_1 = *param_1 ^ 199;
      param_1[2] = param_1[2] ^ 0x4b;
    }
    else {
      param_1[6] = param_1[6] ^ 0xa3;
      bVar1 = param_1[0x1d];
      if (((int)(uint)param_1[0x1d] >> 4 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xfb;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xef;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 0x10;
      }
      bVar1 = param_1[0x14];
      if ((char)((int)(uint)param_1[0x14] >> 7) == '\0') {
        param_1[0x14] = param_1[0x14] & 0xef;
      }
      else {
        param_1[0x14] = param_1[0x14] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x14] = param_1[0x14] & 0x7f;
      }
      else {
        param_1[0x14] = param_1[0x14] | 0x80;
      }
      *param_1 = *param_1 ^ 0xce;
      bVar1 = param_1[4];
      param_1[4] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14] << 5;
      param_1[0x14] = param_1[0x14] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      param_1[2] = param_1[0xd];
      param_1[0xd] = bVar1;
      param_1[0x1f] = param_1[0x1f] ^ 0xd2;
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0xb];
      param_1[0xb] = bVar1;
    }
  }
  return;
}



/* 005a8800 FUN_005a8800 */
/* Callers:
 * Callees:
 */

void FUN_005a8800(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 4;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 4);
      *param_1 = *param_1 ^ 0x61;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      param_1[3] = param_1[3] ^ 100;
      bVar1 = param_1[2];
      param_1[2] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 5 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xdf;
      }
      else {
        param_1[3] = param_1[3] | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 3 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xf7;
      }
      else {
        param_1[2] = param_1[2] | 8;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 6 & 1U) == 0) {
        param_1[5] = param_1[5] & 0x7f;
      }
      else {
        param_1[5] = param_1[5] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[5] = param_1[5] & 0xbf;
      }
      else {
        param_1[5] = param_1[5] | 0x40;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      param_1[3] = param_1[3] ^ 0x47;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 1 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xfd;
      }
      else {
        param_1[4] = param_1[4] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[7];
      param_1[7] = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 4;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 2;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 4;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[7] = param_1[7] ^ 0xaa;
      bVar1 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x20) {
      param_1[2] = param_1[2] ^ 0x4b;
      *param_1 = *param_1 ^ 199;
      bVar1 = *param_1;
      *param_1 = *param_1 << 3;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 1;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 7);
      param_1[5] = param_1[5] ^ 0x45;
    }
    else {
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0xb];
      param_1[0xb] = bVar1;
      param_1[0x1f] = param_1[0x1f] ^ 0xd2;
      bVar1 = param_1[2];
      param_1[2] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x14] << 3;
      param_1[0x14] = param_1[0x14] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[4];
      param_1[4] = param_1[0x1b];
      param_1[0x1b] = bVar1;
      *param_1 = *param_1 ^ 0xce;
      bVar1 = param_1[0x14];
      if ((char)((int)(uint)param_1[0x14] >> 7) == '\0') {
        param_1[0x14] = param_1[0x14] & 0xef;
      }
      else {
        param_1[0x14] = param_1[0x14] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x14] = param_1[0x14] & 0x7f;
      }
      else {
        param_1[0x14] = param_1[0x14] | 0x80;
      }
      bVar1 = param_1[0x1d];
      if (((int)(uint)param_1[0x1d] >> 4 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xfb;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x1d] = param_1[0x1d] & 0xef;
      }
      else {
        param_1[0x1d] = param_1[0x1d] | 0x10;
      }
      param_1[6] = param_1[6] ^ 0xa3;
    }
  }
  return;
}



/* 005a9040 FUN_005a9040 */
/* Callers:
 * Callees:
 */

void FUN_005a9040(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      param_1[2] = param_1[2] ^ 0xe4;
      bVar1 = param_1[6];
      param_1[6] = param_1[4];
      param_1[4] = bVar1;
      *param_1 = *param_1 ^ 0x62;
      param_1[3] = param_1[3] ^ 0x9e;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 6;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 6;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 5;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[8];
      param_1[8] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
    }
    else {
      bVar1 = param_1[0x10];
      if (((int)(uint)param_1[0x10] >> 6 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xdf;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xbf;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x40;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x16];
      param_1[0x16] = bVar1;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 7;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 1);
      param_1[0x1e] = param_1[0x1e] ^ 0xed;
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[6];
      param_1[6] = bVar1;
      param_1[0x1e] = param_1[0x1e] ^ 100;
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
    }
  }
  return;
}



/* 005a9750 FUN_005a9750 */
/* Callers:
 * Callees:
 */

void FUN_005a9750(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 3;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0x7f;
      }
      else {
        param_1[2] = param_1[2] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 2;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 2;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 6);
      param_1[3] = param_1[3] ^ 0x9e;
      *param_1 = *param_1 ^ 0x62;
      bVar1 = param_1[6];
      param_1[6] = param_1[4];
      param_1[4] = bVar1;
      param_1[2] = param_1[2] ^ 0xe4;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfb;
      }
      else {
        param_1[7] = param_1[7] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xf];
      param_1[0xf] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[8];
      param_1[8] = param_1[4];
      param_1[4] = bVar1;
    }
    else {
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      param_1[0x1e] = param_1[0x1e] ^ 100;
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[6];
      param_1[6] = bVar1;
      param_1[0x1e] = param_1[0x1e] ^ 0xed;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 1;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = *param_1;
      if ((char)((int)(uint)*param_1 >> 7) == '\0') {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0x7f;
      }
      else {
        *param_1 = *param_1 | 0x80;
      }
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x16];
      param_1[0x16] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x10];
      if (((int)(uint)param_1[0x10] >> 6 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xdf;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xbf;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x40;
      }
    }
  }
  return;
}



/* 005a9e50 FUN_005a9e50 */
/* Callers:
 * Callees:
 */

void FUN_005a9e50(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[1] = param_1[1] ^ 0x88;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      param_1[3] = param_1[3] ^ 0xe4;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      param_1[3] = param_1[3] ^ 0x52;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      param_1[6] = param_1[6] ^ 0x67;
      param_1[2] = param_1[2] ^ 0x14;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[10];
      param_1[10] = param_1[2];
      param_1[2] = bVar1;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 7;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 1);
      param_1[5] = param_1[5] ^ 0x73;
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 5 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xdf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x20;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 0xc1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 7;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 1);
    }
    else {
      bVar1 = param_1[3];
      param_1[3] = param_1[0xc];
      param_1[0xc] = bVar1;
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 2;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 3 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xf7;
      }
      else {
        param_1[10] = param_1[10] | 8;
      }
      param_1[10] = param_1[10] ^ 0xed;
      param_1[0xb] = param_1[0xb];
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0x7f;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      param_1[0x11] = param_1[0x11] ^ 0xa0;
      param_1[0x16] = param_1[0x16] ^ 0xb5;
    }
  }
  return;
}



/* 005aa9a0 FUN_005aa9a0 */
/* Callers:
 * Callees:
 */

void FUN_005aa9a0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 1 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfd;
      }
      else {
        param_1[1] = param_1[1] | 2;
      }
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 5 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xdf;
      }
      else {
        *param_1 = *param_1 | 0x20;
      }
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      param_1[1] = param_1[1] ^ 0x88;
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[6];
      param_1[6] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[5];
      if (((int)(uint)param_1[5] >> 1 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xef;
      }
      else {
        param_1[5] = param_1[5] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[5] = param_1[5] & 0xfd;
      }
      else {
        param_1[5] = param_1[5] | 2;
      }
      bVar1 = param_1[4];
      param_1[4] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      param_1[2] = param_1[2] ^ 0x14;
      param_1[6] = param_1[6] ^ 0x67;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      param_1[3] = param_1[3] ^ 0x52;
      bVar1 = param_1[6];
      if (((int)(uint)param_1[6] >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xfb;
      }
      else {
        param_1[6] = param_1[6] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      bVar1 = *param_1;
      *param_1 = param_1[3];
      param_1[3] = bVar1;
      param_1[3] = param_1[3] ^ 0xe4;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 1;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 4 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xef;
      }
      else {
        param_1[7] = param_1[7] | 0x10;
      }
      param_1[2] = param_1[2] ^ 0xc1;
      bVar1 = param_1[5];
      param_1[5] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 4;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[10];
      param_1[10] = param_1[4];
      param_1[4] = bVar1;
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 5 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xdf;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x20;
      }
      param_1[5] = param_1[5] ^ 0x73;
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 1;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[10];
      param_1[10] = param_1[2];
      param_1[2] = bVar1;
    }
    else {
      param_1[0x16] = param_1[0x16] ^ 0xb5;
      param_1[0x11] = param_1[0x11] ^ 0xa0;
      bVar1 = param_1[0xc];
      if (((int)(uint)param_1[0xc] >> 6 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0x7f;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xc] = param_1[0xc] & 0xbf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x40;
      }
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[4];
      param_1[4] = bVar1;
      param_1[0xb] = param_1[0xb];
      param_1[10] = param_1[10] ^ 0xed;
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 3 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xf7;
      }
      else {
        param_1[10] = param_1[10] | 8;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[0x19];
      param_1[0x19] = param_1[0x19] << 6;
      param_1[0x19] = param_1[0x19] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[7];
      param_1[7] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[0xc];
      param_1[0xc] = bVar1;
    }
  }
  return;
}



/* 005ab4f0 FUN_005ab4f0 */
/* Callers:
 * Callees:
 */

void FUN_005ab4f0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      param_1[2] = param_1[2] ^ 0xc;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      *param_1 = *param_1 ^ 0xd6;
      param_1[3] = param_1[3] ^ 0x9b;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      param_1[2] = param_1[2] ^ 0xc6;
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 6;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      param_1[5] = param_1[5] ^ 0xe5;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      param_1[1] = param_1[1] ^ 0x88;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      bVar1 = param_1[6];
      param_1[6] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x20) {
      param_1[8] = param_1[8] ^ 0x94;
      param_1[8] = param_1[8] ^ 0xcf;
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 3;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 3 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xf7;
      }
      else {
        param_1[0xb] = param_1[0xb] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xf7;
      }
      else {
        param_1[0xb] = param_1[0xb] | 8;
      }
    }
    else {
      param_1[0x18] = param_1[0x18] ^ 0x9d;
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 5 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xef;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xdf;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 2;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[0x10];
      if (((int)(uint)param_1[0x10] >> 5 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xef;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xdf;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x20;
      }
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 7;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x13];
      if (((int)(uint)param_1[0x13] >> 1 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0x7f;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x13] = param_1[0x13] & 0xfd;
      }
      else {
        param_1[0x13] = param_1[0x13] | 2;
      }
      bVar1 = param_1[0x12];
      param_1[0x12] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 1 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xfd;
      }
      else {
        param_1[0x11] = param_1[0x11] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xfd;
      }
      else {
        param_1[0x11] = param_1[0x11] | 2;
      }
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[0x1a] << 1;
      param_1[0x1a] = param_1[0x1a] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 4;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 4);
    }
  }
  return;
}



/* 005abec0 FUN_005abec0 */
/* Callers:
 * Callees:
 */

void FUN_005abec0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[1];
      if ((char)((int)(uint)param_1[1] >> 7) == '\0') {
        param_1[1] = param_1[1] & 0xdf;
      }
      else {
        param_1[1] = param_1[1] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[1] = param_1[1] & 0x7f;
      }
      else {
        param_1[1] = param_1[1] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[3];
      param_1[3] = bVar1;
      param_1[2] = param_1[2] ^ 0xc6;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 4 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xef;
      }
      else {
        param_1[2] = param_1[2] | 0x10;
      }
      bVar1 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      param_1[3] = param_1[3] ^ 0x9b;
      *param_1 = *param_1 ^ 0xd6;
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      param_1[2] = param_1[2] ^ 0xc;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = *param_1;
      *param_1 = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[1] = param_1[1] ^ 0x88;
      bVar1 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = *param_1;
      if (((int)(uint)*param_1 >> 1 & 1U) == 0) {
        *param_1 = *param_1 & 0xfb;
      }
      else {
        *param_1 = *param_1 | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        *param_1 = *param_1 & 0xfd;
      }
      else {
        *param_1 = *param_1 | 2;
      }
      param_1[5] = param_1[5] ^ 0xe5;
      bVar1 = param_1[2];
      if (((int)(uint)param_1[2] >> 1 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xdf;
      }
      else {
        param_1[2] = param_1[2] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[2] = param_1[2] & 0xfd;
      }
      else {
        param_1[2] = param_1[2] | 2;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 2;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xef;
      }
      else {
        param_1[3] = param_1[3] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xb];
      if (((int)(uint)param_1[0xb] >> 3 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xf7;
      }
      else {
        param_1[0xb] = param_1[0xb] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xf7;
      }
      else {
        param_1[0xb] = param_1[0xb] | 8;
      }
      bVar1 = param_1[8];
      param_1[8] = param_1[8] << 5;
      param_1[8] = param_1[8] | (byte)((int)(uint)bVar1 >> 3);
      param_1[8] = param_1[8] ^ 0xcf;
      param_1[8] = param_1[8] ^ 0x94;
    }
    else {
      bVar1 = param_1[0x1d];
      param_1[0x1d] = param_1[0x1d] << 4;
      param_1[0x1d] = param_1[0x1d] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x1a];
      param_1[0x1a] = param_1[0x1a] << 7;
      param_1[0x1a] = param_1[0x1a] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[0x11];
      if (((int)(uint)param_1[0x11] >> 1 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xfd;
      }
      else {
        param_1[0x11] = param_1[0x11] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x11] = param_1[0x11] & 0xfd;
      }
      else {
        param_1[0x11] = param_1[0x11] | 2;
      }
      bVar1 = param_1[0x12];
      param_1[0x12] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[0x13];
      if (((int)(uint)param_1[0x13] >> 1 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0x7f;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0x13] = param_1[0x13] & 0xfd;
      }
      else {
        param_1[0x13] = param_1[0x13] | 2;
      }
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 1;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 6 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xbf;
      }
      else {
        param_1[7] = param_1[7] | 0x40;
      }
      bVar1 = param_1[0x10];
      if (((int)(uint)param_1[0x10] >> 5 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xef;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x10] = param_1[0x10] & 0xdf;
      }
      else {
        param_1[0x10] = param_1[0x10] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 6;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 2);
      bVar1 = param_1[0x17];
      if (((int)(uint)param_1[0x17] >> 5 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xef;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0x17] = param_1[0x17] & 0xdf;
      }
      else {
        param_1[0x17] = param_1[0x17] | 0x20;
      }
      param_1[0x18] = param_1[0x18] ^ 0x9d;
    }
  }
  return;
}



/* 005ac8a0 FUN_005ac8a0 */
/* Callers:
 * Callees:
 */

void FUN_005ac8a0(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      param_1[1] = param_1[1] ^ 0xa2;
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      *param_1 = *param_1 ^ 0x5f;
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 3;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 5;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 1;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xdf;
      }
      else {
        param_1[4] = param_1[4] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 5;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[2];
      param_1[2] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[7];
      if ((char)((int)(uint)param_1[7] >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      bVar1 = param_1[5];
      param_1[5] = *param_1;
      *param_1 = bVar1;
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 7;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 7;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[9];
      param_1[9] = param_1[10];
      param_1[10] = bVar1;
    }
    else {
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 4 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xef;
      }
      else {
        param_1[10] = param_1[10] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xef;
      }
      else {
        param_1[10] = param_1[10] | 0x10;
      }
      bVar1 = param_1[0x13];
      if ((char)((int)(uint)param_1[0x13] >> 7) == '\0') {
        param_1[0x13] = param_1[0x13] & 0xfd;
      }
      else {
        param_1[0x13] = param_1[0x13] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0x7f;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x80;
      }
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 4;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 4);
      param_1[0x16] = param_1[0x16] ^ 0x68;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 1;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 7);
      param_1[0x16] = param_1[0x16] ^ 0xe8;
      param_1[0x10] = param_1[0x10] ^ 0xb3;
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x1f];
      param_1[0x1f] = bVar1;
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[1];
      param_1[1] = bVar1;
    }
  }
  return;
}



/* 005ad080 FUN_005ad080 */
/* Callers:
 * Callees:
 */

void FUN_005ad080(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      if ((char)((int)(uint)param_1[3] >> 7) == '\0') {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0x7f;
      }
      else {
        param_1[3] = param_1[3] | 0x80;
      }
      bVar1 = param_1[1];
      param_1[1] = param_1[1] << 3;
      param_1[1] = param_1[1] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 5;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = *param_1;
      *param_1 = bVar1;
      *param_1 = *param_1 ^ 0x5f;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      bVar1 = param_1[1];
      if (((int)(uint)param_1[1] >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xfb;
      }
      else {
        param_1[1] = param_1[1] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      param_1[1] = param_1[1] ^ 0xa2;
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[3];
      param_1[3] = param_1[3];
      param_1[3] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      param_1[5] = *param_1;
      *param_1 = bVar1;
      bVar1 = param_1[7];
      if ((char)((int)(uint)param_1[7] >> 7) == '\0') {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0x7f;
      }
      else {
        param_1[7] = param_1[7] | 0x80;
      }
      bVar1 = param_1[2];
      param_1[2] = param_1[5];
      param_1[5] = bVar1;
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 3;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[4];
      if (((int)(uint)param_1[4] >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xdf;
      }
      else {
        param_1[4] = param_1[4] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[6] << 7;
      param_1[6] = param_1[6] | (byte)((int)(uint)bVar1 >> 1);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[9];
      param_1[9] = param_1[10];
      param_1[10] = bVar1;
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 1;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xe];
      param_1[0xe] = param_1[0xe] << 1;
      param_1[0xe] = param_1[0xe] | (byte)((int)(uint)bVar1 >> 7);
    }
    else {
      bVar1 = param_1[0x17];
      param_1[0x17] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[0x14];
      param_1[0x14] = param_1[0x1f];
      param_1[0x1f] = bVar1;
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      param_1[0x10] = param_1[0x10] ^ 0xb3;
      param_1[0x16] = param_1[0x16] ^ 0xe8;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 7;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 1);
      param_1[0x16] = param_1[0x16] ^ 0x68;
      bVar1 = param_1[0x1e];
      param_1[0x1e] = param_1[0x1e] << 4;
      param_1[0x1e] = param_1[0x1e] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfd;
      }
      else {
        param_1[10] = param_1[10] | 2;
      }
      bVar1 = param_1[0x13];
      if ((char)((int)(uint)param_1[0x13] >> 7) == '\0') {
        param_1[0x13] = param_1[0x13] & 0xfd;
      }
      else {
        param_1[0x13] = param_1[0x13] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[0x13] = param_1[0x13] & 0x7f;
      }
      else {
        param_1[0x13] = param_1[0x13] | 0x80;
      }
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 4 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xef;
      }
      else {
        param_1[10] = param_1[10] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xef;
      }
      else {
        param_1[10] = param_1[10] | 0x10;
      }
    }
  }
  return;
}



/* 005ad860 FUN_005ad860 */
/* Callers:
 * Callees:
 */

void FUN_005ad860(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 6;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 2);
      uVar2 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = uVar2;
      uVar2 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = uVar2;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 7;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 1);
      uVar2 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = uVar2;
      uVar2 = param_1[4];
      param_1[4] = param_1[1];
      param_1[1] = uVar2;
      uVar2 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = uVar2;
      param_1[6] = param_1[6] ^ 0x8e;
      param_1[4] = param_1[4] ^ 0xc1;
      bVar1 = param_1[7];
      if (((int)(uint)(byte)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      bVar1 = param_1[6];
      if (((int)(uint)(byte)param_1[6] >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      param_1[5] = param_1[5] ^ 0x13;
      bVar1 = param_1[9];
      if (((int)(uint)(byte)param_1[9] >> 3 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xf7;
      }
      else {
        param_1[9] = param_1[9] | 8;
      }
      param_1[0xb] = param_1[0xb] ^ 2;
      bVar1 = param_1[0xb];
      if (((int)(uint)(byte)param_1[0xb] >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      bVar1 = param_1[4];
      if (((int)(uint)(byte)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[8];
      if (((int)(uint)(byte)param_1[8] >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xef;
      }
      else {
        param_1[8] = param_1[8] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 7;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 1);
    }
    else {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 2;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 6);
      param_1[3] = param_1[3] ^ 199;
      uVar2 = param_1[0x14];
      param_1[0x14] = param_1[0x12];
      param_1[0x12] = uVar2;
      bVar1 = param_1[0x14];
      if (((int)(uint)(byte)param_1[0x14] >> 1 & 1U) == 0) {
        param_1[0x14] = param_1[0x14] & 0xfb;
      }
      else {
        param_1[0x14] = param_1[0x14] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x14] = param_1[0x14] & 0xfd;
      }
      else {
        param_1[0x14] = param_1[0x14] | 2;
      }
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 4;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 4);
      uVar2 = param_1[0x1e];
      param_1[0x1e] = param_1[0x19];
      param_1[0x19] = uVar2;
      param_1[8] = param_1[8] ^ 0xb5;
      bVar1 = param_1[0xc];
      if (((int)(uint)(byte)param_1[0xc] >> 5 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0x7f;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xc] = param_1[0xc] & 0xdf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x20;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 3;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 5);
      bVar1 = param_1[0x16];
      if (((int)(uint)(byte)param_1[0x16] >> 5 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xfb;
      }
      else {
        param_1[0x16] = param_1[0x16] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xdf;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x20;
      }
    }
  }
  return;
}



/* 005ae070 FUN_005ae070 */
/* Callers:
 * Callees:
 */

void FUN_005ae070(undefined1 *param_1,int param_2)

{
  byte bVar1;
  undefined1 uVar2;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
      uVar2 = param_1[3];
      param_1[3] = *param_1;
      *param_1 = uVar2;
      uVar2 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = uVar2;
      bVar1 = param_1[3];
      param_1[3] = param_1[3] << 2;
      param_1[3] = param_1[3] | (byte)((int)(uint)bVar1 >> 6);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[6];
      if (((int)(uint)(byte)param_1[6] >> 4 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xbf;
      }
      else {
        param_1[6] = param_1[6] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[6] = param_1[6] & 0xef;
      }
      else {
        param_1[6] = param_1[6] | 0x10;
      }
      bVar1 = param_1[7];
      if (((int)(uint)(byte)param_1[7] >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      param_1[4] = param_1[4] ^ 0xc1;
      param_1[6] = param_1[6] ^ 0x8e;
      uVar2 = param_1[5];
      param_1[5] = param_1[5];
      param_1[5] = uVar2;
      uVar2 = param_1[4];
      param_1[4] = param_1[1];
      param_1[1] = uVar2;
      uVar2 = param_1[4];
      param_1[4] = param_1[2];
      param_1[2] = uVar2;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 1;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 7);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 1;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[1];
      if (((int)(uint)(byte)param_1[1] >> 4 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xbf;
      }
      else {
        param_1[1] = param_1[1] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[1] = param_1[1] & 0xef;
      }
      else {
        param_1[1] = param_1[1] | 0x10;
      }
      bVar1 = param_1[8];
      if (((int)(uint)(byte)param_1[8] >> 1 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xef;
      }
      else {
        param_1[8] = param_1[8] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[8] = param_1[8] & 0xfd;
      }
      else {
        param_1[8] = param_1[8] | 2;
      }
      bVar1 = param_1[4];
      if (((int)(uint)(byte)param_1[4] >> 6 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xef;
      }
      else {
        param_1[4] = param_1[4] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[4] = param_1[4] & 0xbf;
      }
      else {
        param_1[4] = param_1[4] | 0x40;
      }
      bVar1 = param_1[0xb];
      if (((int)(uint)(byte)param_1[0xb] >> 2 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xef;
      }
      else {
        param_1[0xb] = param_1[0xb] | 0x10;
      }
      if (((int)(uint)bVar1 >> 4 & 1U) == 0) {
        param_1[0xb] = param_1[0xb] & 0xfb;
      }
      else {
        param_1[0xb] = param_1[0xb] | 4;
      }
      param_1[0xb] = param_1[0xb] ^ 2;
      bVar1 = param_1[9];
      if (((int)(uint)(byte)param_1[9] >> 3 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xfb;
      }
      else {
        param_1[9] = param_1[9] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[9] = param_1[9] & 0xf7;
      }
      else {
        param_1[9] = param_1[9] | 8;
      }
      param_1[5] = param_1[5] ^ 0x13;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
    }
    else {
      bVar1 = param_1[0x16];
      if (((int)(uint)(byte)param_1[0x16] >> 5 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xfb;
      }
      else {
        param_1[0x16] = param_1[0x16] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x16] = param_1[0x16] & 0xdf;
      }
      else {
        param_1[0x16] = param_1[0x16] | 0x20;
      }
      bVar1 = param_1[4];
      param_1[4] = param_1[4] << 5;
      param_1[4] = param_1[4] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[0xc];
      if (((int)(uint)(byte)param_1[0xc] >> 5 & 1U) == 0) {
        param_1[0xc] = param_1[0xc] & 0x7f;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x80;
      }
      if ((char)((int)(uint)bVar1 >> 7) == '\0') {
        param_1[0xc] = param_1[0xc] & 0xdf;
      }
      else {
        param_1[0xc] = param_1[0xc] | 0x20;
      }
      param_1[8] = param_1[8] ^ 0xb5;
      uVar2 = param_1[0x1e];
      param_1[0x1e] = param_1[0x19];
      param_1[0x19] = uVar2;
      bVar1 = param_1[0x1f];
      param_1[0x1f] = param_1[0x1f] << 4;
      param_1[0x1f] = param_1[0x1f] | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[0x14];
      if (((int)(uint)(byte)param_1[0x14] >> 1 & 1U) == 0) {
        param_1[0x14] = param_1[0x14] & 0xfb;
      }
      else {
        param_1[0x14] = param_1[0x14] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[0x14] = param_1[0x14] & 0xfd;
      }
      else {
        param_1[0x14] = param_1[0x14] | 2;
      }
      uVar2 = param_1[0x14];
      param_1[0x14] = param_1[0x12];
      param_1[0x12] = uVar2;
      param_1[3] = param_1[3] ^ 199;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 6;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 2);
    }
  }
  return;
}



/* 005ae880 FUN_005ae880 */
/* Callers:
 * Callees:
 */

void FUN_005ae880(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 5;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
      param_1[4] = param_1[4] ^ 0x80;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      param_1[7] = param_1[7] ^ 0x3e;
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 3;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 5 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xdf;
      }
      else {
        param_1[10] = param_1[10] | 0x20;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[6];
      param_1[6] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      bVar1 = *param_1;
      *param_1 = *param_1 << 7;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 1);
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 6;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 2);
      param_1[8] = param_1[8] ^ 0x46;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 6;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 2);
      param_1[0xd] = param_1[0xd] ^ 0x8d;
      param_1[0xf] = param_1[0xf] ^ 0xb5;
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
    }
    else {
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 7;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 1);
      *param_1 = *param_1 ^ 4;
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 2;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 3;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 5);
    }
  }
  return;
}



/* 005aee30 FUN_005aee30 */
/* Callers:
 * Callees:
 */

void FUN_005aee30(byte *param_1,int param_2)

{
  byte bVar1;
  
  if (3 < param_2) {
    if (param_2 < 8) {
      bVar1 = param_1[1];
      param_1[1] = param_1[1];
      param_1[1] = bVar1;
      bVar1 = *param_1;
      *param_1 = param_1[1];
      param_1[1] = bVar1;
      bVar1 = param_1[2];
      param_1[2] = param_1[2] << 3;
      param_1[2] = param_1[2] | (byte)((int)(uint)bVar1 >> 5);
    }
    else if (param_2 < 0x10) {
      bVar1 = param_1[5];
      param_1[5] = param_1[5] << 5;
      param_1[5] = param_1[5] | (byte)((int)(uint)bVar1 >> 3);
      param_1[7] = param_1[7] ^ 0x3e;
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xf7;
      }
      else {
        param_1[7] = param_1[7] | 8;
      }
      if (((int)(uint)bVar1 >> 3 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      param_1[4] = param_1[4] ^ 0x80;
      bVar1 = param_1[3];
      if (((int)(uint)param_1[3] >> 3 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xbf;
      }
      else {
        param_1[3] = param_1[3] | 0x40;
      }
      if (((int)(uint)bVar1 >> 6 & 1U) == 0) {
        param_1[3] = param_1[3] & 0xf7;
      }
      else {
        param_1[3] = param_1[3] | 8;
      }
    }
    else if (param_2 < 0x20) {
      bVar1 = param_1[7];
      if (((int)(uint)param_1[7] >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      if (((int)(uint)bVar1 >> 1 & 1U) == 0) {
        param_1[7] = param_1[7] & 0xfd;
      }
      else {
        param_1[7] = param_1[7] | 2;
      }
      bVar1 = param_1[0xd];
      param_1[0xd] = param_1[1];
      param_1[1] = bVar1;
      param_1[0xf] = param_1[0xf] ^ 0xb5;
      param_1[0xd] = param_1[0xd] ^ 0x8d;
      bVar1 = param_1[0xb];
      param_1[0xb] = param_1[0xb] << 2;
      param_1[0xb] = param_1[0xb] | (byte)((int)(uint)bVar1 >> 6);
      param_1[8] = param_1[8] ^ 0x46;
      bVar1 = param_1[10];
      param_1[10] = param_1[10] << 2;
      param_1[10] = param_1[10] | (byte)((int)(uint)bVar1 >> 6);
      bVar1 = *param_1;
      *param_1 = *param_1 << 1;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 7);
      bVar1 = param_1[0xf];
      if ((char)((int)(uint)param_1[0xf] >> 7) == '\0') {
        param_1[0xf] = param_1[0xf] & 0xdf;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x20;
      }
      if (((int)(uint)bVar1 >> 5 & 1U) == 0) {
        param_1[0xf] = param_1[0xf] & 0x7f;
      }
      else {
        param_1[0xf] = param_1[0xf] | 0x80;
      }
      bVar1 = param_1[6];
      param_1[6] = param_1[0xd];
      param_1[0xd] = bVar1;
      bVar1 = *param_1;
      *param_1 = *param_1 << 4;
      *param_1 = *param_1 | (byte)((int)(uint)bVar1 >> 4);
      bVar1 = param_1[10];
      if (((int)(uint)param_1[10] >> 5 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xfb;
      }
      else {
        param_1[10] = param_1[10] | 4;
      }
      if (((int)(uint)bVar1 >> 2 & 1U) == 0) {
        param_1[10] = param_1[10] & 0xdf;
      }
      else {
        param_1[10] = param_1[10] | 0x20;
      }
    }
    else {
      bVar1 = param_1[9];
      param_1[9] = param_1[9] << 5;
      param_1[9] = param_1[9] | (byte)((int)(uint)bVar1 >> 3);
      bVar1 = param_1[7];
      param_1[7] = param_1[7] << 6;
      param_1[7] = param_1[7] | (byte)((int)(uint)bVar1 >> 2);
      *param_1 = *param_1 ^ 4;
      bVar1 = param_1[0xc];
      param_1[0xc] = param_1[0xc] << 1;
      param_1[0xc] = param_1[0xc] | (byte)((int)(uint)bVar1 >> 7);
    }
  }
  return;
}


