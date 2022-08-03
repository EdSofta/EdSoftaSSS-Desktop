using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using EdSofta.Constants;
using EdSofta.Interfaces;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using EdSofta.Views.Pages;
using Application = System.Windows.Application;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class NotificationsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private NotifyTaskCompletion<ObservableCollection<Notification>> notifications { get; set; }

        public NotifyTaskCompletion<ObservableCollection<Notification>> Notifications
        {
            get { return notifications; }
            set
            {
                notifications = value;
                OnPropertyChanged("Notifications");
            }
        }

        private bool isNotificationAvailable { get; set; }

        public bool IsNotificationAvailable
        {
            get { return isNotificationAvailable; }
            set
            {
                isNotificationAvailable = value;
                OnPropertyChanged("IsNotificationAvailable");
            }
        }


        private bool unreadAvailable { get; set; }

        public bool UnreadAvailable
        {
            get { return unreadAvailable; }
            set
            {
                unreadAvailable = value;
                OnPropertyChanged("UnreadAvailable");
            }
        }


        private INotificationService _notificationService;

        public NotificationsViewModel(INotificationService notificationService, LandingPage landingPage)
        {
            _notificationService = notificationService;
            _landingPage = landingPage;
            Notifications = 
                new NotifyTaskCompletion<ObservableCollection<Notification>>(notificationService.getNotificationsAsync(), OnNotificationLoadComplete);
        }

        public async Task RefreshNotificationsList(bool setToast)
        {
            if (!Notifications.IsSuccessfullyCompleted) return;
            var allNotifications = await _notificationService.getNotificationsAsync();
            foreach (var notification in allNotifications)
            {
                if(Notifications.Result.SingleOrDefault(x=> x.Id == notification.Id) != null) continue;
                Notifications.Result.Add(notification);
            }

            setNotificationAvailable(Notifications.Result, setToast);
        }

        private void OnNotificationLoadComplete(object sender, TaskCompletedEventArgs e)
        {
            var task = (NotifyTaskCompletion<ObservableCollection<Notification>>) sender;
            if (task.Result == null) return;
            setNotificationAvailable(task.Result, true);
        }

        private async void setNotificationAvailable(ObservableCollection<Notification> notifications, bool setToast)
        {
            IsNotificationAvailable = notifications.Count > 0;
            UnreadAvailable = IsNotificationAvailable;
            var isNotificationAllowed = await SavedResourceUtility.getNotificationValue();
            if(!isNotificationAllowed)  return;
            var count = notifications.Count > 1 ? "notifications" : "notification";
            if (IsNotificationAvailable && setToast) SetToastNotification("EdSofta Notifications", $"You have {notifications.Count} new {count}");
        }

        public async Task clearAllNotification()
        {
            if (!Notifications.IsSuccessfullyCompleted) return;
            var isSuccessful = await _notificationService.deleteAllNotifications();
            if (isSuccessful) Notifications.Result.Clear();
            setNotificationAvailable(Notifications.Result, false);
        }

        public async Task deleteNotification(Notification notification)
        {
            var isSuccessful = await _notificationService.deleteNotification(notification);
            if (isSuccessful) Notifications.Result.Remove(notification);
            setNotificationAvailable(Notifications.Result, false);
        }

        private NotifyIcon notifyIcon;
        private LandingPage _landingPage;
        public void SetToastNotification(string title, string body)
        {
            UnreadAvailable = true;

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            // Hides the icon when the notification is closed
            notifyIcon.BalloonTipClosed += (s, evnt) => notifyIcon.Visible = false;

            notifyIcon.Visible = true;
            notifyIcon.Click += (snder, args) =>
            {
                Application.Current.MainWindow?.Activate();
                var contentTag = ((Page)_landingPage.DisplayFrame.Content).Title ?? "Null";
                if (contentTag.Contains("Notifications")) return;
                var page = new NotificationsPage(_landingPage.DisplayFrame, _landingPage._parentFrame, _landingPage.landingPageViewModel);
                _landingPage.DisplayFrame.Navigate(page);
            };

            if (_landingPage.landingPageViewModel.UsersViewModel.Users.IsSuccessfullyCompleted)
            {
                var adminUser = _landingPage.landingPageViewModel.UsersViewModel.Users.Result
                    .SingleOrDefault(x => x.UserRole == UserType.Administrator);

                var username = string.Empty;
                if (adminUser != null)
                {
                    username = adminUser.UserData.FirstName + ", ";
                }
                // Shows a notification with specified message and title
                notifyIcon.ShowBalloonTip(3000, title,
                    $"{username}, {body}", ToolTipIcon.None);
            }

        }

    }
}
