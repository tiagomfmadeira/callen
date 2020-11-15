using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

using Callen.Pages;
using Callen.Windows;

namespace Callen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MouseEventArgs e = new MouseEventArgs(Mouse.PrimaryDevice, 0);

            App.Current.Properties["PrintList"] = new List<String>();

            menu_bar_MouseLeave(new object(), e); // Menu starts closed

            // Starts in Home Page
            checkedMenu(1); 
            Switcher.Switch(this.Content_plane, new proSearch());
        }

        public void btn_close_Click(object sender, RoutedEventArgs e) 
        {
            this.Close();
        }

        public void btn_mini_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void btn_expand_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e) // Double click top bar to expand
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void menu_min_bar_MouseDown(object sender, MouseButtonEventArgs e) // Drag window 
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized) // If window is fullscreen when draged - set screen to normal 
                    this.WindowState = WindowState.Normal;
                
                this.DragMove();
            }
        }

        private void menu_bar_MouseEnter(object sender, MouseEventArgs e) // Expands Menu when entered 
        {
            GridLength grd = new GridLength(103, GridUnitType.Pixel);
            grid_menu_col.Width = grd;

            lbl_menu2.Visibility = Visibility.Visible;
            lbl_menu5.Visibility = Visibility.Visible;
            lbl_menu6.Visibility = Visibility.Visible;
            lbl_menu7.Visibility = Visibility.Visible;
        }

        private void menu_bar_MouseLeave(object sender, MouseEventArgs e) // Minimizes menu when left 
        {
            GridLength grd = new GridLength(0, GridUnitType.Pixel);
            grid_menu_col.Width = grd;

            lbl_menu2.Visibility = Visibility.Hidden;
            lbl_menu5.Visibility = Visibility.Hidden;
            lbl_menu6.Visibility = Visibility.Hidden;
            lbl_menu7.Visibility = Visibility.Hidden;
        }

        public void btn_colec_Click(object sender, RoutedEventArgs e) // Switch to Collection Page 
        {
            checkedMenu(2);
            Switcher.Switch(this.Content_plane, new Search());
        }

        public void btn_print_Click(object sender, RoutedEventArgs e) // Opens Print List window 
        {
            btn_print.IsChecked = false;
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winPrint popPrint = new winPrint();
            popPrint.Owner = win;
            win.Opacity = 0.5;
            popPrint.ShowDialog();

            this.Opacity = 1;
        }

        public void btn_help_Click(object sender, RoutedEventArgs e) // Opens Help window
        {
            btn_help.IsChecked = false;
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winHelp popHelp = new winHelp();
            popHelp.Owner = win;
            win.Opacity = 0.5;
            popHelp.ShowDialog();

            this.Opacity = 1;
        }

        public void btn_settings_Click(object sender, RoutedEventArgs e) // Opens Settings window 
        {
            btn_settings.IsChecked = false;
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winSettings popSettings = new winSettings();
            popSettings.Owner = win;
            win.Opacity = 0.5;
            popSettings.ShowDialog();

            this.Opacity = 1;
        }

        void checkedMenu(int menuNum) // HighLights the click button in menu (only) 
        {
            uncheckAll(); // remove highlight from all the buttons in menu
            switch (menuNum)
            {
                case 1:
                    btn_colec.IsChecked = true;
                    break;
            }
        }

        void uncheckAll() // Removes Hightlight from buttons in menu 
        {
            btn_colec.IsChecked = false;
        }
    }
}
