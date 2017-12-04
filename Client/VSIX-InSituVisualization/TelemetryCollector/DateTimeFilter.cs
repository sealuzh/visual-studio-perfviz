using System;
using System.Collections.Generic;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    class DateTimeFilter : IFilter
    {

        private readonly DateTime _filterString;
        private readonly PropertyInfo _property;
        private readonly DateTimeFilterType _dateTimeFilterType;

        public DateTimeFilter(PropertyInfo property, DateTimeFilterType dateTimeFilterType, DateTime filterString)
        {
            _filterString = filterString;
            _property = property;
            _dateTimeFilterType = dateTimeFilterType;
        }

        public enum DateTimeFilterType
        {
            IsEqual, IsGreaterEqualThen, IsSmallerEqualThen
        }

        public IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> ApplyFilter(IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> inDictionary)
        {
            var outDictionary = new Dictionary<string, IDictionary<string, ConcreteMemberTelemetry>>(inDictionary);
            var toRemoveMethodKeys = new List<string>();
            foreach (var kvpMethod in inDictionary)
            {
                var toRemoveMemberKeys = new List<string>();
                foreach (var kvpMember in inDictionary[kvpMethod.Key])
                {
                    var memberPropertyValue = (DateTime)_property.GetValue(kvpMember.Value);
                    switch (_dateTimeFilterType)
                    {
                        case DateTimeFilterType.IsEqual:
                            //TODO
                            if (!memberPropertyValue.Equals(_filterString))
                            {
                                toRemoveMemberKeys.Add(kvpMember.Key);
                            }
                            break;
                        case DateTimeFilterType.IsGreaterEqualThen:
                            //TODO
                            if (!(memberPropertyValue >= _filterString))
                            {
                                toRemoveMemberKeys.Add(kvpMember.Key);
                            }
                            break;
                        case DateTimeFilterType.IsSmallerEqualThen:
                            //TODO
                            if (!(memberPropertyValue <= _filterString))
                            {
                                toRemoveMemberKeys.Add(kvpMember.Key);
                            }
                            break;
                    }
                }
                foreach (var removeKey in toRemoveMemberKeys)
                {
                    outDictionary[kvpMethod.Key].Remove(removeKey);
                }
                //check whether db on method level is empty --> remove
                if (outDictionary[kvpMethod.Key].Count <= 0)
                {
                    toRemoveMethodKeys.Add(kvpMethod.Key);
                }
            }
            foreach (var removeKey in toRemoveMethodKeys)
            {
                outDictionary.Remove(removeKey);
            }
            return outDictionary;

        }
    }
}
