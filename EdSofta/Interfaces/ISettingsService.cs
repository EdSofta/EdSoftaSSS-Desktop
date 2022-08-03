using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Interfaces
{
    internal interface ISettingsService
    {
        Task<bool> SetNotificationAsync(bool value);
        Task<bool> SetThemeAsync(string value);
    }
}
