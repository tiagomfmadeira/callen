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

using System.Data;
using System.Data.SqlClient;

using Callen.Windows;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for Stats.xaml
    /// </summary>
    public partial class Stats : UserControl
    {
        public Stats()
        {
            InitializeComponent();

            fillLastInst();
            fillLastMod();
            fillLastView();
        }

        private void fillLastMod() // Fills LastMod DataGrid
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.LastModItems";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("LMOD");
                sda.Fill(dt);

                LastMod.ItemsSource = dt.DefaultView;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void fillLastView() // Fills LastView DataGrid
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.LastVisItems";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("LVIEW");
                sda.Fill(dt);

                LastView.ItemsSource = dt.DefaultView;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void fillLastInst() // Fills LastInst DataGrid
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.LastInsertItems";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("LINST");
                sda.Fill(dt);

                LastInst.ItemsSource = dt.DefaultView;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e) // Double click to open desc
        {
            DataGridRow r = sender as DataGridRow;
            DataRowView row = r.Item as DataRowView;
            getInstInfo(row["Inst_Number"].ToString());
        }

        private void KeyDown_Item(object sender, KeyEventArgs e) //Press enter to open desc
        {
            if (e.Key == Key.Enter)
            {
                DataGrid grid = sender as DataGrid;
                DataRowView row = grid.SelectedItem as DataRowView;
                getInstInfo(row["Inst_Number"].ToString());
            }
            e.Handled = true;
        }

        private void Grid_LostFocus(object sender, RoutedEventArgs e) //Uncheck selected Item when datagrid loses focus
        {
            DataGrid grd = sender as DataGrid;
            switch (grd.Name)
            {
                case "LastMod":
                    LastMod.UnselectAll();
                    break;

                case "LastInst":
                    LastInst.UnselectAll();
                    break;

                case "LastView":
                    LastView.UnselectAll();
                    break;
            }
        }

        private void getInstInfo(String id) // Gets Selected item info 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.GET_INST_INFO @InstID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();

                param.ParameterName = "@InstID";
                param.Value = id;
                cmd.Parameters.Add(param);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Instance it = new Instance(rdr["name"].ToString(), rdr["item_id"].ToString(), id, rdr["descr"].ToString(), rdr["year"].ToString(),
                    rdr["theme"].ToString(), rdr["folder"].ToString(), rdr["other"].ToString(),
                    rdr["img_path"].ToString(), rdr["note"].ToString(), rdr["collec"].ToString());

                    openDesc(it);
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            fillLastView(); //Refresh last view grid
        }

        private void openDesc(Instance it) // Opens the description of the item given
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

            if (popDesc.wasEdited())
            {
                fillLastInst();
                fillLastMod();
                fillLastView();
            }

            if (popDesc.wasDeleted())
            {
                fillLastInst();
                fillLastMod();
                fillLastView();
            }

            win.Opacity = 1;
        }
    }
}
