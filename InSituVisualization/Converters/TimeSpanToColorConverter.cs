using System;
using System.Windows.Data;
using System.Windows.Media;
using InSituVisualization.Utils;

namespace InSituVisualization.Converters
{
    public class TimeSpanToColorConverter : IValueConverter
    {
        private static readonly TimeSpan MaxTimeSpan = TimeSpan.FromMilliseconds(1000);
        /// <inheritdoc />
        /// <summary>
        /// Using HSV Values to get a nice transition:
        /// Hue: 0 ° = Red
        /// Hue: 120 ° = Green
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "Undefined";
            }
            var timeSpan = (TimeSpan)value;
            if (timeSpan == TimeSpan.MinValue)
            {
                return Colors.GreenYellow;
            }
            if (timeSpan > MaxTimeSpan)
            {
                return Colors.Red;
            }
            
            const double hueColorGreen = 120;
            return (Color)new HsvColor((1-timeSpan.TotalMilliseconds / MaxTimeSpan.TotalMilliseconds) * hueColorGreen, 1, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
