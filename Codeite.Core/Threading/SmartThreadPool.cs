using System;
using System.Collections.Generic;
using System.Linq;

namespace Codeite.Core.Threading
{
    public class SmartThreadPool
    {
        private readonly int _maxThreadCount;
        private readonly List<SmartThreadPoolWorkerThread> _threads;
        private readonly Queue<SmartThreadPoolWorkerThread> _freeWorkers;
        private readonly Queue<Action> _workBacklog;
        private readonly object _syncLock = new object();

        private bool _completeAndTerminate;
        private bool _paused;

        public SmartThreadPool(int maxThreadCount = 5, bool initialPausedState = false)
        {
            _maxThreadCount = maxThreadCount;
            _threads = new List<SmartThreadPoolWorkerThread>(maxThreadCount);
            _freeWorkers = new Queue<SmartThreadPoolWorkerThread>(maxThreadCount);
            _workBacklog = new Queue<Action>();
            _paused = initialPausedState;
        }

        public bool HasMoreTasks
        {
            get
            {
                lock (_syncLock)
                {
                    return _workBacklog.Any();
                }
            }
        }

        public bool Queue(Action action)
        {
            // Do not allow new tasks to be added if trying to complete nad then terminate
            if (_completeAndTerminate)
            {
                return false;
            }

            lock (_syncLock)
            {
                // if paused then just add to backlog
                if (_paused)
                {
                    _workBacklog.Enqueue(action);
                    return true;
                }

                // Is there a working sitting idle that could compelte this task?
                if (_freeWorkers.Any())
                {
                    var worker = _freeWorkers.Dequeue();

                    worker.SetWorkItem(action);
                }
                // Maybe we could start another thread to complete this task?
                else if (_threads.Count < _maxThreadCount)
                {
                    var worker = new SmartThreadPoolWorkerThread(this, _threads.Count);
                    _threads.Add(worker);

                    worker.SetWorkItem(action);
                }
                // Nobody free right now so shove the task onto the backlog
                else
                {
                    _workBacklog.Enqueue(action);
                }
            }

            return true;
        }

        public int Terminate(TimeSpan timeoutPerThread)
        {
            var failCount = 0;
            List<SmartThreadPoolWorkerThread> threads;
            lock (_syncLock)
            {
                threads = _threads.ToList();
            }


            foreach (var workerThread in threads)
            {
                if (!workerThread.Terminate(timeoutPerThread))
                {
                    failCount++;
                }
                else
                {
                    _threads.Remove(workerThread);
                }
            }

            return failCount;
        }

        public void TerminateWhenComplete()
        {
            _completeAndTerminate = true;

            foreach (var workerThread in _threads.ToArray())
            {
                workerThread.TerminateIfNoWork();
            }
        }

        internal Action WorkerThreadHasCompleted(SmartThreadPoolWorkerThread smartThreadPoolWorkerThread)
        {
            lock (_syncLock)
            {
                if (_paused || !_workBacklog.Any())
                {
                    // If we are paused then we should not load a new task from the backlog
                    // Dito for trying to terminate
                    // Dito if there are no new tasks to start
                    _freeWorkers.Enqueue(smartThreadPoolWorkerThread);
                    return null;
                }

                var workItem = _workBacklog.Dequeue();
                return workItem;
            }
        }

        public void Pause()
        {
            _paused = true;
        }

        public void UnPause()
        {
            _paused = false;

            lock (_syncLock)
            {
                var workItems = _workBacklog.ToList();
                _workBacklog.Clear();

                foreach (var workItem in workItems)
                {
                    Queue(workItem);
                }
            }
        }

        public void RunOneOnCallerThread()
        {
            Action item;
            lock (_syncLock)
            {
                item = _workBacklog.Dequeue();
            }

            item();
        }

        public void ClearBacklok()
        {
            lock (_syncLock)
            {
                _workBacklog.Clear();
            }
        }
    }
}