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

        private String _filterString;
        private PropertyInfo _property;
        private FilterType _filterType;

        public StringFilter(PropertyInfo property, FilterType filterType, string filterString)
        {
            _filterString = filterString;
            _property = property;
            _filterType = filterType;
        }

        public enum FilterType
        {
            IsEqual, Contains
        }

        public Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> ApplyFilter(Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> inDictionary)
        {
            Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>> outDictionary = new Dictionary<string, Dictionary<string, ConcreteMemberTelemetry>>(inDictionary);
            List<String> toRemoveMethod = new List<String>();
            foreach (KeyValuePair<String, Dictionary<String, ConcreteMemberTelemetry>> kvpMethod in inDictionary)
            {
                List<String> toRemove = new List<string>();
                foreach (KeyValuePair<String, ConcreteMemberTelemetry> kvpMember in inDictionary[kvpMethod.Key])
                {
                    String memberName = (String)_property.GetValue(kvpMember.Value);
                    switch (_filterType)
                    {
                        case FilterType.IsEqual:
                            if (!memberName.Equals(_filterString))
                            {
                                toRemove.Add(kvpMember.Key);
                            }
                            break;
                        case FilterType.Contains:
                            if (!memberName.Contains(_filterString))
                            {
                                toRemove.Add(kvpMember.Key);
                            }
                            break;
                    }
                }
                foreach (String removeKey in toRemove)
                {
                    outDictionary[kvpMethod.Key].Remove(removeKey);
                }
                //check whether db on method level is empty --> remove
                if (outDictionary[kvpMethod.Key].Count <= 0)
                {
                    toRemoveMethod.Add(kvpMethod.Key);
                }
            }
            foreach (String removeKey in toRemoveMethod)
            {
                outDictionary.Remove(removeKey);
            }
            return outDictionary;

        }
    }
}
