// * Created by ElmistRo
// * Copyright © 2010-2014
// * ElmistRo - Project

using System;
using System.Linq;
using System.Text;
using System.Security;
using System.Threading.Generic;
using System.Collections.Generic;

namespace System.Threading
{
    public class StaticPool : IDisposable
    {
        public const int SleepTime = 1;
        internal object object_0;
        internal object object_1;
        internal Dictionary<int, Class2> dictionary_0;
        internal Queue<Class2> queue_0;
        internal List<Thread> list_0;
        protected internal Thread propagationThread;
        internal volatile bool bool_0;
        internal volatile bool bool_1;
        internal int int_0;
        internal int int_1;
        internal int int_2;
        internal int int_3;
        internal int int_4;

        public int Threads
        {
            get
            {
                return this.int_0;
            }
        }

        public int IdleThreads
        {
            get
            {
                return this.int_1;
            }
        }

        public int InUseThreads
        {
            get
            {
                return this.int_2;
            }
        }

        public int Treshold
        {
            get
            {
                return this.queue_0.Count;
            }
        }

        public StaticPool(int maximumPoolSize = 32)
        {
            this.bool_1 = false;
            this.object_1 = new object();
            this.object_0 = new object();
            this.dictionary_0 = new Dictionary<int, Class2>();
            this.queue_0 = new Queue<Class2>();
            this.list_0 = new List<Thread>();
            this.int_3 = maximumPoolSize;
            this.int_4 = maximumPoolSize;
        }

        ~StaticPool()
        {
            this.method_1(false);
        }

        void IDisposable.Dispose()
        {
            this.method_1(true);
        }

        [SecuritySafeCritical]
        internal void method_0()
        {
            if (this.bool_1)
                return;
            Interlocked.Increment(ref this.int_0);
            Interlocked.Increment(ref this.int_1);
            Thread thread = new Thread(new ThreadStart(this.method_2), 1048576);
            this.list_0.Add(thread);
            thread.Priority = ThreadPriority.Normal;
            thread.Start();
        }

        internal void method_1(bool bool_2)
        {
            if (this.bool_1)
                return;
            this.bool_1 = true;
            this.bool_0 = false;
            if (bool_2)
            {
                foreach (Thread thread in this.list_0)
                    thread.Abort();
            }
            this.dictionary_0.Clear();
            this.dictionary_0 = (Dictionary<int, Class2>)null;
            this.queue_0 = (Queue<Class2>)null;
            this.list_0 = (List<Thread>)null;
        }

        internal void method_2()
        {
            Thread currentThread = Thread.CurrentThread;
            while (this.bool_0)
            {
                // Block until the propagation thread queues work (it pulses object_0) instead of waking
                // every millisecond on every worker; the timeout only guards against a missed pulse.
                lock (this.object_0)
                {
                    if (this.queue_0 != null && this.queue_0.Count == 0)
                        Monitor.Wait(this.object_0, 50);
                }
                Class2 class2_0;
                if (this.method_3(out class2_0))
                {
                    if (class2_0.bool_0)
                    {
                        Interlocked.Decrement(ref this.int_1);
                        Interlocked.Increment(ref this.int_2);
                        currentThread.Priority = class2_0.vmethod_3();
                        try
                        {
                            class2_0.vmethod_0();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine((object)ex);
                        }
                        finally
                        {
                            class2_0.bool_1 = false;
                        }
                        currentThread.Priority = ThreadPriority.Normal;
                        Interlocked.Decrement(ref this.int_2);
                        Interlocked.Increment(ref this.int_1);
                    }
                    else
                        this.method_4(class2_0.GetHashCode());
                }
            }
            Interlocked.Decrement(ref this.int_1);
        }

        internal bool method_3(out Class2 class2_0)
        {
            class2_0 = (Class2)null;
            lock (this.object_0)
            {
                if (this.queue_0.Count != 0)
                {
                    Class2 local_0 = this.queue_0.Dequeue();
                    class2_0 = local_0;
                }
            }
            return class2_0 != null;
        }

        internal void method_4(int int_5)
        {
            lock (this.object_1)
                this.dictionary_0.Remove(int_5);
        }

        public IDisposable Subscribe(TimerRule instruction)
        {
            Class2 class2 = (Class2)null;
            lock (this.object_1)
            {
                class2 = (Class2)new Class4(instruction);
                if (instruction is LazyDelegate)
                    class2.method_1(instruction.int_0);
                this.dictionary_0[class2.GetHashCode()] = class2;
            }
            return (IDisposable)class2;
        }

        public IDisposable Subscribe<T>(TimerRule<T> instruction, T param)
        {
            Class2 class2 = (Class2)null;
            lock (this.object_1)
            {
                class2 = (Class2)new Class3<T>(instruction, param);
                if (instruction is LazyDelegate<T>)
                    class2.method_1(instruction.int_0);
                this.dictionary_0[class2.GetHashCode()] = class2;
            }
            return (IDisposable)class2;
        }

        public StaticPool Run()
        {
            this.bool_0 = true;
            for (int index = 0; index < this.int_3; ++index)
                this.method_0();
            this.propagationThread = new Thread(new ThreadStart(this.method_5));
            this.propagationThread.Start();
            return this;
        }

        private void method_5()
        {
            while (this.bool_0)
            {
                Queue<Class2> queue1 = new Queue<Class2>();
                Queue<int> queue2 = new Queue<int>();
                lock (this.object_1)
                {
                    foreach (Class2 item_0 in this.dictionary_0.Values)
                    {
                        if (item_0.bool_0)
                        {
                            if (!item_0.bool_1 && item_0.method_0())
                            {
                                item_0.bool_1 = true;
                                queue1.Enqueue(item_0);
                            }
                        }
                        else
                            queue2.Enqueue(item_0.GetHashCode());
                    }
                    while (queue2.Count != 0)
                        this.dictionary_0.Remove(queue2.Dequeue());
                }
                if (queue1.Count != 0)
                {
                    lock (this.object_0)
                    {
                        int queued = queue1.Count;
                        while (queue1.Count != 0)
                            this.queue_0.Enqueue(queue1.Dequeue());
                        if (queued == 1)
                            Monitor.Pulse(this.object_0);
                        else
                            Monitor.PulseAll(this.object_0);
                    }
                }
                Thread.Sleep(1);
            }
        }

        public void Clear()
        {
            lock (this.object_0)
                this.queue_0.Clear();
        }

        public override string ToString()
        {
            return string.Format("{0} waiting exec, {1} subscriptions, {2} threads: {3} in use, {4} idle", (object)this.queue_0.Count, (object)this.dictionary_0.Count, (object)this.int_0, (object)this.int_2, (object)this.int_1);
        }
    }
}