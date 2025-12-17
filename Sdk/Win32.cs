using System;
using System.Runtime.InteropServices;

internal static class Win32
{
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();
}
