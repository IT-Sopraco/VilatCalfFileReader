﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VSM.RUMA.SOAPSANITEL.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("https://prd.sanitel.be/int/PRD_Sanitel_ExtInterfacingSvcs_v2/SanitelServices.asmx" +
            "")]
        public string SoapSanitel_SanitelServices_Sanitel_SanitelServices {
            get {
                return ((string)(this["SoapSanitel_SanitelServices_Sanitel_SanitelServices"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("https://qas.sanitel.be/int/QAS_Sanitel_ExtInterfacingSvcs_v2/SanitelServices.asmx" +
            "")]
        public string SoapSanitel_SanitelServices_Sanitel_SanitelServices_Test {
            get {
                return ((string)(this["SoapSanitel_SanitelServices_Sanitel_SanitelServices_Test"]));
            }
        }
    }
}
