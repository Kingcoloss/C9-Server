import json
import struct
import unittest

from observe_game_frames import Direction


def plain(kind, payload):
    return struct.pack('<HBHII', 13 + len(payload), 0, kind, 0, 0) + payload


class LiveMetadataTests(unittest.TestCase):
    def test_fragmented_auth_frame_and_retransmit_never_export_body(self):
        state = Direction()
        frame = plain(0x2f04, b'private-session-value')
        self.assertEqual([], state.feed(100, True, b''))
        self.assertEqual([], state.feed(101, False, frame[:8]))
        self.assertEqual([], state.feed(101, False, frame[:8]))
        result = state.feed(109, False, frame[8:])
        self.assertEqual('0x2f04', result[0]['type'])
        self.assertEqual(21, result[0]['payload_length'])
        self.assertNotIn('private-session-value', json.dumps(result))
        self.assertNotIn('payload', result[0])

    def test_coalesced_greeting_and_small_result_classes(self):
        state = Direction()
        state.feed(0xfffffffe, True, b'')
        frames = plain(4, b'\x00') + plain(0x2f05, struct.pack('<IIII', 0, 1, 0xffffffff, 123456789))
        records = state.feed(0xffffffff, False, frames)
        self.assertEqual(2, len(records))
        self.assertEqual(0, records[0]['greeting_mode'])
        self.assertEqual([0], records[1]['zero_word_offsets'])
        self.assertEqual([4], records[1]['one_word_offsets'])
        self.assertEqual([8], records[1]['max_word_offsets'])
        self.assertNotIn('123456789', json.dumps(records))

    def test_missing_syn_and_gap_are_explicit(self):
        state = Direction()
        with self.assertRaisesRegex(ValueError, 'missed SYN'):
            state.feed(42, False, plain(4, b'\x00'))
        state = Direction()
        state.feed(100, True, b'')
        with self.assertRaisesRegex(ValueError, 'gap'):
            state.feed(102, False, b'x')
        self.assertEqual([], state.feed(101, False, plain(4, b'\x00')))

    def test_syn_segment_can_include_data(self):
        state = Direction()
        records = state.feed(100, True, plain(4, b'\x00'))
        self.assertEqual('0x0004', records[0]['type'])
        self.assertEqual(0, records[0]['greeting_mode'])


if __name__ == '__main__':
    unittest.main()
