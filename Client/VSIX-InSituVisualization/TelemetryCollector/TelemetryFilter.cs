using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    
    class TelemetryFilter
    {

        private List<PropertyInfo> _properties;
        private readonly Dictionary<String, PropertyInfo> _propertyMap;
        private readonly List<IFilter> _currentFilters;

        public TelemetryFilter()
        {
            _properties = new List<PropertyInfo>();
            _propertyMap = new Dictionary<String, PropertyInfo>();
            _currentFilters = new List<IFilter>();
            var propertyInfoArray = typeof(ConcreteMemberTelemetry).GetProperties();
            foreach (PropertyInfo prop in propertyInfoArray)
            {
                _properties.Add(prop);
                _propertyMap.Add(prop.Name, prop);
            }
            
        }

        public Dictionary<String, PropertyInfo> GetFilterProperties()
        {
            return _propertyMap;
        }

        public void AddFilter(PropertyInfo property, String filterType, Object parameter)
        {
            switch (property.PropertyType.ToString())
            {
                case "System.String":
                    try
                    {
                        StringFilter.StringFilterType type = (StringFilter.StringFilterType)Enum.Parse(typeof(StringFilter.StringFilterType), filterType);
                        IFilter newFilter = new StringFilter(property, type, parameter.ToString());
                        _currentFilters.Add(newFilter);
                    }
                    catch
                    {

                    }
                    break;
                case "System.DateTime":
                    try
                    {
                        DateTimeFilter.DateTimeFilterType type = (DateTimeFilter.DateTimeFilterType)Enum.Parse(typeof(DateTimeFilter.DateTimeFilterType), filterType);
                        IFilter newFilter = new DateTimeFilter(property, type, (DateTime)parameter);
                        _currentFilters.Add(newFilter);
                    }
                    catch
                    {

                    }
                    break;
                default:
                    break;
            }
        }

        public Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> ApplyFilters(Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> inDictionary)
        {
            if (_currentFilters.Count <= 0) return inDictionary;
            Dictionary<String, Dictionary<String, ConcreteMemberTelemetry>> outDictionary = new Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>>(inDictionary);
            foreach (IFilter filter in _currentFilters)
            {
                outDictionary = filter.ApplyFilter(inDictionary);
            }

            return outDictionary;
        }

        public void ResetFilter()
        {
            _currentFilters.Clear();
        }

    }
}
