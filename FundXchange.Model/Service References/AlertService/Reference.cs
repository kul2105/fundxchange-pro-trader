﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FundXchange.Model.AlertService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AlertScriptDTO", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.State.DTOs")]
    [System.SerializableAttribute()]
    public partial class AlertScriptDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AbbreviationField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AlertNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int BarsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BuyScriptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool DayHoursField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DefaultScriptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool EnabledField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool EndOfDayField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExchangeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExitLongScriptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExitShortScriptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool GTCField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool GTCHoursField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int IntervalField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsUserDefinedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool LimitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool MarketField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PeriodField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PortfolioField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int QuantityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SellScriptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool StopLimitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StopLimitValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool StopMarketField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SymbolField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TradeSignalScriptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid UniqueIdField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Abbreviation {
            get {
                return this.AbbreviationField;
            }
            set {
                if ((object.ReferenceEquals(this.AbbreviationField, value) != true)) {
                    this.AbbreviationField = value;
                    this.RaisePropertyChanged("Abbreviation");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AlertName {
            get {
                return this.AlertNameField;
            }
            set {
                if ((object.ReferenceEquals(this.AlertNameField, value) != true)) {
                    this.AlertNameField = value;
                    this.RaisePropertyChanged("AlertName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Bars {
            get {
                return this.BarsField;
            }
            set {
                if ((this.BarsField.Equals(value) != true)) {
                    this.BarsField = value;
                    this.RaisePropertyChanged("Bars");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BuyScript {
            get {
                return this.BuyScriptField;
            }
            set {
                if ((object.ReferenceEquals(this.BuyScriptField, value) != true)) {
                    this.BuyScriptField = value;
                    this.RaisePropertyChanged("BuyScript");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool DayHours {
            get {
                return this.DayHoursField;
            }
            set {
                if ((this.DayHoursField.Equals(value) != true)) {
                    this.DayHoursField = value;
                    this.RaisePropertyChanged("DayHours");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DefaultScript {
            get {
                return this.DefaultScriptField;
            }
            set {
                if ((object.ReferenceEquals(this.DefaultScriptField, value) != true)) {
                    this.DefaultScriptField = value;
                    this.RaisePropertyChanged("DefaultScript");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Enabled {
            get {
                return this.EnabledField;
            }
            set {
                if ((this.EnabledField.Equals(value) != true)) {
                    this.EnabledField = value;
                    this.RaisePropertyChanged("Enabled");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool EndOfDay {
            get {
                return this.EndOfDayField;
            }
            set {
                if ((this.EndOfDayField.Equals(value) != true)) {
                    this.EndOfDayField = value;
                    this.RaisePropertyChanged("EndOfDay");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Exchange {
            get {
                return this.ExchangeField;
            }
            set {
                if ((object.ReferenceEquals(this.ExchangeField, value) != true)) {
                    this.ExchangeField = value;
                    this.RaisePropertyChanged("Exchange");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ExitLongScript {
            get {
                return this.ExitLongScriptField;
            }
            set {
                if ((object.ReferenceEquals(this.ExitLongScriptField, value) != true)) {
                    this.ExitLongScriptField = value;
                    this.RaisePropertyChanged("ExitLongScript");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ExitShortScript {
            get {
                return this.ExitShortScriptField;
            }
            set {
                if ((object.ReferenceEquals(this.ExitShortScriptField, value) != true)) {
                    this.ExitShortScriptField = value;
                    this.RaisePropertyChanged("ExitShortScript");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool GTC {
            get {
                return this.GTCField;
            }
            set {
                if ((this.GTCField.Equals(value) != true)) {
                    this.GTCField = value;
                    this.RaisePropertyChanged("GTC");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool GTCHours {
            get {
                return this.GTCHoursField;
            }
            set {
                if ((this.GTCHoursField.Equals(value) != true)) {
                    this.GTCHoursField = value;
                    this.RaisePropertyChanged("GTCHours");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Interval {
            get {
                return this.IntervalField;
            }
            set {
                if ((this.IntervalField.Equals(value) != true)) {
                    this.IntervalField = value;
                    this.RaisePropertyChanged("Interval");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsUserDefined {
            get {
                return this.IsUserDefinedField;
            }
            set {
                if ((this.IsUserDefinedField.Equals(value) != true)) {
                    this.IsUserDefinedField = value;
                    this.RaisePropertyChanged("IsUserDefined");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Limit {
            get {
                return this.LimitField;
            }
            set {
                if ((this.LimitField.Equals(value) != true)) {
                    this.LimitField = value;
                    this.RaisePropertyChanged("Limit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Market {
            get {
                return this.MarketField;
            }
            set {
                if ((this.MarketField.Equals(value) != true)) {
                    this.MarketField = value;
                    this.RaisePropertyChanged("Market");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Period {
            get {
                return this.PeriodField;
            }
            set {
                if ((object.ReferenceEquals(this.PeriodField, value) != true)) {
                    this.PeriodField = value;
                    this.RaisePropertyChanged("Period");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Portfolio {
            get {
                return this.PortfolioField;
            }
            set {
                if ((object.ReferenceEquals(this.PortfolioField, value) != true)) {
                    this.PortfolioField = value;
                    this.RaisePropertyChanged("Portfolio");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Quantity {
            get {
                return this.QuantityField;
            }
            set {
                if ((this.QuantityField.Equals(value) != true)) {
                    this.QuantityField = value;
                    this.RaisePropertyChanged("Quantity");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SellScript {
            get {
                return this.SellScriptField;
            }
            set {
                if ((object.ReferenceEquals(this.SellScriptField, value) != true)) {
                    this.SellScriptField = value;
                    this.RaisePropertyChanged("SellScript");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool StopLimit {
            get {
                return this.StopLimitField;
            }
            set {
                if ((this.StopLimitField.Equals(value) != true)) {
                    this.StopLimitField = value;
                    this.RaisePropertyChanged("StopLimit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StopLimitValue {
            get {
                return this.StopLimitValueField;
            }
            set {
                if ((object.ReferenceEquals(this.StopLimitValueField, value) != true)) {
                    this.StopLimitValueField = value;
                    this.RaisePropertyChanged("StopLimitValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool StopMarket {
            get {
                return this.StopMarketField;
            }
            set {
                if ((this.StopMarketField.Equals(value) != true)) {
                    this.StopMarketField = value;
                    this.RaisePropertyChanged("StopMarket");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Symbol {
            get {
                return this.SymbolField;
            }
            set {
                if ((object.ReferenceEquals(this.SymbolField, value) != true)) {
                    this.SymbolField = value;
                    this.RaisePropertyChanged("Symbol");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TradeSignalScript {
            get {
                return this.TradeSignalScriptField;
            }
            set {
                if ((object.ReferenceEquals(this.TradeSignalScriptField, value) != true)) {
                    this.TradeSignalScriptField = value;
                    this.RaisePropertyChanged("TradeSignalScript");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid UniqueId {
            get {
                return this.UniqueIdField;
            }
            set {
                if ((this.UniqueIdField.Equals(value) != true)) {
                    this.UniqueIdField = value;
                    this.RaisePropertyChanged("UniqueId");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ActionResultOfboolean", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.ActionResults")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(FundXchange.Model.AlertService.GeneralResult))]
    public partial class ActionResultOfboolean : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorMessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ObjectField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FundXchange.Model.AlertService.ResultTypes ResultField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorMessage {
            get {
                return this.ErrorMessageField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorMessageField, value) != true)) {
                    this.ErrorMessageField = value;
                    this.RaisePropertyChanged("ErrorMessage");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Object {
            get {
                return this.ObjectField;
            }
            set {
                if ((this.ObjectField.Equals(value) != true)) {
                    this.ObjectField = value;
                    this.RaisePropertyChanged("Object");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FundXchange.Model.AlertService.ResultTypes Result {
            get {
                return this.ResultField;
            }
            set {
                if ((this.ResultField.Equals(value) != true)) {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GeneralResult", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.ActionResults")]
    [System.SerializableAttribute()]
    public partial class GeneralResult : FundXchange.Model.AlertService.ActionResultOfboolean {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ResultTypes", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.ActionResults")]
    public enum ResultTypes : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Success = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Warning = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Failure = 2,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ScannerDTO", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.State.DTOs")]
    [System.SerializableAttribute()]
    public partial class ScannerDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AlertScriptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int BarHistoryField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int BarIntervalField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExchangeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsLockedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsPausedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FundXchange.Model.AlertService.Periodicities PeriodicityField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ScannerNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SymbolField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AlertScript {
            get {
                return this.AlertScriptField;
            }
            set {
                if ((object.ReferenceEquals(this.AlertScriptField, value) != true)) {
                    this.AlertScriptField = value;
                    this.RaisePropertyChanged("AlertScript");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BarHistory {
            get {
                return this.BarHistoryField;
            }
            set {
                if ((this.BarHistoryField.Equals(value) != true)) {
                    this.BarHistoryField = value;
                    this.RaisePropertyChanged("BarHistory");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int BarInterval {
            get {
                return this.BarIntervalField;
            }
            set {
                if ((this.BarIntervalField.Equals(value) != true)) {
                    this.BarIntervalField = value;
                    this.RaisePropertyChanged("BarInterval");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Exchange {
            get {
                return this.ExchangeField;
            }
            set {
                if ((object.ReferenceEquals(this.ExchangeField, value) != true)) {
                    this.ExchangeField = value;
                    this.RaisePropertyChanged("Exchange");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsLocked {
            get {
                return this.IsLockedField;
            }
            set {
                if ((this.IsLockedField.Equals(value) != true)) {
                    this.IsLockedField = value;
                    this.RaisePropertyChanged("IsLocked");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsPaused {
            get {
                return this.IsPausedField;
            }
            set {
                if ((this.IsPausedField.Equals(value) != true)) {
                    this.IsPausedField = value;
                    this.RaisePropertyChanged("IsPaused");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FundXchange.Model.AlertService.Periodicities Periodicity {
            get {
                return this.PeriodicityField;
            }
            set {
                if ((this.PeriodicityField.Equals(value) != true)) {
                    this.PeriodicityField = value;
                    this.RaisePropertyChanged("Periodicity");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ScannerName {
            get {
                return this.ScannerNameField;
            }
            set {
                if ((object.ReferenceEquals(this.ScannerNameField, value) != true)) {
                    this.ScannerNameField = value;
                    this.RaisePropertyChanged("ScannerName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Symbol {
            get {
                return this.SymbolField;
            }
            set {
                if ((object.ReferenceEquals(this.SymbolField, value) != true)) {
                    this.SymbolField = value;
                    this.RaisePropertyChanged("Symbol");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Periodicities", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.State")]
    public enum Periodicities : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Secondly = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Minutely = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Hourly = 3,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Daily = 4,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Weekly = 5,
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AlertDTO", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.State.DTOs")]
    [System.SerializableAttribute()]
    public partial class AlertDTO : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string AlertNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FundXchange.Model.AlertService.AlertType AlertTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ExchangeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SymbolField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TagField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AlertName {
            get {
                return this.AlertNameField;
            }
            set {
                if ((object.ReferenceEquals(this.AlertNameField, value) != true)) {
                    this.AlertNameField = value;
                    this.RaisePropertyChanged("AlertName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public FundXchange.Model.AlertService.AlertType AlertType {
            get {
                return this.AlertTypeField;
            }
            set {
                if ((this.AlertTypeField.Equals(value) != true)) {
                    this.AlertTypeField = value;
                    this.RaisePropertyChanged("AlertType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Exchange {
            get {
                return this.ExchangeField;
            }
            set {
                if ((object.ReferenceEquals(this.ExchangeField, value) != true)) {
                    this.ExchangeField = value;
                    this.RaisePropertyChanged("Exchange");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Symbol {
            get {
                return this.SymbolField;
            }
            set {
                if ((object.ReferenceEquals(this.SymbolField, value) != true)) {
                    this.SymbolField = value;
                    this.RaisePropertyChanged("Symbol");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Tag {
            get {
                return this.TagField;
            }
            set {
                if ((object.ReferenceEquals(this.TagField, value) != true)) {
                    this.TagField = value;
                    this.RaisePropertyChanged("Tag");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AlertType", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.State.Enumerations")]
    public enum AlertType : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        News = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Script = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Order = 2,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AlertService.IAlertService")]
    public interface IAlertService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlertService/AddAlertScript", ReplyAction="http://tempuri.org/IAlertService/AddAlertScriptResponse")]
        FundXchange.Model.AlertService.GeneralResult AddAlertScript(int userId, FundXchange.Model.AlertService.AlertScriptDTO alertScriptDTO);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlertService/RemoveAlertScript", ReplyAction="http://tempuri.org/IAlertService/RemoveAlertScriptResponse")]
        FundXchange.Model.AlertService.GeneralResult RemoveAlertScript(int userId, string alertName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlertService/RemoveAlertScriptById", ReplyAction="http://tempuri.org/IAlertService/RemoveAlertScriptByIdResponse")]
        FundXchange.Model.AlertService.GeneralResult RemoveAlertScriptById(int userId, System.Guid alertId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlertService/SaveScanner", ReplyAction="http://tempuri.org/IAlertService/SaveScannerResponse")]
        FundXchange.Model.AlertService.GeneralResult SaveScanner(int userId, FundXchange.Model.AlertService.ScannerDTO scannerDTO);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlertService/RemoveScanner", ReplyAction="http://tempuri.org/IAlertService/RemoveScannerResponse")]
        FundXchange.Model.AlertService.GeneralResult RemoveScanner(int userId, string scannerName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlertService/AddAlert", ReplyAction="http://tempuri.org/IAlertService/AddAlertResponse")]
        FundXchange.Model.AlertService.GeneralResult AddAlert(int userId, FundXchange.Model.AlertService.AlertDTO alert);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAlertServiceChannel : FundXchange.Model.AlertService.IAlertService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AlertServiceClient : System.ServiceModel.ClientBase<FundXchange.Model.AlertService.IAlertService>, FundXchange.Model.AlertService.IAlertService {
        
        public AlertServiceClient() {
        }
        
        public AlertServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AlertServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AlertServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AlertServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public FundXchange.Model.AlertService.GeneralResult AddAlertScript(int userId, FundXchange.Model.AlertService.AlertScriptDTO alertScriptDTO) {
            return base.Channel.AddAlertScript(userId, alertScriptDTO);
        }
        
        public FundXchange.Model.AlertService.GeneralResult RemoveAlertScript(int userId, string alertName) {
            return base.Channel.RemoveAlertScript(userId, alertName);
        }
        
        public FundXchange.Model.AlertService.GeneralResult RemoveAlertScriptById(int userId, System.Guid alertId) {
            return base.Channel.RemoveAlertScriptById(userId, alertId);
        }
        
        public FundXchange.Model.AlertService.GeneralResult SaveScanner(int userId, FundXchange.Model.AlertService.ScannerDTO scannerDTO) {
            return base.Channel.SaveScanner(userId, scannerDTO);
        }
        
        public FundXchange.Model.AlertService.GeneralResult RemoveScanner(int userId, string scannerName) {
            return base.Channel.RemoveScanner(userId, scannerName);
        }
        
        public FundXchange.Model.AlertService.GeneralResult AddAlert(int userId, FundXchange.Model.AlertService.AlertDTO alert) {
            return base.Channel.AddAlert(userId, alert);
        }
    }
}