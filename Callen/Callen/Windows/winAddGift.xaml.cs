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
            fillSponsorCombo();
            fillPeerCombo();
            fillSeriesCombo();
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

                string Get_Data = "SELECT * FROM G_Callen.ITEMS_VIEW";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("items");
                sda.Fill(dt);

                List<Item> items = new List<Item>();
                foreach (DataRow row in dt.Rows)
                {
                    items.Add(new Item(row["Item_Name"].ToString(), row["Item_ID"].ToString(), row["Item_Descr"].ToString(), row["Item_Year"].ToString(),
                                                row["Sponsor"].ToString(), row["Other"].ToString(),row["Series"].ToString(),row["NumberInSeries"].ToString()));
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

                string Get_Data = "SELECT * FROM G_Callen.SPONSOR_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
                sda.Fill(dt);

                List<Entities> ft = new List<Entities>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Entities { name = row["name"].ToString(), id = row["ID"].ToString() });
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

                string Get_Data = "SELECT * FROM G_Callen.PEER_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
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

                string Get_Data = "SELECT * FROM G_Callen.SERIES_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("series");
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

        private void btn_insert_Click(object sender, RoutedEventArgs e) // inserts information in data base 
        {
            if (combo_item.SelectedIndex > -1)
                insert_gift_old();
            else
                insert_gift_new();
        }

        private void insert_gift_new()
        {
            if (string.IsNullOrEmpty(name_box.Text.ToString()) || (combo_sponsor.SelectedItem == null)
                       ||  (combo_peer.SelectedItem == null))
           {
               MessageBox.Show("Necessita de Nome, Patrocinador e Destinatário");
               return;
           }

           try
           {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_Callen.ADD_GIFT @Name, @Sponsor, @Desc, @Year, @Other, @Series, @SeriesNum, @Dest";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

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

                SqlParameter paramPeer = new SqlParameter();
                paramPeer.ParameterName = "@Dest";
                paramPeer.Value = combo_peer.SelectedValue.ToString();
                cmd.Parameters.Add(paramPeer);

                var inst_id = cmd.ExecuteScalar();

                thisConnection.Close();
           }
           catch (Exception ee)
           {
               MessageBox.Show(ee.ToString());

           }
           this.Close();
        }

        private void insert_gift_old()
        {
            if (combo_peer.SelectedItem == null)
            {
                MessageBox.Show("Necessita de Item e Destinatário");
                return;
            }

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_Callen.ADD_GIFT_WITH_ITEM @ItemID, @Dest";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@ItemID";
                paramName.Value = combo_item.SelectedValue.ToString();
                cmd.Parameters.Add(paramName);

                SqlParameter paramPeer = new SqlParameter();
                paramPeer.ParameterName = "@Dest";
                paramPeer.Value = combo_peer.SelectedValue.ToString();
                cmd.Parameters.Add(paramPeer);

                var inst_id = cmd.ExecuteScalar();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());

            }
            this.Close();
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
                series_num_box.Text = it.getSeriesNumber();

                combo_sponsor.SelectedIndex = -1;
                foreach (Entities sponsor in combo_sponsor.Items)
                {
                    if (sponsor.id == it.getSponsor())
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
            winAddPeer popAddSpon = new winAddPeer();
            popAddSpon.Owner = this;
            this.Opacity = 0.85;
            popAddSpon.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
            fillPeerCombo();
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
