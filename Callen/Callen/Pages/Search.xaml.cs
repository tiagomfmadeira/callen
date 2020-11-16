using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Callen.Windows.Forms;

namespace Callen.Pages
{
    /// <summary>
    ///     Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        public Search()
        {
            InitializeComponent();
            btn_pro_mode.IsChecked = true;
            Switcher.Switch(Search_mode, new proSearch());
        }

        private void btn_pic_mode_Click(object sender, RoutedEventArgs e)
        {
            if (btn_pic_mode.IsChecked == true)
            {
                Switcher.Switch(Search_mode, new picSearch());
                btn_pro_mode.IsChecked = false;
                return;
            }

            btn_pic_mode.IsChecked = true;
        }

        private void btn_pro_mode_Click(object sender, RoutedEventArgs e)
        {
            if (btn_pro_mode.IsChecked == true)
            {
                Switcher.Switch(Search_mode, new proSearch());
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
            if (string.IsNullOrEmpty(id_box.Text) && string.IsNullOrEmpty(name_box.Text) &&
                string.IsNullOrEmpty(desc_box.Text) && string.IsNullOrEmpty(year_box.Text)
                && string.IsNullOrEmpty(note_box.Text) && string.IsNullOrEmpty(theme_box.Text) &&
                string.IsNullOrEmpty(folder_box.Text)
                && string.IsNullOrEmpty(other_box.Text) && string.IsNullOrEmpty(collec_box.Text))
                Switcher.Switch(Search_mode, new proSearch());

            var inst = new Instance(name_box.Text, id_box.Text, "", desc_box.Text, year_box.Text, theme_box.Text,
                folder_box.Text, other_box.Text, "", note_box.Text, collec_box.Text);

            var dt = DBConnect.searchInstances(inst, btn_pic_mode.IsChecked == true);

            if (dt != null)
            {
                if (btn_pic_mode.IsChecked == false)
                    Switcher.Switch(Search_mode, new proSearch(dt));
                else
                    Switcher.Switch(Search_mode, new picSearch(dt));
            }
        }

        private void formPage(object sender, RoutedEventArgs e) // Opens the form to add a new item 
        {
            var win = (MainWindow) Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var popForm = new winAddItem();
            popForm.Owner = win;
            win.Opacity = 0.5;
            popForm.ShowDialog();

            if (popForm.inserted) // A item was inserted (refreshes datagrid)
                btn_adv_search.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

            win.Opacity = 1;
        }

        private void Border_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) // If Enter key is pressed when in the search menu
            {
                // search
                btn_adv_search.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

                if (advanceSearch.IsChecked == true) // Close search menu
                    advanceSearch.IsChecked = false;
                else
                    advanceSearch.IsChecked = true;
            }
        }
    }
}