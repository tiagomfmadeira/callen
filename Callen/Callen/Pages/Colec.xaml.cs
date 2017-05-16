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
    /// Interaction logic for ColecU.xaml
    /// </summary>
    public partial class Colec : UserControl
    {
        public Colec()
        {
            InitializeComponent();
        }

        private void openColec(object sender, RoutedEventArgs e)
        {
            MainWindow parentWindow = (MainWindow) Window.GetWindow(this);
            Switcher.Switch(parentWindow.Content_plane, new picSearch());
        }

        private void addColec(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
