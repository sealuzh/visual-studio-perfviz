using System;
using System.Collections.Generic;
using System.Linq;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    internal class StringFilter : Filter
    {
        private readonly Func<RecordedMethodTelemetry, string> _getStringFunc;
        private readonly string _string;

        public StringFilter(FilterKind filterKind, Func<RecordedMethodTelemetry, string> getStringFunc, string @string) : base(filterKind)
        {
            _getStringFunc = getStringFunc ?? throw new ArgumentNullException(nameof(getStringFunc));
            _string = @string;
        }

        public override IEnumerable<RecordedMethodTelemetry> ApplyFilter(IEnumerable<RecordedMethodTelemetry> list)
        {
            switch (FilterKind)
            {
                case FilterKind.None:
                    break;
                case FilterKind.IsEqual:
                    return list.Where(telemetry => _getStringFunc(telemetry) == _string);
                case FilterKind.IsGreaterEqualThen:
                    break;
                case FilterKind.IsSmallerEqualThen:
                    break;
                case FilterKind.Contains:
                    return list.Where(telemetry => _getStringFunc(telemetry).Contains(_string));
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return list;
        }
    }
}
