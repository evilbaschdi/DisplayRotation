using System.Windows;
using EvilBaschdi.CoreExtended.Metro;

namespace DisplayRotation
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var themeManagerHelper = new ThemeManagerHelper();
            themeManagerHelper.RegisterSystemColorTheme();

            base.OnStartup(e);
        }
    }
}