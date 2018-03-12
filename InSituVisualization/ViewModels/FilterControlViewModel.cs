using System;
using System.Collections.ObjectModel;
using InSituVisualization.Filter;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class FilterControlViewModel : ViewModelBase
    {
        public class FilterCriteriaWrapper
        {
            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

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
        public static ObservableCollection<FilterCriteriaWrapper> AvailableFilterCriteria { get; } = new ObservableCollection<FilterCriteriaWrapper>
        {
            new FilterCriteriaWrapper{ Name = "Request Date"},
            new FilterCriteriaWrapper{ Name = "Request City"},
            new FilterCriteriaWrapper{ Name = "Duration (ms)"},
        };

        public static ObservableCollection<FilterKindWrapper> AvailableFilterKinds { get; } = new ObservableCollection<FilterKindWrapper>
        {
            new FilterKindWrapper{ Name = "=", FilterKind = FilterKind.IsEqual },
            new FilterKindWrapper{ Name = ">=", FilterKind = FilterKind.IsGreaterEqualThen },
            new FilterKindWrapper{ Name = "<=", FilterKind = FilterKind.IsSmallerEqualThen },
            new FilterKindWrapper{ Name = "Contains", FilterKind = FilterKind.Contains },
        };




        public FilterKindWrapper SelectedFilterKind { get; set; }

        public FilterCriteriaWrapper SelectedFilterCriteria { get; set; }


        public string FilterText
        {
            get => _filterText;
            set => SetProperty(ref _filterText, value);
        }

        public IFilter GetFilter()
        {
            // TODO RR: Rework
            switch (AvailableFilterCriteria.IndexOf(SelectedFilterCriteria))
            {
                case 0:
                    if (DateTime.TryParse(FilterText, out var dateTime))
                    {
                        return new ComparableFilter<DateTime>(telemetry => telemetry.Timestamp, dateTime) { FilterKind = SelectedFilterKind.FilterKind };
                    }
                    break;
                case 1:
                    return new StringFilter(telemetry => telemetry.ClientData.City, FilterText) { FilterKind = SelectedFilterKind.FilterKind };
                case 2:
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
