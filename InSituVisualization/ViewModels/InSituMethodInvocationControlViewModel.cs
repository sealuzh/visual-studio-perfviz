using System;
using System.Windows.Input;
using System.Windows.Media;
using DryIoc;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class InSituMethodInvocationControlViewModel : ViewModelBase
    {
        public InSituMethodInvocationControlViewModel(MethodPerformanceInfo methodPerformanceInfo)
        {
            MethodPerformanceInfo = methodPerformanceInfo ?? throw new ArgumentNullException(nameof(methodPerformanceInfo));
            OpenDetailViewCommand = new RelayCommand<object>(obj => OnOpenDetailViewCommand());
        }

        public ICommand OpenDetailViewCommand { get; }

        public MethodPerformanceInfo MethodPerformanceInfo { get; }

        private TimeSpan MeanExecutionTime => MethodPerformanceInfo.MethodPerformanceData.MeanExecutionTime;

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
                if (MeanExecutionTime < TimeSpan.FromMilliseconds(20))
                {
                    return Colors.GreenYellow;
                }
                if (MeanExecutionTime < TimeSpan.FromMilliseconds(30))
                {
                    return Colors.ForestGreen;
                }
                if (MeanExecutionTime < TimeSpan.FromMilliseconds(60))
                {
                    return Colors.DarkGreen;
                }
                if (MeanExecutionTime < TimeSpan.FromMilliseconds(70))
                {
                    return Colors.Orange;
                }
                if (MeanExecutionTime < TimeSpan.FromMilliseconds(80))
                {
                    return Colors.DarkOrange;
                }
                if (MeanExecutionTime < TimeSpan.FromMilliseconds(80))
                {
                    return Colors.OrangeRed;
                }
                return Colors.Red;
                //const double hueColorGreen = 120;
                //return new HsvColor((1 - MethodPerformanceInfo.MemberTime) * hueColorGreen, 1, 1);
            }
        }

        public void OnOpenDetailViewCommand()
        {
            var settings = IocHelper.Container.Resolve<Settings>();
            settings.SetDetailWindowContent(MethodPerformanceInfo);
        }

    }
}
