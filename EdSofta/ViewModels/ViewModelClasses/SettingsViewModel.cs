using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.Interfaces;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private NotifyTaskCompletion<List<ThemeViewModel>> themeOptions { get; set; }

        public NotifyTaskCompletion<List<ThemeViewModel>> ThemeOptions
        {
            get { return themeOptions; }
            set
            {
                themeOptions = value;
                OnPropertyChanged("ThemeOptions");
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

        private string productID { get; set; }

        public string ProductID
        {
            get { return $"Product Key: {productID}"; }
            set
            {
                productID = value;
                OnPropertyChanged("ProductID");
            }
        }

        private string appVersion { get; set; }

        public string AppVersion
        {
            get { return appVersion; }
            set
            {
                appVersion = value;
                OnPropertyChanged("AppVersion");
            }
        }

        private NotifyTaskCompletion<User> owner { get; set; }

        public NotifyTaskCompletion<User> Owner
        {
            get { return owner; }
            set
            {
                owner = value;
                OnPropertyChanged("Owner");
            }
        }

        public SettingsViewModel()
        {
            
        }

        private ISettingsService _settingsService;
        internal SettingsViewModel(ISettingsService settingsService)
        {
            ThemeOptions  = new NotifyTaskCompletion<List<ThemeViewModel>>(GetThemeOptions());
            new NotifyTaskCompletion<bool>(GetNotificationValue(), OnValueReturned);
            new NotifyTaskCompletion<string>(AppValidation.getProductKey(), OnProductIDReturned);
            var userService = new UserService();
            Owner = new NotifyTaskCompletion<User>(userService.getAdminUser());
            AppVersion = DeviceInformationClass.getRunningVersion();
            _settingsService = settingsService;
        }

        private void OnValueReturned(object sender, TaskCompletedEventArgs e)
        {
            var notification = (NotifyTaskCompletion<bool>)sender;
            AllowNotifications = notification.Result;
        }

        private void OnProductIDReturned(object sender, TaskCompletedEventArgs e)
        {
            var productId = (NotifyTaskCompletion<string>)sender;
            ProductID = productId.Result;
        }


        public async Task<bool> ChangeThemeSettings(string value)
        {
            var themeViewModel = ThemeOptions.Result.SingleOrDefault(x => x.Value == value);
            if (themeViewModel == null) return false;
            var isSuccessful = await _settingsService.SetThemeAsync(value);
            themeViewModel.IsSelected = isSuccessful;
            return isSuccessful;
        }

        public async Task<bool> ChangeNotificationSettings()
        {
            var value = !AllowNotifications;
            var isSuccessful = await _settingsService.SetNotificationAsync(value);
            AllowNotifications = isSuccessful ? value : !value;
            return isSuccessful;
        }

        private async Task<bool> GetNotificationValue()
        {
            return await SavedResourceUtility.getNotificationValue();
        }

        private async Task<List<ThemeViewModel>> GetThemeOptions()
        {
           

            var systemDefault = new ThemeViewModel
            {
                IsSelected = false,
                Text = "Use system default",
                Value = Theme.Default
            };

            var light = new ThemeViewModel
            {
                IsSelected = false,
                Text = "Light",
                Value = Theme.Light
            };

            var dark = new ThemeViewModel
            {
                IsSelected = false,
                Text = "Dark",
                Value = Theme.Dark
            };

            var list = new List<ThemeViewModel>();

            if (DeviceInformationClass.IsWindows10()) list.Add(systemDefault);
            list.Add(light);
            list.Add(dark);

            var currentTheme = await SavedResourceUtility.getCurrentTheme();
            var item = list.SingleOrDefault(x => x.Value == currentTheme);

            if (item != null)
            {
                item.IsSelected = true;
            }
            else
            {
                list[0].IsSelected = true;
            }

            return list;
        }
        
    }
}
