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

using Callen.Windows.Other;

using System.Data;
using System.Data.SqlClient;

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for winDesc.xaml
    /// </summary>
    public partial class winDesc : Window
    {
        int id;
        public winDesc()
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

        public winDesc(Item it)  // sets the text Boxes with information from an given Item 
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            Window parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized) {
                this.WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            id = Int32.Parse(it.getID());

            item_name.Text = it.getName();
            item_year.Text = it.getYear();
            item_sponsor.Text = it.getSponsor();
            item_peer.Text = it.getPeer();
            item_folder.Text = it.getFolder();
            item_theme.Text = it.getTheme();
            item_desc.Text = it.getDesc();

            if (it.getOther() != "")
                item_other.Text = it.getOther();

            if (it.getImagePath() != "")
            {
                try
                {
                    img.Source = new BitmapImage(new Uri(it.getImagePath() + ".jpeg", UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    MessageBox.Show("File not found : " + it.getImagePath());
                }
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

        private void btn_img_Click(object sender, RoutedEventArgs e)
        {
            winZoomImage popZoomImg = new winZoomImage(img.Source);
            popZoomImg.Owner = this;
            popZoomImg.ShowDialog();
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            if (item_name.IsEnabled)
            {
                updateInfo();
                item_name.IsEnabled = false;
                item_year.IsEnabled = false;
                item_other.IsEnabled = false;
                item_desc.IsEnabled = false;
            }
            else
            {
                item_name.IsEnabled = true;
                item_year.IsEnabled = true;
                item_other.IsEnabled = true;
                item_desc.IsEnabled = true;
            }
        }

        private void updateInfo() // Needs to check if some value is changed before performing the procedure
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_Callen.UPDATE_INST_INFO @InstID, @ItemName, @ItemYear, @ItemOther, @ItemDesc";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@InstID";
                param.Value = id;
                cmd.Parameters.Add(param);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@ItemName";
                paramName.Value = item_name.Text;
                cmd.Parameters.Add(paramName);

                SqlParameter paramYear = new SqlParameter();
                paramYear.ParameterName = "@ItemYear";
                paramYear.Value = item_year.Text;
                cmd.Parameters.Add(paramYear);

                SqlParameter paramDesc = new SqlParameter();
                paramDesc.ParameterName = "@ItemOther";
                paramDesc.Value = item_other.Text;
                cmd.Parameters.Add(paramDesc);

                SqlParameter paramOth = new SqlParameter();
                paramOth.ParameterName = "@ItemDesc";
                paramOth.Value = item_desc.Text;
                cmd.Parameters.Add(paramOth);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            String inst = id + " - " + item_name.Text;

            if (!(App.Current.Properties["PrintList"] as List<String>).Contains(inst))
                (App.Current.Properties["PrintList"] as List<String>).Add(inst);
        }
    }
}
 