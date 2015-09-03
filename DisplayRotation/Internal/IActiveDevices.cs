using System.Collections.Generic;

namespace DisplayRotation.Internal
{
    public interface IActiveDevices
    {
        IEnumerable<DisplayHelper> Get();
    }
}