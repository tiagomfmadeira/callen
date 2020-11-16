using System.Windows;
using System.Windows.Input;

namespace Callen.Windows
{
    /// <summary>
    ///     Interaction logic for winHelp.xaml
    /// </summary>
    public partial class winHelp : Window
    {
        public winHelp()
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