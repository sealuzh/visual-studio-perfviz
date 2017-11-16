using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using EnvDTE80;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace VSIX_InSituVisualization.TelemetryCollector

{
    class AzureTelemetryFactory
    {

        private static AzureTelemetry telemetryInstance;

        public static AzureTelemetry getInstance()
        {
            String appId = "";
            String apiKey = "";
            if (GetWritableSettingsStore().PropertyExists("Performance Visualization", "AppId") &&
                GetWritableSettingsStore().PropertyExists("Performance Visualization", "ApiKey"))
            {
                appId = GetWritableSettingsStore().GetString("Performance Visualization", "AppId");
                apiKey = GetWritableSettingsStore().GetString("Performance Visualization", "ApiKey");
            }

            //check whether necessary variables are given - if not abort
            if (apiKey == "" || appId == "")
            {
                return null;
            }
            else
            {
                //create factory instance
                if (telemetryInstance == null)
                {
                    telemetryInstance = new AzureTelemetry(appId, apiKey);
                    return telemetryInstance;
                }
                else
                {
                    return telemetryInstance;
                }
            }
 
        }

        private static WritableSettingsStore GetWritableSettingsStore()
        {
            var shellSettingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            return shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

    }
}
