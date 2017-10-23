namespace VSIX_InSituVisualization.ViewModels
{
    public class PerformanceInfoDetailWindowViewModel : ViewModelBase
    {
        private PerformanceInfo _performanceInfo;

        public PerformanceInfo PerformanceInfo
        {
            get => _performanceInfo;
            set
            {
                SetProperty(ref _performanceInfo, value);
                OnPropertyChanged(nameof(MemberName));
            }
        }

        public string MemberName => PerformanceInfo?.IdentifierName;

    }
}
