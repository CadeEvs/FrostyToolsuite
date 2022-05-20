using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Frosty.Core.Misc
{
    // includes the ability to get the time left while the Timer is active
    public class FrostyTimer : Timer
    {
        public double TimeLeft => (time - DateTime.Now).TotalMilliseconds;

        private DateTime time;

        public FrostyTimer(ElapsedEventHandler elapsedEvent, double interval, bool autoReset)
            : base(interval)
        {
            // set properties
            AutoReset = autoReset;

            // bind custom event and FrostyTimer event
            Elapsed += elapsedEvent;
            Elapsed += Complete;

            // automatically start timer on construct
            Start();
        }

        public new void Start()
        {
            time = DateTime.Now.AddMilliseconds(Interval);

            base.Start();
        }

        private void Complete(object sender, ElapsedEventArgs e)
        {
            if (AutoReset)
            {
                time = DateTime.Now.AddMilliseconds(Interval);
            }
        }

        protected override void Dispose(bool disposing)
        {
            Elapsed -= Complete;

            base.Dispose(disposing);
        }
    }
}
