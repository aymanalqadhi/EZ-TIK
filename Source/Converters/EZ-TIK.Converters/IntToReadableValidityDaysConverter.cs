using System;
using System.Globalization;
using System.Windows.Data;

namespace EZ_TIK.Converters
{
    public class IntToReadableValidityDaysConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value != null ? (int.Parse(value.ToString()) >= 0 ? $"{value} Days" : "Unlimited") : "Unset";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
