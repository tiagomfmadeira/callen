using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Callen.Windows;

namespace Callen.Pages
{
    public partial class ListSearch : UserControl
    {
        public event EventHandler LoadMoreRequested;
        public event EventHandler ArchiveStructureChangedRequested;

        private ScrollViewer gridScrollViewer;
        private bool isRequestingMore;

        public ListSearch()
        {
            InitializeComponent();
            PreviewKeyDown += HandleEnter;
            Loaded += ListSearch_Loaded;

            ReloadData();
            ScrollToLastRowSafe();
        }

        public ListSearch(DataTable data)
        {
            InitializeComponent();
            PreviewKeyDown += HandleEnter;
            Loaded += ListSearch_Loaded;

            grdColec.ItemsSource = data == null ? null : data.DefaultView;
        }

        public void ApplyData(DataTable data)
        {
            // Guard against ScrollChanged load-more events while replacing the result set.
            isRequestingMore = true;
            grdColec.ItemsSource = data == null ? null : data.DefaultView;

            // Fresh searches must start at the top to avoid accidental pagination cascades.
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (gridScrollViewer != null)
                    gridScrollViewer.ScrollToTop();
                isRequestingMore = false;
            }), DispatcherPriority.Background);
        }

        public void AppendData(DataTable data)
        {
            if (data == null || data.Rows.Count == 0)
            {
                isRequestingMore = false;
                return;
            }

            var currentView = grdColec.ItemsSource as DataView;
            if (currentView == null)
            {
                grdColec.ItemsSource = data.DefaultView;
                isRequestingMore = false;
                return;
            }

            currentView.Table.Merge(data, false, MissingSchemaAction.Ignore);
            grdColec.Items.Refresh();
            isRequestingMore = false;
        }

        public void CompleteLoadMoreRequest()
        {
            isRequestingMore = false;
        }

        public void ReloadData()
        {
            var selectedId = GetSelectedId();

            grdColec.ItemsSource = DBConnect.GetItemsInfo();
            RestoreSelectionById(selectedId);
        }

        public void ScrollToLastRowSafe()
        {
            if (grdColec.Items == null || grdColec.Items.Count == 0)
                return;

            grdColec.ScrollIntoView(grdColec.Items[grdColec.Items.Count - 1]);
        }

        public void RemoveDeletedItem(int deletedItemId)
        {
            var rows = grdColec.Items.Cast<object>().OfType<DataRowView>();
            var rowToDelete = rows.FirstOrDefault(row => row["ID"].ToString() == deletedItemId.ToString());
            if (rowToDelete == null)
                return;

            var view = grdColec.ItemsSource as DataView;
            if (view == null)
                return;

            view.Table.Rows.Remove(rowToDelete.Row);
        }

        private void HandleEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OpenSelectedCalendar();
                e.Handled = true;
            }
            else if (e.Key == Key.P)
            {
                AddSelectedRowsToPrintList();
                e.Handled = true;
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenSelectedCalendar();
        }

        private void AddSelectedRowsToPrintList()
        {
            if (grdColec.SelectedItems.Count == 0)
            {
                new NotificationWindow(Loc.T("Noti.PrintLabels"), Loc.T("Noti.ZeroAdded"), Loc.T("Noti.NoneSelected")).Show();
                return;
            }

            var addedItemCount = 0;

            foreach (DataRowView item in grdColec.SelectedItems)
            {
                var calendar = DBConnect.GetCalendarInfo(item["ID"].ToString());
                if (PrintListStore.AddIfMissing(calendar))
                {
                    addedItemCount++;
                }
            }

            new NotificationWindow(
                Loc.T("Noti.PrintLabels"),
                Loc.F("Noti.NewItemsCount", addedItemCount),
                Loc.T("Noti.AddedToPrintList")).Show();
        }

        private int? GetSelectedId()
        {
            var selected = grdColec.SelectedItem as DataRowView;
            if (selected == null)
                return null;

            int id;
            return int.TryParse(selected["ID"].ToString(), out id) ? (int?)id : null;
        }

        private void RestoreSelectionById(int? id)
        {
            if (id == null)
                return;

            var view = grdColec.ItemsSource as DataView;
            if (view == null)
                return;

            foreach (DataRowView row in view)
            {
                if (row["ID"].ToString() == id.Value.ToString())
                {
                    grdColec.SelectedItem = row;
                    grdColec.ScrollIntoView(row);
                    break;
                }
            }
        }

        private void OpenSelectedCalendar()
        {
            var selected = grdColec.SelectedItem as DataRowView;
            if (selected == null)
                return;

            var calendar = DBConnect.GetCalendarInfo(selected["ID"].ToString());
            if (calendar != null)
                OpenDescriptionWindow(calendar);
        }

        private void OpenDescriptionWindow(Calendar calendar)
        {
            var navigationIds = GetCurrentCalendarIds();
            var descriptionWindow = string.IsNullOrEmpty(calendar.pic_path)
                ? new ItemDetailsWindow(calendar, false, navigationIds)
                : new ItemDetailsWindow(calendar, true, navigationIds);

            descriptionWindow.OnItemUpdated += UpdateDataGrid;
            descriptionWindow.OnItemDeleted += RemoveDeletedItem;
            descriptionWindow.OnItemInserted += delegate
            {
                ReloadData();
                ScrollToLastRowSafe();
            };
            descriptionWindow.OnCalendarNavigated += SelectCalendarRowById;
            descriptionWindow.OnArchiveStructureChanged += () =>
                ArchiveStructureChangedRequested?.Invoke(this, EventArgs.Empty);

            DialogHelper.ShowOwnedDialog(descriptionWindow, DialogHelper.GetActiveMainWindow());
        }

        private void SelectCalendarRowById(int calendarId)
        {
            RestoreSelectionById(calendarId);
        }

        private List<int> GetCurrentCalendarIds()
        {
            var ids = new List<int>();
            foreach (var item in grdColec.Items)
            {
                var row = item as DataRowView;
                if (row == null)
                    continue;

                int id;
                if (int.TryParse(row["ID"].ToString(), out id))
                    ids.Add(id);
            }

            return ids;
        }

        private void UpdateDataGrid(Calendar updatedCalendar)
        {
            var selectedItem = grdColec.SelectedItem as DataRowView;
            if (selectedItem == null)
                return;

            selectedItem["Name"] = updatedCalendar.name;
            selectedItem["Description"] = updatedCalendar.description;
            selectedItem["Year"] = updatedCalendar.year;
            selectedItem["Theme"] = updatedCalendar.theme;
            selectedItem["Code"] = updatedCalendar.code;
            selectedItem["Matrix"] = updatedCalendar.matrix;
            selectedItem["Collection"] = updatedCalendar.collection;
        }

        private void ListSearch_Loaded(object sender, RoutedEventArgs e)
        {
            if (gridScrollViewer != null)
                return;

            gridScrollViewer = FindChildScrollViewer(grdColec);
            if (gridScrollViewer != null)
                gridScrollViewer.ScrollChanged += GridScrollViewer_ScrollChanged;
        }

        private void GridScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (isRequestingMore || LoadMoreRequested == null)
                return;

            if (e.VerticalChange <= 0 && e.ExtentHeightChange == 0)
                return;

            var nearBottom = e.VerticalOffset + e.ViewportHeight >= e.ExtentHeight - 6;
            if (!nearBottom)
                return;

            isRequestingMore = true;
            LoadMoreRequested.Invoke(this, EventArgs.Empty);
        }

        private static ScrollViewer FindChildScrollViewer(DependencyObject parent)
        {
            if (parent == null)
                return null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                var childScroll = child as ScrollViewer;
                if (childScroll != null)
                    return childScroll;

                var nested = FindChildScrollViewer(child);
                if (nested != null)
                    return nested;
            }

            return null;
        }
    }
}

