using System.Windows;
using System.Windows.Controls;

namespace Callen.Windows
{
    public enum DialogAction
    {
        Cancel,
        Secondary,
        Primary
    }

    public partial class ActionDialogWindow : Window
    {
        public DialogAction SelectedAction { get; private set; } = DialogAction.Cancel;

        public ActionDialogWindow(
            string title,
            string context,
            string message,
            string primaryButtonText,
            string secondaryButtonText = null,
            string cancelButtonText = null)
        {
            InitializeComponent();

            dialogTitle.Text = title;
            dialogMessage.Text = message;

            if (string.IsNullOrWhiteSpace(context))
            {
                dialogContext.Visibility = Visibility.Collapsed;
            }
            else
            {
                dialogContext.Text = context;
                dialogContext.Visibility = Visibility.Visible;
            }

            btn_primary.Content = primaryButtonText;
            ConfigureButton(btn_secondary, secondaryButtonText);
            ConfigureButton(btn_cancel, cancelButtonText);
        }

        private static void ConfigureButton(Button button, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                button.Visibility = Visibility.Collapsed;
                return;
            }

            button.Content = text;
            button.Visibility = Visibility.Visible;
        }

        private void btn_primary_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = DialogAction.Primary;
            DialogResult = true;
        }

        private void btn_secondary_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = DialogAction.Secondary;
            DialogResult = false;
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            SelectedAction = DialogAction.Cancel;
            DialogResult = false;
        }
    }
}

