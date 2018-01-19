using System;
using Microsoft.ApplicationInsights;

namespace InSituInjectionDonor
{
    public class InSituInjectionDonor
    {
        public static void PreMethodBodyHook()
        {
            System.Diagnostics.Debug.WriteLine("Before Method Body");
            Console.WriteLine("Before Method Body Injection");
        }
        public static void PostMethodBodyHook()
        {
            System.Diagnostics.Debug.WriteLine("After Method Body");
            Console.WriteLine("After Method Body Injection");
        }

        //tracepath, type not important and can be neglected.
        public static void SendTelemetryToAzure(DateTime start, string type, string methodName, string tracePath)
        {
            TimeSpan duration = DateTime.UtcNow - start;
            TelemetryClient telemetryClient = new TelemetryClient();
            telemetryClient.TrackDependency(type, methodName, tracePath, "InsightsTest", start, duration, "0", true);
        }

    }
}
