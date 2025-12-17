namespace SocketClient.Protocol
{
    internal struct AuthEnvelope
    {
        public ushort TotalLength;
        public ushort MessageType;
        public ushort Flags;
        public ushort Phase;
        public ushort Reserved;
        public byte[] Payload;
    }
}
