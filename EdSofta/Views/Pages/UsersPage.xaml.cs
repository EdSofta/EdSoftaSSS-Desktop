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
using EdSofta.DataAccess;
using EdSofta.Models;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
        }

        private readonly Frame _parentFrame;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private readonly UsersViewModel _usersViewModel;

        internal UsersPage(Frame parentFrame, UsersViewModel usersViewModel)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _usersViewModel = usersViewModel;
            DataContext = usersViewModel;
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private async void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (SavedResourceUtility.getUserRegistered()) return;

            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                using (var dal = new UnitOfWork())
                {
                    var user = dal.UserRepository.SingleOrDefault(x => x.UserRole == UserType.Administrator);
                    if (user == null) return;
                    var userData = new UserData
                    {
                        firstName = user.FirstName,
                        surname = user.LastName,
                        username = user.Username,
                        accountType = Keys.AccountType,
                        email = user.Email,
                        phoneNumber = user.PhoneNumber
                    };

                    var onboardingService = new OnboardingService();
                    var response = await onboardingService.registerUserAsync(userData);

                    if (response != null && response.status) SavedResourceUtility.setUserRegistered(true);
                }
            }
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {

        }


        private async void UsersList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var userItem = UsersList.SelectedItem as UserViewModel;
            if (userItem == null || userItem.IsCurrent) return;
            await _usersViewModel.setCurrentUser(userItem);
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private async void DeleteUserButton_OnClick(object sender, RoutedEventArgs e)
        {
            var item = ((Button)sender).DataContext as UserViewModel;
            if (item == null) return;

            var currentUser = await _usersViewModel.getCurrentUser();
            if (currentUser == null) return;

            if (currentUser.UserRole != UserType.Administrator)
            {
                var notAdminDialog = new DialogWindow("Permission required", "Only the administrator account can delete a guest account", false);
                notAdminDialog.Owner = Application.Current.MainWindow;
                notAdminDialog.ShowDialog();
                return;
            }

            var dialog = new DialogWindow("Remove user", "This will clear all data related to this user");
            dialog.Owner = Application.Current.MainWindow;
            var result = dialog.ShowDialog() ?? false;
            if (!result) return;
            await _usersViewModel.deleteUser(item);
        }
    }
}
