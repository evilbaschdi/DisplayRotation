using System.Runtime.InteropServices;

namespace DisplayRotation.Core;

/// <inheritdoc />
public class RotateDisplay : IRotateDisplay
{
    /// <inheritdoc />
    public void RunFor(int rotation, uint deviceId)
    {
        //uint deviceId = 1; // zero origin (i.e. 1 means DISPLAY2)

        var displayDevice = new DisplayDevice();
        var devMode = new Devmode();
        displayDevice.cb = Marshal.SizeOf(displayDevice);

        NativeMethods.EnumDisplayDevices(null, deviceId, ref displayDevice, 0);
        if (NativeMethods.EnumDisplaySettings(displayDevice.DeviceName, NativeMethods.EnumCurrentSettings, ref devMode) != 0)
        {
            (devMode.dmPelsHeight, devMode.dmPelsWidth) = (devMode.dmPelsWidth, devMode.dmPelsHeight);
        }

        devMode.dmDisplayOrientation = rotation;

        NativeMethods.ChangeDisplaySettingsEx(displayDevice.DeviceName, ref devMode, IntPtr.Zero, DisplaySettingsFlags.CdsUpdateregistry, IntPtr.Zero);
    }
}