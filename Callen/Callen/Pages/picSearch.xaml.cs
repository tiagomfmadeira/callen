using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Callen.Windows;
using Callen.Windows.Forms;

namespace Callen.Pages
{
    /// <summary>
    ///     Interaction logic for picSearchU.xaml
    /// </summary>
    public partial class picSearch : UserControl
    {
        private readonly int current_page;
        private readonly DataTable dt;
        private readonly List<PicItem> items;
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
            pageCnt = Math.Ceiling((double) dt.Rows.Count / 16);

            pageCount.Text = current_page + "/" + pageCnt;

            items.Clear();

            for (var i = 0; i < 16 && i < dt.Rows.Count; i++)
                items.Add(new PicItem
                {
                    ID = dt.Rows[i]["Inst_Number"].ToString(),
                    Name = dt.Rows[i]["Item_Name"].ToString(),
                    ImgPath = dt.Rows[i]["Inst_PicPath"] + ".jpeg"
                });


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
            var win = (MainWindow) Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var popForm = new winAddItem();
            popForm.Owner = win;
            win.Opacity = 0.5;
            popForm.ShowDialog();

            win.Opacity = 1;
        }

        private void btn_open_desc(object sender, RoutedEventArgs e) // Opens description window 
        {
            var instance = DBConnect.getInstanceInfo((sender as Button).CommandParameter.ToString());

            if (instance != null)
                openDesc(instance);
        }

        private void openDesc(Instance it) // Opens the description of the item given
        {
            var win = (MainWindow) Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var popDesc = new winDesc(it, true);
            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            win.Opacity = 1;
        }

        private class PicItem
        {
            public string ID { set; get; }
            public string Name { set; get; }
            public string ImgPath { set; get; }
        }
    }
}