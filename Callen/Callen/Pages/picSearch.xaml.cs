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
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.ITEMS_PIC_MODE";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                dt = new DataTable("PicItems");
                sda.Fill(dt);
                
                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void fillList(int page)
        {
            pageCnt = Math.Ceiling((double)dt.Rows.Count / 16);

            pageCount.Text = current_page + "/" + pageCnt;

            items.Clear();

            for (int i = 0; i < 16 && i < dt.Rows.Count; i++)
                items.Add(new PicItem() { ID = dt.Rows[i]["Item_ID"].ToString(),
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
            getInstInfo((sender as Button).CommandParameter.ToString());
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
                    rdr["theme"].ToString(), rdr["folder"].ToString(), rdr["peer"].ToString(),
                    rdr["sponsor"].ToString(), rdr["other"].ToString(), rdr["img_path"].ToString(),rdr["note"].ToString(),
                    rdr["Series_Name"].ToString(), rdr["NumberInSeries"].ToString());

                    openDesc(it);
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void openDesc(Instance it) // Opens the description of the item given
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
