using DisplayRotation.Internal;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace DisplayRotation.Core
{
    public class ApplicationSettings : IApplicationSettings
    {
        private readonly MainWindow _mainWindow;
        private NotifyIcon _ni;
        private readonly IActiveDevices _activeDevices;
        private readonly IRotateDisplay _rotateDisplay;

        /// <summary>
        /// </summary>
        /// <param name="mainWindow"></param>
        /// <param name="ni"></param>
        /// <param name="activeDevices"></param>
        /// <param name="rotateDisplay"></param>
        public ApplicationSettings(MainWindow mainWindow, NotifyIcon ni, IActiveDevices activeDevices,
            IRotateDisplay rotateDisplay)
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
            _mainWindow = mainWindow;
            _ni = ni;
            _activeDevices = activeDevices;
            _rotateDisplay = rotateDisplay;
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
            //_ni.Click += (sender, args) => _ni.ShowBalloonTip(10);
        }

        /// <summary>
        /// </summary>
        public void StartMinimized()
        {
            _ni = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().Location),
                //BalloonTipTitle = Resources.ApplicationSettings_StartMinimized_BalloonTipTitle,
                Visible = true
            };

            _mainWindow.Hide();
        }

        private ContextMenu NotifyIconContextMenu()
        {
            var contextMenu = new ContextMenu();

            foreach (var device in _activeDevices.Get().OrderBy(d => d.PositionX))
            {
                var parent = new MenuItem(device.Name);

                //anticlockwise
                var anticlockwiseItem = new MenuItem("Hochformat 'anticlockwise'");
                anticlockwiseItem.Click += (sender, args) => _rotateDisplay.For(NativeMethods.Dmdo90, device.Id);

                //180
                var clockwiseItem = new MenuItem("Querformat (gedreht)");
                clockwiseItem.Click += (sender, args) => _rotateDisplay.For(NativeMethods.Dmdo180, device.Id);

                //clockwise
                var mirrorItem = new MenuItem("Hochformat (gedreht) 'clockwise'");
                mirrorItem.Click += (sender, args) => _rotateDisplay.For(NativeMethods.Dmdo270, device.Id);

                //restore
                var restoreItem = new MenuItem("Querformat");
                restoreItem.DefaultItem = true;
                restoreItem.Click += (sender, args) => _rotateDisplay.For(NativeMethods.DmdoDefault, device.Id);

                parent.MenuItems.AddRange(new[] { anticlockwiseItem, clockwiseItem, mirrorItem, restoreItem });

                contextMenu.MenuItems.Add(parent);
            }
            contextMenu.MenuItems.Add("-");
            var restore = new MenuItem("Fenster öffnen");
            restore.Click += ContextMenuItemRestore_Click;

            var close = new MenuItem("Beenden");
            close.Click += ContextMenuItemClose_Click;

            contextMenu.MenuItems.AddRange(new[] { restore, close });

            return contextMenu;
        }

        private void ContextMenuItemClose_Click(object sender, EventArgs e)
        {
            // Will Close Your Application
            _ni.Dispose();
            _mainWindow.Close();
        }

        private void ContextMenuItemRestore_Click(object sender, EventArgs e)
        {
            //Will Restore Your Application
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            //Will Restore Your Application
            _mainWindow.Show();
            _mainWindow.WindowState = WindowState.Normal;
        }
    }
}