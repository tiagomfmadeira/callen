using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Callen.Infrastructure.Printing;
using Window = System.Windows.Window;

namespace Callen.Windows
{
    /// <summary>
    ///     Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : Window
    {
        private const int ItemsPerPage = LabelPrintDocumentBuilder.ItemsPerPage;
        private static readonly double TagCardHeight = (double)new LengthConverter().ConvertFrom("2cm");
        private static readonly Thickness TagCardMargin = new Thickness(0.5, 0, 0.5, 1);
        private readonly WindowOverlaySync overlaySync;
        private int currentPage = 1;
        private int totalPages = 1;
        private int selectedCalendarId = -1;

        public PrintWindow()
        {
            InitializeComponent();
            overlaySync = new WindowOverlaySync(this);

            PreviewKeyDown += HandleEsc;
            SourceInitialized += WinPrint_SourceInitialized;
            Closed += WinPrint_Closed;

            RefreshTagItems();
        }

        private void WinPrint_SourceInitialized(object sender, EventArgs e)
        {
            overlaySync.Attach();
        }

        private void WinPrint_Closed(object sender, EventArgs e)
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

        public void btn_print_Click(object sender, RoutedEventArgs e)
        {
            var instances_to_print = PrintListStore.GetOrCreate();
            if (instances_to_print.Count == 0)
            {
                new NotificationWindow(Loc.T("Noti.PrintLabels"), Loc.T("Noti.NoItems"), Loc.T("Noti.NoCalendarsToPrint")).Show();
                return;
            }

            var printDialog = new PrintDialog();
            var printConfirmed = printDialog.ShowDialog();
            if (printConfirmed != true)
                return;

            var pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);
            var documentBuilder = new LabelPrintDocumentBuilder();
            var document = documentBuilder.Build(instances_to_print, pageSize);
            printDialog.PrintDocument(document.DocumentPaginator, Loc.T("Print.Title"));
        }

        public void btn_clear_list_Click(object sender, RoutedEventArgs e)
        {
            var printList = PrintListStore.GetOrCreate();
            var removedCount = printList.Count;
            printList.Clear();
            selectedCalendarId = -1;

            RefreshTagItems();

            new NotificationWindow(Loc.T("Noti.PrintLabels"), Loc.F("Noti.ClearedCount", removedCount), Loc.T("Noti.ClearedSuccess")).Show();
        }

        public void btn_remove_selected_Click(object sender, RoutedEventArgs e)
        {
            if (selectedCalendarId <= 0)
            {
                new NotificationWindow(Loc.T("Noti.PrintLabels"), Loc.T("Noti.NoneSelected"), string.Empty).Show();
                return;
            }

            var printList = PrintListStore.GetOrCreate();
            Calendar removedItem = null;
            foreach (var calendar in printList)
            {
                if (calendar != null && calendar.id == selectedCalendarId)
                {
                    removedItem = calendar;
                    break;
                }
            }

            var removedCount = printList.RemoveAll(item => item != null && item.id == selectedCalendarId);

            if (removedCount <= 0)
            {
                selectedCalendarId = -1;
                RefreshTagItems();
                return;
            }

            selectedCalendarId = -1;
            RefreshTagItems();

            var removedContext = removedItem == null ? Loc.T("Noti.NoneSelected") : removedItem.id + " - " + removedItem.name;
            new NotificationWindow(Loc.T("Noti.PrintLabels"), removedContext, Loc.T("Noti.RemovedFromPrint")).Show();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void RefreshTagItems()
        {
            tagItems.Items.Clear();

            var instances_to_print = PrintListStore.GetOrCreate();
            if (selectedCalendarId > 0 && !instances_to_print.Exists(item => item != null && item.id == selectedCalendarId))
                selectedCalendarId = -1;

            if (instances_to_print.Count == 0)
            {
                currentPage = 1;
                totalPages = 1;
                UpdatePagingUi();
                return;
            }

            totalPages = Math.Max(1, (int)Math.Ceiling((double)instances_to_print.Count / ItemsPerPage));
            currentPage = Math.Max(1, Math.Min(currentPage, totalPages));

            var startIndex = (currentPage - 1) * ItemsPerPage;
            var endIndex = Math.Min(startIndex + ItemsPerPage, instances_to_print.Count);

            for (var i = startIndex; i < endIndex; i++)
            {
                tagItems.Items.Add(new global::Callen.PrintTagCardItem
                {
                    TextBoxHeight = TagCardHeight,
                    BorderColor = Brushes.Black,
                    TextBoxBackground = Brushes.White,
                    TextBoxMargin = TagCardMargin,
                    ID = instances_to_print[i].id,
                    Matrix = instances_to_print[i].matrix,
                    Collec = instances_to_print[i].collection,
                    Year = instances_to_print[i].year,
                    Name = instances_to_print[i].name,
                    BreakSpan = 1,
                    IsSelected = instances_to_print[i].id == selectedCalendarId
                });
            }

            UpdatePagingUi();
            tagScroll.ScrollToTop();
        }

        private void UpdatePagingUi()
        {
            pageCount.Text = totalPages <= 0 ? "0/0" : currentPage + "/" + totalPages;
            btn_prev_page.IsEnabled = currentPage > 1;
            btn_next_page.IsEnabled = currentPage < totalPages;
            btn_remove_selected.IsEnabled = selectedCalendarId > 0;
        }

        public void btn_prev_page_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage <= 1)
                return;

            currentPage--;
            RefreshTagItems();
        }

        public void btn_next_page_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage >= totalPages)
                return;

            currentPage++;
            RefreshTagItems();
        }

        public void tag_item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var clickedBorder = sender as System.Windows.Controls.Border;
            var clickedItem = clickedBorder == null ? null : clickedBorder.DataContext as PrintTagCardItem;
            if (clickedItem == null)
                return;

            selectedCalendarId = clickedItem.ID;
            RefreshTagItems();
        }
    }
}

