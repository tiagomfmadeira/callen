using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;

namespace Callen.Windows
{
    /// <summary>
    /// Interaction logic for winPrint.xaml
    /// </summary>
    public partial class winPrint : System.Windows.Window
    {
        public winPrint()
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);

            System.Windows.Window parent = System.Windows.Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            List<Instance> instances_to_print = (App.Current.Properties["PrintList"] as List<Instance>);

            for (int i = 0; i < instances_to_print.Count; i++)
            {
                tagItems.Items.Add(new { ID = instances_to_print[i].inst_num, Matrix = instances_to_print[i].other, Collec = instances_to_print[i].collec, Year = instances_to_print[i].year, Name = instances_to_print[i].name });
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

            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            app.WindowState = XlWindowState.xlMaximized;

            Workbook wb = app.Workbooks.Open("C:\\Users\\NegativeSpade\\Desktop\\tag_printing_template.xlsx");
            Worksheet ws = wb.Worksheets[1];

            DateTime currentDate = DateTime.Now;

            List<Instance> instances_to_print = (App.Current.Properties["PrintList"] as List<Instance>);

            int nr_of_ws = instances_to_print.Count / 36;
            if(instances_to_print.Count % 36 > 0)
            {
                nr_of_ws++;
            }

            for (int i = 1; i < nr_of_ws; i++)
            {
                ws.Copy(Type.Missing, wb.Worksheets[wb.Sheets.Count]);
                wb.Sheets[wb.Sheets.Count].Name = "Folha" + i;
            }

            int k = 0;
            for (int j = 1; j <= nr_of_ws; j++)
            {
                ws = wb.Worksheets[j];
                for (int i = 0; i < 36; i = i + 2)
                {
                    if (k >= instances_to_print.Count)
                    {
                        break;
                    }

                    ws.Range["A" + (i + 1)].Value = "Nº" + instances_to_print[k].inst_num;
                    ws.Range["B" + (i + 1)].Value = instances_to_print[k].other;
                    ws.Range["C" + (i + 1)].Value = instances_to_print[k].collec;
                    ws.Range["D" + (i + 1)].Value = instances_to_print[k].year;
                    ws.Range["A" + (i + 2)].Value = instances_to_print[k].name;
                    k++;

                    if (k >= instances_to_print.Count)
                    {
                        break;
                    }

                    ws.Range["E" + (i + 1)].Value = "Nº" + instances_to_print[k].inst_num;
                    ws.Range["F" + (i + 1)].Value = instances_to_print[k].other;
                    ws.Range["G" + (i + 1)].Value = instances_to_print[k].collec;
                    ws.Range["H" + (i + 1)].Value = instances_to_print[k].year;
                    ws.Range["E" + (i + 2)].Value = instances_to_print[k].name;
                    k++;
                }
            }
            wb.SaveAs("C:\\Users\\NegativeSpade\\Desktop\\Etiquetas_para_imprimir_"+ DateTime.Now.ToString("yyyy-M-dd_HH-mm-ss") + ".xlsx");
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
