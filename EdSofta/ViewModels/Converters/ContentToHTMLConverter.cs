using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace EdSofta.ViewModels.Converters
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class ContentToHTMLConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //var content = (string)value;
            //var htmlString = $@" <html><body> {(string)value} </body></html>";
            //return htmlString;
            return (FlowDocument)System.Windows.Markup.XamlReader.Parse((string)value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
