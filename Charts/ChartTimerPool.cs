using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace M4.Charts
{
    public static class ChartTimerPool
    {
        private static readonly List<Timer> _timers = new List<Timer>();

        public static bool TimerExists(int timerInterval)
        {
            Timer timer;
            return TryGetExistingTimerFor(timerInterval, out timer);
        }

        public static Timer GetTimerFor(int timerInterval)
        {
            Timer timer;
            if (!TryGetExistingTimerFor(timerInterval, out timer))
            {
                timer = new Timer()
                {
                    Interval = timerInterval
                };
                timer.Start();
                _timers.Add(timer);
            }
            return timer;
        }

        private static bool TryGetExistingTimerFor(int timerInterval, out System.Windows.Forms.Timer timer)
        {
            timer = _timers.FirstOrDefault(t => t.Interval == timerInterval);
            return timer != null;
        }
    }
}
