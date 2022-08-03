using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Web;
using EdSofta.Constants;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for ActivationPage.xaml
    /// </summary>
    public partial class ActivationPage : Page
    {
        public ActivationPage()
        {
            InitializeComponent();
        }

        private readonly Frame _parentFrame;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private readonly LandingPageViewModel _landingPageViewModel;
        private IActivationService activationService;

        internal ActivationPage(Frame parentFrame, LandingPageViewModel landingPageViewModel)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _landingPageViewModel = landingPageViewModel;
            activationService = new ActivationService();

           
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

        private void BankPaymentBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var page = new PaymentDetailsPage();
            _parentFrame.Navigate(page);
        }

        private async void GenerateKeyButton_OnClick(object sender, RoutedEventArgs e)
        {
            var message = string.Empty;
            SMSPanel.Visibility = Visibility.Collapsed;

            if (!validateField(PinTextBox, PinError))
            {
                return;
            }

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var dialog = new DialogWindow("No Internet Connection",
                    "Try turning on your Wi-Fi, connecting to a network, or checking the signal in your area", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
                SMSContent.Text = $"ACTIVATE-{Keys.AppType}-{PinTextBox.Text}-{ViewModels.Utility.App.ProductKey}";
                SMSPanel.Visibility = Visibility.Visible;
                return;
            }

            GenerateLoadingIcon.Visibility = Visibility.Visible;
            GenerateButtonText.Visibility = Visibility.Hidden;

            var isSuccessful = await activationService.activateByPinAsync(PinTextBox.Text);

            if (isSuccessful)
            {
                ViewModels.Utility.App.IsActivated = true;
                _landingPageViewModel.IsActivated = true;
                if(_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();
            }
            else
            {

                GenerateLoadingIcon.Visibility = Visibility.Hidden;
                GenerateButtonText.Visibility = Visibility.Visible;
                var dialog = new DialogWindow("Activation Failed", "Please check the key and try again", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
            }

            GenerateLoadingIcon.Visibility = Visibility.Hidden;
            GenerateButtonText.Visibility = Visibility.Visible;
        }

        private async void ActivateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!validateField(ActivationKeyTextBox, KeyError))
            {
                return;
            }

            var key = ActivationKeyTextBox.Text;
            var isActivated = await activationService.activateByKeyAsync(key);

            if (isActivated)
            {
                ViewModels.Utility.App.IsActivated = true;
                _landingPageViewModel.IsActivated = true;
                if (_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();
                return;
            }
            var dialog = new DialogWindow("Activation Failed", "Please check the key and try again", false);
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();

        }

        private void SalespointBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var longUrl = "edsofta.com/salespoint";
            var uriBuilder = new UriBuilder(longUrl);

            try
            {
                Process.Start(new ProcessStartInfo(uriBuilder.Uri.AbsoluteUri));
            }
            catch
            {
                var dialog = new DialogWindow("An error occured",
                    $"Unable to open browser to link: {uriBuilder.Uri.AbsoluteUri}", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
            }
        }


        private async void CardPaymentBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var productKey = await AppValidation.getProductKey();
            if (string.IsNullOrEmpty(productKey)) return;

            var longUrl = "edsofta.com/EdsoftaOnlinePayment/index.html";
            var uriBuilder = new UriBuilder(longUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["productCode"] = Keys.ProductCode;
            query["price"] = Keys.Price;
            query["productKey"] = productKey;
            uriBuilder.Query = query.ToString();
            longUrl = uriBuilder.ToString();

            try
            {
                Process.Start(new ProcessStartInfo(uriBuilder.Uri.AbsoluteUri));
            }
            catch
            {
                var dialog = new DialogWindow("An error occured",
                    $"Unable to open browser to link: {uriBuilder.Uri.AbsoluteUri}", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
            }
        }
    }
}
