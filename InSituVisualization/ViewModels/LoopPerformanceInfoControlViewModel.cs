using System;
using System.Windows.Input;
using System.Windows.Media;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    internal class LoopPerformanceInfoControlViewModel: ViewModelBase
    {

        public LoopPerformanceInfoControlViewModel(LoopPerformanceInfo loopPerformanceInfo)
        {
            LoopPerformanceInfo = loopPerformanceInfo;
            OpenDetailViewCommand = new RelayCommand<object>(obj => OnOpenDetailViewCommand());
        }

        public LoopPerformanceInfo LoopPerformanceInfo { get; }

        public ICommand OpenDetailViewCommand { get; }



        public void OnOpenDetailViewCommand()
        {
            Settings.PerformanceInfoDetailWindow.ShowLoopPerformance(LoopPerformanceInfo);
        }

        /// <summary>
        /// Using HSV Values to get a nice transition:
        /// Hue: 0 ° = Red
        /// Hue: 120 ° = Green
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                var sum = LoopPerformanceInfo.SumOfMethodInvocations;
                // TODO RR:
                if (sum < TimeSpan.FromMilliseconds(20))
                {
                    return Colors.GreenYellow;
                }
                if (sum < TimeSpan.FromMilliseconds(30))
                {
                    return Colors.ForestGreen;
                }
                if (sum < TimeSpan.FromMilliseconds(60))
                {
                    return Colors.DarkGreen;
                }
                if (sum < TimeSpan.FromMilliseconds(70))
                {
                    return Colors.Orange;
                }
                if (sum < TimeSpan.FromMilliseconds(80))
                {
                    return Colors.DarkOrange;
                }
                if (sum < TimeSpan.FromMilliseconds(80))
                {
                    return Colors.OrangeRed;
                }
                return Colors.Red;
                //const double hueColorGreen = 120;
                //return new HsvColor((1 - MethodPerformanceInfo.MemberTime) * hueColorGreen, 1, 1);
            }
        }
    }
}
