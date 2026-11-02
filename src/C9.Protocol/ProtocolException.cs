namespace C9.Protocol;

public enum ProtocolError
{
    InvalidLength, InvalidPacket, InvalidPadding, ChecksumMismatch,
    UnexpectedSequence, IncompleteFrame, UnsupportedEpoch
}

public class ProtocolException(ProtocolError code, string message) : Exception(message)
{
    public ProtocolError Code { get; } = code;
}

public sealed class UnsupportedCipherEpochException() : ProtocolException(
    ProtocolError.UnsupportedEpoch,
    "Initial cipher epoch exhausted; subsequent key rotation is not implemented");
