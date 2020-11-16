using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Callen.Windows.Other
{
    /// <summary>
    ///     Interaction logic for winZoomImage.xaml
    /// </summary>
    public partial class winZoomImage : Window
    {
        public winZoomImage(ImageSource src)
        {
            InitializeComponent();
            PreviewKeyDown += HandleEsc;

            var parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            img.Source = src;
        }

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