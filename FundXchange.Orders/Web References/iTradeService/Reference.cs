﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace FundXchange.Orders.iTradeService {
    using System.Diagnostics;
    using System;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System.Web.Services;
    using System.Data;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="FundXchangeSoap", Namespace="iTrade")]
    public partial class FundXchange : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetPortfolioOperationCompleted;
        
        private System.Threading.SendOrPostCallback VerifyUserOperationCompleted;
        
        private System.Threading.SendOrPostCallback CancelOrderOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetOrdersOperationCompleted;
        
        private System.Threading.SendOrPostCallback PlaceOrderOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public FundXchange() {
            this.Url = global::FundXchange.Orders.Properties.Settings.Default.FundXchange_Orders_iTradeService_FundXchange;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetPortfolioCompletedEventHandler GetPortfolioCompleted;
        
        /// <remarks/>
        public event VerifyUserCompletedEventHandler VerifyUserCompleted;
        
        /// <remarks/>
        public event CancelOrderCompletedEventHandler CancelOrderCompleted;
        
        /// <remarks/>
        public event GetOrdersCompletedEventHandler GetOrdersCompleted;
        
        /// <remarks/>
        public event PlaceOrderCompletedEventHandler PlaceOrderCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("iTrade/GetPortfolio", RequestNamespace="iTrade", ResponseNamespace="iTrade", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataTable GetPortfolio(string sBDA) {
            object[] results = this.Invoke("GetPortfolio", new object[] {
                        sBDA});
            return ((System.Data.DataTable)(results[0]));
        }
        
        /// <remarks/>
        public void GetPortfolioAsync(string sBDA) {
            this.GetPortfolioAsync(sBDA, null);
        }
        
        /// <remarks/>
        public void GetPortfolioAsync(string sBDA, object userState) {
            if ((this.GetPortfolioOperationCompleted == null)) {
                this.GetPortfolioOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetPortfolioOperationCompleted);
            }
            this.InvokeAsync("GetPortfolio", new object[] {
                        sBDA}, this.GetPortfolioOperationCompleted, userState);
        }
        
        private void OnGetPortfolioOperationCompleted(object arg) {
            if ((this.GetPortfolioCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetPortfolioCompleted(this, new GetPortfolioCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("iTrade/VerifyUser", RequestNamespace="iTrade", ResponseNamespace="iTrade", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataTable VerifyUser(string sUsername, string sPassword) {
            object[] results = this.Invoke("VerifyUser", new object[] {
                        sUsername,
                        sPassword});
            return ((System.Data.DataTable)(results[0]));
        }
        
        /// <remarks/>
        public void VerifyUserAsync(string sUsername, string sPassword) {
            this.VerifyUserAsync(sUsername, sPassword, null);
        }
        
        /// <remarks/>
        public void VerifyUserAsync(string sUsername, string sPassword, object userState) {
            if ((this.VerifyUserOperationCompleted == null)) {
                this.VerifyUserOperationCompleted = new System.Threading.SendOrPostCallback(this.OnVerifyUserOperationCompleted);
            }
            this.InvokeAsync("VerifyUser", new object[] {
                        sUsername,
                        sPassword}, this.VerifyUserOperationCompleted, userState);
        }
        
        private void OnVerifyUserOperationCompleted(object arg) {
            if ((this.VerifyUserCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.VerifyUserCompleted(this, new VerifyUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("iTrade/CancelOrder", RequestNamespace="iTrade", ResponseNamespace="iTrade", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataTable CancelOrder(string sBDA, string sExternalReference) {
            object[] results = this.Invoke("CancelOrder", new object[] {
                        sBDA,
                        sExternalReference});
            return ((System.Data.DataTable)(results[0]));
        }
        
        /// <remarks/>
        public void CancelOrderAsync(string sBDA, string sExternalReference) {
            this.CancelOrderAsync(sBDA, sExternalReference, null);
        }
        
        /// <remarks/>
        public void CancelOrderAsync(string sBDA, string sExternalReference, object userState) {
            if ((this.CancelOrderOperationCompleted == null)) {
                this.CancelOrderOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCancelOrderOperationCompleted);
            }
            this.InvokeAsync("CancelOrder", new object[] {
                        sBDA,
                        sExternalReference}, this.CancelOrderOperationCompleted, userState);
        }
        
        private void OnCancelOrderOperationCompleted(object arg) {
            if ((this.CancelOrderCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CancelOrderCompleted(this, new CancelOrderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("iTrade/GetOrders", RequestNamespace="iTrade", ResponseNamespace="iTrade", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataTable GetOrders(string sBDA) {
            object[] results = this.Invoke("GetOrders", new object[] {
                        sBDA});
            return ((System.Data.DataTable)(results[0]));
        }
        
        /// <remarks/>
        public void GetOrdersAsync(string sBDA) {
            this.GetOrdersAsync(sBDA, null);
        }
        
        /// <remarks/>
        public void GetOrdersAsync(string sBDA, object userState) {
            if ((this.GetOrdersOperationCompleted == null)) {
                this.GetOrdersOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetOrdersOperationCompleted);
            }
            this.InvokeAsync("GetOrders", new object[] {
                        sBDA}, this.GetOrdersOperationCompleted, userState);
        }
        
        private void OnGetOrdersOperationCompleted(object arg) {
            if ((this.GetOrdersCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetOrdersCompleted(this, new GetOrdersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("iTrade/PlaceOrder", RequestNamespace="iTrade", ResponseNamespace="iTrade", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public System.Data.DataTable PlaceOrder(string sUsername, string sBDA, string sSymbol, string sSide, double Price, int Volume, System.DateTime ExpiryDate) {
            object[] results = this.Invoke("PlaceOrder", new object[] {
                        sUsername,
                        sBDA,
                        sSymbol,
                        sSide,
                        Price,
                        Volume,
                        ExpiryDate});
            return ((System.Data.DataTable)(results[0]));
        }
        
        /// <remarks/>
        public void PlaceOrderAsync(string sUsername, string sBDA, string sSymbol, string sSide, double Price, int Volume, System.DateTime ExpiryDate) {
            this.PlaceOrderAsync(sUsername, sBDA, sSymbol, sSide, Price, Volume, ExpiryDate, null);
        }
        
        /// <remarks/>
        public void PlaceOrderAsync(string sUsername, string sBDA, string sSymbol, string sSide, double Price, int Volume, System.DateTime ExpiryDate, object userState) {
            if ((this.PlaceOrderOperationCompleted == null)) {
                this.PlaceOrderOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPlaceOrderOperationCompleted);
            }
            this.InvokeAsync("PlaceOrder", new object[] {
                        sUsername,
                        sBDA,
                        sSymbol,
                        sSide,
                        Price,
                        Volume,
                        ExpiryDate}, this.PlaceOrderOperationCompleted, userState);
        }
        
        private void OnPlaceOrderOperationCompleted(object arg) {
            if ((this.PlaceOrderCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PlaceOrderCompleted(this, new PlaceOrderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void GetPortfolioCompletedEventHandler(object sender, GetPortfolioCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetPortfolioCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetPortfolioCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataTable Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataTable)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void VerifyUserCompletedEventHandler(object sender, VerifyUserCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class VerifyUserCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal VerifyUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataTable Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataTable)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void CancelOrderCompletedEventHandler(object sender, CancelOrderCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CancelOrderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CancelOrderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataTable Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataTable)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void GetOrdersCompletedEventHandler(object sender, GetOrdersCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetOrdersCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetOrdersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataTable Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataTable)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    public delegate void PlaceOrderCompletedEventHandler(object sender, PlaceOrderCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4161.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PlaceOrderCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PlaceOrderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public System.Data.DataTable Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.Data.DataTable)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591