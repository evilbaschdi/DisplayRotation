using System.Runtime.InteropServices;

// ReSharper disable All

namespace DisplayRotation.Internal;

// ReSharper disable once ClassNeverInstantiated.Global
public class NativeMethods
{
    public const int EnumCurrentSettings = -1;

    public const int DispChangeSuccessful = 0;
    public const int DispChangeBaddualview = -6;
    public const int DispChangeBadflags = -4;
    public const int DispChangeBadmode = -2;
    public const int DispChangeBadparam = -5;
    public const int DispChangeFailed = -1;
    public const int DispChangeNotupdated = -3;
    public const int DispChangeRestart = 1;
    public const int DmdoDefault = 0;
    public const int Dmdo90 = 1;
    public const int Dmdo180 = 2;

    public const int Dmdo270 = 3;

    // PInvoke declaration for EnumDisplaySettings Win32 API
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
#pragma warning disable CA1401 // P/Invokes should not be visible
    public static extern int EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref Devmode lpDevMode);
#pragma warning restore CA1401 // P/Invokes should not be visible

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern bool EnumDisplayDevices(
        string lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice,
        uint dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern DisplayChange ChangeDisplaySettingsEx(
        string lpszDeviceName, ref Devmode lpDevMode, IntPtr hwnd,
        DisplaySettingsFlags dwflags, IntPtr lParam);
}