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

using System.Diagnostics;

using System.Text.RegularExpressions;

using System.Data;
using System.Data.SqlClient;

using Microsoft.Win32;

using Callen.Windows.Forms;

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for winAddItem.xaml
    /// </summary>
    public partial class winAddItem : Window
    {
        public bool inserted; // tell if a item was inserted

        public winAddItem()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            Window parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized; // Maximize window and close border
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            fillFolderCombo();
            fillSponsorCombo();
            fillPeerCombo();

            inserted = false;
        }

        public bool getInserted()
        {
            return inserted;
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
                    ft.Add(new Folders { folder = row["Code"].ToString(), theme = row["Theme_Descr"].ToString(),id = row["Arquive_ID"].ToString()});
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

                string Get_Data = "SELECT Entity_Name AS Nome, Sponsor_ID as ID "
                                 +"FROM G_Callen.ENTITY "
                                 +"INNER JOIN G_Callen.SPONSOR "
                                 +"on Entity_ID = Sponsor_ID";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
                sda.Fill(dt);

                List<Entities> ft = new List<Entities>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Entities { name = row["Nome"].ToString(),id = row["ID"].ToString()});
                }

                combo_sponsor.ItemsSource = ft;
                combo_sponsor.DisplayMemberPath = "name";
                combo_sponsor.SelectedValuePath = "id";

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

                string Get_Data = "SELECT Entity_Name AS Nome, Peer_ID AS ID "
                                 +"FROM G_Callen.ENTITY "
                                 +"INNER JOIN G_Callen.PEER "
                                 +"on Entity_ID = Peer_ID";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
                sda.Fill(dt);

                List<Entities> ft = new List<Entities>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Entities { name = row["Nome"].ToString(), id = row["ID"].ToString() });
                }

                combo_peer.ItemsSource = ft;
                combo_peer.DisplayMemberPath = "name";
                combo_peer.SelectedValuePath = "id";

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
                try
                {
                    ImageSource src = new BitmapImage(new Uri(op.FileName));

                    if (src.Width < 127)
                    {
                        if (src.Height < 90)
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
                        if (src.Height < 90)
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
                catch(Exception ee)
                {
                    MessageBox.Show("Ficheiro " + op.FileName + " não pode ser aberto");
                    Debug.WriteLine("File Error: " + ee.ToString());
                }
            }
        }

        private void btn_insert_Click(object sender, RoutedEventArgs e) // inserts information in data base 
        {
            if (string.IsNullOrEmpty(name_box.Text.ToString()) || (combo_sponsor.SelectedItem == null)
                        || string.IsNullOrEmpty(desc_box.Text.ToString()) || string.IsNullOrEmpty(year_box.Text.ToString())
                            || (combo_folder.SelectedItem == null) || (combo_peer.SelectedItem == null))
            {
                MessageBox.Show("Necessita de nome, Patrocinador,Fornecedor, Descrição, Ano e Pasta");
                return;
            }

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_Callen.ADD_ITEM @Name, @Sponsor, @Peer, @Desc, @Year, @Folder, @Other, @Img_Path";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Name";
                paramName.Value = name_box.Text.ToString();
                cmd.Parameters.Add(paramName);

                SqlParameter paramSpon = new SqlParameter();
                paramSpon.ParameterName = "@Sponsor";
                paramSpon.Value = combo_sponsor.SelectedValue.ToString();
                cmd.Parameters.Add(paramSpon);

                SqlParameter paramPeer = new SqlParameter();
                paramPeer.ParameterName = "@Peer";
                paramPeer.Value = combo_peer.SelectedValue.ToString();
                cmd.Parameters.Add(paramPeer);

                SqlParameter paramDesc = new SqlParameter();
                paramDesc.ParameterName = "@Desc";
                paramDesc.Value = desc_box.Text.ToString();
                cmd.Parameters.Add(paramDesc);

                SqlParameter paramYear = new SqlParameter();
                paramYear.ParameterName = "@Year";
                paramYear.Value = year_box.Text.ToString();
                cmd.Parameters.Add(paramYear);

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@Folder";
                paramFolder.Value = (combo_folder.SelectedItem as Folders).id.ToString();
                cmd.Parameters.Add(paramFolder);

                // Other
                SqlParameter paramOther = new SqlParameter();
                paramOther.ParameterName = "@Other";
                if (string.IsNullOrEmpty(name_box.Text.ToString()))
                    paramOther.Value = "";
                else
                    paramOther.Value = other_box.Text.ToString();
                cmd.Parameters.Add(paramOther);

                // Image Path
                SqlParameter paramImg = new SqlParameter();
                paramImg.ParameterName = "@Img_Path";
                if (img.Source != null) // Theres an img
                {
                    var img_path = "C:\\Callen_Pics\\Instance_"; // gets filled in database trigger
                    paramImg.Value = img_path;
                }
                else
                {
                    paramImg.Value = "";
                }
                cmd.Parameters.Add(paramImg);

                // Execute
                var inst_id = cmd.ExecuteScalar();

                if (img.Source != null) // Theres an img
                {
                    var filename = img.Source.ToString().Substring(img.Source.ToString().LastIndexOf("///") + 3);
                    System.IO.File.Copy(filename, "C:\\Callen_Pics\\Instance_" + inst_id.ToString() + ".jpeg");
                }

                thisConnection.Close();
             }
             catch (Exception ee)
             {
                 MessageBox.Show(ee.ToString());

             }
            inserted = true;
            this.Close();
        }

        private void combo_folder_SelectionChanged(object sender, SelectionChangedEventArgs e) // changes theme text box when folder is selected  
        {
            if (combo_folder.SelectedItem != null)
                text_theme.Text = combo_folder.SelectedValue.ToString();
            else
                text_theme.Text = "";
        }

        private void btn_add_sponsor_Click(object sender, RoutedEventArgs e)
        {
            winAddSponsor popAddSpon = new winAddSponsor();
            popAddSpon.Owner = this;
            this.Opacity = 0.85;
            popAddSpon.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
            fillSponsorCombo();
        }

        private void btn_add_peer_Click(object sender, RoutedEventArgs e)
        {
            winAddPeer popAddPerr = new winAddPeer();
            popAddPerr.Owner = this;
            this.Opacity = 0.85;
            popAddPerr.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
            fillPeerCombo();
        }

        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            winAddFolder popAddFol = new winAddFolder();
            popAddFol.Owner = this;
            this.Opacity = 0.85;
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
