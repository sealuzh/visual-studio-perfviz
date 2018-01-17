using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter.Property
{
    interface IFilterProperty
    {
        PropertyInfo GetPropertyInfo();
        FilterKind GetFilterKinds();
    }
}
