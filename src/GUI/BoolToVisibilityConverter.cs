using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Intems.SunPoint
{
    class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueVisibility { get; set; }
        public Visibility FalseVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = (bool) value;
            return isVisible ? TrueVisibility : FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
