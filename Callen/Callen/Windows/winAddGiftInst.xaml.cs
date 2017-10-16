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

using Callen.Windows.Forms;

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for winAddGiftInst.xaml
    /// </summary>
    public partial class winAddGiftInst : Window
    {
        private String instId;

        public winAddGiftInst(String inst)
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

            instId = inst;
            fillPeerCombo();
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

        public void fillPeerCombo() // gets info about the peer and sets the respoective combo box 
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.FILL_PEER_BOX";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Dest");
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

        private void btn_add_peer_Click(object sender, RoutedEventArgs e)
        {
            winAddPeer popAddSpon = new winAddPeer();
            popAddSpon.Owner = this;
            this.Opacity = 0.85;
            popAddSpon.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
            fillPeerCombo();
        }

        private void btn_insert_Click(object sender, RoutedEventArgs e)
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

                string Get_Data = "EXEC G_CALLEN.CALLEN.ADD_GIFT_WITH_INST @InstID, @Dest, @Offered";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramInst = new SqlParameter();
                paramInst.ParameterName = "@InstID";
                paramInst.Value = instId;
                cmd.Parameters.Add(paramInst);

                SqlParameter paramDest = new SqlParameter();
                paramDest.ParameterName = "@Dest";
                paramDest.Value = combo_peer.SelectedValue.ToString();
                cmd.Parameters.Add(paramDest);

                SqlParameter paramOffered = new SqlParameter();
                paramOffered.ParameterName = "@Offered";
                if (check_offer.IsChecked == true)
                    paramOffered.Value = 1;
                else
                    paramOffered.Value = 0;
                cmd.Parameters.Add(paramOffered);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            this.Close();
        }
    }
}
