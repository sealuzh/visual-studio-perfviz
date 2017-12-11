using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{

    /// <summary>
    /// Decorator for TelemetryDataMapper that caches...
    /// </summary>
    internal class CachedTelemetryDataMapper
    {
        private readonly ITelemetryDataMapper _telemetryDataMapper;
        private readonly Dictionary<MemberDeclarationSyntax, PerformanceInfo> _performanceInfos = new Dictionary<MemberDeclarationSyntax, PerformanceInfo>();


        public CachedTelemetryDataMapper(ITelemetryDataMapper telemetryDataMapper)
        {
            _telemetryDataMapper = telemetryDataMapper;
        }

        public PerformanceInfo GetPerformanceInfo(MemberDeclarationSyntax memberDeclarationSyntax)
        {
            if (_performanceInfos.TryGetValue(memberDeclarationSyntax, out var perfInfo))
            {
                return perfInfo;
            }
            var performanceInfo = _telemetryDataMapper.GetPerformanceInfo(memberDeclarationSyntax.GetMemberIdentifier().ToString());
            _performanceInfos.Add(memberDeclarationSyntax, performanceInfo);
            return performanceInfo;
        }
    }
}
