using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace DisplayRotation.Core;

/// <summary>
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
// ReSharper disable once IdentifierTypo
public struct Devmode
{
    /// <summary />
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string dmDeviceName;

    /// <summary />
    public short dmSpecVersion;

    /// <summary />
    public short dmDriverVersion;

    /// <summary />
    public short dmSize;

    /// <summary />
    public short dmDriverExtra;

    /// <summary />
    public int dmFields;

    /// <summary />
    public int dmPositionX;

    /// <summary />
    public int dmPositionY;

    /// <summary />
    public int dmDisplayOrientation;

    /// <summary />
    public int dmDisplayFixedOutput;

    /// <summary />
    public short dmColor;

    /// <summary />
    public short dmDuplex;

    /// <summary />
    public short dmYResolution;

    /// <summary />
    public short dmTTOption;

    /// <summary />
    public short dmCollate;

    /// <summary />
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string dmFormName;

    /// <summary />
    public short dmLogPixels;

    /// <summary />
    public short dmBitsPerPel;

    /// <summary />
    public int dmPelsWidth;

    /// <summary />
    public int dmPelsHeight;

    /// <summary />
    public int dmDisplayFlags;

    /// <summary />
    public int dmDisplayFrequency;

    /// <summary />
    public int dmICMMethod;

    /// <summary />
    public int dmICMIntent;

    /// <summary />
    public int dmMediaType;

    /// <summary />
    public int dmDitherType;

    /// <summary />
    public int dmReserved1;

    /// <summary />
    public int dmReserved2;

    /// <summary />
    public int dmPanningWidth;

    /// <summary />
    public int dmPanningHeight;
}