using System;
using System.IO;
using System.Linq;

namespace SocketClient.Protocol
{
    internal static class Phase2ChallengeDecoder
    {
        public static byte[] Decode(byte[] frame)
        {
            Console.WriteLine("[phase2] decoding server challenge");

            ushort totalLen = ReadU16(frame, 0);
            ushort msgType  = ReadU16(frame, 2);
            ushort phase    = ReadU16(frame, 6);
            ushort flags    = ReadU16(frame, 8);
            uint   sig      = ReadU32(frame, 10);

            Console.WriteLine($"[phase2] totalLen = {totalLen}");
            Console.WriteLine($"[phase2] msgType  = 0x{msgType:X4}");
            Console.WriteLine($"[phase2] phase    = 0x{phase:X4}");
            Console.WriteLine($"[phase2] flags    = 0x{flags:X4}");
            Console.WriteLine($"[phase2] sig      = 0x{sig:X8}");

            int payloadOffset = 14;
            byte[] payload = frame.Skip(payloadOffset).ToArray();

            Console.WriteLine($"[phase2] payload length = {payload.Length}");
            HexDump.Dump(payload, Math.Min(payload.Length, 256), "[phase2-payload]");

            File.WriteAllBytes(
                $"captures/phase2-payload-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.bin",
                payload
            );

            return payload;
        }

        private static ushort ReadU16(byte[] b, int o)
            => (ushort)((b[o] << 8) | b[o + 1]);

        private static uint ReadU32(byte[] b, int o)
            => (uint)((b[o] << 24) | (b[o + 1] << 16) | (b[o + 2] << 8) | b[o + 3]);
    }
}
