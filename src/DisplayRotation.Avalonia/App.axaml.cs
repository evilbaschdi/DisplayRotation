using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DisplayRotation.Avalonia.ViewModels;
using DisplayRotation.Core;
using EvilBaschdi.Core.Avalonia.DependencyInjection;
using EvilBaschdi.Core.Avalonia.Themes;

namespace DisplayRotation.Avalonia;

/// <inheritdoc />
public class App : Application
{
    private MainWindow _mainWindow;
    private IRotateDisplay _rotateDisplay;

    /// <inheritdoc />
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        IActiveDevices activeDevices = new ActiveDevices();
        _rotateDisplay = new RotateDisplay();
        BuildDeviceTrayIconMenuItems(activeDevices);
    }

    private void BuildDeviceTrayIconMenuItems(IActiveDevices activeDevices)
    {
        var trayIcons = TrayIcon.GetIcons(this); // Gets all TrayIcons defined in XAML for this application
        var trayIcon = trayIcons?.FirstOrDefault();

        if (trayIcon == null)
        {
            Console.WriteLine("Warning: TrayIcon not found. Ensure it's defined in App.xaml.");
            return;
        }

        // Clear existing dynamic items if this method is called multiple times
        // This assumes your static menu items (Restore, Exit) are defined in XAML
        // and you only want to dynamically add the device-specific ones.
        // You might need a more robust way to identify and remove only the dynamic ones.
        if (trayIcon.Menu != null)
        {
            // Find the index after which device-specific menus should be added.
            // Assuming "Settings" is first, then a separator, then "Restore" and "Exit".
            // We want to insert before "Restore".
            var restoreMenuItem = trayIcon.Menu.Items.FirstOrDefault(item => (item as NativeMenuItem)?.Header == "Separator");
            var insertIndex = -1;
            if (restoreMenuItem != null)
            {
                insertIndex = trayIcon.Menu.Items.IndexOf(restoreMenuItem);
            }

            // Remove previous dynamic device menus to prevent duplicates if called again.
            // This is a basic approach; a more sophisticated one might iterate and check for specific tags/properties.
            for (var i = trayIcon.Menu.Items.Count - 1; i >= 0; i--)
            {
                if (trayIcon.Menu.Items[i] is NativeMenuItem { Menu: not null } item
                    && item.Header != "Restore"
                    && item.Header != "Exit")
                {
                    trayIcon.Menu.Items.RemoveAt(i);
                }
            }

            foreach (var device in activeDevices.Value.OrderBy(d => d.PositionX))
            {
                var rotateMenuItemClockwise = new NativeMenuItem("Clockwise Rotation");
                rotateMenuItemClockwise.Click += RotateMenuItemOnClick;
                rotateMenuItemClockwise.CommandParameter = new KeyValuePair<uint, int>(device.Id, NativeMethods.Dmdo270);

                var rotateMenuItemAntiClockwise = new NativeMenuItem("Anticlockwise Rotation");
                rotateMenuItemAntiClockwise.Click += RotateMenuItemOnClick;
                rotateMenuItemAntiClockwise.CommandParameter = new KeyValuePair<uint, int>(device.Id, NativeMethods.Dmdo90);

                var rotateMenuItemReset = new NativeMenuItem("Reset Rotation");
                rotateMenuItemReset.Click += RotateMenuItemOnClick;
                rotateMenuItemReset.CommandParameter = new KeyValuePair<uint, int>(device.Id, NativeMethods.DmdoDefault);

                var rotateMenu = new NativeMenu
                                 {
                                     Items =
                                     {
                                         rotateMenuItemClockwise,
                                         rotateMenuItemAntiClockwise,
                                         rotateMenuItemReset,
                                     }
                                 };

                var deviceMenuItem = new NativeMenuItem($"{device.Name} ({device.Id})") { Menu = rotateMenu };

                // Add the device menu item at the correct position
                if (insertIndex != -1 && insertIndex <= trayIcon.Menu.Items.Count)
                {
                    trayIcon.Menu.Items.Insert(insertIndex, deviceMenuItem);
                    insertIndex++; // Increment insertIndex for the next item
                }
                else
                {
                    trayIcon.Menu.Items.Add(deviceMenuItem);
                }
            }
        }
    }

    private void RotateMenuItemOnClick(object sender, EventArgs e)
    {
        if (sender is not NativeMenuItem menuItem || menuItem.Menu?.Items.Count == 0 || menuItem.CommandParameter == null)
        {
            return;
        }

        var commandParameter = (KeyValuePair<uint, int>)menuItem.CommandParameter;
        _rotateDisplay.RunFor(commandParameter.Value, Convert.ToUInt32(commandParameter.Key));
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ThemeEngine.Initialize(this);

            _mainWindow = new()
                          {
                              DataContext = ApplicationServices.GetRequiredService<MainWindowViewModel>(),
                              ShowInTaskbar = false
                          };

            ThemeEngine.ApplyThemeToWindow(_mainWindow, false);
            desktop.MainWindow = _mainWindow;
            _mainWindow.ShowInTaskbar = false;
            _mainWindow.WindowState = WindowState.Minimized;

            _mainWindow.Hide();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void TrayIconMenuItemExitOnClick(object sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainWindow?.Close();
            desktop.Shutdown();
        }
    }

    private void TrayIconMenuItemRestoreOnClick(object sender, EventArgs e)
    {
        _mainWindow.ShowInTaskbar = true;
        _mainWindow.Show();
        _mainWindow.Activate();
        _mainWindow.WindowState = WindowState.Normal;
    }
}