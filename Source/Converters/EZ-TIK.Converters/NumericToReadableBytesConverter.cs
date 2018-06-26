using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using EZ_TIK.Parsers;

namespace EZ_TIK.Converters
{
    public class NumericToReadableBytesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !Regex.IsMatch(value.ToString(), @"^\d+$")) return "0 B";

            return ByteSize.FromBytes(long.Parse(value.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
