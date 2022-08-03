using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Interfaces
{
    internal interface IWebClientService<T, TResponse>
    {
        Task<TResponse> getDataAsync(string endPoint);

        Task<TResponse> postDataAsync(string endPoint, T data);

        Task postDataSingleAsync(string endpoint, T data);
    }
}
