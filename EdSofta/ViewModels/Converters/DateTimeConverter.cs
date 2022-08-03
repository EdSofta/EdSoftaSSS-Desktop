using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.Converters
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class DateTimeConverter : IValueConverter    
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DateTime.Now;

            var dateTime = (DateTime) value;

            if (dateTime == DateTime.MinValue) return "Never";

            var days = DateTime.Now.Subtract(dateTime).Days;
            switch (days)
            {
                case 0:
                    return $"Today, {dateTime.ToShortTimeString()}";
                case 1:
                    return $"Yesterday, {dateTime.ToShortTimeString()}";
                default:
                    return days > 1 && days < 10 ? $"{days} days ago, {dateTime.ToShortTimeString()}" : dateTime.ToLongDateString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
