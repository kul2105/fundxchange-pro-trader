using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using Fin24.Util.General.Container;
using Fin24.Util.General.Threading;
using log4net;

namespace Fin24.Util.General.Ipc.Udp
{
   public class MulticastCannel : IChannel
   {
      public event ChannelActionHandler ChannelConnected;
      public event ChannelActionHandler ChannelDisconnected;
      public event ChannelActionHandler ChannelDataReceived;
      public event ChannelActionHandler ChannelDataSent;

      private Socket _socket;
      private MulticastChannelConfig _config;
      public ISession Session { get; set; }

      private readonly object _syncLock = new object();
      private SocketAsyncEventArgs _sendEvtArgs;
      private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


      //---------------------------------------------------------------------------------*---------/
      public void Configure(MulticastChannelConfig config)
      {
         _config = config;

         _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
         _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

         if ((_config.Usage & MulticastChannelConfig.ChannelType.Receive) == MulticastChannelConfig.ChannelType.Receive)
         {
            IPAddress localAddress;
            if ((_config.LocalInterfaceAddress == null) || (_config.LocalInterfaceAddress.ToUpper() == "ANY"))
            {
               localAddress= IPAddress.Any;
               
            }
            else
            {
               localAddress = Dns.GetHostAddresses(_config.LocalInterfaceAddress)[0];
            }

            IPEndPoint localEndPoint = new IPEndPoint( localAddress, _config.LocalPort);
            _socket.Bind((EndPoint)localEndPoint);

            IPAddress multicastAddr = IPAddress.Parse(_config.MultiCastGroup);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multicastAddr, localAddress));


            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            SocketAsyncEventArgs recvEvtArgs = CreateSocketAsyncEventArgs(remoteEndPoint);

            LOG.DebugFormat( "Waiting for data on multicast {0}:{1}", multicastAddr, _config.LocalPort);

            bool resultPending= _socket.ReceiveFromAsync(recvEvtArgs);
            if (!resultPending)
            {
               ProcessReceive( recvEvtArgs);
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Send(byte[] data)
      {
         lock (_syncLock)
         {
            Future<SocketError> sendReuslt = SendAsync(data);
            //TODO Handle errors
            sendReuslt.WaitOne();
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public Future<SocketError> SendAsync (byte []data)
      {
         lock (_syncLock)
         {
            Future<SocketError> sendResult= new Future<SocketError>();
            if (_sendEvtArgs == null)
            {
               IPAddress multicastAddr = IPAddress.Parse(_config.MultiCastGroup);
               EndPoint remoteEndPoint = new IPEndPoint(multicastAddr, _config.MulticastPort);
               _sendEvtArgs = CreateSocketAsyncEventArgs(remoteEndPoint);
            }

            _sendEvtArgs.SetBuffer(data, 0, data.Length);
            _sendEvtArgs.UserToken = sendResult;
            _socket.SendToAsync(_sendEvtArgs);

            return sendResult;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Close()
      {
         IPAddress multicastAddr = IPAddress.Parse(_config.MultiCastGroup);

         _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(multicastAddr, IPAddress.Any));
         if (_socket.Connected)
         {
            try
            {
               LOG.Debug("Closing channel");

               _socket.Shutdown(SocketShutdown.Both);
               _socket.Close();
            }
            catch (Exception err)
            {
               LOG.Debug( "Error closing socket", err);
            }
         }
         
      }

      //---------------------------------------------------------------------------------*---------/
      public string Uri
      {
         get
         {
            throw new NotImplementedException();
         }
         set
         {
            throw new NotImplementedException();
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public bool Connected
      {
         get
         {
            return true;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void ProcessReceive(SocketAsyncEventArgs recvEvtArgs)
      {
         if (recvEvtArgs.SocketError != SocketError.Success)
         {
            //TODO Handle Error
            return;
         }

         LOG.DebugFormat( "Received {0} bytes from {1}", recvEvtArgs.BytesTransferred, recvEvtArgs.AcceptSocket.RemoteEndPoint);
         byte []buffer= new byte[ recvEvtArgs.BytesTransferred];
         Buffer.BlockCopy( recvEvtArgs.Buffer, 0, buffer, 0, buffer.Length);
         NotifyDataReceived( buffer);

         bool resultPending = _socket.ReceiveFromAsync(recvEvtArgs);
         //TODO Possible infinite recursion
         if (!resultPending)
         {
            ProcessReceive( recvEvtArgs);
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void ProcessSend(SocketAsyncEventArgs sendEvtArgs)
      {
         Future<SocketError> future = (Future<SocketError>) sendEvtArgs.UserToken;

         if (sendEvtArgs.SocketError != SocketError.Success)
         {
            future.Result = sendEvtArgs.SocketError;
            LOG.ErrorFormat( "Error {0} sending to socket {1}", sendEvtArgs.SocketError, sendEvtArgs.AcceptSocket.RemoteEndPoint);
            //TODO Handle error
         }
         else
         {
            LOG.DebugFormat("Sent {0} bytes to {1}", sendEvtArgs.BytesTransferred,
                            sendEvtArgs.AcceptSocket.RemoteEndPoint);

            NotifyDataSent( sendEvtArgs.Buffer);
            future.Result = SocketError.Success;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void SocketEventCompleted(object sender, SocketAsyncEventArgs socketEvt)
      {
         if (socketEvt.LastOperation == SocketAsyncOperation.ReceiveFrom)
         {
            ProcessReceive( socketEvt);
            return;
         }

         if (socketEvt.LastOperation == SocketAsyncOperation.SendTo)
         {
            ProcessSend( socketEvt);
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifyDataReceived(byte[] data)
      {
         if (ChannelDataReceived != null)
         {
            foreach (Delegate evtListener in ChannelDataReceived.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, new ChannelActionEventArgs(this, ChannelActionEnum.Receive, data));
               }
               catch (Exception err)
               {
                  LOG.Error( string.Format( "Error notifying {0} of data received", evtListener.Method), err);
               }
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifyDataSent(byte[] data)
      {
         if (ChannelDataSent!= null)
         {
            foreach (Delegate evtListener in ChannelDataReceived.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, new ChannelActionEventArgs(this, ChannelActionEnum.Send, data));
               }
               catch (Exception err)
               {
                  LOG.Error(string.Format("Error notifying {0} of data sent", evtListener.Method), err);
               }
            }
         }
      }
      //---------------------------------------------------------------------------------*---------/
      private SocketAsyncEventArgs CreateSocketAsyncEventArgs(EndPoint endPoint)
      {
         byte[] buffer = new byte[1024 * 64];

         SocketAsyncEventArgs recvEvtArgs = new SocketAsyncEventArgs();
         recvEvtArgs.SetBuffer(buffer, 0, buffer.Length);
         recvEvtArgs.Completed += new EventHandler<SocketAsyncEventArgs>(SocketEventCompleted);
         recvEvtArgs.RemoteEndPoint = endPoint;
         return recvEvtArgs;
      }

   }
}
