using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Net.Sockets;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Collections;
using System.Timers;
using WebSocket4Net;
using System.Net;

namespace JSEServersAPI
{
    class WebSocketFeed

    {
        const int FEED_LOGIN_REQUEST = 50;
        const int FEED_LOGIN_RESPONSE = 51;
        const int FEED_SUBCRIBE_REQUEST = 52;
        const int FEED_RESPONSE = 53;
        const int BUSINESS_LEVEL_REJECT = 54;
        const int ACCOUNTS_DETAILS_REQUEST = 55;
        const int ACCOUNTS_DETAILS_RESPONSE = 56;
        const int SYMBOL_DETAILS_REQUEST = 57;
        const int SYMBOL_DETAILS_RESPONSE = 58;
        const int SERVER_DETAILS_REQUEST = 59;
        const int SERVER_DETAILS_RESPONSE = 60;
        const int LOGOUT_REQUEST = 61;
        const int LOGOUT_RESPONSE = 62;
        private WebSocket websocket;

       public event Action<FeedResponse> OnDataFeeds = delegate { };
        public WebSocketFeed(string Url)
        {
            try
            {

                websocket = new WebSocket(@"ws://197.242.148.230:9064/test");
                websocket.Opened -= websocket_Opened;
                websocket.Error -= websocket_Error;
                // websocket.Closed -= websocket_Closed;
                websocket.MessageReceived -= websocket_MessageReceived;
                websocket.EnableAutoSendPing = false;
                websocket.Opened += websocket_Opened;
                websocket.Error += websocket_Error;
                // websocket.Closed += websocket_Closed;
                websocket.MessageReceived += websocket_MessageReceived;
                websocket.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //Console.ReadKey();
        }
        public void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                //Console.WriteLine(e.Message);
                //var syncClient = new WebClient();
                //var content = syncClient.DownloadData(url);
                byte[] bytes = Encoding.ASCII.GetBytes(e.Message);
                JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                FeedResponse _socketResponce = serializer.Deserialize<FeedResponse>(System.Text.Encoding.UTF8.GetString(bytes));
                //return _socketResponce.lstData;
                if (OnDataFeeds != null)
                    OnDataFeeds(_socketResponce);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        public void websocket_Opened(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("Connection Opened");
                loginRequest objLogin = new loginRequest();
                objLogin.login = 7886;
                objLogin.msgtype = 50;
                objLogin.password = "password";
                objLogin.Server = "TestServer";

                var json = new JavaScriptSerializer().Serialize(objLogin);
                websocket.Send(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            try
            {
                Console.WriteLine("Error: " + e.Exception.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        //public void TestClient(object state)
        //{
        //    try
        //    {
        //        _logger = new WebSocketLogger();
        //        //Thread clientThread = new Thread(new ParameterizedThreadStart(TestClient));
        //        //clientThread.IsBackground = false;

        //        // to enable ssl change the port to 443 in the settings file and use the wss schema below
        //        // clientThread.Start("wss://localhost/chat");

        //        //clientThread.Start("ws://197.242.148.230:9064/test");
        //        //clientThread.Start("ws://127.0.0.1:9063/test");
        //        //WebSocketClient.Form1 frm = new WebSocketClient.Form1();
        //        //frm.Show();
        //        //TestClient("ws://197.242.148.230:9064/test");
        //        //Console.ReadKey();

        //        //string url = (string)state;
        //        string url = "ws://197.242.148.230:9064/test";
        //        using (var client = new TestWebSocketClient(true, _logger))
        //        {
        //            Uri uri = new Uri(url);
        //            client.TextFrame += Client_TextFrame;
        //            client.ConnectionOpened += Client_ConnectionOpened;

        //            // test the open handshake
        //            client.OpenBlocking(uri);
        //        }

        //        Console.WriteLine("Client finished, press any key");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //        Console.WriteLine("Client terminated: Press any key");
        //    }

        //    Console.ReadKey();
        //}
        //private void Client_ConnectionOpened(object sender, EventArgs e)
        //{
        //    Console.WriteLine("Client: Connection Opened");
        //    var client = (TestWebSocketClient)sender;

        //    // test sending a message to the server
        //    loginRequest objLogin = new loginRequest();
        //    objLogin.login = 7886;
        //    objLogin.msgtype = FEED_LOGIN_REQUEST;
        //    objLogin.password = "password";
        //    objLogin.Server = "TestServer";

        //    var json = new JavaScriptSerializer().Serialize(objLogin);

        //    client.Send(json);
        //}

        //private void Client_TextFrame(object sender, TextFrameEventArgs e)
        //{
        //    Console.WriteLine("Client: {0}", e.Text);
        //    var client = (TestWebSocketClient)sender;

        //    //// lets test the close handshake
        //    //client.Dispose();
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    loginResponse collection = serializer.Deserialize<loginResponse>(e.Text);
        //    if (collection.msgtype == FEED_LOGIN_RESPONSE && collection.message.ToUpper().Trim() == "SUCCESS")
        //    {
        //        FeedRequest objReq = new FeedRequest();
        //        objReq.msgtype = FEED_SUBCRIBE_REQUEST;
        //        objReq.request = 1;
        //        var json = new JavaScriptSerializer().Serialize(objReq);

        //        client.Send(json);
        //    }
        //}

    }

    //internal class WebSocketLogger : IWebSocketLogger
    //{
    //    public void Information(Type type, string format, params object[] args)
    //    {
    //        Trace.TraceInformation(format, args);
    //    }

    //    public void Warning(Type type, string format, params object[] args)
    //    {
    //        Trace.TraceWarning(format, args);
    //    }

    //    public void Error(Type type, string format, params object[] args)
    //    {
    //        Trace.TraceError(format, args);
    //    }

    //    public void Error(Type type, Exception exception)
    //    {
    //        Error(type, "{0}", exception);
    //    }
    //}

    public struct loginRequest
    {
        public int login;
        public string password;
        public string Server;
        public int msgtype;
    };

    public struct loginResponse
    {
        public int login;
        public string message;
        public string Server;
        public int msgtype;

    };

    public struct logoutRequest
    {
        public string Server;
        public int login;
        public int msgtype;
    };

    public struct logoutResponse
    {
        public string Server;
        public int login;
        public string message;
        public int msgtype;
    };

    public struct FeedRequest
    {
        public int request; // send 1
        public int msgtype;
    };

    public struct FeedResponse
    {
        public List<DataFeed> DataFeed;
        public int NoOfRecords;
        public int msgtype;
    };
    public struct DataFeed
    {
        public string symbol;            // symbol name
        public double bid;
        public double ask;
        public double high;
        public double low;
        public long lasttime;
        public int level;

    };
}
