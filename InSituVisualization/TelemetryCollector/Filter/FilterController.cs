using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using InSituVisualization.TelemetryCollector.Filter.Property;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.Filter
{
    public class FilterController<T>
    {
        //private readonly Dictionary<string, PropertyInfo> _propertyMap;
        private readonly List<IFilter> _activeFilters;
        private readonly List<FilterProperty> _filterProperties;

        public FilterController()
        {
            //_propertyMap = new Dictionary<string, PropertyInfo>();
            _activeFilters = new List<IFilter>();
            _filterProperties = new List<FilterProperty>();

            var propertyInfoArray = typeof(T).GetProperties();
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


        //Returs a list of FilterPropertyObjects that describe all possible variables in ConcreteMethod to filter on.
        public List<FilterProperty> GetFilterProperties()
        {
            return _filterProperties;
        }

        //Adds a Filter that is used over all different methods available in the syntax
        public bool AddFilterGlobal(IFilterProperty filterProperty, FilterKind filterKind, object parameter)
        {
            IFilter newFilter;
            switch (filterProperty.GetPropertyInfo().PropertyType.ToString())
            {
                case "System.String":
                    if (!filterProperty.GetFilterKinds().HasFlag(filterKind)) return false;
                    newFilter = new StringFilter(filterProperty, (string)parameter, filterKind);
                    _activeFilters.Add(newFilter);
                    return true;
                case "System.DateTime":
                    if (!filterProperty.GetFilterKinds().HasFlag(filterKind)) return false;
                    newFilter = new DateTimeFilter(filterProperty, (DateTime)parameter, filterKind);
                    _activeFilters.Add(newFilter);
                    return true;
                case "System.Int32":
                    if (!filterProperty.GetFilterKinds().HasFlag(filterKind)) return false;
                    newFilter = new IntFilter(filterProperty, (int)parameter, filterKind);
                    _activeFilters.Add(newFilter);
                    return true;
                default:
                    return false;
            }
        }

        //Adds a Filter that only applies to a single method in the syntax.
        public bool AddFilterLocal(IFilterProperty filterProperty, FilterKind filterKind, object parameter, string filterMethodFullName)
        {
            IFilter newFilter;
            switch (filterProperty.GetPropertyInfo().PropertyType.ToString())
            {
                case "System.String":
                    if (!filterProperty.GetFilterKinds().HasFlag(filterKind)) return false;
                    newFilter = new StringFilter(filterProperty, (string)parameter, filterKind, filterMethodFullName);
                    _activeFilters.Add(newFilter);
                    return true;
                    case "System.DateTime":
                        if (!filterProperty.GetFilterKinds().HasFlag(filterKind)) return false;
                    newFilter = new DateTimeFilter(filterProperty, (DateTime)parameter, filterKind, filterMethodFullName);
                    _activeFilters.Add(newFilter);
                        return true;
                  
                case "System.Int32":
                    if (!filterProperty.GetFilterKinds().HasFlag(filterKind)) return false;
                    newFilter = new IntFilter(filterProperty, (int)parameter, filterKind, filterMethodFullName);
                    _activeFilters.Add(newFilter);
                    return true;
                   
                default:
                    return false;
            }
        }

        //Applies the filters currently stored in _activeFilters.
        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> ApplyFilters(ConcurrentDictionary<string, ConcurrentDictionary<string, T>> inDictionary)
        {
            if (_activeFilters.Count <= 0)
            {
                return inDictionary;
            }
            ConcurrentDictionary<string, ConcurrentDictionary<string, T>> outDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>(inDictionary);
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
