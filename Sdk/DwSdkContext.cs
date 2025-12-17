using System;

internal sealed class DwSdkContext : IDisposable
{
    private bool _initialized;
    private bool _created;

    public void Init(
    string address = "auth.deltaworlds.com",
    int port = 6671,
    string username = "test",
    string password = "test",
    int logLevel = 1)
{
    // Native handles
    IntPtr hwnd  = Win32.GetConsoleWindow();
    IntPtr hinst = Win32.GetModuleHandle(null);

    int hwnd32  = unchecked((int)hwnd.ToInt64());
    int hinst32 = unchecked((int)hinst.ToInt64());

    TraceLog.Info($"aw_int_set(AW_HANDLE, 0x{hwnd32:X})");
    Check(DwSdkNative.aw_int_set(DwSdkNative.AW_HANDLE, hwnd32));

    TraceLog.Info($"aw_int_set(AW_INSTANCE, 0x{hinst32:X})");
    Check(DwSdkNative.aw_int_set(DwSdkNative.AW_INSTANCE, hinst32));

    // Network (REQUIRED before bind)
    TraceLog.Info($"aw_string_set(AW_ADDRESS, {address})");
    Check(DwSdkNative.aw_string_set(DwSdkNative.AW_ADDRESS, address));

    TraceLog.Info($"aw_int_set(AW_PORT, {port})");
    Check(DwSdkNative.aw_int_set(DwSdkNative.AW_PORT, port));

    // 🔑 Credentials REQUIRED before bind in this build
    TraceLog.Info("aw_string_set(AW_USERNAME)");
    Check(DwSdkNative.aw_string_set(DwSdkNative.AW_USERNAME, username));

    TraceLog.Info("aw_string_set(AW_PASSWORD)");
    Check(DwSdkNative.aw_string_set(DwSdkNative.AW_PASSWORD, password));

    // Bind + create + init
    TraceLog.Info("aw_init_bind");
    Check(DwSdkNative.aw_init_bind());

    TraceLog.Info("aw_create");
    Check(DwSdkNative.aw_create());

    TraceLog.Info("aw_init");
    Check(DwSdkNative.aw_init(logLevel));

    _initialized = true;
    _created = true;
}







    public void Create()
    {
        EnsureInit();
        TraceLog.Info("aw_create");
        Check(DwSdkNative.aw_create());
        _created = true;
    }

    public void Configure(string addr, int port, string user, string pass)
    {
        EnsureCreated();

        TraceLog.Info($"aw_string_set(AW_ADDRESS, {addr})");
        Check(DwSdkNative.aw_string_set(DwSdkNative.AW_ADDRESS, addr));

        TraceLog.Info($"aw_int_set(AW_PORT, {port})");
        Check(DwSdkNative.aw_int_set(DwSdkNative.AW_PORT, port));

        TraceLog.Info("aw_string_set(AW_USERNAME)");
        Check(DwSdkNative.aw_string_set(DwSdkNative.AW_USERNAME, user));

        TraceLog.Info("aw_string_set(AW_PASSWORD)");
        Check(DwSdkNative.aw_string_set(DwSdkNative.AW_PASSWORD, pass));
    }

    public void Login()
    {
        EnsureCreated();
        TraceLog.Info("aw_login");
        Check(DwSdkNative.aw_login());
    }

    public void Pump(int ms = 50)
    {
        Check(DwSdkNative.aw_wait(ms));
    }

    private static void Check(int rc)
{
    // 444 = OK / success in Classic SDK
    if (rc == 444)
        return;

    // 0 is also sometimes returned as success
    if (rc == 0)
        return;

    throw new InvalidOperationException($"SDK error {rc}");
}


    private void EnsureInit()
    {
        if (!_initialized)
            throw new InvalidOperationException("SDK not initialized");
    }

    private void EnsureCreated()
    {
        if (!_created)
            throw new InvalidOperationException("SDK not created");
    }

    public void Dispose()
    {
        if (_created)
        {
            TraceLog.Info("aw_destroy");
            DwSdkNative.aw_destroy();
            _created = false;
        }
    }
}
