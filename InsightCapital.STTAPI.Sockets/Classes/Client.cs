using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using InsightCapital.STTAPI.MessageLibrary;
using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;
using InsightCapital.STTAPI.MessageLibrary.Utility;

namespace InsightCapital.STTAPI.Sockets.Classes
{
    public class Client : IDisposable
    {

        #region Variables

        private bool IsDisposed;
        public bool IsClosed;

        private readonly object SendSync = new object();
        private readonly object RecvSync = new object();

        private readonly IParser Parser;
        //private readonly ProtocolParser proParser;
        //private readonly StringParser strParser;

        DataType DataType4Client;

        public Socket Clientsocket;
        protected byte[] BuffeforReceive; //Receive Buffer
        protected byte[] BufferForSend; //Send Buffer
        public int BufferSize = 65535;
        protected int OffsetForReceive, OffsetForSend;

        public string Id;

        private readonly Dictionary<int, Type> structures = new Dictionary<int, Type>();

        public delegate void DataReceivedHandler(Client sender, IProtocolStruct structure);
        public event DataReceivedHandler DataReceived;

        public delegate void OnExceptionHandler(Client sender, Exception exception, bool? okToContinue);
        public event OnExceptionHandler OnException;

        public event Action<Client> OnConnecting;
        private IAsyncResult ObjAsyncReceiveResult, objAsyncSendResult;

        public event Action<Client> OnConnected;

        public delegate void OnDisconnectHandler(Client sender, Exception exception, bool isDisposing);
        public event OnDisconnectHandler OnDisconnection;
        public bool EnableHeartBeat = true;
        IPAddress ipAddress;
        int port;

        #endregion

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetBufferSize(int BufSize)
        {
            BufferSize = BufSize;
        }

        private void Dispose(bool disposing)
        {
            if (Clientsocket != null)
            {
                Clientsocket.Close();
            }
            if (IsDisposed)
                return;
            IsDisposed = true;
        }

        public void Close()
        {
            try
            {
                if (Clientsocket != null)
                {
                    Clientsocket.Close();
                }
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
            }
        }

        public string ClientID
        {
            get
            {
                if (Id != null)
                    return Id;
                else
                    return "NULL ID";
            }
        }

        public EndPoint GetIPAddress()
        {
            if (Clientsocket != null)
                return Clientsocket.RemoteEndPoint;
            else
                return null;
        }

        public Client(DataType DataType4Client_)
        {
            DataType4Client = DataType4Client_;

            Id = "NEW CLIENT";
            BuffeforReceive = new byte[BufferSize];
            BufferForSend = new byte[BufferSize];
            IsDisposed = false;

            switch (DataType4Client_)
            {
                case DataType.BinaryType:
                    Parser = new ProtocolParser();
                    Parser.PSRead += (ClientParser, structure) =>
                    {
                        if (DataReceived != null)
                        {
                           DataReceived(this, structure);
                        }
                    };
                    break;
                case DataType.StringType:
                    Parser = new StringParser();
                    Parser.PSRead += (ClientParser, structure) =>
                    {
                        if (DataReceived != null)
                        {
                            DataReceived(this, structure);
                        }
                    };
                    break;
                default:
                    break;
            }

        }

        public void Attach(Socket socket)
        {
            IsDisposed = false;
            IsClosed = false;
            Clientsocket = socket;
        }

        public void setProtocolStructs(Dictionary<int, Type> structures)
        {
            Parser.Structs_ = structures;
        }

        public void RegisterStructure(int structId, Type structureType)
        {
            try
            {
                if (!structures.ContainsKey(structId))
                {
                    structures.Add(structId, structureType);
                }
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                Console.WriteLine("");
            }
        }

        private void InvokeOnConnecting(Client sender)
        {
            Action<Client> connecting = OnConnecting;
            if (connecting != null) connecting(sender);
        }

        private void InvokeOnConnected(Client sender)
        {
            Action<Client> connected = OnConnected;
            if (connected != null) connected(sender);
        }

        private void InvokeOnException(Exception exception, bool? okToContinue)
        {
            OnExceptionHandler exceptionHandler = OnException;
            if (exceptionHandler != null)
                exceptionHandler(this, exception, okToContinue);

            if (okToContinue == false)
                Dispose();

            if (OnDisconnection != null)
                OnDisconnection(this, exception, true);
        }

        public bool Disposed
        {
            get { return IsDisposed; }
        }

        public bool Closed
        {
            get { return IsClosed; }
        }

        public bool Connected
        {
            get { return Clientsocket != null && Clientsocket.Connected && !IsClosed && !IsDisposed; }
        }

