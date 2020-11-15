using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Configuration;

using Microsoft.Win32;

namespace Callen.Windows.Forms
{
    /// <summary>
    /// Interaction logic for winAddItem.xaml
    /// </summary>
    public partial class winAddItem : Window
    {
        // TODO check what this variables do
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
            List<Folders> folders = DBConnect.getFolders();

            combo_folder.ItemsSource = folders;
            combo_folder.DisplayMemberPath = "folder";
            combo_folder.SelectedValuePath = "folder";
        }

        private void fillThemeCombo()
        {
            List<Folders> foldersThemes = DBConnect.getFoldersThemes(combo_folder.SelectedValue.ToString());

            combo_theme.ItemsSource = foldersThemes;
            combo_theme.DisplayMemberPath = "theme";
            combo_theme.SelectedValuePath = "id";
        }

        private void fillThemeCombo(Instance it)
        {
            fillThemeCombo();

            foreach (Folders folder in combo_theme.Items)
            {
                if (folder.theme == it.getTheme())
                {
                    combo_theme.SelectedItem = folder;
                    break;
                }
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

            Instance inst = new Instance(name_box.Text, "", "", desc_box.Text, year_box.Text, "", (combo_theme.SelectedItem as Folders).id.ToString(), other_box.Text, "", note_box.Text, collec_box.Text);

            DBConnect.addInstance(inst, img.Source, @config.AppSettings.Settings["image_path"].Value);

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
