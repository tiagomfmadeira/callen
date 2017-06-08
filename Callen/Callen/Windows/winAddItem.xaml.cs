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
        private bool inserted; // tell if a item was inserted
        private bool oldItem;

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

            fillItemCombo();
            fillFolderCombo();
            fillSponsorCombo();
            fillPeerCombo();
            fillSeriesCombo();

            inserted = false;
            oldItem = false;
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

        private void fillItemCombo()
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.ITEMS_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Items");
                sda.Fill(dt);

                List<Item> items = new List<Item>();
                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new Item(row["Item_Name"].ToString(),row["Item_ID"].ToString(),row["Item_Descr"].ToString(),row["Item_Year"].ToString(),
                                                row["Sponsor"].ToString(),row["Other"].ToString(),row["Series"].ToString(),row["NumberInSeries"].ToString()));
                }

                combo_item.ItemsSource = items;
                combo_item.DisplayMemberPath = "Name";
                combo_item.SelectedValuePath = "ID";

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());

            }
        }

        public void fillFolderCombo() // gets info about the folder and sets the respective combo box 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.FOLDER_INFO";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Folder");
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

                string Get_Data = "EXEC CALLEN.FILL_SPONSOR_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Sponsor");
                sda.Fill(dt);

                List<Entities> ft = new List<Entities>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Entities { name = row["name"].ToString(),id = row["ID"].ToString()});
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

        public void fillPeerCombo() // gets info about the peer and sets the respective combo box 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.FILL_PEER_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Peer");
                sda.Fill(dt);

                List<Entities> ft = new List<Entities>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Entities { name = row["name"].ToString(), id = row["ID"].ToString() });
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

        public void fillSeriesCombo() // gets info about the the series and sets the respective combo box 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC CALLEN.FILL_SERIES_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Series");
                sda.Fill(dt);

                List<Entities> ft = new List<Entities>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Entities { name = row["name"].ToString(), id = row["ID"].ToString() });
                }

                combo_series.ItemsSource = ft;
                combo_series.DisplayMemberPath = "name";
                combo_series.SelectedValuePath = "id";

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
                            || (combo_folder.SelectedItem == null))
            {
                MessageBox.Show("Necessita de nome, Patrocinador, Descrição, Ano e Pasta");
                return;
            }

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "";

                if (oldItem)
                    Get_Data = "EXEC CALLEN.ADD_INST_WITH_ITEM @ItemID, @Peer, @Folder, @Note, @Img_Path";
                else
                    Get_Data = "EXEC CALLEN.ADD_INST @Name, @Sponsor, @Peer, @Desc, @Year, @Series, @SeriesNum, @Folder, @Other, @Note, @Img_Path";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                if (oldItem)
                {
                    SqlParameter paramItem = new SqlParameter();
                    paramItem.ParameterName = "@ItemID";
                    paramItem.Value = combo_item.SelectedValue.ToString();
                    cmd.Parameters.Add(paramItem);
                }
                else
                { 
                    SqlParameter paramName = new SqlParameter();
                    paramName.ParameterName = "@Name";
                    paramName.Value = name_box.Text.ToString();
                    cmd.Parameters.Add(paramName);

                    SqlParameter paramSpon = new SqlParameter();
                    paramSpon.ParameterName = "@Sponsor";
                    paramSpon.Value = combo_sponsor.SelectedValue.ToString();
                    cmd.Parameters.Add(paramSpon);

                    SqlParameter paramDesc = new SqlParameter();
                    paramDesc.ParameterName = "@Desc";
                    paramDesc.Value = desc_box.Text.ToString();
                    cmd.Parameters.Add(paramDesc);

                    SqlParameter paramYear = new SqlParameter();
                    paramYear.ParameterName = "@Year";
                    paramYear.Value = year_box.Text.ToString();
                    cmd.Parameters.Add(paramYear);

                    SqlParameter paramOther = new SqlParameter();
                    paramOther.ParameterName = "@Other";
                    paramOther.Value = other_box.Text.ToString();
                    cmd.Parameters.Add(paramOther);

                    SqlParameter paramSeries = new SqlParameter();
                    paramSeries.ParameterName = "@Series";
                    if(combo_series.SelectedIndex > -1)
                        paramSeries.Value = combo_series.SelectedValue.ToString();
                    else
                        paramSeries.Value = -1;
                    cmd.Parameters.Add(paramSeries);

                    SqlParameter paramSeriesNum = new SqlParameter();
                    paramSeriesNum.ParameterName = "@SeriesNum";
                    paramSeriesNum.Value = series_num_box.Text.ToString();
                    cmd.Parameters.Add(paramSeriesNum);
                }

                SqlParameter paramPeer = new SqlParameter();
                paramPeer.ParameterName = "@Peer";
                if (combo_peer.SelectedIndex > -1)
                    paramPeer.Value = combo_peer.SelectedValue.ToString();
                else
                    paramPeer.Value = -1;
                cmd.Parameters.Add(paramPeer);

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@Folder";
                paramFolder.Value = (combo_folder.SelectedItem as Folders).id.ToString();
                cmd.Parameters.Add(paramFolder);

                SqlParameter paramNote = new SqlParameter();
                paramNote.ParameterName = "@Note";
                paramNote.Value = note_box.Text.ToString();
                cmd.Parameters.Add(paramNote);

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

        private void combo_item_SelectionChanged(object sender, SelectionChangedEventArgs e) // changes theme text box when folder is selected  
        {
            if (combo_item.SelectedItem != null)
            {
                oldItem = true;

                Item it = (Item) combo_item.SelectedItem;

                name_box.Text = it.getName();
                desc_box.Text = it.getDesc();
                other_box.Text = it.getOther();
                year_box.Text = it.getYear();
                series_num_box.Text = it.getSeriesNumber();

                combo_sponsor.SelectedIndex = -1;
                foreach (Entities sponsor in combo_sponsor.Items)
                {
                    if(sponsor.id == it.getSponsor())
                    {
                        combo_sponsor.SelectedItem = sponsor;
                        break;
                    }
                }

                combo_series.SelectedIndex = -1; // reset
                foreach (Entities serie in combo_series.Items)
                {
                    if (serie.id == it.getSeries())
                    {
                        combo_series.SelectedItem = serie;
                        break;
                    }
                }

                name_box.IsEnabled = false;
                desc_box.IsEnabled = false;
                other_box.IsEnabled = false;
                year_box.IsEnabled = false;
                combo_sponsor.IsEnabled = false;
                btn_add_sponsor.IsEnabled = false;
                combo_series.IsEnabled = false;
                btn_add_series.IsEnabled = false;
                series_num_box.IsEnabled = false;
            }
            else
            {
                oldItem = false;

                name_box.IsEnabled = true;
                desc_box.IsEnabled = true;
                other_box.IsEnabled = true;
                year_box.IsEnabled = true;
                combo_sponsor.IsEnabled = true;
                btn_add_sponsor.IsEnabled = true;
                combo_series.IsEnabled = true;
                btn_add_series.IsEnabled = true;
            }
        }

        private void combo_series_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combo_series.SelectedIndex > -1)
                series_num_box.IsEnabled = true;
            else
                series_num_box.IsEnabled = false;
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

        private void btn_add_series_Click(object sender, RoutedEventArgs e)
        {
            winAddSeries popAddSe = new winAddSeries();
            popAddSe.Owner = this;
            this.Opacity = 0.85;
            popAddSe.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
            fillSeriesCombo();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
