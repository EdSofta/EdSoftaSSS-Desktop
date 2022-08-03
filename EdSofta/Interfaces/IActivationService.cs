using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Models;

namespace EdSofta.Interfaces
{
    internal interface IActivationService
    {
        Task<bool> activateByKeyAsync(string key);

        Task<bool> activateByPinAsync(string pin);

    }
}
