using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DryIoc;
using InSituVisualization.Converters;
using InSituVisualization.Model;

namespace InSituVisualization.ViewModels
{
    public class InSituMethodControlViewModel : ViewModelBase
    {
        public InSituMethodControlViewModel(MethodPerformanceInfo methodPerformanceInfo)
        {
            MethodPerformanceInfo = methodPerformanceInfo ?? throw new ArgumentNullException(nameof(methodPerformanceInfo));
            OpenDetailViewCommand = new RelayCommand<object>(obj => OnOpenDetailViewCommand());

            MethodPerformanceInfo.PropertyChanged += MethodPerformanceInfoPropertyChanged;
        }

        public ICommand OpenDetailViewCommand { get; }

        public MethodPerformanceInfo MethodPerformanceInfo { get; }

        /// <summary>
        /// The last invocations in points
        /// </summary>
        public PointCollection Points
        {
            get
            {
                var points = new PointCollection();
                var executionTimeMethodTelemetries = MethodPerformanceInfo.MethodPerformanceData.FilteredExecutionTimes.OrderByDescending(o => o.Timestamp).Take(30);
                var i = 0;
                foreach (var timeSpan in executionTimeMethodTelemetries)
                {
                    points.Add(new Point(i++ * 2, timeSpan.Duration.TotalMilliseconds));
                }
                return points;
            }
        }

        private IValueConverter TimeSpanToColoConverter { get; } = new TimeSpanToColorConverter();

        // ReSharper disable once PossibleNullReferenceException
        public Color BackgroundColor => (Color)TimeSpanToColoConverter.Convert(MethodPerformanceInfo.ExecutionTime, typeof(DateTime), null, null);

        public void OnOpenDetailViewCommand()
        {
            var settings = IocHelper.Container.Resolve<Settings>();
            settings.SetDetailWindowContent(MethodPerformanceInfo);
        }

        private void MethodPerformanceInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MethodPerformanceInfo.ExecutionTime):
                    OnPropertyChanged(nameof(BackgroundColor));
                    break;
            }
        }

    }
}
