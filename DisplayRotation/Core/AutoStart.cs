using System;
using System.Reflection;
using Microsoft.Win32;

namespace DisplayRotation.Core
{
    public class AutoStart : IAutoStart
    {
        private readonly string _appName;
        private const string SubKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public AutoStart(string appName)
        {
            _appName = appName ?? throw new ArgumentNullException(nameof(appName));
        }

        public void Enable()
        {
            var registryKey = Registry.CurrentUser.OpenSubKey(SubKey, true);
            var location = Assembly.GetExecutingAssembly().Location;
            if (location != null)
            {
                registryKey?.SetValue(_appName, location);
            }
        }


        public void Disable()
        {
            var registryKey = Registry.CurrentUser.OpenSubKey(SubKey, true);
            registryKey?.DeleteValue(_appName, false);
        }

        public bool IsEnabled
        {
            get
            {
                var registryKey = Registry.CurrentUser.OpenSubKey(SubKey, true);
                var value = registryKey?.GetValue(_appName);
                if (value != null)
                {
                    return true;
                }
                return false;
            }
        }
    }
}