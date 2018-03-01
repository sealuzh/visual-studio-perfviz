using System.Threading.Tasks;
using InSituVisualization.Model;

namespace InSituVisualization.TelemetryCollector
{
    internal interface ITelemetryProvider
    {
        Task<BundleMethodTelemetry> GetTelemetryDataAsync(string documentationCommentId);
    }
}
