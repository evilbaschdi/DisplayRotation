namespace DisplayRotation.Internal;

/// <inheritdoc />
/// <summary>
///     Class that provides the count of current connected screens of the current device / session.
/// </summary>
public class ScreenCount : IScreenCount
{
    /// <inheritdoc />
    /// <summary>
    ///     Count of current connected screens.
    /// </summary>
    public int Value => Screen.AllScreens.Length;
}