using System.Runtime.InteropServices;

namespace DisplayRotation.Internal
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct DisplayDevice
    {
        [MarshalAs(UnmanagedType.U4)] public int cb;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string DeviceName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceString;

        [MarshalAs(UnmanagedType.U4)] public DisplayDeviceStateFlags StateFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceKey;
    }
}