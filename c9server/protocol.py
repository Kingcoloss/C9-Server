"""Bounded C9 TCP framing and the statically recovered initial cipher epoch.

This implementation covers the initial RC6 epoch only (256 transformed frames
per direction). Later cipher rotation is deliberately rejected until recovered.
"""
import hashlib
import struct
from dataclasses import dataclass

from .rc6 import RC6
from .twister import transform

HEADER_SIZE = 13
INITIAL_SEED = bytes.fromhex('3333323334353637383930313233343536373839303132333435363738393132')


class ProtocolError(ValueError):
    pass


class UnsupportedEpoch(ProtocolError):
    pass


def signed_checksum(data):
    value = 0xBD136D48
    for byte in data:
        value = ((value * 0x03FAA1FF) ^ (byte if byte < 128 else byte - 256)) & 0xFFFFFFFF
    return value


def decoded_checksum(frame):
    if len(frame) < HEADER_SIZE:
        raise ProtocolError('Decoded frame shorter than 13 bytes')
    work = bytearray(frame[3:])
    struct.pack_into('<I', work, 2, 100)
    return signed_checksum(work)


class FrameSplitter:
    def __init__(self, max_frame=65535):
        if not HEADER_SIZE <= max_frame <= 65535:
            raise ValueError('max_frame must be between 13 and 65535')
        self.max_frame = max_frame
        self.pending = bytearray()

    def feed(self, chunk):
        self.pending.extend(chunk)
        frames = []
        while len(self.pending) >= 2:
            size = struct.unpack_from('<H', self.pending)[0]
            if not HEADER_SIZE <= size <= self.max_frame:
                raise ProtocolError(f'Invalid frame length {size}')
            if len(self.pending) < size:
                break
            frames.append(bytes(self.pending[:size]))
            del self.pending[:size]
        return frames

    def finish(self):
        if self.pending:
            raise ProtocolError(f'Incomplete final frame: {len(self.pending)} bytes')


@dataclass(frozen=True)
class Packet:
    kind: int
    payload: bytes = b''
    flag: int = 1
    sequence: int = 0
    checksum: int = 0

    def __post_init__(self):
        if not 0 <= self.kind <= 65535 or not 0 <= self.flag <= 255:
            raise ProtocolError('Type or flag out of range')
        if not 0 <= self.sequence <= 0xFFFFFFFF or not 0 <= self.checksum <= 0xFFFFFFFF:
            raise ProtocolError('Sequence or checksum out of range')
        if not isinstance(self.payload, bytes) or len(self.payload) > 65535 - HEADER_SIZE:
            raise ProtocolError('Payload must be bounded immutable bytes')

    def decoded_bytes(self):
        return struct.pack('<HBHII', HEADER_SIZE + len(self.payload), self.flag,
                           self.kind, self.checksum, self.sequence) + self.payload

    @classmethod
    def from_decoded(cls, frame):
        if len(frame) < HEADER_SIZE:
            raise ProtocolError('Decoded frame shorter than header')
        size, flag, kind, checksum, sequence = struct.unpack_from('<HBHII', frame)
        if size != len(frame):
            raise ProtocolError('Decoded length mismatch')
        return cls(kind, bytes(frame[13:]), flag, sequence, checksum)


class InitialOuterCipher:
    """00DCECB0 seed -> RIPEMD160 -> selector 5 -> RC6 with digest[:16]."""
    frame_limit = 256

    def __init__(self):
        try:
            digest = hashlib.new('ripemd160', INITIAL_SEED).digest()
        except ValueError as error:
            raise RuntimeError('Python/OpenSSL must provide RIPEMD160') from error
        if digest.hex() != '6f749a998ed7373b8413712b6b3a201381850d31' or digest[5] % 10 != 5:
            raise RuntimeError('Initial cipher derivation mismatch')
        self.cipher = RC6(digest[:16])
        self.frames = 0
        self.table = bytearray([99] * 128)

    def _check_epoch(self):
        if self.frames >= self.frame_limit:
            raise UnsupportedEpoch('Initial cipher epoch exhausted; subsequent key rotation is not implemented')

    def commit(self, transformed_plaintext):
        if transformed_plaintext:
            values = transformed_plaintext[:8] if len(transformed_plaintext) > 8 else transformed_plaintext[:1]
            for value in values:
                self.table[value & 127] = value
        self.frames += 1

    def encrypt(self, plaintext):
        self._check_epoch()
        padding = (-len(plaintext)) % 16
        padded = plaintext + bytes(padding)
        return b''.join(self.cipher.encrypt_block(padded[i:i + 16])
                        for i in range(0, len(padded), 16)) + bytes([padding])

    def decrypt(self, ciphertext):
        self._check_epoch()
        if len(ciphertext) < 17 or len(ciphertext) % 16 != 1 or ciphertext[-1] >= 16:
            raise ProtocolError('Invalid RC6 wrapper length/footer')
        padding = ciphertext[-1]
        clear = b''.join(self.cipher.decrypt_block(ciphertext[i:i + 16])
                         for i in range(0, len(ciphertext) - 1, 16))
        if padding and clear[-padding:] != bytes(padding):
            raise ProtocolError('Nonzero RC6 wrapper padding')
        return clear[:-padding] if padding else clear


