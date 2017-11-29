using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Settings;
//using Microsoft.VisualStudio.Shell.Settings;

namespace AzureTelemetryCollector.TelemetryCollector

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
            //TODO: Remove noVSIX flag
            Boolean noVSIX = true;
            if (noVSIX)
            {
                appId = "d6e2cc0a-5e85-4176-b44f-ef539f6e9492";
                apiKey = "u2u421ryg2ebynj3j1kbcqvbirek5atkcuhqoxcq";
            }
            else
            //TODO: Uncomment
            {
                //if (!GetWritableSettingsStore().PropertyExists("Performance Visualization", "AppId") ||
                //    !GetWritableSettingsStore().PropertyExists("Performance Visualization", "ApiKey"))
                //{
                //    return null;
                //}

                //appId = GetWritableSettingsStore().GetString("Performance Visualization", "AppId");
                //apiKey = GetWritableSettingsStore().GetString("Performance Visualization", "ApiKey");

                //check whether necessary variables are given - if not abort
                if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(appId))
                {
                    return null;
                }
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

        //TODO: Uncomment
        //private static WritableSettingsStore GetWritableSettingsStore()
        //{
        //    var shellSettingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
        //    return shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        //}

        //private static WritableSettingsStore GetWritableSettingsStore(bool noVSIX)
        //{

        //}

    }
}
