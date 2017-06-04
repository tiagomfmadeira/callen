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
    /// Interaction logic for Gifts.xaml
    /// </summary>
    public partial class Gifts : UserControl
    {
        public Gifts()
        {
            InitializeComponent();
            FillDataGrid();
        }

        private void btn_gift_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winAddGift popGift = new winAddGift();
            popGift.Owner = win;
            win.Opacity = 0.5;
            popGift.ShowDialog();

            win.Opacity = 1;
        }

        private void FillDataGrid() // Used to Fill the Data Grid with items information (Favourite, Name, Descr, Year, Theme, Folder, Peer, Sponsor) 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT * FROM G_Callen.GIFT_INST";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Gifts");
                sda.Fill(dt);

                grdGifts.ItemsSource = dt.DefaultView;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty((grdGifts.SelectedItem as DataRowView)["Inst"].ToString()))
                getInstInfo((grdGifts.SelectedItem as DataRowView)["Inst"].ToString());
            else {
                DataRowView row = (grdGifts.SelectedItem as DataRowView);
                openDesc(new Item(row["name"].ToString(),row["Item"].ToString(),row["descr"].ToString(),row["year"].ToString(),row["sponsor"].ToString(),""));
            }
        }

        private void getInstInfo(String id) // Gets Selected item info 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_Callen.GET_INST_INFO @InstID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();

                param.ParameterName = "@InstID";
                param.Value = id;
                cmd.Parameters.Add(param);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Instance it = new Instance(rdr["name"].ToString(), rdr["ID"].ToString(), rdr["descr"].ToString(), rdr["year"].ToString(),
                    rdr["theme"].ToString(), rdr["folder"].ToString(), rdr["peer"].ToString(), rdr["sponsor"].ToString(), rdr["other"].ToString(),
                                        rdr["img_path"].ToString(), rdr["note"].ToString(),rdr["Series_Name"].ToString(),rdr["NumberInSeries"].ToString());

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
            winDescGift popDesc = new winDescGift(it);
            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            win.Opacity = 1;
        }

        private void openDesc(Item it) // Opens the description of specific item in Row
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winDescGift popDesc = new winDescGift(it);
            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            win.Opacity = 1;
        }
    }
}
