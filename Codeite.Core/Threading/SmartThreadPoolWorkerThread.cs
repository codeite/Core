using System;
using System.Threading;

namespace Codeite.Core.Threading
{
    internal class SmartThreadPoolWorkerThread
    {
        private readonly SmartThreadPool _smartThreadPool;
        private readonly Thread _thread;
        private readonly ManualResetEventSlim _resetEventSlim;
        private readonly object _syncLock = new object();

        private bool _alive = true;
        private Action _action;
        private bool _terminateIfNoWork;

        public SmartThreadPoolWorkerThread(SmartThreadPool smartThreadPool, int index)
        {
            _resetEventSlim = new ManualResetEventSlim();
            _smartThreadPool = smartThreadPool;
            _thread = new Thread(Loop);
            _thread.Name = "stp" + index;
            _thread.Start();
        }

        public void SetWorkItem(Action action)
        {
            lock (_syncLock)
            {
                _action = action;
                _resetEventSlim.Set();
            }
        }

        private void Loop()
        {
            Action action = null;

            while (_alive)
            {
                lock (_syncLock)
                {
                    if (_action != null)
                    {
                        action = _action;
                    }
                }

                if (action != null)
                {
                    action();
                    action = null;

                    var nextItem = _smartThreadPool.WorkerThreadHasCompleted(this);

                    if (nextItem == null && _terminateIfNoWork)
                    {
                        return;
                    }

                    lock (_syncLock)
                    {
                        _action = nextItem;
                    }
                }
                else
                {
                    if (_terminateIfNoWork)
                    {
                        return;
                    }

                    _resetEventSlim.Wait(TimeSpan.FromSeconds(60));
                    if (_alive)
                    {
                        _resetEventSlim.Reset();
                    }
                }
            }
        }

        public bool Terminate(TimeSpan timeout)
        {
            _alive = false;
            _resetEventSlim.Set();
            return _thread.Join(timeout);
        }

        public void TerminateIfNoWork()
        {
            _terminateIfNoWork = true;
            _resetEventSlim.Set();
            _thread.Join();
        }
    }
}