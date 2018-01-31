using System.Reflection;

namespace InSituVisualization.TelemetryCollector.Filter.Property
{
    public class StringFilterProperty : FilterProperty
    {
        
        private readonly FilterKind _filterKinds;
        
        public StringFilterProperty(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            _filterKinds = FilterKind.IsEqual | FilterKind.Contains;
        }

        public override FilterKind GetFilterKinds()
        {
            return _filterKinds;
        }
    }
}
