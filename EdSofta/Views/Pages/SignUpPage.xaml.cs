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
using EdSofta.Constants;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private readonly Frame _parentFrame;
        private IOnboardingService onboardingService;

        public SignUpPage(Frame parentFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            onboardingService = new OnboardingService();
        }

        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(_parentFrame.CanGoBack) _parentFrame.GoBack();
        }

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (textBox == null) return;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Tag = string.Empty;
            }
        }


        private bool validateField(TextBox textBox, TextBlock errorTextBlock)
        {

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorTextBlock.Text = "Field cannot be empty";
                textBox.Tag = "Invalid";
                return false;
            }

            

            textBox.Tag = string.Empty;
            errorTextBlock.Text = string.Empty;
            return true;
        }

        private bool validateLength(TextBox textBox, TextBlock errorTextBlock)
        {
            if (textBox.Text.Length < textBox.MaxLength)
            {
                errorTextBlock.Text = "Invalid input";
                textBox.Tag = "Invalid";
                return false;
            }

            textBox.Tag = string.Empty;
            errorTextBlock.Text = string.Empty;
            return true;
        }

        private bool validateEmail(TextBox textBox, TextBlock errorTextBlock)
        {
            try
            {

                var eMailValidator = new System.Net.Mail.MailAddress(textBox.Text);
                errorTextBlock.Text = string.Empty;
                textBox.Tag = string.Empty;
                return true;
            }
            catch (FormatException ex)
            {
                errorTextBlock.Text = "Invalid email address";
                textBox.Tag = "Invalid";
                return false;
            }
        }

        private async void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!validateField(FirstNameTextBox, FirstNameError))
            {
                return;
            }

            if (!validateField(LastNameTextBox, LastNameError))
            {
                return;
            }

            if (!validateField(UserNameTextBox, UserNameError))
            {
                return;
            }

            if (!validateField(EmailTextBox, EmailError) || !validateEmail(EmailTextBox, EmailError))
            {
                return;
            }

            if (!validateField(PhoneNumberTextBox, PhoneNumberError) || !validateLength(PhoneNumberTextBox, PhoneNumberError))
            {
                return;
            }

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                //var dialog = new DialogWindow("No Internet Connection",
                //    "Try turning on your Wi-Fi, connecting to a network, or checking the signal in your area", false);
                //dialog.Owner = Application.Current.MainWindow;
                //dialog.ShowDialog();
                //return;

                LoadingIcon.Visibility = Visibility.Visible;
                SubmitButtonText.Visibility = Visibility.Hidden;

                var userData = new UserData
                {
                    firstName = FirstNameTextBox.Text,
                    surname = LastNameTextBox.Text,
                    username = UserNameTextBox.Text,
                    accountType = Keys.AccountType,
                    email = EmailTextBox.Text,
                    phoneNumber = PhoneNumberTextBox.Text
                };

                var response = await onboardingService.registerUserAsync(userData);

                if (response == null || !response.status)
                {
                    LoadingIcon.Visibility = Visibility.Hidden;
                    SubmitButtonText.Visibility = Visibility.Visible;
                    var dialog = new DialogWindow("An error occured", "Please retry the process", false);
                    dialog.Owner = Application.Current.MainWindow;
                    dialog.ShowDialog();
                    return;
                }
                else
                {
                    SavedResourceUtility.setUserRegistered(true);
                }
            }

            

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneNumberTextBox.Text,
                UserRole = UserType.Administrator,
                IsCurrent = true,
                Username = string.Empty
            };

            var isSuccessful = await onboardingService.saveUserDataAsync(user);

            if (isSuccessful)
            {
                var page = new LandingPage(_parentFrame);
                _parentFrame.Navigate(page);
            }

            LoadingIcon.Visibility = Visibility.Hidden;
            SubmitButtonText.Visibility = Visibility.Visible;
        }

        private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return text.All(char.IsDigit);
        }

        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
