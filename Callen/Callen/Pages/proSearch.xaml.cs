using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Callen.Windows;

using System.Data;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for proSearchU.xaml
    /// </summary>
    public partial class proSearch : UserControl
    {
        DataTable items;

        public proSearch()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEnter);
            FillDataGrid();
        }

        public proSearch(DataTable dt)
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEnter);
            items = dt;
            grdColec.ItemsSource = items.DefaultView;
        }

        private void HandleEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Instance instance = DBConnect.getInstanceInfo((grdColec.SelectedItem as DataRowView)["ID"].ToString());

                if (instance != null)
                    openDesc(instance);
            }

            e.Handled = true;
        }

        private void FillDataGrid() // Used to Fill the Data Grid with items information (Favourite, Name, Descr, Year, Theme, Folder, Other) 
        {
            DataView itemsView = DBConnect.getItemsInfo();

            if (itemsView != null)
                grdColec.ItemsSource = itemsView;
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Instance instance = DBConnect.getInstanceInfo((grdColec.SelectedItem as DataRowView)["ID"].ToString());

            if (instance != null)
                openDesc(instance);
        }

        private void openDesc(Instance it) // Opens the description of specific item in Row
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            winDesc popDesc;

            // Check if there is a image
            if (it.getImagePath() != "")
            {
                popDesc = new winDesc(it, true);
            }
            else
            {
                popDesc = new winDesc(it, false);
            }

            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            if (popDesc.wasDeleted())
            {
                for (int i = items.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = items.Rows[i];

                    if (dr["ID"].ToString() == it.getInstID())
                    {
                        dr.Delete();
                    }
                }
            }

            win.Opacity = 1;
        }

        private void check_fav(object sender, RoutedEventArgs e)
        {
            DBConnect.toggleFavourite((grdColec.SelectedItem as DataRowView)["ID"].ToString());
        }
    }
}