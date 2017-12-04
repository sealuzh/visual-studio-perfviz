using System.Collections.Generic;
using System.Reflection;

namespace VSIX_InSituVisualization.TelemetryCollector
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

        public IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> ApplyFilter(IDictionary<string, IDictionary<string, ConcreteMemberTelemetry>> inDictionary)
        {
            var outDictionary = new Dictionary<string, IDictionary<string, ConcreteMemberTelemetry>>(inDictionary);
            var toRemoveMethodKeys = new List<string>();
            foreach (var kvpMethod in inDictionary)
            {
                var toRemoveMemberKeys = new List<string>();
                foreach (var kvpMember in inDictionary[kvpMethod.Key])
                {
                    var memberPropertyValue = (string)_property.GetValue(kvpMember.Value);
                    switch (_stringFilterType)
                    {
                        case StringFilterType.IsEqual:
                            if (!memberPropertyValue.Equals(_filterString))
                            {
                                toRemoveMemberKeys.Add(kvpMember.Key);
                            }
                            break;
                        case StringFilterType.Contains:
                            if (!memberPropertyValue.Contains(_filterString))
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
