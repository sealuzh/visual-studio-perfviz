using Microsoft.CodeAnalysis;
using VSIX_InSituVisualization.Model;

namespace VSIX_InSituVisualization.TelemetryMapper
{
    internal interface ITelemetryDataMapper
    {
        MethodPerformanceInfo GetMethodPerformanceInfo(IMethodSymbol methodSymbol);
    }
}
