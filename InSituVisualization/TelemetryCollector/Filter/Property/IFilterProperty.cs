using System.Reflection;

namespace InSituVisualization.TelemetryCollector.Filter.Property
{
    interface IFilterProperty
    {
        PropertyInfo GetPropertyInfo();
        FilterKind GetFilterKinds();
    }
}
