"""Synthetic boundary/error checks, not client-captured compatibility fixtures."""
import struct
import unittest

from c9_protocol_offline import FrameSplitter, decoded_checksum, inspect_decoded, signed_checksum


def frame(kind=0x2EE5, payload=b'', flag=1):
    return struct.pack('<HBHII', 13 + len(payload), flag, kind, 0, 0) + payload


class OfflineProtocolTests(unittest.TestCase):
    def test_signed_byte_checksum_vectors(self):
        # Literal arithmetic vectors from the recovered MUL/XOR/MOVSX rule.
        for data, expected in [(b'', 0xBD136D48), (b'\0', 0x1E6422B8),
                               (b'\x7f', 0x1E6422C7), (b'\x80', 0xE19BDD38),
                               (b'\xff', 0xE19BDD47),
                               (bytes.fromhex('007f80ff'), 0x8EDB4D48)]:
            self.assertEqual(signed_checksum(data), expected)

    def test_every_stream_split_and_multiple_frames(self):
        frames = [frame(), frame(0x2EE6, b'\0')]
        stream = b''.join(frames)
        for boundary in range(len(stream) + 1):
            parser = FrameSplitter()
            actual = parser.feed(stream[:boundary]) + parser.feed(stream[boundary:])
            parser.finish()
            self.assertEqual(actual, frames)

    def test_limits_and_incomplete_frames(self):
        for length in [0, 12, 4097]:
            with self.assertRaises(ValueError):
                FrameSplitter(4096).feed(struct.pack('<H', length))
        parser = FrameSplitter()
        parser.feed(frame()[:-1])
        with self.assertRaises(ValueError):
            parser.finish()

    def test_wire_type_is_never_interpreted(self):
        wire = frame(0xFFFF)
        parser = FrameSplitter()
        self.assertEqual(parser.feed(wire), [wire])
        parser.finish()

    def test_decoded_lengths_and_checksum_scope(self):
        with self.assertRaises(ValueError):
            inspect_decoded(b'\0' * 12)
        with self.assertRaises(ValueError):
            inspect_decoded(frame() + b'\0')
        plain = frame(flag=0)
        self.assertNotIn('checksum_matches', inspect_decoded(plain))
        with self.assertRaises(ValueError):
            inspect_decoded(plain, True)

    def test_checksum_field_normalization_and_corruption(self):
        data = bytearray(frame(payload=bytes.fromhex('80ff')))
        checksum = decoded_checksum(data)
        struct.pack_into('<I', data, 5, checksum)
        self.assertEqual(decoded_checksum(data), checksum)
        self.assertTrue(inspect_decoded(data, True)['checksum_matches'])
        data[-1] ^= 1
        self.assertFalse(inspect_decoded(data, True)['checksum_matches'])

    def test_server_list_entry_offsets_and_preserved_bytes(self):
        entry = b'\x91' + struct.pack('<I', 0x12345678) + bytes(range(47))
        data = frame(0x2EE6, b'\x02' + entry + entry + b'\xaa')
        result = inspect_decoded(data)['server_list_static_layout']
        self.assertEqual(result['count'], 2)
        self.assertEqual(result['entries'][1]['lookup_key_u32_le'], 0x12345678)
        self.assertEqual(result['entries'][0]['raw_hex'], entry.hex())
        self.assertEqual(result['entries'][0]['name_source_tail_hex'], bytes(range(47)).hex())
        self.assertEqual(result['trailing_hex'], 'aa')

    def test_server_list_count_bounds(self):
        for payload in [b'', b'\x01', b'\xff' + bytes(52)]:
            with self.assertRaises(ValueError):
                inspect_decoded(frame(0x2EE6, payload))
        self.assertEqual(inspect_decoded(frame(0x2EE6, b'\0'))[
            'server_list_static_layout']['entries'], [])


if __name__ == '__main__':
    unittest.main()
