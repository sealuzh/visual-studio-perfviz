using System;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    /// <summary>
    /// Aggregated Average Data
    /// </summary>
    internal class AverageMemberTelemety : MemberTelemetry
    {
        public AverageMemberTelemety(string memberName, TimeSpan duration) : base(memberName, duration)
        {
        }
    }
}
