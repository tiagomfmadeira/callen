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

using System.Data;
using System.Data.SqlClient;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for proSearchU.xaml
    /// </summary>
    public partial class proSearch : UserControl
    {
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
            grdColec.ItemsSource = dt.DefaultView;
        }

        private void HandleEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                getInstInfo((grdColec.SelectedItem as DataRowView)["ID"].ToString());

            e.Handled = true;
        }

        private void FillDataGrid() // Used to Fill the Data Grid with items information (Favourite, Name, Descr, Year, Theme, Folder, Peer, Sponsor) 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.ITEMS_INFO";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Items");
                sda.Fill(dt);

                grdColec.ItemsSource = dt.DefaultView;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)  
        {
            getInstInfo((grdColec.SelectedItem as DataRowView)["ID"].ToString());
        }

        private void getInstInfo(String id) // Gets Selected item info 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.GET_INST_INFO @InstID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();

                param.ParameterName = "@InstID";
                param.Value = id;
                cmd.Parameters.Add(param);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Instance it = new Instance(rdr["name"].ToString(), rdr["ID"].ToString(), rdr["descr"].ToString(), rdr["year"].ToString(),
                    rdr["theme"].ToString(), rdr["folder"].ToString(), rdr["peer"].ToString(), rdr["sponsor"].ToString(),
                    rdr["other"].ToString(), rdr["img_path"].ToString(), rdr["note"].ToString(), rdr["Series_Name"].ToString(), rdr["NumberInSeries"].ToString());

                    openDesc(it);
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void openDesc(Instance it) // Opens the description of specific item in Row
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winDesc popDesc = new winDesc(it);
            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            win.Opacity = 1;
        }

        private void formPage(object sender, RoutedEventArgs e) // Opens the form to add a new item 
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winAddItem popForm = new winAddItem();
            popForm.Owner = win;
            win.Opacity = 0.5;
            popForm.ShowDialog();

            if (popForm.getInserted()) // A item was inserted (refreshes datagrid)
                FillDataGrid();

            win.Opacity = 1;
        }

        private void check_fav(object sender, RoutedEventArgs e)
        {
            String id = (grdColec.SelectedItem as DataRowView)["ID"].ToString();

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.TOGGLE_FAVOURITE @ItemID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@ItemID";
                param.Value = id;
                cmd.Parameters.Add(param);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
    }
}