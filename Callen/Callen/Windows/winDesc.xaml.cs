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
using Callen.Windows.Forms;

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
        bool deleted;
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
            deleted = false;

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

            DoubleAnimation da_delete_dis = new DoubleAnimation();
            da_delete_dis.From = 1;
            da_delete_dis.To = 0;
            da_delete_dis.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            DoubleAnimation da_delete_app = new DoubleAnimation();
            da_delete_app.From = 0;
            da_delete_app.To = 1;
            da_delete_app.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            da_delete_app.BeginTime = TimeSpan.FromMilliseconds(500);

            // Exit edit mode (Hidde stuff)
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

                Storyboard.SetTarget(da_delete_dis, btn_delete);
                Storyboard.SetTargetProperty(da_delete_dis, new PropertyPath(Button.OpacityProperty));

                Storyboard.SetTarget(da_delete_app, btn_delete);
                Storyboard.SetTargetProperty(da_delete_app, new PropertyPath(Button.OpacityProperty));

                Storyboard.SetTarget(da_save_dis, btn_save);
                Storyboard.SetTargetProperty(da_save_dis, new PropertyPath(Button.OpacityProperty));

                storyboard.Children.Add(da_print_dis);
                storyboard.Children.Add(da_print_app);
                storyboard.Children.Add(da_delete_dis);
                storyboard.Children.Add(da_delete_app);
                storyboard.Children.Add(da_save_dis);

                storyboard.Begin();


                TimedAction.ExecuteWithDelay(new Action(delegate {
                    Canvas.SetLeft(grd_pop_print, 380);
                    Canvas.SetLeft(btn_print, 518);
                    Canvas.SetLeft(btn_delete, 543);

                    // Button
                    btn_save.IsEnabled = false;
                    btn_save.Visibility = System.Windows.Visibility.Hidden;

                    // Boxes
                    item_name.IsEnabled = false;
                    item_year.IsEnabled = false;
                    item_collec.IsEnabled = false;
                    item_other.IsEnabled = false;
                    item_desc.IsEnabled = false;

                    combo_folder.Visibility = Visibility.Hidden;
                    btn_add_folder.Visibility = Visibility.Hidden;
                    combo_theme.Visibility = Visibility.Hidden;
                    item_note.IsEnabled = false;
                    item_theme.Text = inst.getTheme(); //Update theme text box

                }), TimeSpan.FromMilliseconds(500));
            }
            else // Enter edit mode
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

                Storyboard.SetTarget(da_delete_dis, btn_delete);
                Storyboard.SetTargetProperty(da_delete_dis, new PropertyPath(Button.OpacityProperty));

                Storyboard.SetTarget(da_delete_app, btn_delete);
                Storyboard.SetTargetProperty(da_delete_app, new PropertyPath(Button.OpacityProperty));

                Storyboard.SetTarget(da_save_app, btn_save);
                Storyboard.SetTargetProperty(da_save_app, new PropertyPath(Button.OpacityProperty));

                storyboard.Children.Add(da_print_dis);
                storyboard.Children.Add(da_print_app);
                storyboard.Children.Add(da_delete_dis);
                storyboard.Children.Add(da_delete_app);
                storyboard.Children.Add(da_save_app);

                storyboard.Begin();

                TimedAction.ExecuteWithDelay(new Action(delegate {
                    Canvas.SetLeft(grd_pop_print, 355);
                    Canvas.SetLeft(btn_print, 493);
                    Canvas.SetLeft(btn_delete, 518);

                    //Buttons
                    btn_save.IsEnabled = true;
                    btn_save.Visibility = System.Windows.Visibility.Visible;

                    // Boxes
                    item_name.IsEnabled = true;
                    item_year.IsEnabled = true;
                    item_collec.IsEnabled = true;
                    item_other.IsEnabled = true;
                    item_desc.IsEnabled = true;

                    combo_folder.Visibility = Visibility.Visible;
                    btn_add_folder.Visibility = Visibility.Visible;
                    combo_theme.Visibility = Visibility.Visible;
                    item_note.IsEnabled = true;
                }), TimeSpan.FromMilliseconds(500));
            }
        }

        private void updateInfo() // Needs to check if some value is changed before performing the procedure
        {
            bool updated = false;
            #region UPDATE ITEM
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_CALLEN.UPDATE_ITEM_INFO @ItemID, @ItemName, @ItemDescr, @ItemYear, @ItemOther, @ItemCollec, @InstID";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter paramID = new SqlParameter();
                paramID.ParameterName = "@ItemID";
                paramID.Value = inst.getID();
                cmd.Parameters.Add(paramID);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@ItemName";
                paramName.Value = item_name.Text;
                cmd.Parameters.Add(paramName);

                SqlParameter paramDescr = new SqlParameter();
                paramDescr.ParameterName = "@ItemDescr";
                paramDescr.Value = item_desc.Text;
                cmd.Parameters.Add(paramDescr);

                SqlParameter paramYear = new SqlParameter();
                paramYear.ParameterName = "@ItemYear";
                paramYear.Value = item_year.Text;
                cmd.Parameters.Add(paramYear);

                SqlParameter paramOther = new SqlParameter();
                paramOther.ParameterName = "@ItemOther";
                paramOther.Value = item_other.Text;
                cmd.Parameters.Add(paramOther);

                SqlParameter paramCollec = new SqlParameter();
                paramCollec.ParameterName = "@ItemCollec";
                paramCollec.Value = item_collec.Text;
                cmd.Parameters.Add(paramCollec);

                SqlParameter paramInst = new SqlParameter();
                paramInst.ParameterName = "@InstID";
                paramInst.Value = inst.getInstID();
                cmd.Parameters.Add(paramInst);

                updated = (bool)cmd.ExecuteScalar();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            #endregion

            #region UPDATE INSTANCE
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

                bool tmpUpdated = (bool)cmd.ExecuteScalar();

                if (tmpUpdated)
                {
                    updated = true;

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
            #endregion

            if (updated)
            {
                updateLocalInfo();
                winNotification noti = new winNotification("Update Item", inst.getInstID() + " - " + item_name.Text, "foi modificado com sucesso");
                noti.Show();
            }
        }

        private void updateLocalInfo()
        {
            inst = new Instance(item_name.Text, inst.getID(), inst.getInstID(), item_desc.Text, 
                item_year.Text, combo_theme.Text, combo_folder.Text, item_other.Text, 
                inst.getImagePath(), item_note.Text, item_collec.Text);
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

            TimedAction.ExecuteWithDelay(new Action(delegate { // prevent spamming of save
                btn_save.IsEnabled = true;
            }), TimeSpan.FromMilliseconds(1500));
        }

        public bool wasEdited()
        {
            return edited;
        }

        public bool wasDeleted()
        {
            return deleted;
        }

        private void combo_theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            winDelete popDelete = new winDelete(inst.getInstID(), inst.getName());
            popDelete.Owner = this;
            this.Opacity = 0.85;
            popDelete.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1

            if (popDelete.wasDeleted())
            {
                deleted = true;
                this.Close();
            }
        }

        private void btn_duplicate_Click(object sender, RoutedEventArgs e)
        {
            winAddItem popDup = new winAddItem(inst);
            popDup.Owner = this;
            this.Opacity = 0.85;
            popDup.ShowDialog();

            this.Opacity = 1; // turn opacity back to 1
        }

        private void btn_duplicate_MouseLeave(object sender, MouseEventArgs e) // Hide print popup
        {
            pop_dup.IsOpen = false;
        }

        private void btn_duplicate_MouseEnter(object sender, MouseEventArgs e) // Show print popup
        {
            pop_dup.IsOpen = true;
        }
    }
}
 