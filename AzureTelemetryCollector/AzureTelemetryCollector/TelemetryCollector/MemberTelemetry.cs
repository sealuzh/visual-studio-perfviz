using System;

namespace AzureTelemetryCollector.TelemetryCollector
{
    internal abstract class MemberTelemetry
    {
        protected MemberTelemetry(string memberName, TimeSpan duration)
        {
            MemberName = memberName;
            Duration = duration;
        }

        public string MemberName { get; }
        public TimeSpan Duration { get; }
    }
}
