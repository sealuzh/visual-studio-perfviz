using System;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    internal abstract class TelemetryMember
    {
        protected TelemetryMember(string memberName, string nameSpace, int duration)
        {
            MemberName = memberName;
            NameSpace = nameSpace;
            Duration = duration;
        }

        public string MemberName { get; }
        public string NameSpace { get; }
        public int Duration { get; }

    }
}
