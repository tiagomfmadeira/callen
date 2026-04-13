using System;
using System.Windows;

namespace Callen.Windows
{
    internal sealed class WindowOverlaySync : IDisposable
    {
        private readonly Window overlay;
        private readonly Action<Window> onSynchronized;
        private Window host;

        public WindowOverlaySync(Window overlay, Action<Window> onSynchronized = null)
        {
            this.overlay = overlay ?? throw new ArgumentNullException(nameof(overlay));
            this.onSynchronized = onSynchronized;
        }

        public bool IsAttached => host != null;

        public void Attach()
        {
            Detach();

            host = overlay.Owner ?? Application.Current?.MainWindow;
            if (host == null || ReferenceEquals(host, overlay))
            {
                host = null;
                return;
            }

            overlay.WindowStartupLocation = WindowStartupLocation.Manual;
            overlay.SizeToContent = SizeToContent.Manual;

            SyncToHost(host);

            host.LocationChanged += Host_Changed;
            host.SizeChanged += Host_SizeChanged;
            host.StateChanged += Host_Changed;
        }

        public void Detach()
        {
            if (host == null)
                return;

            host.LocationChanged -= Host_Changed;
            host.SizeChanged -= Host_SizeChanged;
            host.StateChanged -= Host_Changed;
            host = null;
        }

        public void Dispose()
        {
            Detach();
        }

        private void Host_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Window ownerWindow)
                SyncToHost(ownerWindow);
        }

        private void Host_Changed(object sender, EventArgs e)
        {
            if (sender is Window ownerWindow)
                SyncToHost(ownerWindow);
        }

        private void SyncToHost(Window ownerWindow)
        {
            overlay.Left = ownerWindow.Left;
            overlay.Top = ownerWindow.Top;
            overlay.Width = ownerWindow.ActualWidth > 0 ? ownerWindow.ActualWidth : ownerWindow.Width;
            overlay.Height = ownerWindow.ActualHeight > 0 ? ownerWindow.ActualHeight : ownerWindow.Height;

            onSynchronized?.Invoke(ownerWindow);
        }
    }
}
