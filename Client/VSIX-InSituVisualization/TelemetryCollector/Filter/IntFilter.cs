using System;
using System.Collections.Generic;
using System.Reflection;
using VSIX_InSituVisualization.TelemetryCollector.Filter.Property;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter
{
    class IntFilter : IFilter
    {

        private readonly int _filterString;
        private readonly IntFilterProperty _filterProperty;
        private readonly bool _isGlobalFilter;
        private readonly string _toFilterMethodFullName;
        private readonly int _toFilterType;

        public IntFilter(IFilterProperty filterProperty, int filterString, int toFilterType)
        {
            _filterString = filterString;
            _filterProperty = (IntFilterProperty)filterProperty;
            _toFilterType = toFilterType;
            _isGlobalFilter = true;
        }

        public IntFilter(IFilterProperty filterProperty, int filterString, int toFilterType, string toFilterMethodFullName)
        {
            _filterString = filterString;
            _filterProperty = (IntFilterProperty)filterProperty;
            _toFilterType = toFilterType;
            _isGlobalFilter = false;
            _toFilterMethodFullName = toFilterMethodFullName;
        }



        public IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> ApplyFilter(IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> inDictionary)
        {
            var outDictionary = new Dictionary<string, IDictionary<string, ConcreteTelemetryMember>>();
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
                        outDictionary.Add(kvpMethod.Key, new Dictionary<string, ConcreteTelemetryMember>(kvpMethod.Value));
                    }
                    else
                    {
                        outDictionary.Add(kvpMethod.Key, ApplyFilterMethodLevel(kvpMethod.Key, inDictionary[kvpMethod.Key]));
                    }
                }
            }
            return outDictionary;
        }

        private IDictionary<string, ConcreteTelemetryMember> ApplyFilterMethodLevel(string kvpMethodKey, IDictionary<string, ConcreteTelemetryMember> inDictionary)
        {
            var outDictionary = new Dictionary<string, ConcreteTelemetryMember>();
            foreach (var kvpMember in inDictionary)
            {
                var memberPropertyValue = (int)_filterProperty.GetPropertyInfo().GetValue(kvpMember.Value);
                switch (_toFilterType)
                {
                    case IntFilterProperty.IsEqual:
                        if (!memberPropertyValue.Equals(_filterString))
                        {
                            outDictionary.Add(kvpMember.Key, kvpMember.Value);
                        }
                        break;
                    case IntFilterProperty.IsGreaterEqualThen:
                        if (!(memberPropertyValue <= _filterString))
                        {
                            outDictionary.Add(kvpMember.Key, kvpMember.Value);
                        }
                        break;
                    case IntFilterProperty.IsSmallerEqualThen:
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
