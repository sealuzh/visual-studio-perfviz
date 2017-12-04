using System;
using System.Collections.Generic;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter
{
    internal class FilterController
    {
        private readonly Dictionary<string, PropertyInfo> _propertyMap;
        private readonly List<IFilter> _currentFilters;

        public FilterController()
        {
            _propertyMap = new Dictionary<string, PropertyInfo>();
            _currentFilters = new List<IFilter>();
            var propertyInfoArray = typeof(ConcreteMemberTelemetry).GetProperties();
            foreach (var prop in propertyInfoArray)
            {
                _propertyMap.Add(prop.Name, prop);
            }
        }

        public Dictionary<string, PropertyInfo> GetFilterProperties()
        {
            return _propertyMap;
        }

        public void AddFilter(PropertyInfo property, string filterType, object parameter)
        {
            switch (property.PropertyType.ToString())
            {
                case "System.String":
                    try
                    {
                        var type = (StringFilter.StringFilterType)Enum.Parse(typeof(StringFilter.StringFilterType), filterType);
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
                        var type = (DateTimeFilter.DateTimeFilterType)Enum.Parse(typeof(DateTimeFilter.DateTimeFilterType), filterType);
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

        public IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> ApplyFilters(IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> inDictionary)
        {
            if (_currentFilters.Count <= 0)
            {
                return inDictionary;
            }
            IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> outDictionary = new Dictionary<string, IDictionary<string, ConcreteMemberTelemetry>>(inDictionary);
            foreach (var filter in _currentFilters)
            {
                outDictionary = filter.ApplyFilter(outDictionary);
            }
            return outDictionary;
        }

        public void ResetFilter()
        {
            _currentFilters.Clear();
        }

    }
}
