using System;
using System.Runtime.InteropServices;

namespace DeltaWorlds.Injection
{
    public static class EntryPoint
    {
        // The EntryPoint string must match the native "method_name" argument exactly.
        [UnmanagedCallersOnly(EntryPoint = "Load")]
        public static int Load(IntPtr arg, int size)
        {
            // KEEP THIS EMPTY for the first successful test. 
            // Logging or complex logic here can crash before the return if dependencies aren't loaded.
            return 1337; // Return a recognizable number to verify success
        }
    }
}