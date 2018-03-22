using System;
using System.Collections.Generic;
using System.Linq;
using InSituVisualization.Model;

namespace InSituVisualization.Filter
{
    public class StringFilter : Filter
    {
        private readonly Func<RecordedMethodTelemetry, string> _getStringFunc;
        private readonly string _string;

        public StringFilter(Func<RecordedMethodTelemetry, string> getStringFunc, string @string)
        {
            _getStringFunc = getStringFunc ?? throw new ArgumentNullException(nameof(getStringFunc));
            _string = @string;
        }

        public override IList<T> ApplyFilter<T>(IList<T> list)
        {
            switch (FilterKind)
            {
                case FilterKind.None:
                    break;
                case FilterKind.IsEqual:
                    return list.Where(telemetry => _getStringFunc(telemetry) == _string).ToList();
                case FilterKind.IsGreaterEqualThen:
                    break;
                case FilterKind.IsSmallerEqualThen:
                    break;
                case FilterKind.Contains:
                    return list.Where(telemetry => _getStringFunc(telemetry).Contains(_string)).ToList();
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return list;
        }
    }
}
