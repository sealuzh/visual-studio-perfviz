using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.Model;
using InSituVisualization.Predictions;
using InSituVisualization.TelemetryCollector;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.TelemetryMapper
{

    // ReSharper disable once ClassNeverInstantiated.Global Justification: IoC
    internal class TelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly IPredictionEngine _predictionEngine;
        private readonly ITelemetryProvider _telemetryProvider;

        private readonly Dictionary<string, MethodPerformanceInfo> _methodPerformancInfoCache = new Dictionary<string, MethodPerformanceInfo>();

        public TelemetryDataMapper(ITelemetryProvider telemetryProvider, IPredictionEngine predictionEngine)
        {
            _predictionEngine = predictionEngine ?? throw new ArgumentNullException(nameof(predictionEngine));
            _telemetryProvider = telemetryProvider ?? throw new ArgumentNullException(nameof(telemetryProvider));
        }

        public async Task<MethodPerformanceInfo> GetMethodPerformanceInfoAsync(IMethodSymbol methodSymbol)
        {
            var documentationCommentId = methodSymbol.GetDocumentationCommentId();

            if (_methodPerformancInfoCache.TryGetValue(documentationCommentId, out var existingMethodPerformanceInfo))
            {
                return existingMethodPerformanceInfo;
            }

            // TODO RR: Use SyntaxAnnotation https://joshvarty.wordpress.com/2015/09/18/learn-roslyn-now-part-13-keeping-track-of-syntax-nodes-with-syntax-annotations/
            // TODO RR: Do one Dictionary per Class/File
            var methodTelemetry = await _telemetryProvider.GetTelemetryDataAsync(documentationCommentId).ConfigureAwait(false);
            if (methodTelemetry == null)
            {
                return null;
            }

            var performanceInfo = new MethodPerformanceInfo(_predictionEngine, methodSymbol, methodTelemetry);
            _methodPerformancInfoCache.Add(documentationCommentId, performanceInfo);
            return performanceInfo;
        }
    }

}
