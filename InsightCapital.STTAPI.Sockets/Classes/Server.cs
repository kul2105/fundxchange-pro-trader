using System;
using System.Collections.Generic;
using System.Collections;
using InsightCapital.STTAPI.Sockets.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using InsightCapital.STTAPI.MessageLibrary;
using InsightCapital.STTAPI.MessageLibrary.Utility;
using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;

namespace InsightCapital.STTAPI.Sockets.Classes
{
    public class Server : IDisposable, IServerEvents
    {
        #region Variables

        private bool _disposed;
        public delegate void LogHandler(string format, params object[] args);
        public LogHandler Log;
        private Socket DSServer;
        private bool IsStoped;
        private bool IsDisposed;
        private Thread AcceptThread;
        private object SyncHandle = new object();
        private object Lck4Broadcast = new object();
        private readonly Dictionary<int, Type> Structures_ = new Dictionary<int, Type>();
        private List<Client> Clients = new List<Client>();
        ArrayList arClients = ArrayList.Synchronized(new ArrayList());
        public bool EnableHeartBeat = true;
        private IPEndPoint ServerAddress;
        DataType dType4DataSending;
        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Dispose(false);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            DoStop(disposing);

            _disposed = true;
        }

        #endregion

        #region Implementation of IServerEvents

        public event StartingHandler Starting = delegate { };
        public event StartingHandler Started = delegate { };
        public event IncommingConnectionHandler IncommingConnection = delegate { };
        public event ExceptionHandler Exception = delegate { };
        public event ClientConnectedHandler ClientOnConnect = delegate { };
        public event ClientDisconnectedHandler ClientDisconnected = delegate { };
        public event MessageHandler Message = delegate { };
        public event StopingHandler Stoping = delegate { };
        public event StopedHandler Stoped = delegate { };


        #endregion

        #region ClientEvents

        public event Client.DataReceivedHandler DataReceived;
        public delegate void ClientNotAuthenticatedHandler(Server sender, string clientId);

        public event Client.OnExceptionHandler ClientExceptionHandler;

        #endregion

