using System.Runtime.InteropServices;

// ReSharper disable All

namespace DisplayRotation.Core;

/// <summary />
public class NativeMethods
{
    /// <summary />
    public const int EnumCurrentSettings = -1;

    /// <summary />
    public const int DispChangeSuccessful = 0;

    /// <summary />
    public const int DispChangeBaddualview = -6;

    /// <summary />
    public const int DispChangeBadflags = -4;

    /// <summary />
    public const int DispChangeBadmode = -2;

    /// <summary />
    public const int DispChangeBadparam = -5;

    /// <summary />
    public const int DispChangeFailed = -1;

    /// <summary />
    public const int DispChangeNotupdated = -3;

    /// <summary />
    public const int DispChangeRestart = 1;

    /// <summary />
    public const int DmdoDefault = 0;

    /// <summary />
    public const int Dmdo90 = 1;

    /// <summary />
    public const int Dmdo180 = 2;

    /// <summary />
    public const int Dmdo270 = 3;

    // PInvoke declaration for EnumDisplaySettings Win32 API
    /// <summary>
    /// </summary>
    /// <param name="lpszDeviceName"></param>
    /// <param name="iModeNum"></param>
    /// <param name="lpDevMode"></param>
    /// <returns></returns>
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