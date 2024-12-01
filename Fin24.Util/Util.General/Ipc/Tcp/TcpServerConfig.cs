using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Fin24.Util.General.Ipc.Tcp
{
   public class TcpServerConfig : SocketConfig
   {
      private int m_maxConcurrentConnections = 40;
      private string m_listenIp = "127.0.0.1";
      private int m_port = 8080;
      private IPEndPoint m_listendEndPoint = null;
      private int m_connectionBacklog = 100;
      private SocketType m_socketType = SocketType.Stream;
      private ProtocolType m_protocolType = ProtocolType.Tcp;

      //---------------------------------------------------------------------------------*---------/
      public TcpServerConfig()
      {

      }

      //---------------------------------------------------------------------------------*---------/
      public TcpServerConfig(
         string listenIp,
         int? port,
         int? maxConcurrentConnections,
         int? connectionBacklog,
         SocketType? socketType,
         ProtocolType? protocolType,
         int? socketBufferSize)
         : base(socketBufferSize)
      {
         m_maxConcurrentConnections = (maxConcurrentConnections ?? m_maxConcurrentConnections);
         m_listenIp = (listenIp ?? m_listenIp);
         m_port = (port ?? m_port);
         m_connectionBacklog = (connectionBacklog ?? m_connectionBacklog);
         m_socketType = (socketType ?? m_socketType);
         m_protocolType = (protocolType ?? m_protocolType);
      }

      //---------------------------------------------------------------------------------*---------/
      public SocketType SocketType
      {
         get
         {
            return m_socketType;
         }
         set
         {
            m_socketType = value;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public ProtocolType ProtocolType
      {
         get
         {
            return m_protocolType;
         }
         set
         {
            m_protocolType = value;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public IPEndPoint ListenEndPoint
      {
          get
         {
            if (m_listendEndPoint == null)
            {
               if ((m_listenIp == null) || ("ANY" == m_listenIp.ToUpper()))
               {
                  m_listendEndPoint = new IPEndPoint(IPAddress.Any, m_port);
               }
               else
               {
                  m_listendEndPoint = new IPEndPoint(IPAddress.Parse(m_listenIp), m_port);
               }
            }

            return m_listendEndPoint;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public int ConnectionBacklog
      {
         get
         {
            return m_connectionBacklog;
         }
         set
         {
            m_connectionBacklog = value;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public int MaxConcurrentConnections
      {
         get
         {
            return m_maxConcurrentConnections;
         }

         set
         {
            m_maxConcurrentConnections = value;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public string ListenIp
      {
         get
         {
            return m_listenIp;
         }
         set
         {
            m_listenIp = value;
            m_listendEndPoint = null;
         }
      }

      //---------------------------------------------------------------------------------*---------/
      public int Port
      {
         get
         {
            return m_port;
         }
         set
         {
            m_port = value;
            m_listendEndPoint = null;
         }
      }
   }
}