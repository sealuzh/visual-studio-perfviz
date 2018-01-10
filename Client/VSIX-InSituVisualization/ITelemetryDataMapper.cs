using Microsoft.CodeAnalysis;

namespace VSIX_InSituVisualization
{
    internal interface ITelemetryDataMapper
    {
        // TODO RR: instead of MemberDeclarationSyntax, pass Semantic ModelInfo
        MethodPerformanceInfo GetMethodPerformanceInfo(IMethodSymbol methodSymbol);
    }
}
