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
                openDesc();

            e.Handled = true;
        }

        private void FillDataGrid() // Used to Fill the Data Grid with items information (Favourite, Name, Descr, Year, Theme, Folder, Peer, Sponsor) 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT * FROM G_Callen.ITEMS_INFO";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("INST");
                sda.Fill(dt);

                grdColec.ItemsSource = dt.DefaultView;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private String[] getExtra(String id) // Gets Note and Image Path for specific Item 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Note, Inst_PicPath "
                                 +"FROM G_Callen.INST "
                                 +"WHERE Inst_Number = " + id;

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("descr");
                sda.Fill(dt);

                String tmp = string.Join(", ", dt.Rows[0].ItemArray);

                return tmp.Split(',');
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                return new string[2];
            }
        } 

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)  
        {
            openDesc();
        }

        private void openDesc() // Opens the description of specific item in Row
        {
            DataRowView row = (DataRowView)grdColec.SelectedItem;

            Item it = new Item(row["name"].ToString(), row["ID"].ToString(), row["descr"].ToString(), row["year"].ToString(),
                 row["theme"].ToString(), row["folder"].ToString(), row["sponsor"].ToString(), row["peer"].ToString()); ;

            // Get Other and Image Path
            String[] extra = getExtra(row["ID"].ToString());
            if (extra[0] != "")
                it.setOther(extra[0]);
            if (extra[1] != "")
                it.setImagePath(extra[1]);

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

            win.Opacity = 1;
        }

        private void check_fav(object sender, RoutedEventArgs e)
        {
            String id = (grdColec.SelectedItem as DataRowView)["ID"].ToString();

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_Callen.SET_FAVOURITE @ItemID";

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