import asyncio
import json
import struct
import unittest
from dataclasses import replace
from pathlib import Path

from c9server.messages import parse_server_list, server_entry_lookup_key
from c9server.protocol import ConnectionCodec, Packet
from c9server.server import ResearchServer, ServerConfig

ROOT = Path(__file__).resolve().parents[1]


async def read_wire(reader):
    prefix = await asyncio.wait_for(reader.readexactly(2), 2)
    size = struct.unpack('<H', prefix)[0]
    return prefix + await asyncio.wait_for(reader.readexactly(size - 2), 2)


class ConfigTests(unittest.TestCase):
    def test_config_bounds_and_explicit_fixture_profile(self):
        with self.assertRaises(ValueError):
            ServerConfig(host='0.0.0.0')
        with self.assertRaises(ValueError):
            ServerConfig(max_frame=19)
        fixture = ServerConfig.load(ROOT / 'config/lab-fixture.json')
        self.assertEqual(fixture.profile, 'fixture')
        with self.assertRaises(ValueError):
            replace(fixture, profile='probe')


class ServerTests(unittest.IsolatedAsyncioTestCase):
    async def asyncSetUp(self):
        self.events = []
        self.config = ServerConfig.load(ROOT / 'config/lab-fixture.json', port=0, timeout_seconds=0.4)
        self.server = await ResearchServer(self.config, self.events.append).start()
        self.writers = []

    async def asyncTearDown(self):
        for writer in self.writers:
            writer.close()
            try:
                await writer.wait_closed()
            except ConnectionError:
                pass
        await self.server.close()

    async def connect(self):
        reader, writer = await asyncio.open_connection('127.0.0.1', self.server.port)
        self.writers.append(writer)
        codec = ConnectionCodec()
        self.assertEqual(codec.decode(await read_wire(reader)), Packet(4, b'\0', flag=0))
        return reader, writer, codec

    async def test_encrypted_login_fixture_then_server_list_over_tcp(self):
        reader, writer, codec = await self.connect()
        marker = b'PRIVATE-FIXTURE-DO-NOT-LOG'
        payload = marker + bytes(2162 - len(marker))
        wire = codec.encode(Packet(0x2EE1, payload))
        # Exercise a split two-byte length prefix and a fragmented encrypted body.
        for chunk in [wire[:1], wire[1:7], wire[7:1024], wire[1024:]]:
            writer.write(chunk)
            await writer.drain()
            await asyncio.sleep(0)
        reply = codec.decode(await read_wire(reader))
        self.assertEqual(reply.kind, 0x2EE2)
        self.assertEqual(len(reply.payload), 20)
        writer.write(codec.encode(Packet(0x2EE5)))
        await writer.drain()
        reply = codec.decode(await read_wire(reader))
        entries, trailing = parse_server_list(reply)
        self.assertEqual(len(entries), 1)
        self.assertEqual(server_entry_lookup_key(entries[0]), 1)
        self.assertEqual(trailing, b'')
        self.assertNotIn(marker.decode(), json.dumps(self.events))
        self.assertEqual([e['state'] for e in self.events if e['event'] == 'state'],
                         ['login_fixture_sent', 'server_list_sent'])

    async def test_two_requests_coalesced_and_reconnect_state_isolation(self):
        for _ in range(2):
            reader, writer, codec = await self.connect()
            writer.write(codec.encode(Packet(0x2EE1, bytes(2162))) + codec.encode(Packet(0x2EE5)))
            await writer.drain()
            self.assertEqual(codec.decode(await read_wire(reader)).sequence, 0)
            self.assertEqual(codec.decode(await read_wire(reader)).sequence, 1)
            writer.close()
            await writer.wait_closed()

    async def test_wrong_session_state_closes_connection(self):
        reader, writer, codec = await self.connect()
        writer.write(codec.encode(Packet(0x2EE5)))
        await writer.drain()
        self.assertEqual(await asyncio.wait_for(reader.read(), 2), b'')
        self.assertTrue(any(e['event'] == 'session_error' for e in self.events))

    async def test_oversized_length_rejected_before_body_read(self):
        reader, writer, _ = await self.connect()
        writer.write(struct.pack('<H', self.config.max_frame + 1))
        await writer.drain()
        self.assertEqual(await asyncio.wait_for(reader.read(), 2), b'')

    async def test_truncated_frame_and_timeout(self):
        reader, writer, _ = await self.connect()
        writer.write(struct.pack('<H', 100) + b'\x01')
        await writer.drain()
        writer.write_eof()
        self.assertEqual(await asyncio.wait_for(reader.read(), 2), b'')
        self.assertTrue(any(e['event'] == 'truncated_frame' for e in self.events))
        reader, _, _ = await self.connect()
        self.assertEqual(await asyncio.wait_for(reader.read(), 2), b'')
        self.assertTrue(any(e.get('reason') == 'TimeoutError' for e in self.events))

    async def test_connection_limit(self):
        await self.server.close()
        self.server = await ResearchServer(replace(self.config, max_connections=1), self.events.append).start()
        await self.connect()
        reader, writer = await asyncio.open_connection('127.0.0.1', self.server.port)
        self.writers.append(writer)
        self.assertEqual(await asyncio.wait_for(reader.read(), 2), b'')
        self.assertTrue(any(e['event'] == 'connection_limit' for e in self.events))


if __name__ == '__main__':
    unittest.main()
