using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        private uint _currentDisplayId;
        private Button _currentButton;
        private readonly IRotateDisplay _rotateDisplay;
        private readonly IRotateButtonAndCanvas _rotateButtonAndCanvas;
        private readonly IMetroStyle _style;
        private readonly int _overrideProtection;

        public MainWindow()
        {
            ISettings coreSettings = new CoreSettings();
            InitializeComponent();
            _style = new MetroStyle(this, Accent, ThemeSwitch, coreSettings);
            _style.Load(true);
            var activeDevices = new ActiveDevices();
            _rotateDisplay = new RotateDisplay();
            _rotateButtonAndCanvas = new RotateButtonAndCanvas();
            BuildDeviceButtons(activeDevices);
            var appSettings = new ApplicationSettings(this, new NotifyIcon(), activeDevices, _rotateDisplay, _rotateButtonAndCanvas);
            appSettings.Run();
            var linkerTime = Assembly.GetExecutingAssembly().GetLinkerTime();
            LinkerTime.Content = linkerTime.ToString(CultureInfo.InvariantCulture);
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
            var w = 192;
            var h = 108;

            foreach (var device in activeDevices.Get().OrderBy(d => d.PositionX))
            {
                var buttonHeight = device.Width > device.Height ? h : w;
                var buttonWidth = device.Width > device.Height ? w : h;
                // ReSharper disable once UseObjectOrCollectionInitializer
                var displayButton = new Button
                                    {
                                        Name = $"ButtonDisplay{device.Id}",
                                        Height = buttonHeight,
                                        Width = buttonWidth,
                                        Background = (SolidColorBrush) FindResource("AccentColorBrush"),
                                        Content = new TextBlock
                                                  {
                                                      //Text = $"{displayHelper.Name}{Environment.NewLine}{displayHelper.Width} x {displayHelper.Height}",
                                                      Text = $"{device.Name}",
                                                      TextAlignment = TextAlignment.Center,
                                                      TextWrapping = TextWrapping.Wrap
                                                  },
                                        BorderThickness = new Thickness(2),
                                        ToolTip = device.Id
                                    };
                displayButton.Click += DisplayButtonOnClick;

                var displayCanvas = new Canvas
                                    {
                                        Name = $"CanvasDisplay{device.Id}",
                                        Margin = new Thickness(x, 0, buttonWidth, 0)
                                    };
                displayCanvas.Children.Add(displayButton);
                DisplayStackPanel.Children.Add(displayCanvas);
            }

            SetWindowMargins();
        }


        public void SetWindowMargins()
        {
            var children = DisplayStackPanel.Children;

            var childWidth = children.Count*10d + 10d + children.Cast<Canvas>().Sum(child => child.Margin.Right);

            var childHeight = (from Canvas canvas in children from Button button in canvas.Children select button.Height).Concat(new[] { 0d }).Max() + 90d;

            var windowWidth = 490d;
            var windowHeight = 200d;
            Width = childWidth > windowWidth ? childWidth : windowWidth;
            Height = childHeight > windowHeight ? childHeight : windowHeight;
        }

        private void DisplayButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
        {
            foreach (var childButton in DisplayStackPanel.Children.Cast<Canvas>().SelectMany(childCanvas => childCanvas.Children.Cast<Button>()))
            {
                childButton.BorderBrush = BtnClockwise.BorderBrush;
                childButton.Foreground = BtnClockwise.Foreground;
            }

            var button = (Button) sender;
            button.BorderBrush = (SolidColorBrush) FindResource("HighlightBrush");
            button.Foreground = (SolidColorBrush) FindResource("TextBrush");
            _currentDisplayId = (uint) button.ToolTip;
            _currentButton = button;
        }

        private void BtnClockwise_OnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.For(NativeMethods.Dmdo270, _currentDisplayId);
            _rotateButtonAndCanvas.For(NativeMethods.Dmdo270, _currentButton);
            SetWindowMargins();
        }

        private void BtnAntiClock_OnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.For(NativeMethods.Dmdo90, _currentDisplayId);
            _rotateButtonAndCanvas.For(NativeMethods.Dmdo90, _currentButton);
            SetWindowMargins();
        }

        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.For(NativeMethods.DmdoDefault, _currentDisplayId);
            _rotateButtonAndCanvas.For(NativeMethods.DmdoDefault, _currentButton);
            SetWindowMargins();
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

            if (activeFlyout.IsOpen && stayOpen)
            {
                activeFlyout.IsOpen = true;
            }
            else
            {
                activeFlyout.IsOpen = !activeFlyout.IsOpen;
            }
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
            var routedEventArgs = e as RoutedEventArgs;
            if (routedEventArgs != null)
            {
                _style.SetTheme(sender, routedEventArgs);
            }
            else
            {
                _style.SetTheme(sender);
            }
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