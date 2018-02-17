using System.Windows.Forms;

namespace DisplayRotation.Internal
{
    public class ScreenCount : IScreenCount
    {
        public int Value => Screen.AllScreens.Length;
    }
}