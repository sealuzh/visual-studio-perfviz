using System;
using System.Collections.Concurrent;
using InSituVisualization.TelemetryCollector.Filter.Property;

namespace InSituVisualization.TelemetryCollector.Filter
{
    public class IntFilter : Filter
    {
        protected readonly int FilterString;
        protected readonly IntFilterProperty FilterProperty;

        public IntFilter(IFilterProperty filterProperty, int filterString, FilterKind filterKind) : base(filterKind)
        {
            FilterString = filterString;
            FilterProperty = (IntFilterProperty)filterProperty;
        }

        public override ConcurrentDictionary<string, T> ApplyFilter<T>(ConcurrentDictionary<string, T> inDictionary)
        {
            var outDictionary = new ConcurrentDictionary<string, T>();
            foreach (var kvpMember in inDictionary)
            {
                var memberPropertyValue = (int)FilterProperty.GetPropertyInfo().GetValue(kvpMember.Value);
                switch (FilterKind)
                {
                    case FilterKind.IsEqual:
                        if (!memberPropertyValue.Equals(FilterString))
                        {
                            outDictionary.TryAdd(kvpMember.Key, kvpMember.Value);
                        }
                        break;
                    case FilterKind.IsGreaterEqualThen:
                        if (!(memberPropertyValue <= FilterString))
                        {
                            outDictionary.TryAdd(kvpMember.Key, kvpMember.Value);
                        }
                        break;
                    case FilterKind.IsSmallerEqualThen:
                        if (!(memberPropertyValue >= FilterString))
                        {
                            outDictionary.TryAdd(kvpMember.Key, kvpMember.Value);
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
