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
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for NotificationsPage.xaml
    /// </summary>
    public partial class NotificationsPage : Page
    {
        public NotificationsPage()
        {
            InitializeComponent();
        }

        private readonly Frame _parentFrame;
        private readonly Frame _outerFrame;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private NotificationsViewModel _notificationsViewModel;
        private LandingPageViewModel _landingPageViewModel;
        internal NotificationsPage(Frame parentFrame, Frame outerFrame, LandingPageViewModel landingPageViewModel)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _outerFrame = outerFrame;
            _landingPageViewModel = landingPageViewModel;
            _notificationsViewModel = _landingPageViewModel.NotificationsViewModel;
            DataContext = _notificationsViewModel;
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            _notificationsViewModel.UnreadAvailable = false;
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {

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

        private async void ClearListButton_OnClick(object sender, RoutedEventArgs e)
        {
            _landingPageViewModel.NotificationsViewModel.UnreadAvailable = false;
            await _notificationsViewModel.clearAllNotification();
        }

        private async void NotificationsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = NotificationsList.SelectedItem as Notification;
            if (item == null) return;

            if (!ViewModels.Utility.App.IsActivated && item.Type == NotificationType.LRec)
            {
                var title = "App not activated";
                var message = "To have access to all the study materials, you need to activate the app";

                DialogWindow dialog;
                dialog = new DialogWindow(title, message, "Activate now", "Later");

                dialog.Owner = Application.Current.MainWindow;
                var result = dialog.ShowDialog() ?? false;

                if (!result) return;

                var page = new ActivationPage(_parentFrame, _landingPageViewModel);
                _parentFrame.Navigate(page);
                return;
            }


            await _notificationsViewModel.deleteNotification(item);

            _landingPageViewModel.NotificationsViewModel.UnreadAvailable = false;

            if (item.Type == NotificationType.LRec)
            {
                var page = new LRPage(_parentFrame, _outerFrame);
                _parentFrame.Navigate(page);
            }

            if (item.Type == NotificationType.Content)
            {
                ViewModels.Utility.App.IsContentUpdateChecked = true;
                var page = new ContentUpdatePage(_outerFrame, _parentFrame, _landingPageViewModel);
                _outerFrame.Navigate(page);
            }
        }

    }

    
}
