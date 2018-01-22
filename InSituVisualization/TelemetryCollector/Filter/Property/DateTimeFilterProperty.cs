using System.Reflection;

namespace InSituVisualization.TelemetryCollector.Filter.Property
{
    class DateTimeFilterProperty : FilterProperty
    {

        private readonly FilterKind _filterKinds;
        
        public DateTimeFilterProperty(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            _filterKinds = FilterKind.IsEqual | FilterKind.IsGreaterEqualThen | FilterKind.IsSmallerEqualThen;
        }

        public override FilterKind GetFilterKinds()
        {
            return _filterKinds;
        }
    }
}
