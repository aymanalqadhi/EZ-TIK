using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Data;
using EZ_TIK.Parsers;

namespace EZ_TIK.Converters
{
    public class MikrotikTimeToReadableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = MikrotikTime.Parse(value?.ToString());
            if (time == null || value.ToString().Trim() == "00:00:00") return parameter != null ? "Not Set" : "Unlimited";

            var readable = new StringBuilder();
            
            // TODO:
            // Do culture stuff

            // Build a readable string
            if (time.Days > 0) readable.Append($"{time.Days} Days");
            if (time.Hours > 0) readable.Append((time.Days > 0 ? ", " : string.Empty) + $"{time.Hours} Hours");
            if (time.Mintues > 0) readable.Append((time.Hours > 0 ? ", " : string.Empty) + $"{time.Mintues} Mintes");

            // return the built value
            return readable.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
