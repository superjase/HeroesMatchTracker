﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HeroesMatchTracker.Data.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Replays")]
        public string ReplaysConnNameDb {
            get {
                return ((string)(this["ReplaysConnNameDb"]));
            }
            set {
                this["ReplaysConnNameDb"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Settings")]
        public string SettingsConnNameDb {
            get {
                return ((string)(this["SettingsConnNameDb"]));
            }
            set {
                this["SettingsConnNameDb"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Databases")]
        public string DatabaseFolderName {
            get {
                return ((string)(this["DatabaseFolderName"]));
            }
            set {
                this["DatabaseFolderName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Replays.sqlite")]
        public string ReplaysDbFileName {
            get {
                return ((string)(this["ReplaysDbFileName"]));
            }
            set {
                this["ReplaysDbFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Settings.sqlite")]
        public string SettingsDbFileName {
            get {
                return ((string)(this["SettingsDbFileName"]));
            }
            set {
                this["SettingsDbFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("14")]
        public int ReplaysDatabaseMigrationVersion {
            get {
                return ((int)(this["ReplaysDatabaseMigrationVersion"]));
            }
            set {
                this["ReplaysDatabaseMigrationVersion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int SettingsDatabaseMigrationVersion {
            get {
                return ((int)(this["SettingsDatabaseMigrationVersion"]));
            }
            set {
                this["SettingsDatabaseMigrationVersion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ReleaseNotes")]
        public string ReleaseNotesConnNameDb {
            get {
                return ((string)(this["ReleaseNotesConnNameDb"]));
            }
            set {
                this["ReleaseNotesConnNameDb"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ReleaseNotes.sqlite")]
        public string ReleaseNotesDbFileName {
            get {
                return ((string)(this["ReleaseNotesDbFileName"]));
            }
            set {
                this["ReleaseNotesDbFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int ReleaseNotesDatabaseMigrationVersion {
            get {
                return ((int)(this["ReleaseNotesDatabaseMigrationVersion"]));
            }
            set {
                this["ReleaseNotesDatabaseMigrationVersion"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HeroesParserData.db")]
        public string Version1DatabaseName {
            get {
                return ((string)(this["Version1DatabaseName"]));
            }
            set {
                this["Version1DatabaseName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DatabasesMigrationLog.txt")]
        public string MigrationLogFile {
            get {
                return ((string)(this["MigrationLogFile"]));
            }
            set {
                this["MigrationLogFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HeroesParserData")]
        public string OldHeroesParserDatabaseConnName {
            get {
                return ((string)(this["OldHeroesParserDatabaseConnName"]));
            }
            set {
                this["OldHeroesParserDatabaseConnName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Database")]
        public string OldDatabaseFolderName {
            get {
                return ((string)(this["OldDatabaseFolderName"]));
            }
            set {
                this["OldDatabaseFolderName"] = value;
            }
        }
    }
}
