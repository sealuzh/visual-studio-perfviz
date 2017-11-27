using System;
using System.Windows.Input;
using System.Windows.Media;

namespace VSIX_InSituVisualization.ViewModels
{
    public class PerfVizViewModel : ViewModelBase
    {
        public PerfVizViewModel(PerformanceInfo performanceInfo)
        {
            PerformanceInfo = performanceInfo ?? throw new ArgumentNullException(nameof(performanceInfo));
            OpenDetailViewCommand = new RelayCommand<object>(obj => OnOpenDetailViewCommand());
        }

        public ICommand OpenDetailViewCommand { get; }

        public PerformanceInfo PerformanceInfo { get; }

        /// <summary>
        /// Using HSV Values to get a nice transition:
        /// Hue: 0 ° = Red
        /// Hue: 120 ° = Green
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                // TODO RR:
                
                // ReSharper disable once ArrangeAccessorOwnerBody
                return Colors.GreenYellow;
                //const double hueColorGreen = 120;
                //return new HsvColor((1 - PerformanceInfo.MemberTime) * hueColorGreen, 1, 1);
            }
        }

        public void OnOpenDetailViewCommand()
        {
            Settings.PerformanceInfoDetailWindowViewModel.PerformanceInfo = PerformanceInfo;
        }

        public bool ShowFirstCallerArrow => PerformanceInfo?.CallerPerformanceInfo.Count >= 1;
        public bool ShowSecondCallerArrow => PerformanceInfo?.CallerPerformanceInfo.Count >= 2;

        public bool ShowFirstCalleeArrow => PerformanceInfo?.CalleePerformanceInfo.Count >= 1;
        public bool ShowSecondCalleeArrow => PerformanceInfo?.CalleePerformanceInfo.Count >= 2;

    }
}
