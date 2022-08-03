using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Models;
using EdSofta.Repositories;

namespace EdSofta.Interfaces
{
    internal interface IOnboardingService
    {
        Task<Response> registerUserAsync(UserData userData);
        Task<bool> saveUserDataAsync(User user);
    }
}
