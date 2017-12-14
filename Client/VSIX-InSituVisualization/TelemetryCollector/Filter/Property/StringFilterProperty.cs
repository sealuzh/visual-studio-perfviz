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

        public IList<string> FilterParameter;
        
        public StringFilterProperty(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            FilterParameter = new List<string> {"IsEqual", "Contains"};
        }
    }
}
