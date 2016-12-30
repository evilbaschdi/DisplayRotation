using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using DisplayRotation.Internal;
using Hardcodet.Wpf.TaskbarNotification;

namespace DisplayRotation.Core
{
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly MainWindow _mainWindow;
        private readonly TaskbarIcon _taskbarIcon;
        private readonly IActiveDevices _activeDevices;
        private readonly IRotateDisplay _rotateDisplay;
        private readonly IRotateButtonAndCanvas _rotateButtonAndCanvas;

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
        public ApplicationSettings(MainWindow mainWindow, TaskbarIcon taskbarIcon, IActiveDevices activeDevices, IRotateDisplay rotateDisplay,
                                   IRotateButtonAndCanvas rotateButtonAndCanvas)
        {
            if (mainWindow == null)
            {
                throw new ArgumentNullException(nameof(mainWindow));
            }
            if (taskbarIcon == null)
            {
                throw new ArgumentNullException(nameof(taskbarIcon));
            }
            if (activeDevices == null)
            {
                throw new ArgumentNullException(nameof(activeDevices));
            }
            if (rotateDisplay == null)
            {
                throw new ArgumentNullException(nameof(rotateDisplay));
            }
            if (rotateButtonAndCanvas == null)
            {
                throw new ArgumentNullException(nameof(rotateButtonAndCanvas));
            }

            _mainWindow = mainWindow;
            _taskbarIcon = taskbarIcon;
            _activeDevices = activeDevices;
            _rotateDisplay = rotateDisplay;
            _rotateButtonAndCanvas = rotateButtonAndCanvas;
        }

        /// <summary>
        ///     Initialisiert eine neue Instanz der <see cref="T:System.Object" />-Klasse.
        /// </summary>
        /// <summary>
        /// </summary>
        public void Run()
        {
            StartMinimized();
            _taskbarIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location);
            _taskbarIcon.ContextMenu = TaskbarIconContextMenu();
            _taskbarIcon.TrayMouseDoubleClick += TaskbarIconDoubleClick;
        }

        /// <summary>
        /// </summary>
        private void StartMinimized()
        {
            _taskbarIcon.Visibility = Visibility.Visible;
            _mainWindow.Hide();
        }

        private ContextMenu TaskbarIconContextMenu()
        {
            var contextMenu = new ContextMenu();

            foreach (var device in _activeDevices.Get().OrderBy(d => d.PositionX))
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

                var parent = new MenuItem();
                parent.Header = device.Name;

                //anticlockwise
                var anticlockwiseItem = new MenuItem();
                anticlockwiseItem.Header = "Upright 'anticlockwise'";
                anticlockwiseItem.Click += (sender, args) =>
                                           {
                                               _rotateDisplay.For(NativeMethods.Dmdo90, device.Id);
                                               _rotateButtonAndCanvas.For(NativeMethods.Dmdo90, currentButton);
                                               _mainWindow.SetWindowMargins();
                                           };

                //180
                var clockwiseItem = new MenuItem();
                clockwiseItem.Header = "Landscape (rotated)";
                clockwiseItem.Click += (sender, args) =>
                                       {
                                           _rotateDisplay.For(NativeMethods.Dmdo180, device.Id);
                                           _rotateButtonAndCanvas.For(NativeMethods.Dmdo180, currentButton);
                                           _mainWindow.SetWindowMargins();
                                       };

                //clockwise
                var mirrorItem = new MenuItem();
                mirrorItem.Header = "Upright 'clockwise'";
                mirrorItem.Click += (sender, args) =>
                                    {
                                        _rotateDisplay.For(NativeMethods.Dmdo270, device.Id);
                                        _rotateButtonAndCanvas.For(NativeMethods.Dmdo270, currentButton);
                                        _mainWindow.SetWindowMargins();
                                    };

                //restore
                var restoreItem = new MenuItem();
                restoreItem.Header = "Reset";
                restoreItem.Click += (sender, args) =>
                                     {
                                         _rotateDisplay.For(NativeMethods.DmdoDefault, device.Id);
                                         _rotateButtonAndCanvas.For(NativeMethods.DmdoDefault, currentButton);
                                         _mainWindow.SetWindowMargins();
                                     };


                parent.Items.Add(anticlockwiseItem);
                parent.Items.Add(clockwiseItem);
                parent.Items.Add(mirrorItem);
                parent.Items.Add(restoreItem);
                contextMenu.Items.Add(parent);
            }

            var restoreApplication = new MenuItem();
            restoreApplication.Header = "Restore application";
            restoreApplication.Click += ContextMenuItemRestoreClick;

            var closeApplication = new MenuItem();
            closeApplication.Header = "Close application";
            closeApplication.Click += ContextMenuItemCloseClick;

            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(restoreApplication);
            contextMenu.Items.Add(closeApplication);


            return contextMenu;
        }

        private void ContextMenuItemCloseClick(object sender, EventArgs e)
        {
            _taskbarIcon.Dispose();
            _mainWindow.Close();
        }

        private void ContextMenuItemRestoreClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }

        private void TaskbarIconDoubleClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }
    }
}