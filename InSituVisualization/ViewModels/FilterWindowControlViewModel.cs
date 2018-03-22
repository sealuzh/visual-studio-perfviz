using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using InSituVisualization.Annotations;
using InSituVisualization.Filter;

namespace InSituVisualization.ViewModels
{
    public class FilterWindowControlViewModel : ViewModelBase
    {
        private readonly IFilterController _filterController;

        public ObservableCollection<FilterControlViewModel> EnabledFilters { get; } = new ObservableCollection<FilterControlViewModel> { new FilterControlViewModel() };

        public FilterWindowControlViewModel([NotNull] IFilterController filterController)
        {
            _filterController = filterController ?? throw new ArgumentNullException(nameof(filterController));
            ApplyFiltersCommand = new RelayCommand<object>(obj => OnApplyFiltersCommand());
            AddFilterCommand = new RelayCommand<FilterControlViewModel>(OnAddFilterCommand);
            RemoveFilterCommand = new RelayCommand<FilterControlViewModel>(OnRemoveFilterCommand);
        }

        /// <summary>
        /// TODO RR: Insert and remove at clicked index...
        /// </summary>
        private void OnRemoveFilterCommand(FilterControlViewModel filterControlViewModel)
        {
            EnabledFilters.Remove(filterControlViewModel);
            if (EnabledFilters.Count <= 0)
            {
                EnabledFilters.Add(new FilterControlViewModel());
            }
        }

        private void OnAddFilterCommand(FilterControlViewModel filterControlViewModel)
        {
            EnabledFilters.Insert(EnabledFilters.IndexOf(filterControlViewModel) + 1, new FilterControlViewModel());
        }

        public ICommand ApplyFiltersCommand { get; }

        public ICommand AddFilterCommand { get; }

        public ICommand RemoveFilterCommand { get; }

        private void OnApplyFiltersCommand()
        {
            // TODO RR Rework:

            _filterController.Filters.Clear();
            foreach (var filterControlViewModel in EnabledFilters)
            {
                var filter = filterControlViewModel.GetFilter();
                if (filter != null)
                {
                    _filterController.Filters.Add(filter);
                }
            }
        }
    }
}
