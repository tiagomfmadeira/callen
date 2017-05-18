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

using System.Data;
using System.Data.SqlClient;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for proSearchU.xaml
    /// </summary>
    public partial class proSearch : UserControl
    {
        public proSearch()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEnter);
            FillDataGrid();
        }

        public proSearch(DataTable dt)
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEnter);
            grdColec.ItemsSource = dt.DefaultView;
        }

        private void HandleEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                openDesc();
        }

        private void FillDataGrid() // Used to Fill the Data Grid with items information (Favourite, Name, Descr, Year, Theme, Folder, Peer, Sponsor) 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Favorite AS Favorito,Inst_Number AS ID,Item_Name AS Nome,Item_Descr AS Descrição,Item_Year AS Ano ,Theme_Descr AS Tema,Code AS Pasta,Peer_name AS Fornecedor,Entity_Name AS Patrocinador "
                                  +"FROM(SELECT Favorite, Inst_Number, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Entity_Name AS Peer_name, Sponsor "
                                        +"FROM(SELECT Favorite, Inst_Number, Item_Name, Item_Descr, Item_Year, Theme_Descr, Code, Peer, Sponsor "
                                           +"FROM(SELECT Favorite, Inst_Number, Item_ID, Code, Peer, Theme_Descr "
                                               +"FROM(SELECT Item_ID, Inst_Number, Favorite, Arquive, Peer "
                                                    +"FROM G_Callen.INST) AS IT "
                                                    +"JOIN(SELECT * "
                                                         +"FROM G_Callen.ARQUIVE) AS A "
                                                    +"ON IT.Arquive = A.Arquive_ID) AS IA "
                                                + "JOIN(SELECT Item_ID, Item_Name, Item_Descr, Item_Year, Sponsor "
                                                     + "FROM G_Callen.ITEM) AS I "
                                                + "ON IA.Item_ID = I.Item_ID) AS IAI "
                                            + "JOIN(SELECT Entity_ID, Entity_Name "
                                                 + "FROM G_Callen.ENTITY) AS E "
                                           + " ON IAI.Peer = E.Entity_ID) AS IAIE "
                                        + "JOIN(SELECT Entity_ID, Entity_Name "
                                            + " FROM G_Callen.ENTITY) AS EE "
                                        + "ON IAIE.Sponsor = EE.Entity_ID; ";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("INST");
                sda.Fill(dt);

                grdColec.ItemsSource = dt.DefaultView;

                thisConnection.Close();
            }
            catch
            {
                MessageBox.Show("db error");
            }
        }

        private String[] getExtra(String id) // Gets Note and Image Path for specific Item 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "SELECT Note, Inst_PicPath "
                                 +"FROM G_Callen.INST "
                                 +"WHERE Inst_Number = " + id;

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("desc");
                sda.Fill(dt);

                String tmp = string.Join(", ", dt.Rows[0].ItemArray);

                return tmp.Split(',');
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                return new string[2];
            }
        } 

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)  
        {
            openDesc();
        }

        private void openDesc() // Opens the description of specific item in Row
        {
            DataRowView row = (DataRowView)grdColec.SelectedItem;

            Item it = new Item(row["Nome"].ToString(), row["ID"].ToString(), row["Descrição"].ToString(), row["Ano"].ToString(),
                 row["Tema"].ToString(), row["Pasta"].ToString(), row["Fornecedor"].ToString(), row["Patrocinador"].ToString()); ;

            // Get Other and Image Path
            String[] extra = getExtra(row["ID"].ToString());
            if (extra[0] != "")
                it.setOther(extra[0]);
            if (extra[1] != "")
                it.setImagePath(extra[1]);

            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winDesc popDesc = new winDesc(it);
            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            win.Opacity = 1;
        }

        private void formPage(object sender, RoutedEventArgs e) // Opens the form to add a new item 
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winAddItem popForm = new winAddItem();
            popForm.Owner = win;
            win.Opacity = 0.5;
            popForm.ShowDialog();

            win.Opacity = 1;
        }
    }
}
