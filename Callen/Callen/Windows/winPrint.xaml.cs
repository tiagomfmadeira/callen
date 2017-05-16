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
    /// Interaction logic for winPrint.xaml
    /// </summary>
    public partial class winPrint : Window
    {
        public winPrint()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
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
