﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VSIX_InSituVisualization.TelemetryCollector.Filter.Property;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter
{
    class StringFilter : IFilter
    {

        private readonly string _filterString;
        private readonly StringFilterProperty _filterProperty;
        private readonly bool _isGlobalFilter;
        private readonly string _toFilterMethodFullName;
        private readonly int _toFilterType;

        public StringFilter(IFilterProperty filterProperty, string filterString, int toFilterType)
        {
            _filterString = filterString;
            _filterProperty = (StringFilterProperty)filterProperty;
            _toFilterType = toFilterType;
            _isGlobalFilter = true;
        }

        public StringFilter(IFilterProperty filterProperty, string filterString, int toFilterType, string toFilterMethodFullName)
        {
            _filterString = filterString;
            _filterProperty = (StringFilterProperty)filterProperty;
            _toFilterType = toFilterType;
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
                        outDictionary.Add(kvpMethod.Key, new Dictionary<string, ConcreteMethodTelemetry>(kvpMethod.Value));
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
            foreach(var kvpMember in inDictionary)
                {
                var memberPropertyValue = (string)_filterProperty.GetPropertyInfo().GetValue(kvpMember.Value);
                    switch (_toFilterType)
                    {
                        case StringFilterProperty.IsEqual:
                            if (memberPropertyValue.Equals(_filterString))
                            {
                                outDictionary.Add(kvpMember.Key, kvpMember.Value);
                            }
                            break;
                        case StringFilterProperty.Contains:
                            if (!memberPropertyValue.Contains(_filterString))
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
