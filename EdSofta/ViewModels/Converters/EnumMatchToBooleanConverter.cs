using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.Converters
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class EnumMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null || parameter == null)
            //    return false;

            //var radioButton = (RadioButton) value;
            //var checkValue = (string)radioButton.Tag;
            //var questionBank = radioButton.GetParentOfType<StackPanel>().DataContext as QuestionBank;
            //if (questionBank == null) return false;
            //var question = questionBank.currentQuestion;
            //return question.IsAnswered && checkValue.Equals(question.selectedAnswer, StringComparison.InvariantCultureIgnoreCase);
            return false;
            //var checkValue = value.ToString().Trim();
            //var targetValue = parameter.ToString().Trim();
            //return checkValue.Equals(targetValue,
            //         StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value == null || parameter == null)
            //    return null;

            //var useValue = (bool)value;
            //var targetValue = parameter.ToString();
            //if (useValue)
            //    return Enum.Parse(targetType, targetValue);

            //return null;
            throw new NotImplementedException();
        }
    }
}
