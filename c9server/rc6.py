"""RC6-32/20 block primitive; used only for legacy C9 protocol compatibility.

Verified with Crypto++ TestData/rc6val.dat independent known-answer vectors.
Recovered C9 routines: E6F950 key schedule, E6FB30 encode, E6FC30 decode.
"""
import struct

MASK = 0xFFFFFFFF


def rol(value, count):
    value &= MASK
    count &= 31
    return ((value << count) | (value >> ((32 - count) & 31))) & MASK


def ror(value, count):
    return rol(value, -count)


class RC6:
    block_size = 16

    def __init__(self, key):
        if len(key) not in (16, 24, 32):
            raise ValueError('RC6 requires a 16, 24 or 32 byte key')
        words = list(struct.unpack('<' + 'I' * (len(key) // 4), key))
        self.schedule = [(0xB7E15163 + i * 0x9E3779B9) & MASK for i in range(44)]
        a = b = i = j = 0
        for _ in range(3 * max(44, len(words))):
            a = self.schedule[i] = rol(self.schedule[i] + a + b, 3)
            b = words[j] = rol(words[j] + a + b, a + b)
            i = (i + 1) % 44
            j = (j + 1) % len(words)

    def encrypt_block(self, block):
        if len(block) != 16:
            raise ValueError('RC6 block must be 16 bytes')
        a, b, c, d = struct.unpack('<4I', block)
        s = self.schedule
        b = (b + s[0]) & MASK
        d = (d + s[1]) & MASK
        for round_number in range(1, 21):
            t = rol(b * (2 * b + 1), 5)
            u = rol(d * (2 * d + 1), 5)
            a = (rol(a ^ t, u) + s[2 * round_number]) & MASK
            c = (rol(c ^ u, t) + s[2 * round_number + 1]) & MASK
            a, b, c, d = b, c, d, a
        return struct.pack('<4I', (a + s[42]) & MASK, b, (c + s[43]) & MASK, d)

    def decrypt_block(self, block):
        if len(block) != 16:
            raise ValueError('RC6 block must be 16 bytes')
        a, b, c, d = struct.unpack('<4I', block)
        s = self.schedule
        a = (a - s[42]) & MASK
        c = (c - s[43]) & MASK
        for round_number in range(20, 0, -1):
            a, b, c, d = d, a, b, c
            u = rol(d * (2 * d + 1), 5)
            t = rol(b * (2 * b + 1), 5)
            c = ror(c - s[2 * round_number + 1], t) ^ u
            a = ror(a - s[2 * round_number], u) ^ t
        return struct.pack('<4I', a, (b - s[0]) & MASK, c, (d - s[1]) & MASK)
