using System;

namespace SocketClient.Protocol
{
    internal static class Phase2LoginChallengeDecoder
    {
        public static void Decode(byte[] frame)
        {
            if (frame.Length < 10)
            {
                Console.WriteLine("[phase2] frame too short");
                return;
            }

            ushort totalLen = ReadU16(frame, 0);
            ushort msgType  = ReadU16(frame, 2);
            ushort flags    = ReadU16(frame, 4);
            ushort phase    = ReadU16(frame, 6);
            uint signature  = ReadU32(frame, 8);

            Console.WriteLine("[phase2] decoding server challenge");
            Console.WriteLine($"[phase2] totalLen = {totalLen}");
            Console.WriteLine($"[phase2] msgType  = 0x{msgType:X4}");
            Console.WriteLine($"[phase2] phase    = 0x{phase:X4}");
            Console.WriteLine($"[phase2] flags    = 0x{flags:X4}");
            Console.WriteLine($"[phase2] sig      = 0x{signature:X8}");

            int payloadOffset = 12;
            int payloadLen = frame.Length - payloadOffset;

            Console.WriteLine($"[phase2] payload length = {payloadLen}");

            // DO NOT inflate yet — capture-only phase
            Console.WriteLine("[phase2] payload left untouched (by design)");
        }

        private static ushort ReadU16(byte[] b, int o)
            => (ushort)((b[o] << 8) | b[o + 1]);

        private static uint ReadU32(byte[] b, int o)
            => (uint)((b[o] << 24) | (b[o + 1] << 16) | (b[o + 2] << 8) | b[o + 3]);
    }
}
