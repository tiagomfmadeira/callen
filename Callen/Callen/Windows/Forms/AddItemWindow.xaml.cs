using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SQLite;

namespace Callen.Windows.Forms
{
    public partial class AddItemWindow : Window
    {
        private readonly WindowOverlaySync overlaySync;
        private readonly bool duplicated;
        private readonly Calendar calendarToDuplicate;

        // True when at least one insert was successful in this window instance.
        public bool inserted { get; private set; }

        // Fires after each successful insert.
        public event Action OnInserted;

        public AddItemWindow()
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);

            PreviewKeyDown += HandleEsc;

            Loaded += WinAddItem_Loaded;
            Closed += WinAddItem_Closed;

            inserted = false;
            duplicated = false;
        }

        public AddItemWindow(Calendar calendar)
            : this()
        {
            calendarToDuplicate = calendar;
            duplicated = true;
        }

        // ---------- Overlay sizing + initialization ----------

        private void WinAddItem_Loaded(object sender, RoutedEventArgs e)
        {
            overlaySync.Attach();

            // Existing behavior
            FillFolderCombo();

            if (duplicated && calendarToDuplicate != null)
                FillWindow(calendarToDuplicate);
        }

        private void WinAddItem_Closed(object sender, EventArgs e)
        {
            PreviewKeyDown -= HandleEsc;
            overlaySync.Detach();
        }

        // ---------- Close behavior ----------

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        // ---------- Folder/theme combos ----------

        public void FillFolderCombo()
        {
            var folders = DBConnect.GetFolders();

            combo_folder.ItemsSource = folders;
            combo_folder.DisplayMemberPath = "code";
            combo_folder.SelectedValuePath = "code";
        }

        private void FillThemeCombo()
        {
            var folder = combo_folder.SelectedItem as Archive;
            if (folder == null)
            {
                combo_theme.ItemsSource = null;
                combo_theme.SelectedIndex = -1;
                return;
            }

            var folderThemes = DBConnect.GetFoldersThemes(folder.code);

            combo_theme.ItemsSource = folderThemes;
            combo_theme.DisplayMemberPath = "theme";
            combo_theme.SelectedValuePath = "id";
            combo_theme.SelectedIndex = -1;
        }

        private void combo_folder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combo_folder.SelectedItem != null)
            {
                combo_theme.IsEnabled = true;
                btn_manage_archive.IsEnabled = true;
                FillThemeCombo();
            }
            else
            {
                combo_theme.IsEnabled = false;
                combo_theme.ItemsSource = null;
                btn_manage_archive.IsEnabled = false;
            }
        }

        // ---------- Image upload ----------

        public void btn_upload_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog
            {
                Title = "Select a picture",
                Filter =
                    "All supported graphics|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png"
            };

            if (op.ShowDialog() != true) return;

            try
            {
                ImageSource src = new BitmapImage(new Uri(op.FileName));

                img_border.Visibility = Visibility.Visible;
                img.Source = src;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ficheiro " + op.FileName + " não pode ser aberto");
                Debug.WriteLine("File Error: " + ex);
            }
        }

        // ---------- Insert ----------

        private void btn_insert_Click(object sender, RoutedEventArgs e)
        {
            var selectedTheme = (Archive)combo_theme.SelectedItem;

            var calendar = new Calendar
            {
                name = name_box.Text,
                description = desc_box.Text,
                year = year_box.Text,
                matrix = other_box.Text,
                collection = collec_box.Text,
                pic_path = string.Empty,
                archive_id = selectedTheme == null ? 0 : selectedTheme.id,
                code = selectedTheme == null ? null : selectedTheme.code,
                theme = selectedTheme == null ? null : selectedTheme.theme
            };

            var imagePath = App.ImagePath;

            try
            {
                DBConnect.AddCalendar(calendar, img.Source, imagePath);
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Erro ao inserir: " + ex.Message);
                Debug.WriteLine(ex);
                return;
            }

            inserted = true;
            AddInsertedItemToPrintList(calendar);

            new NotificationWindow("New Item", name_box.Text, "Foi inserido com sucesso").Show();

            OnInserted?.Invoke();

            DisableInsertTemporarily();
        }

        private void DisableInsertTemporarily()
        {
            btn_insert.IsEnabled = false;
            TimedAction.ExecuteWithDelay(() => btn_insert.IsEnabled = true, TimeSpan.FromMilliseconds(1500));
        }

        private void btn_manage_archive_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = combo_folder.SelectedItem as Archive;
            var folderCode = selectedFolder == null ? null : selectedFolder.code;
            var selectedTheme = combo_theme.SelectedItem as Archive;
            var archiveId = selectedTheme == null ? (int?)null : selectedTheme.id;

            var manageArchiveWindow = new ManageArchiveWindow(folderCode, archiveId) { Owner = this };
            DialogHelper.ShowOwnedDialog(manageArchiveWindow, this, 0.85);

            RefreshArchiveSelection(
                manageArchiveWindow.SelectedFolderCode ?? folderCode,
                manageArchiveWindow.SelectedArchiveId ?? archiveId);
        }

        // ---------- Duplicate fill ----------

        private void FillWindow(Calendar calen)
        {
            name_box.Text = calen.name;
            year_box.Text = calen.year;
            collec_box.Text = calen.collection;
            other_box.Text = calen.matrix;
            desc_box.Text = calen.description;

            var archiveThemeRow = DBConnect.GetArchiveById(calen.archive_id);
            if (archiveThemeRow != null)
            {
                combo_folder.SelectedValue = archiveThemeRow.code;

                FillThemeCombo();

                combo_theme.SelectedValue = archiveThemeRow.id;
            }

            if (!string.IsNullOrWhiteSpace(calen.pic_path))
            {
                try
                {
                    ImageSource src = new BitmapImage(new Uri(calen.pic_path + ".jpeg", UriKind.RelativeOrAbsolute));

                    img_border.Visibility = Visibility.Visible;
                    img.Source = src;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Image load error: " + ex);
                }
            }
        }

        private void RefreshArchiveSelection(string folderCode, int? archiveId)
        {
            combo_theme.ItemsSource = null;
            combo_theme.SelectedIndex = -1;

            var selectedArchive = archiveId == null ? null : DBConnect.GetArchiveById(archiveId.Value);
            var resolvedFolderCode = selectedArchive == null ? folderCode : selectedArchive.code;

            FillFolderCombo();

            if (!string.IsNullOrWhiteSpace(resolvedFolderCode))
            {
                combo_folder.SelectedValue = resolvedFolderCode;
                FillThemeCombo();
            }

            if (archiveId != null)
                combo_theme.SelectedValue = archiveId.Value;
        }

        private static void AddInsertedItemToPrintList(Calendar calendar)
        {
            PrintListStore.AddIfMissing(calendar);
        }
    }
}


