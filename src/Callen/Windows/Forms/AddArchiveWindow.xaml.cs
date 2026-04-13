using System;
using System.Windows;
using System.Windows.Input;

namespace Callen.Windows.Forms
{
    /// <summary>
    /// Creates a folder or a theme entry depending on how the window is opened.
    /// </summary>
    public partial class AddArchiveWindow : Window
    {
        private readonly WindowOverlaySync overlaySync;
        private readonly string fixedFolderCode;

        public AddArchiveWindow()
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);
            PreviewKeyDown += HandleEsc;

            Loaded += WinAddFolder_Loaded;
            Closed += WinAddFolder_Closed;
        }

        public AddArchiveWindow(string code)
            : this()
        {
            fixedFolderCode = code;
        }

        private void WinAddFolder_Loaded(object sender, RoutedEventArgs e)
        {
            overlaySync.Attach();

            // Existing behavior (theme mode)
            if (!string.IsNullOrEmpty(fixedFolderCode))
            {
                wind_name.Content = "Adicionar Tema";
                folder_box.Text = fixedFolderCode;
                folder_box.IsEnabled = false;
            }
        }

        private void WinAddFolder_Closed(object sender, EventArgs e)
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

        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(folder_box.Text) || string.IsNullOrWhiteSpace(theme_box.Text))
            {
                MessageBox.Show("Necessita de ambos os atributos");
                return;
            }

            DBConnect.CreateFolder(folder_box.Text, theme_box.Text);
            Close();
        }
    }
}

