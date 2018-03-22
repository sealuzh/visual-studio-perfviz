using System;
using System.Collections.Generic;
using System.Linq;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    /// <summary>
    /// Filter for any Comparable Type
    /// TProperty is the type of the property to compare.
    /// for example Int, DateTime etc.
    /// </summary>
    public class ComparableFilter<TProperty> : Filter where TProperty : IComparable<TProperty>
    {
        private readonly Func<RecordedMethodTelemetry, TProperty> _getIntFunc;
        private readonly TProperty _comparable;

        public ComparableFilter(Func<RecordedMethodTelemetry, TProperty> getComparableFunc, TProperty comparable)
        {
            _getIntFunc = getComparableFunc ?? throw new ArgumentNullException(nameof(getComparableFunc));
            _comparable = comparable;
        }

        public override IList<T> ApplyFilter<T>(IList<T> list)
        {
            switch (FilterKind)
            {
                case FilterKind.None:
                    break;
                case FilterKind.IsEqual:
                    return list.Where(telemetry => _getIntFunc(telemetry).CompareTo(_comparable) == 0).ToList();
                case FilterKind.IsGreaterEqualThen:
                    return list.Where(telemetry => _getIntFunc(telemetry).CompareTo(_comparable) >= 0).ToList();
                case FilterKind.IsSmallerEqualThen:
                    return list.Where(telemetry => _getIntFunc(telemetry).CompareTo(_comparable) <= 0).ToList();
                case FilterKind.Contains:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return list;
        }
    }
}
