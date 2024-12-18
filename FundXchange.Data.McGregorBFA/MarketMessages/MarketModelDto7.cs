//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: MarketModelDto7.proto
namespace Fin24.LiveData.Common.DTOs
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TradeDtoCollection")]
  public partial class TradeDtoCollection : global::ProtoBuf.IExtensible
  {
    public TradeDtoCollection() {}
    
    private readonly global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.TradeDto> _Trades = new global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.TradeDto>();
    [global::ProtoBuf.ProtoMember(1, Name=@"Trades", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.TradeDto> Trades
    {
      get { return _Trades; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"TradeDto")]
  public partial class TradeDto : global::ProtoBuf.IExtensible
  {
    public TradeDto() {}
    

    private string _Symbol = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Symbol", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Symbol
    {
      get { return _Symbol; }
      set { _Symbol = value; }
    }

    private string _Exchange = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"Exchange", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Exchange
    {
      get { return _Exchange; }
      set { _Exchange = value; }
    }

    private string _ISIN = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"ISIN", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ISIN
    {
      get { return _ISIN; }
      set { _ISIN = value; }
    }

    private string _MarketSegmentCode = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"MarketSegmentCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string MarketSegmentCode
    {
      get { return _MarketSegmentCode; }
      set { _MarketSegmentCode = value; }
    }

    private string _CountryOfRegister = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"CountryOfRegister", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string CountryOfRegister
    {
      get { return _CountryOfRegister; }
      set { _CountryOfRegister = value; }
    }

    private string _CurrencyCode = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"CurrencyCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string CurrencyCode
    {
      get { return _CurrencyCode; }
      set { _CurrencyCode = value; }
    }

    private string _TradeCode = "";
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"TradeCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string TradeCode
    {
      get { return _TradeCode; }
      set { _TradeCode = value; }
    }

    private long _TradePrice = default(long);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"TradePrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long TradePrice
    {
      get { return _TradePrice; }
      set { _TradePrice = value; }
    }

    private long _TradeSize = default(long);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"TradeSize", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long TradeSize
    {
      get { return _TradeSize; }
      set { _TradeSize = value; }
    }

    private string _Counterparty1 = "";
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"Counterparty1", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Counterparty1
    {
      get { return _Counterparty1; }
      set { _Counterparty1 = value; }
    }

    private string _Counterparty2 = "";
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"Counterparty2", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Counterparty2
    {
      get { return _Counterparty2; }
      set { _Counterparty2 = value; }
    }

    private long _TradeDateAsTicks = default(long);
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"TradeDateAsTicks", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long TradeDateAsTicks
    {
      get { return _TradeDateAsTicks; }
      set { _TradeDateAsTicks = value; }
    }

    private long _TradeTimeAsTicks = default(long);
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"TradeTimeAsTicks", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long TradeTimeAsTicks
    {
      get { return _TradeTimeAsTicks; }
      set { _TradeTimeAsTicks = value; }
    }

    private string _TradeTypeIndicator = "";
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"TradeTypeIndicator", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string TradeTypeIndicator
    {
      get { return _TradeTypeIndicator; }
      set { _TradeTypeIndicator = value; }
    }

    private string _BargainConditionIndicator = "";
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"BargainConditionIndicator", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string BargainConditionIndicator
    {
      get { return _BargainConditionIndicator; }
      set { _BargainConditionIndicator = value; }
    }

    private string _TradeTimeIndicator = "";
    [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name=@"TradeTimeIndicator", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string TradeTimeIndicator
    {
      get { return _TradeTimeIndicator; }
      set { _TradeTimeIndicator = value; }
    }

    private string _ConvertedPriceIndicator = "";
    [global::ProtoBuf.ProtoMember(17, IsRequired = false, Name=@"ConvertedPriceIndicator", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ConvertedPriceIndicator
    {
      get { return _ConvertedPriceIndicator; }
      set { _ConvertedPriceIndicator = value; }
    }

    private Fin24.LiveData.Common.DTOs.QuoteDto _Quote = null;
    [global::ProtoBuf.ProtoMember(18, IsRequired = false, Name=@"Quote", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public Fin24.LiveData.Common.DTOs.QuoteDto Quote
    {
      get { return _Quote; }
      set { _Quote = value; }
    }

    private long _SequenceNumber = default(long);
    [global::ProtoBuf.ProtoMember(19, IsRequired = false, Name=@"SequenceNumber", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long SequenceNumber
    {
      get { return _SequenceNumber; }
      set { _SequenceNumber = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"QuoteDto")]
  public partial class QuoteDto : global::ProtoBuf.IExtensible
  {
    public QuoteDto() {}
    

    private string _Symbol = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Symbol", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Symbol
    {
      get { return _Symbol; }
      set { _Symbol = value; }
    }

    private string _TradableInstrumentCode = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"TradableInstrumentCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string TradableInstrumentCode
    {
      get { return _TradableInstrumentCode; }
      set { _TradableInstrumentCode = value; }
    }

    private string _CountryOfRegister = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"CountryOfRegister", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string CountryOfRegister
    {
      get { return _CountryOfRegister; }
      set { _CountryOfRegister = value; }
    }

    private string _MarketSegmentCode = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"MarketSegmentCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string MarketSegmentCode
    {
      get { return _MarketSegmentCode; }
      set { _MarketSegmentCode = value; }
    }

    private string _CurrencyCode = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"CurrencyCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string CurrencyCode
    {
      get { return _CurrencyCode; }
      set { _CurrencyCode = value; }
    }

    private string _MarketSectorCode = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"MarketSectorCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string MarketSectorCode
    {
      get { return _MarketSectorCode; }
      set { _MarketSectorCode = value; }
    }

    private long _VolumeOfOrdersAtBestBidPrice = default(long);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"VolumeOfOrdersAtBestBidPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long VolumeOfOrdersAtBestBidPrice
    {
      get { return _VolumeOfOrdersAtBestBidPrice; }
      set { _VolumeOfOrdersAtBestBidPrice = value; }
    }

    private long _VolumeOfOrdersAtBestOfferPrice = default(long);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"VolumeOfOrdersAtBestOfferPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long VolumeOfOrdersAtBestOfferPrice
    {
      get { return _VolumeOfOrdersAtBestOfferPrice; }
      set { _VolumeOfOrdersAtBestOfferPrice = value; }
    }

    private int _NumberOfOrdersAtBestBidPrice = default(int);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"NumberOfOrdersAtBestBidPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int NumberOfOrdersAtBestBidPrice
    {
      get { return _NumberOfOrdersAtBestBidPrice; }
      set { _NumberOfOrdersAtBestBidPrice = value; }
    }

    private int _NumberOfOrdersAtBestOfferPrice = default(int);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"NumberOfOrdersAtBestOfferPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int NumberOfOrdersAtBestOfferPrice
    {
      get { return _NumberOfOrdersAtBestOfferPrice; }
      set { _NumberOfOrdersAtBestOfferPrice = value; }
    }

    private string _BidMarketOrdersIndicator = "";
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"BidMarketOrdersIndicator", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string BidMarketOrdersIndicator
    {
      get { return _BidMarketOrdersIndicator; }
      set { _BidMarketOrdersIndicator = value; }
    }

    private string _OfferMarketOrdersIndicator = "";
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"OfferMarketOrdersIndicator", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string OfferMarketOrdersIndicator
    {
      get { return _OfferMarketOrdersIndicator; }
      set { _OfferMarketOrdersIndicator = value; }
    }

    private long _BestBidPrice = default(long);
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"BestBidPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long BestBidPrice
    {
      get { return _BestBidPrice; }
      set { _BestBidPrice = value; }
    }

    private long _BestOfferPrice = default(long);
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"BestOfferPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long BestOfferPrice
    {
      get { return _BestOfferPrice; }
      set { _BestOfferPrice = value; }
    }

    private long _MidPrice = default(long);
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"MidPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long MidPrice
    {
      get { return _MidPrice; }
      set { _MidPrice = value; }
    }

    private string _BestPriceStatusIndicator = "";
    [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name=@"BestPriceStatusIndicator", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string BestPriceStatusIndicator
    {
      get { return _BestPriceStatusIndicator; }
      set { _BestPriceStatusIndicator = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
