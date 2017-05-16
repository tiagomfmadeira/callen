using System.Windows.Controls;

namespace Callen
{
    public static class Switcher
    {
        public static MainWindow pageSwitcher;
        
        public static void Switch(Grid grd, UserControl newPage) // Switches grid with UserControl
        {
            if (grd.Children.Count > 0)
            {
                grd.Children.Clear();
                grd.Children.Add(newPage);
            }
            else
            {
                grd.Children.Add(newPage);
            }
        }

    }
}
