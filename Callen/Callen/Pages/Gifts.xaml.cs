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

using Callen.Windows;

using System.Text.RegularExpressions;

using System.Data;
using System.Data.SqlClient;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for Gifts.xaml
    /// </summary>
    public partial class Gifts : UserControl
    {
        public Gifts()
        {
            InitializeComponent();
            gifted_toggle.IsChecked = true;
            Switcher.Switch(this.Gift_zone, new Gifted());
        }

        private void btn_gift_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winAddGift popGift = new winAddGift();
            popGift.Owner = win;
            win.Opacity = 0.5;
            popGift.ShowDialog();
            
            win.Opacity = 1;
        }

        private void gifted_toggle_Click(object sender, RoutedEventArgs e)
        {
            if (gifted_toggle.IsChecked == true)
            {
                Switcher.Switch(this.Gift_zone, new Gifted());
                planned_toggle.IsChecked = false;
                return;
            }
            gifted_toggle.IsChecked = true;
        }

        private void planned_toggle_Click(object sender, RoutedEventArgs e)
        {
            if (planned_toggle.IsChecked == true)
            {
                Switcher.Switch(this.Gift_zone, new Planned());
                gifted_toggle.IsChecked = false;
                return;
            }
            planned_toggle.IsChecked = true;
        }

        private void btn_adv_search_Click(object sender, RoutedEventArgs e)
        {
            // if all textbox are empty, don't search
            if (String.IsNullOrEmpty(id_box.Text) && String.IsNullOrEmpty(name_box.Text) && String.IsNullOrEmpty(desc_box.Text) && String.IsNullOrEmpty(year_box.Text)
                 && String.IsNullOrEmpty(note_box.Text) && String.IsNullOrEmpty(theme_box.Text) && String.IsNullOrEmpty(folder_box.Text) && String.IsNullOrEmpty(dest_box.Text)
                  && String.IsNullOrEmpty(sponsor_box.Text))
                return;
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "";

                Get_Data = "EXEC G_CALLEN.SEARCH_GIFTS @InstID, @Item_Name, @Item_Desc, @Item_Year, "
                            + "@Item_Note, @Item_Theme, @Item_Folder, @Item_Dest, @Item_Sponsor,@Item_Other,@Offer;";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlParameter paramID = new SqlParameter();
                paramID.ParameterName = "@InstID";
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
                paramPeer.ParameterName = "@Item_Dest";
                paramPeer.Value = dest_box.Text;
                cmd.Parameters.Add(paramPeer);

                SqlParameter paramSponsor = new SqlParameter();
                paramSponsor.ParameterName = "@Item_Sponsor";
                paramSponsor.Value = sponsor_box.Text;
                cmd.Parameters.Add(paramSponsor);

                SqlParameter paramOther = new SqlParameter();
                paramOther.ParameterName = "@Item_Other";
                paramOther.Value = other_box.Text;
                cmd.Parameters.Add(paramOther);

                SqlParameter paramOffer = new SqlParameter();
                paramOffer.ParameterName = "@Offer";
                if (gifted_toggle.IsChecked == true)
                    paramOffer.Value = 1;
                else
                    paramOffer.Value = 0;
                cmd.Parameters.Add(paramOffer);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("INST");
                sda.Fill(dt);

                if (gifted_toggle.IsChecked == true)
                    Switcher.Switch(this.Gift_zone, new Gifted(dt));
                else
                    Switcher.Switch(this.Gift_zone, new Planned(dt));

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void advanceSearch_Click(object sender, RoutedEventArgs e) // rotates arrow in search bar when clicked 
        {

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^(0-9)0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
