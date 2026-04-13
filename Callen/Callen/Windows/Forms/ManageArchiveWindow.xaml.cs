using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Callen.Windows;

namespace Callen.Windows.Forms
{
    public partial class ManageArchiveWindow : Window
    {
        private readonly WindowOverlaySync overlaySync;
        private readonly string initialFolderCode;
        private readonly int? initialArchiveId;
        public string SelectedFolderCode { get; private set; }
        public int? SelectedArchiveId { get; private set; }

        public ManageArchiveWindow()
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);

            PreviewKeyDown += HandleEsc;
            Loaded += WinManageArchive_Loaded;
            Closed += WinManageArchive_Closed;
        }

        public ManageArchiveWindow(string folderCode, int? archiveId = null)
            : this()
        {
            initialFolderCode = folderCode;
            initialArchiveId = archiveId;
        }

        private void WinManageArchive_Loaded(object sender, RoutedEventArgs e)
        {
            overlaySync.Attach();
            LoadFolders();
        }

        private void WinManageArchive_Closed(object sender, EventArgs e)
        {
            PreviewKeyDown -= HandleEsc;
            SelectedFolderCode = (combo_folder.SelectedItem as Archive)?.code;
            SelectedArchiveId = (list_themes.SelectedItem as Archive)?.id;
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

        private void combo_folder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btn_add_theme.IsEnabled = combo_folder.SelectedItem != null;
            SelectedFolderCode = (combo_folder.SelectedItem as Archive)?.code;
            RefreshThemeList(initialArchiveId);
        }

        private void list_themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedArchive = list_themes.SelectedItem as Archive;
            if (selectedArchive == null)
            {
                SelectedArchiveId = null;
                lbl_usage.Content = "Selecione um tema";
                theme_name_box.Text = string.Empty;
                btn_delete_theme.IsEnabled = false;
                btn_rename_theme.IsEnabled = false;
                return;
            }

            SelectedArchiveId = selectedArchive.id;
            theme_name_box.Text = selectedArchive.theme;
            btn_delete_theme.IsEnabled = selectedArchive.usage_count == 0;
            btn_rename_theme.IsEnabled = true;
            lbl_usage.Content = selectedArchive.usage_count > 0
                ? "Tema em uso por " + selectedArchive.usage_count + " calendários"
                : "Tema sem calendários associados";
        }

        private void btn_rename_folder_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = combo_folder.SelectedItem as Archive;
            if (selectedFolder == null || string.IsNullOrWhiteSpace(folder_name_box.Text))
                return;

            var newFolderCode = folder_name_box.Text.Trim();
            var oldFolderCode = selectedFolder.code;

            DBConnect.RenameFolder(oldFolderCode, newFolderCode);
            LoadFolders(newFolderCode);

            new NotificationWindow("Pasta Modificada", oldFolderCode, "Foi modificada com sucesso").Show();
        }

        private void btn_rename_theme_Click(object sender, RoutedEventArgs e)
        {
            var selectedArchive = list_themes.SelectedItem as Archive;
            if (selectedArchive == null || string.IsNullOrWhiteSpace(theme_name_box.Text))
                return;

            var oldThemeName = selectedArchive.theme;
            var newThemeName = theme_name_box.Text.Trim();

            DBConnect.RenameArchiveTheme(selectedArchive.id, newThemeName);
            RefreshThemeList(selectedArchive.id);

            new NotificationWindow("Tema Modificado", oldThemeName, "Foi modificado com sucesso").Show();
        }

        private void btn_delete_theme_Click(object sender, RoutedEventArgs e)
        {
            var selectedArchive = list_themes.SelectedItem as Archive;
            if (selectedArchive == null)
                return;

            if (selectedArchive.usage_count > 0)
            {
                var usageDialog = new ActionDialogWindow(
                    "Tema em uso",
                    selectedArchive.theme,
                    "Não é possível eliminar um tema que ainda está associado a calendários.",
                    "Fechar")
                {
                    Owner = this
                };

                DialogHelper.ShowOwnedDialog(usageDialog, this, 0.85);
                return;
            }

            var deleteDialog = new ActionDialogWindow(
                "Eliminar tema",
                selectedArchive.theme,
                "Tem a certeza que pretende eliminar este tema?",
                "Eliminar",
                "Cancelar")
            {
                Owner = this
            };

            DialogHelper.ShowOwnedDialog(deleteDialog, this, 0.85);

            if (deleteDialog.SelectedAction != DialogAction.Primary)
                return;

            if (!DBConnect.DeleteArchiveEntry(selectedArchive.id))
            {
                var errorDialog = new ActionDialogWindow(
                    "Erro a eliminar tema",
                    selectedArchive.theme,
                    "Não foi possível eliminar o tema.",
                    "Fechar")
                {
                    Owner = this
                };

                DialogHelper.ShowOwnedDialog(errorDialog, this, 0.85);
                return;
            }

            var selectedFolder = combo_folder.SelectedItem as Archive;
            LoadFolders(selectedFolder == null ? null : selectedFolder.code);

            new NotificationWindow("Tema Eliminado", selectedArchive.theme, "Foi eliminado com sucesso").Show();
        }

        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            var addFolderWindow = new AddArchiveWindow { Owner = this };
            DialogHelper.ShowOwnedDialog(addFolderWindow, this, 0.85);

            LoadFolders();
        }

        private void btn_add_theme_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = combo_folder.SelectedItem as Archive;
            if (selectedFolder == null)
                return;

            var addThemeWindow = new AddArchiveWindow(selectedFolder.code) { Owner = this };
            DialogHelper.ShowOwnedDialog(addThemeWindow, this, 0.85);

            LoadFolders(selectedFolder.code);
        }

        private void LoadFolders(string folderCodeToSelect = null)
        {
            var folders = DBConnect.GetFolders();
            combo_folder.ItemsSource = folders;
            combo_folder.DisplayMemberPath = "code";
            combo_folder.SelectedValuePath = "code";

            var folderCode = folderCodeToSelect ?? initialFolderCode;
            if (!string.IsNullOrWhiteSpace(folderCode))
            {
                var folder = folders.FirstOrDefault(item => item.code == folderCode);
                if (folder != null)
                    combo_folder.SelectedItem = folder;
            }

            if (combo_folder.SelectedItem == null && folders.Count > 0)
                combo_folder.SelectedIndex = 0;

            if (combo_folder.SelectedItem == null)
            {
                list_themes.ItemsSource = null;
                lbl_usage.Content = "Não existem pastas configuradas";
                folder_name_box.Text = string.Empty;
                theme_name_box.Text = string.Empty;
                btn_add_theme.IsEnabled = false;
                btn_delete_theme.IsEnabled = false;
                btn_rename_theme.IsEnabled = false;
            }
        }

        private void RefreshThemeList(int? archiveIdToSelect = null)
        {
            var selectedFolder = combo_folder.SelectedItem as Archive;
            if (selectedFolder == null)
            {
                list_themes.ItemsSource = null;
                btn_delete_theme.IsEnabled = false;
                btn_rename_theme.IsEnabled = false;
                return;
            }

            var themes = DBConnect.GetArchiveEntriesByCode(selectedFolder.code);
            list_themes.ItemsSource = themes;
            folder_name_box.Text = selectedFolder.code;

            var archiveId = archiveIdToSelect ?? initialArchiveId;

            if (archiveId != null)
            {
                var archive = themes.FirstOrDefault(item => item.id == archiveId.Value);
                if (archive != null)
                    list_themes.SelectedItem = archive;
            }

            if (list_themes.SelectedItem == null && themes.Count > 0)
                list_themes.SelectedIndex = 0;

            if (list_themes.SelectedItem != null)
            {
                list_themes.UpdateLayout();
                list_themes.ScrollIntoView(list_themes.SelectedItem);
                list_themes.Focus();
            }

            if (themes.Count == 0)
                lbl_usage.Content = "Esta pasta deixou de ter temas";
        }
    }
}

