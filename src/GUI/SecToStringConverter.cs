using System;
using System.Globalization;
using System.Windows.Data;

namespace Intems.SunPoint
{
    public class SecToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int seconds = (int) value;

            int min = seconds / 60;
            int sec = seconds % 60;

            var minStr = (min > 9) ? "{0}" : "0{0}";
            var secStr = (sec < 10) ? "0{1}" : "{1}";
            return String.Format(minStr + ":" + secStr, min, sec);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
