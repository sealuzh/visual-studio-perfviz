using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using VSIX_InSituVisualization.TelemetryCollector.Filter.Property;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter
{
    internal class FilterController
    {
        //private readonly Dictionary<string, PropertyInfo> _propertyMap;
        private readonly List<IFilter> _activeFilters;
        private readonly List<FilterProperty> _filterProperties;

        public FilterController()
        {
            //_propertyMap = new Dictionary<string, PropertyInfo>();
            _activeFilters = new List<IFilter>();
            _filterProperties = new List<FilterProperty>();

            var propertyInfoArray = typeof(ConcreteMethodTelemetry).GetProperties();
            foreach (var prop in propertyInfoArray)
            {
                switch (prop.PropertyType.ToString())
                {
                    case "System.String":
                        _filterProperties.Add(new StringFilterProperty(prop));
                        break;
                    case "System.DateTime":
                        _filterProperties.Add(new DateTimeFilterProperty(prop));
                        break;
                    case "System.Int32":
                        _filterProperties.Add(new IntFilterProperty(prop));
                        break;
                    default:
                        break;
                }
            }
        }

        //Returs a list of FilterPropertyObjects that describe all possible variables in ConcreteMethodTelemetry to filter on.
        public List<FilterProperty> GetFilterProperties()
        {
            return _filterProperties;
        }

        //Adds a Filter that is used over all different methods available in the syntax
        public void AddFilterGlobal(IFilterProperty filterProperty, int filterType, object parameter)
        {
            IFilter newFilter;
            switch (filterProperty.GetPropertyInfo().PropertyType.ToString())
            {
                case "System.String":
                    newFilter = new StringFilter(filterProperty, (string)parameter, filterType);
                    _activeFilters.Add(newFilter);
                    break;
                case "System.DateTime":
                    newFilter = new DateTimeFilter(filterProperty, (DateTime)parameter, filterType);
                    _activeFilters.Add(newFilter);
                    break;
                case "System.Int32":
                    newFilter = new IntFilter(filterProperty, (int)parameter, filterType);
                    _activeFilters.Add(newFilter);
                    break;
                default:
                    break;
            }
        }

        //Adds a Filter that only applies to a single method in the syntax.
        public void AddFilterLocal(IFilterProperty filterProperty, int filterType, object parameter, string filterMethodFullName)
        {
            IFilter newFilter;
            switch (filterProperty.GetPropertyInfo().PropertyType.ToString())
            {
                case "System.String":
                    newFilter = new StringFilter(filterProperty, (string)parameter, filterType, filterMethodFullName);
                    _activeFilters.Add(newFilter);
                    break;
                case "System.DateTime":
                    newFilter = new DateTimeFilter(filterProperty, (DateTime)parameter, filterType, filterMethodFullName);
                    _activeFilters.Add(newFilter);
                    break;
                case "System.Int32":
                    newFilter = new IntFilter(filterProperty, (int)parameter, filterType, filterMethodFullName);
                    _activeFilters.Add(newFilter);
                    break;
                default:
                    break;
            }
        }

        //Applies the filters currently stored in _activeFilters.
        public IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> ApplyFilters(IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> inDictionary)
        {
            if (_activeFilters.Count <= 0)
            {
                return inDictionary;
            }
            IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> outDictionary = new Dictionary<string, IDictionary<string, ConcreteMethodTelemetry>>(inDictionary);
            foreach (var filter in _activeFilters)
            {
                outDictionary = filter.ApplyFilter(outDictionary);
            }
            return outDictionary;
        }

        public void ResetFilter()
        {
            _activeFilters.Clear();
        }

    }
}
