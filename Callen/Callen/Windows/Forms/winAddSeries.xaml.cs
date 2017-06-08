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

using System.Data.SqlClient;

namespace Callen.Windows.Forms
{
    /// <summary>
    /// Interaction logic for winAddSeries.xaml
    /// </summary>
    public partial class winAddSeries : Window
    {
        public winAddSeries()
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

        private void btn_add_series_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(name_box.Text.ToString()))
            {
                MessageBox.Show("Necessita de nome");
                return;
            }

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.CREATE_SERIES @Name, @Desc";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Name";
                param.Value = name_box.Text.ToString();
                cmd.Parameters.Add(param);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@Desc";
                param2.Value = desc_box.Text.ToString();
                cmd.Parameters.Add(param2);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            this.Close();
        }
    }
}
