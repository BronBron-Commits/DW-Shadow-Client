using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace SocketClient
{
    internal static class ShadowClient
    {
        private const string HOST = "auth.deltaworlds.com";
        private const int PORT = 6671;
        private const int PHASE1_LEN = 107;

        static void Main()
        {
            Log("starting");

            Directory.CreateDirectory("captures");

            var sw = Stopwatch.StartNew();

            using var client = new TcpClient
            {
                NoDelay = true // critical: match SDK behavior
            };

            Log("connecting...");
            client.Connect(HOST, PORT);
            Log($"connected @ {sw.ElapsedMilliseconds}ms");

            using var stream = client.GetStream();
            stream.ReadTimeout = 8000;
            stream.WriteTimeout = 8000;

            // -------------------------------------------------
            // PHASE 1: CLIENT HELLO (VERIFIED)
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
            // PHASE 1: RECEIVE SERVER ENVELOPE (EXACT 107 BYTES)
            // -------------------------------------------------
            byte[] phase1 = new byte[PHASE1_LEN];
            int offset = 0;

            while (offset < PHASE1_LEN)
            {
                int n = stream.Read(phase1, offset, PHASE1_LEN - offset);
                if (n <= 0)
                {
                    Log("server closed connection during phase1");
                    return;
                }
                offset += n;
            }

            File.WriteAllBytes("captures/server-phase1.bin", phase1);

            Log($"received phase1 ({PHASE1_LEN} bytes)");
            HexDump(phase1);

            // -------------------------------------------------
            // IMPORTANT: DO NOT SEND ANYTHING HERE
            // Let TCP ACK occur naturally
            // -------------------------------------------------
            Log("phase1 fully read; idling to allow TCP ACK");

            // Small idle window to ensure ACK emission
            Thread.Sleep(500);

            // -------------------------------------------------
            // PHASE 2: WAIT FOR SERVER RESPONSE
            // -------------------------------------------------
            Log("waiting for server response...");

            var buffer = new byte[8192];
            int read = stream.Read(buffer, 0, buffer.Length);

            if (read <= 0)
            {
                Log("no response after phase1");
                return;
            }

            byte[] phase2 = new byte[read];
            Array.Copy(buffer, phase2, read);

            File.WriteAllBytes("captures/server-phase2.bin", phase2);

            Log($"received server response ({read} bytes)");
            HexDump(phase2);

            Log("done");
        }

        private static void Send(NetworkStream s, byte[] data, string label)
        {
            s.Write(data, 0, data.Length);
            s.Flush();
            Log($"sent {label}");
        }

        private static void Log(string msg)
        {
            Console.WriteLine($"[shadow] {msg}");
        }

        private static void HexDump(byte[] data)
        {
            Console.Write("[RX] ");
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write($"{data[i]:X2} ");
            }
            Console.WriteLine();
        }
    }
}
