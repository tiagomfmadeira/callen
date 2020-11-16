using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using Window = System.Windows.Window;

namespace Callen.Windows
{
    /// <summary>
    ///     Interaction logic for winPrint.xaml
    /// </summary>
    public partial class winPrint : Window
    {
        public winPrint()
        {
            InitializeComponent();

            PreviewKeyDown += HandleEsc;

            var parent = System.Windows.Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            var instances_to_print = System.Windows.Application.Current.Properties["PrintList"] as List<Instance>;

            for (var i = 0; i < instances_to_print.Count; i++)
                tagItems.Items.Add(new
                {
                    ID = instances_to_print[i].inst_num, Matrix = instances_to_print[i].other,
                    Collec = instances_to_print[i].collec, Year = instances_to_print[i].year,
                    Name = instances_to_print[i].name
                });
        }

        public void DisposeExcelInstance(Application app, Workbook workBook, Worksheet workSheet)
        {
            app.DisplayAlerts = false;
            workBook.Close();
            app.Workbooks.Close();
            app.Quit();
            if (workSheet != null)
                Marshal.ReleaseComObject(workSheet);
            if (workBook != null)
                Marshal.ReleaseComObject(workBook);
            if (app != null)
                Marshal.ReleaseComObject(app);
            workSheet = null;
            workBook = null;
            app = null;
            GC.Collect(); // force final cleanup!
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void btn_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void btn_print_Click(object sender, RoutedEventArgs e)
        {
            var app = new Application();
            //app.WindowState = XlWindowState.xlMaximized;

            var template_tmp_path = Path.GetTempPath() + Guid.NewGuid() + ".xlsx";
            File.WriteAllBytes(template_tmp_path, Properties.Resources.tag_printing_template);

            var wb = app.Workbooks.Open(template_tmp_path);
            Worksheet ws = wb.Worksheets[1];

            var currentDate = DateTime.Now;

            var instances_to_print = System.Windows.Application.Current.Properties["PrintList"] as List<Instance>;

            var nr_of_ws = instances_to_print.Count / 36;
            if (instances_to_print.Count % 36 > 0) nr_of_ws++;

            for (var i = 1; i < nr_of_ws; i++)
            {
                ws.Copy(Type.Missing, wb.Worksheets[wb.Sheets.Count]);
                wb.Sheets[wb.Sheets.Count].Name = "Folha" + i;
            }

            var k = 0;
            for (var j = 1; j <= nr_of_ws; j++)
            {
                ws = wb.Worksheets[j];
                for (var i = 0; i < 36; i = i + 2)
                {
                    if (k >= instances_to_print.Count) break;

                    ws.Range["A" + (i + 1)].Value = "Nº" + instances_to_print[k].inst_num;
                    ws.Range["B" + (i + 1)].Value = instances_to_print[k].other;
                    ws.Range["C" + (i + 1)].Value = instances_to_print[k].collec;
                    ws.Range["D" + (i + 1)].Value = instances_to_print[k].year;
                    ws.Range["A" + (i + 2)].Value = instances_to_print[k].name;
                    k++;

                    if (k >= instances_to_print.Count) break;

                    ws.Range["E" + (i + 1)].Value = "Nº" + instances_to_print[k].inst_num;
                    ws.Range["F" + (i + 1)].Value = instances_to_print[k].other;
                    ws.Range["G" + (i + 1)].Value = instances_to_print[k].collec;
                    ws.Range["H" + (i + 1)].Value = instances_to_print[k].year;
                    ws.Range["E" + (i + 2)].Value = instances_to_print[k].name;
                    k++;
                }
            }
            //wb.SaveAs("C:\\Users\\NegativeSpade\\Desktop\\Etiquetas_para_imprimir_"+ DateTime.Now.ToString("yyyy-M-dd_HH-mm-ss") + ".xlsx");

            //app.Visible = true;
            //ws.PrintPreview();
            //app.Visible = false;

            wb.PrintOut(
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // KILL EVERYTHING
            DisposeExcelInstance(app, wb, ws);

            var tryAgain = true;
            while (tryAgain)
                try
                {
                    File.Delete(template_tmp_path);
                    tryAgain = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(1000);
                }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}