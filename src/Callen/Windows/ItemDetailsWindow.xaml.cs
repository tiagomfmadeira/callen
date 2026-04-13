using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Callen.Windows.Forms;
using Callen.Windows.Other;

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for ItemDetailsWindow.xaml
    /// </summary>
    public partial class ItemDetailsWindow : Window
    {
        private readonly WindowOverlaySync overlaySync;
        private bool suppressCloseConfirmation;
        private Calendar cal;
        private readonly bool previewImage;
        private readonly List<int> navigationCalendarIds;
        private int navigationIndex;

        // Event to notify when the item is updated
        public event Action<Calendar> OnItemUpdated;

        // Event to notify when the item is deleted
        public event Action<int> OnItemDeleted;

        // Event to notify when a new item is inserted (e.g. via duplicate)
        public event Action OnItemInserted;

        public ItemDetailsWindow(Calendar calen, bool preview)
            : this(calen, preview, null)
        {
        }

        public ItemDetailsWindow(Calendar calen, bool preview, List<int> calendarIds)
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);

            PreviewKeyDown += HandleEsc;

            // Make this overlay window match the owner/main window so closeBorder is truly full coverage
            Loaded += WinDesc_Loaded;
            Closed += WinDesc_Closed;
            Closing += WinDesc_Closing;

            previewImage = preview;
            navigationCalendarIds = calendarIds ?? new List<int>();

            cal = calen;
            navigationIndex = navigationCalendarIds.IndexOf(cal.id);

            FillFolderCombo();
            LoadCalendarValues();

            LoadPreviewImage();
            UpdateNavigationButtons();
        }

        // ---------- Overlay sizing (KISS) ----------

        private void WinDesc_Loaded(object sender, RoutedEventArgs e)
        {
            overlaySync.Attach();
            PositionCardAdjacents();
            SizeChanged += WinDesc_SizeChanged;
        }

        private void WinDesc_Closed(object sender, EventArgs e)
        {
            PreviewKeyDown -= HandleEsc;
            Closing -= WinDesc_Closing;
            SizeChanged -= WinDesc_SizeChanged;
            overlaySync.Detach();
        }

        private void WinDesc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PositionCardAdjacents();
        }

        // ---------- Close behavior ----------

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Left)
            {
                NavigateToOffset(-1);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Right)
            {
                NavigateToOffset(1);
                e.Handled = true;
            }
        }

        public void btn_close_Click(object sender, RoutedEventArgs e) => Close();

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Close();

        // ---------- Image ----------

        private void btn_img_Click(object sender, RoutedEventArgs e)
        {
            var popZoomImg = new ImageZoomWindow(img.Source) { Owner = this };
            popZoomImg.ShowDialog();
        }

        // ---------- Edit mode helpers ----------

        private bool IsEditMode => item_desc.IsEnabled;

        private bool HasUnsavedChanges()
        {
            if (!IsEditMode)
                return false;

            var selectedArchive = combo_theme.SelectedItem as Archive;
            var selectedArchiveId = selectedArchive == null ? 0 : selectedArchive.id;

            return !TextMatches(item_name.Text, cal.name)
                   || !TextMatches(item_year.Text, cal.year)
                   || !TextMatches(item_collec.Text, cal.collection)
                   || !TextMatches(item_other.Text, cal.matrix)
                   || !TextMatches(item_desc.Text, cal.description)
                   || selectedArchiveId != cal.archive_id;
        }

        private static bool TextMatches(string currentValue, string originalValue)
        {
            return string.Equals(currentValue ?? string.Empty, originalValue ?? string.Empty, StringComparison.Ordinal);
        }

        private void LoadCalendarValues()
        {
            item_name.Text = cal.name ?? string.Empty;
            item_year.Text = cal.year ?? string.Empty;
            item_folder.Text = cal.code ?? string.Empty;
            item_theme.Text = cal.theme ?? string.Empty;
            item_desc.Text = cal.description ?? string.Empty;
            item_other.Text = cal.matrix ?? string.Empty;
            item_collec.Text = cal.collection ?? string.Empty;
        }

        private void LoadPreviewImage()
        {
            if (!previewImage)
            {
                img_border.Visibility = Visibility.Hidden;
                img.Source = null;
                return;
            }

            img_border.Visibility = Visibility.Visible;

            try
            {
                img.Source = new BitmapImage(new Uri(cal.pic_path + ".jpeg", UriKind.RelativeOrAbsolute));
            }
            catch
            {
                img.Source = null;
                MessageBox.Show("File not found: " + cal.pic_path);
            }
        }

        private bool ConfirmPendingChanges()
        {
            if (!HasUnsavedChanges())
                return true;

            var saveChangesDialog = new ActionDialogWindow(
                "Alterações por guardar",
                cal.id + " - " + cal.name,
                "Pretende guardar as alterações antes de continuar?",
                "Guardar",
                "Descartar",
                "Cancelar")
            {
                Owner = this
            };

            DialogHelper.ShowOwnedDialog(saveChangesDialog, this, 0.85);

            if (saveChangesDialog.SelectedAction == DialogAction.Primary)
            {
                if (!updateInfo())
                    return false;

                return true;
            }

            return saveChangesDialog.SelectedAction == DialogAction.Secondary;
        }

        private void EnterEditMode()
        {
            btn_save.IsEnabled = true;
            btn_manage_archive.IsEnabled = true;

            item_name.IsEnabled = true;
            item_year.IsEnabled = true;
            item_collec.IsEnabled = true;
            item_other.IsEnabled = true;
            item_desc.IsEnabled = true;

            item_folder.Visibility = Visibility.Hidden;
            item_theme.Visibility = Visibility.Hidden;

            combo_folder.Visibility = Visibility.Visible;
            combo_theme.Visibility = Visibility.Visible;

            // Ensure theme combo is loaded for selected folder
            if (combo_folder.SelectedItem != null)
                FillThemeComboForSelectedFolder(combo_folder.SelectedValue.ToString());
        }

        private void ExitEditMode()
        {
            btn_save.IsEnabled = false;
            btn_manage_archive.IsEnabled = false;

            item_name.IsEnabled = false;
            item_year.IsEnabled = false;
            item_collec.IsEnabled = false;
            item_other.IsEnabled = false;
            item_desc.IsEnabled = false;

            item_folder.Visibility = Visibility.Visible;
            item_theme.Visibility = Visibility.Visible;

            combo_folder.Visibility = Visibility.Hidden;
            combo_theme.Visibility = Visibility.Hidden;

            LoadCalendarValues();
        }

        // ---------- Edit toggle ----------

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditMode)
            {
                if (!ConfirmPendingChanges())
                    return;

                ExitEditMode();
            }
            else
            {
                EnterEditMode();
            }
        }

        // ---------- Save (Save & exit edit mode) ----------

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEditMode)
                return;

            if (!updateInfo())
                return;

            ExitEditMode();
        }

        // ---------- Update ----------

        private bool updateInfo()
        {
            var selectedArchive = combo_theme.SelectedItem as Archive;

            var updatedCalendar = new Calendar
            {
                id = cal.id,
                name = item_name.Text,
                description = item_desc.Text,
                year = item_year.Text,
                matrix = item_other.Text,
                collection = item_collec.Text,
                pic_path = cal.pic_path,
                archive_id = selectedArchive == null ? 0 : selectedArchive.id,
                code = selectedArchive == null ? null : selectedArchive.code,
                theme = selectedArchive == null ? null : selectedArchive.theme
            };

            if (!DBConnect.UpdateCalendarInfo(updatedCalendar))
                return false;

            item_folder.Text = updatedCalendar.code ?? string.Empty;
            item_theme.Text = updatedCalendar.theme ?? string.Empty;

            UpdateLocalInfo();

            new NotificationWindow("Update Item", cal.id + " - " + item_name.Text, "Foi modificado com sucesso").Show();

            OnItemUpdated?.Invoke(updatedCalendar);
            return true;
        }

        private void UpdateLocalInfo()
        {
            var selectedArchive = combo_theme.SelectedItem as Archive;

            cal = new Calendar
            {
                id = cal.id,
                name = item_name.Text,
                description = item_desc.Text,
                year = item_year.Text,
                matrix = item_other.Text,
                collection = item_collec.Text,
                pic_path = cal.pic_path,
                archive_id = selectedArchive == null ? 0 : selectedArchive.id,
                code = selectedArchive == null ? null : selectedArchive.code,
                theme = selectedArchive == null ? null : selectedArchive.theme
            };
        }

        private void UpdateNavigationButtons()
        {
            var canNavigate = navigationCalendarIds != null && navigationCalendarIds.Count > 1 && navigationIndex >= 0;
            btn_prev_item.IsEnabled = canNavigate && navigationIndex > 0;
            btn_next_item.IsEnabled = canNavigate && navigationIndex < navigationCalendarIds.Count - 1;
        }

        private bool NavigateToOffset(int offset)
        {
            if (navigationCalendarIds == null || navigationCalendarIds.Count == 0 || navigationIndex < 0)
                return false;

            var targetIndex = navigationIndex + offset;
            if (targetIndex < 0 || targetIndex >= navigationCalendarIds.Count)
                return false;

            if (IsEditMode)
            {
                if (!ConfirmPendingChanges())
                    return false;

                ExitEditMode();
            }

            var targetId = navigationCalendarIds[targetIndex];
            var nextCalendar = DBConnect.GetCalendarInfo(targetId.ToString());
            if (nextCalendar == null)
                return false;

            cal = nextCalendar;
            navigationIndex = targetIndex;

            FillFolderCombo();
            LoadCalendarValues();
            LoadPreviewImage();
            UpdateNavigationButtons();
            return true;
        }

        // ---------- Print ----------

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            if (PrintListStore.AddIfMissing(cal))
            {
                new NotificationWindow("Etiquetas para imprimir", cal.id + " - " + cal.name,
                    "Foi adicionado com sucesso à lista para imprimir").Show();
            }
        }

        private void btn_prev_item_Click(object sender, RoutedEventArgs e)
        {
            NavigateToOffset(-1);
        }

        private void btn_next_item_Click(object sender, RoutedEventArgs e)
        {
            NavigateToOffset(1);
        }

        // ---------- Folder / theme combos ----------

        private void FillFolderCombo()
        {
            List<Archive> folders = DBConnect.GetFolders();

            combo_folder.ItemsSource = folders;
            combo_folder.DisplayMemberPath = "code";
            combo_folder.SelectedValuePath = "code";

            foreach (Archive folder in combo_folder.Items)
            {
                if (folder.code == cal.code)
                {
                    combo_folder.SelectedItem = folder;
                    break;
                }
            }

            if (combo_folder.SelectedItem != null)
                FillThemeComboForSelectedFolder(combo_folder.SelectedValue.ToString());
        }

        private void FillThemeComboForSelectedFolder(string folderCode)
        {
            List<Archive> folders_themes = DBConnect.GetFoldersThemes(folderCode);

            combo_theme.ItemsSource = folders_themes;
            combo_theme.DisplayMemberPath = "theme";
            combo_theme.SelectedValuePath = "id";

            if (combo_theme.Items.Count > 0)
                combo_theme.SelectedIndex = 0;

            foreach (Archive t in combo_theme.Items)
            {
                if (t.theme == cal.theme)
                {
                    combo_theme.SelectedItem = t;
                    break;
                }
            }

            if (combo_theme.SelectedItem == null && combo_theme.Items.Count == 0)
                combo_theme.SelectedIndex = -1;
        }

        private void combo_folder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combo_folder.SelectedItem == null)
            {
                combo_theme.ItemsSource = null;
                combo_theme.SelectedIndex = -1;
                return;
            }

            FillThemeComboForSelectedFolder(combo_folder.SelectedValue.ToString());
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
                FillThemeComboForSelectedFolder(resolvedFolderCode);
            }

            if (archiveId != null)
                combo_theme.SelectedValue = archiveId.Value;
        }

        // ---------- Delete ----------

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            var deleteDialog = new ActionDialogWindow(
                "Eliminar item",
                cal.id + " - " + cal.name,
                "Tem a certeza que pretende eliminar este item?",
                "Eliminar",
                "Cancelar")
            {
                Owner = this
            };

            DialogHelper.ShowOwnedDialog(deleteDialog, this, 0.85);

            if (deleteDialog.SelectedAction != DialogAction.Primary)
                return;

            DBConnect.RemoveCalendar(cal.id);

            OnItemDeleted?.Invoke(cal.id);

            new NotificationWindow("Item Eliminado", cal.id + " - " + cal.name, "Foi eliminado com sucesso").Show();

            suppressCloseConfirmation = true;
            Close();
        }

        // ---------- Duplicate ----------

        private void btn_duplicate_Click(object sender, RoutedEventArgs e)
        {
            var popDup = new AddItemWindow(cal) { Owner = this };

            popDup.OnInserted += () =>
            {
                // Parent window should handle scroll-to-bottom after reload.
                OnItemInserted?.Invoke();
            };

            DialogHelper.ShowOwnedDialog(popDup, this, 0.85);
        }

        // ---------- Popups ----------

        private static double ResolveElementWidth(FrameworkElement element)
        {
            if (element.ActualWidth > 0)
                return element.ActualWidth;

            return double.IsNaN(element.Width) ? 0 : element.Width;
        }

        private static double ResolveElementHeight(FrameworkElement element)
        {
            if (element.ActualHeight > 0)
                return element.ActualHeight;

            return double.IsNaN(element.Height) ? 0 : element.Height;
        }

        private static void PositionTooltip(Border tooltip, FrameworkElement anchor)
        {
            var tooltipParent = tooltip.Parent as UIElement;
            if (tooltipParent == null)
                return;

            var anchorWidth = ResolveElementWidth(anchor);
            var anchorTopLeft = anchor.TranslatePoint(new Point(0, 0), tooltipParent);
            var tooltipLeft = anchorTopLeft.X + (anchorWidth / 2.0) - tooltip.Width;
            var tooltipTop = anchorTopLeft.Y - ResolveElementHeight(tooltip) - 3;

            Canvas.SetLeft(tooltip, tooltipLeft);
            Canvas.SetTop(tooltip, tooltipTop);
        }

        private static void ShowActionTooltip(Border tooltip, TranslateTransform translate, FrameworkElement anchor)
        {
            PositionTooltip(tooltip, anchor);

            tooltip.Visibility = Visibility.Visible;
            tooltip.Opacity = 0;
            translate.Y = 6;

            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(140));
            var popUp = new DoubleAnimation(6, 0, TimeSpan.FromMilliseconds(140));

            tooltip.BeginAnimation(OpacityProperty, fadeIn);
            translate.BeginAnimation(TranslateTransform.YProperty, popUp);
        }

        private static void HideActionTooltip(Border tooltip, TranslateTransform translate)
        {
            tooltip.Visibility = Visibility.Collapsed;
            tooltip.BeginAnimation(OpacityProperty, null);
            translate.BeginAnimation(TranslateTransform.YProperty, null);
        }

        private void PositionCardAdjacents()
        {
            if (overlayCanvas == null || detailsCard == null)
                return;

            var cardTopLeft = detailsCard.TranslatePoint(new Point(0, 0), overlayCanvas);
            var cardLeft = cardTopLeft.X;
            var cardTop = cardTopLeft.Y;
            var cardWidth = ResolveElementWidth(detailsCard);
            var cardHeight = ResolveElementHeight(detailsCard);

            const double imageToCardGap = 2.0;
            var imageWidth = ResolveElementWidth(img_border);
            var imageHeight = ResolveElementHeight(img_border);
            Canvas.SetLeft(img_border, cardLeft + ((cardWidth - imageWidth) / 2.0));
            Canvas.SetTop(img_border, cardTop - imageHeight - imageToCardGap);

            var navTop = cardTop + ((cardHeight - ResolveElementHeight(btn_prev_item)) / 2.0);
            Canvas.SetTop(btn_prev_item, navTop);
            Canvas.SetTop(btn_next_item, navTop);

            Canvas.SetLeft(btn_prev_item, Math.Max(0, cardLeft - ResolveElementWidth(btn_prev_item) - 5));
            Canvas.SetLeft(btn_next_item, cardLeft + cardWidth + 5);
        }

        private void btn_print_MouseEnter(object sender, MouseEventArgs e) => ShowActionTooltip(pop_print, pop_print_translate, btn_print);
        private void btn_print_MouseLeave(object sender, MouseEventArgs e) => HideActionTooltip(pop_print, pop_print_translate);

        private void btn_edit_MouseEnter(object sender, MouseEventArgs e) => ShowActionTooltip(pop_edit, pop_edit_translate, btn_edit);
        private void btn_edit_MouseLeave(object sender, MouseEventArgs e) => HideActionTooltip(pop_edit, pop_edit_translate);

        private void btn_duplicate_MouseEnter(object sender, MouseEventArgs e) => ShowActionTooltip(pop_dup, pop_dup_translate, btn_duplicate);
        private void btn_duplicate_MouseLeave(object sender, MouseEventArgs e) => HideActionTooltip(pop_dup, pop_dup_translate);

        private void btn_delete_MouseEnter(object sender, MouseEventArgs e) => ShowActionTooltip(pop_delete, pop_delete_translate, btn_delete);
        private void btn_delete_MouseLeave(object sender, MouseEventArgs e) => HideActionTooltip(pop_delete, pop_delete_translate);

        private void btn_save_MouseEnter(object sender, MouseEventArgs e) => ShowActionTooltip(pop_save, pop_save_translate, btn_save);
        private void btn_save_MouseLeave(object sender, MouseEventArgs e) => HideActionTooltip(pop_save, pop_save_translate);

        private void WinDesc_Closing(object sender, CancelEventArgs e)
        {
            if (suppressCloseConfirmation || !HasUnsavedChanges())
                return;

            e.Cancel = true;

            if (!ConfirmPendingChanges())
                return;

            suppressCloseConfirmation = true;
            Dispatcher.BeginInvoke(new Action(Close), DispatcherPriority.Background);
        }
    }
}
