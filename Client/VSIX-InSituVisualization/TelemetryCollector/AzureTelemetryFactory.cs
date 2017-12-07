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
            //create factory instance
            if (_telemetryInstance != null)
            {
                return _telemetryInstance;
            }
            else
            {
                _telemetryInstance = new AzureTelemetryStore();
                return _telemetryInstance;

            }
        }


        

    }
}
