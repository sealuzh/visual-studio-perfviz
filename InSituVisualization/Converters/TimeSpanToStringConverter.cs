using System;
using System.Windows.Data;

namespace InSituVisualization.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "Undefined";
            }
            var timeSpan = (TimeSpan) value;
            if (timeSpan == TimeSpan.MinValue)
            {
                return "0 ms";
            }

            // < 1 Sec
            if (timeSpan.TotalMilliseconds < 1000)
            {
                return $"{timeSpan.TotalMilliseconds} ms";
            }

            // < 2 min
            if (timeSpan.TotalSeconds < 120)
            {
                return $"{timeSpan.Seconds}.{timeSpan.Milliseconds} s";
            }

            // < 1 hour
            if (timeSpan.TotalMinutes < 60)
            {
                return $"{timeSpan.Minutes}.{timeSpan.Seconds} min";
            }

            return $"{timeSpan.Hours}.{timeSpan.Minutes} h";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (TimeSpan.TryParse(value as string, out var resultSpan))
            {
                return resultSpan;
            }
            throw new Exception("Unable to convert string to date time");
        }
    }
}
