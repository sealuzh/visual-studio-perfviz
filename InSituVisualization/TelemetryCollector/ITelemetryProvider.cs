using System.Threading.Tasks;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryProvider
    {
        Task<IMethodPerformanceData> GetTelemetryDataAsync(string documentationCommentId);
    }
}
