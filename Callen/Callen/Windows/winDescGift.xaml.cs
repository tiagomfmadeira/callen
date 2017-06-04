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

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for winDescGift.xaml
    /// </summary>
    public partial class winDescGift : Window
    {
        public winDescGift(Instance inst)
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

            item_name.Text = inst.getName();
            item_year.Text = inst.getYear();
            item_sponsor.Text = inst.getSponsor();
            item_peer.Text = inst.getPeer();
            item_folder.Text = "";
            item_theme.Text = inst.getTheme();
            item_desc.Text = inst.getDesc();

            if (inst.getNote() != "")
                item_note.Text = inst.getNote();

            if (inst.getOther() != "")
                item_other.Text = inst.getOther();

            if (inst.getImagePath() != "")
            {
                try
                {
                    img.Source = new BitmapImage(new Uri(inst.getImagePath() + ".jpeg", UriKind.RelativeOrAbsolute));
                }
                catch
                {
                    MessageBox.Show("File not found : " + inst.getImagePath());
                }
            }
            else
                btn_img.IsEnabled = false;
        }

        public winDescGift(Item it)
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

            item_name.Text = it.getName();
            item_year.Text = it.getYear();
            item_sponsor.Text = it.getSponsor();
            item_desc.Text = it.getDesc();

            btn_img.IsEnabled = false;
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
    }
}
