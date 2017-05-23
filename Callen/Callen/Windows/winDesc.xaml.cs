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
    /// Interaction logic for winDesc.xaml
    /// </summary>
    public partial class winDesc : Window
    {
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

            Window parent =  Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized) {
                this.WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

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
    }
}
 