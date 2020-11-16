using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Callen.Pages;
using Callen.Windows;

namespace Callen
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var e = new MouseEventArgs(Mouse.PrimaryDevice, 0);

            Application.Current.Properties["PrintList"] = new List<Instance>();

            menu_bar_MouseLeave(new object(), e); // Menu starts closed

            // Starts in Home Page
            checkedMenu(1);
            Switcher.Switch(Content_plane, new proSearch());
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
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void
            ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e) // Double click top bar to expand
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }

        private void menu_min_bar_MouseDown(object sender, MouseButtonEventArgs e) // Drag window 
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (WindowState == WindowState.Maximized) // If window is fullscreen when draged - set screen to normal 
                    WindowState = WindowState.Normal;

                DragMove();
            }
        }

        private void menu_bar_MouseEnter(object sender, MouseEventArgs e) // Expands Menu when entered 
        {
            var grd = new GridLength(103, GridUnitType.Pixel);
            grid_menu_col.Width = grd;

            lbl_menu2.Visibility = Visibility.Visible;
            lbl_menu5.Visibility = Visibility.Visible;
            lbl_menu6.Visibility = Visibility.Visible;
            lbl_menu7.Visibility = Visibility.Visible;
        }

        private void menu_bar_MouseLeave(object sender, MouseEventArgs e) // Minimizes menu when left 
        {
            var grd = new GridLength(0, GridUnitType.Pixel);
            grid_menu_col.Width = grd;

            lbl_menu2.Visibility = Visibility.Hidden;
            lbl_menu5.Visibility = Visibility.Hidden;
            lbl_menu6.Visibility = Visibility.Hidden;
            lbl_menu7.Visibility = Visibility.Hidden;
        }

        public void btn_colec_Click(object sender, RoutedEventArgs e) // Switch to Collection Page 
        {
            checkedMenu(2);
            Switcher.Switch(Content_plane, new Search());
        }

        public void btn_print_Click(object sender, RoutedEventArgs e) // Opens Print List window 
        {
            btn_print.IsChecked = false;
            var win = (MainWindow) Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var popPrint = new winPrint();
            popPrint.Owner = win;
            win.Opacity = 0.5;
            popPrint.ShowDialog();

            Opacity = 1;
        }

        public void btn_help_Click(object sender, RoutedEventArgs e) // Opens Help window
        {
            btn_help.IsChecked = false;
            var win = (MainWindow) Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var popHelp = new winHelp();
            popHelp.Owner = win;
            win.Opacity = 0.5;
            popHelp.ShowDialog();

            Opacity = 1;
        }

        public void btn_settings_Click(object sender, RoutedEventArgs e) // Opens Settings window 
        {
            btn_settings.IsChecked = false;
            var win = (MainWindow) Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var popSettings = new winSettings();
            popSettings.Owner = win;
            win.Opacity = 0.5;
            popSettings.ShowDialog();

            Opacity = 1;
        }

        private void checkedMenu(int menuNum) // HighLights the click button in menu (only) 
        {
            uncheckAll(); // remove highlight from all the buttons in menu
            switch (menuNum)
            {
                case 1:
                    btn_colec.IsChecked = true;
                    break;
            }
        }

        private void uncheckAll() // Removes Hightlight from buttons in menu 
        {
            btn_colec.IsChecked = false;
        }
    }
}