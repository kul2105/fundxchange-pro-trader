using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Fin24.Util.General.Container;
using Fin24.Util.General.Threading;
using log4net;

namespace Fin24.Util.General.Ipc.Tcp
{
   public class TcpChannel : IChannel 
   {
      public event ChannelActionHandler ChannelConnected;
      public event ChannelActionHandler ChannelDisconnected;
      public event ChannelActionHandler ChannelDataReceived;
      public event ChannelActionHandler ChannelDataSent;

      private SocketAsyncEventArgs _recvEventArgs;
      private SocketAsyncEventArgs _sendEventArgs;
      private const int BUFFER_SIZE = 1024*4;
      public ISession Session { get; set; }
      private string _uri;
      private readonly object _syncLock = new object();
      private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
      private bool _connected = false;
      private bool _receiveEnabled = true;


      //---------------------------------------------------------------------------------*---------/
      public TcpChannel()
      {
      }

      //---------------------------------------------------------------------------------*---------/
      public TcpChannel(bool receiveEnabled)
      {
         _receiveEnabled = receiveEnabled;
      }

      //---------------------------------------------------------------------------------*---------/
      public void Connect (string target, int port)
      {
         Socket clientSock= new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
         IPAddress destAddr = GetIP4InterfaceAddress(target);

         if (destAddr == null)
         {
            throw new Exception(string.Format("Invalid destination address {0}:{1}", target, port));
         }

         clientSock.Connect( destAddr, port);

         SetConnected( clientSock);
      }

      //---------------------------------------------------------------------------------*---------/
      private IPAddress GetIP4InterfaceAddress(string target)
      {
         IPAddress []possibleAddresses = Dns.GetHostAddresses(target);
         IPAddress destAddr= null;
         foreach (IPAddress address in possibleAddresses)
         {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
               destAddr = address;
               break;
            }
         }
         return destAddr;
      }

