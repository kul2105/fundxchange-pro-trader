
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Net;

namespace MQTTAPI
{
    public class MQTT
    {
        MqttClientOptionsBuilder optionsBuilder;// = new MqttClientOptionsBuilder().WithTcpServer("41.181.46.210", 5500).WithCredentials("brian", "%maher%");
        ManagedMqttClientOptions managedOptions;
        IManagedMqttClient mqttClient;

        public event Action<SymbolDirectory> OnSymbolDirectoryResponse = delegate { };
        public event Action<SymbolStatus> OnSymbolStatusResponse = delegate { };
        public event Action<MarketDataEntry> OnIndexResponse = delegate { };
        public event Action<FullSensNews> OnSensNews = delegate { };
        public event Action<Book> OnQuoteResponse = delegate { };
        public event Action<OrderBook> OnLevel2Response = delegate { };
        public event Action<Trade> OnTradeResponse = delegate { };
        public event Action<OHLCV> OnOHLVC = delegate { };

        List<MqttTopicFilter> topicfilters;

        public MQTT()
        {            
            topicfilters = new List<MqttTopicFilter>();            
            //var topicfilters = new List<MqttTopicFilter>
            //    {
            //        new MqttTopicFilterBuilder().WithTopic("jse/orderbook/prx").Build(),
            //        new MqttTopicFilterBuilder().WithTopic("jse/ohlcv/prx").Build(),
            //        new MqttTopicFilterBuilder().WithTopic("jse/index/j200").Build(),
            //        new MqttTopicFilterBuilder().WithTopic("jse/book/prx").Build(),
            //        new MqttTopicFilterBuilder().WithTopic("jse/sens/+").Build(),
            //        new MqttTopicFilterBuilder().WithTopic("jse/trade/prx").Build(),
            //        new MqttTopicFilterBuilder().WithTopic("jse/symboldirectory/+").Build(),
            //        new MqttTopicFilterBuilder().WithTopic("jse/symbolstatus/+").Build(),

            //    };
        }
        public void Connect(string username, string password)
        {
            optionsBuilder = new MqttClientOptionsBuilder().WithTcpServer("profile.diro.co.za", 5500).WithCredentials("brian", "%maher%");//new MqttClientOptionsBuilder().WithTcpServer("41.181.46.210", 5500).WithCredentials("brian", "%maher%");
            managedOptions = new ManagedMqttClientOptionsBuilder()
            .WithAutoReconnectDelay(TimeSpan.FromSeconds(2))
            .WithClientOptions(optionsBuilder)
            .Build();
            mqttClient = new MqttFactory().CreateManagedMqttClient();
            mqttClient.StartAsync(managedOptions).Wait();
            topicfilters = new List<MqttTopicFilter>
                {                    
                    new MqttTopicFilterBuilder().WithTopic("jse/sens/+").Build(),                   
                    new MqttTopicFilterBuilder().WithTopic("jse/symboldirectory/+").Build(),
                    new MqttTopicFilterBuilder().WithTopic("jse/symbolstatus/+").Build()
                };

            mqttClient.SubscribeAsync(topicfilters.ToArray()).Wait();
            mqttClient.UseDisconnectedHandler(e =>
            {
                //Console.WriteLine("Disconnected {0} {1}", e.AuthenticateResult, e.ClientWasConnected);
            });
            #region "Receiver Inline"

            //mqttClient.UseApplicationMessageReceivedHandler(e =>
            //{
            //    if (e.ApplicationMessage.Topic.Contains("/sens/"))
            //    {
            //        var decompressed = Decompress(e.ApplicationMessage.Payload);
            //        var json = Encoding.GetEncoding(1252).GetString(decompressed);
            //        var sens = Newtonsoft.Json.JsonConvert.DeserializeObject<FullSensNews>(json);
            //        if (OnSensNews != null)
            //            OnSensNews(sens);
            //    }
            //    if (e.ApplicationMessage.Topic.Contains("/index/"))
            //    {
            //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
            //        var entry = Newtonsoft.Json.JsonConvert.DeserializeObject<MarketDataEntry>(json);
            //        if (OnIndexResponse != null)
            //            OnIndexResponse(entry);
            //    }
            //    if (e.ApplicationMessage.Topic.Contains("/symboldirectory/"))
            //    {
            //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
            //        var symboldir = Newtonsoft.Json.JsonConvert.DeserializeObject<SymbolDirectory>(json);
            //        if (OnSymbolDirectoryResponse != null)
            //        {
            //            OnSymbolDirectoryResponse(symboldir);
            //        }
            //    }
            //    if (e.ApplicationMessage.Topic.Contains("/symbolstatus/"))
            //    {
            //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
            //        var symbolstatus = Newtonsoft.Json.JsonConvert.DeserializeObject<SymbolStatus>(json);
            //        if (OnSymbolStatusResponse != null)
            //            OnSymbolStatusResponse(symbolstatus);
            //    }
            //    if (e.ApplicationMessage.Topic.Contains("/auctioninfo/"))
            //    {
            //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
            //        var auctioninfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AuctionInfo>(json);
            //        //Console.WriteLine("AuctionInfo received for {0}", auctioninfo.Symbol);
            //    }
            //});
            #endregion
            StartReceiver();
        }

