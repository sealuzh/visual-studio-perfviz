using Microsoft.CodeAnalysis;

namespace VSIX_InSituVisualization
{
    internal interface ITelemetryDataMapper
    {
        MethodPerformanceInfo GetMethodPerformanceInfo(IMethodSymbol methodSymbol);
    }
}
