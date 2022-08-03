using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Interfaces;
using EdSofta.ViewModels.Utility;

namespace EdSofta.Services
{
    internal class SettingsService : ISettingsService
    {
        public async Task<bool> SetNotificationAsync(bool value)
        {
            return await SavedResourceUtility.changeNotification(value);
        }

        public async Task<bool> SetThemeAsync(string value)
        {
            return await SavedResourceUtility.changeTheme(value);
        }
    }
}
