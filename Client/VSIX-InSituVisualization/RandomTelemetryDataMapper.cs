using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace VSIX_InSituVisualization
{
    internal class RandomTelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly Dictionary<IMethodSymbol, MethodPerformanceInfo> _telemetryDatas = new Dictionary<IMethodSymbol, MethodPerformanceInfo>();

        public MethodPerformanceInfo GetMethodPerformanceInfo(IMethodSymbol methodSymbol)
        {
            if (_telemetryDatas.TryGetValue(methodSymbol, out var performanceInfo))
            {
                return performanceInfo;
            }
            var newPerformanceInfo = new RandomMethodPerformanceInfo(methodSymbol);
            _telemetryDatas.Add(methodSymbol, newPerformanceInfo);
            return newPerformanceInfo;
        }
    }
}
