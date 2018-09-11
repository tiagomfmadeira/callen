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
    /// Interaction logic for winDelete.xaml
    /// </summary>
    public partial class winDelete : Window
    {
        bool deleted;
        String instance_id, instance_name;

        public winDelete(String inst_id, String inst_name)
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

            deleted = false;
            instance_id = inst_id;
            instance_name = inst_name;
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

        private void btn_yes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.REMOVE_INST @InstID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramInst = new SqlParameter();
                paramInst.ParameterName = "@InstID";
                paramInst.Value = instance_id;
                cmd.Parameters.Add(paramInst);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            deleted = true;

            winNotification noti = new winNotification("Item Deleted", instance_id + " - " + instance_name , "foi eliminado com sucesso");
            noti.Show();

            this.Close();
        }

        public bool wasDeleted()
        {
            return deleted;
        }

        private void btn_no_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}