namespace C9.Protocol;

public enum ProtocolError
{
    InvalidLength, InvalidPacket, InvalidPadding, ChecksumMismatch,
    UnexpectedSequence, IncompleteFrame
}

public class ProtocolException(ProtocolError code, string message) : Exception(message)
{
    public ProtocolError Code { get; } = code;
}
