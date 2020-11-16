using System.Windows;
using System.Windows.Input;

namespace Callen.Windows.Forms
{
    /// <summary>
    ///     Interaction logic for winDelete.xaml
    /// </summary>
    public partial class winDelete : Window
    {
        // TODO check if this bool makes sense
        private bool deleted;

        private readonly string instance_id;
        private readonly string instance_name;

        public winDelete(string inst_id, string inst_name)
        {
            InitializeComponent();
            PreviewKeyDown += HandleEsc;

            var parent = Application.Current.MainWindow;
            if (parent.WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
                closeBorder.Width = parent.Width;
                closeBorder.Height = parent.Height;
            }

            deleted = false;
            instance_id = inst_id;
            instance_name = inst_name;
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

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void btn_yes_Click(object sender, RoutedEventArgs e)
        {
            DBConnect.removeInstance(instance_id);

            deleted = true;

            var noti = new winNotification("Item Deleted", instance_id + " - " + instance_name,
                "foi eliminado com sucesso");
            noti.Show();

            Close();
        }

        public bool wasDeleted()
        {
            return deleted;
        }

        private void btn_no_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}