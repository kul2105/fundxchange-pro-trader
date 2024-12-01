using System;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    public class SymbolMasterResponsePS : IProtocolStruct
    {
        public SymbolMasterResponsePS()
        {

        }

        public SymbolMasterResponsePS(SymbolMasterResponse SymbolResponsedata)
        {
            SymbolResponse = SymbolResponsedata;
        }

        public SymbolMasterResponse SymbolResponse = new SymbolMasterResponse();

        public override int ID
        {
            get { return ProtocolStructIDS.SymbolMasterResponsePS_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(SymbolResponse.InstrumentId);
            bw_.Write(SymbolResponse.Symbol);
            bw_.Write(SymbolResponse.SymbolStatus);
            bw_.Write(SymbolResponse.ISIN);
            bw_.Write(SymbolResponse.TIDM);
            bw_.Write(SymbolResponse.Segment);
            bw_.Write(SymbolResponse.Underlying);
            bw_.Write(SymbolResponse.Issuer);
            bw_.Write(SymbolResponse.IssueDate.ToString());           
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            InitRead(buffer);
            SymbolResponse.InstrumentId = br_.ReadUInt32();
            SymbolResponse.Symbol = br_.ReadString();
            SymbolResponse.SymbolStatus = br_.ReadString();
            SymbolResponse.ISIN = br_.ReadString();
            SymbolResponse.TIDM = br_.ReadString();
            SymbolResponse.Segment = br_.ReadString();
            SymbolResponse.Underlying = br_.ReadString();
            SymbolResponse.Issuer = br_.ReadString();
            SymbolResponse.IssueDate = br_.ReadString();           
            CloseRead();
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
    public class SymbolMasterResponse
    {
        public uint InstrumentId { get; set; }
        public string Symbol { get; set; }
        public string SymbolStatus { get; set; }
        public string ISIN { get; set; }
        public string TIDM { get; set; }
        public string Segment { get; set; }
        public string Underlying { get; set; }
        public string Issuer { get; set; }
        public string IssueDate { get; set; }

        public override string ToString()
        {
            return "Symbol Master Data :-  InstrumentId = " + InstrumentId + " Symbol = " + Symbol + " SymbolStatus = " + SymbolStatus + " ISIN = " + ISIN + " TIDM = " + TIDM
                + " Segment = " + Segment + " Underlying = " + Underlying + " Issuer = " + Issuer + " IssueDate = " + IssueDate.ToString();
        }
    }
}

