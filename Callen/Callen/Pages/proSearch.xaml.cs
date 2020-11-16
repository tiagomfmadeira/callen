using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Callen.Windows;

namespace Callen.Pages
{
    /// <summary>
    ///     Interaction logic for proSearchU.xaml
    /// </summary>
    public partial class proSearch : UserControl
    {
        private readonly DataTable items;

        public proSearch()
        {
            InitializeComponent();
            PreviewKeyDown += HandleEnter;
            FillDataGrid();
        }

        public proSearch(DataTable dt)
        {
            InitializeComponent();
            PreviewKeyDown += HandleEnter;
            items = dt;
            grdColec.ItemsSource = items.DefaultView;
        }

        private void HandleEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var instance = DBConnect.getInstanceInfo((grdColec.SelectedItem as DataRowView)["ID"].ToString());

                if (instance != null)
                    openDesc(instance);
            }
            else if (e.Key == Key.P)
            {
                if (grdColec.SelectedItems.Count == 0)
                {
                    var noti = new winNotification("Print List", "0 items adicionados", "Nenhum item está selecionado");
                    noti.Show();
                }
                else
                {
                    Instance instance;
                    var addedInstCount = 0;

                    foreach (DataRowView item in grdColec.SelectedItems)
                    {
                        instance = DBConnect.getInstanceInfo(item["ID"].ToString());

                        if (!(Application.Current.Properties["PrintList"] as List<Instance>).Contains(instance))
                        {
                            (Application.Current.Properties["PrintList"] as List<Instance>).Add(instance);
                            addedInstCount++;
                        }
                    }

                    var noti = new winNotification("Print List", addedInstCount + " novos items",
                        "foram adicionados com sucesso à lista de impressão");
                    noti.Show();
                }
            }

            e.Handled = true;
        }

        private void
            FillDataGrid() // Used to Fill the Data Grid with items information (Name, Descr, Year, Theme, Folder, Other) 
        {
            var itemsView = DBConnect.getItemsInfo();

            if (itemsView != null)
                grdColec.ItemsSource = itemsView;
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var instance = DBConnect.getInstanceInfo((grdColec.SelectedItem as DataRowView)["ID"].ToString());

            if (instance != null)
                openDesc(instance);
        }

        private void openDesc(Instance it) // Opens the description of specific item in Row
        {
            var win = (MainWindow) Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            winDesc popDesc;

            // Check if there is a image
            if (it.image_path != "")
                popDesc = new winDesc(it, true);
            else
                popDesc = new winDesc(it, false);

            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            if (popDesc.wasDeleted())
                for (var i = items.Rows.Count - 1; i >= 0; i--)
                {
                    var dr = items.Rows[i];

                    if (dr["ID"].ToString() == it.inst_num) dr.Delete();
                }

            win.Opacity = 1;
        }
    }
}