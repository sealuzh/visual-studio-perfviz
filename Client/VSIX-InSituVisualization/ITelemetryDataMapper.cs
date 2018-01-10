using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace VSIX_InSituVisualization
{
    internal interface ITelemetryDataMapper
    {
        // TODO RR: instead of MemberDeclarationSyntax, pass Semantic ModelInfo
        MethodPerformanceInfo GetMethodPerformanceInfo(CSharpSyntaxNode syntaxNode, IMethodSymbol methodSymbol);
    }
}
