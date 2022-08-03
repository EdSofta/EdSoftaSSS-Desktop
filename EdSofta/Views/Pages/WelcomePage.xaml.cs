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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private readonly Frame _parentFrame;
        public WelcomePage(Frame parentFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
        }

        private void NextLink_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var page = new OnboardingPage(_parentFrame);
            _parentFrame.Navigate(page);
        }

        private void SkipLink_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var page = new SignUpPage(_parentFrame);
            _parentFrame.Navigate(page);
        }

        private void NextLink_OnClick(object sender, RoutedEventArgs e)
        {
            var page = new OnboardingPage(_parentFrame);
            _parentFrame.Navigate(page);
        }
    }
}
