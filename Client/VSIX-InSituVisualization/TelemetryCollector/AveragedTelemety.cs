using System;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    /// <summary>
    /// Aggregated Average Data
    /// </summary>
    internal class AveragedTelemetry
    {
        public string MemberName { get; }
        public string NameSpace { get; }
        public TimeSpan Duration { get; }
        public int MemberCount { get; }

        public AveragedTelemetry(string memberName, string nameSpace, TimeSpan duration, int memberCount)
        {
            MemberName = memberName;
            NameSpace = nameSpace;
            Duration = duration;
            MemberCount = memberCount;
        }
    }
}
