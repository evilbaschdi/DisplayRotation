using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro.IconPacks;

namespace DisplayRotation.Internal
{
    public class TaskbarIconConfiguration : ITaskbarIconConfiguration
    {
        private readonly IActiveDevices _activeDevices;
        private readonly MainWindow _mainWindow;
        private readonly IRotateButtonAndCanvas _rotateButtonAndCanvas;
        private readonly IRotateDisplay _rotateDisplay;
        private readonly TaskbarIcon _taskbarIcon;

        /// <summary>
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="taskbarIcon"></param>
        /// <param name="activeDevices"></param>
        /// <param name="rotateDisplay"></param>
        /// <param name="rotateButtonAndCanvas"></param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="mainWindow" /> is <see langword="null" />.
        ///     <paramref name="taskbarIcon" /> is <see langword="null" />.
        ///     <paramref name="activeDevices" /> is <see langword="null" />.
        ///     <paramref name="rotateDisplay" /> is <see langword="null" />.
        ///     <paramref name="rotateButtonAndCanvas" /> is <see langword="null" />.
        /// </exception>
        public TaskbarIconConfiguration(MainWindow mainWindow, TaskbarIcon taskbarIcon, IActiveDevices activeDevices, IRotateDisplay rotateDisplay,
                                        IRotateButtonAndCanvas rotateButtonAndCanvas)
        {
            _mainWindow = mainWindow ?? throw new ArgumentNullException(nameof(mainWindow));
            _taskbarIcon = taskbarIcon ?? throw new ArgumentNullException(nameof(taskbarIcon));
            _activeDevices = activeDevices ?? throw new ArgumentNullException(nameof(activeDevices));
            _rotateDisplay = rotateDisplay ?? throw new ArgumentNullException(nameof(rotateDisplay));
            _rotateButtonAndCanvas = rotateButtonAndCanvas ?? throw new ArgumentNullException(nameof(rotateButtonAndCanvas));
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        /// <summary>
        /// </summary>
        public void Run()
        {
            var filePath = Assembly.GetEntryAssembly().Location;
            _taskbarIcon.Icon = Icon.ExtractAssociatedIcon(filePath);
            _taskbarIcon.ContextMenu = TaskbarIconContextMenu();
            _taskbarIcon.TrayMouseDoubleClick += TaskbarIconDoubleClick;
        }

        /// <summary>
        /// </summary>
        public void StartMinimized()
        {
            _taskbarIcon.Visibility = Visibility.Visible;
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
                {
                    foreach (Button button in canvas.Children)
                    {
                        if (button.Name == $"ButtonDisplay{device.Id}")
                        {
                            currentButton = button;
                            break;
                        }
                    }
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
                anticlockwiseItem.Click += (sender, args) =>
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
                clockwiseItem.Click += (sender, args) =>
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
                mirrorItem.Click += (sender, args) =>
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
                restoreItem.Click += (sender, args) =>
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
            contextMenu.Items.Add(restoreApplication);
            contextMenu.Items.Add(closeApplication);


            return contextMenu;
        }

        // ReSharper restore UseObjectOrCollectionInitializer

        private void ContextMenuItemCloseClick(object sender, EventArgs e)
        {
            _taskbarIcon.Dispose();
            _mainWindow.Close();
        }

        private void ContextMenuItemRestoreClick(object sender, EventArgs e)
        {
            _mainWindow.CheckScreenCountAndRestore(sender, null);
        }

        private void TaskbarIconDoubleClick(object sender, EventArgs e)
        {
            _mainWindow.CheckScreenCountAndRestore(sender, null);
        }
    }
}