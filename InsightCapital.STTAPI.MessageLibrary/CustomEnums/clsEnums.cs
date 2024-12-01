using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightCapital.STTAPI.MessageLibrary
{
    public enum Status
    {
        LoginAccepted = 'A',
        CompIDInactiveLocked = 'a',
        LoginLimitReached = 'b',
        ServiceUnavailable = 'c',
        ConcurrentLimitReached = 'd',
        FailedOther = 'e'
    }

    public enum ReplayStatus
    {
        RequestAccepted = 'A',
        RequestLimitReached = 'D',
        InvalidMarketDataGroup = 'I',
        OutofRange = 'O',
        ReplayUnavailable = 'U',
        ConcurrentLimitReached = 'c',
        Unsupportedmessagetype = 'd',
        FailedOther = 'e'
    }

    public enum SnapshotStatus
    {
        RequestAccepted = 'A',
        OutofRange = 'O',
        ReplayUnavailable = 'U',
        SegmentInstrumentIDorSubBookInvalidorNotSpecified = 'a',
        RequestLimitReached = 'b',
        ConcurrentLimitReached = 'c',
        Unsupportedmessagetype = 'd',
        FailedOther = 'e'
    }

    public enum SnapshotType
    {
        OrderBook = 0,
        InstrumentStatus = 1,
        Instrument = 2,
        Trades = 3,
        Statistics = 4,
        News = 5,
        TopofBook = 8
    }

    public enum Urgency
    {
        Regular = 0,
        HighPriority = 1,
        LowPriority = 2
    }

    public enum EventCode
    {
        EndOfDay = 'C',
        StartOfDay = 'O',
        StartOfSystemHours = 'S',
        StartOfMarketHours = 'Q',
        EndOfMarketHours = 'M',
        EndOfSystemHours = 'E',
        EMCHalt = 'A',
        EMCQuoteOnlyPeriod = 'R',
        EMCResumption = 'B'
    }

    public enum MarketCategory
    {
        NYSE = 'N',
        NYSEAmex = 'A',
        NYSEArca = 'P',
        NASDAQGlobalSelectMarket = 'Q',
        NASDAQGlobalMarket = 'G',
        NASDAQCapitalMarket = 'S'
    }

    public enum FinancialStatusIndicator
    {
        Deficient = 'D',
        Delinquent = 'E',
        Bankrupt = 'Q',
        Suspended = 'S',
        DeficientAndBankrupt = 'G',
        DeficientAndDelinquent = 'H',
        DelinquentAndBankrupt = 'J',
        DeficientDelinquentAndBankrupt = 'K',
        InCompliance = ' '
    }

    public enum RoundLotsOnly
    {
        Yes = 'Y',
        No = 'N'
    }

    public enum TradingStatus
    {
        Halt = 'H',
        RegularTradingORStartTradeReporting = 'T',
        OpeningAuctionCall = 'a',
        PostClose = 'b',
        MarketClose = 'c',
        ClosingAuctionCall = 'd',
        VolatalityAuctionCall = 'e',
        EODVolumeAuctionCall = 'E',
        ReOpeningAuctionCall = 'f',
        Pause = 'l',
        FuturesCloseOut = 'p',
        ClosingPriceCross = 's',
        IntraDayAuctionCall = 'u',
        EndTradeReporting = 'v',
        NoActiveSession = 'w',
        EndOfPostClose = 'x',
        StartOfTrading = 'y',
        ClosingPricePublication = 'z'
    }

    public enum PrimaryMarketMaker
    {
        Yes = 'Y',
        No = 'N'
    }

    public enum MarketMakerMode
    {
        Normal = 'N',
        Passive = 'P',
        Syndicate = 'S',
        PreSyndicate = 'R',
        Penalty = 'L'
    }

    public enum MarketParticipantState
    {
        Active = 'A',
        ExcusedOrWithdrawn = 'E',
        Withdrawn = 'W',
        Suspended = 'S',
        Deleted = 'D'
    }

    public enum Side
    {
        Buy = 'B',
        Sell = 'S',
        NA = 'X'
    }

    public enum Printable
    {
        NonPrintable = 'N',
        Printable = 'Y'
    }

    public enum AuctionType
    {
        OpeningAuction = 'C',
        ClosingAuction = 'O',
        Volatility = 'A',
        ReOpeningAuction = 'E',
        IntraDayAuction = 'K',
        FuturesCloseOutAuction = 'L',
        EODVolumneAuctionCall = 'D'
    }

    public enum ActionType
    {
        CancelledTrade = 'C',
        Trade = 'N'
    }

    public enum Action
    {
        Update = 1,
        Delete = 2
    }

    public enum ImbalanceDirection
    {
        InsufficientOrdersforAuction = 'O',
        BuyImbalance = 'B',
        SellImbalance = 'S',
        NoImbalance = 'N'
    }

    public enum BookType
    {
        OnBook = 1,
        OffBook = 2,
        BulletinBoard = 9,
        NegotiatedTrades = 11
    }

    public enum BookTypeOrderClear
    {
        MBO = 0,
        TopOfBook = 1
    }

    public enum SettlementMethod
    {
        Cash = 'C',
        Physical = 'P',
        NA = 'X'
    }

    public enum optionType
    {
        CallOption = 'C',
        PutOption = 'P'
    }

    public enum Flags
    {
        TradeConditionFlag = 0,
        CrossOrderTrade = 1
    }

    public enum OrderModifiedFlags
    {
        PriorityLost = 0,
        PriorityRetained = 1
    }

    public enum SubBook
    {
        Regular = 1,
        OffBook = 2,
        BulletinBoard = 9,
        NegotiatedTrades = 11
    }

    public enum SubBookSnapshot
    {
        Regular = 0,
        OffBook = 1,
        BulletinBoard = 5,
        NegotiatedTrades = 6
    }

    public enum SessionChangeReason
    {
        ScheduledTransition = 0,
        ExtendedbyMarketOps = 1,
        ShortenedbyMarketOps = 2,
        MarketOrderImbalance = 3,
        PriceOutsideRange = 4,
        CircuitBreakerTripped = 5,
        Unavailable = 9
    }

    public enum PriceVariationIndicator
    {
        LessThanOnePercent = 'L',
        OneToTwoPercent = '1',
        TwoToThreePercent = '2',
        ThreeToFourPercent = '3',
        FourToFivePercent = '4',
        FiveToSixPercent = '5',
        SixToSevenPercent = '6',
        SevenToEightPercent = '7',
        EightToNinePercent = '8',
        NineToTenPercent = '9',
        TenToTwentyPercent = 'A',
        TwentyToThirtyPercent = 'B',
        GreaterThanThirtyPercent = 'C',
        CannotCalculate = ' '
    }


    public enum TradeType
    {
        OnBookTrade = 'T',
        OffBookTrade = 'N',
        NegotiatedTrade = 'R'
    }

    public enum OrderTransType
    {
        Add,
        Update,
        Delete,
        Initial,
        NA
    }
    public enum OrderType
    {
        MARKET = 1,
        LIMIT,
        STP,
        STL,
        SLTP,
        NA
    }

    public enum OrderRequestType
    {
        NA = 1,
        NEW,
        MODIFIED,
        CANCELLED,
        CLOSED,
        SLTP_URG_CLOSED

    }

    public enum UserType
    {
        Client,
        Accountant,
        Programmer,
        Administrator,
        Boss,
        HouseAccount,
        NA
    }

    //Closed added in  OrderRequestType
    //public enum enm4OMS
    //{
    //    NA,
    //    CLOSED,
    //    SLTP

    //}
    public enum TIF
    {
        DAY = 1,
        DATE = 2,
        GTC = 3,
        NA = 4,
        NOT_DEFINED

    }

    public enum Reason
    {
        NA = 1,
        ACCEPTED,
        ORDER_NOT_FOUND,
        CANCELLED,
        MODIFIED,
        EXCHANGE_EMPTY,
        BALANCE_ZERO,
        NOT_ENOUGH_MONEY,
        MARGIN_DATA_UNAVAILABLE,
        SL_TP_QTY_SHOULD_BE_GREATER_THAN_ZERO,
        PRICE_QTY_SHOULD_BE_GREATER_THAN_ZERO,
        OME_INTERNAL_ERROR,
        EXPIRED,
        SESSION_CLOSED,
        SYMBOL_NOT_AVAILABLE,
        SERVICCE_UNAVAILABLE,
        SYMBOL_EXPIRED,
        VALIDATION_FAILED,
        MARKET_PRICE_NOT_AVAILABLE
    }

    public enum OrderStatus
    {
        NEW = 1,
        WORKING = 2,
        FILLED = 3,
        PARTIALLY_FILLED = 4,
        CANCELED = 5,
        REJECTED = 6,
        EXPIRED = 7,
        CANCEL_PENDING = 8,
        PENDING = 9,
        CLOSE_PROCESS = 10,
        PARTIAL_CLOSED = 11,
        CLOSED = 12,
        MULTIPLE = 13,
        NOTVALID = 15,

        CNCIP = 17,
        MODIFY_PENDING = 18,
        MODIFIED = 19,
        ACKNOWLEDGE = 20,
        PARTIALLY_REJECTED = 21,
        SLTP_URG_CLOSED_IN_PROCESS = 23,
        PARTIAL_CANCELLED = 24,
        SENT = 25
    }

    public enum TradeStatus
    {
        NA = 1,
        FILLED,
        PARTIALLY_FILLED

    }
    public enum MarketStatus
    {
        Open = 1,
        Closed,
        Suspend,
        CompMode,
        NA
    }
    public enum SvrlMSG
    {
        Duplicate_Client,
        NA
    }

    public enum DataType
    {
        BinaryType,
        StringType
    }

    public enum Authentication
    {
        Authenticated,
        Declined
    }
}


