using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization
{

    // ReSharper disable once ClassNeverInstantiated.Global Justification: IoC
    internal class TelemetryDataMapper : ITelemetryDataMapper
    {
        private readonly ITelemetryDataProvider _telemetryDataProvider;

        public TelemetryDataMapper(ITelemetryDataProvider telemetryDataProvider)
        {
            _telemetryDataProvider = telemetryDataProvider;
        }

        private readonly Dictionary<MemberDeclarationSyntax, PerformanceInfo> _performanceInfos = new Dictionary<MemberDeclarationSyntax, PerformanceInfo>();

        public PerformanceInfo GetPerformanceInfo(MemberDeclarationSyntax memberDeclarationSyntax)
        {
            // TODO RR: Use SyntaxAnnotation https://joshvarty.wordpress.com/2015/09/18/learn-roslyn-now-part-13-keeping-track-of-syntax-nodes-with-syntax-annotations/
            // TODO RR: Do one Dictionary per Class/File
            var memberName = memberDeclarationSyntax.GetMemberIdentifier().ToString();
            try
            {
                // TODO RR: Do Real Mapping
                var averageMemberTelemetries = _telemetryDataProvider.GetAveragedMemberTelemetry();
                //var averageMemberTelemetries = dataStore.GetAveragedMemberTelemetry();
                // is null when being written to at the same time
                if (averageMemberTelemetries != null)
                {
                    // if no information given for this method it does not exist in dict
                    if (!averageMemberTelemetries.ContainsKey(memberName))
                    {
                        return null;
                    }
                    var performanceInfo = new PerformanceInfo(memberName)
                    {
                        MeanExecutionTime = averageMemberTelemetries[memberName].Duration,
                        //TODO RR: integrate MemberCount in interface.
                        MemberCount = averageMemberTelemetries[memberName].MemberCount
                    };

                    if (_performanceInfos.ContainsKey(memberDeclarationSyntax))
                    {
                        _performanceInfos.Remove(memberDeclarationSyntax);
                    }
                    _performanceInfos.Add(memberDeclarationSyntax, performanceInfo);
                    return performanceInfo;
                }
                return _performanceInfos.TryGetValue(memberDeclarationSyntax, out var perfInfo) ? perfInfo : null;
            }
            catch
            {
                return null;
            }
        }
    }

}
