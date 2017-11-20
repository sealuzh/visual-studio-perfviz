using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace VSIX_InSituVisualization.TelemetryCollector

{
    /// <summary>
    /// AzureTelemetryFactory creates and returns exactly one instance of AzureTelemetry. Loads Authorization Keys from settings store.
    /// </summary>
    internal static class AzureTelemetryFactory
    {

        private static AzureTelemetry _telemetryInstance;

        /// <summary>
        /// AzureTelemetry returns the one and current instance of AzureTelemetry.
        /// </summary>
        /// <returns>The current instance that is returned. Returns null if settings pane empty.</returns>
        public static AzureTelemetry GetInstance()
        {
            if (!GetWritableSettingsStore().PropertyExists("Performance Visualization", "AppId") ||
                !GetWritableSettingsStore().PropertyExists("Performance Visualization", "ApiKey"))
            {
                return null;
            }

            var appId = GetWritableSettingsStore().GetString("Performance Visualization", "AppId");
            var apiKey = GetWritableSettingsStore().GetString("Performance Visualization", "ApiKey");

            //check whether necessary variables are given - if not abort
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(appId))
            {
                return null;
            }

            //create factory instance
            return _telemetryInstance ?? (_telemetryInstance = new AzureTelemetry(appId, apiKey));
        }

        private static WritableSettingsStore GetWritableSettingsStore()
        {
            var shellSettingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            return shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

    }
}
