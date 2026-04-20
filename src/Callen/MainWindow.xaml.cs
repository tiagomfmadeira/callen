using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Callen.Pages;
using Callen.Windows;
using Callen.Windows.Forms;

namespace Callen
{
    public partial class MainWindow : Window
    {
        private const int CollectionMenuIndex = 1;
        private static readonly Brush ActiveSidebarActionBrush = Brushes.White;

        private bool isWorkAreaMaximized;
        private bool pendingDragFromMaximized;
        private Point dragStartPoint;
        private Rect restoreBounds;

        public MainWindow()
        {
            InitializeComponent();
            PrintListStore.GetOrCreate();

            // Starts in Search
            checkedMenu(CollectionMenuIndex);
            Switcher.Switch(Content_plane, new Search());

            restoreBounds = new Rect(Left, Top, Width, Height);
            MaximizeToWorkArea();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void btn_mini_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public void btn_expand_Click(object sender, RoutedEventArgs e)
        {
            if (isWorkAreaMaximized)
            {
                RestoreFromWorkArea();
                return;
            }

            restoreBounds = new Rect(Left, Top, Width, Height);
            MaximizeToWorkArea();
        }

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btn_expand_Click(sender, e);
        }

        private void menu_min_bar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed || e.ChangedButton != MouseButton.Left)
                return;

            dragStartPoint = e.GetPosition(this);
            pendingDragFromMaximized = isWorkAreaMaximized;

            if (!pendingDragFromMaximized)
                DragMove();
        }

        private void menu_min_bar_MouseMove(object sender, MouseEventArgs e)
        {
            if (!pendingDragFromMaximized || Mouse.LeftButton != MouseButtonState.Pressed)
                return;

            var current = e.GetPosition(this);
            if (Math.Abs(current.X - dragStartPoint.X) < 4 && Math.Abs(current.Y - dragStartPoint.Y) < 4)
                return;

            var screen = PointToScreen(current);
            var restoredWidth = restoreBounds.Width > 0 ? restoreBounds.Width : Math.Max(800, Width);
            var restoredHeight = restoreBounds.Height > 0 ? restoreBounds.Height : Math.Max(600, Height);
            var widthRatio = ActualWidth <= 0 ? 0.5 : current.X / ActualWidth;

            Left = screen.X - (restoredWidth * widthRatio);
            Top = screen.Y - dragStartPoint.Y;
            Width = restoredWidth;
            Height = restoredHeight;
            isWorkAreaMaximized = false;

            pendingDragFromMaximized = false;
            DragMove();
        }

        private void menu_min_bar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            pendingDragFromMaximized = false;
        }

        public void btn_colec_Click(object sender, RoutedEventArgs e)
        {
            checkedMenu(CollectionMenuIndex);
            Switcher.Switch(Content_plane, new Search());
        }

        public void btn_print_Click(object sender, RoutedEventArgs e)
        {
            ShowSidebarActionDialog(btn_print, () =>
                DialogHelper.ShowOwnedDialog(new PrintWindow(), DialogHelper.GetActiveMainWindow()));
        }

        public void btn_add_item_Click(object sender, RoutedEventArgs e)
        {
            var addItemWindow = new AddItemWindow();
            ShowSidebarActionDialog(btn_add_item, () =>
                DialogHelper.ShowOwnedDialog(addItemWindow, DialogHelper.GetActiveMainWindow()));

            if (!addItemWindow.inserted)
                return;

            if (Content_plane.Children.Count == 0)
                return;

            var searchPage = Content_plane.Children[0] as Search;
            if (searchPage != null)
                searchPage.ReloadData();
        }

        public void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            ShowSidebarActionDialog(btn_settings, () =>
                DialogHelper.ShowOwnedDialog(
                    new SettingsWindow(() =>
                    {
                        if (Content_plane.Children.Count == 0)
                            return;

                        var searchPage = Content_plane.Children[0] as Search;
                        searchPage?.ReloadData();
                    }),
                    DialogHelper.GetActiveMainWindow()));
        }

        private static void ShowSidebarActionDialog(Button actionButton, Action openDialog)
        {
            actionButton.Foreground = ActiveSidebarActionBrush;
            try
            {
                openDialog();
            }
            finally
            {
                actionButton.ClearValue(ForegroundProperty);
            }
        }

        private void checkedMenu(int menuNum)
        {
            btn_colec.IsChecked = false;
            switch (menuNum)
            {
                case 1:
                    btn_colec.IsChecked = true;
                    break;
            }
        }

        private void MaximizeToWorkArea()
        {
            var workArea = SystemParameters.WorkArea;
            Left = workArea.Left;
            Top = workArea.Top;
            Width = workArea.Width;
            Height = workArea.Height;
            isWorkAreaMaximized = true;
        }

        private void RestoreFromWorkArea()
        {
            var hasValidRestoreBounds = restoreBounds.Width > 0 && restoreBounds.Height > 0;
            if (!hasValidRestoreBounds)
                return;

            Left = restoreBounds.Left;
            Top = restoreBounds.Top;
            Width = restoreBounds.Width;
            Height = restoreBounds.Height;
            isWorkAreaMaximized = false;
        }

    }
}



