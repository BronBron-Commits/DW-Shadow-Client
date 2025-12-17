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

            // -------------------------------
            // PHASE 1: CLIENT HELLO
            // -------------------------------
            byte[] clientHello =
            {
                0x00, 0x0A,
                0x00, 0x02,
                0x00, 0x24,
                0x00, 0x03,
                0x00, 0x00
            };

            Send(stream, clientHello, "client-hello");

            // -------------------------------
            // RECEIVE PHASE-1 ENVELOPE
            // -------------------------------
            byte[] buffer = new byte[8192];
            int read = stream.Read(buffer, 0, buffer.Length);
            if (read <= 0)
            {
                Log("server closed connection");
                return;
            }

            byte[] phase1 = buffer.Take(read).ToArray();
            File.WriteAllBytes("captures/phase1.bin", phase1);

            Log($"received phase1 ({read} bytes)");
            HexDump.Dump(phase1, phase1.Length, "[RX]");

            AuthEnvelopeDecoder.Decode(phase1);

            // -------------------------------
            // SEND LOGIN FRAME (INTENTIONALLY INVALID)
            // -------------------------------
            Log("sending login-frame candidate");
            Send(stream, clientHello, "login-frame");

            // -------------------------------
            // RECEIVE PHASE-2 (OR REPLAY)
            // -------------------------------
            Log("waiting for phase-2 challenge");

            read = stream.Read(buffer, 0, buffer.Length);
            if (read <= 0)
            {
                Log("server closed connection");
                return;
            }

            byte[] phase2 = buffer.Take(read).ToArray();
            File.WriteAllBytes("captures/phase2.bin", phase2);

            Log($"received phase2 ({read} bytes)");
            HexDump.Dump(phase2, phase2.Length, "[RX]");

            Phase2LoginChallengeDecoder.Decode(phase2);

            Log("phase-2 captured successfully - exiting cleanly");
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
