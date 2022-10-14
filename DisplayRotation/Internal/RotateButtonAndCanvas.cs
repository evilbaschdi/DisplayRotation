using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;

namespace DisplayRotation.Internal;

public class RotateButtonAndCanvas : IRotateButtonAndCanvas
{
    public void RunFor(int rotation, [NotNull] Button button)
    {
        if (button == null)
        {
            throw new ArgumentNullException(nameof(button));
        }

        const double x = 10d;
        const double w = 192d;
        const double h = 108d;

        switch (rotation)
        {
            case NativeMethods.Dmdo90:
            case NativeMethods.Dmdo270:
                button.SetCurrentValue(FrameworkElement.HeightProperty, w);
                button.SetCurrentValue(FrameworkElement.WidthProperty, h);
                ((Canvas)button.Parent).SetCurrentValue(FrameworkElement.MarginProperty, new Thickness(x, 0, h, 0));
                break;

            case NativeMethods.Dmdo180:
            case NativeMethods.DmdoDefault:

                button.SetCurrentValue(FrameworkElement.HeightProperty, h);
                button.SetCurrentValue(FrameworkElement.WidthProperty, w);
                ((Canvas)button.Parent).SetCurrentValue(FrameworkElement.MarginProperty, new Thickness(x, 0, w, 0));
                break;
        }
    }
}