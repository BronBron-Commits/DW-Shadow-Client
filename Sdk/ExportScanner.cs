using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

internal static class ExportScanner
{
    public static void Dump()
    {
        TraceLog.Info("Dumping ClassicSdk exports (via dumpbin)...");

        var psi = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c dumpbin /exports ClassicSdk.dll",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var p = Process.Start(psi);
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();

        foreach (var line in output.Split('\n').Where(l => l.Contains("aw_")))
        {
            TraceLog.Info(line.Trim());
        }
    }
}
