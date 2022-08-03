using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.ViewModels.Converters
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class MultiObjectsToValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            var key = (string) values[0];
            var radio = values[1] as RadioButton;
            if (radio != null)
            {
                var questionBank = radio.GetParentOfType<StackPanel>().DataContext as QuestionBank;
                if (questionBank == null) return false;
                var question = questionBank.currentQuestion;
                return question.IsAnswered && key == question.SelectedAnswer;
            }

            var canvas = values[1] as Canvas;
            if (canvas != null)
            {
                var questionBank = canvas.GetParentOfType<StackPanel>().DataContext as QuestionBank;
                if (questionBank == null) return false;
                var question = questionBank.currentQuestion;
                return question.IsAnswered && key == question.SelectedAnswer;
            }

            return false;


        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return new object[] { };
        }
    }
}
