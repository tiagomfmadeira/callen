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

                string Get_Data = "EXEC G_Callen.ADD_PEER @Name, @Email, @Phone, @Street, @City, @State,@Country, @PostalCode;";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Name";
                paramName.Value = name_box.Text.ToString();
                cmd.Parameters.Add(paramName);

                SqlParameter paramEmail = new SqlParameter();
                paramEmail.ParameterName = "@Email";
                paramEmail.Value = mail_box.Text.ToString();
                cmd.Parameters.Add(paramEmail);

                SqlParameter paramPhone = new SqlParameter();
                paramPhone.ParameterName = "@Phone";
                paramPhone.Value = phone_box.Text.ToString();
                cmd.Parameters.Add(paramPhone);

                SqlParameter paramStreet = new SqlParameter();
                paramStreet.ParameterName = "@Street";
                paramStreet.Value = street_box.Text.ToString();
                cmd.Parameters.Add(paramStreet);

                SqlParameter paramCity = new SqlParameter();
                paramCity.ParameterName = "@City";
                paramCity.Value = city_box.Text.ToString();
                cmd.Parameters.Add(paramCity);

                SqlParameter paramState = new SqlParameter();
                paramState.ParameterName = "@State";
                paramState.Value = state_box.Text.ToString();
                cmd.Parameters.Add(paramState);

                SqlParameter paramCountry = new SqlParameter();
                paramCountry.ParameterName = "@Country";
                paramCountry.Value = country_box.Text.ToString();
                cmd.Parameters.Add(paramCountry);

                SqlParameter paramPostalCode = new SqlParameter();
                paramPostalCode.ParameterName = "@PostalCode";
                paramPostalCode.Value = postal_box.Text.ToString();
                cmd.Parameters.Add(paramPostalCode);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            this.Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^(0-9)0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
