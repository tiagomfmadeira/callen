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
using System.Data;

namespace Callen.Windows.Forms
{
    /// <summary>
    /// Interaction logic for winAddFolder.xaml
    /// </summary>
    public partial class winAddFolder : Window
    {
        public winAddFolder()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
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

        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(folder_box.Text.ToString()) || string.IsNullOrEmpty(theme_box.Text.ToString()))
            {
                MessageBox.Show("Necessita de ambos os atributos");
                return;
            }
     
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "INSERT INTO G_Callen.ARQUIVE(Code, Theme_Descr) "
                                 +"VALUES(@Code ,@Theme )";

                SqlCommand cmd = new SqlCommand(Get_Data,thisConnection);

                SqlParameter param = new SqlParameter();

                param.ParameterName = "@Code";
                param.Value = folder_box.Text.ToString();
                cmd.Parameters.Add(param);

                SqlParameter param2 = new SqlParameter();
                param2.ParameterName = "@Theme";
                param2.Value = theme_box.Text.ToString();
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