using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using InSituVisualization.TelemetryCollector.Filter.Property;
using InSituVisualization.TelemetryCollector.Model.ConcreteMember;

namespace InSituVisualization.TelemetryCollector.Filter
{
    public class DateTimeFilter : Filter
    {
        protected readonly DateTime FilterString;
        protected readonly DateTimeFilterProperty FilterProperty;
        
        public DateTimeFilter(IFilterProperty filterProperty, DateTime filterString, FilterKind filterKind) : base(filterKind, true)
        {
            FilterString = filterString;
            FilterProperty = (DateTimeFilterProperty)filterProperty;
        }

        public DateTimeFilter(IFilterProperty filterProperty, DateTime filterString, FilterKind filterKind, string toFilterMethodFullName) : base(filterKind, false, toFilterMethodFullName)
        {
            FilterString = filterString;
            FilterProperty = (DateTimeFilterProperty)filterProperty;
        }

        protected override ConcurrentDictionary<string, T> ApplyFilterMethodLevel<T>(string kvpMethodKey, ConcurrentDictionary<string, T> inDictionary)
        {
            var outDictionary = new ConcurrentDictionary<string, T>();
            foreach (var kvpMember in inDictionary)
            {
                var memberPropertyValue = (DateTime)FilterProperty.GetPropertyInfo().GetValue(kvpMember.Value);
                switch (FilterKind)
                {
                    case FilterKind.IsEqual:
                        if (!memberPropertyValue.Equals(FilterString))
                        {
                            if (!outDictionary.TryAdd(kvpMember.Key, kvpMember.Value))
                            {
                                Console.WriteLine("Could not add element " + kvpMember.Key);
                            }
                        }
                        break;
                    case FilterKind.IsGreaterEqualThen:
                        if (!(memberPropertyValue <= FilterString))
                        {
                            if (!outDictionary.TryAdd(kvpMember.Key, kvpMember.Value))
                            {
                                Console.WriteLine("Could not add element " + kvpMember.Key);
                            }
                        }
                        break;
                    case FilterKind.IsSmallerEqualThen:
                        if (!(memberPropertyValue >= FilterString))
                        {
                            if (!outDictionary.TryAdd(kvpMember.Key, kvpMember.Value))
                            {
                                Console.WriteLine("Could not add element " + kvpMember.Key);
                            }
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
