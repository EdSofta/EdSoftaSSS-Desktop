using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Views.Windows
{
    /// <summary>
    /// Interaction logic for CalculatorWindow.xaml
    /// </summary>
    public partial class CalculatorWindow : Window
    {
        private readonly CalculatorViewModel calculatorViewModel = new CalculatorViewModel();
        public CalculatorWindow()
        {
            InitializeComponent();
            DataContext = calculatorViewModel;
        }

        private void Window_Click(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void NumberButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            if (button == null) return;

            var number = (string) button.Tag;
            calculatorViewModel.evaluate(number);
        }

        private void OperatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (button == null) return;

            var opr = (string)button.Tag;
            calculatorViewModel.addOperator(opr);
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            calculatorViewModel.deleteInput();
        }

        private void EqualButton_OnClick(object sender, RoutedEventArgs e)
        {
            calculatorViewModel.displayResult();
        }
    }
}
