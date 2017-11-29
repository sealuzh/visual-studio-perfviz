using System;

namespace AzureTelemetryCollector.TelemetryCollector
{
    /// <summary>
    /// Aggregated Average Data
    /// </summary>
    internal class AverageMemberTelemetry : MemberTelemetry
    {
        public AverageMemberTelemetry(string memberName, TimeSpan duration) : base(memberName, duration)
        {
        }
    }
}
