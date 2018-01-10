using System.Collections.Generic;

namespace VSIX_InSituVisualization
{
    internal class RandomTelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly Dictionary<string, PerformanceInfo> _telemetryDatas = new Dictionary<string, PerformanceInfo>();

        public PerformanceInfo GetPerformanceInfo(string memberIdentification)
        {
            if (_telemetryDatas.TryGetValue(memberIdentification, out var performanceInfo))
            {
                return performanceInfo;
            }
            var newPerformanceInfo = new RandomPerformanceInfo(memberIdentification);
            _telemetryDatas.Add(memberIdentification, newPerformanceInfo);
            return newPerformanceInfo;
        }
    }
}
