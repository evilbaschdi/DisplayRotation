using DisplayRotation.Core;
using DisplayRotation.Internal;
using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
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

        public MainWindow()
        {
            InitializeComponent();
            var activeDevices = new ActiveDevices();
            _rotateDisplay = new RotateDisplay();
            BuildDeviceButtons(activeDevices);
            var appSettings = new ApplicationSettings(this, new NotifyIcon(), activeDevices, _rotateDisplay);
            appSettings.Run();
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
                    Background = (SolidColorBrush)FindResource("AccentColorBrush"),
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
            var button = (Button)sender;
            button.BorderBrush = (SolidColorBrush)FindResource("HighlightBrush");
            button.Foreground = (SolidColorBrush)FindResource("TextBrush");
            _currentDisplayId = (uint)button.ToolTip;
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
    }
}