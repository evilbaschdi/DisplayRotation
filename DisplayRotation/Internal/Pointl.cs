using System.Runtime.InteropServices;

namespace DisplayRotation.Internal
{
    [StructLayout(LayoutKind.Sequential)]
    struct Pointl
    {
        readonly long x;
        readonly long y;
    }
}