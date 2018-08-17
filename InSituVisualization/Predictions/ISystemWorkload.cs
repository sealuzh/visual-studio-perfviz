namespace InSituVisualization.Predictions
{
    public interface ISystemWorkload
    {
        /// <summary>
        /// The systemWorkload difference to average (- for less, + for more)
        /// </summary>
        /// <example>
        /// e.g. 0.1 => 110 % workload compared to average
        /// </example>
        double PercentageWorkloadDifferenceToAverage { get; set; }
    }
}
