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

using System.Configuration;

using System.Data;
using System.Data.SqlClient;

using Microsoft.Win32;

namespace Callen.Windows.Forms
{
    /// <summary>
    /// Interaction logic for winAddItem.xaml
    /// </summary>
    public partial class winAddItem : Window
    {
        private bool inserted; // tell if a item was inserted
        private bool duplicated;

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

            inserted = false;
            duplicated = false;
        }

        public winAddItem(Instance it)
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

            fillWindow(it);

            inserted = false;
            duplicated = true;
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

                string Get_Data = "EXEC G_CALLEN.FOLDERS_NAMES";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Folder");
                sda.Fill(dt);

                List<Folders> ft = new List<Folders>();
                foreach(DataRow row in dt.Rows)
                {
                    ft.Add(new Folders { folder = row["Code"].ToString()});
                }

                combo_folder.ItemsSource = ft;
                combo_folder.DisplayMemberPath = "folder";
                combo_folder.SelectedValuePath = "folder";

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                
            }
        }

        private void fillThemeCombo()
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.FOLDERS_THEMES @Code";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@Code";
                paramFolder.Value = combo_folder.SelectedValue.ToString();
                cmd.Parameters.Add(paramFolder);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Folder");
                sda.Fill(dt);

                List<Folders> ft = new List<Folders>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Folders { theme = row["Theme_Descr"].ToString(), id = row["Archive_ID"].ToString() });
                }

                combo_theme.ItemsSource = ft;
                combo_theme.DisplayMemberPath = "theme";
                combo_theme.SelectedValuePath = "id";

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());

            }
        }

        private void fillThemeCombo(Instance it)
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.FOLDERS_THEMES @Code";

                SqlCommand cmd = thisConnection.CreateCommand();
                cmd.CommandText = Get_Data;

                SqlParameter paramItem = new SqlParameter();
                paramItem.ParameterName = "@Code";
                paramItem.Value = combo_folder.SelectedValue.ToString();
                cmd.Parameters.Add(paramItem);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable("Folder");
                sda.Fill(dt);

                List<Folders> ft = new List<Folders>();
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Folders { theme = row["Theme_Descr"].ToString(), id = row["Archive_ID"].ToString() });
                }

                combo_theme.ItemsSource = ft;
                combo_theme.DisplayMemberPath = "theme";
                combo_theme.SelectedValuePath = "id";

                foreach (Folders folder in combo_theme.Items)
                {
                    if (folder.theme == it.getTheme())
                    {
                        combo_theme.SelectedItem = folder;
                        break;
                    }
                }

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

                    show_img(src);

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

        private void show_img(ImageSource src)
        {
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
        }

        private void btn_insert_Click(object sender, RoutedEventArgs e) // inserts information in data base 
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (string.IsNullOrEmpty(name_box.Text.ToString())
                        || string.IsNullOrEmpty(desc_box.Text.ToString()) || string.IsNullOrEmpty(year_box.Text.ToString())
                            || (combo_folder.SelectedItem == null))
            {
                MessageBox.Show("Necessita de nome, Descrição, Ano e Pasta");
                return;
            }

            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "";

                Get_Data = "EXEC G_CALLEN.ADD_INST @Name, @Desc, @Year, @Collec, @Folder, @Other, @Note, @Img_Path";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@Name";
                paramName.Value = name_box.Text.ToString();
                cmd.Parameters.Add(paramName);

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

                SqlParameter paramCollec = new SqlParameter();
                paramCollec.ParameterName = "@Collec";
                paramCollec.Value = collec_box.Text.ToString();
                cmd.Parameters.Add(paramCollec);

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@Folder";
                paramFolder.Value = (combo_theme.SelectedItem as Folders).id.ToString();
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
                    var img_path = @config.AppSettings.Settings["image_path"].Value + "\\Instance_"; // gets filled in database trigger
                    paramImg.Value = img_path;
                }
                else
                {
                    paramImg.Value = "";
                }
                cmd.Parameters.Add(paramImg);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (img.Source != null) // Theres an image
                    {
                        var filename = img.Source.ToString().Substring(img.Source.ToString().LastIndexOf("///") + 3);
                        System.IO.File.Copy(filename, @config.AppSettings.Settings["image_path"].Value + "\\Instance_" + rdr["Inst_Number"].ToString() + ".jpeg");
                    }
                    break;
                }

                thisConnection.Close();
             }
             catch (Exception ee)
             {
                 MessageBox.Show(ee.ToString());
             }
            inserted = true;

            winNotification noti = new winNotification("New Item", name_box.Text.ToString() , "foi inserido com sucesso");
            noti.Show();

            btn_insert.IsEnabled = false;

            TimedAction.ExecuteWithDelay(new Action(delegate { // prevent spamming of save
                btn_insert.IsEnabled = true;
            }), TimeSpan.FromMilliseconds(1500));
        }

        private void combo_folder_SelectionChanged(object sender, SelectionChangedEventArgs e) // changes theme text box when folder is selected  
        {
            if (combo_folder.SelectedItem != null)
            {
                combo_theme.IsEnabled = true;
                btn_add_theme.IsEnabled = true;
                btn_add_theme.Visibility = System.Windows.Visibility.Visible;

                if (duplicated == false)
                {
                    fillThemeCombo();
                }
            }
            else
            {
                combo_theme.IsEnabled = false;
                btn_add_theme.IsEnabled = false;
                btn_add_theme.Visibility = System.Windows.Visibility.Hidden;
            }
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

        private void btn_add_theme_Click(object sender, RoutedEventArgs e)
        {
            winAddFolder popAddFol = new winAddFolder(combo_folder.SelectedValue.ToString());
            popAddFol.Owner = this;
            this.Opacity = 0.85;
            popAddFol.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
            fillThemeCombo();
        }

        private void fillWindow(Instance it)
        {
            name_box.Text = it.getName();
            year_box.Text = it.getYear();
            collec_box.Text = it.getCollec();
            other_box.Text = it.getOther();
            desc_box.Text = it.getDesc();
            note_box.Text = it.getNote();

            foreach (Folders folder in combo_folder.Items)
            {
                if (folder.folder == it.getFolder())
                {
                    combo_folder.SelectedItem = folder;
                    break;
                }
            }

            fillThemeCombo(it);

            // Check if there is a image
            if (it.getImagePath() != "")
            {
                ImageSource src = new BitmapImage(new Uri(it.getImagePath() + ".jpeg", UriKind.RelativeOrAbsolute));

                show_img(src);

                img_border.Visibility = Visibility.Visible;
                img.Source = src;
            }
        }
    }
}
