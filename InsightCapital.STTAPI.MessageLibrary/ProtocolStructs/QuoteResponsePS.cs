using System;

namespace InsightCapital.STTAPI.MessageLibrary.ProtocolStructs
{
    public class QuoteResponsePS : IProtocolStruct
    {
        public SymbolData symbolData = new SymbolData();

        public QuoteResponsePS()
        {

        }

        public QuoteResponsePS(SymbolData data)
        {
            symbolData = new SymbolData(data);
        }

        public override int ID
        {
            get { return ProtocolStructIDS.Quote_ResponsePS_ID; }
        }

        public override void StartWrite(byte[] buffer)
        {
            InitWrite(buffer);
            bw_.Write(symbolData.InstrumentID);
            bw_.Write(symbolData.Symbol);
            bw_.Write(symbolData.ISIN);
            bw_.Write(symbolData.PreviousClose);

            bw_.Write(symbolData.BidPx);
            bw_.Write(symbolData.BidSize);
            bw_.Write(symbolData.AskPx);
            bw_.Write(symbolData.AskSize);

            bw_.Write(symbolData.LastTradeDateTime.ToBinary());
            bw_.Write(symbolData.LastTradePrice);

            bw_.Write(symbolData.LastOptionPrice);
            bw_.Write(symbolData.UnderlyingReferencePrice);
            bw_.Write(symbolData.Volatility);

            bw_.Write(symbolData.Exchange);

            bw_.Write(symbolData.Open);
            bw_.Write(symbolData.Close);

            bw_.Write(symbolData.High);
            bw_.Write(symbolData.Low);
            bw_.Write(symbolData.VWAP);
            bw_.Write(symbolData.Volume);
            bw_.Write(symbolData.Turnover);
            bw_.Write(symbolData.NoOfTrades);
            bw_.Write(symbolData.NationalExposure);
            bw_.Write(symbolData.NationalDeltaExposure);
            bw_.Write(symbolData.OpenInterest);
            CloseWrite();
        }

        public override void StartRead(byte[] buffer)
        {
            InitRead(buffer);
            symbolData.InstrumentID = br_.ReadUInt32();
            symbolData.Symbol = br_.ReadString();
            symbolData.ISIN = br_.ReadString();
            symbolData.PreviousClose = br_.ReadDecimal();

            symbolData.BidPx = br_.ReadDouble();
            symbolData.BidSize = br_.ReadUInt32();
            symbolData.AskPx = br_.ReadDouble();
            symbolData.AskSize = br_.ReadUInt32();

            symbolData.LastTradeDateTime = DateTime.FromBinary(br_.ReadInt64());
            symbolData.LastTradePrice = br_.ReadDouble();

            symbolData.LastOptionPrice = br_.ReadDouble();
            symbolData.UnderlyingReferencePrice = br_.ReadDouble();
            symbolData.Volatility = br_.ReadDouble();

            symbolData.Exchange = br_.ReadString();

            symbolData.Open = br_.ReadDecimal();
            symbolData.Close = br_.ReadDecimal();

            symbolData.High = br_.ReadDecimal();
            symbolData.Low = br_.ReadDecimal();
            symbolData.VWAP = br_.ReadDecimal();
            symbolData.Volume = br_.ReadUInt32();
            symbolData.Turnover = br_.ReadDecimal();
            symbolData.NoOfTrades = br_.ReadUInt32();
            symbolData.NationalExposure = br_.ReadDecimal();
            symbolData.NationalDeltaExposure = br_.ReadDecimal();
            symbolData.OpenInterest = br_.ReadDecimal();
            CloseRead();
        }

