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
        private int _selectedFilterControlIndex;

        public ObservableCollection<FilterControlViewModel> EnabledFilters { get; } = new ObservableCollection<FilterControlViewModel> { new FilterControlViewModel() };

        public FilterWindowControlViewModel([NotNull] IFilterController filterController)
        {
            _filterController = filterController ?? throw new ArgumentNullException(nameof(filterController));
            ApplyFiltersCommand = new RelayCommand<object>(obj => OnApplyFiltersCommand());
            AddFilterCommand = new RelayCommand<object>(obj => OnAddFilterCommand());
            RemoveFilterCommand = new RelayCommand<object>(obj => OnRemoveFilterCommand());
        }

        /// <summary>
        /// TODO RR: Insert and remove at clicked index...
        /// </summary>
        private void OnRemoveFilterCommand()
        {
            var selected = SelectedFilterControlIndex;
            EnabledFilters.RemoveAt(selected);
            if (EnabledFilters.Count <= 0)
            {
                EnabledFilters.Add(new FilterControlViewModel());
            }
        }

        private void OnAddFilterCommand()
        {
            EnabledFilters.Insert(SelectedFilterControlIndex+1, new FilterControlViewModel());
        }

        public ICommand ApplyFiltersCommand { get; }

        public ICommand AddFilterCommand { get; }

        public ICommand RemoveFilterCommand { get; }

        public int SelectedFilterControlIndex
        {
            get => _selectedFilterControlIndex;
            set => SetProperty(ref _selectedFilterControlIndex, value);
        }

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
