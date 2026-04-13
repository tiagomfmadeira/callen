using System;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;

namespace Callen.Windows
{
    /// <summary>
    ///     Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly Configuration config;
        private readonly WindowOverlaySync overlaySync;

        public SettingsWindow()
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);

            PreviewKeyDown += HandleEsc;
            Loaded += WinSettings_Loaded;
            Closed += WinSettings_Closed;

            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            db_string.Text = App.DatabasePath;
            img_folder.Text = App.ImagePath;
        }

        private void WinSettings_Loaded(object sender, RoutedEventArgs e)
        {
            overlaySync.Attach();
        }

        private void WinSettings_Closed(object sender, EventArgs e)
        {
            PreviewKeyDown -= HandleEsc;
            overlaySync.Detach();
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

        private void btn_save_changes_Click(object sender, RoutedEventArgs e)
        {
            var configuredDbPath = db_string.Text?.Trim() ?? string.Empty;
            var configuredImagePath = img_folder.Text?.Trim() ?? string.Empty;

            if (config.AppSettings.Settings["database_path"] == null)
                config.AppSettings.Settings.Add("database_path", configuredDbPath);
            else
                config.AppSettings.Settings["database_path"].Value = configuredDbPath;

            if (config.AppSettings.Settings["image_path"] == null)
                config.AppSettings.Settings.Add("image_path", configuredImagePath);
            else
                config.AppSettings.Settings["image_path"].Value = configuredImagePath;

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("AppSettings");

            new NotificationWindow("Settings Saved", "As modificações nas definições foram salvas com sucesso", "").Show();
            Close();
        }

        private void btn_img_folder_Click(object sender, RoutedEventArgs e)
        {
            var diag = new VistaFolderBrowserDialog();
            if (diag.ShowDialog() == true)
                img_folder.Text = diag.SelectedPath;
        }

        private void btn_db_path_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Selecionar base de dados",
                Filter = "SQLite DB (*.db;*.sqlite)|*.db;*.sqlite|All files (*.*)|*.*",
                CheckFileExists = false
            };

            if (dialog.ShowDialog() == true)
                db_string.Text = dialog.FileName;
        }
    }
}