class ConnectionCodec:
    def __init__(self, *, max_frame=8192, discover_receive_selector=False):
        self.max_frame = max_frame
        if not 20 <= max_frame <= 65535:
            raise ValueError('max_frame must be between 20 and 65535')
        self.send_outer = InitialOuterCipher()
        self.receive_outer = InitialOuterCipher()
        self.send_sequence = 0
        self.receive_sequence = 0
        self.discover_receive_selector = discover_receive_selector
        self.last_receive_selector = None

    def encode(self, packet, *, selector_adjustment=0):
        if packet.flag == 0:
            data = packet.decoded_bytes()
        else:
            raw = bytearray(Packet(packet.kind, packet.payload, packet.flag, self.send_sequence).decoded_bytes())
            checksum = decoded_checksum(raw)
            struct.pack_into('<I', raw, 5, checksum)
            selector = ((checksum + selector_adjustment) & 0xFFFFFFFF) % 80
            raw[13:] = transform(raw[13:], selector)
            encrypted = self.send_outer.encrypt(bytes(raw[3:]))
            size = 3 + len(encrypted)
            if size > self.max_frame or size > 65535:
                raise ProtocolError('Encoded frame exceeds configured maximum')
            data = struct.pack('<HB', size, packet.flag) + encrypted
            self.send_outer.commit(bytes(raw[3:]))
            self.send_sequence = (self.send_sequence + 1) & 0xFFFFFFFF
        if len(data) > self.max_frame:
            raise ProtocolError('Frame exceeds configured maximum')
        return data

    def decode(self, wire):
        if len(wire) < HEADER_SIZE or len(wire) > self.max_frame:
            raise ProtocolError('Wire frame outside configured bounds')
        if struct.unpack_from('<H', wire)[0] != len(wire):
            raise ProtocolError('Wire length mismatch')
        if wire[2] == 0:
            return Packet.from_decoded(wire)
        transformed = self.receive_outer.decrypt(wire[3:])
        raw = bytearray(struct.pack('<HB', 3 + len(transformed), wire[2]) + transformed)
        if len(raw) < HEADER_SIZE:
            raise ProtocolError('Decrypted body shorter than header')
        checksum = struct.unpack_from('<I', raw, 5)[0]
        sequence = struct.unpack_from('<I', raw, 9)[0]
        if sequence != self.receive_sequence:
            raise ProtocolError('Unexpected receive sequence')
        # Client sends can add a per-type adjustment. For unknown adjustments,
        # require exactly one checksum-valid plaintext across the bounded registry.
        selectors = range(80) if self.discover_receive_selector else [checksum % 80]
        candidates = {}
        for selector in selectors:
            payload = transform(raw[13:], selector, decode=True)
            candidate = raw[:13] + payload
            if decoded_checksum(candidate) == checksum:
                candidates.setdefault(payload, selector)
        if len(candidates) != 1:
            raise ProtocolError('Checksum mismatch or ambiguous payload transform')
        payload, selector = next(iter(candidates.items()))
        packet = Packet.from_decoded(raw[:13] + payload)
        self.receive_outer.commit(transformed)
        self.receive_sequence = (self.receive_sequence + 1) & 0xFFFFFFFF
        self.last_receive_selector = selector
        return packet
