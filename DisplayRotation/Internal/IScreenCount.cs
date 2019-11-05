using EvilBaschdi.Core;

namespace DisplayRotation.Internal
{
    /// <inheritdoc />
    /// <summary>
    ///     Interface for classes that provide the count of current connected screens of the current device / session.
    /// </summary>
    public interface IScreenCount : IValue<int>
    {
    }
}