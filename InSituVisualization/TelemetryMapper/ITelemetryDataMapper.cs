using System.Threading.Tasks;
using InSituVisualization.Model;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.TelemetryMapper
{
    internal interface ITelemetryDataMapper
    {
        Task<MethodPerformanceInfo> GetMethodPerformanceInfoAsync(IMethodSymbol methodSymbol);
    }
}
