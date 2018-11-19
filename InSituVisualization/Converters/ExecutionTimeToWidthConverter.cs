using System;
using System.Windows.Data;

namespace InSituVisualization.Converters
{
    internal class ExecutionTimeToWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            var timeSpan = (TimeSpan)value;
            if (timeSpan == TimeSpan.MinValue)
            {
                return 0;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(20))
            {
                return 10;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(50))
            {
                return 20;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(100))
            {
                return 30;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(300))
            {
                return 40;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(600))
            {
                return 50;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(1000))
            {
                return 58;
            }
            return 60;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
