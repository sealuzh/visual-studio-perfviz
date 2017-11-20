using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VSIX_InSituVisualization.TelemetryCollector;

namespace VSIX_InSituVisualization
{
    internal static class TelemetryCache
    {
        private static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(5);


        private static IList<AverageMemberTelemety> _averageMemberTelemeties;
        private static DateTime LastUpdate { get; } = DateTime.MinValue;

        public static async Task<IList<AverageMemberTelemety>> GetAverageMemberTelemetyAsync()
        {
            var telemetry = AzureTelemetryFactory.GetInstance();
            if (telemetry == null)
            {
                return new List<AverageMemberTelemety>();
            }
            if (DateTime.Now > LastUpdate + UpdateInterval)
            {
                _averageMemberTelemeties = await telemetry.GetAverageMemberTelemetiesAsync();
            }
            return _averageMemberTelemeties;
        }
    }
}
