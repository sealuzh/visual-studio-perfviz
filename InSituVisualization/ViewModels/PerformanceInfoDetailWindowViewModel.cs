using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class PerformanceInfoDetailWindowViewModel : ViewModelBase , IPerformanceInfoDetailWindow
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

        public void ShowMethodPerformance(MethodPerformanceInfo methodPerformanceInfo)
        {
            MethodPerformanceInfo = methodPerformanceInfo;
        }

        public void ShowLoopPerformance(LoopPerformanceInfo loopPerformanceinfo)
        {
            // TODO RR:
            throw new System.NotImplementedException();
        }
    }
}
