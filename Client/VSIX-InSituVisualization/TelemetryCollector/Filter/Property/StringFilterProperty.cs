using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter.Property
{
    class StringFilterProperty : FilterProperty
    {
        public const int IsEqual = 0;
        public const int Contains = 1;

        private readonly List<string> _filterParameterList;
        
        public StringFilterProperty(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            _filterParameterList = new List<string> {"IsEqual", "Contains"};
        }

        public override List<string> GetFilterParameterList()
        {
            return _filterParameterList;
        }
    }
}
