using System;
using System.Collections.Concurrent;
using InSituVisualization.TelemetryCollector.Filter.Property;

namespace InSituVisualization.TelemetryCollector.Filter
{
    public class DateTimeFilter : Filter
    {
        protected readonly DateTime FilterString;
        protected readonly DateTimeFilterProperty FilterProperty;

        public DateTimeFilter(IFilterProperty filterProperty, DateTime filterString, FilterKind filterKind) : base(filterKind)
        {
            FilterString = filterString;
            FilterProperty = (DateTimeFilterProperty)filterProperty;
        }

        public override ConcurrentDictionary<string, T> ApplyFilter<T>(ConcurrentDictionary<string, T> inDictionary)
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
