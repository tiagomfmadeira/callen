using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Callen.Pages
{
    public class SearchFilters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Year { get; set; }
        public string Collection { get; set; }
        public string Folder { get; set; }
        public string Theme { get; set; }
        public string Other { get; set; }
    }

    public partial class Search : UserControl
    {
        private const int SearchPageSize = 250;

        private readonly SearchFilters currentFilters = new SearchFilters();
        private ListSearch listSearchPage;
        private int searchRequestVersion;
        private int loadedOffset;
        private bool hasMoreRows;
        private bool isLoadingMore;

        public Search()
        {
            InitializeComponent();
            EnsureListSearchPage();
            _ = RunSearchAsync();
        }

        private async void btn_adv_search_Click(object sender, RoutedEventArgs e)
        {
            CaptureCurrentFilters();
            await RunSearchAsync();
            advanceSearch.IsChecked = false;
        }

        private async void btn_adv_clear_Click(object sender, RoutedEventArgs e)
        {
            name_box.Text = string.Empty;
            id_box.Text = string.Empty;
            collec_box.Text = string.Empty;
            folder_box.Text = string.Empty;
            year_box.Text = string.Empty;
            other_box.Text = string.Empty;
            theme_box.Text = string.Empty;
            desc_box.Text = string.Empty;

            CaptureCurrentFilters();
            await RunSearchAsync();
        }

        public async void ReloadData()
        {
            CaptureCurrentFilters();
            await RunSearchAsync();
        }

        private void Border_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            btn_adv_search.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            e.Handled = true;
        }

        private void RootSearch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (advanceSearch.IsChecked != true)
                return;

            var source = e.OriginalSource as DependencyObject;
            if (source == null)
                return;

            if (IsDescendantOf(source, advanceSearch) || IsDescendantOf(source, advanceSearchPanel))
                return;

            advanceSearch.IsChecked = false;
        }

        private static bool IsDescendantOf(DependencyObject source, DependencyObject ancestor)
        {
            var current = source;
            while (current != null)
            {
                if (current == ancestor)
                    return true;

                if (current is Visual || current is System.Windows.Media.Media3D.Visual3D)
                {
                    current = VisualTreeHelper.GetParent(current);
                    continue;
                }

                if (current is FrameworkContentElement contentElement)
                {
                    current = contentElement.Parent;
                    continue;
                }

                break;
            }

            return false;
        }

        private Calendar BuildSearchCalendar()
        {
            return new Calendar
            {
                id = currentFilters.Id,
                name = currentFilters.Name,
                description = currentFilters.Description,
                year = currentFilters.Year,
                matrix = currentFilters.Other,
                collection = currentFilters.Collection,
                pic_path = string.Empty,
                code = currentFilters.Folder,
                theme = currentFilters.Theme
            };
        }

        private void CaptureCurrentFilters()
        {
            int id;

            currentFilters.Id = int.TryParse(id_box.Text, out id) ? id : 0;
            currentFilters.Name = name_box.Text;
            currentFilters.Description = desc_box.Text;
            currentFilters.Year = year_box.Text;
            currentFilters.Collection = collec_box.Text;
            currentFilters.Folder = folder_box.Text;
            currentFilters.Theme = theme_box.Text;
            currentFilters.Other = other_box.Text;
        }

        private async Task RunSearchAsync()
        {
            var requestVersion = ++searchRequestVersion;
            SetSearchBusy(true);
            isLoadingMore = true;
            loadedOffset = 0;
            hasMoreRows = false;

            try
            {
                var filter = BuildSearchCalendar();
                var data = await Task.Run(() => DBConnect.SearchCalendarPaged(filter, SearchPageSize, 0));

                if (requestVersion != searchRequestVersion || data == null)
                    return;

                EnsureListSearchPage();
                listSearchPage.ApplyData(data);
                loadedOffset = data.Rows.Count;
                hasMoreRows = data.Rows.Count == SearchPageSize;
            }
            finally
            {
                isLoadingMore = false;
                if (requestVersion == searchRequestVersion)
                    SetSearchBusy(false);
            }
        }

        private void EnsureListSearchPage()
        {
            if (listSearchPage != null)
                return;

            listSearchPage = new ListSearch(new DataTable());
            listSearchPage.LoadMoreRequested += ListSearchPage_LoadMoreRequested;
            listSearchPage.ArchiveStructureChangedRequested += ListSearchPage_ArchiveStructureChangedRequested;
            Switcher.Switch(Search_mode, listSearchPage);
        }

        private void ListSearchPage_ArchiveStructureChangedRequested(object sender, EventArgs e)
        {
            ReloadData();
        }

        private async void ListSearchPage_LoadMoreRequested(object sender, EventArgs e)
        {
            if (isLoadingMore || !hasMoreRows)
            {
                if (listSearchPage != null)
                    listSearchPage.CompleteLoadMoreRequest();
                return;
            }

            isLoadingMore = true;
            var requestVersion = searchRequestVersion;

            try
            {
                var filter = BuildSearchCalendar();
                var nextPage = await Task.Run(() => DBConnect.SearchCalendarPaged(filter, SearchPageSize, loadedOffset));

                if (requestVersion != searchRequestVersion || nextPage == null || nextPage.Rows.Count == 0)
                {
                    hasMoreRows = false;
                    if (listSearchPage != null)
                        listSearchPage.CompleteLoadMoreRequest();
                    return;
                }

                loadedOffset += nextPage.Rows.Count;
                hasMoreRows = nextPage.Rows.Count == SearchPageSize;

                listSearchPage.AppendData(nextPage);
            }
            finally
            {
                isLoadingMore = false;
                if (listSearchPage != null)
                    listSearchPage.CompleteLoadMoreRequest();
            }
        }

        private void SetSearchBusy(bool isBusy)
        {
            btn_adv_search.IsEnabled = !isBusy;
            btn_adv_clear.IsEnabled = !isBusy;
            Cursor = isBusy ? Cursors.Wait : null;
        }
    }
}

