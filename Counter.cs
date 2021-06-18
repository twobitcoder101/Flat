using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Flat
{
    public sealed class Counter
    {
        private int count;
        private Stopwatch watch;
        private double interval;

        public double CountPerInterval
        {
            get { return (double)this.count / (this.watch.Elapsed.TotalSeconds / interval); }
        }

        public double CountTotal
        {
            get { return this.count; }
        }

        public Counter(double interval)
        {
            interval = FlatMath.Clamp(interval, 0.1d, 10d);

            this.count = 0;
            this.watch = new Stopwatch();
            this.interval = interval;
        }

        public void Start()
        {
            this.watch.Start();
        }

        public void Restart()
        {
            this.count = 0;
            this.watch.Restart();
        }

        public void Stop()
        {
            this.count = 0;
            this.watch.Stop();
        }

        public void IncCount(int amount = 1)
        {
            this.count += amount;
        }

    }
}
