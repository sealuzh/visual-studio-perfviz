using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter.Property
{
    class IntFilterProperty : FilterProperty
    {
        public const int IsEqual = 0;
        public const int IsGreaterEqualThen = 1;
        public const int IsSmallerEqualThen = 2;

        public IList<string> FilterParameter;

        public IntFilterProperty(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            FilterParameter = new List<string> { "IsEqual", "IsGreaterEqualThen", "IsSmallerEqualThen" };
        }
    }
}
