using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NCalc;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class CalculatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private string inputField { get; set; }

        public string InputField
        {
            get { return inputField; }
            set
            {
                inputField = value;
                OnPropertyChanged("InputField");
            }
        }

        private string outputField { get; set; }

        public string OutputField
        {
            get { return outputField; }
            set
            {
                outputField = value;
                OnPropertyChanged("OutputField");
            }
        }


        Operator opr = new Operator();

        public void deleteInput()
        {
            try
            {
                InputField = InputField.Substring(0, InputField.Length - 1);
                var last = InputField.Substring(InputField.Length - 1);
                if (opr.isOperator(last)) return;
                var expression = new Expression(InputField);
                OutputField = expression.Evaluate().ToString();
            }
            catch
            {
                OutputField = string.Empty;
            }
        }

        public void displayResult()
        {
            try
            {
                if(!opr.containsOperator(InputField)) return;

                var last = InputField.Substring(InputField.Length - 1);
                if (opr.isOperator(last)) return;

                var expression = new Expression(InputField);
                InputField = expression.Evaluate().ToString();
                OutputField = string.Empty;
            }
            catch
            {
                OutputField = "Bad expression";
            }
        }

        public void evaluate(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(InputField))
                {
                    OutputField = string.Empty;
                    //return;
                }

                //if first input is not -
                //if(opr.isUnary(InputField.Substring(0, 1));

                InputField = $"{InputField}{value}";
                var expression = new Expression(InputField);
                OutputField = string.Empty;
                OutputField = expression.Evaluate().ToString();
            }
            catch
            {
                OutputField = string.Empty;
            }
        }

        public void addOperator(string oprtr)
        {

            if (string.IsNullOrEmpty(InputField) && opr.isUnary(oprtr))
            {
                InputField = $"{oprtr}";
                return;
            }

            if (string.IsNullOrEmpty(InputField)) return;

            var last = InputField.Substring(InputField.Length - 1);
            if (opr.isOperator(oprtr) && oprtr != Operator.Add && opr.isUnary(oprtr))
            {
                InputField = $"{InputField}{oprtr}";
                return;
            }

            if (opr.isOperator(last))
            {
                InputField = $"{InputField.Substring(0, InputField.Length - 1)}{oprtr}";
                return;
            }

            InputField = $"{InputField}{oprtr}";
        }
    }

    public class Operator
    {
        public const string Subtract = "-";
        public const string Multiply = "*";
        public const string Add = "+";
        public const string Divide = "/";

        public bool containsOperator(string expression)
        {
            return expression.Any(chr => isOperator(chr.ToString()));
        }

        public bool isOperator(string opr)
        {
            switch (opr)
            {
                case Subtract:
                    return true;
                case Multiply:
                    return true;
                case Add:
                    return true;
                case Divide:
                    return true;
                default:
                    return false;
            }
        }

        public bool isUnary(string opr)
        {
            switch (opr)
            {
                case Subtract:
                    return true;
                default:
                    return false;
            }
        }
    }
}
