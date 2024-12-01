using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MQTTAPI
{
    public class OHLCV
    {
        public string Symbol { get; set; }
        public string Segment { get; set; }
        public int Volume { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double PreviousClose { get; set; }
        public double VWAP { get; set; }
        public double Volatility { get; set; }
        public int OpenInterest { get; set; }
        public int SecurityId { get; set; }
    }
    public class OrderLine
    {
        public uint Qty { get; set; }
        public double Price { get; set; }
        public string Broker { get; set; }
        public OrderLine(uint qty, double price, string broker)
        {
            this.Qty = qty;
            this.Price = price;
            this.Broker = broker;
        }
        public OrderLine() { }
    }
    public class OrderBook
    {
        public OrderBook()
        {
            BuyBook = new List<OrderLine>();
            SellBook = new List<OrderLine>();
        }
        public List<OrderLine> BuyBook { get; set; }
        public List<OrderLine> SellBook { get; set; }
        public string Symbol { get; set; }
        public string Segment { get; set; }
        public int SecurityId { get; set; }

    }
    public class Quote
    {
        public string Symbol { get; set; }
        public string Segment { get; set; }
        public int Volume { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public double PreviousClose { get; set; }
        public double VWAP { get; set; }
        public double Volatility { get; set; }
        public int OpenInterest { get; set; }
        public int SecurityId { get; set; }
    }
    public class Book
    {
        public string Symbol { get; set; }
        public int SecurityId { get; set; }
        public double BidQty { get; set; }
        public double BidPrice { get; set; }
        public double OfferQty { get; set; }
        public double OfferPrice { get; set; }
    }
    public class Trade
    {
        public long TradeId { get; set; }
        public string Symbol { get; set; }
        public int SecurityId { get; set; }
        public double Price { get; set; }
        public uint Quantity { get; set; }
        public DateTime TradeTime { get; set; }
        public string Segment { get; set; }
        public string Type { get; set; }
    }
    public class MarketDataEntry
    {
        public UInt32 MDUpdateAction { get; set; }
        public string Symbol { get; set; }
        public MDEntryType MDEntryType { get; set; }
        public decimal MDEntryPx { get; set; }
        public decimal NetChgPrevDay { get; set; }
        public string Status { get; set; }
        public string StatusDescription
        {
            get
            {
                switch (Status)
                {
                    case "PRE_MQP":
                        return "Before Mandatory Quote Period, quotes not firm";
                    case "LIVE":
                        return "Market is live, index is normal";
                    case "PART":
                        return "Part calculated value i.e. part of the constituent market is not live";
                    case "INDICATIVE":
                        return "Index is indicative";
                    case "HELD":
                        return "A data link has failed or index has exceeded parameters";
                    case "POST_MQP":
                        return "After Mandatory Quote Period";
                    case "CLOSE":
                        return "Official closing index";
                    default:
                        return "";
                }
            }
        }
        public DateTime MDEntryTime { get; set; }
        public DateTime MDEntryDate { get; set; }
        public string Currency { get; set; }
        public UInt32 RptSeq { get; set; }
    }
    public enum MDEntryType : byte
    {
        IndexValue = (byte)'3',
        PreviousClose = (byte)'f',
        IndexStatus = (byte)'x',
        TotalReturnValue = (byte)'y',
    }
    public enum NewsRefType
    {
        Replacement = 0,
        Cancellation = 100,
    }
    public enum NewsCategory
    {
        Regulatory = 101,
        NonRegulatory = 102,
    }
    public enum Urgency
    {
        Normal = 0,
        HighPriority = 1,
    }
    public class FullSensNews
    {
        public string NewsID { get; set; }
        public UInt32 NewsCounter { get; set; }
        public string SecurityID { get; set; }
        public List<SensNews> NewsItems { get; set; }
        public FullSensNews() { }
        public FullSensNews(SensNews first)
        {
            NewsItems = new List<SensNews>
            {
                first
            };
            NewsID = first.NewsID;
            NewsCounter = first.NewsCounter;
            if (first.RelatedSymbols.Count > 0)
            {
                SecurityID = first.RelatedSymbols[0].SecurityID;
            }
            else
            {
                SecurityID = "NONE";
            }
            AllSymbols = new List<string>();
        }
        public List<string> AllSecurities => NewsItems.SelectMany(x => x.RelatedSymbols.Select(r => r.SecurityID).ToList()).ToList();
        public List<string> AllSymbols { get; set; }
        public bool DoWeHaveAFullMessage => Enumerable.Range(1, (int)NewsCounter).Except(NewsItems.Select(x => (int)x.NewsSequence)).ToList().Count == 0;
        public string FullMessage => String.Join("", NewsItems.OrderBy(x => x.NewsSequence).Select(x => x.FullNewsEntry));
        public string FullAsciiMessage => String.Join("", NewsItems.OrderBy(x => x.NewsSequence).Select(x => x.FullAsciiNewsEntry));
        public DateTime NewsTime => NewsItems.Count > 0 ? NewsItems[0].OrigTime : DateTime.Now;

    }
    public class SensNews
    {
        public string AppID { get; set; }
        public int AppSeqNum { get; set; }
        public string LastRptRequested { get; set; }
        public string NewsID { get; set; }
        public NewsCategory NewsCategory { get; set; }
        public DateTime OrigTime { get; set; }
        public UInt32 EncodedHeadlineLen { get; set; }
        public string EncodedHeadline { get; set; }
        public UInt32 NewsSequence { get; set; }
        public UInt32 NewsCounter { get; set; }
        public string AnnouncementGroupCode { get; set; }
        public string AnnouncementGroupCodeDescription
        {
            get
            {
                switch (AnnouncementGroupCode)
                {
                    case "CCO":
                        return "Competition Commission";
                    case "EXCH":
                        return "Exchange";
                    case "FSB":
                        return "Financial Services Board";
                    case "JSEO":
                        return "Other JSE";
                    case "NSXO":
                        return "Other NSX";
                    case "TRP":
                        return "Takeover Regulation Panel";
                    default: return "";
                }

            }
        }
        public Urgency Urgency { get; set; }
        public string URLLink { get; set; }
        public List<CorrectionEntry> CorrectionEntries { get; set; }
        public List<NewsEntry> NewsEntries { get; set; }
        public List<CompanyEntry> RelatedSymbols { get; set; }
        public string FullNewsEntry => String.Join("", NewsEntries.Select(x => x.EncodedText));
        public string FullAsciiNewsEntry => String.Join("", NewsEntries.Select(x => Encoding1525ToAscii(x.EncodedText)));
        public DateTime Time { get; set; }
        private string Encoding1525ToAscii(string value)
        {
            return Encoding.ASCII.GetString(Encoding.GetEncoding(1252).GetBytes(value));
        }
    }
    public class CompanyEntry
    {
        public string SecurityExchange { get; set; }
        public string CountryOfIssue { get; set; }
        public string SecurityID { get; set; }
        public string SecurityIDSource { get; set; }
        public List<SecurityEntry> SecurityAltIDs { get; set; }
        public string NewsSource { get; set; }

    }
    public class SecurityEntry
    {
        public string SecurityAltID { get; set; }
        public string SecurityAltIDSource { get; set; }
    }
    public class NewsEntry
    {
        public UInt32 EncodedTextLen { get; set; }
        public string EncodedText { get; set; }

    }
    public class CorrectionEntry
    {
        public string NewsRefID { get; set; }
        public NewsRefType NewsRefType { get; set; }

    }
    public partial class SymbolStatus
    {
        public TradingStatus TradingStatus { get; set; }

        public long Flags { get; set; }

        public string Reason { get; set; }

        public SessionChangeReason SessionChangeReason { get; set; }

        public DateTime NewEndTime { get; set; }

        public BookType BookType { get; set; }

        public string Segment { get; set; }

        public string Symbol { get; set; }
        public DateTime Time { get; set; }

        public long SecurityId { get; set; }

        public long SequenceNumber { get; set; }

    }
    public enum TradingStatus : byte
    {
        Halt = (byte)'H',
        RegularTrading = (byte)'T',
        OpeningAuctionCall = (byte)'a',
        PostClose = (byte)'b',
        MarketClose = (byte)'c',
        ClosingAuctionCall = (byte)'d',
        VolatilityAuctionCall = (byte)'e',
        EODVolumeAuctionCall = (byte)'E',
        ReOpeningAuctionCall = (byte)'f',
        Pause = (byte)'I',
        FuturesCloseOut = (byte)'p',
        ClosingPriceCross = (byte)'s',
        IntraDayAuctionCall = (byte)'u',
        EndTradeReporting = (byte)'v',
        NoActiveSession = (byte)'w',
        EndOfPostClose = (byte)'x',
        StartOfTrading = (byte)'y',
        ClosingPricePublication = (byte)'z',
    }
    public enum SessionChangeReason : byte
    {
        ScheduledTransition = 0,
        ExtendedMarketOps = 1,
        ShortenedMarketOps = 2,
        MarketOrderImbalance = 3,
        PriceOutsideRange = 4,
        CircuitBreakerTripped = 5,
        Unavailable = 9,
    }
    public enum BookType : byte
    {
        None = 0,
        Regular = 1,
        OffBook = 2,
        BulletinBoard = 9,
        NegotiatedTrades = 11,
    }

    public class SymbolDirectory
    {
        public SymbolActivityStatus SymbolStatus { get; set; }
        public string ISIN { get; set; }
        public string Symbol { get; set; }
        public string TIDM { get; set; }
        public string Segment { get; set; }
        public long PreviousClosePrice { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Underlying { get; set; }
        public long StrikePrice { get; set; }
        public string OptionType { get; set; }
        public string Issuer { get; set; }
        public DateTime IssueDate { get; set; }
        public long Coupon { get; set; }
        public byte Flags { get; set; }
        public bool InverseOrderBook { get; set; }
        public byte SubBook { get; set; }
        public bool RegularBook { get; set; }
        public bool OffBook { get; set; }
        public bool BulletinBoard { get; set; }
        public bool NegotiatedTrades { get; set; }
        public string CorporateAction { get; set; }
        public List<CorporateActionData> CorporateActions { get; set; }
        public string Leg1Symbol { get; set; }
        public string Leg2Symbol { get; set; }
        public long ContractMultiplier { get; set; }
        public SettlementMethod SettlementMethod { get; set; }
        public string InstrumentSubCategory { get; set; }
    }

    public enum SymbolActivityStatus : byte
    {
        Halted = (byte)'H',
        Suspended = (byte)'S',
        Inactive = (byte)'a',
        Active = (byte)' ',
    }

    public class CorporateActionData
    {
        public string ExMarker { get; set; }
        public DateTime EffectiveFromDate { get; set; }
        public DateTime EffectiveToDate { get; set; }
    }

    public enum SettlementMethod : byte
    {
        Cash = (byte)'C',
        Physical = (byte)'P',
        None = (byte)' ',
    }

    public class AuctionInfo
    {
        public uint PairedQuantity { get; set; }
        public char ImbalanceDirection { get; set; } = ' ';
        public long Price { get; set; }
        public AuctionType AuctionType { get; set; }
        public string Segment { get; set; }
        public string Symbol { get; set; }

    }

    public enum AuctionType : byte
    {
        None = (byte)' ',
        Closing = (byte)'C',
        Opening = (byte)'O',
        Volatility = (byte)'A',
        ReOpening = (byte)'E',
        IntraDay = (byte)'K',
        FuturesCloseOut = (byte)'L',
        EODVolume = (byte)'D',
    }
}
