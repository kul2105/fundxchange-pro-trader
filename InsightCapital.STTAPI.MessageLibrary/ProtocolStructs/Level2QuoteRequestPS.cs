using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;
using System;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    public class Level2QuoteRequestPS : IProtocolStruct
    {
        public Level2QuoteRequest lvl2QuoteRequest = new Level2QuoteRequest();

        public override int ID
        {
            get { return ProtocolStructIDS.Level2QuotesRequestPS_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(lvl2QuoteRequest.instrumentID);
            bw_.Write(lvl2QuoteRequest.ClientID);            
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            InitRead(buffer);
            lvl2QuoteRequest.instrumentID = br_.ReadUInt32();
            lvl2QuoteRequest.ClientID = br_.ReadString();           
            CloseRead();
        }

        public override void ReadString(string Msgbfr)
        {
            StartStrR(Msgbfr);
        }

        public override void WriteString()
        {
            StartStrW();
            EndStrW();
        }
    }

    public class Level2QuoteRequest
    {
        public string ClientID;
        public uint instrumentID;

        public Level2QuoteRequest()
        {
            ClientID = string.Empty;
            instrumentID = 0;
        }
    }
}

