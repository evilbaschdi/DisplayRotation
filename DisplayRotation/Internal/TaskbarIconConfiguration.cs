using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using EvilBaschdi.CoreExtended;
using EvilBaschdi.CoreExtended.Controls.About;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Annotations;
using MahApps.Metro.IconPacks;

namespace DisplayRotation.Internal;

public class TaskbarIconConfiguration : ITaskbarIconConfiguration
{
    private readonly IActiveDevices _activeDevices;
    private readonly IApplicationStyle _applicationStyle;
    private readonly Assembly _assembly = typeof(MainWindow).Assembly;
    private readonly MainWindow _mainWindow = (MainWindow)Application.Current.MainWindow;
    private readonly IRotateButtonAndCanvas _rotateButtonAndCanvas;
    private readonly IRotateDisplay _rotateDisplay;
    private readonly TaskbarIcon _taskbarIcon;

    /// <summary>
    /// </summary>
    /// <param name="taskbarIcon"></param>
    /// <param name="applicationStyle"></param>
    /// <param name="activeDevices"></param>
    /// <param name="rotateDisplay"></param>
    /// <param name="rotateButtonAndCanvas"></param>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="taskbarIcon" /> is <see langword="null" />.
    ///     <paramref name="activeDevices" /> is <see langword="null" />.
    ///     <paramref name="rotateDisplay" /> is <see langword="null" />.
    ///     <paramref name="rotateButtonAndCanvas" /> is <see langword="null" />.
    /// </exception>
    public TaskbarIconConfiguration([NotNull] TaskbarIcon taskbarIcon,
                                    [NotNull] IApplicationStyle applicationStyle,
                                    [NotNull] IActiveDevices activeDevices,
                                    [NotNull] IRotateDisplay rotateDisplay,
                                    [NotNull] IRotateButtonAndCanvas rotateButtonAndCanvas)
    {
        _taskbarIcon = taskbarIcon ?? throw new ArgumentNullException(nameof(taskbarIcon));
        _applicationStyle = applicationStyle ?? throw new ArgumentNullException(nameof(applicationStyle));
        _activeDevices = activeDevices ?? throw new ArgumentNullException(nameof(activeDevices));
        _rotateDisplay = rotateDisplay ?? throw new ArgumentNullException(nameof(rotateDisplay));
        _rotateButtonAndCanvas =
            rotateButtonAndCanvas ?? throw new ArgumentNullException(nameof(rotateButtonAndCanvas));
    }

    /// <summary>
    /// </summary>
    public void Run()
    {
        _taskbarIcon.Icon = Icon.ExtractAssociatedIcon(_assembly.Location);
        _taskbarIcon.SetCurrentValue(FrameworkElement.ContextMenuProperty, TaskbarIconContextMenu());
        _taskbarIcon.TrayMouseDoubleClick += TaskbarIconDoubleClick;
    }

    /// <summary>
    /// </summary>
    public void StartMinimized()
    {
        _taskbarIcon.SetCurrentValue(UIElement.VisibilityProperty, Visibility.Visible);
        _mainWindow.Hide();
    }

