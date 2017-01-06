using System.Collections.Generic;
using EvilBaschdi.Core.DotNetExtensions;

namespace DisplayRotation.Internal
{
    public interface IActiveDevices : IValue<IEnumerable<DisplayHelper>>
    {
    }
}