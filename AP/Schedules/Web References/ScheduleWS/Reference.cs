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

namespace Schedules.ScheduleWS {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ScheduleServiceSoap", Namespace="http://sp8888.net/webservice")]
    public partial class ScheduleService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CheckScheduleExistsOperationCompleted;
        
        private System.Threading.SendOrPostCallback AddScheduleBatchOperationCompleted;
        
        private System.Threading.SendOrPostCallback AddScheduleOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public ScheduleService() {
            this.Url = global::Schedules.Properties.Settings.Default.Schedules_ScheduleWS_ScheduleService;
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
        public event CheckScheduleExistsCompletedEventHandler CheckScheduleExistsCompleted;
        
        /// <remarks/>
        public event AddScheduleBatchCompletedEventHandler AddScheduleBatchCompleted;
        
        /// <remarks/>
        public event AddScheduleCompletedEventHandler AddScheduleCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/CheckScheduleExists", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteResult CheckScheduleExists(string key, Schedule schedule) {
            object[] results = this.Invoke("CheckScheduleExists", new object[] {
                        key,
                        schedule});
            return ((ExecuteResult)(results[0]));
        }
        
        /// <remarks/>
        public void CheckScheduleExistsAsync(string key, Schedule schedule) {
            this.CheckScheduleExistsAsync(key, schedule, null);
        }
        
        /// <remarks/>
        public void CheckScheduleExistsAsync(string key, Schedule schedule, object userState) {
            if ((this.CheckScheduleExistsOperationCompleted == null)) {
                this.CheckScheduleExistsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCheckScheduleExistsOperationCompleted);
            }
            this.InvokeAsync("CheckScheduleExists", new object[] {
                        key,
                        schedule}, this.CheckScheduleExistsOperationCompleted, userState);
        }
        
        private void OnCheckScheduleExistsOperationCompleted(object arg) {
            if ((this.CheckScheduleExistsCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CheckScheduleExistsCompleted(this, new CheckScheduleExistsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/AddScheduleBatch", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteDataChangeResult AddScheduleBatch(string key, Schedule[] scheduleList) {
            object[] results = this.Invoke("AddScheduleBatch", new object[] {
                        key,
                        scheduleList});
            return ((ExecuteDataChangeResult)(results[0]));
        }
        
        /// <remarks/>
        public void AddScheduleBatchAsync(string key, Schedule[] scheduleList) {
            this.AddScheduleBatchAsync(key, scheduleList, null);
        }
        
        /// <remarks/>
        public void AddScheduleBatchAsync(string key, Schedule[] scheduleList, object userState) {
            if ((this.AddScheduleBatchOperationCompleted == null)) {
                this.AddScheduleBatchOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddScheduleBatchOperationCompleted);
            }
            this.InvokeAsync("AddScheduleBatch", new object[] {
                        key,
                        scheduleList}, this.AddScheduleBatchOperationCompleted, userState);
        }
        
        private void OnAddScheduleBatchOperationCompleted(object arg) {
            if ((this.AddScheduleBatchCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddScheduleBatchCompleted(this, new AddScheduleBatchCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://sp8888.net/webservice/AddSchedule", RequestNamespace="http://sp8888.net/webservice", ResponseNamespace="http://sp8888.net/webservice", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ExecuteResult AddSchedule(string key, Schedule schedule) {
            object[] results = this.Invoke("AddSchedule", new object[] {
                        key,
                        schedule});
            return ((ExecuteResult)(results[0]));
        }
        
        /// <remarks/>
        public void AddScheduleAsync(string key, Schedule schedule) {
            this.AddScheduleAsync(key, schedule, null);
        }
        
        /// <remarks/>
        public void AddScheduleAsync(string key, Schedule schedule, object userState) {
            if ((this.AddScheduleOperationCompleted == null)) {
                this.AddScheduleOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAddScheduleOperationCompleted);
            }
            this.InvokeAsync("AddSchedule", new object[] {
                        key,
                        schedule}, this.AddScheduleOperationCompleted, userState);
        }
        
        private void OnAddScheduleOperationCompleted(object arg) {
            if ((this.AddScheduleCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AddScheduleCompleted(this, new AddScheduleCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    public partial class Schedule {
        
        private System.Nullable<int> idField;
        
        private string gameTypeField;
        
        private System.Nullable<int> orderByField;
        
        private System.Nullable<int> allianceIDField;
        
        private System.DateTime fullGameTimeField;
        
        private System.Nullable<int> numberField;
        
        private string gameStatesField;
        
        private System.Nullable<int> teamAIDField;
        
        private System.Nullable<int> teamBIDField;
        
        private System.Nullable<int> controlStatesField;
        
        private string trackerTextField;
        
        private bool isDeletedField;
        
        private string webIDField;
        
        private System.Nullable<bool> isRescheduleField;
        
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
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> OrderBy {
            get {
                return this.orderByField;
            }
            set {
                this.orderByField = value;
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
        public System.DateTime FullGameTime {
            get {
                return this.fullGameTimeField;
            }
            set {
                this.fullGameTimeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> Number {
            get {
                return this.numberField;
            }
            set {
                this.numberField = value;
            }
        }
        
        /// <remarks/>
        public string GameStates {
            get {
                return this.gameStatesField;
            }
            set {
                this.gameStatesField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> TeamAID {
            get {
                return this.teamAIDField;
            }
            set {
                this.teamAIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> TeamBID {
            get {
                return this.teamBIDField;
            }
            set {
                this.teamBIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> ControlStates {
            get {
                return this.controlStatesField;
            }
            set {
                this.controlStatesField = value;
            }
        }
        
        /// <remarks/>
        public string TrackerText {
            get {
                return this.trackerTextField;
            }
            set {
                this.trackerTextField = value;
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
        public string WebID {
            get {
                return this.webIDField;
            }
            set {
                this.webIDField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<bool> IsReschedule {
            get {
                return this.isRescheduleField;
            }
            set {
                this.isRescheduleField = value;
            }
        }
    }
    
    /// <remarks/>
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
    public delegate void CheckScheduleExistsCompletedEventHandler(object sender, CheckScheduleExistsCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CheckScheduleExistsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CheckScheduleExistsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public delegate void AddScheduleBatchCompletedEventHandler(object sender, AddScheduleBatchCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AddScheduleBatchCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AddScheduleBatchCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public delegate void AddScheduleCompletedEventHandler(object sender, AddScheduleCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AddScheduleCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AddScheduleCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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