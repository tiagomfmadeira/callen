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
    /// Interaction logic for picSearchU.xaml
    /// </summary>
    public partial class picSearch : UserControl
    {
        public picSearch()
        {
            InitializeComponent();
        }

        private void formPage(object sender, RoutedEventArgs e) // Opens form window 
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winAddItem popForm = new winAddItem();
            popForm.Owner = win;
            win.Opacity = 0.5;
            popForm.ShowDialog();

            win.Opacity = 1;
        }
        
        private void openDesc(object sender, RoutedEventArgs e) // Opens description window 
        {
            MainWindow win = (MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            winDesc popDesc = new winDesc();
            popDesc.Owner = win;
            win.Opacity = 0.5;
            popDesc.ShowDialog();

            win.Opacity = 1;
        }
    }
}
