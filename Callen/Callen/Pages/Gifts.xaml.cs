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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Callen.Windows;

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for Gifts.xaml
    /// </summary>
    public partial class Gifts : UserControl
    {
        public Gifts()
        {
            InitializeComponent();
        }

        private void btn_gift_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winAddGift popGift = new winAddGift();
            popGift.Owner = win;
            win.Opacity = 0.5;
            popGift.ShowDialog();

            win.Opacity = 1;
        }
    }
}
