using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{
    internal interface ITelemetryDataMapper
    {
        PerformanceInfo GetPerformanceInfo(MemberDeclarationSyntax memberDeclarationSyntax);
    }
}
