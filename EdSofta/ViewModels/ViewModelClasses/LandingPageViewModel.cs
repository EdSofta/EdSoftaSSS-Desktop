using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class LandingPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string timeOfDayGreetings { get; set; }

        public string TimeOfDayGreetings
        {
            get { return timeOfDayGreetings; }
            set
            {
                timeOfDayGreetings = value;
                OnPropertyChanged("TimeOfDayGreetings");
            }
        }

        private bool allowNotifications { get; set; }

        public bool AllowNotifications
        {
            get { return allowNotifications; }
            set
            {
                allowNotifications = value;
                OnPropertyChanged("AllowNotifications");
            }
        }


        private bool isActivated { get; set; }

        public bool IsActivated
        {
            get { return isActivated; }
            set
            {
                isActivated = value;
                OnPropertyChanged("IsActivated");
            }
        }



        private NotificationsViewModel notificationsViewModel { get; set; }

        public NotificationsViewModel NotificationsViewModel
        {
            get { return notificationsViewModel; }
            set
            {
                notificationsViewModel = value;
                OnPropertyChanged("NotificationsViewModel");
            }
        }

        private SettingsViewModel settingsViewModel { get; set; }

        public SettingsViewModel SettingsViewModel
        {
            get { return settingsViewModel; }
            set
            {
                settingsViewModel = value;
                OnPropertyChanged("SettingsViewModel");
            }
        }

        private UsersViewModel usersViewModel { get; set; }

        public UsersViewModel UsersViewModel
        {
            get { return usersViewModel; }
            set
            {
                usersViewModel = value;
                OnPropertyChanged("UsersViewModel");
            }
        }


        private string resultReport { get; set; }

        public string ResultReport
        {
            get { return resultReport; }
            set
            {
                resultReport = value;
                OnPropertyChanged("ResultReport");
            }
        }

        public async Task fetchLatestResult()
        {
            var results = await SavedResourceUtility.getResultHistoryAsync();
            if (results.Count == 0)
            {
                ResultReport = "No results available";
                ResultDate = string.Empty;
            }
            else
            {
                var latestResult = results.Last();
                var result = new ResultViewModel(latestResult);
                var percentage = result.Percentage;
                ResultReport = $"You scored a total of {result.Score} in {result.SubjectsList}";
                ResultDate = result.Date.HumanizeDateTime();
            }
        }


        private string resultDate { get; set; }

        public string ResultDate
        {
            get { return resultDate; }
            set
            {
                resultDate = value;
                OnPropertyChanged("ResultDate");
            }
        }

        private DispatcherTimer timer = new DispatcherTimer();

        public LandingPageViewModel()
        {
            IsActivated = Utility.App.IsActivated;
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 1, 0);
            SetTimeOfDayGreetings();
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            SetTimeOfDayGreetings();
        }

        public void SetTimeOfDayGreetings()
        {
            var time = DateTime.Now;
            if (time.Hour >= 0 && time.Hour < 12)
            {
                TimeOfDayGreetings = "Good morning,";
                return;
            }

            if (time.Hour >= 12 && time.Hour < 16)
            {
                TimeOfDayGreetings = "Good afternoon,";
                return;
            }

            TimeOfDayGreetings = "Good evening,";
        }
    }
}
