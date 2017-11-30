using System;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    internal abstract class MemberTelemetry
    {
        protected MemberTelemetry(string memberName, String nameSpace, TimeSpan duration)
        {
            MemberName = memberName;
            NameSpace = nameSpace;
            Duration = duration;
        }

        public string MemberName { get; }
        public string NameSpace { get; }
        public TimeSpan Duration { get; }
    }
}
