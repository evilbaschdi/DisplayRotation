using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DisplayRotation.Internal
{
    public class ActiveDevices : IActiveDevices
    {
        public IEnumerable<DisplayHelper> Value
        {
            get
            {
                var device = new DisplayDevice();
                device.cb = Marshal.SizeOf(device);

                var list = new List<DisplayHelper>();
                for (uint id = 0; NativeMethods.EnumDisplayDevices(null, id, ref device, 0); id++)
                {
                    device.cb = Marshal.SizeOf(device);

                    NativeMethods.EnumDisplayDevices(device.DeviceName, 0, ref device, 0);

                    device.cb = Marshal.SizeOf(device);

                    device.cb = Marshal.SizeOf(device);

                    if (device.DeviceName.Trim().Length > 0)
                    {
                        var helper = new DisplayHelper
                                     {
                                         Id = id,
                                         Name = device.DeviceString
                                     };

                        foreach (var screen in Screen.AllScreens)
                        {
                            if (device.DeviceName.Contains(screen.DeviceName))
                            {
                                var rectangle = screen.Bounds;
                                helper.PositionX = rectangle.X;
                                helper.PositionY = rectangle.Y;
                                helper.Height = rectangle.Height;
                                helper.Width = rectangle.Width;
                            }
                        }

                        list.Add(helper);
                    }
                }

                return list;
            }
        }
    }
}