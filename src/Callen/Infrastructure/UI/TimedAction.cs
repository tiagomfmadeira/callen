using System;
using System.Windows.Threading;

namespace Callen
{
    public static class TimedAction
    {
        public static void ExecuteWithDelay(Action action, TimeSpan delay)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var timer = new DispatcherTimer { Interval = delay };
            EventHandler handler = null;
            handler = (sender, args) =>
            {
                timer.Tick -= handler;
                timer.Stop();
                action();
            };

            timer.Tick += handler;
            timer.Start();
        }
    }
}
