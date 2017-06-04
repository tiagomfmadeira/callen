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
    /// Interaction logic for winAddGift.xaml
    /// </summary>
    public partial class winAddGift : Window
    {
        private bool oldItem;
        public winAddGift()
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

            oldItem = false;

            fillItemCombo();
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

                string Get_Data = "SELECT * "
                                 + "FROM G_Callen.ITEM";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
                sda.Fill(dt);

                List<Item> items = new List<Item>();
                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new Item(row["Item_Name"].ToString(), row["Item_ID"].ToString(), row["Item_Descr"].ToString(), row["Item_Year"].ToString(),
                                                row["Sponsor"].ToString(), row["Other"].ToString()));
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

        public void fillSponsorCombo() // gets info about the the sponsor and sets the respective combo box 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Entity_Name AS Nome, Sponsor_ID as ID "
                                 + "FROM G_Callen.ENTITY "
                                 + "INNER JOIN G_Callen.SPONSOR "
                                 + "on Entity_ID = Sponsor_ID";

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

        private void btn_insert_Click(object sender, RoutedEventArgs e) // inserts information in data base 
        {                
            /*if (string.IsNullOrEmpty(name_box.Text.ToString()) || (combo_sponsor.SelectedItem == null)
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
            this.Close();*/
        }

        private void combo_item_SelectionChanged(object sender, SelectionChangedEventArgs e) // changes theme text box when folder is selected  
        {
            if (combo_item.SelectedItem != null)
            {
                oldItem = true;

                Item it = (Item)combo_item.SelectedItem;

                name_box.Text = it.getName();
                desc_box.Text = it.getDesc();
                other_box.Text = it.getOther();
                year_box.Text = it.getYear();
                foreach (Entities sponsor in combo_sponsor.Items)
                {
                    if (sponsor.id == it.getSponsor())
                    {
                        combo_sponsor.SelectedItem = sponsor;
                        break;
                    }
                }

                name_box.IsEnabled = false;
                desc_box.IsEnabled = false;
                other_box.IsEnabled = false;
                year_box.IsEnabled = false;
                combo_sponsor.IsEnabled = false;
                btn_add_sponsor.IsEnabled = false;
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
            }
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
