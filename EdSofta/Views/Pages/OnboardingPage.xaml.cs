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
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for OnboardingPage.xaml
    /// </summary>
    public partial class OnboardingPage : Page
    {
        public OnboardingPage()
        {
            InitializeComponent();
        }

        private readonly Frame _parentFrame;
        private readonly OnboardingViewModel onboardingViewModel;

        public OnboardingPage(Frame parentFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            onboardingViewModel = new OnboardingViewModel();
            DataContext = onboardingViewModel;
        }

        private void SlideButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var border = (Border) sender;
            if (border == null) return;
            onboardingViewModel.changeSlide(border.Name);
        }

        private void Slide1Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                onboardingViewModel.changeSlide("Slide1");
            }
            catch
            {

            }
        }

        private void Slide2Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                onboardingViewModel.changeSlide("Slide2");
            }
            catch
            {

            }
        }

        private void Slide3Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                onboardingViewModel.changeSlide("Slide3");
            }
            catch
            {

            }
        }

        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(_parentFrame.CanGoBack) _parentFrame.GoBack();
        }

        private void GetStartedButton_OnClick(object sender, RoutedEventArgs e)
        {
            var page = new SignUpPage(_parentFrame);
            _parentFrame.Navigate(page);
        }
    }
}
