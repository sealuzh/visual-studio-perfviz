using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace VSIX_InSituVisualization.TelemetryCollector
{
    class StringFilter : IFilter
    {

        private readonly String _filterString;
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

        public Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> ApplyFilter(Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> inDictionary)
        {
            Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> outDictionary = new Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>>(inDictionary);
            List<String> toRemoveMethodKeys = new List<String>();
            foreach (KeyValuePair<String, Dictionary<String, ConcreteMemberTelemetry>> kvpMethod in inDictionary)
            {
                List<String> toRemoveMemberKeys = new List<string>();
                foreach (KeyValuePair<String, ConcreteMemberTelemetry> kvpMember in inDictionary[kvpMethod.Key])
                {
                    String memberPropertyValue = (String)_property.GetValue(kvpMember.Value);
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
