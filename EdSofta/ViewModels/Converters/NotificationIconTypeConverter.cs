using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using EdSofta.Enums;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.Converters
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class NotificationIconTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (NotificationType) value;
            return type == NotificationType.Update ? "Info" : "New";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