        public void Connect(IPAddress ipAddress, int port)
        {
            //switch (DataType4Client)
            //{
            //    case DataType.BinaryType:
            Parser.Structs_ = structures;
            //        break;
            //    case DataType.StringType:
            //        Parser.Structs_ = structures;
            //        break;
            //    default:
            //        break;
            //}
            try
            {
                this.ipAddress = ipAddress;
                this.port = port;
                InvokeOnConnecting(this);
                if (Clientsocket == null)
                    Clientsocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                LingerOption lo = new LingerOption(true, 2);
                Clientsocket.LingerState = lo;
                Clientsocket.Connect(ipAddress, port);
                Clientsocket.NoDelay = true;
                BuffeforReceive = new byte[BufferSize];
                BufferForSend = new byte[BufferSize];
                OffsetForReceive = 0;
                IsClosed = false;
                InvokeOnConnected(this);
            }
            catch (Exception exception)
            {
                FileHandling.WriteToLogEx("Exception:" + exception.Message + ":" + exception.StackTrace);
                Console.WriteLine(exception.ToString());
                InvokeOnException(exception, false);
            }
        }

        public void ResumeClient()
        {
            try
            {
                InvokeOnConnecting(this);
                if (Clientsocket == null)
                    Clientsocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                Clientsocket.Connect(ipAddress, port);
                Clientsocket.NoDelay = true;
                BuffeforReceive = new byte[BufferSize];
                BufferForSend = new byte[BufferSize];
                OffsetForReceive = 0;
                IsClosed = false;
                InvokeOnConnected(this);
            }
            catch (Exception ex)
            {
                FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                throw ex;
            }
        }

        public void PauseClient()
        {
            if (Clientsocket != null)
            {
                Clientsocket.Close();
                Clientsocket = null;
            }
        }

        public void StartReceiving()
        {
            BeginReceive(BufferSize, Receive_CB);
        }

        public void BeginReceive(int bufferSize, AsyncCallback callback)
        {
            lock (RecvSync)
            {
                try
                {
                    //System.Diagnostics.Debug.WriteLine("From Begin");
                    SocketError error;
                    BuffeforReceive = new byte[BufferSize];
                    ObjAsyncReceiveResult = Clientsocket.BeginReceive(BuffeforReceive, OffsetForReceive, bufferSize, SocketFlags.None, out error, callback, this);
                    if (error != SocketError.Success)
                    {
                        FileHandling.WriteToLogEx(string.Format("[SocketClient] BeginReceive finished with error '{0}'", error));
                        Debug.WriteLine(string.Format("[SocketClient] BeginReceive finished with error '{0}'", error));
                    }
                }
                catch (Exception exception)
                {
                    FileHandling.WriteToLogEx("Exception:" + exception.Message + ":" + exception.StackTrace);
                    InvokeOnException(exception, false);
                }
            }
        }

        private void Receive_CB(IAsyncResult result)
        {
            if (!EndReceive(result))
            {
                this.IsDisposed = true;
                FileHandling.WriteToLogEx(string.Format("[Client Socket] EndReceive said NO. Can't Receive more."));
                Debug.WriteLine("[Client Socket] EndReceive said NO. Can't Receive more.");
                return;
            }
            StartReceiving();
        }

        public bool EndReceive(IAsyncResult result)
        {
            lock (RecvSync)
            {
                if (ObjAsyncReceiveResult == null)
                    return true;
                if (!result.IsCompleted)
                    return true;
                if (result != this.ObjAsyncReceiveResult)
                    return true;

                Client client = (Client)result.AsyncState;
                if (client.Disposed || client.IsClosed)
                {
                    FileHandling.WriteToLogEx("[SocketClient] Can't EndReceive struct cause Disposed='{0}' or Close='{1}'" + Disposed + "--" + IsClosed);
                    Debug.WriteLine(string.Format("[SocketClient] Can't EndReceive struct cause Disposed='{0}' or Close='{1}'", Disposed, IsClosed));
                    return false;
                }
                try
                {
                    SocketError socketError;
                    System.Diagnostics.Debug.WriteLine("From Begin");
                    if (client.Clientsocket == null || result == null)
                        return false;
                    int read = client.Clientsocket.EndReceive(result, out socketError);

                    if (socketError != SocketError.Success)
                    {

                        FileHandling.WriteToLogEx("[SocketClient] Could not receive. Error = '{0}'" + socketError + "--" + read);
                        Debug.WriteLine(string.Format("[SocketClient] Could not receive. Error = '{0}'", socketError, read));
                        return false;
                    }
                    if (read <= 0)
                    {
                        FileHandling.WriteToLogEx("[SocketClient] Received a negative quantity '{0}' " + read + "--" + socketError);
                        Debug.WriteLine(string.Format("[SocketClient] Received a negative quantity '{0}'", read, socketError));
                        return false;
                    }
                    //switch (DataType4Client)
                    //{
                    //    case DataType.BinaryType:
                    client.Parser.Read(client.BuffeforReceive, read);
                    client.Parser.Parse();
                    //    break;
                    //case DataType.StringType:
                    //    client.proParser.Write(client.BuffeforReceive, read);
                    //    client.strParser.Parse();
                    //    break;
                    //default:
                    //    break;
                    //}
                }
                catch (ArgumentException ex)
                {
                    FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                    Debug.WriteLine(ex.ToString());
                }
                catch (ObjectDisposedException ex)
                {
                    FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                    Debug.WriteLine(ex.ToString());
                }
                catch (Exception exception)
                {
                    FileHandling.WriteToLogEx("Exception:" + exception.Message + ":" + exception.StackTrace);
                    client.InvokeOnException(exception, false);
                }
            }
            return true;
        }

