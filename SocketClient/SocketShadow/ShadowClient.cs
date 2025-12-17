using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using SocketClient.Protocol;

namespace SocketClient
{
    internal static class ShadowClient
    {
        private const string HOST = "auth.deltaworlds.com";
        private const int PORT = 6671;

        static void Main()
        {
            Log("starting");
            Directory.CreateDirectory("captures");

            var sw = Stopwatch.StartNew();

            using var client = new TcpClient { NoDelay = true };

            Log("connecting...");
            client.Connect(HOST, PORT);
            Log($"connected @ {sw.ElapsedMilliseconds}ms");

            using var stream = client.GetStream();
            stream.ReadTimeout = 5000;
            stream.WriteTimeout = 5000;

            Thread.Sleep(TimingProfile.AfterConnectDelay);

            // =====================================================
            // PHASE 1 — VERIFIED CLIENT HELLO
            // =====================================================
            byte[] clientHello =
            {
                0x00, 0x0A,
                0x00, 0x02,
                0x00, 0x24,
                0x00, 0x03,
                0x00, 0x00
            };

            Send(stream, clientHello, "client-hello");

            Thread.Sleep(TimingProfile.BeforeLoginDelay);

            // =====================================================
            // PHASE 1 RESPONSE — SERVER ENVELOPE
            // =====================================================
            byte[] buffer = new byte[8192];
            int read = stream.Read(buffer, 0, buffer.Length);

            if (read <= 0)
            {
                Log("server closed connection");
                return;
            }

            byte[] envelope = buffer.Take(read).ToArray();
            File.WriteAllBytes("captures/server-envelope.bin", envelope);

            Log($"received server envelope ({read} bytes)");
            HexDump.Dump(envelope, envelope.Length, "[RX]");

            AuthEnvelopeDecoder.Decode(envelope);

            // =====================================================
            // PHASE 2 — LOGIN FRAME (STRUCTURAL REPLAY)
            // =====================================================
            Log("building login-frame candidate");

            byte[] loginFrame = AuthFrameBuilder.Build(
                messageType: 0x0000,
                flags: 0xFFFF,
                phase: 0x0002,
                inflatedPayload: AuthEnvelopeDecoder.LastInflatedPayload
            );

            Send(stream, loginFrame, "login-frame");

            // =====================================================
            // PHASE 2 RESPONSE — SERVER CHALLENGE
            // =====================================================
            Log("waiting for phase-2 login response");

            read = stream.Read(buffer, 0, buffer.Length);
            if (read <= 0)
            {
                Log("server closed connection");
                return;
            }

            byte[] phase2 = buffer.Take(read).ToArray();
            File.WriteAllBytes("captures/phase2-challenge.bin", phase2);

            Log($"received {read} bytes");
            HexDump.Dump(phase2, phase2.Length, "[RX]");

            Phase2ChallengeDecoder.Decode(phase2);

            Log("complete — exiting cleanly");
        }

        private static void Send(NetworkStream s, byte[] data, string label)
        {
            s.Write(data, 0, data.Length);
            s.Flush();
            Log($"sent {label} ({data.Length} bytes)");
        }

        private static void Log(string msg)
        {
            Console.WriteLine($"[shadow] {msg}");
        }
    }
}
