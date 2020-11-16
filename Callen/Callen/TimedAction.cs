using System;
using System.Windows.Threading;

namespace Callen
{
    public static class TimedAction
    {
        public static void ExecuteWithDelay(Action action, TimeSpan delay)
        {
            var timer = new DispatcherTimer();
            timer.Interval = delay;
            timer.Tag = action;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private static void timer_Tick(object sender, EventArgs e)
        {
            var timer = (DispatcherTimer) sender;
            var action = (Action) timer.Tag;

            action.Invoke();
            timer.Stop();
        }
    }
}