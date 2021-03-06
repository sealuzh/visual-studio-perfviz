﻿using System;
using System.Collections.Generic;
using InSituVisualization.Filter;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class FilterControlViewModel : ViewModelBase
    {

        private static class FilterCriteria
        {
            public const string RequestDate = "Request Date";
            public const string ClientCity = "Client City";
            public const string ClientCountry = "Client Country";
            public const string ClientOs = "Client OS";
            public const string ClientIp = "Client IP";
            public const string Duration = "Duration (ms)";
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
        public static IReadOnlyList<string> AvailableFilterCriteria { get; } = new List<string>
        {
            FilterCriteria.RequestDate,
            FilterCriteria.ClientCity,
            FilterCriteria.ClientCountry,
            FilterCriteria.ClientOs,
            FilterCriteria.ClientIp,
            FilterCriteria.Duration,
        };

        public static IReadOnlyList<FilterKindWrapper> AvailableFilterKinds { get; } = new List<FilterKindWrapper>
        {
            new FilterKindWrapper{ Name = "==", FilterKind = FilterKind.IsEqual },
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
                case FilterCriteria.RequestDate:
                    if (DateTime.TryParse(FilterText, out var dateTime))
                    {
                        return new ComparableFilter<DateTime>(telemetry => telemetry.Timestamp, dateTime) { FilterKind = SelectedFilterKind.FilterKind };
                    }
                    break;
                case FilterCriteria.ClientCity:
                    return new StringFilter(telemetry => telemetry.ClientData.City, FilterText) { FilterKind = SelectedFilterKind.FilterKind };
                case FilterCriteria.ClientCountry:
                    return new StringFilter(telemetry => telemetry.ClientData.CountryOrRegion, FilterText) { FilterKind = SelectedFilterKind.FilterKind };
                case FilterCriteria.ClientOs:
                    return new StringFilter(telemetry => telemetry.ClientData.Os, FilterText) { FilterKind = SelectedFilterKind.FilterKind };
                case FilterCriteria.ClientIp:
                    return new StringFilter(telemetry => telemetry.ClientData.Ip, FilterText) { FilterKind = SelectedFilterKind.FilterKind };
                case FilterCriteria.Duration:
                    if (double.TryParse(FilterText, out var parsedInt))
                    {
                        // TODO RR Remove cast
                        return new ComparableFilter<double>(telemetry => ((RecordedExecutionTimeMethodTelemetry)telemetry).Duration.TotalMilliseconds, parsedInt) { FilterKind = SelectedFilterKind.FilterKind };
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

    }
}
