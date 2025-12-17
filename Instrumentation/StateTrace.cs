using System;
using System.Diagnostics;
using System.Threading;

namespace ConsoleApp.Instrumentation
{
    internal static class StateTrace
    {
        private static readonly Stopwatch _clock = Stopwatch.StartNew();
        private static long _seq = 0;

        public static long NextSeq() => Interlocked.Increment(ref _seq);

        public static void Log(
            string call,
            string parameters = "",
            int? rc = null)
        {
            long seq = NextSeq();
            long us = _clock.ElapsedTicks * 1_000_000 / Stopwatch.Frequency;
            int tid = Environment.CurrentManagedThreadId;

            if (rc.HasValue)
            {
                Console.WriteLine(
                    $"[{seq:D6}] +{us:D10}us [T{tid}] {call}({parameters}) -> rc={rc.Value}");
            }
            else
            {
                Console.WriteLine(
                    $"[{seq:D6}] +{us:D10}us [T{tid}] {call}({parameters})");
            }
        }
    }
}
