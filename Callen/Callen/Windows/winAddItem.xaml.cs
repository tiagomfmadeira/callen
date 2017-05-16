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

using System.Data;
using System.Data.SqlClient;
using Microsoft.Win32;
using Callen.Windows.Forms;
using System.Text.RegularExpressions;

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for winAddItem.xaml
    /// </summary>
    public partial class winAddItem : Window
    {

        public winAddItem()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
            fillFolderCombo();
            fillSponsorCombo();
            fillPeerCombo();
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                closeWin();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            closeWin();
        }

        private void closeWin()
        {
            this.Close();
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            win.Opacity = 1;
        }

        public void fillFolderCombo() // gets info about the folder and sets the respective combo box 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT * "
                                 + "FROM G_Callen.ARQUIVE ";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
                sda.Fill(dt);

                List<Folders> ft = new List<Folders>();
                foreach(DataRow row in dt.Rows)
                {
                    ft.Add(new Folders { folder = row["Code"].ToString(), theme = row["Theme_Descr"].ToString() });
                }

                combo_folder.ItemsSource = ft;
                combo_folder.DisplayMemberPath = "folder";
                combo_folder.SelectedValuePath = "theme";

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                
            }
        }

        public void fillSponsorCombo() // gets info about the the sponsor and sets the respective combo box 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Entity_Name AS Nome "
                                 +"FROM G_Callen.ENTITY "
                                 +"INNER JOIN G_Callen.SPONSOR "
                                 +"on Entity_ID = Sponsor_ID";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
                sda.Fill(dt);

                List<String> ft = new List<String>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(row["Nome"].ToString());
                }

                combo_sponsor.ItemsSource = ft;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());

            }
        }

        public void fillPeerCombo() // gets info about the peer and sets the respoective combo box 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Entity_Name AS Nome "
                                 +"FROM G_Callen.ENTITY "
                                 +"INNER JOIN G_Callen.PEER "
                                 +"on Entity_ID = Peer_ID";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
                sda.Fill(dt);

                List<String> ft = new List<String>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(row["Nome"].ToString());
                }

                combo_peer.ItemsSource = ft;

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());

            }
        }

        public void btn_upload_Click(object sender, RoutedEventArgs e) // Uploads a image from the users system and sets image to respective border  
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                ImageSource src = new BitmapImage(new Uri(op.FileName));
                if (src.Width < 127)
                {
                    if(src.Height < 90)
                    {
                        img_border.Height = src.Height;
                        img_border.Width = src.Width;
                    }
                    else
                    {
                        img_border.Height = 90;
                    }
                }
                else
                {
                    if(src.Height < 90)
                    {
                        img_border.Height = src.Height;
                    }
                    else
                    {
                        img_border.Width = 127;
                        img_border.Height = 90;
                    }
                }

                img_border.Visibility = Visibility.Visible;
                img.Source = src;
            }
        }

        private void btn_insert_Click(object sender, RoutedEventArgs e) // inserts information in data base 
        {

        }

        private void combo_folder_SelectionChanged(object sender, SelectionChangedEventArgs e) // changes theme text box when folder is selected  
        {
            text_theme.Text = combo_folder.SelectedValue.ToString();
        }

        private void btn_add_sponsor_Click(object sender, RoutedEventArgs e)
        {
            winAddSponsor popAddSpon = new winAddSponsor();
            popAddSpon.Owner = this;
            this.Opacity = 0.5;
            popAddSpon.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
        }

        private void btn_add_peer_Click(object sender, RoutedEventArgs e)
        {
            winAddPeer popAddPerr = new winAddPeer();
            popAddPerr.Owner = this;
            this.Opacity = 0.5;
            popAddPerr.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
        }

        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            winAddFolder popAddFol = new winAddFolder();
            popAddFol.Owner = this;
            this.Opacity = 0.5;
            popAddFol.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
            fillFolderCombo();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