    // ReSharper disable UseObjectOrCollectionInitializer
    private ContextMenu TaskbarIconContextMenu()
    {
        var contextMenu = new ContextMenu();

        foreach (var device in _activeDevices.Value.OrderBy(d => d.PositionX))
        {
            var currentButton = new Button();

            foreach (Canvas canvas in _mainWindow.DisplayStackPanel.Children)
            foreach (Button button in canvas.Children)
            {
                if (button.Name != $"ButtonDisplay{device.Id}")
                {
                    continue;
                }

                currentButton = button;
                break;
            }

            var parent = new MenuItem
                         {
                             Header = device.Name
                         };
            parent.Icon = new PackIconMaterial
                          {
                              Kind = PackIconMaterialKind.Monitor
                          };

            //anticlockwise
            var anticlockwiseItem = new MenuItem
                                    {
                                        Header = "Upright 'anticlockwise'",
                                        Icon = new PackIconMaterial
                                               {
                                                   Kind = PackIconMaterialKind.ChevronLeft
                                               }
                                    };
            anticlockwiseItem.Click += (_, _) =>
                                       {
                                           _rotateDisplay.RunFor(NativeMethods.Dmdo90, device.Id);
                                           _rotateButtonAndCanvas.RunFor(NativeMethods.Dmdo90, currentButton);
                                           _mainWindow.SetWindowMargins();
                                       };

            //180
            var clockwiseItem = new MenuItem
                                {
                                    Header = "Landscape (rotated)",
                                    Icon = new PackIconMaterial
                                           {
                                               Kind = PackIconMaterialKind.ChevronUp
                                           }
                                };
            clockwiseItem.Click += (_, _) =>
                                   {
                                       _rotateDisplay.RunFor(NativeMethods.Dmdo180, device.Id);
                                       _rotateButtonAndCanvas.RunFor(NativeMethods.Dmdo180, currentButton);
                                       _mainWindow.SetWindowMargins();
                                   };

            //clockwise
            var mirrorItem = new MenuItem
                             {
                                 Header = "Upright 'clockwise'",
                                 Icon = new PackIconMaterial
                                        {
                                            Kind = PackIconMaterialKind.ChevronRight
                                        }
                             };
            mirrorItem.Click += (_, _) =>
                                {
                                    _rotateDisplay.RunFor(NativeMethods.Dmdo270, device.Id);
                                    _rotateButtonAndCanvas.RunFor(NativeMethods.Dmdo270, currentButton);
                                    _mainWindow.SetWindowMargins();
                                };

            //restore
            var restoreItem = new MenuItem
                              {
                                  Header = "Reset",
                                  Icon = new PackIconMaterial
                                         {
                                             Kind = PackIconMaterialKind.ChevronDown
                                         }
                              };
            restoreItem.Click += (_, _) =>
                                 {
                                     _rotateDisplay.RunFor(NativeMethods.DmdoDefault, device.Id);
                                     _rotateButtonAndCanvas.RunFor(NativeMethods.DmdoDefault, currentButton);
                                     _mainWindow.SetWindowMargins();
                                 };

            parent.Items.Add(anticlockwiseItem);
            parent.Items.Add(clockwiseItem);
            parent.Items.Add(mirrorItem);
            parent.Items.Add(restoreItem);
            contextMenu.Items.Add(parent);
        }

        var aboutApplication = new MenuItem
                               {
                                   Header = "About...",
                                   Icon = new PackIconMaterial
                                          {
                                              Kind = PackIconMaterialKind.Information
                                          }
                               };
        aboutApplication.Click += ContextMenuItemAboutClick;

        var restoreApplication = new MenuItem
                                 {
                                     Header = "Restore application",
                                     Icon = new PackIconMaterial
                                            {
                                                Kind = PackIconMaterialKind.WindowRestore
                                            }
                                 };
        restoreApplication.Click += ContextMenuItemRestoreClick;

        var closeApplication = new MenuItem
                               {
                                   Header = "Close application",
                                   Icon = new PackIconMaterial
                                          {
                                              Kind = PackIconMaterialKind.Power
                                          }
                               };
        closeApplication.Click += ContextMenuItemCloseClick;

        contextMenu.Items.Add(new Separator());
        contextMenu.Items.Add(aboutApplication);
        contextMenu.Items.Add(restoreApplication);
        contextMenu.Items.Add(closeApplication);

        return contextMenu;
    }

    // ReSharper restore UseObjectOrCollectionInitializer

    private void ContextMenuItemCloseClick(object sender, RoutedEventArgs e)
    {
        _taskbarIcon.Dispose();
        _mainWindow.Close();
    }

    private void ContextMenuItemAboutClick(object sender, RoutedEventArgs e)
    {
        ICurrentAssembly currentAssembly = new CurrentAssembly();
        IAboutContent aboutContent = new AboutContent(currentAssembly);
        IAboutModel aboutModel = new AboutViewModel(aboutContent, _applicationStyle);
        var aboutWindow = new AboutWindow(aboutModel);
        aboutWindow.ShowDialog();
    }

    private void ContextMenuItemRestoreClick(object sender, RoutedEventArgs e)
    {
        _mainWindow.CheckScreenCountAndRestore();
    }

    private void TaskbarIconDoubleClick(object sender, RoutedEventArgs e)
    {
        _mainWindow.CheckScreenCountAndRestore();
    }
}