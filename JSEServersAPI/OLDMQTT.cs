//using Dst.Jse.Equity.ProfileMQTT;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace JSEServersAPI
{
    //public class OLDMQTT
    //{
    //    MqttClientOptionsBuilder optionsBuilder;// = new MqttClientOptionsBuilder().WithTcpServer("41.181.46.210", 5500).WithCredentials("brian", "%maher%");
    //    IMqttClient mqttClient;

    //    public event Action<List<string>> OnSymbolDirectoryResponse = delegate { };
    //    public event Action<Book> OnQuoteResponse = delegate { };
    //    public event Action<OrderBook> OnLevel2Response = delegate { };
    //    public event Action<Trade> OnTradeResponse = delegate { };
    //    public event Action<OHLCV> OnOHLVC = delegate { };

    //    List<TopicFilter> lstTopics;
    //    List<TopicFilter> topicfilters;

    //    public MQTT()
    //    {
    //        //mqttClient= new MqttFactory().CreateMqttClient();
    //        topicfilters = new List<TopicFilter>();
    //        //StartReceiver();
    //        //mqttClient.UseConnectedHandler(async e =>
    //        //{
    //        //    //Console.WriteLine("### CONNECTED WITH SERVER ###");
    //        //    var topicfilters = new List<TopicFilter>
    //        //    {
    //        //        new TopicFilterBuilder().WithTopic("jse/index/j200").Build(),
    //        //        new TopicFilterBuilder().WithTopic("jse/orderbook/prx").Build(),
    //        //        new TopicFilterBuilder().WithTopic("jse/book/prx").Build(),
    //        //        new TopicFilterBuilder().WithTopic("jse/ohlcv/prx").Build(),
    //        //        new TopicFilterBuilder().WithTopic("jse/sens/+").Build(),
    //        //        new TopicFilterBuilder().WithTopic("jse/trade/prx").Build()
    //        //    };
    //        //    await mqttClient.SubscribeAsync(topicfilters.ToArray());
    //        //    //Console.WriteLine("### SUBSCRIBED ###");
    //        //});
    //    }

    //    private void StartReceiver()
    //    {
    //        mqttClient.UseDisconnectedHandler(e =>
    //        {
    //            Console.WriteLine("Disconnected {0} {1} {2}", e.AuthenticateResult, e.ClientWasConnected, e.Exception);
    //            if (e.ClientWasConnected)
    //            {
    //                mqttClient.ConnectAsync(optionsBuilder.Build(), CancellationToken.None);
    //            }
    //        });
    //        mqttClient.UseApplicationMessageReceivedHandler(e =>
    //        {
    //            if (e.ApplicationMessage.Topic.Contains("/trade/"))
    //            {
    //                var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                var trade = Newtonsoft.Json.JsonConvert.DeserializeObject<Trade>(json);
    //                if (OnTradeResponse != null)
    //                    OnTradeResponse(trade);
    //                //Console.WriteLine("Trade received for {0}", trade.Symbol);
    //            }
    //            if (e.ApplicationMessage.Topic.Contains("/book/"))//quotes
    //            {
    //                var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                var book = Newtonsoft.Json.JsonConvert.DeserializeObject<Book>(json);
    //                if (OnQuoteResponse != null)
    //                    OnQuoteResponse(book);
    //                //Console.WriteLine("Book received for {0}", book.Symbol);
    //            }
    //            if (e.ApplicationMessage.Topic.Contains("/ohlcv/"))
    //            {
    //                var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                var ohlcv = Newtonsoft.Json.JsonConvert.DeserializeObject<OHLCV>(json);
    //                if (OnOHLVC != null)
    //                    OnOHLVC(ohlcv);
    //                //Console.WriteLine("OHLCV received for {0}", ohlcv.Symbol);
    //            }
    //            if (e.ApplicationMessage.Topic.Contains("/orderbook/"))
    //            {
    //                var decompressed = Decompress(e.ApplicationMessage.Payload);
    //                var json = ASCIIEncoding.ASCII.GetString(decompressed);
    //                var orderbook = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderBook>(json);
    //                if (OnLevel2Response != null)
    //                    OnLevel2Response(orderbook);
    //                //Console.WriteLine("Order Book received for {0}", orderbook.Symbol);
    //            }
    //            if (e.ApplicationMessage.Topic.Contains("/sens/"))
    //            {
    //                var decompressed = Decompress(e.ApplicationMessage.Payload);
    //                var json = Encoding.GetEncoding(1252).GetString(decompressed);
    //                var sens = Newtonsoft.Json.JsonConvert.DeserializeObject<FullSensNews>(json);
    //                if (OnSymbolDirectoryResponse != null)
    //                {
    //                    OnSymbolDirectoryResponse(sens.AllSymbols);
    //                }

    //                Console.WriteLine("Sens received for {0} {1}", sens.NewsID, sens.NewsItems[0].EncodedHeadline);
    //            }
    //            if (e.ApplicationMessage.Topic.Contains("/index/"))
    //            {
    //                var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                var entry = Newtonsoft.Json.JsonConvert.DeserializeObject<MarketDataEntry>(json);
    //                Console.WriteLine("Index received for {0}", entry.Symbol);
    //            }
    //        });
    //    }

    //    public void SubscribeTopic(string topic)
    //    {
    //        try
    //        {
    //            if (!topicfilters.Exists(i => i.Topic == topic))
    //            {
    //                topicfilters.Add(new TopicFilterBuilder().WithTopic(topic).Build());
    //            }

    //            if (mqttClient != null)
    //            {
    //                mqttClient.ConnectAsync(optionsBuilder.Build(), CancellationToken.None);
    //                //TopicFilter topicfilter = new TopicFilterBuilder().WithTopic(topic).Build();
    //                //TopicFilter topicfilter = new TopicFilterBuilder().WithTopic("jse/book/prx").Build();

    //                mqttClient.UseConnectedHandler(async e =>
    //                {
    //                    await mqttClient.SubscribeAsync(topicfilters.ToArray());
    //                });
    //                //mqttClient.UseDisconnectedHandler(e =>
    //                //{
    //                //    Console.WriteLine("Disconnected {0} {1} {2}", e.AuthenticateResult, e.ClientWasConnected, e.Exception);
    //                //    if (e.ClientWasConnected)
    //                //    {
    //                //        mqttClient.ConnectAsync(optionsBuilder.Build(), CancellationToken.None);
    //                //    }
    //                //});
    //                //mqttClient.UseApplicationMessageReceivedHandler(e =>
    //                //{
    //                //    if (e.ApplicationMessage.Topic.Contains("/trade/"))
    //                //    {
    //                //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                //        var trade = Newtonsoft.Json.JsonConvert.DeserializeObject<Trade>(json);
    //                //        if (OnTradeResponse != null)
    //                //            OnTradeResponse(trade);
    //                //        //Console.WriteLine("Trade received for {0}", trade.Symbol);
    //                //    }
    //                //    if (e.ApplicationMessage.Topic.Contains("/book/"))//quotes
    //                //    {
    //                //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                //        var book = Newtonsoft.Json.JsonConvert.DeserializeObject<Book>(json);
    //                //        if (OnQuoteResponse != null)
    //                //            OnQuoteResponse(book);
    //                //        //Console.WriteLine("Book received for {0}", book.Symbol);
    //                //    }
    //                //    if (e.ApplicationMessage.Topic.Contains("/ohlcv/"))
    //                //    {
    //                //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                //        var ohlcv = Newtonsoft.Json.JsonConvert.DeserializeObject<OHLCV>(json);
    //                //        if (OnOHLVC != null)
    //                //            OnOHLVC(ohlcv);
    //                //        //Console.WriteLine("OHLCV received for {0}", ohlcv.Symbol);
    //                //    }
    //                //    if (e.ApplicationMessage.Topic.Contains("/orderbook/"))
    //                //    {
    //                //        var decompressed = Decompress(e.ApplicationMessage.Payload);
    //                //        var json = ASCIIEncoding.ASCII.GetString(decompressed);
    //                //        var orderbook = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderBook>(json);
    //                //        if (OnLevel2Response != null)
    //                //            OnLevel2Response(orderbook);
    //                //        //Console.WriteLine("Order Book received for {0}", orderbook.Symbol);
    //                //    }
    //                //    if (e.ApplicationMessage.Topic.Contains("/sens/"))
    //                //    {
    //                //        var decompressed = Decompress(e.ApplicationMessage.Payload);
    //                //        var json = Encoding.GetEncoding(1252).GetString(decompressed);
    //                //        var sens = Newtonsoft.Json.JsonConvert.DeserializeObject<FullSensNews>(json);
    //                //        if (OnSymbolDirectoryResponse != null)
    //                //        {
    //                //            OnSymbolDirectoryResponse(sens.AllSymbols);
    //                //        }

    //                //        Console.WriteLine("Sens received for {0} {1}", sens.NewsID, sens.NewsItems[0].EncodedHeadline);
    //                //    }
    //                //    if (e.ApplicationMessage.Topic.Contains("/index/"))
    //                //    {
    //                //        var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                //        var entry = Newtonsoft.Json.JsonConvert.DeserializeObject<MarketDataEntry>(json);
    //                //        Console.WriteLine("Index received for {0}", entry.Symbol);
    //                //    }
    //                //});


    //                StartReceiver();
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //            //throw;
    //        }
    //        //if (lstTopics.Count == 0) return;

    //    }
    //    public void UnSubscribeTopic(string topic)
    //    {
    //        //if (lstTopics.Count == 0) return;
    //        TopicFilter filter = topicfilters.FirstOrDefault(i => i.Topic.Equals(topic));
    //        if (filter != null)
    //        {
    //            topicfilters.Remove(filter);
    //            //topicfilters.Add(new TopicFilterBuilder().WithTopic(topic).Build());
    //            //SubscribeTopic(topicfilters);
    //        }
    //        if (mqttClient != null)
    //        {
    //            TopicFilter topicfilter = new TopicFilterBuilder().WithTopic(topic).Build();
    //            mqttClient.UseConnectedHandler(async e =>
    //            {
    //                await mqttClient.SubscribeAsync(topicfilters.ToArray());
    //            });
    //        }
    //    }
    //    public void HandleLevel1Request(string symbol)
    //    {
    //        string topicQuote = "jse/book/" + symbol.ToLower();
    //        string topicohlc = "jse/ohlcv/" + symbol.ToLower();
    //        string topicTrade = "jse/trade/" + symbol.ToLower();
    //        SubscribeTopic(topicQuote);
    //        SubscribeTopic(topicohlc);
    //        SubscribeTopic(topicTrade);
    //    }
    //    public void HandleLevel2Request(string symbol)
    //    {
    //        string topic = "jse/orderbook/" + symbol.ToLower();
    //        SubscribeTopic(topic);
    //    }

    //    public void UnsusbcribeLevel1(string symbol)
    //    {
    //        string topic = "jse/book/" + symbol.ToLower();
    //        UnSubscribeTopic(topic);
    //    }
    //    public void UnsusbcribeLevel2(string symbol)
    //    {
    //        string topic = "jse/orderbook/" + symbol.ToLower();
    //        UnSubscribeTopic(topic);
    //    }
    //    public void Connect(string username, string password)
    //    {
    //        optionsBuilder = new MqttClientOptionsBuilder().WithTcpServer("41.181.46.210", 5500).WithCredentials("brian", "%maher%");
    //        mqttClient = new MqttFactory().CreateMqttClient();
    //        mqttClient.ConnectAsync(optionsBuilder.Build(), CancellationToken.None);
    //        mqttClient.UseConnectedHandler(async e =>
    //        {
    //            TopicFilter topicfilter = new TopicFilterBuilder().WithTopic("jse/sens/+").Build();
    //            await mqttClient.SubscribeAsync(topicfilter);
    //        });
    //        mqttClient.UseApplicationMessageReceivedHandler(e =>
    //        {
    //            if (e.ApplicationMessage.Topic.Contains("/sens/"))
    //            {
    //                var decompressed = Decompress(e.ApplicationMessage.Payload);
    //                var json = Encoding.GetEncoding(1252).GetString(decompressed);
    //                var sens = Newtonsoft.Json.JsonConvert.DeserializeObject<FullSensNews>(json);
    //                if (OnSymbolDirectoryResponse != null)
    //                {
    //                    OnSymbolDirectoryResponse(sens.AllSymbols);
    //                }

    //                Console.WriteLine("Sens received for {0} {1}", sens.NewsID, sens.NewsItems[0].EncodedHeadline);
    //            }
    //            if (e.ApplicationMessage.Topic.Contains("/index/"))
    //            {
    //                var json = ASCIIEncoding.ASCII.GetString(e.ApplicationMessage.Payload);
    //                var entry = Newtonsoft.Json.JsonConvert.DeserializeObject<MarketDataEntry>(json);
    //                Console.WriteLine("Index received for {0}", entry.Symbol);
    //            }
    //        });

    //        //StartReceiver();
    //    }

    //    static byte[] Decompress(byte[] gzip)
    //    {
    //        // Create a GZIP stream with decompression mode.
    //        // ... Then create a buffer and write into while reading from the GZIP stream.
    //        using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
    //            CompressionMode.Decompress))
    //        {
    //            const int size = 4096;
    //            byte[] buffer = new byte[size];
    //            using (MemoryStream memory = new MemoryStream())
    //            {
    //                int count = 0;
    //                do
    //                {
    //                    count = stream.Read(buffer, 0, size);
    //                    if (count > 0)
    //                    {
    //                        memory.Write(buffer, 0, count);
    //                    }
    //                }
    //                while (count > 0);
    //                return memory.ToArray();
    //            }
    //        }
    //    }
    //    static byte[] Compress(byte[] raw)
    //    {
    //        using (MemoryStream memory = new MemoryStream())
    //        {
    //            using (GZipStream gzip = new GZipStream(memory,
    //                CompressionMode.Compress, true))
    //            {
    //                gzip.Write(raw, 0, raw.Length);
    //            }
    //            return memory.ToArray();
    //        }
    //    }

    //}
}
