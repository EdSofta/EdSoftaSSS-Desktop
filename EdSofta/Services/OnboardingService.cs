using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;

namespace EdSofta.Services
{
    internal class OnboardingService : IOnboardingService
    {
        public async Task<Response> registerUserAsync(UserData userData)
        {
            var webClientService = new WebClientService<UserData, Response>();
            return await webClientService.postDataAsync(@"auth/register", userData);
        }

        public async Task<bool> saveUserDataAsync(User user)
        {
            using (var dal = new UnitOfWork())
            {
                var record = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                if (record != null) return false;
                dal.UserRepository.Add(user);
                return await dal.SaveChangesAsync();
            }
        }
    }
}
