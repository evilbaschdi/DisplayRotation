using System;
using System.Runtime.InteropServices;

namespace DisplayRotation.Internal
{
    public class RotateDisplay : IRotateDisplay
    {
        public void For(int rotation, uint deviceId)
        {
            //uint deviceId = 1; // zero origin (i.e. 1 means DISPLAY2)

            var displayDevice = new DisplayDevice();
            var devMode = new Devmode();
            displayDevice.cb = Marshal.SizeOf(displayDevice);

            NativeMethods.EnumDisplayDevices(null, deviceId, ref displayDevice, 0);
            if (0 != NativeMethods.EnumDisplaySettings(
                displayDevice.DeviceName, NativeMethods.EnumCurrentSettings, ref devMode))
            {
                int temp = devMode.dmPelsHeight;
                devMode.dmPelsHeight = devMode.dmPelsWidth;
                devMode.dmPelsWidth = temp;

                //switch(devMode.dmDisplayOrientation)
                //{
                //    case NativeMethods.DmdoDefault:
                //        devMode.dmDisplayOrientation = NativeMethods.Dmdo270;
                //        break;

                //    case NativeMethods.Dmdo270:
                //        devMode.dmDisplayOrientation = NativeMethods.Dmdo180;
                //        break;

                //    case NativeMethods.Dmdo180:
                //        devMode.dmDisplayOrientation = NativeMethods.Dmdo90;
                //        break;

                //    case NativeMethods.Dmdo90:
                //        devMode.dmDisplayOrientation = NativeMethods.DmdoDefault;
                //        break;
            }

            devMode.dmDisplayOrientation = rotation;

            DisplayChange iRet = NativeMethods.ChangeDisplaySettingsEx(
                displayDevice.DeviceName, ref devMode, IntPtr.Zero,
                DisplaySettingsFlags.CdsUpdateregistry, IntPtr.Zero);
        }
    }
}