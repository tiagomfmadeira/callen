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

using Callen.Windows;
using Callen.Windows.Forms;

using System.Data;
using System.Data.SqlClient;

using System.Text.RegularExpressions;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        public Search()
        {
            InitializeComponent();
            btn_pro_mode.IsChecked = true;
            Switcher.Switch(this.Search_mode, new proSearch());
        }

        private void btn_pic_mode_Click(object sender, RoutedEventArgs e)
        {
            if (btn_pic_mode.IsChecked == true)
            {
                Switcher.Switch(this.Search_mode, new picSearch());
                btn_pro_mode.IsChecked = false;
                return;
            }
            btn_pic_mode.IsChecked = true;
        }

        private void btn_pro_mode_Click(object sender, RoutedEventArgs e)
        {
            if (btn_pro_mode.IsChecked == true)
            {
                Switcher.Switch(this.Search_mode, new proSearch());
                btn_pic_mode.IsChecked = false;
                return;
            }
            btn_pro_mode.IsChecked = true;
        }

        private void advanceSearch_Click(object sender, RoutedEventArgs e) // rotates arrow in search bar when clicked 
        {
            /*if(advanceSearch.IsChecked == true)
            {
                RotateTransform rotateTransform = new RotateTransform(90,advanceSearch.Height/2,advanceSearch.Width/2);
                advanceSearch.RenderTransform = rotateTransform;
            }
            else
            {
                RotateTransform rotateTransform = new RotateTransform(0, advanceSearch.Height/2, advanceSearch.Width/2);
                advanceSearch.RenderTransform = rotateTransform;
            }*/
        }

        private void btn_adv_search_Click(object sender, RoutedEventArgs e)
        {
            // if all textbox are empty, just fill the data grid again (This way it uses the procedure used to fill the data grid with everything instead of the searcg one)
            if (String.IsNullOrEmpty(id_box.Text) && String.IsNullOrEmpty(name_box.Text) && String.IsNullOrEmpty(desc_box.Text) && String.IsNullOrEmpty(year_box.Text)
                 && String.IsNullOrEmpty(note_box.Text) && String.IsNullOrEmpty(theme_box.Text) && String.IsNullOrEmpty(folder_box.Text)
                 && String.IsNullOrEmpty(other_box.Text) && String.IsNullOrEmpty(collec_box.Text))
                Switcher.Switch(this.Search_mode, new proSearch());

            Instance inst = new Instance(name_box.Text, id_box.Text, "", desc_box.Text, year_box.Text, theme_box.Text, folder_box.Text, other_box.Text, "", note_box.Text, collec_box.Text);

            DataTable dt = DBConnect.searchInstances(inst, btn_pic_mode.IsChecked == true);

            if (dt != null)
            {
                if (btn_pic_mode.IsChecked == false)
                    Switcher.Switch(this.Search_mode, new proSearch(dt));
                else
                    Switcher.Switch(this.Search_mode, new picSearch(dt));
            }
        }

        private void formPage(object sender, RoutedEventArgs e) // Opens the form to add a new item 
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winAddItem popForm = new winAddItem();
            popForm.Owner = win;
            win.Opacity = 0.5;
            popForm.ShowDialog();

            if (popForm.getInserted()) // A item was inserted (refreshes datagrid)
                btn_adv_search.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

            win.Opacity = 1;
        }

        private void Border_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)// If Enter key is pressed when in the search menu
            {   // search
                btn_adv_search.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                if (advanceSearch.IsChecked == true) // Close search menu
                    advanceSearch.IsChecked = false;
                else
                    advanceSearch.IsChecked = true;
            }
        }
    }
}
