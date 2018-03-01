using System.Threading.Tasks;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryProvider
    {
        Task<MethodPerformanceData> GetTelemetryDataAsync(string documentationCommentId);
    }
}
