using System;
using System.Threading.Tasks;
using InSituVisualization.Model;
using InSituVisualization.TelemetryCollector;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.TelemetryMapper
{

    // ReSharper disable once ClassNeverInstantiated.Global Justification: IoC
    internal class TelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly ITelemetryProvider _telemetryProvider;

        public TelemetryDataMapper(ITelemetryProvider telemetryProvider)
        {
            _telemetryProvider = telemetryProvider ?? throw new ArgumentNullException(nameof(telemetryProvider));
        }

        public async Task<MethodPerformanceInfo> GetMethodPerformanceInfoAsync(IMethodSymbol methodSymbol)
        {
            var documentationCommentId = methodSymbol.GetDocumentationCommentId();

            // TODO RR: Use SyntaxAnnotation https://joshvarty.wordpress.com/2015/09/18/learn-roslyn-now-part-13-keeping-track-of-syntax-nodes-with-syntax-annotations/
            // TODO RR: Do one Dictionary per Class/File
            var methodTelemetry = await _telemetryProvider.GetTelemetryDataAsync(documentationCommentId);
            if (methodTelemetry == null)
            {
                return null;
            }
            return new MethodPerformanceInfo(methodSymbol)
            {
                MeanExecutionTime = methodTelemetry.GetAverageDuration(),
                //TODO RR: integrate MemberCount in interface.
                MemberCount = methodTelemetry.Durations.Count
            };
        }
    }

}
