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
        }

        public winDesc(Item it)  // sets the text Boxes with information from an given Item 
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            item_name.Text = it.getName();
            item_year.Text = it.getYear();
            item_sponsor.Text = it.getSponsor();
            item_peer.Text = it.getPeer();
            item_folder.Text = it.getFolder();
            item_theme.Text = it.getTheme();
            item_desc.Text = it.getDesc();

            if(it.getOther() != "")
                item_other.Text = it.getOther();

            if (it.getImagePath() != "") {
                try
                {
                    ImageSource src = new BitmapImage(new Uri(it.getImagePath() + ".jpeg"));
                    item_img.Source = src;
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
                closeWin();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            closeWin();
        }

        private void closeWin()
        {
            this.Close();
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            win.Opacity = 1;
        }
    }
}
 