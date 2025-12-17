using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace SocketClient
{
    internal static class ShadowClient
    {
        private const string HOST = "auth.deltaworlds.com";
        private const int PORT = 6671;

        static void Main(string[] args)
        {
            Log("starting");

            var stopwatch = Stopwatch.StartNew();

            using var client = new TcpClient
            {
                NoDelay = true
            };

            // -------------------------------
            // PHASE 1: TCP CONNECT
            // -------------------------------

            Log("connecting...");
            client.Connect(HOST, PORT);
            Log($"connected @ {stopwatch.ElapsedMilliseconds}ms");

            using var stream = client.GetStream();
            stream.ReadTimeout = 2000;
            stream.WriteTimeout = 2000;

            Thread.Sleep(TimingProfile.AfterConnectDelay);

            // -------------------------------
            // PHASE 2: REAL CLIENT HELLO
            // -------------------------------

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

            // Small delay observed in SDK
            Thread.Sleep(TimingProfile.BeforeLoginDelay);

            // -------------------------------
            // PHASE 2b: PLACEHOLDER FOLLOW-UP
            // -------------------------------

            Log("sending placeholder follow-up");

            byte[] followUpFrame =
            {
                0x00, 0x00, 0x00, 0x00
            };

            SendFrame(stream, followUpFrame, "follow-up");

            Log("handshake frames sent, entering receive loop");

            // -------------------------------
            // PHASE 3: RECEIVE
            // -------------------------------

            ReceiveLoop(client, stream);

            Log("exiting");
        }

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

                HexDump.Dump(buffer, read, "[RX]");
            }
        }

        private static void Log(string message)
        {
            Console.WriteLine($"[shadow] {message}");
        }
    }
}
