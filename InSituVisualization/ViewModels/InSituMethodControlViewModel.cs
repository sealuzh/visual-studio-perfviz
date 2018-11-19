using System;
using System.ComponentModel;
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
