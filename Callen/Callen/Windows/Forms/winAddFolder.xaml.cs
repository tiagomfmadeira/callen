using System;
using System.Windows;
using System.Windows.Input;


using System.Data.SqlClient;

namespace Callen.Windows.Forms
{
    /// <summary>
    /// Interaction logic for winAddFolder.xaml
    /// </summary>
    public partial class winAddFolder : Window
    {
        public winAddFolder()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            Window parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }
        }

        public winAddFolder(string code)
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            Window parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
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
                this.Close();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(folder_box.Text.ToString()) || string.IsNullOrEmpty(theme_box.Text.ToString()))
            {
                MessageBox.Show("Necessita de ambos os atributos");
                return;
            }

            DBConnect.createFolder(folder_box.Text.ToString(), theme_box.Text.ToString());
     
            this.Close();
        }
    }
}