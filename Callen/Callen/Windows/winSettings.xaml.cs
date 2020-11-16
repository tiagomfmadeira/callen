using System.Configuration;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ookii.Dialogs.Wpf;

namespace Callen.Windows
{
    /// <summary>
    ///     Interaction logic for winSettings.xaml
    /// </summary>
    public partial class winSettings : Window
    {
        //Create the object
        private readonly Configuration config;

        public winSettings()
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

            if (Application.Current.Resources["mainColor"].ToString() == "#FFFFFFFF") // Check Color Scheme
                btn_change_color.Content = "Light Mode";

            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            db_string.Text = config.ConnectionStrings.ConnectionStrings["ConString"].ConnectionString;
            img_folder.Text = config.AppSettings.Settings["image_path"].Value;
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

        private void btn_change_color_Click(object sender, RoutedEventArgs e) // Changes the color Scheme 
        {
            if (btn_change_color.Content.ToString() == "Light Mode") // Set to Dark Mode
            {
                Application.Current.Resources["mainColor"] = new SolidColorBrush(Color.FromRgb(0x36, 0x39, 0x3E));
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
            // Write the changes to the connection string and image path to there settings
            config.ConnectionStrings.ConnectionStrings["ConString"].ConnectionString = db_string.Text;
            config.AppSettings.Settings["image_path"].Value = img_folder.Text;

            // Save and refresh settings
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("ConnectionStrings");
            ConfigurationManager.RefreshSection("AppSettings");

            // Saved Notification popup
            var noti = new winNotification("Settings Saved", "As modificações nas definições foram salvas com sucesso",
                "");
            noti.Show();

            // Close the settings window
            Close();
        }

        private void btn_img_folder_Click(object sender, RoutedEventArgs e)
        {
            var diag = new VistaFolderBrowserDialog();
            diag.ShowDialog();
            img_folder.Text = diag.SelectedPath;
        }
    }
}