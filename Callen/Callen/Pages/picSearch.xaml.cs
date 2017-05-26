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

            fillList(); // fills 

            PicGrid.ItemsSource = items;
        }

        private void getItems()
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT * FROM G_Callen.ITEMS_PIC_MODE";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                dt = new DataTable("PicItems");
                sda.Fill(dt);

                pageCnt = Math.Ceiling((double)dt.Rows.Count / 16);

                pageCount.Text = current_page + "/" + pageCnt;
                
                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void fillList()
        {
            for(int i = 0; i < 16 && i < dt.Rows.Count; i++)
                items.Add(new PicItem() { ID = dt.Rows[i]["Item_ID"].ToString(),
                                            Name = dt.Rows[i]["Item_Name"].ToString(),
                                                ImgPath = dt.Rows[i]["Inst_PicPath"].ToString()+".jpeg"});
        }

        private void btn_next_page(object sender, RoutedEventArgs e) // Opens description window 
        {

        }

        private void btn_prev_page(object sender, RoutedEventArgs e) // Opens description window 
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
            getInstInfo((sender as Button).CommandParameter.ToString());
        }

        private void getInstInfo(String id) // Gets Selected item info 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_Callen.GET_ITEM_INFO @Item_id";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();

                param.ParameterName = "@Item_id";
                param.Value = id;
                cmd.Parameters.Add(param);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Item it = new Item(rdr["name"].ToString(), rdr["ID"].ToString(), rdr["descr"].ToString(), rdr["year"].ToString(),
                    rdr["theme"].ToString(), rdr["folder"].ToString(), rdr["sponsor"].ToString(), rdr["peer"].ToString(), rdr["Note"].ToString(), rdr["img_path"].ToString());

                    openDesc(it);
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void openDesc(Item it) // Opens the description of the item given
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winDesc popDesc = new winDesc(it);
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
