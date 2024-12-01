//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: MarketModelDto4.proto
namespace Fin24.LiveData.Common.DTOs
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"IndexCandlestickCollection")]
  public partial class IndexCandlestickCollection : global::ProtoBuf.IExtensible
  {
    public IndexCandlestickCollection() {}
    
    private readonly global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.IndexCandlestick> _Candlesticks = new global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.IndexCandlestick>();
    [global::ProtoBuf.ProtoMember(1, Name=@"Candlesticks", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.IndexCandlestick> Candlesticks
    {
      get { return _Candlesticks; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"IndexCandlestick")]
  public partial class IndexCandlestick : global::ProtoBuf.IExtensible
  {
    public IndexCandlestick() {}
    

    private string _Exchange = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Exchange", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Exchange
    {
      get { return _Exchange; }
      set { _Exchange = value; }
    }

    private string _Identifier = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"Identifier", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Identifier
    {
      get { return _Identifier; }
      set { _Identifier = value; }
    }

    private int _HighPrice = default(int);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"HighPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int HighPrice
    {
      get { return _HighPrice; }
      set { _HighPrice = value; }
    }

    private int _LowPrice = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"LowPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int LowPrice
    {
      get { return _LowPrice; }
      set { _LowPrice = value; }
    }

    private int _OpenPrice = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"OpenPrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int OpenPrice
    {
      get { return _OpenPrice; }
      set { _OpenPrice = value; }
    }

    private int _ClosePrice = default(int);
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"ClosePrice", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int ClosePrice
    {
      get { return _ClosePrice; }
      set { _ClosePrice = value; }
    }

    private long _CloseSequenceNumber = default(long);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"CloseSequenceNumber", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long CloseSequenceNumber
    {
      get { return _CloseSequenceNumber; }
      set { _CloseSequenceNumber = value; }
    }

    private int _NumberOfTrades = default(int);
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"NumberOfTrades", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int NumberOfTrades
    {
      get { return _NumberOfTrades; }
      set { _NumberOfTrades = value; }
    }

    private long _TimeOfStartAsTicks = default(long);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"TimeOfStartAsTicks", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long TimeOfStartAsTicks
    {
      get { return _TimeOfStartAsTicks; }
      set { _TimeOfStartAsTicks = value; }
    }

    private long _TimeOfCloseAsTicks = default(long);
    [global::ProtoBuf.ProtoMember(10, IsRequired = false, Name=@"TimeOfCloseAsTicks", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long TimeOfCloseAsTicks
    {
      get { return _TimeOfCloseAsTicks; }
      set { _TimeOfCloseAsTicks = value; }
    }

    private long _TotalValue = default(long);
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"TotalValue", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long TotalValue
    {
      get { return _TotalValue; }
      set { _TotalValue = value; }
    }

    private long _TotalVolume = default(long);
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"TotalVolume", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long TotalVolume
    {
      get { return _TotalVolume; }
      set { _TotalVolume = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}