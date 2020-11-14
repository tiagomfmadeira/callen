using System;
using System.Windows;
using System.Windows.Input;


using System.Data.SqlClient;

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

            Window parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }
        }

        public winAddFolder(string code)
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

            wind_name.Content = "Adicionar Tema";
            folder_box.Text = code;
            folder_box.IsEnabled = false;
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

                string Get_Data = "EXEC G_CALLEN.CREATE_FOLDER @Code, @Theme";

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