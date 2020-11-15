using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using Callen.Windows;
using Callen.Windows.Forms;

using System.Data;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for picSearchU.xaml
    /// </summary>
    public partial class picSearch : UserControl
    {
        private DataTable dt;
        private List<PicItem> items;

        private int current_page;
        private double pageCnt;

        public picSearch()
        {
            InitializeComponent();

            items = new List<PicItem>();
            current_page = 1;

            getItems(); // saves items in data table 

            fillList(current_page); // fills 
        }

        public picSearch(DataTable tmpdt)
        {
            InitializeComponent();

            items = new List<PicItem>();
            current_page = 1;

            dt = tmpdt;

            fillList(current_page); // fills 
        }

        private void getItems()
        {
            DBConnect.getPicItems(dt);
        }

        private void fillList(int page)
        {
            pageCnt = Math.Ceiling((double)dt.Rows.Count / 16);

            pageCount.Text = current_page + "/" + pageCnt;

            items.Clear();

            for (int i = 0; i < 16 && i < dt.Rows.Count; i++)
                items.Add(new PicItem() { ID = dt.Rows[i]["Inst_Number"].ToString(),
                                            Name = dt.Rows[i]["Item_Name"].ToString(),
                                                ImgPath = dt.Rows[i]["Inst_PicPath"].ToString()+".jpeg"});


            PicGrid.ItemsSource = items;
        }

        private void btn_next_page(object sender, RoutedEventArgs e) // TODO goes to the next page 
        {

        }

        private void btn_prev_page(object sender, RoutedEventArgs e) // TODO goes to the prev page 
        {

        }

        private void formPage(object sender, RoutedEventArgs e) // Opens form window 
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winAddItem popForm = new winAddItem();
            popForm.Owner = win;
            win.Opacity = 0.5;
            popForm.ShowDialog();

            win.Opacity = 1;
        }
        
        private void btn_open_desc(object sender, RoutedEventArgs e) // Opens description window 
        {
            Instance instance = DBConnect.getInstanceInfo((sender as Button).CommandParameter.ToString());

            if (instance != null)
                openDesc(instance);
        }

        private void openDesc(Instance it) // Opens the description of the item given
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winDesc popDesc = new winDesc(it, true);
            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            win.Opacity = 1;
        }

        private class PicItem
        {
            public String ID { set; get; }
            public String Name { set; get; }
            public String ImgPath { set; get; }
        }
    }
}
