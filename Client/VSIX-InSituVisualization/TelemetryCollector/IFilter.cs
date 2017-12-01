using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    interface IFilter
    {
        Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> ApplyFilter(Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> inDictionary);
    }
}
