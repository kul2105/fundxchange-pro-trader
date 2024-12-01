﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FundXchange.Model.PortfolioService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ActionResultOfboolean", Namespace="http://schemas.datacontract.org/2004/07/FundXchange.Services.ActionResults")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(FundXchange.Model.PortfolioService.GeneralResult))]
    public partial class ActionResultOfboolean : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorMessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool ObjectField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private FundXchange.Model.PortfolioService.ResultTypes ResultField;
        
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
        public FundXchange.Model.PortfolioService.ResultTypes Result {
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
    public partial class GeneralResult : FundXchange.Model.PortfolioService.ActionResultOfboolean {
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PortfolioService.IPortfolioService")]
    public interface IPortfolioService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortfolioService/AddDataPortfolio", ReplyAction="http://tempuri.org/IPortfolioService/AddDataPortfolioResponse")]
        FundXchange.Model.PortfolioService.GeneralResult AddDataPortfolio(int userId, string portfolioName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortfolioService/RemoveDataPortfolio", ReplyAction="http://tempuri.org/IPortfolioService/RemoveDataPortfolioResponse")]
        FundXchange.Model.PortfolioService.GeneralResult RemoveDataPortfolio(int userId, string portfolioName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortfolioService/AddDataPortfolioInstrument", ReplyAction="http://tempuri.org/IPortfolioService/AddDataPortfolioInstrumentResponse")]
        FundXchange.Model.PortfolioService.GeneralResult AddDataPortfolioInstrument(int userId, string portfolioName, string exchange, string symbol);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPortfolioService/RemoveDataPortfolioInstrument", ReplyAction="http://tempuri.org/IPortfolioService/RemoveDataPortfolioInstrumentResponse")]
        FundXchange.Model.PortfolioService.GeneralResult RemoveDataPortfolioInstrument(int userId, string portfolioName, string exchange, string symbol);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPortfolioServiceChannel : FundXchange.Model.PortfolioService.IPortfolioService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PortfolioServiceClient : System.ServiceModel.ClientBase<FundXchange.Model.PortfolioService.IPortfolioService>, FundXchange.Model.PortfolioService.IPortfolioService {
        
        public PortfolioServiceClient() {
        }
        
        public PortfolioServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PortfolioServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PortfolioServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PortfolioServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public FundXchange.Model.PortfolioService.GeneralResult AddDataPortfolio(int userId, string portfolioName) {
            return base.Channel.AddDataPortfolio(userId, portfolioName);
        }
        
        public FundXchange.Model.PortfolioService.GeneralResult RemoveDataPortfolio(int userId, string portfolioName) {
            return base.Channel.RemoveDataPortfolio(userId, portfolioName);
        }
        
        public FundXchange.Model.PortfolioService.GeneralResult AddDataPortfolioInstrument(int userId, string portfolioName, string exchange, string symbol) {
            return base.Channel.AddDataPortfolioInstrument(userId, portfolioName, exchange, symbol);
        }
        
        public FundXchange.Model.PortfolioService.GeneralResult RemoveDataPortfolioInstrument(int userId, string portfolioName, string exchange, string symbol) {
            return base.Channel.RemoveDataPortfolioInstrument(userId, portfolioName, exchange, symbol);
        }
    }
}
