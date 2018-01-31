using System.Reflection;

namespace InSituVisualization.TelemetryCollector.Filter.Property
{
    public interface IFilterProperty
    {
        PropertyInfo GetPropertyInfo();
        FilterKind GetFilterKinds();
    }
}
