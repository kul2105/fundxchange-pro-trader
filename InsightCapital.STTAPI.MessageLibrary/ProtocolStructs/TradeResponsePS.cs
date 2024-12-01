using System;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    public class TradeResponsePS : IProtocolStruct
    {
        public TradeResponsePS()
        {

        }

        public TradeResponsePS(TradeMessage message)
        {
            response_ = message;
        }

        public TradeMessage response_ = new TradeMessage();
        public override int ID
        {
            get { return ProtocolStructIDS.TradeResponsePS_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(response_.InstrumentId);
            bw_.Write(response_.BuySell);
            bw_.Write(response_.OrderId);
            bw_.Write(response_.Price);
            bw_.Write(response_.Quantity);
            bw_.Write(response_.TradeId);
            bw_.Write(response_.LastOptPx);
            bw_.Write(response_.Volatility);
            bw_.Write(response_.UnderlyingReferencePrice);
            bw_.Write(response_.TradeTime.ToBinary());
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            InitRead(buffer);           
            response_.InstrumentId = br_.ReadUInt32();
            response_.BuySell = br_.ReadString();
            response_.OrderId = br_.ReadString();
            response_.Price = br_.ReadDouble();
            response_.Quantity = br_.ReadInt32(); ;
            response_.TradeId = br_.ReadString();
            response_.LastOptPx = br_.ReadDouble();
            response_.Volatility = br_.ReadDouble();
            response_.UnderlyingReferencePrice = br_.ReadDouble();
            response_.TradeTime = DateTime.FromBinary(br_.ReadInt64());
            CloseRead();
        }

        public override void ReadString(string Msgbfr)
        {
            StartStrR(Msgbfr);
            //reponse_.Status_ = Convert.ToBoolean(SpltString[0]);
            //reponse_.Reason_ = SpltString[1];
            //reponse_.Info_ = SpltString[2];
        }
        public override void WriteString()
        {
            StartStrW();
            //AppendStr(reponse_.Status_.ToString());
            //AppendStr(reponse_.Reason_);
            //AppendStr(reponse_.Info_);
            EndStrW();
        }
    }

    [Serializable]
    public class TradeMessage
    {
        public uint InstrumentId { get; set; }
        public string BuySell { get; set; }
        public string OrderId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string TradeId { get; set; }
        public double LastOptPx { get; set; }
        public double Volatility { get; set; }
        public double UnderlyingReferencePrice { get; set; }
        public DateTime TradeTime { get; set; }

        public override string ToString()
        {
            return "Trade Message :-  InstrumentId = " + InstrumentId + " BuySell = " + BuySell
                + " OrderId = " + OrderId + " Price = " + Price + " Quantity = " + Quantity
                + " TradeId = " + TradeId + " LastOptPx = " + LastOptPx + " Volatility = " + Volatility
                + " UnderlyingReferencePrice = " + UnderlyingReferencePrice.ToString()
                 + " TradeTime = " + TradeTime.ToString();
        }
    }
}
