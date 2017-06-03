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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for Search.xaml
    /// </summary>
    public partial class Search : UserControl
    {
        public Search()
        {
            InitializeComponent();
            btn_pro_mode.IsChecked = true;
            Switcher.Switch(this.Search_mode, new proSearch());
        }

        private void btn_pic_mode_Click(object sender, RoutedEventArgs e)
        {
            if (btn_pic_mode.IsChecked == true)
            {
                Switcher.Switch(this.Search_mode, new picSearch());
                btn_pro_mode.IsChecked = false;
                return;
            }
            btn_pic_mode.IsChecked = true;
        }

        private void btn_pro_mode_Click(object sender, RoutedEventArgs e)
        {
            if (btn_pro_mode.IsChecked == true)
            {
                Switcher.Switch(this.Search_mode, new proSearch());
                btn_pic_mode.IsChecked = false;
                return;
            }
            btn_pro_mode.IsChecked = true;
        }

        private void advanceSearch_Click(object sender, RoutedEventArgs e) // rotates arrow in search bar when clicked 
        {
            /*if(advanceSearch.IsChecked == true)
            {
                RotateTransform rotateTransform = new RotateTransform(90,advanceSearch.Height/2,advanceSearch.Width/2);
                advanceSearch.RenderTransform = rotateTransform;
            }
            else
            {
                RotateTransform rotateTransform = new RotateTransform(0, advanceSearch.Height/2, advanceSearch.Width/2);
                advanceSearch.RenderTransform = rotateTransform;
            }*/
        }

        private void btn_adv_search_Click(object sender, RoutedEventArgs e)
        {
            // if all textbox are empty, don't search
            if (String.IsNullOrEmpty(id_box.Text) && String.IsNullOrEmpty(name_box.Text) && String.IsNullOrEmpty(desc_box.Text) && String.IsNullOrEmpty(year_box.Text)
                 && String.IsNullOrEmpty(note_box.Text) && String.IsNullOrEmpty(theme_box.Text) && String.IsNullOrEmpty(folder_box.Text) && String.IsNullOrEmpty(peer_box.Text)
                  && String.IsNullOrEmpty(sponsor_box.Text))
                return;
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "";
                if (btn_pic_mode.IsChecked == false) { 
                    Get_Data = "EXEC G_Callen.SEARCH_ITEMS_PRO_VIEW @Item_ID, @Item_Name, @Item_Desc, @Item_Year, "
                                + "@Item_Note, @Item_Theme, @Item_Folder, @Item_Peer, @Item_Sponsor;";
                }
                else {
                    Get_Data = "EXEC G_Callen.SEARCH_ITEMS_PIC @Item_ID, @Item_Name, @Item_Desc, @Item_Year, "
                                                                + "@Item_Note, @Item_Theme, @Item_Folder, @Item_Peer, @Item_Sponsor;";
                }

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlParameter paramID = new SqlParameter();
                paramID.ParameterName = "@Item_id";
                paramID.Value = id_box.Text;
                cmd.Parameters.Add(paramID);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Item_Name";
                paramName.Value = name_box.Text;
                cmd.Parameters.Add(paramName);

                SqlParameter paramDesc = new SqlParameter();
                paramDesc.ParameterName = "@Item_Desc";
                paramDesc.Value = desc_box.Text;
                cmd.Parameters.Add(paramDesc);

                SqlParameter paramYear = new SqlParameter();
                paramYear.ParameterName = "@Item_Year";
                paramYear.Value = year_box.Text;
                cmd.Parameters.Add(paramYear);

                SqlParameter paramNote = new SqlParameter();
                paramNote.ParameterName = "@Item_Note";
                paramNote.Value = note_box.Text;
                cmd.Parameters.Add(paramNote);

                SqlParameter paramTheme = new SqlParameter();
                paramTheme.ParameterName = "@Item_Theme";
                paramTheme.Value = theme_box.Text;
                cmd.Parameters.Add(paramTheme);

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@Item_Folder";
                paramFolder.Value = folder_box.Text;
                cmd.Parameters.Add(paramFolder);

                SqlParameter paramPeer = new SqlParameter();
                paramPeer.ParameterName = "@Item_Peer";
                paramPeer.Value = peer_box.Text;
                cmd.Parameters.Add(paramPeer);

                SqlParameter paramSponsor = new SqlParameter();
                paramSponsor.ParameterName = "@Item_Sponsor";
                paramSponsor.Value = sponsor_box.Text;
                cmd.Parameters.Add(paramSponsor);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("INST");
                sda.Fill(dt);

                if (btn_pic_mode.IsChecked == false)
                    Switcher.Switch(this.Search_mode, new proSearch(dt));
                else
                    Switcher.Switch(this.Search_mode, new picSearch(dt));

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^(0-9)0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
