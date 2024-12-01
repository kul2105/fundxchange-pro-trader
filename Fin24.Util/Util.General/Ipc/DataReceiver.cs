using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;

namespace Fin24.Util.General.Ipc
{
    public class DataReceiver : IDataReceiver
    {
        public event EventHandler DataAvailable;
        private readonly IChannel _channel;
        private readonly object _syncLock = new object();
        private readonly Queue<object> _data = new Queue<object>();
        private readonly Func<byte[], IList<object>> _deserializer;
        private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //---------------------------------------------------------------------------------*---------/
        public DataReceiver(IChannel channel)
            : this(channel, null)
        {
        }

        //---------------------------------------------------------------------------------*---------/
        public DataReceiver(IChannel channel, Func<byte[], IList<object>> deserializer)
        {
            _channel = channel;
            _channel.ChannelDataReceived += new ChannelActionHandler(DataReceivedFromChannel);
            _deserializer = deserializer;
        }

        //---------------------------------------------------------------------------------*---------/
        public int Count
        {
            get
            {
                lock (_syncLock)
                {
                    return _data.Count;
                }
            }
        }

        //---------------------------------------------------------------------------------*---------/
        public object Read(int timeout)
        {
            object result;
            lock (_syncLock)
            {
                if (_data.Count == 0)
                {
                    if (timeout > 0)
                    {
                        Monitor.Wait(_syncLock, timeout);
                    }

                    if (_data.Count == 0)
                    {
                        return null;
                    }
                }

                result = _data.Dequeue();
            }

            return result;
        }

        //---------------------------------------------------------------------------------*---------/
        public object BlockingRead()
        {
            object result;
            lock (_syncLock)
            {
                while (_data.Count == 0)
                {
                    Monitor.Wait(_syncLock);
                }

                result = _data.Dequeue();
            }

            if ((result != null) && (_deserializer != null) && (result is byte[]))
            {
                return _deserializer((byte[])result);
            }

            return result;
        }

        //---------------------------------------------------------------------------------*---------/
        private void DataReceivedFromChannel(object sender, ChannelActionEventArgs evtArgs)
        {
            LOG.DebugFormat("Received [{0}] bytes from [{1}]", evtArgs.ChannelData.Length, evtArgs.Channel.Uri);

            bool dataAdded = false;
            lock (_syncLock)
            {
                if (_deserializer != null)
                {
                    IList<object> deserializedList = _deserializer(evtArgs.ChannelData);
                    if (deserializedList != null)
                    {
                        foreach (object deserializedObj in deserializedList)
                        {
                            LOG.DebugFormat("Received [{0}]", deserializedObj);
                            _data.Enqueue(deserializedObj);
                            dataAdded = true;
                        }
                        Monitor.PulseAll(_syncLock);
                    }
                }
                else
                {
                    LOG.DebugFormat("Received [{0}] bytes", evtArgs.ChannelData.Length);
                    _data.Enqueue(evtArgs.ChannelData);
                    dataAdded = true;
                    Monitor.PulseAll(_syncLock);
                }

                LOG.DebugFormat("Receive Queue now at [{0}] items", _data.Count);
            }

            if (dataAdded)
            {
                NotifyDataAvailable();
            }
        }

        //---------------------------------------------------------------------------------*---------/
        private void NotifyDataAvailable()
        {
            if (DataAvailable != null)
            {
                foreach (Delegate evtListener in DataAvailable.GetInvocationList())
                {
                    try
                    {
                        evtListener.DynamicInvoke(this, new EventArgs());
                    }
                    catch (Exception err)
                    {
                        LOG.Debug(string.Format("Error executing [{0}:{1}]", evtListener.Target, evtListener.Method, err));
                    }
                }
            }
        }
    }
}
