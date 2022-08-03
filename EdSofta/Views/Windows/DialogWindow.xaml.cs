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
using System.Windows.Threading;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Views.Windows
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        public DialogWindow(string title, string message, bool isResultDialog)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            MessageTextBlock.Text = $"{message}";

            if (!isResultDialog)
            {
                AcceptButton.Visibility = Visibility.Collapsed;
            }
        }

        public DialogWindow(string title, string message)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            MessageTextBlock.Text = $"{message}";
        }

        public DialogWindow(string title, string message, string acceptText, string cancelText)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            AcceptTextBlock.Text = acceptText;
            CancelTextBlock.Text = cancelText;
            MessageTextBlock.Text = $"{message}";
        }

        internal DialogWindow(string title, string message, DispatcherTimer timer, PracticeViewModel practiceViewModel)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            MessageTextBlock.Text = $"{message}";
            timer.Tick += (sender, e) =>
            {
                if (practiceViewModel.TimeLeft < 2) Close();
            };
        }

        internal DialogWindow(string title, string message, DispatcherTimer timer, PracticeViewModel practiceViewModel, string acceptText, string cancelText)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            AcceptTextBlock.Text = acceptText;
            CancelTextBlock.Text = cancelText;
            MessageTextBlock.Text = $"{message}";
            timer.Tick += (sender, e) =>
            {
                if (practiceViewModel.TimeLeft < 2) Close();
            };
        }

        internal DialogWindow(string title, string message, DispatcherTimer timer, PracticeViewModel practiceViewModel, bool isResultDialog, string acceptText, string cancelText)
        {
            InitializeComponent();
            TitleTextBlock.Text = title;
            AcceptTextBlock.Text = acceptText;
            CancelTextBlock.Text = cancelText;
            MessageTextBlock.Text = $"{message}";

            if (!isResultDialog)
            {
                AcceptButton.Visibility = Visibility.Collapsed;
            }

            timer.Tick += (sender, e) =>
            {
                if (practiceViewModel.TimeLeft < 2) Close();
            };
        }



        private void AcceptButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Window_Click(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
