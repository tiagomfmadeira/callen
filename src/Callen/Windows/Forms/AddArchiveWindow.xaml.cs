using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Callen.Windows;

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

            SourceInitialized += WinAddFolder_SourceInitialized;
            Loaded += WinAddFolder_Loaded;
            Closed += WinAddFolder_Closed;
        }

        public AddArchiveWindow(string code)
            : this()
        {
            fixedFolderCode = code;
        }

        private void WinAddFolder_SourceInitialized(object sender, EventArgs e)
        {
            overlaySync.Attach();
        }

        private void WinAddFolder_Loaded(object sender, RoutedEventArgs e)
        {
            // Existing behavior (theme mode)
            if (!string.IsNullOrEmpty(fixedFolderCode))
            {
                wind_name.Content = Loc.T("AddArchive.TitleTheme");
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
            var missingFields = new List<string>();
            if (string.IsNullOrWhiteSpace(folder_box.Text))
                missingFields.Add(Loc.T("AddArchive.Folder"));
            if (string.IsNullOrWhiteSpace(theme_box.Text))
                missingFields.Add(Loc.T("AddArchive.Theme"));

            if (missingFields.Count > 0)
            {
                var missingDialog = new ActionDialogWindow(
                    Loc.T("AddArchive.MissingFieldsTitle"),
                    string.Join(", ", missingFields),
                    Loc.T("AddArchive.MissingFieldsMessage"),
                    Loc.T("Dlg.Close"));

                DialogHelper.ShowOwnedDialog(missingDialog, this);
                return;
            }

            DBConnect.CreateFolder(folder_box.Text, theme_box.Text);
            Close();
        }
    }
}


