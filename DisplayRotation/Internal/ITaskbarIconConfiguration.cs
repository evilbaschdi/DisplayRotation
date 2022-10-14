using EvilBaschdi.Core;

namespace DisplayRotation.Internal;

public interface ITaskbarIconConfiguration : IRun
{
    void StartMinimized();
}