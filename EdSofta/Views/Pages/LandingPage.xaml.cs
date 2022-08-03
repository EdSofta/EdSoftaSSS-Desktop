using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EdSofta.Constants;
using EdSofta.Data;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;
using Application = System.Windows.Application;
using CheckBox = System.Windows.Controls.CheckBox;
using TextBox = System.Windows.Controls.TextBox;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for LandingPage.xaml
    /// </summary>
    public partial class LandingPage : Page
    {
        internal readonly List<IModal> _modals = new List<IModal>();
        public readonly Frame _parentFrame;
        internal LandingPageViewModel landingPageViewModel;

        public LandingPage(Frame parentFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            landingPageViewModel = new LandingPageViewModel();
            landingPageViewModel.NotificationsViewModel = new NotificationsViewModel(new NotificationService(), this);
            landingPageViewModel.UsersViewModel = new UsersViewModel(new UserService());
            landingPageViewModel.SettingsViewModel = new SettingsViewModel(new SettingsService());
            DataContext = landingPageViewModel;
            var page = new HomePage(DisplayFrame, _parentFrame, landingPageViewModel);
            DisplayFrame.Navigate(page);
            SetModals();
            Loaded += LandingPageLoaded;
        }

        private async void LandingPageLoaded(object sender, RoutedEventArgs e)
        {
            var service = new LRecService();
            var isGenerated = await service.generateRecommendations();
            if (isGenerated)
            {
                await landingPageViewModel.NotificationsViewModel.RefreshNotificationsList(false);
                var isNotificationAllowed = await SavedResourceUtility.getNotificationValue();
                if (!isNotificationAllowed) return;
                landingPageViewModel.NotificationsViewModel.SetToastNotification("New Study Recommendations", "you have new learning recommendations.");
            }
            
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var contentService = new ContentService();
                var updateResult = await contentService.checkForContentUpdateAsync();
                var isSuccessful = await contentService.setUpdateNotificationAsync(updateResult);
                if (isSuccessful && !ViewModels.Utility.App.IsContentUpdateChecked)
                {
                    ViewModels.Utility.App.IsContentUpdateChecked = true;
                    await landingPageViewModel.NotificationsViewModel.RefreshNotificationsList(false);
                    var isNotificationAllowed = await SavedResourceUtility.getNotificationValue();
                    if (!isNotificationAllowed) return;
                    landingPageViewModel.NotificationsViewModel.SetToastNotification("EdSofta Content Updates", "There are new content updates available.");
                }
            }
        }

        private void SetModals()
        {
            _modals.AddRange(
                new List<IModal>
                {
                    new Modal(AddUserModal),
                    new Modal(TopicSelectionModal)
                });
        }

        private void HomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            // DisplayFrame.Source = new Uri("HomePage.xaml", UriKind.Relative);
            //DisplayFrame.Navigate(new Uri("Views/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute));
            var contentTag = ((Page)DisplayFrame.Content).Title ?? "Null";
            if (contentTag.Contains("Home")) return;
            var page = new HomePage(DisplayFrame, _parentFrame, landingPageViewModel);
            DisplayFrame.Navigate(page);
        }

        private void NotesButton_OnClick(object sender, RoutedEventArgs e)
        {
            //DisplayFrame.Source = new Uri("NotesPage.xaml", UriKind.Relative);
            DisplayFrame.Navigate(new Uri("Views/Pages/NotesPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void UserListButton_OnClick(object sender, RoutedEventArgs e)
        {
            //UsersContextMenu.IsOpen = true
            var contentTag = ((Page)DisplayFrame.Content).Title ?? "Null";
            if (contentTag.Contains("Users")) return;
            var page = new UsersPage(DisplayFrame, landingPageViewModel.UsersViewModel);
            DisplayFrame.Navigate(page);
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var contentTag = ((Page)DisplayFrame.Content).Title ?? "Null";
            if (contentTag.Contains("Settings")) return;
            var page = new SettingsPage(DisplayFrame, _parentFrame, landingPageViewModel);
            DisplayFrame.Navigate(page);
        }

        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {

            //MessageBox.Show("Number", "Navigation Stack Info");
            if (!DisplayFrame.CanGoBack) return;
            DisplayFrame.NavigationService.GoBack();
        }

        private void Overlay_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _modals.CloseModals();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.CloseModals();
        }

        private async void AddUserButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.SetModal(AddUserModal.Name);
            
        }


        //Topic Selection Modal

        internal QuestionBankViewModel _questionBankViewModel;

        //todo
        private void SelectAllCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = ((CheckBox)sender).IsChecked;
            var topicList = TopicListView.ItemsSource as ObservableCollection<TopicViewModel>;
            if (topicList == null || isChecked == null) return;

            if (isChecked == true)
            {
                foreach (var topic in topicList.Where(x => !x.IsSelected))
                {
                    topic.IsSelected = true;
                }
            }
            else
            {
                foreach (var topic in topicList.Where(x => x.IsSelected))
                {
                    topic.IsSelected = false;
                }
            }

        }

        private async void TopicCheckBox_OnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = ((CheckBox)sender).IsChecked;
            var topicList = TopicListView.ItemsSource as ObservableCollection<TopicViewModel>;
            if (topicList == null || isChecked == null) return;
            SelectAllCheckBox.IsChecked = !topicList.ToList().Exists(x => !x.IsSelected);
            _questionBankViewModel?.updateSelectedQuestionsAsync();
        }

        private void TopicItemGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ((Grid)sender).DataContext as TopicViewModel;
            if (item == null) return;
            item.IsSelected = !item.IsSelected;
        }


        //Topic Selection Modal

        

        private void NewsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var longUrl = "edsofta.info";
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

        private void NotificationsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var contentTag = ((Page)DisplayFrame.Content).Title ?? "Null";
            if (contentTag.Contains("Notifications")) return;
            var page = new NotificationsPage(DisplayFrame, _parentFrame, landingPageViewModel);
            DisplayFrame.Navigate(page);
        }

        private async void SaveUserButton_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (!validateField(FirstNameTextBox, FirstNameError))
            {
                AddUserScrollViewer.ScrollToTop();
                return;
            }

            if (!validateField(LastNameTextBox, LastNameError))
            {
                AddUserScrollViewer.ScrollToTop();
                return;
            }

            if (!validateField(UserNameTextBox, UserNameError))
            {
                return;
            }

            if (!validateField(EmailTextBox, EmailError) || !validateEmail(EmailTextBox, EmailError))
            {
                AddUserScrollViewer.ScrollToBottom();
                return;
            }

            if (!validateField(PhoneNumberTextBox, PhoneNumberError) || !validateLength(PhoneNumberTextBox, PhoneNumberError))
            {
                AddUserScrollViewer.ScrollToBottom();
                return;
            }

            var user = new User
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                UserRole = UserType.Guest,
                Id = Guid.NewGuid(),
                PhoneNumber = PhoneNumberTextBox.Text,
                Username = UserNameTextBox.Text,
                Email = EmailTextBox.Text
            };

            await landingPageViewModel.UsersViewModel.addUser(user);
            _modals.CloseModals();
            FirstNameTextBox.Text = "";
            LastNameTextBox.Text = "";
            EmailTextBox.Text = "";
            UserNameTextBox.Text = "";
            PhoneNumberTextBox.Text = "";
        }

        //private bool validateField(TextBox textBox, out string message)
        //{
        //    message = string.Empty;

        //    if (string.IsNullOrWhiteSpace(textBox.Text))
        //    {
        //        message = "Field cannot be empty";
        //        textBox.Tag = "Invalid";
        //        return false;
        //    }

        //    textBox.Tag = string.Empty;
        //    return true;
        //}

        //private bool validateEmail(TextBox textBox, out string message)
        //{
        //    message = string.Empty;
        //    try
        //    {

        //        var eMailValidator = new System.Net.Mail.MailAddress(textBox.Text);
        //        return true;
        //    }
        //    catch (FormatException ex)
        //    {
        //        message = "Invalid email address";
        //        textBox.Tag = "Invalid";
        //        return false;
        //    }
        //}

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

        private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox) sender;
            if (textBox == null) return;
            if (!string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Tag = string.Empty;
            }
        }

        private void ActivateButton_OnClickButton_OnClick(object sender, RoutedEventArgs e)
        {
            var contentTag = ((Page)DisplayFrame.Content).Title ?? "Null";
            if (contentTag.Contains("Activate")) return;
            var page = new ActivationPage(DisplayFrame, landingPageViewModel);
            DisplayFrame.Navigate(page);
        }
    }

}
