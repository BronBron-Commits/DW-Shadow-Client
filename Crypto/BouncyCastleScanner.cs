using System;
using System.Linq;
using System.Reflection;

internal static class BouncyCastleScanner
{
    public static void Scan()
    {
        TraceLog.Info("Scanning loaded assemblies for BouncyCastle...");

        var bcAsm = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a =>
                a.GetName().Name != null &&
                a.GetName().Name.Contains("BouncyCastle", StringComparison.OrdinalIgnoreCase));

        if (bcAsm == null)
        {
            TraceLog.Error("BouncyCastle assembly NOT loaded yet");
            return;
        }

        TraceLog.Info($"BouncyCastle loaded: {bcAsm.FullName}");

        var interesting = bcAsm.GetTypes()
            .Where(t =>
                t.Name.Contains("Cipher", StringComparison.OrdinalIgnoreCase) ||
                t.Name.Contains("Engine", StringComparison.OrdinalIgnoreCase) ||
                t.Name.Contains("Encrypt", StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.FullName);

        foreach (var t in interesting)
        {
            TraceLog.Info($"[BC] {t.FullName}");
        }
    }
}
