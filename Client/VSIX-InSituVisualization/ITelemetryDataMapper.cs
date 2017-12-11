using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{
    internal interface ITelemetryDataMapper
    {
        // TODO RR: instead of MemberDeclarationSyntax, pass Semantic ModelInfo
        PerformanceInfo GetPerformanceInfo(string memberIdenfitication);
    }
}
