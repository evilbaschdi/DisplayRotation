using System.Runtime.InteropServices;

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global

namespace DisplayRotation.Core;

/// <summary />
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct DisplayDevice
{
    /// <summary />
    [MarshalAs(UnmanagedType.U4)] public int cb;

    /// <summary />
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string DeviceName;

    /// <summary />
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceString;

    /// <summary />
    [MarshalAs(UnmanagedType.U4)] public DisplayDeviceStateFlags StateFlags;

    /// <summary />
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceID;

    /// <summary />
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string DeviceKey;
}