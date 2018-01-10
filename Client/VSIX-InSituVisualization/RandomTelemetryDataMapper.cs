using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace VSIX_InSituVisualization
{
    internal class RandomTelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly Dictionary<CSharpSyntaxNode, MethodPerformanceInfo> _telemetryDatasPerNode = new Dictionary<CSharpSyntaxNode, MethodPerformanceInfo>();
        private readonly Dictionary<IMethodSymbol, MethodPerformanceInfo> _telemetryDatas = new Dictionary<IMethodSymbol, MethodPerformanceInfo>();

        public MethodPerformanceInfo GetMethodPerformanceInfo(CSharpSyntaxNode syntaxNode, IMethodSymbol methodSymbol)
        {
            if (_telemetryDatasPerNode.TryGetValue(syntaxNode, out var performanceInfoFromExistingNode))
            {
                return performanceInfoFromExistingNode;
            }
            if (_telemetryDatas.TryGetValue(methodSymbol, out var performanceInfo))
            {
                return performanceInfo;
            }
            var newPerformanceInfo = new RandomMethodPerformanceInfo(methodSymbol);
            _telemetryDatas.Add(methodSymbol, newPerformanceInfo);
            _telemetryDatasPerNode.Add(syntaxNode, newPerformanceInfo);
            return newPerformanceInfo;
        }
    }
}
