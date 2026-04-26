using System;
using System.Linq;
using System.Windows;

namespace Callen
{
    internal static class DialogHelper
    {
        public static MainWindow GetActiveMainWindow()
        {
            return Application.Current?.Windows
                       .OfType<MainWindow>()
                       .FirstOrDefault(window => window.IsActive)
                   ?? Application.Current?.MainWindow as MainWindow;
        }

        public static void ShowOwnedDialog(Window dialog, Window owner)
        {
            if (dialog == null)
                throw new ArgumentNullException(nameof(dialog));

            var safeOwner = GetSafeOwner(owner, dialog);
            if (safeOwner != null)
                dialog.Owner = safeOwner;

            dialog.ShowDialog();
        }

        private static Window GetSafeOwner(Window owner, Window dialog)
        {
            if (owner == null || dialog == null || ReferenceEquals(owner, dialog))
                return null;

            // Closed windows cannot be assigned as Owner.
            if (!owner.IsLoaded)
                return null;

            var ownerHandle = new System.Windows.Interop.WindowInteropHelper(owner).Handle;
            return ownerHandle == IntPtr.Zero ? null : owner;
        }
    }
}
