using System.Windows.Input;
using DryIoc;
using InSituVisualization.Model;
using InSituVisualization.Utils;

namespace InSituVisualization.ViewModels
{
    internal class LoopPerformanceInfoControlViewModel : ViewModelBase
    {
        private int _averageLoopIterations;

        public LoopPerformanceInfoControlViewModel(LoopPerformanceInfo loopPerformanceInfo)
        {
            LoopPerformanceInfo = loopPerformanceInfo;
            OpenDetailViewCommand = new RelayCommand<object>(obj => OnOpenDetailViewCommand());
            _averageLoopIterations = LoopPerformanceInfo.MeanNumberOfLoopIterations;
        }

        public LoopPerformanceInfo LoopPerformanceInfo { get; }

        public int AverageLoopIterations
        {
            get => _averageLoopIterations;
            set
            {
                var oldAverage = _averageLoopIterations;
                SetProperty(ref _averageLoopIterations, value);
                LoopPerformanceInfo.MethodPerformanceInfo.PredictedMeanExecutionTime =
                    LoopPerformanceInfo.MethodPerformanceInfo.PredictedMeanExecutionTime -
                    (LoopPerformanceInfo.SumOfMethodInvocations.Multiply(oldAverage)) +
                     (LoopPerformanceInfo.SumOfMethodInvocations.Multiply(value));
                LoopPerformanceInfo.MethodPerformanceInfo.HasChanged = true;
            }
        }

        public ICommand OpenDetailViewCommand { get; }

        public void OnOpenDetailViewCommand()
        {
            var settings = IocHelper.Container.Resolve<Settings>();
            settings.PerformanceInfoDetailWindow.ShowLoopPerformance(LoopPerformanceInfo);
        }

    }
}
