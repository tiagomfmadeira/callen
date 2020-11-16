using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Callen.Windows.Forms;
using Callen.Windows.Other;

namespace Callen.Windows
{
    /// <summary>
    ///     Interaction logic for winDesc.xaml
    /// </summary>
    public partial class winDesc : Window
    {
        private bool deleted;
        private bool edited;
        private Instance inst;
        private readonly bool previewImage;

        public winDesc(Instance it, bool preview) // sets the text Boxes with information from an given Item
        {
            InitializeComponent();
            PreviewKeyDown += HandleEsc;

            var parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            edited = false;
            deleted = false;
            previewImage = preview;

            inst = it;

            fillFolderCombo();

            item_name.Text = it.name;
            item_year.Text = it.year;
            item_folder.Text = it.folder;
            item_theme.Text = it.theme;
            item_desc.Text = it.desc;
            item_note.Text = it.note;
            item_other.Text = it.other;
            item_collec.Text = it.collec;

            if (previewImage)
            {
                img_border.Visibility = Visibility.Visible;

                try
                {
                    img.Source = new BitmapImage(new Uri(it.image_path + ".jpeg", UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    MessageBox.Show("File not found : " + it.image_path);
                }
            }
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        public bool wasEdited()
        {
            return edited;
        }

        public bool wasDeleted()
        {
            return deleted;
        }

        private void btn_img_Click(object sender, RoutedEventArgs e)
        {
            var popZoomImg = new winZoomImage(img.Source);
            popZoomImg.Owner = this;
            popZoomImg.ShowDialog();
        }

        private void btn_edit_Click(object sender, RoutedEventArgs e)
        {
            var da_print_dis = new DoubleAnimation();
            da_print_dis.From = 1;
            da_print_dis.To = 0;
            da_print_dis.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            var da_print_app = new DoubleAnimation();
            da_print_app.From = 0;
            da_print_app.To = 1;
            da_print_app.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            da_print_app.BeginTime = TimeSpan.FromMilliseconds(500);

            var da_delete_dis = new DoubleAnimation();
            da_delete_dis.From = 1;
            da_delete_dis.To = 0;
            da_delete_dis.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            var da_delete_app = new DoubleAnimation();
            da_delete_app.From = 0;
            da_delete_app.To = 1;
            da_delete_app.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            da_delete_app.BeginTime = TimeSpan.FromMilliseconds(500);

            var storyboard = new Storyboard();

            Storyboard.SetTarget(da_print_dis, btn_print);
            Storyboard.SetTargetProperty(da_print_dis, new PropertyPath(OpacityProperty));

            Storyboard.SetTarget(da_print_app, btn_print);
            Storyboard.SetTargetProperty(da_print_app, new PropertyPath(OpacityProperty));

            Storyboard.SetTarget(da_delete_dis, btn_delete);
            Storyboard.SetTargetProperty(da_delete_dis, new PropertyPath(OpacityProperty));

            Storyboard.SetTarget(da_delete_app, btn_delete);
            Storyboard.SetTargetProperty(da_delete_app, new PropertyPath(OpacityProperty));

            storyboard.Children.Add(da_print_dis);
            storyboard.Children.Add(da_print_app);
            storyboard.Children.Add(da_delete_dis);
            storyboard.Children.Add(da_delete_app);

            var da_save = new DoubleAnimation();

            // Exit edit mode (Hidde stuff)
            if (item_note.IsEnabled)
            {
                da_save.From = 1;
                da_save.To = 0;
                da_save.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            }
            else
            {
                da_save.From = 0;
                da_save.To = 1;
                da_save.Duration = new Duration(TimeSpan.FromMilliseconds(500));
                da_save.BeginTime = TimeSpan.FromMilliseconds(500);
            }

            Storyboard.SetTarget(da_save, btn_save);
            Storyboard.SetTargetProperty(da_save, new PropertyPath(OpacityProperty));

            storyboard.Children.Add(da_save);

            storyboard.Begin();

            if (item_note.IsEnabled)
                TimedAction.ExecuteWithDelay(delegate
                {
                    Canvas.SetLeft(grd_pop_print, 380);
                    Canvas.SetLeft(grd_pop_delete, 420);

                    Canvas.SetLeft(btn_print, 518);
                    Canvas.SetLeft(btn_delete, 543);

                    // Button
                    btn_save.IsEnabled = false;
                    btn_save.Visibility = Visibility.Hidden;

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
                    item_theme.Text = inst.theme; //Update theme text box
                }, TimeSpan.FromMilliseconds(500));
            else
                TimedAction.ExecuteWithDelay(delegate
                {
                    Canvas.SetLeft(grd_pop_print, 355);
                    Canvas.SetLeft(grd_pop_delete, 395);
                    Canvas.SetLeft(btn_print, 493);
                    Canvas.SetLeft(btn_delete, 518);

                    //Buttons
                    btn_save.IsEnabled = true;
                    btn_save.Visibility = Visibility.Visible;

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
                }, TimeSpan.FromMilliseconds(500));
        }

        private void updateInfo() // Needs to check if some value is changed before performing the procedure
        {
            var updated = false;
            var updated_instance = new Instance(item_name.Text, inst.id, inst.inst_num, item_desc.Text, item_year.Text,
                "", (combo_theme.SelectedItem as Folders).id, item_other.Text, "", item_note.Text, item_collec.Text);

            updated = DBConnect.updateItemInfo(updated_instance);

            // TODO merge this two functions together
            if (DBConnect.updateInstanceInfo(updated_instance))
            {
                updated = true;

                //update folder, theme text boxes
                item_folder.Text = combo_folder.Text;
                item_theme.Text = combo_theme.Text;
            }

            if (updated)
            {
                updateLocalInfo();
                var noti = new winNotification("Update Item", inst.inst_num + " - " + item_name.Text,
                    "foi modificado com sucesso");
                noti.Show();
            }
        }

        private void updateLocalInfo()
        {
            inst = new Instance(item_name.Text, inst.id, inst.inst_num, item_desc.Text,
                item_year.Text, combo_theme.Text, combo_folder.Text, item_other.Text,
                inst.image_path, item_note.Text, item_collec.Text);
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            if (!(Application.Current.Properties["PrintList"] as List<Instance>).Contains(inst))
            {
                (Application.Current.Properties["PrintList"] as List<Instance>).Add(inst);
                var noti = new winNotification("Print List", inst.inst_num + " - " + inst.name,
                    "foi adicionado com sucesso à lista para imprimir");
                noti.Show();
            }
        }

        // TODO REPEATED?
        public void fillFolderCombo() // gets info about the folder and sets the respective combo box 
        {
            var folders = DBConnect.getFolders();

            combo_folder.ItemsSource = folders;
            combo_folder.DisplayMemberPath = "folder";
            combo_folder.SelectedValuePath = "folder";

            foreach (Folders folder in combo_folder.Items)
                if (folder.folder == inst.folder)
                {
                    combo_folder.SelectedItem = folder;
                    break;
                }
        }

        private void
            combo_folder_SelectionChanged(object sender,
                SelectionChangedEventArgs e) // changes theme text box when folder is selected  
        {
            if (combo_folder.SelectedItem != null)
            {
                var folders_themes = DBConnect.getFoldersThemes(combo_folder.SelectedValue.ToString());

                combo_theme.ItemsSource = folders_themes;
                combo_theme.DisplayMemberPath = "theme";
                combo_theme.SelectedValuePath = "id";

                foreach (Folders folder in combo_theme.Items)
                    if (folder.theme == inst.theme)
                    {
                        combo_theme.SelectedItem = folder;
                        break;
                    }
            }
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            updateInfo();
            edited = true;

            btn_save.IsEnabled = false;

            TimedAction.ExecuteWithDelay(delegate
            {
                // prevent spamming of save
                btn_save.IsEnabled = true;
            }, TimeSpan.FromMilliseconds(1500));
        }


        private void btn_add_folder_Click(object sender, RoutedEventArgs e)
        {
            var popAddFol = new winAddFolder();
            popAddFol.Owner = this;
            Opacity = 0.85;
            popAddFol.ShowDialog();

            Opacity = 1; // turn opacity back to 1
            fillFolderCombo();
        }

        private void btn_delete_Click(object sender, RoutedEventArgs e)
        {
            var popDelete = new winDelete(inst.inst_num, inst.name);
            popDelete.Owner = this;
            Opacity = 0.85;
            popDelete.ShowDialog();

            Opacity = 1; // turn opacity back to 1

            if (popDelete.wasDeleted())
            {
                deleted = true;
                Close();
            }
        }

        private void btn_duplicate_Click(object sender, RoutedEventArgs e)
        {
            var popDup = new winAddItem(inst);
            popDup.Owner = this;
            Opacity = 0.85;
            popDup.ShowDialog();

            Opacity = 1; // turn opacity back to 1
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

        private void btn_duplicate_MouseLeave(object sender, MouseEventArgs e) // Hide print popup
        {
            pop_dup.IsOpen = false;
        }

        private void btn_duplicate_MouseEnter(object sender, MouseEventArgs e) // Show print popup
        {
            pop_dup.IsOpen = true;
        }

        private void btn_delete_MouseEnter(object sender, MouseEventArgs e)
        {
            pop_delete.IsOpen = true;
        }

        private void btn_delete_MouseLeave(object sender, MouseEventArgs e)
        {
            pop_delete.IsOpen = false;
        }

        private void btn_save_MouseEnter(object sender, MouseEventArgs e)
        {
            pop_save.IsOpen = true;
        }

        private void btn_save_MouseLeave(object sender, MouseEventArgs e)
        {
            pop_save.IsOpen = false;
        }
    }
}