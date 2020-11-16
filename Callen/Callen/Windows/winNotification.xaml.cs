using System;
using System.Windows;
using System.Windows.Threading;
using Callen;

public partial class winNotification
{
    public winNotification(string title, string Ftext, string text)
    {
        InitializeComponent();

        notiTitle.Text = title;
        notiFText.Text = Ftext;
        notiText.Text = text;

        Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
        {
            var workingArea = SystemParameters.WorkArea;

            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));
        }));

        TimedAction.ExecuteWithDelay(delegate { Close(); }, TimeSpan.FromMilliseconds(4000));
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Left = SystemParameters.WorkArea.Right - Width;
        Top = SystemParameters.WorkArea.Bottom - Height;
    }
}