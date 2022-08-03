using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Repositories;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool isCurrent { get; set; }

        public bool IsCurrent
        {
            get { return isCurrent; }
            set
            {
                isCurrent = value;
                OnPropertyChanged("IsCurrent");
            }
        }

        public string Name => $"{UserData.LastName} {UserData.FirstName}";
        public string UserRole => UserData.UserRole;

        public User UserData;

        public UserViewModel(User user)
        {
            UserData = user;
            IsCurrent = user.IsCurrent;
        }
    }
}
