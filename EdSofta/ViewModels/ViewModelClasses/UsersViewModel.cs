using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Interfaces;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class UsersViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private NotifyTaskCompletion<User> currentUser { get; set; }

        public NotifyTaskCompletion<User> CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }

        private NotifyTaskCompletion<ObservableCollection<UserViewModel>> users { get; set; }

        public NotifyTaskCompletion<ObservableCollection<UserViewModel>> Users
        {
            get { return users; }
            set
            {
                users = value;
                OnPropertyChanged("Users");
            }
        }

        private readonly IUserService _userService;

        public UsersViewModel(IUserService userService)
        {
            _userService = userService;
            Users = new NotifyTaskCompletion<ObservableCollection<UserViewModel>>(userService.getUsers(), OnUserLoaded);
        }

        private async void OnUserLoaded(object sender, TaskCompletedEventArgs e)
        {
            CurrentUser = new NotifyTaskCompletion<User>(getCurrentUser());
        }

        public async Task deleteUser(UserViewModel userItem)
        {
            var isSuccessful = await _userService.deleteUser(userItem.UserData);
            if (isSuccessful) Users.Result.Remove(userItem);
        }

        public async Task addUser(User user)
        {
            var isSuccessful = await _userService.addUser(user);
            if (!isSuccessful) return;
            Users.Result.Add(new UserViewModel(user));
        }

        public async Task setCurrentUser(UserViewModel userItem)
        {
            var currentUser = Users.Result.SingleOrDefault(x => x.IsCurrent);
            if (currentUser == null) return;
            var isSuccessful = await _userService.setCurrentUser(userItem.UserData);
            if (!isSuccessful) return;
            currentUser.IsCurrent = false;
            userItem.IsCurrent = true;
            CurrentUser = new NotifyTaskCompletion<User>(getCurrentUser());
        }

        public async Task<User> getCurrentUser()
        {
            return await _userService.getCurrentUser();
        }
    }
}
