using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using C9.Protocol;

namespace C9.Server;

public static class LengthPrefixedPipe
{
    public static async ValueTask<byte[]?> ReadFrameAsync(PipeReader reader, int maximum,
        TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (maximum is < Packet.HeaderSize or > ushort.MaxValue) throw new ArgumentOutOfRangeException(nameof(maximum));
        using CancellationTokenSource deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(timeout);
        try
        {
            while (true)
            {
                ReadResult result = await reader.ReadAsync(deadline.Token);
                ReadOnlySequence<byte> buffer = result.Buffer;
                if (result.IsCanceled) throw new OperationCanceledException(deadline.Token);
                if (TryReadFrame(buffer, maximum, out byte[]? frame, out SequencePosition consumed))
                {
                    reader.AdvanceTo(consumed);
                    return frame;
                }
                if (result.IsCompleted)
                {
                    reader.AdvanceTo(buffer.End);
                    if (buffer.IsEmpty) return null;
                    throw new ProtocolException(ProtocolError.IncompleteFrame, "Incomplete final frame");
                }
                reader.AdvanceTo(buffer.Start, buffer.End);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("Frame read deadline exceeded");
        }
    }

    private static bool TryReadFrame(ReadOnlySequence<byte> buffer, int maximum,
        out byte[]? frame, out SequencePosition consumed)
    {
        frame = null;
        consumed = buffer.Start;
        if (buffer.Length < 2) return false;
        Span<byte> prefix = stackalloc byte[2];
        buffer.Slice(0, 2).CopyTo(prefix);
        int size = BinaryPrimitives.ReadUInt16LittleEndian(prefix);
        if (size < Packet.HeaderSize || size > maximum)
            throw new ProtocolException(ProtocolError.InvalidLength, "Invalid frame length");
        if (buffer.Length < size) return false;
        frame = buffer.Slice(0, size).ToArray();
        consumed = buffer.GetPosition(size);
        return true;
    }
}
