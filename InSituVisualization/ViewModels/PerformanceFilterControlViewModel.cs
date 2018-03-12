using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using InSituVisualization.Annotations;
using InSituVisualization.Filter;

namespace InSituVisualization.ViewModels
{
    public class PerformanceFilterControlViewModel : ViewModelBase
    {
        private readonly IFilterController _filterController;
        private int _selectedFilterControlIndex;

        public ObservableCollection<FilterControlViewModel> EnabledFilters { get; } = new ObservableCollection<FilterControlViewModel> { new FilterControlViewModel() };

        public PerformanceFilterControlViewModel([NotNull] IFilterController filterController)
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
            if (EnabledFilters.Count > 1)
            {
                EnabledFilters.RemoveAt(selected);
            }
        }

        private void OnAddFilterCommand()
        {
            EnabledFilters.Insert(SelectedFilterControlIndex, new FilterControlViewModel());
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
            // TODO RR:
        }
    }
}
