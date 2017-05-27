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

            this.Left = workingArea.Right - this.Width;
            this.Top = workingArea.Bottom - this.Height;
        }));

        TimedAction.ExecuteWithDelay(new Action(delegate {
            this.Close();
        }), TimeSpan.FromMilliseconds(4000));
    }
}
