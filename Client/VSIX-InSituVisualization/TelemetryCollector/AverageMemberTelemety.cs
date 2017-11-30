using System;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    /// <summary>
    /// Aggregated Average Data
    /// </summary>
    internal class AverageMemberTelemetry : MemberTelemetry
    {
        public AverageMemberTelemetry(string memberName, String nameSpace, TimeSpan duration) : base(memberName, nameSpace, duration)
        {
        }
    }
}
