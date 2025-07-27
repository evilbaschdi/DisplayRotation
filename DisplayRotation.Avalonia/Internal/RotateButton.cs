using Avalonia.Controls;
using DisplayRotation.Core;
using JetBrains.Annotations;

namespace DisplayRotation.Avalonia.Internal;

/// <inheritdoc />
public class RotateButton : IRotateButton
{
    /// <summary />
    public void RunFor(int rotation, [NotNull] Button button)
    {
        ArgumentNullException.ThrowIfNull(button);

        const double w = 192d;
        const double h = 108d;

        switch (rotation)
        {
            case NativeMethods.Dmdo90:
            case NativeMethods.Dmdo270:

                button.Height = w;
                button.Width = h;

                break;

            case NativeMethods.Dmdo180:
            case NativeMethods.DmdoDefault:

                button.Height = h;
                button.Width = w;
                break;
        }
    }
}