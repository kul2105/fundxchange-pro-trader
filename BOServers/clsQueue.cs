using System;
using System.Collections.Concurrent;
using System.Threading;

namespace PALSA.Cls
{
    public interface IMsgDataReceived<T>
    {
        void OnDataReceived(T obj);
    }

    public class ClsQueue<T> : IDisposable
    {
        #region "                   MEMBERS                 "

        private readonly object _locker = new object();
        private readonly ConcurrentQueue<T> _receiveQueue = new ConcurrentQueue<T>();
        private readonly EventWaitHandle _wh = new AutoResetEvent(false);
        private readonly IMsgDataReceived<T> host;
        private bool _flagProcessor;

        #endregion "                 MEMBERS                 "

        #region "                    METHODS                 "

        public ClsQueue(IMsgDataReceived<T> obj)
        {
            host = obj;
            ThreadPool.QueueUserWorkItem(ReceiveDataWorker);
            _flagProcessor = true;
        }

        public void Dispose()
        {
            _flagProcessor = false;
            _wh.Close();
        }

        public void EnqueueInReceiveQueue(T task)
        {
            _receiveQueue.Enqueue(task);
            _wh.Set();
        }

        private void ReceiveDataWorker(object state)
        {
            while (_flagProcessor)
            {
                T task;
                while (_receiveQueue.Count > 0)
                {
                    if (_receiveQueue.TryDequeue(out task))
                    {
                            host.OnDataReceived(task);
                    }
                }
                _wh.WaitOne();
            }
        }

        #endregion "                  METHODS                 "
    }
}