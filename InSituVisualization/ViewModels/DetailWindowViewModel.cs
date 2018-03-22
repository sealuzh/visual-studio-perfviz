using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class DetailWindowViewModel : ViewModelBase
    {
        private PerformanceInfo _performanceInfo;
        private bool _showFilteredExecutionTimes;
        private bool _showExecutionTimes;

        public PerformanceInfo PerformanceInfo
        {
            get => _performanceInfo;
            set => SetProperty(ref _performanceInfo, value);
        }

        public bool ShowFilteredExecutionTimes
        {
            get => _showFilteredExecutionTimes;
            set => SetProperty(ref _showFilteredExecutionTimes, value);
        }

        public bool ShowExecutionTimes
        {
            get => _showExecutionTimes;
            set => SetProperty(ref _showExecutionTimes, value);
        }
    }
}
