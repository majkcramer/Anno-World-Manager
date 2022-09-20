using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Anno_World_Manager.helper
{
    internal static class DelayFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="millisecond"></param>
        /// <param name="action"></param>
        /// <example>
        /// DelayFactory.DelayAction(500, new Action(() => { this.RunAction(); }));
        /// </example>
        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate

            {
                action.Invoke();
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }
    }
}
