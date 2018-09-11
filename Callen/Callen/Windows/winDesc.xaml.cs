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

using Callen.Windows.Other;

using System.Data;
using System.Data.SqlClient;

using System.Windows.Media.Animation;

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for winDesc.xaml
    /// </summary>
    public partial class winDesc : Window
    {
        bool edited;
        Instance inst;

        public winDesc(Instance it)  // sets the text Boxes with information from an given Item
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

            edited = false;

            inst = it;

            fillFolderCombo();

            item_name.Text = it.getName();
            item_year.Text = it.getYear();
            item_folder.Text = it.getFolder();
            item_theme.Text = it.getTheme();
            item_desc.Text = it.getDesc();
            item_note.Text = it.getNote();
            item_other.Text = it.getOther();
            item_collec.Text = it.getCollec();

            try
            {
                img.Source = new BitmapImage(new Uri(it.getImagePath() + ".jpeg", UriKind.RelativeOrAbsolute));
            }
            catch
            {
                MessageBox.Show("File not found : " + it.getImagePath());
            }

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

        private void btn_img_Click(object sender, RoutedEventArgs e)
        {
            winZoomImage popZoomImg = new winZoomImage(img.Source);
            popZoomImg.Owner = this;
            popZoomImg.ShowDialog();
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e) 
        {
            DoubleAnimation da_print_dis = new DoubleAnimation();
            da_print_dis.From = 1;
            da_print_dis.To = 0;
            da_print_dis.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            DoubleAnimation da_print_app = new DoubleAnimation();
            da_print_app.From = 0;
            da_print_app.To = 1;
            da_print_app.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            da_print_app.BeginTime = TimeSpan.FromMilliseconds(500);

            if (item_note.IsEnabled)
            {
                var storyboard = new Storyboard();

                DoubleAnimation da_save_dis = new DoubleAnimation();
                da_save_dis.From = 1;
                da_save_dis.To = 0;
                da_save_dis.Duration = new Duration(TimeSpan.FromMilliseconds(500));

                Storyboard.SetTarget(da_print_dis, btn_print);
                Storyboard.SetTargetProperty(da_print_dis, new PropertyPath(Button.OpacityProperty));

                Storyboard.SetTarget(da_print_app, btn_print);
                Storyboard.SetTargetProperty(da_print_app, new PropertyPath(Button.OpacityProperty));

                Storyboard.SetTarget(da_save_dis, btn_save);
                Storyboard.SetTargetProperty(da_save_dis, new PropertyPath(Button.OpacityProperty));

                storyboard.Children.Add(da_print_dis);
                storyboard.Children.Add(da_print_app);
                storyboard.Children.Add(da_save_dis);

                storyboard.Begin();


                TimedAction.ExecuteWithDelay(new Action(delegate {
                    Canvas.SetLeft(grd_pop_print, 430);
                    Canvas.SetLeft(btn_print, 568);

                    btn_save.IsEnabled = false;
                    btn_save.Visibility = System.Windows.Visibility.Hidden;

                    item_note.IsEnabled = false;
                    combo_folder.Visibility = Visibility.Hidden;
                    combo_theme.Visibility = Visibility.Hidden;
                    item_theme.Text = inst.getTheme();
                }), TimeSpan.FromMilliseconds(500));
            }
            else
            {
                var storyboard = new Storyboard();

                DoubleAnimation da_save_app = new DoubleAnimation();
                da_save_app.From = 0;
                da_save_app.To = 1;
                da_save_app.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                da_save_app.BeginTime = TimeSpan.FromMilliseconds(500);

                Storyboard.SetTarget(da_print_dis, btn_print);
                Storyboard.SetTargetProperty(da_print_dis, new PropertyPath(Button.OpacityProperty));

                Storyboard.SetTarget(da_print_app, btn_print);
                Storyboard.SetTargetProperty(da_print_app, new PropertyPath(Button.OpacityProperty));

                Storyboard.SetTarget(da_save_app, btn_save);
                Storyboard.SetTargetProperty(da_save_app, new PropertyPath(Button.OpacityProperty));

                storyboard.Children.Add(da_print_dis);
                storyboard.Children.Add(da_print_app);
                storyboard.Children.Add(da_save_app);

                storyboard.Begin();

                TimedAction.ExecuteWithDelay(new Action(delegate {
                    Canvas.SetLeft(grd_pop_print, 405);
                    Canvas.SetLeft(btn_print, 543);

                    item_note.IsEnabled = true;
                    combo_folder.Visibility = Visibility.Visible;
                    combo_theme.Visibility = Visibility.Visible;

                    btn_save.IsEnabled = true;
                    btn_save.Visibility = System.Windows.Visibility.Visible;
                }), TimeSpan.FromMilliseconds(500));
            }
        }

        private void updateInfo() // Needs to check if some value is changed before performing the procedure
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.UPDATE_INST_INFO @InstID, @InstNote, @InstFolder";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramInst = new SqlParameter();
                paramInst.ParameterName = "@InstID";
                paramInst.Value = inst.getInstID();
                cmd.Parameters.Add(paramInst);

                SqlParameter paramNote = new SqlParameter();
                paramNote.ParameterName = "@InstNote";
                paramNote.Value = item_note.Text;
                cmd.Parameters.Add(paramNote);

                SqlParameter paramFolder = new SqlParameter();
                paramFolder.ParameterName = "@InstFolder";
                paramFolder.Value = (combo_theme.SelectedItem as Folders).id.ToString();
                cmd.Parameters.Add(paramFolder);

                bool updated = (bool) cmd.ExecuteScalar();

                if (updated)
                {
                    winNotification noti = new winNotification("Update Item", inst.getInstID() + " - " + item_name.Text, "foi modificado com sucesso");
                    noti.Show();
                    //update folder, theme text boxes
                    item_folder.Text = combo_folder.Text;
                    item_theme.Text = combo_theme.Text;
                }

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            String inst = this.inst.getInstID() + " - " + item_name.Text;

            if (!(App.Current.Properties["PrintList"] as List<String>).Contains(inst))
            {
                (App.Current.Properties["PrintList"] as List<String>).Add(inst);
                winNotification noti = new winNotification("Print List", this.inst.getInstID() + " - " + item_name.Text, "foi adicionado com sucesso à lista para imprimir");
                noti.Show();
            }
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
                foreach (DataRow row in dt.Rows)
                {
                    ft.Add(new Folders { folder = row["Code"].ToString() });
                }

                combo_folder.ItemsSource = ft;
                combo_folder.DisplayMemberPath = "folder";
                combo_folder.SelectedValuePath = "folder";

                foreach (Folders folder in combo_folder.Items)
                {
                    if (folder.folder == inst.getFolder())
                    {
                        combo_folder.SelectedItem = folder;
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

        private void combo_folder_SelectionChanged(object sender, SelectionChangedEventArgs e) // changes theme text box when folder is selected  
        {
            if (combo_folder.SelectedItem != null)
                try
                {
                    SqlConnection thisConnection = DBConnect.getConnection();
                    thisConnection.Open();

                    string Get_Data = "EXEC G_CALLEN.FOLDERS_THEMES @Code";

                    SqlCommand cmd = thisConnection.CreateCommand();
                    cmd.CommandText = Get_Data;

                    SqlParameter paramItem = new SqlParameter();
                    paramItem.ParameterName = "@Code";
                    Console.WriteLine(combo_folder.SelectedValue.ToString());
                    paramItem.Value = combo_folder.SelectedValue.ToString();
                    cmd.Parameters.Add(paramItem);

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable("Folder");
                    sda.Fill(dt);

                    List<Folders> ft = new List<Folders>();
                    foreach (DataRow row in dt.Rows)
                    {
                        ft.Add(new Folders { theme = row["Theme_Descr"].ToString(), id = row["Arquive_ID"].ToString() });
                    }

                    combo_theme.ItemsSource = ft;
                    combo_theme.DisplayMemberPath = "theme";
                    combo_theme.SelectedValuePath = "id";

                    foreach (Folders folder in combo_theme.Items)
                    {
                        if (folder.theme == inst.getTheme())
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

        private void btn_print_MouseEnter(object sender, MouseEventArgs e) // Show print popup
        {
            pop_print.IsOpen = true;
        }

        private void btn_print_MouseLeave(object sender, MouseEventArgs e) // Hide print popup
        {
            pop_print.IsOpen = false;
        }

        private void btn_edit_MouseEnter(object sender, MouseEventArgs e) // Show print popup
        {
            pop_edit.IsOpen = true;
        }

        private void btn_edit_MouseLeave(object sender, MouseEventArgs e) // Hide print popup
        {
            pop_edit.IsOpen = false;
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            updateInfo();
            edited = true;

            btn_save.IsEnabled = false;
            btn_save.Visibility = System.Windows.Visibility.Hidden;

            TimedAction.ExecuteWithDelay(new Action(delegate { // prevent spamming of save
                btn_save.IsEnabled = true;
                btn_save.Visibility = System.Windows.Visibility.Visible;
            }), TimeSpan.FromMilliseconds(2250));
        }

        public bool wasEdited()
        {
            return edited;
        }

        private void combo_theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
 