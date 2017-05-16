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

namespace Callen.Pages
{
    /// <summary>
    /// Interaction logic for Stats.xaml
    /// </summary>
    public partial class Stats : UserControl
    {
        public Stats()
        {
            InitializeComponent();
        }

        private void help(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help in Stats");
        }

        private void back(object sender, RoutedEventArgs e)
        {
            //Switcher.Switch(new picSearch());
        }
    }
}
