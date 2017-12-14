using System.Collections.Generic;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter.Property
{
    class DateTimeFilterProperty : FilterProperty
    {
        public const int IsEqual = 0;
        public const int IsGreaterEqualThen = 1;
        public const int IsSmallerEqualThen = 2;

        private readonly List<string> _filterParameterList;
        
        public DateTimeFilterProperty(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            _filterParameterList = new List<string> {"IsEqual", "IsGreaterEqualThen", "IsSmallerEqualThen"};
        }

        public override List<string> GetFilterParameterList()
        {
            return _filterParameterList;
        }
    }
}
