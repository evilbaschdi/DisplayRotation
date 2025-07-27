using System.Windows;
using System.Windows.Controls;
using DisplayRotation.Core;
using JetBrains.Annotations;

namespace DisplayRotation.Internal;

/// <inheritdoc />
public class RotateButtonAndCanvas : IRotateButtonAndCanvas
{
    /// <summary />
    public void RunFor(int rotation, [NotNull] Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

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