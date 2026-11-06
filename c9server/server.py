"""Local research TCP server with explicit fixture response rules."""
import asyncio
import ipaddress
import json
import struct
from dataclasses import dataclass, field
from pathlib import Path

from .protocol import ConnectionCodec, Packet, ProtocolError


def packet_definition(value):
    allowed = {'type', 'flag', 'payload_hex'}
    if set(value) - allowed:
        raise ValueError('Unknown packet definition field')
    return Packet(int(value['type']), bytes.fromhex(value.get('payload_hex', '')), int(value.get('flag', 1)))


@dataclass(frozen=True)
class Rule:
    state: str
    request_type: int
    payload_length: int
    next_state: str
    replies: tuple[Packet, ...]


@dataclass(frozen=True)
class ServerConfig:
    host: str = '127.0.0.1'
    port: int = 14000
    profile: str = 'probe'
    max_frame: int = 8192
    max_connections: int = 4
    max_packets: int = 64
    timeout_seconds: float = 10
    greetings: tuple[Packet, ...] = ()
    rules: tuple[Rule, ...] = ()

    def __post_init__(self):
        if not ipaddress.ip_address(self.host).is_loopback:
            raise ValueError('Research server requires a loopback bind address')
        if not 0 <= self.port <= 65535 or self.profile not in ('probe', 'fixture', 'observer'):
            raise ValueError('Invalid port or profile')
        if not 20 <= self.max_frame <= 65535 or not 1 <= self.max_connections <= 32:
            raise ValueError('Invalid frame or connection limit')
        if not 1 <= self.max_packets <= 255 or not 0 < self.timeout_seconds <= 300:
            raise ValueError('Limits must stay within the implemented cipher epoch')
        if self.profile == 'probe' and (self.greetings or self.rules):
            raise ValueError('Passive probe cannot send greetings or replies')
        if self.profile == 'observer' and (self.rules or len(self.greetings) != 1 or
                self.greetings[0].kind != 4 or self.greetings[0].flag != 0 or
                self.greetings[0].payload not in (b'\0', b'\1')):
            raise ValueError('Observer requires one plain type-4 mode-0/1 greeting and no response rules')
        keys = [(r.state, r.request_type) for r in self.rules]
        if len(keys) != len(set(keys)):
            raise ValueError('Duplicate state/type rule')
        # Preflight reply sizes and cipher budget before accepting connections.
        for rule in self.rules:
            if not 0 <= rule.request_type <= 65535 or not 0 <= rule.payload_length <= self.max_frame - 13:
                raise ValueError('Rule request outside configured bounds')
            if len(rule.replies) > 3:
                raise ValueError('A fixture rule supports at most three replies')
        if len(self.greetings) > 3:
            raise ValueError('At most three greeting packets are supported')
        if 3 * self.max_packets + len(self.greetings) > 255:
            raise ValueError('Reply budget can exceed initial cipher epoch')
        for packet in self.greetings + tuple(p for r in self.rules for p in r.replies):
            size = 13 + len(packet.payload)
            if packet.flag:
                size = 3 + ((size - 3 + 15) // 16) * 16 + 1
            if size > self.max_frame:
                raise ValueError('Fixture reply exceeds configured maximum')

    @classmethod
    def load(cls, path: Path, **overrides):
        data = json.loads(path.read_text())
        allowed = {'profile', 'host', 'port', 'max_frame', 'max_connections', 'max_packets',
                   'timeout_seconds', 'greetings', 'rules', 'description'}
        if set(data) - allowed:
            raise ValueError('Unknown configuration field')
        greetings = tuple(packet_definition(p) for p in data.pop('greetings', []))
        rules = []
        for r in data.pop('rules', []):
            if set(r) != {'state', 'request_type', 'payload_length', 'next_state', 'replies'}:
                raise ValueError('Invalid fixture rule fields')
            rules.append(Rule(str(r['state']), int(r['request_type']), int(r['payload_length']),
                              str(r['next_state']), tuple(packet_definition(p) for p in r['replies'])))
        data.pop('description', None)
        data.update({k: v for k, v in overrides.items() if v is not None})
        return cls(**data, greetings=greetings, rules=tuple(rules))


def json_event(event):
    print(json.dumps(event, separators=(',', ':')), flush=True)


@dataclass
class ResearchServer:
    config: ServerConfig
    emit: object = json_event
    listener: object = field(default=None, init=False)
    sessions: set = field(default_factory=set, init=False)
    _session_number: int = field(default=0, init=False)

    async def start(self):
        # Check runtime hash support before publishing the listening socket.
        ConnectionCodec(max_frame=self.config.max_frame)
        self.listener = await asyncio.start_server(self._handle, self.config.host, self.config.port,
                                                   limit=self.config.max_frame + 2)
        self.emit({'event': 'listening', 'host': self.config.host, 'port': self.port,
                   'profile': self.config.profile, 'client_compatibility': 'unverified',
                   'cipher_epoch_limit': 256})
        return self

    @property
    def port(self):
        return self.listener.sockets[0].getsockname()[1]

    async def close(self):
        if self.listener:
            self.listener.close()
            await self.listener.wait_closed()
        sessions = tuple(self.sessions)
        for task in sessions:
            task.cancel()
        if sessions:
            await asyncio.gather(*sessions, return_exceptions=True)

    async def _handle(self, reader, writer):
        task = asyncio.current_task()
        if len(self.sessions) >= self.config.max_connections:
            self.emit({'event': 'connection_limit'})
            writer.close()
            await writer.wait_closed()
            return
        self.sessions.add(task)
        self._session_number += 1
        session = self._session_number
        codec = ConnectionCodec(max_frame=self.config.max_frame, discover_receive_selector=True)
        state = 'observing' if self.config.profile == 'observer' else 'connected'
        rules = {(r.state, r.request_type): r for r in self.config.rules}
        self.emit({'event': 'connected', 'session': session})

        async def send(packet):
            data = codec.encode(packet)
            writer.write(data)
            await asyncio.wait_for(writer.drain(), self.config.timeout_seconds)
            self.emit({'event': 'sent', 'session': session, 'type': packet.kind, 'wire_length': len(data)})

        try:
            for packet in self.config.greetings:
                await send(packet)
            for _ in range(self.config.max_packets):
                prefix = await asyncio.wait_for(reader.readexactly(2), self.config.timeout_seconds)
                size = struct.unpack('<H', prefix)[0]
                if not 13 <= size <= self.config.max_frame:
                    raise ProtocolError('Invalid frame length')
                rest = await asyncio.wait_for(reader.readexactly(size - 2), self.config.timeout_seconds)
                packet = codec.decode(prefix + rest)
                # Payload bytes and credentials are never written to the event log.
                self.emit({'event': 'received', 'session': session, 'type': packet.kind,
                           'payload_length': len(packet.payload), 'flag': packet.flag,
                           'sequence': packet.sequence, 'state': state})
                if self.config.profile in ('probe', 'observer'):
                    continue
                rule = rules.get((state, packet.kind))
                if rule is None:
                    raise ProtocolError('No fixture rule for session state/type')
                if len(packet.payload) != rule.payload_length:
                    raise ProtocolError('Fixture request payload length mismatch')
                for reply in rule.replies:
                    await send(reply)
                state = rule.next_state
                self.emit({'event': 'state', 'session': session, 'state': state})
            self.emit({'event': 'packet_limit', 'session': session})
        except asyncio.IncompleteReadError as error:
            self.emit({'event': 'truncated_frame' if error.partial else 'peer_closed', 'session': session})
        except (ProtocolError, asyncio.TimeoutError, ConnectionError) as error:
            self.emit({'event': 'session_error', 'session': session,
                       'reason': str(error) if isinstance(error, ProtocolError) else type(error).__name__})
        finally:
            self.sessions.discard(task)
            writer.close()
            try:
                await writer.wait_closed()
            except ConnectionError:
                pass
            self.emit({'event': 'disconnected', 'session': session})