        private void DoStop(bool disposing)
        {
            IsStoped = true;

            if (disposing)
            {
                //Stop Server
                if (DSServer != null)
                    DSServer.Close();

                //Stop thread, No more incomming connections
                if (AcceptThread != null)
                {
                    AcceptThread.Abort();
                    AcceptThread = null;
                }

                //Close All clients
                lock (Lck4Broadcast)
                {
                    foreach (Client client in Clients)
                    {
                        if (!client.Disposed)
                            client.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Start Server to Listen for Client Connection
        /// </summary>
        /// <param name="localEndPoint"></param>
        /// 
        public void Start(IPEndPoint localEndPoint, DataType dType)
        {
            dType4DataSending = dType;
            IsStoped = false;
            AcceptThread = new Thread(DoListen);
            AcceptThread.Start(localEndPoint);
        }

        private void DoListen(object state)
        {
            try
            {
                DSServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerAddress = state as IPEndPoint;
                DSServer.Bind(ServerAddress);
                DSServer.Listen(100);
                DSServer.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                throw ex;
            }
        }

        public void ResumeServer()
        {
            try
            {
                if (DSServer == null)
                {
                    DSServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    DSServer.Bind(ServerAddress);
                    DSServer.Listen(100);
                    DSServer.BeginAccept(new AsyncCallback(OnClientConnect), null);
                }
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                throw ex;
            }
        }

        public void PauseServer()
        {
            if (DSServer != null)
            {
                DSServer.Close();
                DSServer = null;
            }
        }

        private void OnClientConnect(IAsyncResult asyn)
        {
            Client client = null;
            try
            {
                client = AddClient(DSServer.EndAccept(asyn));
                DSServer.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                CloseClientSocket(client);
            }
            finally
            {
                DSServer.BeginAccept(new AsyncCallback(OnClientConnect), null);
            }
        }

        private Client AddClient(Socket socket)
        {
            Client client = new Client(dType4DataSending);
            client.Attach(socket);
            client.setProtocolStructs(Structures_);
            SetDesiredKeepAlive(socket);
            client.DataReceived += ClientOnDataReceived;
            client.OnException += new Client.OnExceptionHandler(client_OnException);
            lock (Lck4Broadcast)
            {
                Clients.Add(client);
            }

            ClientOnConnect(client, client.ClientID);
            IncommingConnection(this, (IPEndPoint)socket.RemoteEndPoint);
            DoReceiveAsync(client);
            return client;
        }

        void client_OnException(Client sender, Exception exception, bool? okToContinue)
        {
            ClientExceptionHandler?.Invoke(sender, exception, true);
        }

        private void DoReceiveAsync(Client client)
        {
            try
            {
                client.StartReceiving();
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                Debug.WriteLine("EXCEPTION: DoReceiveAsync : " + ex.Message);
                CloseClientSocket(client);
            }
        }

        private void ClientReceive_CB(IAsyncResult result)
        {
            Client client = (Client)result.AsyncState;
            if (result.IsCompleted)
            {
                try
                {
                    if (!client.EndReceive(result))
                    {
                        CloseClientSocket(client);
                        return;
                    }
                    if (!IsStoped)
                        DoReceiveAsync(client);
                }
                catch (ProtocolParser.ProtocolParserException ex)
                {
                    FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                }
                catch (Exception ex)
                {
                    FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                    CloseClientSocket(client);
                }
            }
        }

        public void Broadcast(IProtocolStruct @struct)
        {
            List<Client> _clientsToDelete = new List<Client>();
            int totClients = Clients.Count;
            System.Diagnostics.Debug.WriteLine("Broadcast cnt" + totClients);
            for (int i = 0; i < totClients; i++)
            {
                Client cl = Clients[i];
                try
                {
                    lock (cl)
                    {
                        if (cl.Disposed || cl.IsClosed)
                        {
                            _clientsToDelete.Add(cl);
                        }
                        else
                        {
                            cl.BeginSend(@struct,
                                         result =>
                                         {
                                             try
                                             {
                                                 cl.EndSend(result);
                                                 //int res = cl.EndSend(result);
                                                 //if (res == -1)
                                                 //    _clientsToDelete.Add(cl);
                                             }
                                             catch (Exception ex)
                                             {
                                                 FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                                                 //if (!CheckException(ex, _endReceiveExceptions))
                                                 //    throw;
                                                 _clientsToDelete.Add(cl);
                                             }
                                         });
                        }
                    }
                }
                catch (Exception ex)
                {
                    FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                    _clientsToDelete.Add(cl);
                }
                totClients = Clients.Count;
            }


            foreach (var client in _clientsToDelete)
            {
                lock (Lck4Broadcast)
                {
                    Clients.Remove(client);
                }
                CloseClientSocket(client);
            }
        }
        object lck4Structure = new object();
        public void DoSendAsync(Client client, List<IProtocolStruct> structures)
        {
            try
            {
                if (client.Disposed || !client.Connected)
                {
                    Debug.WriteLine(
                      string.Format("[SocketServer] Can't DOSendAsync - Client '{0}' is Disposed '{1}' or Disconnected '{2}'",
                                    client.Id, client.Disposed, !client.Connected));
                    CloseClientSocket(client);
                    return;
                }

                lock (lck4Structure)
                {

                    for (int i = 0; i < structures.Count; i++)
                    {
                        client.BeginSend(structures[i], ClientSend_CB);
                        structures.Remove(structures[i]);
                        i -= 1;
                    }
                    //foreach (var @struct in structures)
                    //{
                    //    client.BeginSend(@struct, ClientSend_CB);
                    //}
                }
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                Debug.WriteLine("EXCEPTION: DoSendAsync : " + ex.Message);
                CloseClientSocket(client);
            }
        }
        public void DoSendAsync(Client client, IProtocolStruct structure)
        {
            try
            {
                if (client.Disposed || !client.Connected)
                {
                    Debug.WriteLine(
                      string.Format("[SocketServer] Can't DOSendAsync - Client '{0}' is Disposed '{1}' or Disconnected '{2}'",
                                    client.Id, client.Disposed, !client.Connected));
                    CloseClientSocket(client);
                    return;
                }


                client.BeginSend(structure, ClientSend_CB);
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                Debug.WriteLine("EXCEPTION: DoSendAsync : " + ex.Message);
                CloseClientSocket(client);
            }
        }


        private void ClientSend_CB(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                Client client = (Client)result.AsyncState;
                try
                {
                    if (client.EndSend(result) == 0)
                    {
                        Debug.WriteLine(string.Format("[ServerSocket] Could not send data to client '{0}'. Closing it.", client.Id));
                        CloseClientSocket(client);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                    Debug.WriteLine("EXCEPTION: ClientSend_CB : " + ex.Message);
                    CloseClientSocket(client);
                    return;
                }
            }
        }

        private void CloseClientSocket(Client client)
        {
            if (client == null)
                return;

            try
            {
                //raise the event 
                ClientDisconnected(this, client.Id);
                //close the socket associated with this client
                client.Close();
                //remove from list of clients
                lock (Lck4Broadcast)
                {
                    if (Clients.Contains(client))
                        Clients.Remove(client);
                }
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
            }

        }

        public void DeleteClient(Client client)
        {
            lock (Lck4Broadcast)
            {
                Clients.Remove(client);
            }
            CloseClientSocket(client);
        }

        private void ClientOnDataReceived(Client client, IProtocolStruct structure)
        {
            if (DataReceived != null)
                DataReceived(client, structure);
        }

        public void RegisterStructure(int Id, Type structureType)
        {
            if (!Structures_.ContainsKey(Id))
                Structures_.Add(Id, structureType);
        }

        private static void SetDesiredKeepAlive(Socket socket)
        {
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            const uint time = 20000;
            const uint interval = 30000;
            SetKeepAlive(socket, true, time, interval);
        }

        static void SetKeepAlive(Socket s, bool on, uint time, uint interval)
        {
            uint dummy = 0;
            var inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(on ? 1 : 0)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)time).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)interval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
            int ignore = s.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }

        public int GetClientCount()
        {
            return Clients.Count;
        }
    }
}
