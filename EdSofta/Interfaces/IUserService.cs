using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Repositories;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Interfaces
{
    internal interface IUserService
    {
        Task<bool> addUser(User user);

        Task<bool> deleteUser(User user);

        Task<ObservableCollection<UserViewModel>> getUsers();

        Task<User> getCurrentUser();

        Task<User> getAdminUser();

        Task<bool> setCurrentUser(User user);
    }
}
