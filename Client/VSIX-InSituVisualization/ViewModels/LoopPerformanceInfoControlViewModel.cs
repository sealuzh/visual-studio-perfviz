using System;
using System.Collections.Generic;
using System.Windows.Media;
using VSIX_InSituVisualization.Model;
using VSIX_InSituVisualization.Utils;

namespace VSIX_InSituVisualization.ViewModels
{
    internal class LoopPerformanceInfoControlViewModel: ViewModelBase
    {
        private readonly MethodPerformanceInfo _methodPerformanceInfo;

        public LoopPerformanceInfoControlViewModel(MethodPerformanceInfo methodPerformanceInfo, IList<MethodPerformanceInfo> methodInvocationsPerformanceInfos)
        {
            _methodPerformanceInfo = methodPerformanceInfo ?? throw new ArgumentNullException(nameof(methodPerformanceInfo));
            MethodInvocationsPerformanceInfos = methodInvocationsPerformanceInfos ?? throw new ArgumentNullException(nameof(methodInvocationsPerformanceInfos));
        }

        public IList<MethodPerformanceInfo> MethodInvocationsPerformanceInfos { get; }

        public TimeSpan SumOfMethodInvocations
        {
            get { return MethodInvocationsPerformanceInfos.Sum(p => p.MeanExecutionTime); }
        }

        public int AverageLoopIterations => SumOfMethodInvocations.Milliseconds == 0 ? 0 : _methodPerformanceInfo.MeanExecutionTime.Milliseconds / SumOfMethodInvocations.Milliseconds;

        /// <summary>
        /// Using HSV Values to get a nice transition:
        /// Hue: 0 ° = Red
        /// Hue: 120 ° = Green
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                var sum = SumOfMethodInvocations;
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
