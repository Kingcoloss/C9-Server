import json
import random
import struct
import unittest
from pathlib import Path

from c9server.messages import parse_server_list, server_entry_lookup_key, server_list_packet
from c9server.protocol import ConnectionCodec, FrameSplitter, Packet, ProtocolError, UnsupportedEpoch
from c9server.rc6 import RC6
from c9server.twister import transform

ROOT = Path(__file__).resolve().parents[1]


class ProtocolTests(unittest.TestCase):
    def test_independent_rc6_known_answers(self):
        vectors = json.loads((ROOT / 'tests/fixtures/rc6_kats.json').read_text())['vectors']
        self.assertEqual(len(vectors), 6)
        for vector in vectors:
            key, plaintext, ciphertext = (bytes.fromhex(vector[k]) for k in
                                          ('key_hex', 'plaintext_hex', 'ciphertext_hex'))
            cipher = RC6(key)
            self.assertEqual(cipher.encrypt_block(plaintext), ciphertext)
            self.assertEqual(cipher.decrypt_block(ciphertext), plaintext)

    def test_all_twister_selectors_and_boundaries(self):
        rng = random.Random(1017)
        for selector in range(80):
            for size in [0, 1, 3, 4, 7, 8, 15, 16, 31, 32, 52, 2162]:
                data = bytes(rng.randrange(256) for _ in range(size))
                encoded = transform(data, selector)
                self.assertEqual(transform(encoded, selector, decode=True), data)
                self.assertEqual(transform(data, selector + 80), encoded)
        # Hand-reduced from 00564CC0's 4..7 byte branch, independent of tables.
        self.assertEqual(transform(bytes(4), 0), bytes.fromhex('ef000000'))

    def test_codec_roundtrip_and_direction_isolation(self):
        client = ConnectionCodec()
        server = ConnectionCodec(discover_receive_selector=True)
        rng = random.Random(49482)
        for size in [0, 1, 3, 4, 7, 8, 15, 16, 31, 32, 52, 2162]:
            payload = bytes(rng.randrange(256) for _ in range(size))
            sent = client.encode(Packet(0x2EE1, payload))
            self.assertEqual(server.decode(sent).payload, payload)
            reply = server.encode(Packet(0x2EE6, payload))
            self.assertEqual(client.decode(reply).payload, payload)
        self.assertEqual(client.send_sequence, 12)
        self.assertEqual(server.receive_sequence, 12)
        self.assertEqual(client.receive_outer.table, server.send_outer.table)
        self.assertEqual(client.send_outer.table, server.receive_outer.table)

    def test_receive_adjustment_discovery(self):
        client = ConnectionCodec()
        server = ConnectionCodec(discover_receive_selector=True)
        payload = bytes(range(52))
        wire = client.encode(Packet(0x2EEC, payload), selector_adjustment=0xFFFFFFF1)
        self.assertEqual(server.decode(wire).payload, payload)

    def test_plain_packets_do_not_advance_cipher_or_sequences(self):
        sender, receiver = ConnectionCodec(), ConnectionCodec()
        wire = sender.encode(Packet(4, b'\0', flag=0))
        self.assertEqual(receiver.decode(wire), Packet(4, b'\0', flag=0))
        self.assertEqual(sender.send_sequence, 0)
        self.assertEqual(receiver.receive_sequence, 0)
        self.assertEqual(sender.send_outer.frames, 0)
        self.assertEqual(receiver.receive_outer.frames, 0)

    def test_invalid_sequence_and_corruption_do_not_commit_receive_state(self):
        sender, receiver = ConnectionCodec(), ConnectionCodec()
        first = sender.encode(Packet(0x2EE5))
        second = sender.encode(Packet(0x2EE5))
        with self.assertRaisesRegex(ProtocolError, 'sequence'):
            receiver.decode(second)
        self.assertEqual(receiver.receive_sequence, 0)
        self.assertEqual(receiver.receive_outer.frames, 0)
        self.assertEqual(receiver.decode(first).sequence, 0)
        bad = bytearray(second)
        bad[4] ^= 0x80
        with self.assertRaises(ProtocolError):
            receiver.decode(bytes(bad))
        self.assertEqual(receiver.receive_sequence, 1)
        self.assertEqual(receiver.decode(second).sequence, 1)

    def test_initial_epoch_boundary_is_explicit(self):
        sender, receiver = ConnectionCodec(), ConnectionCodec()
        for i in range(256):
            self.assertEqual(receiver.decode(sender.encode(Packet(0x2EE5))).sequence, i)
        with self.assertRaises(UnsupportedEpoch):
            sender.encode(Packet(0x2EE5))
        with self.assertRaises(UnsupportedEpoch):
            receiver.decode(ConnectionCodec().encode(Packet(0x2EE5)))

    def test_length_footer_limits_and_failed_send_state(self):
        sender = ConnectionCodec(max_frame=20)
        with self.assertRaises(ProtocolError):
            sender.encode(Packet(0x2EE6, bytes(52)))
        self.assertEqual(sender.send_sequence, 0)
        self.assertEqual(sender.send_outer.frames, 0)
        wire = bytearray(sender.encode(Packet(0x2EE5)))
        wire[-1] = 16
        with self.assertRaises(ProtocolError):
            ConnectionCodec().decode(bytes(wire))
        with self.assertRaises(ProtocolError):
            ConnectionCodec().decode(bytes(wire[:-1]))

    def test_wire_fragmentation_all_boundaries(self):
        sender = ConnectionCodec()
        frames = [sender.encode(Packet(0x2EE5)), sender.encode(Packet(0x2EE1, bytes(2162)))]
        data = b''.join(frames)
        for boundary in range(len(data) + 1):
            splitter = FrameSplitter(8192)
            self.assertEqual(splitter.feed(data[:boundary]) + splitter.feed(data[boundary:]), frames)
            splitter.finish()

    def test_server_list_opaque_schema(self):
        entry = b'\x03' + struct.pack('<I', 17) + bytes(range(47))
        packet = server_list_packet([entry])
        entries, trailing = parse_server_list(packet)
        self.assertEqual(entries, (entry,))
        self.assertEqual(trailing, b'')
        self.assertEqual(server_entry_lookup_key(entries[0]), 17)
        for entries in [[bytes(51)], [bytes(52)] * 256]:
            with self.assertRaises(ProtocolError):
                server_list_packet(entries)
        with self.assertRaises(ProtocolError):
            parse_server_list(Packet(0x2EE6, b'\x02' + entry))


if __name__ == '__main__':
    unittest.main()
