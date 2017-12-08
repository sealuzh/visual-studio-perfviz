using System.Collections.Generic;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector.Filter
{
    class StringFilter : IFilter
    {

        private readonly string _filterString;
        private readonly PropertyInfo _property;
        private readonly StringFilterType _stringFilterType;

        public StringFilter(PropertyInfo property, StringFilterType stringFilterType, string filterString)
        {
            _filterString = filterString;
            _property = property;
            _stringFilterType = stringFilterType;
        }

        public enum StringFilterType
        {
            IsEqual, Contains
        }

        public IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> ApplyFilter(IDictionary<string, IDictionary<string, ConcreteTelemetryMember>> inDictionary)
        {
            var outDictionary = new Dictionary<string, IDictionary<string, ConcreteTelemetryMember>>();
            foreach (var kvpMethod in inDictionary)
            {
                outDictionary.Add(kvpMethod.Key, new Dictionary<string, ConcreteTelemetryMember>());
                foreach (var kvpMember in inDictionary[kvpMethod.Key])
                {
                    var memberPropertyValue = (string)_property.GetValue(kvpMember.Value);
                    switch (_stringFilterType)
                    {
                        case StringFilterType.IsEqual:
                            if (memberPropertyValue.Equals(_filterString))
                            {
                                outDictionary[kvpMethod.Key].Add(kvpMember.Key, kvpMember.Value);
                            }
                            break;
                        case StringFilterType.Contains:
                            if (!memberPropertyValue.Contains(_filterString))
                            {
                                outDictionary[kvpMethod.Key].Add(kvpMember.Key, kvpMember.Value);
                            }
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
