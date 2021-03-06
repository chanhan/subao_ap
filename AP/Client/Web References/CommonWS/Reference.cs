﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.18444
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 原始程式碼已由 Microsoft.VSDesigner 自動產生，版本 4.0.30319.18444。
// 
#pragma warning disable 1591

namespace Client.CommonWS {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Data;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="CommonServiceSoap", Namespace="http://sp8888.net/webservice")]
    public partial class CommonService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetValueFromSetTypeValOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetGameSourceBySourceIDOperationCompleted;
        
        private System.Threading.SendOrPostCallback LoginCheckOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public CommonService() {
            this.Url = global::Client.Properties.Settings.Default.Client_CommonWS_CommonService;
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
        public event GetValueFromSetTypeValCompletedEventHandler GetValueFromSetTypeValCompleted;
        
        /// <remarks/>
        public event GetGameSourceBySourceIDCompletedEventHandler GetGameSourceBySourceIDCompleted;
        
        /// <remarks/>
        public event LoginCheckCompletedEventHandler LoginCheckCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/GetValueFromSetTypeVal", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteResult GetValueFromSetTypeVal(string key, string type, string language) {
            object[] results = this.Invoke("GetValueFromSetTypeVal", new object[] {
                        key,
                        type,
                        language});
            return ((ExecuteResult)(results[0]));
        }
        
        /// <remarks/>
        public void GetValueFromSetTypeValAsync(string key, string type, string language) {
            this.GetValueFromSetTypeValAsync(key, type, language, null);
        }
        
        /// <remarks/>
        public void GetValueFromSetTypeValAsync(string key, string type, string language, object userState) {
            if ((this.GetValueFromSetTypeValOperationCompleted == null)) {
                this.GetValueFromSetTypeValOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetValueFromSetTypeValOperationCompleted);
            }
            this.InvokeAsync("GetValueFromSetTypeVal", new object[] {
                        key,
                        type,
                        language}, this.GetValueFromSetTypeValOperationCompleted, userState);
        }
        
        private void OnGetValueFromSetTypeValOperationCompleted(object arg) {
            if ((this.GetValueFromSetTypeValCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetValueFromSetTypeValCompleted(this, new GetValueFromSetTypeValCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/GetGameSourceBySourceID", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteDataResult GetGameSourceBySourceID(string key, string sourceID) {
            object[] results = this.Invoke("GetGameSourceBySourceID", new object[] {
                        key,
                        sourceID});
            return ((ExecuteDataResult)(results[0]));
        }
        
        /// <remarks/>
        public void GetGameSourceBySourceIDAsync(string key, string sourceID) {
            this.GetGameSourceBySourceIDAsync(key, sourceID, null);
        }
        
        /// <remarks/>
        public void GetGameSourceBySourceIDAsync(string key, string sourceID, object userState) {
            if ((this.GetGameSourceBySourceIDOperationCompleted == null)) {
                this.GetGameSourceBySourceIDOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetGameSourceBySourceIDOperationCompleted);
            }
            this.InvokeAsync("GetGameSourceBySourceID", new object[] {
                        key,
                        sourceID}, this.GetGameSourceBySourceIDOperationCompleted, userState);
        }
        
        private void OnGetGameSourceBySourceIDOperationCompleted(object arg) {
            if ((this.GetGameSourceBySourceIDCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetGameSourceBySourceIDCompleted(this, new GetGameSourceBySourceIDCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/LoginCheck", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteResult LoginCheck(string key) {
            object[] results = this.Invoke("LoginCheck", new object[] {
                        key});
            return ((ExecuteResult)(results[0]));
        }
        
        /// <remarks/>
        public void LoginCheckAsync(string key) {
            this.LoginCheckAsync(key, null);
        }
        
        /// <remarks/>
        public void LoginCheckAsync(string key, object userState) {
            if ((this.LoginCheckOperationCompleted == null)) {
                this.LoginCheckOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLoginCheckOperationCompleted);
            }
            this.InvokeAsync("LoginCheck", new object[] {
                        key}, this.LoginCheckOperationCompleted, userState);
        }
        
        private void OnLoginCheckOperationCompleted(object arg) {
            if ((this.LoginCheckCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.LoginCheckCompleted(this, new LoginCheckCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ExecuteDataResult))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sp8888.net/webservice")]
    public partial class ExecuteResult {
        
        private int resultTypeField;
        
        private string resultMessageField;
        
        /// <remarks/>
        public int ResultType {
            get {
                return this.resultTypeField;
            }
            set {
                this.resultTypeField = value;
            }
        }
        
        /// <remarks/>
        public string ResultMessage {
            get {
                return this.resultMessageField;
            }
            set {
                this.resultMessageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sp8888.net/webservice")]
    public partial class ExecuteDataResult : ExecuteResult {
        
        private System.Data.DataSet resultDsField;
        
        /// <remarks/>
        public System.Data.DataSet ResultDs {
            get {
                return this.resultDsField;
            }
            set {
                this.resultDsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void GetValueFromSetTypeValCompletedEventHandler(object sender, GetValueFromSetTypeValCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetValueFromSetTypeValCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetValueFromSetTypeValCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ExecuteResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ExecuteResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void GetGameSourceBySourceIDCompletedEventHandler(object sender, GetGameSourceBySourceIDCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetGameSourceBySourceIDCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetGameSourceBySourceIDCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ExecuteDataResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ExecuteDataResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void LoginCheckCompletedEventHandler(object sender, LoginCheckCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LoginCheckCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal LoginCheckCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ExecuteResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ExecuteResult)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591