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

using System.Text.RegularExpressions;

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

                string Get_Data = "EXEC CALLEN.FILL_PEER;";

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

                string Get_Data = "EXEC CALLEN.FILL_SPONSOR";

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

        private void btn_adv_search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "";
                
                if(peer_toggle.IsChecked == true)
                    Get_Data = "EXEC CALLEN.SEARCH_PEER @PeerID, @PeerName, @PeerEmail, @PeerPhone, @PeerStreet, @PeerCity, @PeerState, @PeerCountry, @PeerPostalCode;";
                else
                    Get_Data = "EXEC CALLEN.SEARCH_SPONSOR @PeerID, @PeerName, @PeerEmail, @PeerPhone, @PeerStreet, @PeerCity, @PeerState, @PeerCountry, @PeerPostalCode;";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlParameter paramID = new SqlParameter();
                paramID.ParameterName = "@PeerID";
                paramID.Value = id_box.Text;
                cmd.Parameters.Add(paramID);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@PeerName";
                paramName.Value = name_box.Text;
                cmd.Parameters.Add(paramName);

                SqlParameter paramEmail = new SqlParameter();
                paramEmail.ParameterName = "@PeerEmail";
                paramEmail.Value = email_box.Text;
                cmd.Parameters.Add(paramEmail);

                SqlParameter paramPhone = new SqlParameter();
                paramPhone.ParameterName = "@PeerPhone";
                paramPhone.Value = phone_box.Text;
                cmd.Parameters.Add(paramPhone);

                SqlParameter paramStreet = new SqlParameter();
                paramStreet.ParameterName = "@PeerStreet";
                paramStreet.Value = street_box.Text;
                cmd.Parameters.Add(paramStreet);

                SqlParameter paramCity = new SqlParameter();
                paramCity.ParameterName = "@PeerCity";
                paramCity.Value = city_box.Text;
                cmd.Parameters.Add(paramCity);

                SqlParameter paramState = new SqlParameter();
                paramState.ParameterName = "@PeerState";
                paramState.Value = state_box.Text;
                cmd.Parameters.Add(paramState);

                SqlParameter paramCountry = new SqlParameter();
                paramCountry.ParameterName = "@PeerCountry";
                paramCountry.Value = country_box.Text;
                cmd.Parameters.Add(paramCountry);

                SqlParameter paramPC = new SqlParameter();
                paramPC.ParameterName = "@PeerPostalCode";
                paramPC.Value = postal_code_box.Text;
                cmd.Parameters.Add(paramPC);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Entities");
                sda.Fill(dt);

                grdColec.ItemsSource = dt.DefaultView;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            
        }

        private void advanceSearch_Click(object sender, RoutedEventArgs e) // rotates arrow in search bar when clicked 
        {

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^(0-9)0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
