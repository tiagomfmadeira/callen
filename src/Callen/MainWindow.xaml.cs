using System;
using System.Windows;
using System.Windows.Input;
using Callen.Pages;
using Callen.Windows;
using Callen.Windows.Forms;

namespace Callen
{
    public partial class MainWindow : Window
    {
        private const int CollectionMenuIndex = 1;
        private const double ExpandedMenuWidth = 100;

        private bool maximized;
        private bool pendingDragFromMaximized;
        private Point dragStartPoint;
        private double storedHeight;
        private double storedLeft;
        private double storedTop;
        private double storedWidth;

        public MainWindow()
        {
            InitializeComponent();
            PrintListStore.GetOrCreate();
            SetMenuState(false);

            // Starts in Search
            checkedMenu(CollectionMenuIndex);
            Switcher.Switch(Content_plane, new Search());

            StoreCurrentWindowBounds();
            maximized = true;
            ApplyWindowBounds(
                SystemParameters.WorkArea.Left,
                SystemParameters.WorkArea.Top,
                SystemParameters.WorkArea.Height,
                SystemParameters.WorkArea.Width);
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
            if (maximized)
            {
                ApplyWindowBounds(storedLeft, storedTop, storedHeight, storedWidth);
                maximized = false;
            }
            else
            {
                StoreCurrentWindowBounds();
                ApplyWindowBounds(
                    SystemParameters.WorkArea.Left,
                    SystemParameters.WorkArea.Top,
                    SystemParameters.WorkArea.Height,
                    SystemParameters.WorkArea.Width);
                maximized = true;
            }
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
            pendingDragFromMaximized = maximized;

            if (!maximized)
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
            var widthRatio = ActualWidth <= 0 ? 0.5 : current.X / ActualWidth;

            ApplyWindowBounds(
                screen.X - (storedWidth * widthRatio),
                SystemParameters.WorkArea.Top,
                storedHeight,
                storedWidth);

            maximized = false;
            pendingDragFromMaximized = false;
            DragMove();
        }

        private void menu_min_bar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            pendingDragFromMaximized = false;
        }

        private void menu_bar_MouseEnter(object sender, MouseEventArgs e)
        {
            SetMenuState(true);
        }

        private void menu_bar_MouseLeave(object sender, MouseEventArgs e)
        {
            SetMenuState(false);
        }

        public void btn_colec_Click(object sender, RoutedEventArgs e)
        {
            checkedMenu(CollectionMenuIndex);
            Switcher.Switch(Content_plane, new Search());
        }

        public void btn_print_Click(object sender, RoutedEventArgs e)
        {
            btn_print.IsChecked = false;
            DialogHelper.ShowOwnedDialog(new PrintWindow(), DialogHelper.GetActiveMainWindow());
        }

        public void btn_add_item_Click(object sender, RoutedEventArgs e)
        {
            var addItemWindow = new AddItemWindow();
            DialogHelper.ShowOwnedDialog(addItemWindow, DialogHelper.GetActiveMainWindow());

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
            btn_settings.IsChecked = false;
            DialogHelper.ShowOwnedDialog(new SettingsWindow(), DialogHelper.GetActiveMainWindow());
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

        private void ApplyWindowBounds(double left, double top, double height, double width)
        {
            Left = left;
            Top = top;
            Height = height;
            Width = width;
        }

        private void StoreCurrentWindowBounds()
        {
            storedLeft = Left;
            storedTop = Top;
            storedHeight = Height;
            storedWidth = Width;
        }

        private void SetMenuState(bool expanded)
        {
            grid_menu_col.Width = expanded
                ? new GridLength(ExpandedMenuWidth)
                : new GridLength(0, GridUnitType.Pixel);

            var visibility = expanded ? Visibility.Visible : Visibility.Hidden;

            lbl_menu2.Visibility = visibility;
            lbl_menu_add.Visibility = visibility;
            lbl_menu5.Visibility = visibility;
            lbl_menu7.Visibility = visibility;
        }
    }
}



