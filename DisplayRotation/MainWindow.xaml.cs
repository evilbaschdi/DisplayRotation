using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using DisplayRotation.Core;
using DisplayRotation.Internal;
using EvilBaschdi.Core.Application;
using EvilBaschdi.Core.Wpf;
using MahApps.Metro.Controls;
using Button = System.Windows.Controls.Button;

namespace DisplayRotation
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private uint _currentDisplayId;
        private readonly IRotateDisplay _rotateDisplay;
        private readonly IMetroStyle _style;
        private readonly int _overrideProtection;
        private readonly ISettings _coreSettings;

        public MainWindow()
        {
            _coreSettings = new CoreSettings();
            InitializeComponent();
            _style = new MetroStyle(this, Accent, ThemeSwitch, _coreSettings);
            _style.Load(true);
            var activeDevices = new ActiveDevices();
            _rotateDisplay = new RotateDisplay();
            BuildDeviceButtons(activeDevices);
            var appSettings = new ApplicationSettings(this, new NotifyIcon(), activeDevices, _rotateDisplay);
            appSettings.Run();
            _overrideProtection = 1;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        public void BuildDeviceButtons(IActiveDevices activeDevices)
        {
            var x = 10;

            foreach (var displayHelper in activeDevices.Get().OrderBy(d => d.PositionX))
            {
                //Devices.Text +=
                //    $"{displayHelper.Id}, {displayHelper.Name}, {displayHelper.PositionX} {Environment.NewLine}";

                var w = 192;
                var h = 108;

                var displayButton = new Button
                                    {
                                        Height = h,
                                        Width = w,
                                        Background = (SolidColorBrush) FindResource("AccentColorBrush"),
                                        Content = displayHelper.Name,
                                        BorderThickness = new Thickness(2),
                                        ToolTip = displayHelper.Id
                                        //IsEnabled = false
                                    };
                displayButton.Click += DisplayButtonOnClick;

                var displayCanvas = new Canvas
                                    {
                                        Margin = new Thickness(x, 0, w, 0)
                                    };
                displayCanvas.Children.Add(displayButton);
                DisplayStackPanel.Children.Add(displayCanvas);
            }
        }

        private void DisplayButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (
                var childButton in
                DisplayStackPanel.Children.Cast<Canvas>()
                                 .SelectMany(childCanvas => childCanvas.Children.Cast<Button>()))
            {
                childButton.BorderBrush = BtnClockwise.BorderBrush;
                childButton.Foreground = BtnClockwise.Foreground;
            }
            var button = (Button) sender;
            button.BorderBrush = (SolidColorBrush) FindResource("HighlightBrush");
            button.Foreground = (SolidColorBrush) FindResource("TextBrush");
            _currentDisplayId = (uint) button.ToolTip;
        }

        private void BtnClockwise_OnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.For(NativeMethods.Dmdo270, _currentDisplayId);
        }

        private void BtnAntiClock_OnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.For(NativeMethods.Dmdo90, _currentDisplayId);
        }

        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.For(NativeMethods.DmdoDefault, _currentDisplayId);
        }

        #region Flyout

        private void ToggleSettingsFlyoutClick(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void ToggleFlyout(int index, bool stayOpen = false)
        {
            var activeFlyout = (Flyout) Flyouts.Items[index];
            if (activeFlyout == null)
            {
                return;
            }

            foreach (
                var nonactiveFlyout in
                Flyouts.Items.Cast<Flyout>()
                       .Where(nonactiveFlyout => nonactiveFlyout.IsOpen && nonactiveFlyout.Name != activeFlyout.Name))
            {
                nonactiveFlyout.IsOpen = false;
            }

            activeFlyout.IsOpen = activeFlyout.IsOpen && stayOpen || !activeFlyout.IsOpen;
        }

        #endregion Flyout

        #region MetroStyle

        private void SaveStyleClick(object sender, RoutedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            _style.SaveStyle();
        }

        private void Theme(object sender, EventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            _style.SetTheme(sender);
        }

        private void AccentOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            _style.SetAccent(sender, e);
        }

        #endregion MetroStyle
    }
}