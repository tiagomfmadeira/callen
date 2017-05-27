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
using System.Windows.Shapes;

using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace Callen.Windows.Forms
{
    /// <summary>
    /// Interaction logic for winAddPeer.xaml
    /// </summary>
    public partial class winAddPeer : Window
    {
        public winAddPeer()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            Window parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btn_add_address_Click(object sender, RoutedEventArgs e)
        {
            main_border.Height = 348;
            btn_add_address.Visibility = Visibility.Hidden;
            Canvas.SetLeft(btn_submit, 368);
            Canvas.SetTop(btn_submit, 501);

            street_dock.Visibility = Visibility.Visible;
            street_city.Visibility = Visibility.Visible;
            street_state.Visibility = Visibility.Visible;
            street_country.Visibility = Visibility.Visible;
            street_postal.Visibility = Visibility.Visible;
            divider.Visibility = Visibility.Visible;

        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(name_box.Text.ToString()))
            {
                MessageBox.Show("Necessita de ambos os atributos");
                return;
            }

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "G_Callen.CREATE_PEER @Name, @Email, @Phone";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Name";
                param.Value = name_box.Text.ToString();
                cmd.Parameters.Add(param);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@Email";
                param2.Value = mail_box.Text.ToString();
                cmd.Parameters.Add(param2);

                SqlParameter param3 = new SqlParameter();
                param3.ParameterName = "@Phone";
                param3.Value = phone_box.Text.ToString();
                cmd.Parameters.Add(param3);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            this.Close();
        }

        private void add_Address(String entityID)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CREATE_ADDRESS @Entity, @Street, @City, @State, @Country, @PostalCode";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramEntity = new SqlParameter();
                paramEntity.ParameterName = "@Entity";
                paramEntity.Value = street_box.Text.ToString();
                cmd.Parameters.Add(paramEntity);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Street";
                param.Value = street_box.Text.ToString();
                cmd.Parameters.Add(param);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@City";
                param2.Value = city_box.Text.ToString();
                cmd.Parameters.Add(param2);

                SqlParameter param3 = new SqlParameter();
                param3.ParameterName = "@State";
                param3.Value = state_box.Text.ToString();
                cmd.Parameters.Add(param3);

                SqlParameter param4 = new SqlParameter();
                param4.ParameterName = "@Country";
                param4.Value = country_box.Text.ToString();
                cmd.Parameters.Add(param4);

                SqlParameter param5 = new SqlParameter();
                param5.ParameterName = "@PostalCode";
                param5.Value = postal_box.Text.ToString();
                cmd.Parameters.Add(param5);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^(0-9)0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
