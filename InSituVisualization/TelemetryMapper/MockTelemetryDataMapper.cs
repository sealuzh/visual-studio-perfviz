using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InSituVisualization.Model;
using Microsoft.CodeAnalysis;

namespace InSituVisualization.TelemetryMapper
{
    /// <summary>
    /// Returns Mock Method Data
    /// </summary>
    internal class MockTelemetryDataMapper : ITelemetryDataMapper
    {
        private class MockMethodPerformanceData : MethodPerformanceData
        {

        }

        private class MockMethodPerformanceInfo : MethodPerformanceInfo
        {
            private static readonly Random Random = new Random();

            public MockMethodPerformanceInfo(IMethodSymbol methodSymbol, MethodPerformanceData data) : base( methodSymbol, data)
            {
                MethodPerformanceData.MeanExecutionTime = TimeSpan.FromMilliseconds(Random.Next(100));
            }
        }

        private readonly Dictionary<string, MethodPerformanceInfo> _telemetryDatas = new Dictionary<string, MethodPerformanceInfo>();

        public Task<MethodPerformanceInfo> GetMethodPerformanceInfoAsync(IMethodSymbol methodSymbol)
        {
            // DocumentationCommentId is used in Symbol Editor, since methodSymbols aren't equal accross compilations
            // see https://github.com/dotnet/roslyn/issues/3058
            var documentationCommentId = methodSymbol.GetDocumentationCommentId();
            if (_telemetryDatas.TryGetValue(documentationCommentId, out var performanceInfo))
            {
                return Task.FromResult(performanceInfo);
            }
            var newPerformanceInfo = new MockMethodPerformanceInfo(methodSymbol, new MockMethodPerformanceData());
            _telemetryDatas.Add(documentationCommentId, newPerformanceInfo);
            return Task.FromResult((MethodPerformanceInfo) newPerformanceInfo);
        }
    }
}
