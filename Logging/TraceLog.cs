using System;
using System.Diagnostics;

internal static class TraceLog
{
    public static void Info(string msg)
    {
        Console.WriteLine($"[{Stopwatch.GetTimestamp()}] {msg}");
    }

    public static void Error(string msg)
    {
        Console.WriteLine($"[{Stopwatch.GetTimestamp()}][ERR] {msg}");
    }
}
