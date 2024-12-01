using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Fin24.Util.Container;
using log4net;

namespace Fin24.Util.General.Ipc.Tcp
{
   public class TcpServer : IChannelFactory
   {
      private Socket _listenSocket;            // the socket used to listen for incoming connection requests
      private int _numConnectedSockets;      // the total number of clients connected to the server 
      private readonly Semaphore _maxNumberAcceptedClients;
      private readonly TcpServerConfig _serverCfg;
      public event ChannelFactoryEventHandler ChannelCreated;
      private static readonly ILog LOG = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

      //---------------------------------------------------------------------------------*---------/
      public TcpServer(TcpServerConfig cfg)
      {
         _numConnectedSockets = 0;
         _serverCfg = cfg;

         _maxNumberAcceptedClients = new Semaphore(_serverCfg.MaxConcurrentConnections, _serverCfg.MaxConcurrentConnections);
      }

      //---------------------------------------------------------------------------------*---------/
      public void Start()
      {
         _listenSocket = new Socket(_serverCfg.ListenEndPoint.AddressFamily, _serverCfg.SocketType, _serverCfg.ProtocolType);
         _listenSocket.Bind(_serverCfg.ListenEndPoint);
         _listenSocket.Listen(_serverCfg.ConnectionBacklog);

         StartAccept(null);
      }

      //---------------------------------------------------------------------------------*---------/
      private void StartAccept(SocketAsyncEventArgs acceptEventArg)
      {
         if (acceptEventArg == null)
         {
            acceptEventArg = new SocketAsyncEventArgs();
            acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
         }
         else
         {
            // socket must be cleared since the context object is being reused
            acceptEventArg.AcceptSocket = null;
         }

         _maxNumberAcceptedClients.WaitOne();
         LOG.Debug( "Waiting for connection");
         bool resultPending = _listenSocket.AcceptAsync(acceptEventArg);
         if (!resultPending)
         {
            ProcessAccept(acceptEventArg);
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs acceptEvtArgs)
      {
         ProcessAccept(acceptEvtArgs);
      }

      //---------------------------------------------------------------------------------*---------/
      private void ProcessAccept(SocketAsyncEventArgs acceptEvtArgs)
      {
         if (acceptEvtArgs.SocketError != SocketError.Success)
         {
            return;
         }

         LOG.InfoFormat("Connection made from {0}.", acceptEvtArgs.AcceptSocket.RemoteEndPoint);
         int currentlyConnected= Interlocked.Increment(ref _numConnectedSockets);
         LOG.InfoFormat( "Connect: {0} client(s) now connected.", currentlyConnected);

         TcpChannel channel = CreateChannel();
         channel.ChannelDisconnected+= new ChannelActionHandler(ChannelActionHandler);

         channel.SetConnected(acceptEvtArgs.AcceptSocket);

         // Accept the next connection request
         StartAccept(acceptEvtArgs);
      }

      //---------------------------------------------------------------------------------*---------/
      void ChannelActionHandler(object sender, ChannelActionEventArgs evtArgs)
      {
         if (evtArgs.Action == ChannelActionEnum.Closed)
         {
            int currentlyConnected= Interlocked.Decrement(ref _numConnectedSockets);
            _maxNumberAcceptedClients.Release();
            LOG.InfoFormat( "Disconnect: {0} client(s) still connected", currentlyConnected);
         }
      }

      //---------------------------------------------------------------------------------*---------/
      private TcpChannel CreateChannel()
      {
         TcpChannel channel= new TcpChannel();
         NotifyChannelCreated( channel);

         return channel;
      }

      //---------------------------------------------------------------------------------*---------/
      private void NotifyChannelCreated(IChannel newChannel)
      {
         if (ChannelCreated != null)
         {
            foreach (Delegate evtListener in ChannelCreated.GetInvocationList())
            {
               try
               {
                  evtListener.DynamicInvoke(this, new ChannelFactoryEventArgs(newChannel));
               }
               catch (Exception)
               {

               }
            }
         }
      }
   }
}