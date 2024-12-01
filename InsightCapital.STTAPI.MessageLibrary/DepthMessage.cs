using ProtoBuf;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InsightCapital.STTAPI.MessageLibrary
{
    /// <summary>
    /// Author :- Rohit Kumar
    /// Depth Message
    /// </summary>
    [ProtoContract]
    public class DepthMessage : IMessage
    {
        [ProtoMember(1, IsRequired = true)]
        public uint InstrumentID;
        [ProtoMember(2, IsRequired = true)]
        public DepthElements[] buys;
        [ProtoMember(3, IsRequired = true)]
        public DepthElements[] sells;
        [ProtoMember(4, IsRequired = true)]
        public double LastOptionPrice;
        [ProtoMember(5, IsRequired = true)]
        public int SequenceNumber;
        [ProtoMember(6, IsRequired = true)]
        public double LastTradedPrice;
        [ProtoMember(7, IsRequired = true)]
        public double Volatility;
        [ProtoMember(8, IsRequired = true)]
        public double UnderlyingReferencePrice;
        [ProtoMember(9, IsRequired = true)]
        public long LastTradedTime;
        [ProtoMember(10, IsRequired = true)]
        public int BuysCount;
        [ProtoMember(11, IsRequired = true)]
        public int SellsCount;

        public DepthMessage()
        {
            buys = new DepthElements[0];
            sells = new DepthElements[0];
        }

        public DepthMessage(uint pInstrumentID, DepthElements[] pBuys, DepthElements[] pSells, int pSequenceNumber, double pLastOptionprice, double pLastTradedPrice, double pVolatility, double pUnderlyingReferencePrice, DateTime pLastTradedTime)
        {
            this.InstrumentID = pInstrumentID;
            this.buys = pBuys;
            this.BuysCount = pBuys.Length;
            this.sells = pSells;
            this.SellsCount = pSells.Length;
            this.SequenceNumber = pSequenceNumber;
            this.LastOptionPrice = pLastOptionprice;
            this.LastTradedPrice = pLastTradedPrice;
            this.Volatility = pVolatility;
            this.UnderlyingReferencePrice = pUnderlyingReferencePrice;
            this.LastTradedTime = pLastTradedTime.Ticks;
        }

        public string GetMessageString()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "Message Type :- Depth Message" + " InstrumentID :-" + InstrumentID.ToString()
             + " buys :-" + GetBuySellString(buys) + " BuysCount :-" + BuysCount.ToString()
             + " sells :-" + GetBuySellString(sells) + " SellsCount :-" + SellsCount.ToString()
             + " SequenceNumber :-" + SequenceNumber.ToString() + "LastOptionPrice :-" + LastOptionPrice
             + " LastTradedPrice :-" + LastTradedPrice + "Volatility :-" + Volatility
             + " UnderlyingReferencePrice :-" + UnderlyingReferencePrice.ToString()
             + "LastTradedTime :-" + new DateTime(LastTradedTime);
        }

        private string GetBuySellString(DepthElements[] depthElements)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DepthElements item in depthElements)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }
    }
}
