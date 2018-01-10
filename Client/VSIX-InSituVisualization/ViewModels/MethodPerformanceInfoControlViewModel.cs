using System;
using System.Windows.Input;
using System.Windows.Media;

namespace VSIX_InSituVisualization.ViewModels
{
    public class MethodPerformanceInfoControlViewModel : ViewModelBase
    {
        public MethodPerformanceInfoControlViewModel(MethodPerformanceInfo methodPerformanceInfo)
        {
            MethodPerformanceInfo = methodPerformanceInfo ?? throw new ArgumentNullException(nameof(methodPerformanceInfo));
            OpenDetailViewCommand = new RelayCommand<object>(obj => OnOpenDetailViewCommand());
        }

        public ICommand OpenDetailViewCommand { get; }

        public MethodPerformanceInfo MethodPerformanceInfo { get; }

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
                //return new HsvColor((1 - MethodPerformanceInfo.MemberTime) * hueColorGreen, 1, 1);
            }
        }

        public void OnOpenDetailViewCommand()
        {
            Settings.PerformanceInfoDetailWindowViewModel.MethodPerformanceInfo = MethodPerformanceInfo;
        }

        public bool ShowFirstCallerArrow => MethodPerformanceInfo?.CallerPerformanceInfo.Count >= 1;
        public bool ShowSecondCallerArrow => MethodPerformanceInfo?.CallerPerformanceInfo.Count >= 2;

        public bool ShowFirstCalleeArrow => MethodPerformanceInfo?.CalleePerformanceInfo.Count >= 1;
        public bool ShowSecondCalleeArrow => MethodPerformanceInfo?.CalleePerformanceInfo.Count >= 2;

    }
}
