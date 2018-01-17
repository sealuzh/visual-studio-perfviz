using System.Collections.Generic;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter.Property
{
    class IntFilterProperty : FilterProperty
    {
        
        private readonly FilterKind _filterKinds;

        public IntFilterProperty(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            _filterKinds = FilterKind.IsEqual | FilterKind.IsGreaterEqualThen | FilterKind.IsSmallerEqualThen;
        }

        public override FilterKind GetFilterKinds()
        {
            return _filterKinds;
        }
    }
}
