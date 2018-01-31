using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InSituVisualization.TelemetryCollector.Filter.Property;

namespace InSituVisualization.TelemetryCollector.Filter
{
    public abstract class Filter : IFilter
    {
        protected readonly FilterKind FilterKind;
        protected readonly bool IsGlobalFilter;
        protected readonly string ToFilterMethodFullName;

        protected Filter(FilterKind filterKind, bool isGlobalFilter)
        {
            FilterKind = filterKind;
            IsGlobalFilter = isGlobalFilter;
        }

        protected Filter(FilterKind filterKind, bool isGlobalFilter, string toFilterMethodFullName)
        {
            FilterKind = filterKind;
            IsGlobalFilter = isGlobalFilter;
            ToFilterMethodFullName = toFilterMethodFullName;
        }

        public ConcurrentDictionary<string, ConcurrentDictionary<string, T>> ApplyFilter<T>(ConcurrentDictionary<string, ConcurrentDictionary<string, T>> inDictionary)
        {
            var outDictionary = new ConcurrentDictionary<string, ConcurrentDictionary<string, T>>();
            foreach (var kvpMethod in inDictionary)
            {
                if (IsGlobalFilter)
                {
                    if (!outDictionary.TryAdd(kvpMethod.Key,
                        ApplyFilterMethodLevel(kvpMethod.Key, inDictionary[kvpMethod.Key])))
                    {
                        Console.WriteLine("Could not add element " + inDictionary[kvpMethod.Key]);
                    }
                }
                else
                {
                    if (kvpMethod.Key != ToFilterMethodFullName)
                    {
                        if (!outDictionary.TryAdd(kvpMethod.Key, new ConcurrentDictionary<string, T>(kvpMethod.Value)))
                        {
                            Console.WriteLine("Could not add element " + kvpMethod.Key);
                        }
                    }
                    else
                    {
                        if (!outDictionary.TryAdd(kvpMethod.Key,
                            ApplyFilterMethodLevel(kvpMethod.Key, inDictionary[kvpMethod.Key])))
                        {
                            Console.WriteLine("Could not add element " + inDictionary[kvpMethod.Key]);
                        }
                    }
                }
            }
            return outDictionary;
        }

        protected abstract ConcurrentDictionary<string, T> ApplyFilterMethodLevel<T>(string kvpMethodKey,ConcurrentDictionary<string, T> inDictionary);


    }
}
