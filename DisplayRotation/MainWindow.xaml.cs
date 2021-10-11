using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DisplayRotation.Internal;
using EvilBaschdi.CoreExtended;
using EvilBaschdi.CoreExtended.AppHelpers;
using EvilBaschdi.CoreExtended.Controls.About;
using MahApps.Metro.Controls;

//using System.Windows.Forms;

//using Button = System.Windows.Controls.Button;

namespace DisplayRotation
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        private readonly Assembly _assembly = typeof(MainWindow).Assembly;
        private readonly int _overrideProtection;
        private readonly IRotateButtonAndCanvas _rotateButtonAndCanvas;
        private readonly IRotateDisplay _rotateDisplay;
        private readonly IRoundCorners _roundCorners;

        private IAutoStart _autoStart;
        private Button _currentButton;
        private uint _currentDisplayId;
        private int _screenCount;

        public MainWindow()
        {
            InitializeComponent();

            _roundCorners = new RoundCorners();
            IApplicationStyle style = new ApplicationStyle(_roundCorners, true);
            style.Run();

            _rotateDisplay = new RotateDisplay();
            _rotateButtonAndCanvas = new RotateButtonAndCanvas();
            Load();

            Application.Current.Exit += CurrentExit;
            _overrideProtection = 1;
            ConfigureAutoRun();
        }

        protected override void OnClosed(EventArgs e)
        {
            foreach (Window currentWindow in Application.Current.Windows)
            {
                if (currentWindow != Application.Current.MainWindow)
                {
                    currentWindow.Close();
                }
            }

            base.OnClosed(e);
        }

        private void Load()
        {
            IActiveDevices activeDevices = new ActiveDevices();
            BuildDeviceButtons(activeDevices);
            ITaskbarIconConfiguration taskbarIconConfiguration =
                new TaskbarIconConfiguration(DisplayRotationTaskbarIcon, activeDevices, _rotateDisplay, _rotateButtonAndCanvas, _roundCorners);
            taskbarIconConfiguration.StartMinimized();
            taskbarIconConfiguration.Run();
            IScreenCount screenCount = new ScreenCount();
            _screenCount = screenCount.Value;
        }

        // ReSharper disable once UnusedParameter.Local
        private void ReloadClick()
        {
            DisplayStackPanel.Children.Clear();
            Load();
        }

        private void AboutWindowClick(object sender, RoutedEventArgs e)
        {
            IAboutContent aboutWindowContent = new AboutContent(_assembly, $@"{AppDomain.CurrentDomain.BaseDirectory}\512.png");

            var aboutWindow = new AboutWindow
                              {
                                  DataContext = new AboutViewModel(aboutWindowContent, _roundCorners)
                              };

            aboutWindow.ShowDialog();
        }

        private void ConfigureAutoRun()
        {
            _autoStart = new AutoStart("DisplayRotation", Assembly.GetExecutingAssembly().Location);
        }


        private void CurrentExit(object sender, ExitEventArgs e)
        {
            DisplayRotationTaskbarIcon.Dispose();
        }


        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        private void BuildDeviceButtons(IActiveDevices activeDevices)
        {
            const int x = 10;
            const int w = 192;
            const int h = 108;

            foreach (var device in activeDevices.Value.OrderBy(d => d.PositionX))
            {
                var buttonHeight = device.Width > device.Height ? h : w;
                var buttonWidth = device.Width > device.Height ? w : h;
                // ReSharper disable once UseObjectOrCollectionInitializer
                var displayButton = new Button
                                    {
                                        Name = $"ButtonDisplay{device.Id}",
                                        Height = buttonHeight,
                                        Width = buttonWidth,
                                        Background = (SolidColorBrush)FindResource("MahApps.Brushes.AccentBase"),
                                        Content = new TextBlock
                                                  {
                                                      //Text = $"{displayHelper.Name}{Environment.NewLine}{displayHelper.Width} x {displayHelper.Height}",
                                                      Text = $"{device.Name}",
                                                      TextAlignment = TextAlignment.Center,
                                                      TextWrapping = TextWrapping.Wrap
                                                  },
                                        BorderThickness = new(2),
                                        ToolTip = device.Id
                                    };

                displayButton.Click += DisplayButtonOnClick;

                var displayCanvas = new Canvas
                                    {
                                        Name = $"CanvasDisplay{device.Id}",
                                        Margin = new(x, 0, buttonWidth, 0)
                                    };
                displayCanvas.Children.Add(displayButton);
                DisplayStackPanel.Children.Add(displayCanvas);
            }

            SetWindowMargins();
            ActivateFirstDisplay();
        }

        private void ActivateFirstDisplay()
        {
            if (!DisplayStackPanel.Children.Cast<Canvas>().Any())
            {
                return;
            }

            var firstButton = DisplayStackPanel.Children.Cast<Canvas>().SelectMany(childCanvas => childCanvas.Children.Cast<Button>()).First();
            firstButton.BorderBrush = (SolidColorBrush)FindResource("MahApps.Brushes.Highlight");
            firstButton.Foreground = Brushes.White;
            _currentDisplayId = (uint)firstButton.ToolTip;
            _currentButton = firstButton;
        }

        public void SetWindowMargins()
        {
            var children = DisplayStackPanel.Children;

            var childWidth = children.Count * 10d + 10d + children.Cast<Canvas>().Sum(child => child.Margin.Right);

            var childHeight = (from Canvas canvas in children from Button button in canvas.Children select button.Height).Concat(new[] { 0d }).Max() + 105d;

            const double windowWidth = 490d;
            const double windowHeight = 300d;
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

            var button = (Button)sender;
            button.BorderBrush = (SolidColorBrush)FindResource("MahApps.Brushes.Highlight");
            button.Foreground = Brushes.White;
            _currentDisplayId = (uint)button.ToolTip;
            _currentButton = button;
        }

        private void BtnClockwiseOnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.RunFor(NativeMethods.Dmdo270, _currentDisplayId);
            _rotateButtonAndCanvas.RunFor(NativeMethods.Dmdo270, _currentButton);
            SetWindowMargins();
        }

        private void BtnAntiClockOnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.RunFor(NativeMethods.Dmdo90, _currentDisplayId);
            _rotateButtonAndCanvas.RunFor(NativeMethods.Dmdo90, _currentButton);
            SetWindowMargins();
        }

        private void BtnResetOnClick(object sender, RoutedEventArgs e)
        {
            _rotateDisplay.RunFor(NativeMethods.DmdoDefault, _currentDisplayId);
            _rotateButtonAndCanvas.RunFor(NativeMethods.DmdoDefault, _currentButton);
            SetWindowMargins();
        }

        private void AutoStartSwitchToggled(object sender, RoutedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }

            if (!(sender is ToggleSwitch toggleSwitch))
            {
                return;
            }

            if (toggleSwitch.IsOn)
            {
                _autoStart.Enable();
            }
            else if (_autoStart.IsEnabled)
            {
                _autoStart.Disable();
            }
        }

        public void CheckScreenCountAndRestore()
        {
            IScreenCount screenCount = new ScreenCount();

            if (_screenCount != screenCount.Value)
            {
                ReloadClick();
            }

            Show();
            WindowState = WindowState.Normal;
        }

        #region Fly-out

        private void ToggleSettingsFlyoutClick(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void ToggleFlyout(int index, bool stayOpen = false)
        {
            var activeFlyout = (Flyout)Flyouts.Items[index];
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

        #endregion Fly-out
    }
}