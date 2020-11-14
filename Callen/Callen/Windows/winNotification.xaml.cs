using System;
using System.Windows;
using System.Windows.Threading;

using Callen;

public partial class winNotification
{
    public winNotification(String title, String Ftext, String text)
    {
        InitializeComponent();

        notiTitle.Text = title;
        notiFText.Text = Ftext;
        notiText.Text = text;

        Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
        {
            var workingArea = System.Windows.SystemParameters.WorkArea;

            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));
        }));

        TimedAction.ExecuteWithDelay(new Action(delegate {
            this.Close();
        }), TimeSpan.FromMilliseconds(4000));
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        this.Left = System.Windows.SystemParameters.WorkArea.Right - this.Width;
        this.Top = System.Windows.SystemParameters.WorkArea.Bottom - this.Height;
    }
}
