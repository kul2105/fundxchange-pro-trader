using InsightCapital.STTAPI.MessageLibrary.ProtocolStructs;
using System;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    public class QuoteRequestPS : IProtocolStruct
    {
        public QuoteRequest QuoteRequest = new QuoteRequest();

        public override int ID
        {
            get { return ProtocolStructIDS.Quote_Request_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(QuoteRequest.instrumentID);
            bw_.Write(QuoteRequest.ClientID);
            // bw_.Write(QuoteRequest.isSubscribe);
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            InitRead(buffer);
            QuoteRequest.instrumentID = br_.ReadUInt32();
            QuoteRequest.ClientID = br_.ReadString();
            // QuoteRequest.isSubscribe = br_.ReadBoolean();
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

    public class QuoteRequest
    {
        public string ClientID;
        public uint instrumentID;
        // public bool isSubscribe = true;

        public QuoteRequest()
        {
            ClientID = string.Empty;
            instrumentID = 0;
            // isSubscribe = false;
        }
    }
}

