using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using VSIX_InSituVisualization.TelemetryCollector;
using System;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{

    /// <summary>
    /// The Settings of the Extension
    /// 
    /// The use of application settings is not version safe in a VSIX. 
    /// The location of the stored setting file path in part includes the version string and hashes of the executable.
    /// When Visual Studio installs an official update these values change and as a consequence change the setting file path. 
    /// Visual Studio itself doesn’t support the use of application settings hence it makes no attempt to migrate this file to the new location and all information is essentially lost.
    /// 
    /// The supported method of settings is the WritableSettingsStore. It’s very similar to application settings and easy enough to access via SVsServiceProvider
    /// </summary>
    public class OptionsPageGrid : DialogPage
    {

        [Category("Credentials")]
        [DisplayName("Insights Application ID")]
        [Description("Azure Application Insights REST API: Application ID")]
        public string AppId
        {
            get => (string)WritableSettingsStoreController.GetWritableSettingsStoreValue("Performance Visualization", "AppId", typeof(string));
            set => WritableSettingsStoreController.SetWritableSettingsStoreValue("Performance Visualization", "AppId", value);
        }

        [Category("Credentials")]
        [DisplayName("Insights API Key")]
        [Description("Azure Application Insights REST API: API Key")]
        public string ApiKey
        {
            get => (string)WritableSettingsStoreController.GetWritableSettingsStoreValue("Performance Visualization", "ApiKey", typeof(string));
            set => WritableSettingsStoreController.SetWritableSettingsStoreValue("Performance Visualization", "ApiKey", value);
        }

        [Category("Settings")]
        [DisplayName("Insights REST maximum pulling amount.")]
        [Description("Maximum amount of performance elements to be downloaded per request.")]
        public int MaxPullingAmount
        {
            get => (int)WritableSettingsStoreController.GetWritableSettingsStoreValue("Performance Visualization", "MaxPullingAmount", typeof(int));
            set => WritableSettingsStoreController.SetWritableSettingsStoreValue("Performance Visualization", "MaxPullingAmount", value);
        }
    }
}
