using System;
using System.Collections.Generic;
using System.Threading;

namespace FundXchange.Domain
{
    public delegate void MessageReceivedDelegate<T>(T message);

    public class MessageHandler<T> : IDisposable
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly object m_lock = new object();
        private readonly string _identfier;
        private Thread _receiveThread;
        private bool _receiving = true;

        public event MessageReceivedDelegate<T> MessageReceived;
        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        public MessageHandler(string identifier)
        {
            _identfier = identifier;
            _receiveThread = new Thread(Receive)
                                 {
                                     Name = String.Format("MessageHandlerThread_{0}", identifier),
                                     IsBackground = true
                                 };
            _receiveThread.Start();
        }

        public void AddMessage(T message)
        {
            lock (m_lock)
            {
                _queue.Enqueue(message);
                Monitor.PulseAll(m_lock);
            }
        }

        public void Start()
        {
            _manualResetEvent.Set();
        }

        public void Pause()
        {
            _manualResetEvent.Reset();
        }

        private void Receive()
        {
            while (_receiving)
            {
                _manualResetEvent.WaitOne();

                var messages = new List<T>();

                lock (m_lock)
                {
                    while (_queue.Count == 0)
                        Monitor.Wait(m_lock);

                    T message;
                    if (_queue.Count > 10)
                    {
                        while (_queue.Count > 1)
                        {
                            message = _queue.Dequeue();
                            messages.Add(message);
                        }
                    }
                    message = _queue.Dequeue();
                    messages.Add(message);
                }
                foreach (T msg in messages)
                {
                    MessageReceived(msg);
                }
            }
        }

        public void Dispose()
        {
            _receiving = false;
            try
            {
                if (_receiveThread!=null)
                {
                    _receiveThread.Abort();
                    _receiveThread = null;
                }
            }
            catch
            { }
        }
    }
}
