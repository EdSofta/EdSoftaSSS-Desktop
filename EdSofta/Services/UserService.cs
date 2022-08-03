using EdSofta.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Repositories;
using EdSofta.ViewModels.ViewModelClasses;
using System.Collections.ObjectModel;
using System.Windows.Forms.VisualStyles;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.ViewModels.Utility;

namespace EdSofta.Services
{
    class UserService : IUserService
    {
        public async Task<bool> addUser(User user)
        {
            using (var dal = new UnitOfWork())
            {
                var users = await dal.UserRepository.GetAllAsync();
                if (users.SingleOrDefault(x => x.Id == user.Id) != null) return false;
                dal.UserRepository.Add(user);
                return await dal.SaveChangesAsync();
            }
        }

        public async Task<bool> deleteUser(User user)
        {
            using (var dal = new UnitOfWork())
            {
                var users = await dal.UserRepository.GetAllAsync();
                var record = users.ToList().SingleOrDefault(x => x.Id == user.Id);
                if (record == null) return false;
                dal.UserRepository.Remove(record);
                return await dal.SaveChangesAsync();
            }
        }

        public async Task<User> getCurrentUser()
        {
            using (var dal = new UnitOfWork())
            {
                var user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                return user;
            }
        }

        public async Task<User> getAdminUser()
        {
            using (var dal = new UnitOfWork())
            {
                var user = dal.UserRepository.SingleOrDefault(x => x.UserRole == UserType.Administrator);
                return user;
            }
        }

        public async Task<ObservableCollection<UserViewModel>> getUsers()
        {
            using (var dal = new UnitOfWork())
            {
                var users = await dal.UserRepository.GetAllAsync();
                return users.Select(x => new UserViewModel(x)).ToObservableCollection();
            }
        }

        public async Task<bool> setCurrentUser(User user)
        {
            using (var dal = new UnitOfWork())
            {
                var users = await dal.UserRepository.GetAllAsync();
                var enumerable = users as User[] ?? users.ToArray();
                var currentUser = enumerable.SingleOrDefault(x => x.IsCurrent);
                if (currentUser == null) return false;
                currentUser.IsCurrent = false;
                var newCurrent = enumerable.SingleOrDefault(x => x.Id == user.Id);
                if (newCurrent == null) return false;
                newCurrent.IsCurrent = true;
                return await dal.SaveChangesAsync();
            }
        }
    }
}
