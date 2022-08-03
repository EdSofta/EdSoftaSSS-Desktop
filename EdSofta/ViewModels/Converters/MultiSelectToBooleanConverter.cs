using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.ViewModels.Converters
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class MultiSelectToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var topicList = ((ListView)value).ItemsSource as ObservableCollection<TopicViewModel>;
            var topicList = value as ObservableCollection<TopicViewModel>;
            if (topicList == null) return false;
            var exists = !topicList.ToList().Exists(x => !x.IsSelected);
            return exists;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            if (value == null) return new ObservableCollection<TopicViewModel>();
            return new ObservableCollection<TopicViewModel>();
            
        }
    }
}
