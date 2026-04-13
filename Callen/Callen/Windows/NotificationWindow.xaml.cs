using System;
using System.Windows;
using Callen;

namespace Callen.Windows
{
    public partial class NotificationWindow
    {
        public NotificationWindow(string title, string Ftext, string text)
        {
            InitializeComponent();

            notiTitle.Text = title;
            notiFText.Text = Ftext;
            notiText.Text = text;

            TimedAction.ExecuteWithDelay(Close, TimeSpan.FromMilliseconds(4000));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.WorkArea.Right - Width;
            Top = SystemParameters.WorkArea.Bottom - Height;
        }
    }
}

