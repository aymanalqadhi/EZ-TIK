using System;
using System.Globalization;
using System.Windows.Data;

namespace EZ_TIK.Converters
{
    public class MinusIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val, param = 0;

            if (!int.TryParse(value?.ToString(), out val)) return value;
            int.TryParse(parameter?.ToString(), out param);

            return val - param;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