        private void StartReceiver()
        {
            mqttClient.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected {0} {1} {2}", e.AuthenticateResult, e.ClientWasConnected, e.Exception);
                if (e.ClientWasConnected)
                {
                    //mqttClient.ConnectAsync(optionsBuilder.Build(), CancellationToken.None);
                }
            });
            mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                if (e.ApplicationMessage.Topic.Contains("/trade/"))
                {
                    var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    var trade = Newtonsoft.Json.JsonConvert.DeserializeObject<Trade>(json);
                    if (OnTradeResponse != null)
                        OnTradeResponse(trade);
                }
                if (e.ApplicationMessage.Topic.Contains("/book/"))
                {
                    var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    var book = Newtonsoft.Json.JsonConvert.DeserializeObject<Book>(json);
                    if (OnQuoteResponse != null)
                        OnQuoteResponse(book);
                }
                if (e.ApplicationMessage.Topic.Contains("/ohlcv/"))
                {
                    var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    var ohlcv = Newtonsoft.Json.JsonConvert.DeserializeObject<OHLCV>(json);
                    if (OnOHLVC != null)
                        OnOHLVC(ohlcv);
                }
                if (e.ApplicationMessage.Topic.Contains("/orderbook/"))
                {
                    var decompressed = Decompress(e.ApplicationMessage.Payload);
                    var json = ASCIIEncoding.ASCII.GetString(decompressed);
                    var orderbook = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderBook>(json);
                    if (OnLevel2Response != null)
                        OnLevel2Response(orderbook);
                }
                if (e.ApplicationMessage.Topic.Contains("/sens/"))
                {
                    var decompressed = Decompress(e.ApplicationMessage.Payload);
                    var json = Encoding.GetEncoding(1252).GetString(decompressed);
                    var sens = Newtonsoft.Json.JsonConvert.DeserializeObject<FullSensNews>(json);
                    if (OnSensNews != null)
                        OnSensNews(sens);
                    //Console.WriteLine("Sens received for {0} {1}", sens.NewsID, sens.NewsItems[0].EncodedHeadline);
                }
                if (e.ApplicationMessage.Topic.Contains("/index/"))
                {
                    var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    var entry = Newtonsoft.Json.JsonConvert.DeserializeObject<MarketDataEntry>(json);
                    if (OnIndexResponse != null)
                        OnIndexResponse(entry);
                }
                if (e.ApplicationMessage.Topic.Contains("/symboldirectory/"))
                {
                    var decompressed = Decompress(e.ApplicationMessage.Payload);
                    var json = ASCIIEncoding.ASCII.GetString(decompressed);
                    var symboldir = Newtonsoft.Json.JsonConvert.DeserializeObject<SymbolDirectory>(json);
                    if (OnSymbolDirectoryResponse != null)
                    {
                        OnSymbolDirectoryResponse(symboldir);
                        System.Diagnostics.Debug.WriteLine(symboldir.Symbol);
                    }
                }
                if (e.ApplicationMessage.Topic.Contains("/symbolstatus/"))
                {
                    var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    var symbolstatus = Newtonsoft.Json.JsonConvert.DeserializeObject<SymbolStatus>(json);
                    if (OnSymbolStatusResponse != null)
                        OnSymbolStatusResponse(symbolstatus);
                    //Console.WriteLine("SymbolStatus received for {0}", symbolstatus.Symbol);
                }
                if (e.ApplicationMessage.Topic.Contains("/auctioninfo/"))
                {
                    var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    var auctioninfo = Newtonsoft.Json.JsonConvert.DeserializeObject<AuctionInfo>(json);
                    //Console.WriteLine("AuctionInfo received for {0}", auctioninfo.Symbol);
                }
            });
        }

        public void SubscribeTopic(string topic)
        {
            try
            {
                if (!topicfilters.Exists(i => i.Topic == topic))
                {
                    topicfilters.Add(new MqttTopicFilterBuilder().WithTopic(topic).Build());
                }

                if (mqttClient != null)
                {                    
                    //mqttClient.StartAsync(managedOptions).Wait();
                    //TopicFilter topicfilter = new TopicFilterBuilder().WithTopic(topic).Build();
                    //TopicFilter topicfilter = new TopicFilterBuilder().WithTopic("jse/book/prx").Build();
                    mqttClient.SubscribeAsync(topicfilters.ToArray()).Wait();
                    #region "Receiver Inline"
                    //mqttClient.UseDisconnectedHandler(e =>
                    //{
                    //    Console.WriteLine("Disconnected {0} {1} {2}", e.AuthenticateResult, e.ClientWasConnected, e.Exception);
                    //    if (e.ClientWasConnected)
                    //    {
                    //        mqttClient.ConnectAsync(optionsBuilder.Build(), CancellationToken.None);
                    //    }
                    //});
                    //mqttClient.UseApplicationMessageReceivedHandler(e =>
                    //{
                    //    if (e.ApplicationMessage.Topic.Contains("/trade/"))
                    //    {
                    //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    //        var trade = Newtonsoft.Json.JsonConvert.DeserializeObject<Trade>(json);
                    //        if (OnTradeResponse != null)
                    //            OnTradeResponse(trade);
                    //        //Console.WriteLine("Trade received for {0}", trade.Symbol);
                    //    }
                    //    if (e.ApplicationMessage.Topic.Contains("/book/"))//quotes
                    //    {
                    //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    //        var book = Newtonsoft.Json.JsonConvert.DeserializeObject<Book>(json);
                    //        if (OnQuoteResponse != null)
                    //            OnQuoteResponse(book);
                    //        //Console.WriteLine("Book received for {0}", book.Symbol);
                    //    }
                    //    if (e.ApplicationMessage.Topic.Contains("/ohlcv/"))
                    //    {
                    //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    //        var ohlcv = Newtonsoft.Json.JsonConvert.DeserializeObject<OHLCV>(json);
                    //        if (OnOHLVC != null)
                    //            OnOHLVC(ohlcv);
                    //        //Console.WriteLine("OHLCV received for {0}", ohlcv.Symbol);
                    //    }
                    //    if (e.ApplicationMessage.Topic.Contains("/orderbook/"))
                    //    {
                    //        var decompressed = Decompress(e.ApplicationMessage.Payload);
                    //        var json = ASCIIEncoding.ASCII.GetString(decompressed);
                    //        var orderbook = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderBook>(json);
                    //        if (OnLevel2Response != null)
                    //            OnLevel2Response(orderbook);
                    //        //Console.WriteLine("Order Book received for {0}", orderbook.Symbol);
                    //    }
                    //    if (e.ApplicationMessage.Topic.Contains("/sens/"))
                    //    {
                    //        var decompressed = Decompress(e.ApplicationMessage.Payload);
                    //        var json = Encoding.GetEncoding(1252).GetString(decompressed);
                    //        var sens = Newtonsoft.Json.JsonConvert.DeserializeObject<FullSensNews>(json);
                    //        if (OnSymbolDirectoryResponse != null)
                    //        {
                    //            OnSymbolDirectoryResponse(sens.AllSymbols);
                    //        }

                    //        Console.WriteLine("Sens received for {0} {1}", sens.NewsID, sens.NewsItems[0].EncodedHeadline);
                    //    }
                    //    if (e.ApplicationMessage.Topic.Contains("/index/"))
                    //    {
                    //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
                    //        var entry = Newtonsoft.Json.JsonConvert.DeserializeObject<MarketDataEntry>(json);
                    //        Console.WriteLine("Index received for {0}", entry.Symbol);
                    //    }
                    //});
                    #endregion
                    StartReceiver();
                }
            }
            catch (Exception ex)
            {

                //throw;
            }
            //if (lstTopics.Count == 0) return;

        }
        public void UnSubscribeTopic(string topic)
        {
            //if (lstTopics.Count == 0) return;
            MqttTopicFilter filter = topicfilters.FirstOrDefault(i => i.Topic.Equals(topic));
            if (filter != null)
            {
                topicfilters.Remove(filter);
                //topicfilters.Add(new TopicFilterBuilder().WithTopic(topic).Build());
                //SubscribeTopic(topicfilters);
            }
            if (mqttClient != null)
            {
                var topicfilters = new List<MqttTopicFilter>
                    {
                        new MqttTopicFilterBuilder().WithTopic(topic).Build()                       

                    };

                mqttClient.UnsubscribeAsync(topic);
            }
        }
        public void HandleLevel1Request(string symbol)
        {
            string topicQuote = "jse/book/" + symbol.ToLower();
            string topicohlc = "jse/ohlcv/" + symbol.ToLower();
            string topicTrade = "jse/trade/" + symbol.ToLower();
            SubscribeTopic(topicQuote);
            SubscribeTopic(topicohlc);
            SubscribeTopic(topicTrade);
        }
        public void HandleLevel2Request(string symbol)
        {
            string topic = "jse/orderbook/" + symbol.ToLower();
            SubscribeTopic(topic);
        }

        public void UnsusbcribeLevel1(string symbol)
        {
            string topic = "jse/book/" + symbol.ToLower();
            UnSubscribeTopic(topic);
        }
        public void UnsusbcribeLevel2(string symbol)
        {
            string topic = "jse/orderbook/" + symbol.ToLower();
            UnSubscribeTopic(topic);
        }

        public string RequestOHLC(string symbol, string periodicity, int candleInterval, int numberOfCandles)
        {
            //http://129.232.181.61:5003/HistoricalDataService.svc/GetOHLC/J055/D/1/365

            try
            {
                string footer = symbol + "/" + periodicity + "/" + candleInterval + "/" + numberOfCandles;
                var url = @"http://41.76.211.216:75/HistoricalDataService.svc/GetOHLC" + "/" + footer;

                var syncClient = new WebClient();
                var content = syncClient.DownloadString(url);//DownloadData(url);
                return content.ToString();
                //if (content == string.Empty)
                //    return new List<ExpenseSummary>();
                //JavaScriptSerializer serializer = new JavaScriptSerializer() { MaxJsonLength = Int32.MaxValue };
                //OHLCResponse _socketResponce = serializer.Deserialize<OHLCResponse>((content));
                //return _socketResponce.lstOHLCData;


            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        static byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
        static byte[] Compress(byte[] raw)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                {
                    gzip.Write(raw, 0, raw.Length);
                }
                return memory.ToArray();
            }
        }

    }
}
