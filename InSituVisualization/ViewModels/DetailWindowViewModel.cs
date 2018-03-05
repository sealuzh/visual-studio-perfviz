using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class DetailWindowViewModel : ViewModelBase
    {
        private PerformanceInfo _performanceInfo;

        public PerformanceInfo PerformanceInfo
        {
            get => _performanceInfo;
            set => SetProperty(ref _performanceInfo, value);
        }
    }
}
