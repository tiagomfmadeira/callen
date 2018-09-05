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
using System.Printing;

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

            Window parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            Random r = new Random();
            InitializeComponent();
            for (int i = 0; i < 13; i++)
            {
                itemContorlItem.Items.Add(new { ID = "Nº "+i.ToString(), Collec="Uma coleção muito bonita", Name = "A realy long fucking name to test this out but it can get much bigger if I want it to qowihr qowhr oqwht uihqeo tiqhk eyqoiyqwio eyjqwio ejyioqw eyjiqwohy oiqwhey oihqwe yoinqw eoy qwoeunyweoqhy qowenyo qwjenyoqwen yojnqw eoyn qweyn qwojeyn oqwjey noqwjey noç", Matrix = "B-25", Year = "1999" });
            }
            for (int i = 0; i < 13; i++)
            {
                itemContorlItem.Items.Add(new { ID = "Nº " + i.ToString(), Name = "A realy long", Matrix = "B-25", Year = "1999" });
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

        public void btn_print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                flowDocument.ColumnWidth = printDialog.PrintableAreaWidth;
                flowDocument.PagePadding = new Thickness(0);

                printDialog.PrintDocument(((IDocumentPaginatorSource)flowDocument).DocumentPaginator, "Flow Document Print Job");

            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
