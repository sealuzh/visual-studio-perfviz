using System;
using System.Collections.Concurrent;
using InSituVisualization.TelemetryCollector.Filter.Property;

namespace InSituVisualization.TelemetryCollector.Filter
{
    public class StringFilter : Filter
    {
        protected readonly string FilterString;
        protected readonly StringFilterProperty FilterProperty;

        public StringFilter(IFilterProperty filterProperty, string filterString, FilterKind filterKind) : base(filterKind)
        {
            FilterString = filterString;
            FilterProperty = (StringFilterProperty)filterProperty;
        }

        public override ConcurrentDictionary<string, T> ApplyFilter<T>(ConcurrentDictionary<string, T> inDictionary)
        {
            var outDictionary = new ConcurrentDictionary<string, T>();
            foreach (var kvpMember in inDictionary)
            {
                var memberPropertyValue = (string)FilterProperty.GetPropertyInfo().GetValue(kvpMember.Value);
                switch (FilterKind)
                {
                    case FilterKind.IsEqual:
                        if (memberPropertyValue.Equals(FilterString))
                        {
                            if (!outDictionary.TryAdd(kvpMember.Key, kvpMember.Value))
                            {
                                Console.WriteLine("Could not add element " + kvpMember.Key);
                            }
                        }
                        break;
                    case FilterKind.Contains:
                        if (!memberPropertyValue.Contains(FilterString))
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
