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
        private Dictionary<String, PropertyInfo> _propertyMap;
        private List<IFilter> _currentFilters;

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
            switch (property.GetType().ToString())
            {
                case "String":
                    try
                    {
                        Enum.TryParse(filterType, out StringFilter.FilterType type);
                        IFilter newFilter = new StringFilter(property, type, parameter.ToString());
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
