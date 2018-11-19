using System;
using System.Windows.Data;
using System.Windows.Media.TextFormatting;

namespace InSituVisualization.Converters
{
    internal class ExecutionTimeToWidthConverter : IValueConverter
    {
        private const int MinWidth = 0;
        private const int MaxWidth = 60;

        private static readonly TimeSpan MaxTimeSpan = TimeSpan.FromMilliseconds(1000);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is TimeSpan timeSpan) || timeSpan == TimeSpan.MinValue)
            {
                return MinWidth;
            }
            if (timeSpan > MaxTimeSpan)
            {
                return MaxWidth;
            }
            return System.Convert.ToInt32(timeSpan.TotalMilliseconds / MaxTimeSpan.TotalMilliseconds * MaxWidth);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
