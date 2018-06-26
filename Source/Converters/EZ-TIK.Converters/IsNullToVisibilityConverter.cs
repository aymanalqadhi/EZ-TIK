using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EZ_TIK.Converters
{
    public class IsNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value == null;
            return (parameter?.ToString().ToLower() == "true" ? result : !result)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}