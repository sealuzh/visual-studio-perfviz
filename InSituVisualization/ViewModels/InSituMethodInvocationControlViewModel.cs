using System;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using DryIoc;
using InSituVisualization.Converters;
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

        private IValueConverter TimeSpanToColoConverter { get; } = new TimeSpanToColorConverter();

        // ReSharper disable once PossibleNullReferenceException
        public Color BackgroundColor => (Color)TimeSpanToColoConverter.Convert(MeanExecutionTime, typeof(DateTime), null, null);

        public void OnOpenDetailViewCommand()
        {
            var settings = IocHelper.Container.Resolve<Settings>();
            settings.SetDetailWindowContent(MethodPerformanceInfo);
        }

    }
}
