using System.Xml.Serialization;

namespace PALSA
{
    [XmlRoot("ConnectionIPs")]
    public class ConnectionIPs
    {
        [XmlElement("OrderIP")]
        public Settings OrderIP { get; set; }

        [XmlElement("QuotesIP")]
        public Settings QuotesIP { get; set; }

        [XmlElement("RefreshServer")]
        public RefreshServer RefreshServer { get; set; }
    }

    public class Settings
    {
        [XmlAttribute("ServerIP")]
        public string ServerIP { get; set; }

        [XmlAttribute("HostIP")]
        public string HostIP { get; set; }

        [XmlAttribute("PortNo")]
        public int PortNo { get; set; }

        public int ValueAlerts { get; set; }

        public string UserName { get; set; }

        public System.Drawing.Color UpTrendColor { get; set; }

        public string TradingCurrencyAlerts { get; set; }

        public string TradesHotKey { get; set; }

        public string TradeProfile { get; set; }

        public string TopGainersLosersHotKey { get; set; }

        public string TimeFormat { get; set; }

        public int TickerSpeed { get; set; }

        public string TickerPortfolio { get; set; }

        public string TickerHotKey { get; set; }

        public string TickerDisplay { get; set; }

        public int TickerComoditySpeed { get; set; }

        public string TickerComodityDisplay { get; set; }

        public int Theme { get; set; }

        public string StatusBarHotKey { get; set; }

        public int SpreadIOCPriceAlerts { get; set; }

        public string SnapQuoteHotKey { get; set; }

        public string SIOC { get; set; }

        public string ServeillanceDefaultProfile { get; set; }

        public System.Drawing.Color SellOrderColor { get; set; }

        public int QuantityAlerts { get; set; }

        public int PriceAlerts { get; set; }

        public string PrefixOmniIdWith { get; set; }

        public string PrefixClientIDWith { get; set; }

        public string PreferencesHotKey { get; set; }

        public int PositionSizeType { get; set; }

        public string PortfolioProfile { get; set; }

        public string PortfolioHotKey { get; set; }

        public string PlaceSellOrderHotKey { get; set; }

        public string PlaceBuyOrderHotKey { get; set; }

        public string OrderValidity { get; set; }

        public int OrderFormSetting { get; set; }

        public string OrderEntryOnce { get; set; }

        public string OrderEntryMultiple { get; set; }

        public string OrderBookProfile { get; set; }

        public string OrderBookHotKey { get; set; }

        public string OpenAllViewsWith { get; set; }

        public string NetPositionProfile { get; set; }

        public string NetPositionHotKey { get; set; }

        public string ModifyOrderHotKey { get; set; }

        public string ModifiedTradesHotKey { get; set; }

        public int MinOrderQty { get; set; }

        public string MessageLogHotKey { get; set; }

        public string MessageBarMessageType { get; set; }

        public int MaxMessageInMessageBox { get; set; }

        public string MarketWatchProfile { get; set; }

        public string MarketWatchPortfolio { get; set; }

        public string MarketWatchHotKey { get; set; }

        public System.Drawing.Color MarketQuoteUpColor { get; set; }

        public System.Drawing.Color MarketQuoteDownColor { get; set; }

        public System.Drawing.Color MarketQuoteBackColor { get; set; }

        public string MarketPictureHotKey { get; set; }

        public string LstInstruments { get; set; }

        public string LogoffHotKey { get; set; }

        public string LoginPassword { get; set; }

        public string LoginHotKey { get; set; }

        public int LockWorkStationTime { get; set; }

        public string LockWorkstationHotKey { get; set; }

        public string LastTickerPortfolio { get; set; }

        public string LastParticipaintOmniID { get; set; }

        public string LastClientID { get; set; }

        public bool IsUnregisteredClientAlert { get; set; }

        public bool IsTriggerPriceSameAsLimitPrice { get; set; }

        public bool IsTradeModification { get; set; }

        public bool IsShowDateTime { get; set; }

        public bool IsRetainLastParticipaintCodeOmniId { get; set; }

        public bool IsRetainLastClientID { get; set; }

        public bool IsPrintTradeConfirm { get; set; }

        public bool IsPrintOrderConfirm { get; set; }

        public bool IsPriceDecimalSelection { get; set; }
    }

    public class RefreshServer
    {
        [XmlAttribute("ServerIP")]
        public string ServerIP { get; set; }

        [XmlAttribute("User")]
        public string User { get; set; }

        [XmlAttribute("Pwd")]
        public string Pwd { get; set; }

        [XmlAttribute("Root")]
        public string Root { get; set; }

        [XmlAttribute("PortNo")]
        public int PortNo { get; set; }
    }
}