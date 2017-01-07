using System;
using System.Runtime.InteropServices;

namespace DisplayRotation.Internal
{
    public class NativeMethods
    {
        // PInvoke declaration for EnumDisplaySettings Win32 API
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref Devmode lpDevMode);

        [DllImport("user32.dll")]
        internal static extern bool EnumDisplayDevices(
            string lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice,
            uint dwFlags);

        // PInvoke declaration for ChangeDisplaySettings Win32 API
        [DllImport("user32.dll", CharSet = CharSet.Ansi)]
        public static extern int ChangeDisplaySettings(ref Devmode lpDevMode, int dwFlags);

        [DllImport("user32.dll")]
        internal static extern DisplayChange ChangeDisplaySettingsEx(
            string lpszDeviceName, ref Devmode lpDevMode, IntPtr hwnd,
            DisplaySettingsFlags dwflags, IntPtr lParam);

        // helper for creating an initialized DEVMODE structure
        public static Devmode CreateDevmode()
        {
            var dm = new Devmode
                     {
                         dmDeviceName = new string(new char[32]),
                         dmFormName = new string(new char[32])
                     };
            dm.dmSize = (short) Marshal.SizeOf(dm);
            return dm;
        }

        // constants
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
    }
}