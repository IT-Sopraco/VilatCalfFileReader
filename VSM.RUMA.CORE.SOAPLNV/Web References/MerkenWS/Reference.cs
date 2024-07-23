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

namespace VSM.RUMA.CORE.SOAPLNV.MerkenWS {
    using System.Diagnostics;
    using System;
    using System.Xml.Serialization;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System.Web.Services;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="MerkenWSSoapBinding", Namespace="http://www.ienr.org/schemas/services/merkenWS_v2_0")]
    public partial class MerkenServiceService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback controleerLevensnummerOperationCompleted;
        
        private System.Threading.SendOrPostCallback raadplegenMerktypenOperationCompleted;
        
        private System.Threading.SendOrPostCallback raadplegenMerkenOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public MerkenServiceService() {
            this.Url = global::VSM.RUMA.CORE.SOAPLNV.Properties.Settings.Default.VSM_RUMA_CORE_SOAPLNV_MerkenWS_MerkenServiceService;
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
        public event controleerLevensnummerCompletedEventHandler controleerLevensnummerCompleted;
        
        /// <remarks/>
        public event raadplegenMerktypenCompletedEventHandler raadplegenMerktypenCompleted;
        
        /// <remarks/>
        public event raadplegenMerkenCompletedEventHandler raadplegenMerkenCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="", ResponseNamespace="", Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        [return: System.Xml.Serialization.XmlElementAttribute("controleerLevensnummerReturn")]
        public controleerLevensnummerResponseType controleerLevensnummer(controleerLevensnummerRequestType request) {
            object[] results = this.Invoke("controleerLevensnummer", new object[] {
                        request});
            return ((controleerLevensnummerResponseType)(results[0]));
        }
        
        /// <remarks/>
        public void controleerLevensnummerAsync(controleerLevensnummerRequestType request) {
            this.controleerLevensnummerAsync(request, null);
        }
        
        /// <remarks/>
        public void controleerLevensnummerAsync(controleerLevensnummerRequestType request, object userState) {
            if ((this.controleerLevensnummerOperationCompleted == null)) {
                this.controleerLevensnummerOperationCompleted = new System.Threading.SendOrPostCallback(this.OncontroleerLevensnummerOperationCompleted);
            }
            this.InvokeAsync("controleerLevensnummer", new object[] {
                        request}, this.controleerLevensnummerOperationCompleted, userState);
        }
        
        private void OncontroleerLevensnummerOperationCompleted(object arg) {
            if ((this.controleerLevensnummerCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.controleerLevensnummerCompleted(this, new controleerLevensnummerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="", ResponseNamespace="", Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        [return: System.Xml.Serialization.XmlElementAttribute("raadplegenMerktypenReturn")]
        public merktypenResponseType raadplegenMerktypen(merktypenRequestType request) {
            object[] results = this.Invoke("raadplegenMerktypen", new object[] {
                        request});
            return ((merktypenResponseType)(results[0]));
        }
        
        /// <remarks/>
        public void raadplegenMerktypenAsync(merktypenRequestType request) {
            this.raadplegenMerktypenAsync(request, null);
        }
        
        /// <remarks/>
        public void raadplegenMerktypenAsync(merktypenRequestType request, object userState) {
            if ((this.raadplegenMerktypenOperationCompleted == null)) {
                this.raadplegenMerktypenOperationCompleted = new System.Threading.SendOrPostCallback(this.OnraadplegenMerktypenOperationCompleted);
            }
            this.InvokeAsync("raadplegenMerktypen", new object[] {
                        request}, this.raadplegenMerktypenOperationCompleted, userState);
        }
        
        private void OnraadplegenMerktypenOperationCompleted(object arg) {
            if ((this.raadplegenMerktypenCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.raadplegenMerktypenCompleted(this, new raadplegenMerktypenCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("", RequestNamespace="", ResponseNamespace="", Use=System.Web.Services.Description.SoapBindingUse.Literal)]
        [return: System.Xml.Serialization.XmlElementAttribute("raadplegenMerkenReturn")]
        public merkenResponseType raadplegenMerken(merkenRequestType request) {
            object[] results = this.Invoke("raadplegenMerken", new object[] {
                        request});
            return ((merkenResponseType)(results[0]));
        }
        
        /// <remarks/>
        public void raadplegenMerkenAsync(merkenRequestType request) {
            this.raadplegenMerkenAsync(request, null);
        }
        
        /// <remarks/>
        public void raadplegenMerkenAsync(merkenRequestType request, object userState) {
            if ((this.raadplegenMerkenOperationCompleted == null)) {
                this.raadplegenMerkenOperationCompleted = new System.Threading.SendOrPostCallback(this.OnraadplegenMerkenOperationCompleted);
            }
            this.InvokeAsync("raadplegenMerken", new object[] {
                        request}, this.raadplegenMerkenOperationCompleted, userState);
        }
        
        private void OnraadplegenMerkenOperationCompleted(object arg) {
            if ((this.raadplegenMerkenCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.raadplegenMerkenCompleted(this, new raadplegenMerkenCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(controleerLevensnummerResponseType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/merken_v2_0")]
    public partial class controleerLevensnummerRequestType {
        
        private string requestIDField;
        
        private string dierLandcode1Field;
        
        private string dierLandcode2Field;
        
        private string dierLevensnummer1Field;
        
        private string dierLevensnummer2Field;
        
        private int dierSoortField;
        
        /// <remarks/>
        public string requestID {
            get {
                return this.requestIDField;
            }
            set {
                this.requestIDField = value;
            }
        }
        
        /// <remarks/>
        public string dierLandcode1 {
            get {
                return this.dierLandcode1Field;
            }
            set {
                this.dierLandcode1Field = value;
            }
        }
        
        /// <remarks/>
        public string dierLandcode2 {
            get {
                return this.dierLandcode2Field;
            }
            set {
                this.dierLandcode2Field = value;
            }
        }
        
        /// <remarks/>
        public string dierLevensnummer1 {
            get {
                return this.dierLevensnummer1Field;
            }
            set {
                this.dierLevensnummer1Field = value;
            }
        }
        
        /// <remarks/>
        public string dierLevensnummer2 {
            get {
                return this.dierLevensnummer2Field;
            }
            set {
                this.dierLevensnummer2Field = value;
            }
        }
        
        /// <remarks/>
        public int dierSoort {
            get {
                return this.dierSoortField;
            }
            set {
                this.dierSoortField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/merken_v2_0")]
    public partial class merkType {
        
        private string codeFabrikantField;
        
        private string codeSoortMerkField;
        
        private string codeLeverancierField;
        
        private string naamLeverancierField;
        
        private string codeMerktypeField;
        
        private string codeVormOormerkField;
        
        private string omschrijvingMerktypeField;
        
        private string datumBestellingField;
        
        private string dierLandcodeField;
        
        private string dierLevensnummerField;
        
        private string dierWerknummerField;
        
        private System.Nullable<int> merkVersienummerField;
        
        private string ubnOpMerkField;
        
        private System.Nullable<int> groepsmerkAantalField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeFabrikant {
            get {
                return this.codeFabrikantField;
            }
            set {
                this.codeFabrikantField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeSoortMerk {
            get {
                return this.codeSoortMerkField;
            }
            set {
                this.codeSoortMerkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeLeverancier {
            get {
                return this.codeLeverancierField;
            }
            set {
                this.codeLeverancierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string naamLeverancier {
            get {
                return this.naamLeverancierField;
            }
            set {
                this.naamLeverancierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeMerktype {
            get {
                return this.codeMerktypeField;
            }
            set {
                this.codeMerktypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeVormOormerk {
            get {
                return this.codeVormOormerkField;
            }
            set {
                this.codeVormOormerkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string omschrijvingMerktype {
            get {
                return this.omschrijvingMerktypeField;
            }
            set {
                this.omschrijvingMerktypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string datumBestelling {
            get {
                return this.datumBestellingField;
            }
            set {
                this.datumBestellingField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string dierLandcode {
            get {
                return this.dierLandcodeField;
            }
            set {
                this.dierLandcodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string dierLevensnummer {
            get {
                return this.dierLevensnummerField;
            }
            set {
                this.dierLevensnummerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string dierWerknummer {
            get {
                return this.dierWerknummerField;
            }
            set {
                this.dierWerknummerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> merkVersienummer {
            get {
                return this.merkVersienummerField;
            }
            set {
                this.merkVersienummerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string ubnOpMerk {
            get {
                return this.ubnOpMerkField;
            }
            set {
                this.ubnOpMerkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> groepsmerkAantal {
            get {
                return this.groepsmerkAantalField;
            }
            set {
                this.groepsmerkAantalField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(merkenResponseType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/merken_v2_0")]
    public partial class merkenRequestType {
        
        private string requestIDField;
        
        private string selRelatienummerHouderField;
        
        private int selDierSoortField;
        
        private string selIndVrijBesteldField;
        
        private string selDierLevensnummerField;
        
        private string selDierLevensnummerHoogField;
        
        private string selDierWerknummerField;
        
        private string selDierWerknummerHoogField;
        
        private string selDierLandcodeField;
        
        private string selCodeMerktypeField;
        
        private string selCodeSoortMerkField;
        
        private System.Nullable<int> aantalField;
        
        /// <remarks/>
        public string requestID {
            get {
                return this.requestIDField;
            }
            set {
                this.requestIDField = value;
            }
        }
        
        /// <remarks/>
        public string selRelatienummerHouder {
            get {
                return this.selRelatienummerHouderField;
            }
            set {
                this.selRelatienummerHouderField = value;
            }
        }
        
        /// <remarks/>
        public int selDierSoort {
            get {
                return this.selDierSoortField;
            }
            set {
                this.selDierSoortField = value;
            }
        }
        
        /// <remarks/>
        public string selIndVrijBesteld {
            get {
                return this.selIndVrijBesteldField;
            }
            set {
                this.selIndVrijBesteldField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string selDierLevensnummer {
            get {
                return this.selDierLevensnummerField;
            }
            set {
                this.selDierLevensnummerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string selDierLevensnummerHoog {
            get {
                return this.selDierLevensnummerHoogField;
            }
            set {
                this.selDierLevensnummerHoogField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string selDierWerknummer {
            get {
                return this.selDierWerknummerField;
            }
            set {
                this.selDierWerknummerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string selDierWerknummerHoog {
            get {
                return this.selDierWerknummerHoogField;
            }
            set {
                this.selDierWerknummerHoogField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string selDierLandcode {
            get {
                return this.selDierLandcodeField;
            }
            set {
                this.selDierLandcodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string selCodeMerktype {
            get {
                return this.selCodeMerktypeField;
            }
            set {
                this.selCodeMerktypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string selCodeSoortMerk {
            get {
                return this.selCodeSoortMerkField;
            }
            set {
                this.selCodeSoortMerkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> aantal {
            get {
                return this.aantalField;
            }
            set {
                this.aantalField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/merken_v2_0")]
    public partial class merkenResponseType : merkenRequestType {
        
        private verwerkingsresultaatType verwerkingsresultaatField;
        
        private merkType[] merkField;
        
        /// <remarks/>
        public verwerkingsresultaatType verwerkingsresultaat {
            get {
                return this.verwerkingsresultaatField;
            }
            set {
                this.verwerkingsresultaatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("merk")]
        public merkType[] merk {
            get {
                return this.merkField;
            }
            set {
                this.merkField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/algemeen_v2_0")]
    public partial class verwerkingsresultaatType {
        
        private string foutcodeField;
        
        private string foutmeldingField;
        
        private string soortFoutIndicatorField;
        
        private string succesIndicatorField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string foutcode {
            get {
                return this.foutcodeField;
            }
            set {
                this.foutcodeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string foutmelding {
            get {
                return this.foutmeldingField;
            }
            set {
                this.foutmeldingField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string soortFoutIndicator {
            get {
                return this.soortFoutIndicatorField;
            }
            set {
                this.soortFoutIndicatorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string succesIndicator {
            get {
                return this.succesIndicatorField;
            }
            set {
                this.succesIndicatorField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/merken_v2_0")]
    public partial class merkentypeType {
        
        private string aantalLosField;
        
        private string aantalPerColloField;
        
        private string codeField;
        
        private System.Nullable<int> dierSoortField;
        
        private string codeFabrikantField;
        
        private string codeLeverancierField;
        
        private string naamLeverancierField;
        
        private string codeSoortMerkField;
        
        private string codeVormOormerkField;
        
        private string omschrijvingMerktypeField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer", IsNullable=true)]
        public string aantalLos {
            get {
                return this.aantalLosField;
            }
            set {
                this.aantalLosField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer", IsNullable=true)]
        public string aantalPerCollo {
            get {
                return this.aantalPerColloField;
            }
            set {
                this.aantalPerColloField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string code {
            get {
                return this.codeField;
            }
            set {
                this.codeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public System.Nullable<int> dierSoort {
            get {
                return this.dierSoortField;
            }
            set {
                this.dierSoortField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeFabrikant {
            get {
                return this.codeFabrikantField;
            }
            set {
                this.codeFabrikantField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeLeverancier {
            get {
                return this.codeLeverancierField;
            }
            set {
                this.codeLeverancierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string naamLeverancier {
            get {
                return this.naamLeverancierField;
            }
            set {
                this.naamLeverancierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeSoortMerk {
            get {
                return this.codeSoortMerkField;
            }
            set {
                this.codeSoortMerkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string codeVormOormerk {
            get {
                return this.codeVormOormerkField;
            }
            set {
                this.codeVormOormerkField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string omschrijvingMerktype {
            get {
                return this.omschrijvingMerktypeField;
            }
            set {
                this.omschrijvingMerktypeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(merktypenResponseType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/merken_v2_0")]
    public partial class merktypenRequestType {
        
        private string requestIDField;
        
        private string selTypeBestellingField;
        
        private string selDierSoortField;
        
        /// <remarks/>
        public string requestID {
            get {
                return this.requestIDField;
            }
            set {
                this.requestIDField = value;
            }
        }
        
        /// <remarks/>
        public string selTypeBestelling {
            get {
                return this.selTypeBestellingField;
            }
            set {
                this.selTypeBestellingField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string selDierSoort {
            get {
                return this.selDierSoortField;
            }
            set {
                this.selDierSoortField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/merken_v2_0")]
    public partial class merktypenResponseType : merktypenRequestType {
        
        private verwerkingsresultaatType verwerkingsresultaatField;
        
        private merkentypeType[] merkentypeField;
        
        /// <remarks/>
        public verwerkingsresultaatType verwerkingsresultaat {
            get {
                return this.verwerkingsresultaatField;
            }
            set {
                this.verwerkingsresultaatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("merkentype")]
        public merkentypeType[] merkentype {
            get {
                return this.merkentypeField;
            }
            set {
                this.merkentypeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9032.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.ienr.org/schemas/types/merken_v2_0")]
    public partial class controleerLevensnummerResponseType : controleerLevensnummerRequestType {
        
        private verwerkingsresultaatType verwerkingsresultaatField;
        
        private string dierWerknummerField;
        
        /// <remarks/>
        public verwerkingsresultaatType verwerkingsresultaat {
            get {
                return this.verwerkingsresultaatField;
            }
            set {
                this.verwerkingsresultaatField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string dierWerknummer {
            get {
                return this.dierWerknummerField;
            }
            set {
                this.dierWerknummerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    public delegate void controleerLevensnummerCompletedEventHandler(object sender, controleerLevensnummerCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class controleerLevensnummerCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal controleerLevensnummerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public controleerLevensnummerResponseType Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((controleerLevensnummerResponseType)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    public delegate void raadplegenMerktypenCompletedEventHandler(object sender, raadplegenMerktypenCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class raadplegenMerktypenCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal raadplegenMerktypenCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public merktypenResponseType Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((merktypenResponseType)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    public delegate void raadplegenMerkenCompletedEventHandler(object sender, raadplegenMerkenCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.9032.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class raadplegenMerkenCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal raadplegenMerkenCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public merkenResponseType Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((merkenResponseType)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591