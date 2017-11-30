using System;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

//using Microsoft.VisualStudio.Shell.Settings;

namespace VSIX_InSituVisualization.TelemetryCollector

{
    /// <summary>
    /// AzureTelemetryFactory creates and returns exactly one instance of AzureTelemetry. Loads Authorization Keys from settings store.
    /// </summary>
    internal static class AzureTelemetryFactory
    {

        private static AzureTelemetryStore _telemetryInstance;


        /// <summary>
        /// AzureTelemetry returns the one and current instance of AzureTelemetry.
        /// </summary>
        /// <returns>The current instance that is returned. Returns null if settings pane empty.</returns>
        public static AzureTelemetryStore GetInstance()
        {
            var appId = "";
            var apiKey = "";


            if (!GetWritableSettingsStore().PropertyExists("Performance Visualization", "AppId") ||
                !GetWritableSettingsStore().PropertyExists("Performance Visualization", "ApiKey"))
            {
                return null;
            }

            appId = GetWritableSettingsStore().GetString("Performance Visualization", "AppId");
            apiKey = GetWritableSettingsStore().GetString("Performance Visualization", "ApiKey");

            //check whether necessary variables are given - if not abort
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(appId))
            {
                return null;
            }



            //create factory instance
            if (_telemetryInstance != null)
            {
                return _telemetryInstance;
            }
            else
            {
                _telemetryInstance = new AzureTelemetryStore(appId, apiKey);
                return _telemetryInstance;

            }
        }


        private static WritableSettingsStore GetWritableSettingsStore()
        {
            var shellSettingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            return shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

    }
}
