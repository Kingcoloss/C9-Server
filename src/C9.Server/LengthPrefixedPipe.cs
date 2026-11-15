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
                    if (buffer.IsEmpty)
                    {
                        reader.AdvanceTo(buffer.End);
                        return null;
                    }
                    ProtocolException incomplete = new(ProtocolError.IncompleteFrame, "Incomplete final frame");
                    incomplete.Data[PreviewKey] = Preview(buffer);
                    reader.AdvanceTo(buffer.End);
                    throw incomplete;
                }
                reader.AdvanceTo(buffer.Start, buffer.End);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("Frame read deadline exceeded");
        }
    }

    /// <summary>
    /// Key in <see cref="Exception.Data"/> holding up to <see cref="PreviewBytes"/> unconsumed bytes
    /// when framing fails, so packet trace can keep the bytes that could not be framed.
    /// </summary>
    public const string PreviewKey = "c9.preview";
    public const int PreviewBytes = 256;

    private static byte[] Preview(ReadOnlySequence<byte> buffer) =>
        buffer.Slice(0, Math.Min(buffer.Length, PreviewBytes)).ToArray();

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
        {
            ProtocolException invalid = new(ProtocolError.InvalidLength, "Invalid frame length");
            invalid.Data[PreviewKey] = Preview(buffer);
            throw invalid;
        }
        if (buffer.Length < size) return false;
        frame = buffer.Slice(0, size).ToArray();
        consumed = buffer.GetPosition(size);
        return true;
    }
}
