using System;
using System.Linq;
using System.Windows;

namespace Callen
{
    internal static class DialogHelper
    {
        private const double DefaultOwnerOpacity = 0.5;

        public static MainWindow GetActiveMainWindow()
        {
            return Application.Current?.Windows
                       .OfType<MainWindow>()
                       .FirstOrDefault(window => window.IsActive)
                   ?? Application.Current?.MainWindow as MainWindow;
        }

        public static void ShowOwnedDialog(Window dialog, Window owner)
        {
            ShowOwnedDialog(dialog, owner, DefaultOwnerOpacity);
        }

        public static void ShowOwnedDialog(Window dialog, Window owner, double ownerOpacity)
        {
            if (dialog == null)
                throw new ArgumentNullException(nameof(dialog));

            dialog.Owner = owner;

            if (owner != null)
                owner.Opacity = ownerOpacity;

            try
            {
                dialog.ShowDialog();
            }
            finally
            {
                if (owner != null)
                    owner.Opacity = 1;
            }
        }
    }
}
