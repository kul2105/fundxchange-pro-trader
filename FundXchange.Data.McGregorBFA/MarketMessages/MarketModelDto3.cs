//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: MarketModelDto3.proto
namespace Fin24.LiveData.Common.DTOs
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EquityDtoCollection")]
  public partial class EquityDtoCollection : global::ProtoBuf.IExtensible
  {
    public EquityDtoCollection() {}
    
    private readonly global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.EquityDto> _Equities = new global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.EquityDto>();
    [global::ProtoBuf.ProtoMember(1, Name=@"Equities", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Fin24.LiveData.Common.DTOs.EquityDto> Equities
    {
      get { return _Equities; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EquityDto")]
  public partial class EquityDto : global::ProtoBuf.IExtensible
  {
    public EquityDto() {}
    

    private string _Symbol = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Symbol", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Symbol
    {
      get { return _Symbol; }
      set { _Symbol = value; }
    }

    private string _ISIN = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"ISIN", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ISIN
    {
      get { return _ISIN; }
      set { _ISIN = value; }
    }

    private string _ShortName = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"ShortName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ShortName
    {
      get { return _ShortName; }
      set { _ShortName = value; }
    }

    private string _LongName = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"LongName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string LongName
    {
      get { return _LongName; }
      set { _LongName = value; }
    }

    private Fin24.LiveData.Common.DTOs.InstrumentTypeDto _InstrumentType = Fin24.LiveData.Common.DTOs.InstrumentTypeDto.Automated_Input_Facility;
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"InstrumentType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(Fin24.LiveData.Common.DTOs.InstrumentTypeDto.Automated_Input_Facility)]
    public Fin24.LiveData.Common.DTOs.InstrumentTypeDto InstrumentType
    {
      get { return _InstrumentType; }
      set { _InstrumentType = value; }
    }

    private string _ExchangeCode = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"ExchangeCode", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ExchangeCode
    {
      get { return _ExchangeCode; }
      set { _ExchangeCode = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"InstrumentTypeDto")]
    public enum InstrumentTypeDto
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"Automated_Input_Facility", Value=0)]
      Automated_Input_Facility = 0,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Allotment_Letters", Value=1)]
      Allotment_Letters = 1,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Bonds", Value=2)]
      Bonds = 2,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Bulldogs", Value=3)]
      Bulldogs = 3,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Convertible", Value=4)]
      Convertible = 4,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Covered_Warrants", Value=5)]
      Covered_Warrants = 5,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Debentures", Value=6)]
      Debentures = 6,
            
      [global::ProtoBuf.ProtoEnum(Name=@"UK_Equity", Value=7)]
      UK_Equity = 7,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Depository_Receipt", Value=8)]
      Depository_Receipt = 8,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Equity_Warrant", Value=9)]
      Equity_Warrant = 9,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Foreign_Government_Bonds", Value=10)]
      Foreign_Government_Bonds = 10,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Fully_Paid_Letter", Value=11)]
      Fully_Paid_Letter = 11,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Foreign_Unit_Trusts", Value=12)]
      Foreign_Unit_Trusts = 12,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Gilts", Value=13)]
      Gilts = 13,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Gilt_Warrants", Value=14)]
      Gilt_Warrants = 14,
            
      [global::ProtoBuf.ProtoEnum(Name=@"International_Equity", Value=15)]
      International_Equity = 15,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Investment_Product", Value=16)]
      Investment_Product = 16,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Kruger_Rand", Value=17)]
      Kruger_Rand = 17,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Loan_Stock", Value=18)]
      Loan_Stock = 18,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Medium_Term_Loans", Value=19)]
      Medium_Term_Loans = 19,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Miscellaneous_Warrants", Value=20)]
      Miscellaneous_Warrants = 20,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Nil_Paid_Letter", Value=21)]
      Nil_Paid_Letter = 21,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Ordinary_Share", Value=22)]
      Ordinary_Share = 22,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Partly_paid_letter", Value=23)]
      Partly_paid_letter = 23,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Portfolio_Notification", Value=24)]
      Portfolio_Notification = 24,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Preference_Shares", Value=25)]
      Preference_Shares = 25,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Package_Units", Value=26)]
      Package_Units = 26,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Rights", Value=27)]
      Rights = 27,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Structured_Products", Value=28)]
      Structured_Products = 28,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Stapled_Unit", Value=29)]
      Stapled_Unit = 29,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Tradable_Fund", Value=30)]
      Tradable_Fund = 30,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Unit_Trust", Value=31)]
      Unit_Trust = 31,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Warrants", Value=32)]
      Warrants = 32,
            
      [global::ProtoBuf.ProtoEnum(Name=@"Zero_Coupon_Bonds", Value=33)]
      Zero_Coupon_Bonds = 33
    }
  
}
