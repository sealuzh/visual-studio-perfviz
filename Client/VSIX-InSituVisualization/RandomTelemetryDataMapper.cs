using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{
    internal class RandomTelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly Dictionary<MemberDeclarationSyntax, PerformanceInfo> _performanceInfos = new Dictionary<MemberDeclarationSyntax, PerformanceInfo>();

        public PerformanceInfo GetPerformanceInfo(MemberDeclarationSyntax memberDeclarationSyntax)
        {
            if (_performanceInfos.TryGetValue(memberDeclarationSyntax, out var perfInfo))
            {
                return perfInfo;
            }
            var performanceInfo = new RandomPerformanceInfo(memberDeclarationSyntax.GetMemberIdentifier().ToString());
            _performanceInfos.Add(memberDeclarationSyntax, performanceInfo);
            return performanceInfo;
        }
    }
}
