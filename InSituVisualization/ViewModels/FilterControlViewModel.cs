using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InSituVisualization.Filter;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class FilterControlViewModel : ViewModelBase
    {

        public class FilterKindWrapper
        {
            public string Name { get; set; }
            internal FilterKind FilterKind { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        private string _filterText;

        // TODO RR Fix types ... remove wrapper
        public static IReadOnlyList<string> AvailableFilterCriteria { get; } = new List<string>
        {
            "Request Date",
            "Request City",
            "Duration (ms)",
        };

        public static IReadOnlyList<FilterKindWrapper> AvailableFilterKinds { get; } = new List<FilterKindWrapper>
        {
            new FilterKindWrapper{ Name = "=", FilterKind = FilterKind.IsEqual },
            new FilterKindWrapper{ Name = ">=", FilterKind = FilterKind.IsGreaterEqualThen },
            new FilterKindWrapper{ Name = "<=", FilterKind = FilterKind.IsSmallerEqualThen },
            new FilterKindWrapper{ Name = "Contains", FilterKind = FilterKind.Contains },
        };




        public FilterKindWrapper SelectedFilterKind { get; set; }

        public string SelectedFilterCriteria { get; set; }


        public string FilterText
        {
            get => _filterText;
            set => SetProperty(ref _filterText, value);
        }

        public IFilter GetFilter()
        {
            // TODO RR: Rework
            switch (SelectedFilterCriteria)
            {
                case "Request Date":
                    if (DateTime.TryParse(FilterText, out var dateTime))
                    {
                        return new ComparableFilter<DateTime>(telemetry => telemetry.Timestamp, dateTime) { FilterKind = SelectedFilterKind.FilterKind };
                    }
                    break;
                case "Request City":
                    return new StringFilter(telemetry => telemetry.ClientData.City, FilterText) { FilterKind = SelectedFilterKind.FilterKind };
                case "Duration (ms)":
                    if (double.TryParse(FilterText, out var parsedInt))
                    {
                        // TODO RR Remove cast
                        return new ComparableFilter<double>(telemetry => ((RecordedExecutionTimeMethodTelemetry)telemetry).Duration.TotalMilliseconds, parsedInt);
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

    }
}