      //---------------------------------------------------------------------------------*---------/
      public void SetConnected (Socket acceptedSocket)
      {
         _connected = true;
         AsyncUserToken userToken = new AsyncUserToken();
         ConfigureForReceive( acceptedSocket, userToken);
         ConfigureForSend( acceptedSocket, userToken);
         NotifyConnected();

         if (_receiveEnabled)
         {
            StartReceive();
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void ConfigureForReceive(Socket channelSocket, AsyncUserToken userToken)
      {
         _recvEventArgs = new SocketAsyncEventArgs();
         _recvEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
         _recvEventArgs.SetBuffer(new byte[BUFFER_SIZE], 0, BUFFER_SIZE);
         _recvEventArgs.UserToken = userToken;
         _recvEventArgs.AcceptSocket = channelSocket;
         _recvEventArgs.AcceptSocket.LingerState = new LingerOption(false, 0);
      }

      //---------------------------------------------------------------------------------*---------/
      private void ConfigureForSend(Socket channelSocket, AsyncUserToken userToken)
      {
         _sendEventArgs = new SocketAsyncEventArgs();
         _sendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
         _sendEventArgs.UserToken = userToken;
         _sendEventArgs.AcceptSocket = channelSocket;
         _sendEventArgs.AcceptSocket.NoDelay = true;
         _sendEventArgs.AcceptSocket.LingerState= new LingerOption(false, 0);

         SetUri();
      }

      //---------------------------------------------------------------------------------*---------/
      private void StartReceive ()
      {
         // As soon as the client is connected, post a receive to the connection
         bool resultPending = _recvEventArgs.AcceptSocket.ReceiveAsync(_recvEventArgs);
         if (!resultPending)
         {
            ProcessReceive(_recvEventArgs);
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void IO_Completed(object sender, SocketAsyncEventArgs ioEvtArgs)
      {
         // determine which type of operation just completed and call the associated handler
         switch (ioEvtArgs.LastOperation)
         {
            case SocketAsyncOperation.Receive:
               ProcessReceive(ioEvtArgs);
               break;
            case SocketAsyncOperation.Send:
               ProcessSend(ioEvtArgs);
               break;
            default:
               throw new ArgumentException("The last operation completed on the socket was not a receive or send");
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void ProcessReceive(SocketAsyncEventArgs recvEvtArgs)
      {
         if (recvEvtArgs.BytesTransferred > 0 && recvEvtArgs.SocketError == SocketError.Success)
         {
            LOG.DebugFormat( "Received {0} bytes from {1}", recvEvtArgs.BytesTransferred, Uri);
            AsyncUserToken token = (AsyncUserToken)recvEvtArgs.UserToken;

            byte[] dataReceived = new byte[recvEvtArgs.BytesTransferred];
            Buffer.BlockCopy(recvEvtArgs.Buffer, recvEvtArgs.Offset, dataReceived, 0, recvEvtArgs.BytesTransferred);

            NotifyDataReceived(dataReceived);

            if (!recvEvtArgs.AcceptSocket.Connected)
            {
               return;
            }

            bool resultPending = recvEvtArgs.AcceptSocket.ReceiveAsync(recvEvtArgs);
            if (!resultPending)
            {
               ProcessReceive(recvEvtArgs);
            }
         }
         else
         {
            CloseClientSocket(recvEvtArgs);
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public void Send(byte[] data)
      {
         lock (_syncLock)
         {
            Future<SocketError> waitHandle = SendAsync(data);
            waitHandle.WaitOne();
            return;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public Future<SocketError> SendAsync (byte []data)
      {
         Future<SocketError> waitHandle = new Future<SocketError>();
         ((AsyncUserToken)_sendEventArgs.UserToken).SyncLock = waitHandle;
         _sendEventArgs.SetBuffer(data, 0, data.Length);

         try
         {
            if (!_sendEventArgs.AcceptSocket.Connected)
            {
               waitHandle.Result = SocketError.NotConnected;
               return waitHandle;
            }
         }
         catch (Exception e)
         {
            waitHandle.Result = SocketError.NotConnected;
            return waitHandle;
         }

         bool resultPending = _sendEventArgs.AcceptSocket.SendAsync(_sendEventArgs);
         if (!resultPending)
         {
            ProcessSend(_sendEventArgs);
         }

         return waitHandle;

      }

      //---------------------------------------------------------------------------------*---------/
      public bool Connected
      {
         get
         {
            if (_sendEventArgs == null)
            {
               return false;
            }

            if (_sendEventArgs.AcceptSocket == null)
            {
               return false;
            }

            return _sendEventArgs.AcceptSocket.Connected;
         }
      }


      //---------------------------------------------------------------------------------*---------/
      private void ProcessSend(SocketAsyncEventArgs sendEvtArgs)
      {
         AsyncUserToken userToken = (AsyncUserToken)sendEvtArgs.UserToken;

         if (sendEvtArgs.SocketError != SocketError.Success)
         {
            CloseClientSocket(sendEvtArgs);
         }
         else
         {
            NotifyDataSent(sendEvtArgs.Buffer);
            LOG.DebugFormat("Sent {0} bytes to {1}", sendEvtArgs.BytesTransferred, Uri);
         }

         userToken.SyncLock.Result = sendEvtArgs.SocketError;
      }

      //---------------------------------------------------------------------------------*---------/
      private void CloseClientSocket(SocketAsyncEventArgs closeEvtArgs)
      {
         AsyncUserToken userToken = (AsyncUserToken)closeEvtArgs.UserToken;

         lock (userToken)
         {
            if (userToken.Closed)
            {
               return;
            }

            userToken.Closed = true;
         }

         string endPoint = Uri;

         try
         {
            closeEvtArgs.AcceptSocket.Shutdown(SocketShutdown.Both);
         }
         catch (Exception err)
         {
            LOG.Debug( string.Format("Error shutting down socket from {0}", endPoint), err);
         }

         try
         {
            closeEvtArgs.AcceptSocket.Close();
         }
         catch (Exception err)
         {
            LOG.Debug(string.Format("Error closing socket from {0}", endPoint), err);
         }

         _connected = false;
         LOG.DebugFormat("{0} disconnected", endPoint);
         NotifyDisconnected();
      }

      //---------------------------------------------------------------------------------*---------/
      public void Close()
      {
         CloseClientSocket( _sendEventArgs);
      }

      //---------------------------------------------------------------------------------*---------/
      public string Uri
      {
         get
         {
            return _uri;
         }

         set
         {
            throw new ArgumentException( "Cannot set URI");
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void SetUri ()
      {
         //If it is bound, it means we're acting as the server
         // And thus we want to use the remote ip/port as the uri
         if (_sendEventArgs.AcceptSocket.IsBound)
         {
            _uri = "tcp://" + _sendEventArgs.AcceptSocket.RemoteEndPoint;
         }
         else
         {
            _uri = "tcp://" + _sendEventArgs.AcceptSocket.LocalEndPoint;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifyConnected()
      {
         if (ChannelConnected != null)
         {
            ChannelActionEventArgs evt = new ChannelActionEventArgs(this, ChannelActionEnum.Connected);
            foreach (Delegate evtListener in ChannelConnected.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, evt);
               }
               catch (Exception)
               {

               }
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifyDisconnected()
      {
         if (ChannelDisconnected != null)
         {
            ChannelActionEventArgs evt = new ChannelActionEventArgs(this, ChannelActionEnum.Closed);

            foreach (Delegate evtListener in ChannelDisconnected.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, evt);
               }
               catch (Exception err)
               {
                  LOG.Error( string.Format("Erroring invoking {0}:{1}", evtListener.Target, evtListener.Method), err);

               }
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifyDataReceived(byte []data)
      {
         if (ChannelDataReceived != null)
         {
            ChannelActionEventArgs evt = new ChannelActionEventArgs(this, ChannelActionEnum.Receive, data);

            foreach (Delegate evtListener in ChannelDataReceived.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, evt);
               }
               catch (Exception err)
               {
                  LOG.Warn(string.Format(evtListener.Target + " threw an exception [{0}]", err), err);
               }
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifyDataSent(byte[] data)
      {
         if (ChannelDataSent != null)
         {
            ChannelActionEventArgs evt = new ChannelActionEventArgs(this, ChannelActionEnum.Send, data);

            foreach (Delegate evtListener in ChannelDataSent.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, evt);
               }
               catch (Exception err)
               {
                  LOG.Warn(string.Format(evtListener.Target + " threw an exception [{0}]", err), err);
               }
            }
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public override string ToString()
      {
         return Uri;
      }

   }
}