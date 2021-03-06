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

namespace Schedules.TeamWS {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="TeamServiceSoap", Namespace="http://sp8888.net/webservice")]
    public partial class TeamService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CheckTeamExistsOperationCompleted;
        
        private System.Threading.SendOrPostCallback AddTeamBatchOperationCompleted;
        
        private System.Threading.SendOrPostCallback AddTeamOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetTeamByUnionAllOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public TeamService() {
            this.Url = global::Schedules.Properties.Settings.Default.Schedules_TeamWS_TeamService;
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
        public event CheckTeamExistsCompletedEventHandler CheckTeamExistsCompleted;
        
        /// <remarks/>
        public event AddTeamBatchCompletedEventHandler AddTeamBatchCompleted;
        
        /// <remarks/>
        public event AddTeamCompletedEventHandler AddTeamCompleted;
        
        /// <remarks/>
        public event GetTeamByUnionAllCompletedEventHandler GetTeamByUnionAllCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/CheckTeamExists", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteResult CheckTeamExists(string key, Team team) {
            object[] results = this.Invoke("CheckTeamExists", new object[] {
                        key,
                        team});
            return ((ExecuteResult)(results[0]));
        }
        
        /// <remarks/>
        public void CheckTeamExistsAsync(string key, Team team) {
            this.CheckTeamExistsAsync(key, team, null);
        }
        
        /// <remarks/>
        public void CheckTeamExistsAsync(string key, Team team, object userState) {
            if ((this.CheckTeamExistsOperationCompleted == null)) {
                this.CheckTeamExistsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCheckTeamExistsOperationCompleted);
            }
            this.InvokeAsync("CheckTeamExists", new object[] {
                        key,
                        team}, this.CheckTeamExistsOperationCompleted, userState);
        }
        
        private void OnCheckTeamExistsOperationCompleted(object arg) {
            if ((this.CheckTeamExistsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CheckTeamExistsCompleted(this, new CheckTeamExistsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/AddTeamBatch", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteDataChangeResult AddTeamBatch(string key, Team[] teamList) {
            object[] results = this.Invoke("AddTeamBatch", new object[] {
                        key,
                        teamList});
            return ((ExecuteDataChangeResult)(results[0]));
        }
        
        /// <remarks/>
        public void AddTeamBatchAsync(string key, Team[] teamList) {
            this.AddTeamBatchAsync(key, teamList, null);
        }
        
        /// <remarks/>
        public void AddTeamBatchAsync(string key, Team[] teamList, object userState) {
            if ((this.AddTeamBatchOperationCompleted == null)) {
                this.AddTeamBatchOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddTeamBatchOperationCompleted);
            }
            this.InvokeAsync("AddTeamBatch", new object[] {
                        key,
                        teamList}, this.AddTeamBatchOperationCompleted, userState);
        }
        
        private void OnAddTeamBatchOperationCompleted(object arg) {
            if ((this.AddTeamBatchCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddTeamBatchCompleted(this, new AddTeamBatchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/AddTeam", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteResult AddTeam(string key, Team team) {
            object[] results = this.Invoke("AddTeam", new object[] {
                        key,
                        team});
            return ((ExecuteResult)(results[0]));
        }
        
        /// <remarks/>
        public void AddTeamAsync(string key, Team team) {
            this.AddTeamAsync(key, team, null);
        }
        
        /// <remarks/>
        public void AddTeamAsync(string key, Team team, object userState) {
            if ((this.AddTeamOperationCompleted == null)) {
                this.AddTeamOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddTeamOperationCompleted);
            }
            this.InvokeAsync("AddTeam", new object[] {
                        key,
                        team}, this.AddTeamOperationCompleted, userState);
        }
        
        private void OnAddTeamOperationCompleted(object arg) {
            if ((this.AddTeamCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddTeamCompleted(this, new AddTeamCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/GetTeamByUnionAll", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteDataResult GetTeamByUnionAll(string key) {
            object[] results = this.Invoke("GetTeamByUnionAll", new object[] {
                        key});
            return ((ExecuteDataResult)(results[0]));
        }
        
        /// <remarks/>
        public void GetTeamByUnionAllAsync(string key) {
            this.GetTeamByUnionAllAsync(key, null);
        }
        
        /// <remarks/>
        public void GetTeamByUnionAllAsync(string key, object userState) {
            if ((this.GetTeamByUnionAllOperationCompleted == null)) {
                this.GetTeamByUnionAllOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetTeamByUnionAllOperationCompleted);
            }
            this.InvokeAsync("GetTeamByUnionAll", new object[] {
                        key}, this.GetTeamByUnionAllOperationCompleted, userState);
        }
        
        private void OnGetTeamByUnionAllOperationCompleted(object arg) {
            if ((this.GetTeamByUnionAllCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetTeamByUnionAllCompleted(this, new GetTeamByUnionAllCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sp8888.net/webservice")]
    public partial class Team {
        
        private System.Nullable<int> idField;
        
        private string gameTypeField;
        
        private string teamNameField;
        
        private string showNameField;
        
        private System.Nullable<int> allianceIDField;
        
        private string webNameField;
        
        private System.Nullable<int> wField;
        
        private System.Nullable<int> lField;
        
        private System.Nullable<int> tField;
        
        private bool isDeletedField;
        
        private string sourceIDField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> ID {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        public string GameType {
            get {
                return this.gameTypeField;
            }
            set {
                this.gameTypeField = value;
            }
        }
        
        /// <remarks/>
        public string TeamName {
            get {
                return this.teamNameField;
            }
            set {
                this.teamNameField = value;
            }
        }
        
        /// <remarks/>
        public string ShowName {
            get {
                return this.showNameField;
            }
            set {
                this.showNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> AllianceID {
            get {
                return this.allianceIDField;
            }
            set {
                this.allianceIDField = value;
            }
        }
        
        /// <remarks/>
        public string WebName {
            get {
                return this.webNameField;
            }
            set {
                this.webNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> W {
            get {
                return this.wField;
            }
            set {
                this.wField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> L {
            get {
                return this.lField;
            }
            set {
                this.lField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> T {
            get {
                return this.tField;
            }
            set {
                this.tField = value;
            }
        }
        
        /// <remarks/>
        public bool IsDeleted {
            get {
                return this.isDeletedField;
            }
            set {
                this.isDeletedField = value;
            }
        }
        
        /// <remarks/>
        public string SourceID {
            get {
                return this.sourceIDField;
            }
            set {
                this.sourceIDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ExecuteDataResult))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ExecuteDataChangeResult))]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.18408")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://sp8888.net/webservice")]
    public partial class ExecuteDataChangeResult : ExecuteResult {
        
        private int changeCountField;
        
        /// <remarks/>
        public int ChangeCount {
            get {
                return this.changeCountField;
            }
            set {
                this.changeCountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void CheckTeamExistsCompletedEventHandler(object sender, CheckTeamExistsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CheckTeamExistsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CheckTeamExistsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public delegate void AddTeamBatchCompletedEventHandler(object sender, AddTeamBatchCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AddTeamBatchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AddTeamBatchCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ExecuteDataChangeResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ExecuteDataChangeResult)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    public delegate void AddTeamCompletedEventHandler(object sender, AddTeamCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AddTeamCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AddTeamCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public delegate void GetTeamByUnionAllCompletedEventHandler(object sender, GetTeamByUnionAllCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetTeamByUnionAllCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetTeamByUnionAllCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
}

#pragma warning restore 1591