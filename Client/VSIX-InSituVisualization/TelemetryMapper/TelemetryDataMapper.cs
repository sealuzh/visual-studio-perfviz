﻿using Microsoft.CodeAnalysis;
using VSIX_InSituVisualization.Model;
using VSIX_InSituVisualization.TelemetryCollector;

namespace VSIX_InSituVisualization.TelemetryMapper
{

    // ReSharper disable once ClassNeverInstantiated.Global Justification: IoC
    internal class TelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly ITelemetryDataProvider _telemetryDataProvider;

        public TelemetryDataMapper(ITelemetryDataProvider telemetryDataProvider)
        {
            _telemetryDataProvider = telemetryDataProvider;
        }

        public MethodPerformanceInfo GetMethodPerformanceInfo(IMethodSymbol methodSymbol)
        {
            var documentationCommentId = methodSymbol.GetDocumentationCommentId();

            // TODO RR: Use SyntaxAnnotation https://joshvarty.wordpress.com/2015/09/18/learn-roslyn-now-part-13-keeping-track-of-syntax-nodes-with-syntax-annotations/
            // TODO RR: Do one Dictionary per Class/File
            try
            {
                // TODO RR: Do Real Mapping
                var averageMemberTelemetries = _telemetryDataProvider.GetAveragedMemberTelemetry();
                //var averageMemberTelemetries = dataStore.GetAveragedMemberTelemetry();
                // is null when being written to at the same time
                if (averageMemberTelemetries == null)
                {
                    return null;
                }
                // if no information given for this method it does not exist in dict
                if (!averageMemberTelemetries.ContainsKey(documentationCommentId))
                {
                    return null;
                }
                var performanceInfo = new MethodPerformanceInfo(methodSymbol)
                {
                    MeanExecutionTime = averageMemberTelemetries[documentationCommentId].Duration,
                    //TODO RR: integrate MemberCount in interface.
                    MemberCount = averageMemberTelemetries[documentationCommentId].MemberCount
                };
                return performanceInfo;
            }
            catch
            {
                return null;
            }
        }
    }

}