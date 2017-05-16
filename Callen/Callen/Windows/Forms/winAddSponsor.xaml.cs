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

namespace Callen.Windows.Forms
{
    /// <summary>
    /// Interaction logic for addSponsor.xaml
    /// </summary>
    public partial class winAddSponsor : Window
    {
        public winAddSponsor()
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
        }

        private void btn_add_address_Click(object sender, RoutedEventArgs e)
        {
            main_border.Height = 374;
            btn_add_address.Visibility = Visibility.Hidden;
            Canvas.SetLeft(btn_submit, 368);
            Canvas.SetTop(btn_submit, 527);

            street_dock.Visibility = Visibility.Visible;
            street_city.Visibility = Visibility.Visible;
            street_state.Visibility = Visibility.Visible;
            street_country.Visibility = Visibility.Visible;
            street_postal.Visibility = Visibility.Visible;
            divider.Visibility = Visibility.Visible;
        }

        private void btn_submit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
