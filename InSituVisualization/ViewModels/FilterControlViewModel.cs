using System;
using System.Collections.ObjectModel;
using InSituVisualization.Filter;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class FilterControlViewModel : ViewModelBase
    {
        public class FilterWrapper
        {
            public string Name { get; set; }
            internal Func<RecordedMethodTelemetry, object> GetObjectFunc { get; set; }
        }

        public class FilterKindWrapper
        {
            public string Name { get; set; }
            internal FilterKind FilterKind { get; set; }
        }

        private string _filterText;


        public static ObservableCollection<FilterWrapper> AvailableFilters { get; } = new ObservableCollection<FilterWrapper>
        {
            new FilterWrapper{ Name = "Request Date", GetObjectFunc = telemetry => telemetry.Timestamp},
            new FilterWrapper{ Name = "Request City", GetObjectFunc = telemetry => telemetry.ClientData.City},
            // TODO RR Fix types ... remove wrapper
            new FilterWrapper{ Name = "Duration (ms)", GetObjectFunc = telemetry => ((RecordedExecutionTimeMethodTelemetry)telemetry).Duration},
        };

        public static ObservableCollection<FilterKindWrapper> AvailableFilterKinds { get; } = new ObservableCollection<FilterKindWrapper>
        {
            new FilterKindWrapper{ Name = "=", FilterKind = FilterKind.IsEqual },
            new FilterKindWrapper{ Name = ">=", FilterKind = FilterKind.IsGreaterEqualThen },
            new FilterKindWrapper{ Name = "<=", FilterKind = FilterKind.IsSmallerEqualThen },
            new FilterKindWrapper{ Name = "Contains", FilterKind = FilterKind.Contains },
        };

        public string FilterText
        {
            get => _filterText;
            set => SetProperty(ref _filterText, value);
        }

        public FilterKindWrapper SelectedFilterKind { get; set; }

        public FilterWrapper SelectedFilterCriteria { get; set; }
    }
}
