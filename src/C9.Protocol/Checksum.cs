namespace C9.Protocol;

public static class Checksum
{
    private const uint Seed = 0xBD136D48;
    private const uint Multiplier = 0x03FAA1FF;

    // The original routine sign-extends each byte with MOVSX.
    public static uint Compute(ReadOnlySpan<byte> data)
    {
        uint value = Seed;
        unchecked
        {
            foreach (byte item in data)
                value = value * Multiplier ^ (uint)(int)(sbyte)item;
        }
        return value;
    }

    public static uint ComputeDecoded(ReadOnlySpan<byte> frame)
    {
        if (frame.Length < Packet.HeaderSize)
            throw new ProtocolException(ProtocolError.InvalidLength, "Decoded frame shorter than header");
        uint value = Seed;
        unchecked
        {
            for (int i = 3; i < frame.Length; i++)
            {
                byte item = i == 5 ? (byte)100 : i is >= 6 and <= 8 ? (byte)0 : frame[i];
                value = value * Multiplier ^ (uint)(int)(sbyte)item;
            }
        }
        return value;
    }
}
