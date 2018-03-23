using System;
using System.Windows.Data;
using System.Windows.Media;

namespace InSituVisualization.Converters
{
    public class TimeSpanToColorConverter : IValueConverter
    {

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
            if (timeSpan < TimeSpan.FromMilliseconds(20))
            {
                return Colors.GreenYellow;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(50))
            {
                return Colors.ForestGreen;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(100))
            {
                return Colors.DarkGreen;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(300))
            {
                return Colors.Orange;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(600))
            {
                return Colors.DarkOrange;
            }
            if (timeSpan < TimeSpan.FromMilliseconds(1000))
            {
                return Colors.OrangeRed;
            }
            return Colors.Red;
            //const double hueColorGreen = 120;
            //return new HsvColor((1 - timeSpan) * hueColorGreen, 1, 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
