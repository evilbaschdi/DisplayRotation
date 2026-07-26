using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using DisplayRotation.Avalonia.Internal;
using DisplayRotation.Avalonia.ViewModels;
using DisplayRotation.Core;
using EvilBaschdi.Core.Avalonia.DependencyInjection;
using FluentAvalonia.UI.Windowing;

namespace DisplayRotation.Avalonia;

/// <inheritdoc />
public partial class MainWindow : FAAppWindow
{
    private readonly IRotateButton _rotateButton;
    private readonly IRotateDisplay _rotateDisplay;
    private Button _currentButton;
    private uint _currentDisplayId;
    private IBrush _defaultBorderBrush;

    /// <summary>
    ///     Constructor
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        _rotateDisplay = new RotateDisplay();
        _rotateButton = new RotateButton();

        var topLevel = ApplicationServices.GetRequiredService<ITopLevel>();
        topLevel.Value = GetTopLevel(this);

        Load();
        Closing += MainWindowClosing;
    }

    private void MainWindowClosing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        ShowInTaskbar = false;
        Hide();
    }

    private void Load()
    {
        IActiveDevices activeDevices = new ActiveDevices();
        BuildDeviceButtons(activeDevices);
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
                                    Name = $"{device.Id}",
                                    Height = buttonHeight,
                                    Width = buttonWidth,
                                    Content = new TextBlock
                                              {
                                                  Text = $"{device.Name} ({device.Id})",
                                                  TextAlignment = TextAlignment.Center,
                                                  TextWrapping = TextWrapping.Wrap
                                              },
                                    BorderThickness = new(2),
                                    //ToolTip = device.Id
                                };

            _defaultBorderBrush = displayButton.BorderBrush;

            displayButton.Click += DisplayButtonOnClick;

            var displayCanvas = new Canvas
                                {
                                    Name = $"CanvasDisplay{device.Id}",
                                    Margin = new(x, 0, buttonWidth, 0)
                                };
            displayCanvas.Children.Add(displayButton);
            DisplayStackPanel.Children.Add(displayCanvas);
        }

        //Tray.SetMenuItems(DisplayStackPanel.Children.Cast<Canvas>().SelectMany(childCanvas => childCanvas.Children.Cast<Button>()).ToList());
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
        firstButton.BorderBrush = Brushes.Blue;
        _currentDisplayId = Convert.ToUInt32(firstButton.Name);
        _currentButton = firstButton;
    }

    /// <summary />
    public void SetWindowMargins()
    {
        var children = DisplayStackPanel.Children;

        var childWidth = children.Count * 10d + 10d + children.Cast<Canvas>().Sum(child => child.Margin.Right);

        var childHeight = (from Canvas canvas in children from Button button in canvas.Children select button.Height).Concat([0d]).Max() + 105d;

        const double windowWidth = 490d;
        const double windowHeight = 300d;
        SetCurrentValue(WidthProperty, childWidth > windowWidth ? childWidth : windowWidth);
        SetCurrentValue(HeightProperty, childHeight > windowHeight ? childHeight : windowHeight);
    }

    private void DisplayButtonOnClick(object sender, RoutedEventArgs routedEventArgs)
    {
        foreach (var childButton in DisplayStackPanel.Children.Cast<Canvas>().SelectMany(childCanvas => childCanvas.Children.Cast<Button>()))
        {
            childButton.BorderBrush = _defaultBorderBrush;
        }

        var button = (Button)sender;
        button.BorderBrush = Brushes.Blue;
        _currentDisplayId = Convert.ToUInt32(button.Name);
        _currentButton = button;
    }

    private void BtnClockwiseOnClick(object sender, RoutedEventArgs e)
    {
        _rotateDisplay.RunFor(NativeMethods.Dmdo270, _currentDisplayId);
        _rotateButton.RunFor(NativeMethods.Dmdo270, _currentButton);
        SetWindowMargins();
    }

    private void BtnAnticlockwiseOnClick(object sender, RoutedEventArgs e)
    {
        _rotateDisplay.RunFor(NativeMethods.Dmdo90, _currentDisplayId);
        _rotateButton.RunFor(NativeMethods.Dmdo90, _currentButton);
        SetWindowMargins();
    }

    private void BtnResetOnClick(object sender, RoutedEventArgs e)
    {
        _rotateDisplay.RunFor(NativeMethods.DmdoDefault, _currentDisplayId);
        _rotateButton.RunFor(NativeMethods.DmdoDefault, _currentButton);
        SetWindowMargins();
    }
}