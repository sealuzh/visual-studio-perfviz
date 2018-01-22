using System;
using System.Reflection;

namespace InSituVisualization.TelemetryCollector.Filter.Property
{
    [Flags]
    public enum FilterKind { None = 0, IsEqual = 1, IsGreaterEqualThen = 2, IsSmallerEqualThen = 4, Contains = 8}

    //Contains all info concerning one variable in ConcreteMethodTelemetry. Intended to be used statically.
    abstract class FilterProperty : IFilterProperty
    {
        public abstract FilterKind GetFilterKinds();

        public Type FilterType;
        private readonly PropertyInfo _propertyInfo;
        public string FilterName;
        
        protected FilterProperty(PropertyInfo propertyInfo)
        {
            FilterType = propertyInfo.PropertyType;
            _propertyInfo = propertyInfo;
            FilterName = propertyInfo.Name;
        }

        public PropertyInfo GetPropertyInfo()
        {
            return _propertyInfo;
        }

        
    }

    
}
