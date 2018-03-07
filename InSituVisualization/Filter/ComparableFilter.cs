using System;
using System.Collections.Generic;
using System.Linq;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    internal class ComparableFilter<T> : Filter where T : IComparable<T>
    {
        private readonly Func<RecordedMethodTelemetry, T> _getIntFunc;
        private readonly T _comparable;

        public ComparableFilter(FilterKind filterKind, Func<RecordedMethodTelemetry, T> getComparableFunc, T comparable) : base(filterKind)
        {
            _getIntFunc = getComparableFunc ?? throw new ArgumentNullException(nameof(getComparableFunc));
            _comparable = comparable;
        }

        public override IEnumerable<RecordedMethodTelemetry> ApplyFilter(IEnumerable<RecordedMethodTelemetry> list)
        {
            switch (FilterKind)
            {
                case FilterKind.None:
                    break;
                case FilterKind.IsEqual:
                    return list.Where(telemetry => _getIntFunc(telemetry).CompareTo(_comparable) == 0);
                case FilterKind.IsGreaterEqualThen:
                    return list.Where(telemetry => _getIntFunc(telemetry).CompareTo(_comparable) >= 0);
                case FilterKind.IsSmallerEqualThen:
                    return list.Where(telemetry => _getIntFunc(telemetry).CompareTo(_comparable) <= 0);
                case FilterKind.Contains:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return list;
        }
    }
}
