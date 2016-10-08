using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using DisplayRotation.Internal;
using Button = System.Windows.Controls.Button;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;

namespace DisplayRotation.Core
{
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly MainWindow _mainWindow;
        private NotifyIcon _ni;
        private readonly IActiveDevices _activeDevices;
        private readonly IRotateDisplay _rotateDisplay;
        private readonly IRotateButtonAndCanvas _rotateButtonAndCanvas;

        /// <summary>
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="ni"></param>
        /// <param name="activeDevices"></param>
        /// <param name="rotateDisplay"></param>
        /// <param name="rotateButtonAndCanvas"></param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="mainWindow" /> is <see langword="null" />.
        ///     <paramref name="ni" /> is <see langword="null" />.
        ///     <paramref name="activeDevices" /> is <see langword="null" />.
        ///     <paramref name="rotateDisplay" /> is <see langword="null" />.
        ///     <paramref name="rotateButtonAndCanvas" /> is <see langword="null" />.
        /// </exception>
        public ApplicationSettings(MainWindow mainWindow, NotifyIcon ni, IActiveDevices activeDevices, IRotateDisplay rotateDisplay, IRotateButtonAndCanvas rotateButtonAndCanvas)
        {
            if (mainWindow == null)
            {
                throw new ArgumentNullException(nameof(mainWindow));
            }
            if (ni == null)
            {
                throw new ArgumentNullException(nameof(ni));
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
            _ni = ni;
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
            _ni.ContextMenu = NotifyIconContextMenu();
            _ni.DoubleClick += NotifyIcon_DoubleClick;
        }

        /// <summary>
        /// </summary>
        public void StartMinimized()
        {
            _ni = new NotifyIcon
                  {
                      Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location),
                      Visible = true
                  };

            _mainWindow.Hide();
        }

        private ContextMenu NotifyIconContextMenu()
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

                var parent = new MenuItem(device.Name);

                //anticlockwise
                var anticlockwiseItem = new MenuItem("Upright 'anticlockwise'");
                anticlockwiseItem.Click += (sender, args) =>
                                           {
                                               _rotateDisplay.For(NativeMethods.Dmdo90, device.Id);
                                               _rotateButtonAndCanvas.For(NativeMethods.Dmdo90, currentButton);
                                               _mainWindow.SetWindowMargins();
                                           };

                //180
                var clockwiseItem = new MenuItem("Landscape (rotated)");
                clockwiseItem.Click += (sender, args) =>
                                       {
                                           _rotateDisplay.For(NativeMethods.Dmdo180, device.Id);
                                           _rotateButtonAndCanvas.For(NativeMethods.Dmdo180, currentButton);
                                           _mainWindow.SetWindowMargins();
                                       };

                //clockwise
                var mirrorItem = new MenuItem("Upright 'clockwise'");
                mirrorItem.Click += (sender, args) =>
                                    {
                                        _rotateDisplay.For(NativeMethods.Dmdo270, device.Id);
                                        _rotateButtonAndCanvas.For(NativeMethods.Dmdo270, currentButton);
                                        _mainWindow.SetWindowMargins();
                                    };

                //restore
                var restoreItem = new MenuItem("Reset");
                restoreItem.DefaultItem = true;
                restoreItem.Click += (sender, args) =>
                                     {
                                         _rotateDisplay.For(NativeMethods.DmdoDefault, device.Id);
                                         _rotateButtonAndCanvas.For(NativeMethods.DmdoDefault, currentButton);
                                         _mainWindow.SetWindowMargins();
                                     };

                parent.MenuItems.AddRange(new[] { anticlockwiseItem, clockwiseItem, mirrorItem, restoreItem });

                contextMenu.MenuItems.Add(parent);
            }
            contextMenu.MenuItems.Add("-");
            var restore = new MenuItem("Restore application");
            restore.Click += ContextMenuItemRestore_Click;

            var close = new MenuItem("Close application");
            close.Click += ContextMenuItemClose_Click;

            contextMenu.MenuItems.AddRange(new[] { restore, close });

            return contextMenu;
        }

        private void ContextMenuItemClose_Click(object sender, EventArgs e)
        {
            _ni.Dispose();
            _mainWindow.Close();
        }

        private void ContextMenuItemRestore_Click(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }
    }
}