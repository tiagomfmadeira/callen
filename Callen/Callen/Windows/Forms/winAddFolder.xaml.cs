using System.Windows;
using System.Windows.Input;

namespace Callen.Windows.Forms
{
    /// <summary>
    ///     Interaction logic for winAddFolder.xaml
    /// </summary>
    public partial class winAddFolder : Window
    {
        public winAddFolder()
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

        public winAddFolder(string code)
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

            wind_name.Content = "Adicionar Tema";
            folder_box.Text = code;
            folder_box.IsEnabled = false;
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

        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(folder_box.Text) || string.IsNullOrEmpty(theme_box.Text))
            {
                MessageBox.Show("Necessita de ambos os atributos");
                return;
            }

            DBConnect.createFolder(folder_box.Text, theme_box.Text);

            Close();
        }
    }
}