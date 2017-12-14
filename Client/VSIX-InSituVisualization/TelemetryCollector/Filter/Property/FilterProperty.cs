﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter.Property
{
    //Contains all info concerning one variable in ConcreteTelemetryMember. Intended to be used statically.
    class FilterProperty : IFilterProperty
    {
        public Type FilterType;
        private readonly PropertyInfo _propertyInfo;
        public string FilterName;

        
        public FilterProperty(PropertyInfo propertyInfo)
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
