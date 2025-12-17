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
        // =========================================================
        // Target
        // =========================================================

        private const string HOST = "auth.deltaworlds.com";
        private const int PORT = 6671;

        // =========================================================
        // Entry
        // =========================================================

        static void Main(string[] args)
        {
            Log("starting");

            Directory.CreateDirectory("captures");

            var stopwatch = Stopwatch.StartNew();

            using var client = new TcpClient
            {
                NoDelay = true
            };

            // -----------------------------------------------------
            // PHASE 1: TCP CONNECT
            // -----------------------------------------------------

            Log("connecting...");
            client.Connect(HOST, PORT);
            Log($"connected @ {stopwatch.ElapsedMilliseconds}ms");

            using var stream = client.GetStream();
            stream.ReadTimeout = 5000;
            stream.WriteTimeout = 5000;

            Thread.Sleep(TimingProfile.AfterConnectDelay);

            // -----------------------------------------------------
            // PHASE 2: VERIFIED CLIENT HELLO
            // -----------------------------------------------------

            byte[] clientHello =
            {
                0x00, 0x0A,
                0x00, 0x02,
                0x00, 0x24,
                0x00, 0x03,
                0x00, 0x00
            };

            Log("sending client hello");
            SendFrame(stream, clientHello, "client hello");

            Thread.Sleep(TimingProfile.BeforeLoginDelay);

            // -----------------------------------------------------
            // PHASE 3: RECEIVE SERVER ENVELOPE
            // -----------------------------------------------------

            var buffer = new byte[8192];
            int read;

            try
            {
                read = stream.Read(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                Log($"READ ERROR: {ex.GetType().Name} {ex.Message}");
                return;
            }

            if (read <= 0)
            {
                Log("server closed connection");
                return;
            }

            Log($"received server envelope ({read} bytes)");

            byte[] envelope = buffer.Take(read).ToArray();

            string envPath = Path.Combine("captures", "server-envelope.bin");
            File.WriteAllBytes(envPath, envelope);
            Log($"saved raw envelope to {envPath}");

            HexDump.Dump(envelope, envelope.Length, "[RX]");

            // -----------------------------------------------------
            // PHASE 4: DECODE SERVER ENVELOPE
            // -----------------------------------------------------

            AuthEnvelopeDecoder.Decode(envelope);

            // -----------------------------------------------------
            // PHASE 5: PHASE-1 ACK (STATE ADVANCE)
            // -----------------------------------------------------

            byte[] phase1Ack =
            {
                0x00, 0x0A,
                0x00, 0x02,
                0x00, 0x24,
                0x00, 0x03,
                0x00, 0x00
            };

            Log("sending phase1 ACK");
            SendFrame(stream, phase1Ack, "phase1-ack");

            // -----------------------------------------------------
            // PHASE 6: PASSIVE RECEIVE LOOP (NEXT STATE)
            // -----------------------------------------------------

            Log("entering receive loop (post-ACK)");

            ReceiveLoop(client, stream);

            Log("exiting");
        }

        // =========================================================
        // Networking Helpers
        // =========================================================

        private static void SendFrame(NetworkStream stream, byte[] frame, string label)
        {
            try
            {
                stream.Write(frame, 0, frame.Length);
                stream.Flush();
                Log($"sent {label} ({frame.Length} bytes)");
            }
            catch (Exception ex)
            {
                Log($"WRITE ERROR [{label}]: {ex.GetType().Name} {ex.Message}");
                throw;
            }
        }

        private static void ReceiveLoop(TcpClient client, NetworkStream stream)
        {
            var buffer = new byte[8192];

            while (true)
            {
                if (!client.Connected)
                {
                    Log("socket disconnected");
                    break;
                }

                if (!stream.DataAvailable)
                {
                    Thread.Sleep(50);
                    continue;
                }

                int read;
                try
                {
                    read = stream.Read(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    Log($"READ ERROR: {ex.GetType().Name} {ex.Message}");
                    break;
                }

                if (read == 0)
                {
                    Log("server closed connection");
                    break;
                }

                byte[] packet = buffer.Take(read).ToArray();

                Log($"received {read} bytes (post-ACK)");
                HexDump.Dump(packet, packet.Length, "[RX]");

                // Save follow-up packets for analysis
                string fname = $"captures/server-followup-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.bin";
                File.WriteAllBytes(fname, packet);
            }
        }

        // =========================================================
        // Logging
        // =========================================================

        private static void Log(string message)
        {
            Console.WriteLine($"[shadow] {message}");
        }
    }
}
