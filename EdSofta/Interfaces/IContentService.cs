using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;

namespace EdSofta.Interfaces
{
    internal interface IContentService
    {
        Task<UpdateResultType> checkForContentUpdateAsync();

        Task<bool> setUpdateNotificationAsync(UpdateResultType resultType);
    }
}
