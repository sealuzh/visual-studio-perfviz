using InSituVisualization.Model;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.TelemetryMapper
{
    internal interface ITelemetryDataMapper
    {
        MethodPerformanceInfo GetMethodPerformanceInfo(IMethodSymbol methodSymbol);
    }
}
