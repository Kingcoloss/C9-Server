"""Only message layouts established by static evidence; entries stay opaque."""
import struct

from .protocol import Packet, ProtocolError


def server_list_packet(entries):
    entries = tuple(entries)
    if len(entries) > 255 or any(not isinstance(entry, bytes) or len(entry) != 52 for entry in entries):
        raise ProtocolError('Server list requires at most 255 opaque 52-byte entries')
    return Packet(0x2EE6, bytes([len(entries)]) + b''.join(entries))


def parse_server_list(packet):
    if packet.kind != 0x2EE6 or not packet.payload:
        raise ProtocolError('Expected server list with a count byte')
    count = packet.payload[0]
    required = 1 + count * 52
    if len(packet.payload) < required:
        raise ProtocolError('Server list count exceeds payload')
    entries = tuple(packet.payload[1 + i * 52:1 + (i + 1) * 52] for i in range(count))
    return entries, packet.payload[required:]


def server_entry_lookup_key(entry):
    if len(entry) != 52:
        raise ProtocolError('Server entry must be 52 bytes')
    return struct.unpack_from('<I', entry, 1)[0]
