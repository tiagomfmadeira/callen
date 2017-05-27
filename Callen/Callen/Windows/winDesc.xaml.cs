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
        int id;
        public winDesc()
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
        }

        public winDesc(Item it)  // sets the text Boxes with information from an given Item 
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            Window parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized) {
                this.WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            id = Int32.Parse(it.getID());

            item_name.Text = it.getName();
            item_year.Text = it.getYear();
            item_sponsor.Text = it.getSponsor();
            item_peer.Text = it.getPeer();
            item_folder.Text = it.getFolder();
            item_theme.Text = it.getTheme();
            item_desc.Text = it.getDesc();

            if (it.getOther() != "")
                item_other.Text = it.getOther();

            if (it.getImagePath() != "")
            {
                try
                {
                    img.Source = new BitmapImage(new Uri(it.getImagePath() + ".jpeg", UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    MessageBox.Show("File not found : " + it.getImagePath());
                }
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

            if (item_name.IsEnabled)
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
                    Canvas.SetLeft(grd_pop_print, 414);
                    Canvas.SetLeft(btn_print, 555);

                    btn_save.IsEnabled = false;

                    item_name.IsEnabled = false;
                    item_year.IsEnabled = false;
                    item_other.IsEnabled = false;
                    item_desc.IsEnabled = false;

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
                    Canvas.SetLeft(grd_pop_print, 389);
                    Canvas.SetLeft(btn_print, 530);

                    item_name.IsEnabled = true;
                    item_year.IsEnabled = true;
                    item_other.IsEnabled = true;
                    item_desc.IsEnabled = true;

                    btn_save.IsEnabled = true;
                }), TimeSpan.FromMilliseconds(500));
            }
        }

        private void updateInfo() // Needs to check if some value is changed before performing the procedure
        {
            try
            {
                SqlConnection thisConnection = DBConnect.getConnection();
                thisConnection.Open();

                string Get_Data = "EXEC G_Callen.UPDATE_INST_INFO @InstID, @ItemName, @ItemYear, @ItemOther, @ItemDesc";

                SqlCommand cmd = new SqlCommand(Get_Data, thisConnection);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@InstID";
                param.Value = id;
                cmd.Parameters.Add(param);

                SqlParameter paramName = new SqlParameter();
                paramName.ParameterName = "@ItemName";
                paramName.Value = item_name.Text;
                cmd.Parameters.Add(paramName);

                SqlParameter paramYear = new SqlParameter();
                paramYear.ParameterName = "@ItemYear";
                paramYear.Value = item_year.Text;
                cmd.Parameters.Add(paramYear);

                SqlParameter paramDesc = new SqlParameter();
                paramDesc.ParameterName = "@ItemOther";
                paramDesc.Value = item_other.Text;
                cmd.Parameters.Add(paramDesc);

                SqlParameter paramOth = new SqlParameter();
                paramOth.ParameterName = "@ItemDesc";
                paramOth.Value = item_desc.Text;
                cmd.Parameters.Add(paramOth);

                cmd.ExecuteNonQuery();

                thisConnection.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            String inst = id + " - " + item_name.Text;

            if (!(App.Current.Properties["PrintList"] as List<String>).Contains(inst))
            {
                (App.Current.Properties["PrintList"] as List<String>).Add(inst);
                winNotification noti = new winNotification("Print List", id + " - " + item_name.Text, "foi adicionado com sucesso à lista para imprimir");
                noti.Show();
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
            winNotification noti = new winNotification("Update Item",id + " - " + item_name.Text,"foi modificado com sucesso");
            noti.Show();
        }
    }
}
 