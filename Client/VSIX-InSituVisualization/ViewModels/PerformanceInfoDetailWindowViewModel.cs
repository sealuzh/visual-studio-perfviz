namespace VSIX_InSituVisualization.ViewModels
{
    public class PerformanceInfoDetailWindowViewModel : ViewModelBase
    {
        private MethodPerformanceInfo _methodPerformanceInfo;

        public MethodPerformanceInfo MethodPerformanceInfo
        {
            get => _methodPerformanceInfo;
            set
            {
                SetProperty(ref _methodPerformanceInfo, value);
                OnPropertyChanged(nameof(MemberName));
            }
        }

        public string MemberName => MethodPerformanceInfo?.MethodName;

    }
}
