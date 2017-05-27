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

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for Entity.xaml
    /// </summary>
    public partial class Entity : UserControl
    {
        public Entity()
        {
            InitializeComponent();

            peer_toggle.IsChecked = true;
            FillDataGridPeer();
        }

        private void FillDataGridPeer() // Fills data grid with peers information 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Entity_ID AS ID, Entity_Name AS Nome, Email, Phone AS Telefone, QuantityOffered as Ofertas "
                                 +"FROM G_Callen.ENTITY "
                                 +"INNER JOIN G_Callen.PEER "
                                 +"on Entity_ID = Peer_ID";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Peer");
                sda.Fill(dt);

                grdColec.ItemsSource = dt.DefaultView;
            }
            catch
            {
                MessageBox.Show("db error");
            }
        }

        private void FillDataGridSponsor() // Fills data grid with sponsor information 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Entity_ID AS ID, Entity_Name AS Nome, Email, Phone AS Telefone, Website "
                                 +"FROM G_Callen.ENTITY "
                                 +"INNER JOIN G_Callen.SPONSOR "
                                 +"on Entity_ID = Sponsor_ID";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Sponsor");
                sda.Fill(dt);

                grdColec.ItemsSource = dt.DefaultView;
            }
            catch
            {
                MessageBox.Show("db error");
            }
        }

        private void peer_toggle_Click(object sender, RoutedEventArgs e)
        {
            if(peer_toggle.IsChecked == true)
            {
                FillDataGridPeer();
                sponsor_toggle.IsChecked = false;
                return;
            }
            peer_toggle.IsChecked = true;
        }

        private void sponsor_toggle_Click(object sender, RoutedEventArgs e)
        {
            if (sponsor_toggle.IsChecked == true)
            {
                FillDataGridSponsor();
                peer_toggle.IsChecked = false;
                return;
            }
            sponsor_toggle.IsChecked = true;
        }
    }
}
