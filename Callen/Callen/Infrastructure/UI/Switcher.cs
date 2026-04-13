using System.Windows.Controls;

namespace Callen
{
    public static class Switcher
    {
        public static void Switch(Grid targetGrid, UserControl newPage)
        {
            if (targetGrid.Children.Count > 0)
                targetGrid.Children.Clear();

            targetGrid.Children.Add(newPage);
        }
    }
}
