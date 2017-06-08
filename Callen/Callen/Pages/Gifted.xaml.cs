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
    /// Interaction logic for Gifted.xaml
    /// </summary>
    public partial class Gifted : UserControl
    {
        public Gifted()
        {
            InitializeComponent();
            FillGiftedDataGrid();
        }

        public Gifted(DataTable dt)
        {
            InitializeComponent();

            grdGifts.ItemsSource = dt.DefaultView;
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty((grdGifts.SelectedItem as DataRowView)["Inst"].ToString()))
            {
                Instance tmp = GiftsSupp.getInstInfo((grdGifts.SelectedItem as DataRowView)["Inst"].ToString());
                if (tmp != null)
                    openDesc(tmp);
            }
            else
            {
                Item tmp = GiftsSupp.getItemInfo((grdGifts.SelectedItem as DataRowView)["Item"].ToString());
                if (tmp != null)
                    openDesc(tmp);
            }
        }

        private void FillGiftedDataGrid() // Used to Fill the Data Grid with items information (Favourite, Name, Descr, Year, Theme, Folder, Peer, Sponsor) 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.GIFT_INST 1";

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
