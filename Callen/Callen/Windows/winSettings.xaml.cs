using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Configuration;

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for winSettings.xaml
    /// </summary>
    public partial class winSettings : Window
    {
        //Create the object
        Configuration config;

        public winSettings()
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

            if (Application.Current.Resources["mainColor"].ToString() == "#FFFFFFFF") // Check Color Scheme
                btn_change_color.Content = "Light Mode";

            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            db_string.Text = config.ConnectionStrings.ConnectionStrings["ConString"].ConnectionString;
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

        private void btn_change_color_Click(object sender, RoutedEventArgs e) // Changes the color Scheme 
        {
            if (btn_change_color.Content.ToString() == "Light Mode") // Set to Dark Mode
            {
                Application.Current.Resources["mainColor"] = new SolidColorBrush(Color.FromRgb(0x36,0x39,0x3E));
                btn_change_color.Content = "Dark Mode";
            }
            else // Set Light Mode
            {
                Application.Current.Resources["mainColor"] = new SolidColorBrush(Colors.White);
                btn_change_color.Content = "Light Mode";
            }

        }

        private void btn_save_changes_Click(object sender, RoutedEventArgs e) // Save changes
        {
            // Save changes in the connection string
            config.ConnectionStrings.ConnectionStrings["ConString"].ConnectionString = db_string.Text;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("ConnectionStrings");

            // Saved Notification popup
            winNotification noti = new winNotification("Settings Saved", "As modificações nas definições foram salvas com sucesso" , "");
            noti.Show();
        }
    }
}
