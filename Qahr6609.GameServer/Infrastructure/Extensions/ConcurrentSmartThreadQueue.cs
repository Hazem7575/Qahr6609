using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extensions;
using System.Collections.Concurrent;

namespace Conquer6609
{
     public abstract class ConcurrentSmartThreadQueue<T>
    {
        public ConcurrentQueue<T> Queues;
        public Extensions.ThreadGroup.ThreadItem thread;
        private int inFlight;//items dequeued but not finished yet

        public ConcurrentSmartThreadQueue(int Processors)
        {
            Queues = new ConcurrentQueue<T>();
        }
        protected abstract void OnDequeue(T Value, int time);

        /// <summary>Waits until the queue is empty and the item being processed (if any) is done.</summary>
        public bool Finish()
        {
            return Finish(System.Threading.Timeout.Infinite);
        }
        /// <summary>Same as Finish() with a time limit; returns false if work was still pending when it expired.</summary>
        public bool Finish(int timeoutMs)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (Queues.Count > 0 || System.Threading.Volatile.Read(ref inFlight) > 0)
            {
                if (timeoutMs >= 0 && watch.ElapsedMilliseconds >= timeoutMs)
                    return false;
                System.Threading.Thread.Sleep(1);
            }
            if (timeoutMs < 0)
                System.Threading.Thread.Sleep(500);
            return true;
        }
        // Set by Enqueue: the worker starts on new items at once instead of at its next poll.
        private readonly System.Threading.AutoResetEvent wake = new System.Threading.AutoResetEvent(false);

        public void Start(int period)
        {
            thread = new Extensions.ThreadGroup.ThreadItem(period, "ConquerThreadQueue", Work, WaitForWork);
            thread.Open();
        }

        // ThreadItem sleep hook: sleeps like before (at most `ms`) but wakes up as soon as an item is queued.
        private void WaitForWork(int ms)
        {
            if (Queues.IsEmpty && ms > 0)
                wake.WaitOne(ms);
        }

        public void Work()
        {
            Time32 timer = Extensions.Time32.Now;
            while (true)
            {
                T obj;
                // counted before the dequeue so Finish() never sees "empty queue, nothing running" in between
                System.Threading.Interlocked.Increment(ref inFlight);
                try
                {
                    if (!Queues.TryDequeue(out obj))
                        break;
                    // one failing item must not drop the rest of the batch
                    try
                    {
                        OnDequeue(obj, timer.AllMilliseconds);
                    }
                    catch (Exception e) { MyConsole.WriteException(e); }
                }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref inFlight);
                }
            }
        }

        public virtual void Enqueue(T obj)
        {
            Queues.Enqueue(obj);
            wake.Set();
        }
        public int Count { get { return Queues.Count; } }
    }
}
