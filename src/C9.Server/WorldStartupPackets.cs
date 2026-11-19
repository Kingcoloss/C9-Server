using System.Buffers.Binary;
using C9.Protocol;

namespace C9.Server;

public static class WorldStartupPackets
{
    // Ref server 2014: 006A3A90 replies to 0BC5 with a plain 21-byte 0BC6.
    // Current client 008EF110 reads the object id at decoded +0D/+11, starts
    // the entity action and queues event 0x118 when it is the local actor.
    public static Packet ActionStarted(ulong objectId)
    {
        byte[] payload = new byte[sizeof(ulong)];
        BinaryPrimitives.WriteUInt64LittleEndian(payload, objectId);
        return new Packet(0x0BC6, payload, flag: 0);
    }
}
