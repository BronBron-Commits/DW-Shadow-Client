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

            // -------------------------------------------------
            // CLIENT HELLO (VERIFIED)
            // -------------------------------------------------
            byte[] clientHello =
            {
                0x00, 0x0A,
                0x00, 0x02,
                0x00, 0x24,
                0x00, 0x03,
                0x00, 0x00
            };

            Send(stream, clientHello, "client-hello");

            // -------------------------------------------------
            // RECEIVE PHASE-1 CAPABILITIES
            // -------------------------------------------------
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

            // -------------------------------------------------
            // PHASE-1 ACK (EXPLICIT)
            // -------------------------------------------------
            // NOTE:
            // This MUST match the real client ACK.
            // Replace this placeholder with captured bytes.
            byte[] phase1Ack =
            {
                // <<< REPLACE WITH WIRESHARK-CAPTURED ACK >>>
                0x00, 0x0A,
                0x00, 0x02,
                0x00, 0x24,
                0x00, 0x03,
                0x00, 0x00
            };

            Log("sending phase1-ack");
            Send(stream, phase1Ack, "phase1-ack");

            // -------------------------------------------------
            // OBSERVE SERVER RESPONSE
            // -------------------------------------------------
            Log("waiting for server response");

            while (client.Connected)
            {
                if (!stream.DataAvailable)
                {
                    Thread.Sleep(50);
                    continue;
                }

                int r = stream.Read(buffer, 0, buffer.Length);
                if (r <= 0)
                {
                    Log("server closed connection");
                    break;
                }

                byte[] pkt = buffer.Take(r).ToArray();
                string path = $"captures/server-after-ack-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.bin";
                File.WriteAllBytes(path, pkt);

                Log($"received {r} bytes");
                HexDump.Dump(pkt, pkt.Length, "[RX]");
            }

            Log("exit");
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
