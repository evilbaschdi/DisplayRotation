using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DisplayRotation.Internal;
using DisplayRotation.Properties;
using EvilBaschdi.Core.Extensions;
using EvilBaschdi.CoreExtended.AppHelpers;
using EvilBaschdi.CoreExtended.Metro;
using MahApps.Metro.Controls;
using IScreenCount = DisplayRotation.Internal.IScreenCount;
using ScreenCount = DisplayRotation.Internal.ScreenCount;

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
        private readonly int _overrideProtection;
        private readonly IRotateButtonAndCanvas _rotateButtonAndCanvas;
        private readonly IRotateDisplay _rotateDisplay;
        private readonly IApplicationStyle _applicationStyle;
        private IAutoStart _autoStart;
        private Button _currentButton;
        private uint _currentDisplayId;
        private int _screenCount;


        public MainWindow()
        {
            InitializeComponent();
            IAppSettingsBase appSettingsBase = new AppSettingsBase(Settings.Default);
            IApplicationStyleSettings applicationStyleSettings = new ApplicationStyleSettings(appSettingsBase);
            IThemeManagerHelper themeManagerHelper = new ThemeManagerHelper();
            _applicationStyle = new ApplicationStyle(this, Accent, ThemeSwitch, applicationStyleSettings, themeManagerHelper);
            _applicationStyle.Load(true);

            _rotateDisplay = new RotateDisplay();
            _rotateButtonAndCanvas = new RotateButtonAndCanvas();
            Load();
            var linkerTime = Assembly.GetExecutingAssembly().GetLinkerTime();
            LinkerTime.Content = linkerTime.ToString(CultureInfo.InvariantCulture);

            Application.Current.Exit += CurrentExit;
            _overrideProtection = 1;
            ConfigureAutoRun();
        }

        private void Load()
        {
            IActiveDevices activeDevices = new ActiveDevices();
            BuildDeviceButtons(activeDevices);
            ITaskbarIconConfiguration taskbarIconConfiguration =
                new TaskbarIconConfiguration(this, DisplayRotationTaskbarIcon, activeDevices, _rotateDisplay, _rotateButtonAndCanvas);
            taskbarIconConfiguration.StartMinimized();
            taskbarIconConfiguration.Run();
            IScreenCount screenCount = new ScreenCount();
            _screenCount = screenCount.Value;
        }

        private void ReloadClick(object sender, RoutedEventArgs e)
        {
            DisplayStackPanel.Children.Clear();
            Load();
        }

        private void ConfigureAutoRun()
        {
            _autoStart = new AutoStart("DisplayRotation");
            AutoStartSwitch.IsChecked = _autoStart.IsEnabled;
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

        public void BuildDeviceButtons(IActiveDevices activeDevices)
        {
            var x = 10;
            var w = 192;
            var h = 108;

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
            ActivateFirstDisplay();
        }

        private void ActivateFirstDisplay()
        {
            if (DisplayStackPanel.Children.Cast<Canvas>().Any())
            {
                var firstButton = DisplayStackPanel.Children.Cast<Canvas>().SelectMany(childCanvas => childCanvas.Children.Cast<Button>()).First();
                firstButton.BorderBrush = (SolidColorBrush) FindResource("HighlightBrush");
                firstButton.Foreground = Brushes.White;
                _currentDisplayId = (uint) firstButton.ToolTip;
                _currentButton = firstButton;
            }
        }

        public void SetWindowMargins()
        {
            var children = DisplayStackPanel.Children;

            var childWidth = children.Count * 10d + 10d + children.Cast<Canvas>().Sum(child => child.Margin.Right);

            var childHeight = (from Canvas canvas in children from Button button in canvas.Children select button.Height).Concat(new[] { 0d }).Max() + 105d;

            var windowWidth = 490d;
            var windowHeight = 300d;
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
            button.Foreground = Brushes.White;
            _currentDisplayId = (uint) button.ToolTip;
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

        private void AutoStartIsCheckedChanged(object sender, EventArgs e)
        {
            var toggleSwitch = (ToggleSwitch) sender;

            if (_overrideProtection == 0)
            {
                return;
            }

            if (toggleSwitch.IsChecked.HasValue && toggleSwitch.IsChecked.Value)
            {
                _autoStart.Enable();
            }
            else if (_autoStart.IsEnabled)
            {
                _autoStart.Disable();
            }
        }

        public void CheckScreenCountAndRestore(object sender, RoutedEventArgs e)
        {
            IScreenCount screenCount = new ScreenCount();

            if (_screenCount != screenCount.Value)
            {
                ReloadClick(sender, e);
            }


            Show();
            WindowState = WindowState.Normal;
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

            _applicationStyle.SaveStyle();
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
                _applicationStyle.SetTheme(sender, routedEventArgs);
            }
            else
            {
                _applicationStyle.SetTheme(sender);
            }

            ReloadDisplayButtonStyle();
        }

        private void AccentOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }

            _applicationStyle.SetAccent(sender, e);
            ReloadDisplayButtonStyle();
        }

        private void ReloadDisplayButtonStyle()
        {
            var buttons = DisplayStackPanel.Children.Cast<Canvas>().SelectMany(childCanvas => childCanvas.Children.Cast<Button>());

            foreach (var button in buttons)
            {
                button.Background = (SolidColorBrush) FindResource("AccentColorBrush");
                button.BorderBrush = (SolidColorBrush) FindResource("HighlightBrush");
                button.Foreground = Brushes.Black;
            }

            _currentButton.Foreground = Brushes.White;
        }

        #endregion MetroStyle
    }
}