using System;
using HarmonyLib;
using Org.BouncyCastle.Crypto;

namespace ConsoleApp.Crypto
{
    internal static class BouncyCastleHooks
    {
        public static void Install()
        {
            var harmony = new Harmony("dw.crypto.bouncycastle.hooks");
            harmony.PatchAll();
        }
    }

    // =========================================================
    // BufferedBlockCipher.DoFinal(byte[] output, int outOff)
    // =========================================================
    [HarmonyPatch(
        typeof(BufferedBlockCipher),
        nameof(BufferedBlockCipher.DoFinal),
        new Type[] { typeof(byte[]), typeof(int) }
    )]
    internal static class BufferedBlockCipher_DoFinal_Hook
    {
        static void Prefix(object[] __args)
        {
            var output = (byte[])__args[0];
            var outOff = (int)__args[1];

            Console.WriteLine("[CRYPTO] BufferedBlockCipher.DoFinal PRE");
            Dump(output, outOff);
        }

        static void Postfix(object[] __args)
        {
            var output = (byte[])__args[0];
            var outOff = (int)__args[1];

            Console.WriteLine("[CRYPTO] BufferedBlockCipher.DoFinal POST");
            Dump(output, outOff);
        }

        private static void Dump(byte[] buf, int len)
        {
            if (buf == null) return;
            len = Math.Min(len, buf.Length);

            for (int i = 0; i < len; i += 16)
            {
                Console.Write($"{i:X4}: ");
                for (int j = 0; j < 16 && i + j < len; j++)
                    Console.Write($"{buf[i + j]:X2} ");
                Console.WriteLine();
            }
        }
    }

    // =========================================================
    // BufferedBlockCipher.ProcessBytes(byte[], int, int, byte[], int)
    // =========================================================
    [HarmonyPatch(
        typeof(BufferedBlockCipher),
        nameof(BufferedBlockCipher.ProcessBytes),
        new Type[] {
            typeof(byte[]), typeof(int), typeof(int),
            typeof(byte[]), typeof(int)
        }
    )]
    internal static class BufferedBlockCipher_ProcessBytes_Hook
    {
        static void Prefix(object[] __args)
        {
            var input  = (byte[])__args[0];
            var inOff  = (int)__args[1];
            var length = (int)__args[2];

            Console.WriteLine($"[CRYPTO] BufferedBlockCipher.ProcessBytes PRE length={length}");
            Dump(input, inOff, length);
        }

        static void Postfix(object[] __args)
        {
            var output = (byte[])__args[3];
            var outOff = (int)__args[4];
            var length = (int)__args[2];

            Console.WriteLine($"[CRYPTO] BufferedBlockCipher.ProcessBytes POST length={length}");
            Dump(output, outOff, length);
        }

        private static void Dump(byte[] buf, int off, int length)
        {
            if (buf == null) return;

            int end = Math.Min(off + length, buf.Length);
            int rel = 0;

            for (int i = off; i < end; i += 16)
            {
                Console.Write($"{rel:X4}: ");
                for (int j = 0; j < 16 && i + j < end; j++)
                    Console.Write($"{buf[i + j]:X2} ");
                Console.WriteLine();
                rel += 16;
            }
        }
    }
}
