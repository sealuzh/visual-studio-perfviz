using System;
using System.Collections.Generic;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter
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
            var outDictionary = new Dictionary<string, IDictionary<string, ConcreteMemberTelemetry>>();
            foreach (var kvpMethod in inDictionary)
            {
                outDictionary.Add(kvpMethod.Key, new Dictionary<string, ConcreteMemberTelemetry>());
                foreach (var kvpMember in inDictionary[kvpMethod.Key])
                {
                    var memberPropertyValue = (DateTime)_property.GetValue(kvpMember.Value);
                    switch (_dateTimeFilterType)
                    {
                        case DateTimeFilterType.IsEqual:
                            if (!memberPropertyValue.Equals(_filterString))
                            {
                                outDictionary[kvpMethod.Key].Add(kvpMember.Key, kvpMember.Value);
                                }
                            break;
                        case DateTimeFilterType.IsGreaterEqualThen:
                            if (!(memberPropertyValue >= _filterString))
                            {
                                outDictionary[kvpMethod.Key].Add(kvpMember.Key, kvpMember.Value);
                               }
                            break;
                        case DateTimeFilterType.IsSmallerEqualThen:
                            if (!(memberPropertyValue <= _filterString))
                            {
                                outDictionary[kvpMethod.Key].Add(kvpMember.Key, kvpMember.Value);                            }
                            break;
                        default:
                            break;
                    }
                }
                //TODO: If this section is removed, also methods with no filtering results will have some value to show.
                if (outDictionary[kvpMethod.Key].Count <= 0)
                {
                   outDictionary.Remove(kvpMethod.Key);
                }
            }
            
            return outDictionary;

        }
    }
}
