using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace VSIX_InSituVisualization
{
    internal class RandomTelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly Dictionary<string, MethodPerformanceInfo> _telemetryDatas = new Dictionary<string, MethodPerformanceInfo>();

        public MethodPerformanceInfo GetMethodPerformanceInfo(IMethodSymbol methodSymbol)
        {
            // DocumentationCommentId is used in Symbol Editor, since methodSymbols aren't equal accross compilations
            // see https://github.com/dotnet/roslyn/issues/3058
            var documentationCommentId = methodSymbol.GetDocumentationCommentId();
            if (_telemetryDatas.TryGetValue(documentationCommentId, out var performanceInfo))
            {
                return performanceInfo;
            }
            var newPerformanceInfo = new RandomMethodPerformanceInfo(methodSymbol);
            _telemetryDatas.Add(documentationCommentId, newPerformanceInfo);
            return newPerformanceInfo;
        }
    }
}
