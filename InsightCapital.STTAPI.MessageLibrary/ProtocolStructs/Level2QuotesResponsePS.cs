using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;
using InsightCapital.STTAPI.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    public class Level2QuotesResponsePS : IProtocolStruct
    {
        public Level2Quotes level2Quotes = new Level2Quotes();

        public Level2QuotesResponsePS()
        {

        }

        public Level2QuotesResponsePS(Level2Quotes data)
        {
            level2Quotes = new Level2Quotes(data);
        }

        public override int ID
        {
            get { return ProtocolStructIDS.Level2QuotesResponsePS_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(level2Quotes.InstrumentID);
            bw_.Write(level2Quotes.Buydepth);
            bw_.Write(level2Quotes.Selldepth);

            for (int i = 0; i < level2Quotes.BuySide.Count; i++)
            {
                bw_.Write(level2Quotes.BuySide[i].OrderId);
                bw_.Write(level2Quotes.BuySide[i].Price);
                bw_.Write(level2Quotes.BuySide[i].Volume);
                bw_.Write(level2Quotes.BuySide[i].TimeStamp.ToBinary());
            }

            for (int i = 0; i < level2Quotes.SellSide.Count; i++)
            {
                bw_.Write(level2Quotes.SellSide[i].OrderId);
                bw_.Write(level2Quotes.SellSide[i].Price);
                bw_.Write(level2Quotes.SellSide[i].Volume);
                bw_.Write(level2Quotes.SellSide[i].TimeStamp.ToBinary());
            }
            bw_.Write(level2Quotes.orderTransType.ToString());
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            InitRead(buffer);
            level2Quotes.InstrumentID = br_.ReadUInt32();
            level2Quotes.Buydepth = br_.ReadInt32();
            level2Quotes.Selldepth = br_.ReadInt32();

            for (int i = 0; i < level2Quotes.Buydepth; i++)
            {
                Order order = new Order();
                order.OrderId = br_.ReadString();
                order.Price = br_.ReadDouble();
                order.Volume = br_.ReadUInt32();
                order.TimeStamp = DateTime.FromBinary(br_.ReadInt64());
                level2Quotes.BuySide.Add(order);
            }

            for (int i = 0; i < level2Quotes.Selldepth; i++)
            {
                Order order = new Order();
                order.OrderId = br_.ReadString();
                order.Price = br_.ReadDouble();
                order.Volume = br_.ReadUInt32();
                order.TimeStamp = DateTime.FromBinary(br_.ReadInt64());
                level2Quotes.SellSide.Add(order);
            }
            level2Quotes.orderTransType = (OrderTransType)Enum.Parse(typeof(OrderTransType), br_.ReadString(), true);
            CloseRead();
        }

        public override string ToString()
        {
            if (level2Quotes == null)
                return string.Empty;
            else
            {
                return level2Quotes.ToString();
            }
        }
        public override void ReadString(string Msgbfr)
        {
            StartStrR(Msgbfr);
        }

        public override void WriteString()
        {
            StartStrW();
        }
    }

    [Serializable]
    public class Level2Quotes
    {
        public uint InstrumentID { get; set; }
        public List<Order> BuySide;
        public List<Order> SellSide;
        public int Buydepth;
        public int Selldepth;
        public OrderTransType orderTransType;

        public Level2Quotes()
        {
            BuySide = new List<Order>();
            SellSide = new List<Order>();
            Buydepth = 0;
            Selldepth = 0;
            InstrumentID = 0;
            orderTransType = OrderTransType.NA;
        }

        public Level2Quotes(Level2Quotes quote)
        {
            InstrumentID = quote.InstrumentID;
            BuySide = quote.BuySide;
            SellSide = quote.SellSide;
            Buydepth = quote.Buydepth;
            Selldepth = quote.Selldepth;
            orderTransType = quote.orderTransType;
        }

        public override string ToString()
        {
            string buySide = string.Empty;
            foreach (Order item in BuySide)
            {
                int i = 1;
                buySide += i++ + ":" + item.ToString();
            }

            string sellSide = string.Empty;
            foreach (Order item in SellSide)
            {
                int i = 1;
                sellSide += i++ + ":" + item.ToString();
            }

            return "MessageType = Level2Quote Response :" + " InstrumentID = " + InstrumentID.ToString()
                                                          + " OrderTransType = " + orderTransType.ToString()
                                                          + " Buydepth = " + Buydepth.ToString()
                                                          + " Selldepth = " + Selldepth.ToString()
                                                          + " BuySide = " + buySide.ToString()
                                                          + " SellSide = " + sellSide.ToString();
        }
    }

    //[Serializable]
    //public class MarketDepthOrder
    //{
    //    /// <summary>
    //    /// Order Id
    //    /// </summary>
    //    public UInt64 OrderId { get; set; }

    //    /// <summary>Order price</summary>
    //    public decimal Price { get; set; }

    //    /// <summary>Order volume</summary>
    //    public ulong Volume { get; set; }

    //    public MarketDepthOrder()
    //    {

    //    }

    //    public MarketDepthOrder(UInt64 orderid, decimal price, ulong volume)
    //    {
    //        OrderId = orderid;
    //        Price = price;
    //        Volume = volume;
    //    }

    //    /// <summary>
    //    /// Returns tab-separated string representation of the limit order (price, time, volume, recorded)
    //    /// </summary>        
    //    public override string ToString()
    //    {
    //        StringBuilder output = new StringBuilder();
    //        output.Append("Order Id :- " + OrderId.ToString());
    //        output.Append("Price:- " + Price.ToString());
    //        output.Append("Volume:- " + Volume);
    //        return output.ToString();
    //    }
    //}

    [Serializable]
    public class Order
    {
        /// <summary>Order submission side</summary>
        public string OrderId { get; set; }

        /// <summary>Order submission side</summary>
        public Side side { get; set; }

        /// <summary>Order price</summary>
        public double Price { get; set; }

        /// <summary>Order submission time</summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>Order volume</summary>
        public uint Volume { get; set; }

        public Order()
        {

        }

        /// <summary>
        /// LimitOrder constructor
        /// </summary>
        public Order(string Orderid, Side side, double price, uint volume, DateTime timeStamp)
        {
            OrderId = Orderid;
            this.side = side;
            Price = price;
            Volume = volume;
            TimeStamp = timeStamp;
        }

        /// <summary>
        /// Create duplicate of limit order object
        /// </summary>
        public Order Clone()
        {
            return new Order(OrderId, side, Price, Volume, TimeStamp);
        }

        /// <summary>
        /// Returns tab-separated string representation of the limit order (price, time, volume, recorded)
        /// </summary>        
        public override string ToString()
        {
            return "Order Details --> OrderID :" + OrderId
                         + " Side = " + side
                         + " Price = " + Price.ToString()
                         + " Volume = " + Volume.ToString()
                         + " TimeStamp = " + TimeStamp.ToString();
        }
    }
}

