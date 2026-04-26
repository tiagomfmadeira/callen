using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        public bool HasArchiveStructureChanges { get; private set; }

        public ManageArchiveWindow()
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);

            PreviewKeyDown += HandleEsc;
            SourceInitialized += WinManageArchive_SourceInitialized;
            Loaded += WinManageArchive_Loaded;
            Closed += WinManageArchive_Closed;
        }

        public ManageArchiveWindow(string folderCode, int? archiveId = null)
            : this()
        {
            initialFolderCode = folderCode;
            initialArchiveId = archiveId;
        }

        private void WinManageArchive_SourceInitialized(object sender, EventArgs e)
        {
            overlaySync.Attach();
        }

        private void WinManageArchive_Loaded(object sender, RoutedEventArgs e)
        {
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
            btn_split_folder.IsEnabled = combo_folder.SelectedItem != null;
            SelectedFolderCode = (combo_folder.SelectedItem as Archive)?.code;
            RefreshThemeList(initialArchiveId);
        }

        private void list_themes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedArchive = list_themes.SelectedItem as Archive;
            if (selectedArchive == null)
            {
                SelectedArchiveId = null;
                lbl_usage.Content = Loc.T("ManageArchive.SelectTheme");
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
                ? Loc.F("ManageArchive.ThemeInUseCount", selectedArchive.usage_count)
                : Loc.T("ManageArchive.ThemeUnused");
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
            HasArchiveStructureChanges = true;

            new NotificationWindow(Loc.T("Noti.FolderUpdatedTitle"), oldFolderCode, Loc.T("Noti.UpdatedSuccess")).Show();
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
            HasArchiveStructureChanges = true;

            new NotificationWindow(Loc.T("Noti.ThemeUpdatedTitle"), oldThemeName, Loc.T("Noti.UpdatedSuccess")).Show();
        }

        private void btn_delete_theme_Click(object sender, RoutedEventArgs e)
        {
            var selectedArchive = list_themes.SelectedItem as Archive;
            if (selectedArchive == null)
                return;

            if (selectedArchive.usage_count > 0)
            {
                var usageDialog = new ActionDialogWindow(
                    Loc.T("Dlg.ThemeInUseTitle"),
                    selectedArchive.theme,
                    Loc.T("Dlg.ThemeInUseMessage"),
                    Loc.T("Dlg.Close"));

                DialogHelper.ShowOwnedDialog(usageDialog, this);
                return;
            }

            var deleteDialog = new ActionDialogWindow(
                Loc.T("Dlg.DeleteThemeTitle"),
                selectedArchive.theme,
                Loc.T("Dlg.DeleteThemeMessage"),
                Loc.T("Dlg.Delete"),
                Loc.T("Dlg.Cancel"));

            DialogHelper.ShowOwnedDialog(deleteDialog, this);

            if (deleteDialog.SelectedAction != DialogAction.Primary)
                return;

            if (!DBConnect.DeleteArchiveEntry(selectedArchive.id))
            {
                var errorDialog = new ActionDialogWindow(
                    Loc.T("Dlg.DeleteThemeErrorTitle"),
                    selectedArchive.theme,
                    Loc.T("Dlg.DeleteThemeErrorMessage"),
                    Loc.T("Dlg.Close"));

                DialogHelper.ShowOwnedDialog(errorDialog, this);
                return;
            }

            var selectedFolder = combo_folder.SelectedItem as Archive;
            LoadFolders(selectedFolder == null ? null : selectedFolder.code);
            HasArchiveStructureChanges = true;

            new NotificationWindow(Loc.T("Noti.ThemeDeletedTitle"), selectedArchive.theme, Loc.T("Noti.DeletedSuccess")).Show();
        }

        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            var addFolderWindow = new AddArchiveWindow();
            DialogHelper.ShowOwnedDialog(addFolderWindow, this);

            LoadFolders();
            HasArchiveStructureChanges = true;
        }

        private void btn_add_theme_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = combo_folder.SelectedItem as Archive;
            if (selectedFolder == null)
                return;

            var addThemeWindow = new AddArchiveWindow(selectedFolder.code);
            DialogHelper.ShowOwnedDialog(addThemeWindow, this);

            LoadFolders(selectedFolder.code);
            HasArchiveStructureChanges = true;
        }

        private void btn_split_folder_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = combo_folder.SelectedItem as Archive;
            if (selectedFolder == null)
                return;

            SuggestSplit(selectedFolder.code, out var sourceRange, out var suggestedRangeA, out var suggestedRangeB, out var suggestedCodeA, out var suggestedCodeB);

            var splitWindow = new SplitArchiveWindow(
                selectedFolder.code,
                sourceRange,
                suggestedCodeA,
                suggestedRangeA,
                suggestedCodeB,
                suggestedRangeB);

            DialogHelper.ShowOwnedDialog(splitWindow, this);
            if (splitWindow.DialogResult != true)
                return;

            var confirmDialog = new ActionDialogWindow(
                Loc.T("ManageArchive.SplitConfirmTitle"),
                selectedFolder.code,
                Loc.F(
                    "ManageArchive.SplitConfirmMessage",
                    splitWindow.TargetCodeA,
                    splitWindow.TargetRangeA,
                    splitWindow.TargetCodeB,
                    splitWindow.TargetRangeB),
                Loc.T("ManageArchive.SplitConfirm"),
                Loc.T("Dlg.Cancel"));

            DialogHelper.ShowOwnedDialog(confirmDialog, this);
            if (confirmDialog.SelectedAction != DialogAction.Primary)
                return;

            try
            {
                var result = DBConnect.SplitFolder(new FolderSplitRequest
                {
                    SourceCode = selectedFolder.code,
                    TargetCodeA = splitWindow.TargetCodeA,
                    TargetRangeA = splitWindow.TargetRangeA,
                    TargetCodeB = splitWindow.TargetCodeB,
                    TargetRangeB = splitWindow.TargetRangeB
                });

                LoadFolders(splitWindow.TargetCodeA);
                HasArchiveStructureChanges = true;

                var details = Loc.F(
                    "ManageArchive.SplitSummaryCounts",
                    result.TotalCalendarsMoved,
                    result.CalendarsMovedToA,
                    result.CalendarsMovedToB);

                if (result.UnmatchedMovedToA > 0)
                {
                    details += Environment.NewLine + Environment.NewLine
                        + Loc.F("ManageArchive.SplitSummaryUnmatched", result.UnmatchedMovedToA, splitWindow.TargetCodeA);

                    if (result.WarningSamples.Count > 0)
                        details += Environment.NewLine + Loc.F("ManageArchive.SplitSummaryExamples", string.Join(", ", result.WarningSamples));
                }

                var summaryDialog = new ActionDialogWindow(
                    Loc.T("ManageArchive.SplitSummaryTitle"),
                    Loc.F("ManageArchive.SplitSummaryContext", splitWindow.TargetCodeA, splitWindow.TargetCodeB),
                    details,
                    Loc.T("Dlg.Close"));

                DialogHelper.ShowOwnedDialog(summaryDialog, this);
            }
            catch (Exception ex)
            {
                var errorDialog = new ActionDialogWindow(
                    Loc.T("ManageArchive.SplitErrorTitle"),
                    selectedFolder.code,
                    ex.Message,
                    Loc.T("Dlg.Close"));

                DialogHelper.ShowOwnedDialog(errorDialog, this);
            }
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
                lbl_usage.Content = Loc.T("ManageArchive.NoFolders");
                folder_name_box.Text = string.Empty;
                theme_name_box.Text = string.Empty;
                btn_add_theme.IsEnabled = false;
                btn_split_folder.IsEnabled = false;
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
                lbl_usage.Content = Loc.T("ManageArchive.NoThemesInFolder");
        }

        private static void SuggestSplit(
            string sourceCode,
            out string sourceRange,
            out string suggestedRangeA,
            out string suggestedRangeB,
            out string suggestedCodeA,
            out string suggestedCodeB)
        {
            var suggestion = DBConnect.GetFolderSplitSuggestion(sourceCode);
            sourceRange = suggestion.SourceRange;
            suggestedRangeA = suggestion.SuggestedRangeA;
            suggestedRangeB = suggestion.SuggestedRangeB;

            var baseCode = Regex.Replace(sourceCode ?? string.Empty, @"\s*\([^)]*/[^)]*\)\s*$", string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(baseCode))
                baseCode = sourceCode ?? string.Empty;

            suggestedCodeA = baseCode + " A";
            suggestedCodeB = baseCode + " B";
        }
    }
}


