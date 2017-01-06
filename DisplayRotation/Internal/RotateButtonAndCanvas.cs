using System.Windows;
using System.Windows.Controls;

namespace DisplayRotation.Internal
{
    public class RotateButtonAndCanvas : IRotateButtonAndCanvas
    {
        public void Run(int rotation, Button button)
        {
            var x = 10d;
            var w = 192d;
            var h = 108d;

            switch (rotation)
            {
                case NativeMethods.Dmdo90:
                case NativeMethods.Dmdo270:
                    button.Height = w;
                    button.Width = h;
                    ((Canvas) button.Parent).Margin = new Thickness(x, 0, h, 0);
                    break;

                case NativeMethods.Dmdo180:
                case NativeMethods.DmdoDefault:

                    button.Height = h;
                    button.Width = w;
                    ((Canvas) button.Parent).Margin = new Thickness(x, 0, w, 0);
                    break;
            }
        }
    }
}