        public void BeginSend(IProtocolStruct structure, AsyncCallback callback)
        {
            lock (SendSync)
            {
                if (Disposed || IsClosed)
                {
                    FileHandling.WriteToLogEx(string.Format("[SocketClient] Can't BeginSend cause Disposed='{0}' or Close='{1}'", Disposed,
                                                  IsClosed));
                    Debug.WriteLine(string.Format("[SocketClient] Can't BeginSend cause Disposed='{0}' or Close='{1}'", Disposed,
                                                  IsClosed));
                    return;
                }

                try
                {
                    SocketError error = SocketError.SocketError;
                    switch (DataType4Client)
                    {
                        case DataType.BinaryType:
                            structure.StartWrite(BufferForSend);
                            objAsyncSendResult = Clientsocket.BeginSend(BufferForSend, OffsetForSend, structure.Length + 8, SocketFlags.None, out error, callback, this);
                            break;
                        case DataType.StringType:
                            structure.WriteString();
                            BufferForSend = Encoding.ASCII.GetBytes(structure.SbStringTosend.ToString());
                            objAsyncSendResult = Clientsocket.BeginSend(BufferForSend, OffsetForSend, BufferForSend.Length, SocketFlags.None, out error, callback, this);
                            break;
                        default:
                            break;
                    }
                    if (error != SocketError.Success)
                    {
                        FileHandling.WriteToLogEx(string.Format("[SocketClient] Can't begin cause of error '{0}'", error));
                        Debug.WriteLine(string.Format("[SocketClient] Can't begin cause of error '{0}'", error));
                    }
                }
                catch (Exception exception)
                {
                    FileHandling.WriteToLogEx("Exception:" + exception.Message + ":" + exception.StackTrace);
                    InvokeOnException(exception, false);
                }
            }
        }

        public int EndSend(IAsyncResult result)
        {
            lock (SendSync)
            {
                if (objAsyncSendResult == null)
                    return 2;
                if (!result.IsCompleted || !objAsyncSendResult.IsCompleted)
                    return 2;
                if (result != this.objAsyncSendResult)
                    return 2;
                Client client = (Client)result.AsyncState;
                try
                {
                    Socket socket = client.Clientsocket;
                    if (socket == null)
                    {
                        FileHandling.WriteToLogEx(string.Format("[SocketClient] Could not EndSend cause socket is Null"));
                        Debug.WriteLine("[SocketClient] Could not EndSend cause socket is Null");
                        return -1;
                    }
                    SocketError socketError;
                    if (socket == null)
                        return -1;
                    int res = socket.EndSend(result, out socketError);
                    if (socketError != SocketError.Success)
                    {
                        FileHandling.WriteToLogEx(string.Format("[SocketClient] Could not send. Error = '{0}'", socketError));
                        Debug.WriteLine(string.Format("[SocketClient] Could not send. Error = '{0}'", socketError));
                        return -1;
                    }
                    return res;
                }
                catch (ArgumentException ex)
                {
                    FileHandling.WriteToLogEx("Exception:" + ex.Message + ":" + ex.StackTrace);
                    Debug.WriteLine(ex.ToString());
                }
                catch (Exception exception)
                {
                    FileHandling.WriteToLogEx("Exception:" + exception.Message + ":" + exception.StackTrace);
                    client.InvokeOnException(exception, false);
                }
            }
            return -1;
        }

        public void SendDataTable(IProtocolStruct @struct, System.Action onCompleted)
        {
            BeginSend(@struct, result =>
            {
                EndSend(result);
                onCompleted();
            });
        }

        public void SendStruct(IProtocolStruct @struct, System.Action onCompleted)
        {
            BeginSend(@struct, result =>
            {
                EndSend(result);
                onCompleted();
            });
        }

        public void SendStruct(IProtocolStruct @struct, Action<IAsyncResult> onCompleted)
        {
            BeginSend(@struct, result =>
            {
                onCompleted(result);
            });
        }
    }
}
