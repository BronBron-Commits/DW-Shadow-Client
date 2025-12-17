using System.Runtime.InteropServices;

internal static class DwSdkNative
{
    private const string DLL = "ClassicSdk.dll";

    [DllImport("ClassicSdk.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int aw_init_bind();

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int aw_init(int level);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int aw_create();

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int aw_destroy();

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int aw_login();

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int aw_wait(int ms);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int aw_string_set(int attr, string value);

    [DllImport(DLL, CallingConvention = CallingConvention.Cdecl)]
    public static extern int aw_int_set(int attr, int value);

    public const int AW_ADDRESS  = 1;
    public const int AW_PORT     = 2;
    public const int AW_USERNAME = 3;
    public const int AW_PASSWORD = 4;
    public const int AW_HANDLE   = 100;  // window handle
    public const int AW_INSTANCE = 101;  // instance handle

}