        public override string ToString()
        {
            if (symbolData == null)
                return string.Empty;
            else
            {
                return symbolData.ToString();
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
    public class SymbolData
    {
        public uint InstrumentID { get; set; }// Symbol Directory
        public string Symbol { get; set; }// Symbol Directory
        public string ISIN { get; set; }// Symbol Directory
        public decimal PreviousClose { get; set; } // Symbol Directory   
                                                   // public decimal ContractMultiplier { get; set; }// Symbol Directory

        public double BidPx { get; set; }//Order
        public uint BidSize { get; set; }//order
        public double AskPx { get; set; }//Order
        public uint AskSize { get; set; }//Order

        public DateTime LastTradeDateTime { get; set; } // Trade
        public double LastTradePrice { get; set; } // Trade
        public double LastOptionPrice { get; set; } // Trade
        public double UnderlyingReferencePrice { get; set; } // Trade
        public double Volatility { get; set; } // Trade

        public string Exchange { get; set; } // Hardcoded     

        public decimal Open; // Statistics
        public decimal Close; // Statistics

        public decimal High { get; set; } // Extended Statistics
        public decimal Low { get; set; } // Extended Statistics
        public decimal VWAP { get; set; } // Extended Statistics
        public uint Volume { get; set; } // Extended Statistics
        public decimal Turnover { get; set; } // Extended Statistics
        public uint NoOfTrades { get; set; } // Extended Statistics        
        public decimal NationalExposure { get; set; } // Extended Statistics
        public decimal NationalDeltaExposure { get; set; } // Extended Statistics
        public decimal OpenInterest { get; set; } // Extended Statistics

        public SymbolData()
        {
            InstrumentID = 0;
            Symbol = string.Empty;
            ISIN = string.Empty;
            PreviousClose = 0;
            // ContractMultiplier = new decimal(0);

            BidPx = 0;
            BidSize = 0;
            AskPx = 0;
            AskSize = 0;

            LastTradeDateTime = DateTime.UtcNow;
            LastTradePrice = 0;
            LastOptionPrice = 0;
            UnderlyingReferencePrice = 0;
            Volatility = 0;

            Exchange = "JSE";

            Open = 0;
            Close = 0;

            High = new decimal(0);
            Low = new decimal(0);
            VWAP = new decimal(0);
            Volume = 0;
            Turnover = new decimal(0);
            NoOfTrades = 0;
            NationalExposure = new decimal(0);
            NationalDeltaExposure = new decimal(0);
            OpenInterest = new decimal(0);
        }

        public SymbolData(SymbolData quote)
        {
            InstrumentID = quote.InstrumentID;
            Symbol = quote.Symbol;
            ISIN = quote.ISIN;
            PreviousClose = quote.PreviousClose;
            // ContractMultiplier = quote.ContractMultiplier;

            BidPx = quote.BidPx;
            BidSize = quote.BidSize;
            AskPx = quote.AskPx;
            AskSize = quote.AskSize;

            LastTradeDateTime = quote.LastTradeDateTime;
            LastTradePrice = quote.LastTradePrice;
            LastOptionPrice = quote.LastOptionPrice;
            UnderlyingReferencePrice = quote.UnderlyingReferencePrice;
            Volatility = quote.Volatility;

            Exchange = "JSE";

            Open = quote.Open;
            Close = quote.Close;

            High = quote.High;
            Low = quote.Low;
            VWAP = quote.VWAP;
            Volume = quote.Volume;
            Turnover = quote.Turnover;
            NoOfTrades = quote.NoOfTrades;
            NationalExposure = quote.NationalExposure;
            NationalDeltaExposure = quote.NationalDeltaExposure;
            OpenInterest = quote.OpenInterest;
        }

        public override string ToString()
        {
            return "MessageType = Quote Response :" + " InstrumentID = " + InstrumentID.ToString() + " Symbol = " + Symbol + " ISIN = " + ISIN.ToString()
                + " PreviousClose = " + PreviousClose.ToString() + " BidPx = " + BidPx.ToString()
                + " BidSize = " + BidSize.ToString() + " AskPx = " + AskPx.ToString() + " AskSize = " + AskSize.ToString()
                + " LastTradeDateTime = " + LastTradeDateTime.Date.ToString() + " LastTradePrice = " + LastTradePrice.ToString() + " Exchange = " + Exchange.ToString() + " Open = " + Open.ToString()
                + " Close = " + Close.ToString() + " High = " + High.ToString() + " Low = " + Low.ToString()
                + " VWAP = " + VWAP.ToString() + " Volume = " + Volume.ToString() + " Turnover = " + Turnover.ToString()
                + " NoOfTrades = " + NoOfTrades.ToString() + " NationalExposure = " + NationalExposure.ToString() + " NationalDeltaExposure = " + NationalDeltaExposure.ToString()
                + " OpenInterest = " + OpenInterest.ToString();
        }
    }
}

