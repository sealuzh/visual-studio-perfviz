using System;
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

        public MethodPerformanceInfo GetMethodPerformanceInfo(IMethodSymbol methodSymbol)
        {
            var documentationCommentId = methodSymbol.GetDocumentationCommentId();

            // TODO RR: Use SyntaxAnnotation https://joshvarty.wordpress.com/2015/09/18/learn-roslyn-now-part-13-keeping-track-of-syntax-nodes-with-syntax-annotations/
            // TODO RR: Do one Dictionary per Class/File
            try
            {
                if (_telemetryProvider.TelemetryData == null)
                {
                    return null;
                }
                // if no information given for this method it does not exist in dict
                if (!_telemetryProvider.TelemetryData.ContainsKey(documentationCommentId))
                {
                    return null;
                }
                var performanceInfo = new MethodPerformanceInfo(methodSymbol)
                {
                    MeanExecutionTime = _telemetryProvider.TelemetryData[documentationCommentId].Duration,
                    //TODO RR: integrate MemberCount in interface.
                    MemberCount = _telemetryProvider.TelemetryData[documentationCommentId].MemberCount
                };
                return performanceInfo;
            }
            catch (Exception e)
            {
                // TODO RR: Check CatchAll
                return null;
            }
        }
    }

}
