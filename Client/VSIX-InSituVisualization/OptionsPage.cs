using System.ComponentModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

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
    public class OptionsPage : DialogPage
    {

        [Category("Credentials")]
        [DisplayName("Insights Application ID")]
        [Description("Azure Application Insights REST API: Application ID")]
        public string AppId
        {
            get => GetWritableSettingsStore().GetString("Performance Visualization", "AppId");
            set => GetWritableSettingsStore().SetString("Performance Visualization", "AppId", value);
        }

        [Category("Credentials")]
        [DisplayName("Insights API Key")]
        [Description("Azure Application Insights REST API: API Key")]
        public string ApiKey
        {
            get => GetWritableSettingsStore().GetString("Performance Visualization", "ApiKey");
            set => GetWritableSettingsStore().SetString("Performance Visualization", "ApiKey", value);
        }

        private static WritableSettingsStore GetWritableSettingsStore()
        {
            var shellSettingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            return shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }
    }
}
