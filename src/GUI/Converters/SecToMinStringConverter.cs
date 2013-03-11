using System;
using System.Globalization;
using System.Windows.Data;

namespace Intems.SunPoint.Converters
{
    class SecToMinStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int seconds = (int)value;

            int min = seconds / 60;
            var minStr = (min > 9) ? "{0}" : "0{0}";
            return String.Format(minStr, min);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
