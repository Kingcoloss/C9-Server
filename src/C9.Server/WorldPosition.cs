using System.Buffers.Binary;
using C9.Protocol;

namespace C9.Server;

/// <summary>
/// Local player position reported by client b4580ed4 while in the world. 0x0BCE (46 B, move) and 0x0BCC (36 B, action)
/// both start with f32 x, y, z on the same axes as 0x0BC3 +6 and end with the client's timeGetTime tick. Trace
/// lobby/20261120/e3bf4b-001.jsonl session 5: the first 0x0BCE repeats the 0x0BC3 spawn point exactly and the later
/// ones form one continuous path. The fields in between (yaw, motion hash, flags) are not decoded yet.
/// </summary>
public static class WorldPositionPackets
{
    public const int MoveBytes = 46;
    public const int ActionBytes = 36;

    public static bool TryRead(Packet packet, out MapPosition position)
    {
        position = null!;
        if ((packet.Type, packet.Payload.Length) is not ((0x0BCE, MoveBytes) or (0x0BCC, ActionBytes))) return false;
        ReadOnlySpan<byte> payload = packet.Payload.Span;
        MapPosition read = new(BinaryPrimitives.ReadSingleLittleEndian(payload),
            BinaryPrimitives.ReadSingleLittleEndian(payload[4..]), BinaryPrimitives.ReadSingleLittleEndian(payload[8..]));
        if (!read.IsValid) return false;
        position = read;
        return true;
    }
}

/// <summary>
/// Remembers where the selected character is (S01). It starts at the spawn point sent in 0x0BC3, follows
/// 0x0BCE/0x0BCC, and writes to the character store at most once per <see cref="SaveInterval"/> while the player moves,
/// and once more when the session leaves the world (0x2EEA, 0x17D4 or disconnect). Each save adds the play time since
/// the previous one. The map stays the entry map until map changes (S05) exist.
/// </summary>
internal sealed class WorldPositionTracker(CharacterLobby lobby, ulong characterId, int mapId, int submap, MapPosition spawn,
    TimeProvider? clock = null)
{
    public static readonly TimeSpan SaveInterval = TimeSpan.FromSeconds(30);

    private readonly TimeProvider time = clock ?? TimeProvider.System;
    // Play time is counted from the last successful save; the interval from the last attempt, so a database that is
    // down is retried every SaveInterval rather than on every movement packet.
    private long lastSave = (clock ?? TimeProvider.System).GetTimestamp();
    private long lastAttempt = (clock ?? TimeProvider.System).GetTimestamp();
    private MapPosition current = spawn;
    private bool moved;

    /// <summary>True when <paramref name="packet"/> carried a position and a periodic save is now due.</summary>
    public bool Observe(Packet packet)
    {
        if (!WorldPositionPackets.TryRead(packet, out MapPosition position)) return false;
        if (position != current) (current, moved) = (position, true);
        return moved && time.GetElapsedTime(lastAttempt) >= SaveInterval;
    }

    /// <summary>
    /// Saves and reports the outcome as <paramref name="saved"/> or <c>position_save_failed</c>. A failure (database not
    /// ready, timeout, server stop) never ends the session; the next save tries again.
    /// </summary>
    public async ValueTask<bool> TrySaveAsync(IServerEvents events, ServerEvent saved, CancellationToken token)
    {
        try
        {
            await SaveAsync(token);
            events.Emit(saved with { CharacterId = characterId, MapId = mapId });
            return true;
        }
        catch (Exception error)
        {
            events.Emit(saved with
            {
                Event = "position_save_failed", CharacterId = characterId, MapId = mapId, Reason = error.GetType().Name + ": " + error.Message
            });
            return false;
        }
    }

    /// <summary>Writes the current position and the play time since the previous save.</summary>
    public async ValueTask SaveAsync(CancellationToken token)
    {
        lastAttempt = time.GetTimestamp();
        long seconds = (long)time.GetElapsedTime(lastSave).TotalSeconds;
        await lobby.SavePositionAsync(characterId, new SavedPosition(mapId, submap, current), seconds, token);
        // Whole seconds only; the remainder carries over to the next save.
        lastSave += seconds * time.TimestampFrequency;
        moved = false;
    }
}
