using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Callen.Windows.Other
{
    public partial class ImageZoomWindow : Window
    {
        private const double ImageHeightRatio = 0.75; // 75% of available vertical space
        private readonly WindowOverlaySync overlaySync;

        public ImageZoomWindow(ImageSource src)
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this, host =>
                ApplyImageSizing(host.ActualHeight > 0 ? host.ActualHeight : host.Height));

            PreviewKeyDown += HandleEsc;

            SourceInitialized += WinZoomImage_SourceInitialized;
            Loaded += WinZoomImage_Loaded;
            Closed += WinZoomImage_Closed;

            // Keep hidden until sizing is applied (prevents flicker)
            img.Visibility = Visibility.Hidden;
            img.MaxHeight = 0;

            img.Source = src;

            // If Owner is already available, pre-apply sizing once to reduce any chance of a flash
            var ownerWindow = Owner ?? Application.Current?.MainWindow;
            if (ownerWindow != null && !ReferenceEquals(ownerWindow, this))
                ApplyImageSizing(ownerWindow.ActualHeight > 0 ? ownerWindow.ActualHeight : ownerWindow.Height);
        }

        // ---------- Overlay + image sizing (KISS) ----------

        private void WinZoomImage_SourceInitialized(object sender, EventArgs e)
        {
            overlaySync.Attach();
        }

        private void WinZoomImage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!overlaySync.IsAttached)
                ApplyImageSizing(ActualHeight > 0 ? ActualHeight : Height);

            img.Visibility = Visibility.Visible;
        }

        private void WinZoomImage_Closed(object sender, EventArgs e)
        {
            PreviewKeyDown -= HandleEsc;
            overlaySync.Detach();
        }

        private void ApplyImageSizing(double availableHeight)
        {
            if (availableHeight <= 0) return;
            img.MaxHeight = availableHeight * ImageHeightRatio;
        }

        // ---------- Close behavior ----------

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}

