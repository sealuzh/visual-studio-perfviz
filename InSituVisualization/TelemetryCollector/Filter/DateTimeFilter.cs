using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using InSituVisualization.TelemetryCollector.Filter.Property;

namespace InSituVisualization.TelemetryCollector.Filter
{
    class DateTimeFilter : IFilter
    {

        private readonly DateTime _filterString;
        private readonly DateTimeFilterProperty _filterProperty;
        private readonly bool _isGlobalFilter;
        private readonly string _toFilterMethodFullName;
        private readonly FilterKind _filterKind;

        public DateTimeFilter(IFilterProperty filterProperty, DateTime filterString, FilterKind filterKind)
        {
            _filterString = filterString;
            _filterProperty = (DateTimeFilterProperty)filterProperty;
            _filterKind = filterKind;
            _isGlobalFilter = true;
        }

        public DateTimeFilter(IFilterProperty filterProperty, DateTime filterString, FilterKind filterKind, string toFilterMethodFullName)
        {
            _filterString = filterString;
            _filterProperty = (DateTimeFilterProperty) filterProperty;
            _filterKind = filterKind;
            _isGlobalFilter = false;
            _toFilterMethodFullName = toFilterMethodFullName;
        }



        public IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> ApplyFilter(IDictionary<string, IDictionary<string, ConcreteMethodTelemetry>> inDictionary)
        {
            var outDictionary = new Dictionary<string, IDictionary<string, ConcreteMethodTelemetry>>();
            foreach (var kvpMethod in inDictionary)
            {
                if (_isGlobalFilter)
                {
                    outDictionary.Add(kvpMethod.Key, ApplyFilterMethodLevel(kvpMethod.Key, inDictionary[kvpMethod.Key]));
                }
                else
                {
                    if (kvpMethod.Key != _toFilterMethodFullName)
                    {
                        outDictionary.Add(kvpMethod.Key, new ConcurrentDictionary<string, ConcreteMethodTelemetry>(kvpMethod.Value));
                    }
                    else
                    {
                        outDictionary.Add(kvpMethod.Key, ApplyFilterMethodLevel(kvpMethod.Key, inDictionary[kvpMethod.Key]));
                    }
                }
            }
            return outDictionary;
        }

        private IDictionary<string, ConcreteMethodTelemetry> ApplyFilterMethodLevel(string kvpMethodKey, IDictionary<string, ConcreteMethodTelemetry> inDictionary)
        {
            var outDictionary = new Dictionary<string, ConcreteMethodTelemetry>();
            foreach (var kvpMember in inDictionary)
            {
                var memberPropertyValue = (DateTime)_filterProperty.GetPropertyInfo().GetValue(kvpMember.Value);
                switch (_filterKind)
                {
                    case FilterKind.IsEqual:
                        if (!memberPropertyValue.Equals(_filterString))
                        {
                            outDictionary.Add(kvpMember.Key, kvpMember.Value);
                        }
                        break;
                    case FilterKind.IsGreaterEqualThen:
                        if (!(memberPropertyValue <= _filterString))
                        {
                            outDictionary.Add(kvpMember.Key, kvpMember.Value);
                        }
                        break;
                    case FilterKind.IsSmallerEqualThen:
                        if (!(memberPropertyValue >= _filterString))
                        {
                            outDictionary.Add(kvpMember.Key, kvpMember.Value);
                        }
                        break;
                    default:
                        break;
                }
            }
            return outDictionary;
        }
    }
}
