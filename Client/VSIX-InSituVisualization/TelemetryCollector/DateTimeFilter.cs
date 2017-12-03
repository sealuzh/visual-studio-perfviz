using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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

        public Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> ApplyFilter(Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> inDictionary)
        {
            Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> outDictionary = new Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>>(inDictionary);
            List<String> toRemoveMethodKeys = new List<String>();
            foreach (KeyValuePair<String, Dictionary<String, ConcreteMemberTelemetry>> kvpMethod in inDictionary)
            {
                List<String> toRemoveMemberKeys = new List<string>();
                foreach (KeyValuePair<String, ConcreteMemberTelemetry> kvpMember in inDictionary[kvpMethod.Key])
                {
                    DateTime memberPropertyValue = (DateTime)_property.GetValue(kvpMember.Value);
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
                foreach (String removeKey in toRemoveMemberKeys)
                {
                    outDictionary[kvpMethod.Key].Remove(removeKey);
                }
                //check whether db on method level is empty --> remove
                if (outDictionary[kvpMethod.Key].Count <= 0)
                {
                    toRemoveMethodKeys.Add(kvpMethod.Key);
                }
            }
            foreach (String removeKey in toRemoveMethodKeys)
            {
                outDictionary.Remove(removeKey);
            }
            return outDictionary;

        }
    }
}
