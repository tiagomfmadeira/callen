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
            if(advanceSearch.IsChecked == true)
            {
                RotateTransform rotateTransform = new RotateTransform(90,advanceSearch.Height/2,advanceSearch.Width/2);
                advanceSearch.RenderTransform = rotateTransform;
            }
            else
            {
                RotateTransform rotateTransform = new RotateTransform(0, advanceSearch.Height / 2, advanceSearch.Width / 2);
                advanceSearch.RenderTransform = rotateTransform;
            }

        }

        private void btn_adv_search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(year_box.Text.ToString()))
            {
                return;
            }

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Favorite AS Favorito,Inst_Number AS ID,Item_Name AS Nome,Item_Descr AS Descrição,Item_Year AS Ano ,Theme_Descr AS Tema,Code AS Pasta,Peer_name AS Fornecedor,Entity_Name AS Patrocinador "
                                  + "FROM(SELECT Favorite, Inst_Number, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Entity_Name AS Peer_name, Sponsor "
                                        + "FROM(SELECT Favorite, Inst_Number, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Peer, Sponsor "
                                           + "FROM(SELECT Favorite, Inst_Number, Item_ID, Code, Peer, Theme_Descr "
                                               + "FROM(SELECT Item_ID, Inst_Number, Favorite, Arquive, Peer "
                                                    + "FROM G_Callen.INST) AS IT "
                                                    + "JOIN(SELECT * "
                                                         + "FROM G_Callen.ARQUIVE) AS A "
                                                    + "ON IT.Arquive = A.Arquive_ID) AS IA "
                                                + "JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor "
                                                     + "FROM G_Callen.ITEM) AS I "
                                                + "ON IA.Item_ID = I.Item_ID) AS IAI "
                                            + "JOIN(SELECT Entity_ID, Entity_Name "
                                                 + "FROM G_Callen.ENTITY) AS E "
                                           + " ON IAI.Peer = E.Entity_ID) AS IAIE "
                                        + "JOIN(SELECT Entity_ID, Entity_Name "
                                            + " FROM G_Callen.ENTITY) AS EE "
                                        + "ON IAIE.Sponsor = EE.Entity_ID "
                                + "WHERE Item_Year LIKE  '%" + year_box.Text.ToString()+ "%';";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("INST");
                sda.Fill(dt);

                if (btn_pic_mode.IsChecked == false)
                    Switcher.Switch(this.Search_mode, new proSearch(dt));

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
