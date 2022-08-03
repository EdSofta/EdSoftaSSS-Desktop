using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Interfaces
{
    internal interface IResultService
    {
        Task<ObservableCollection<ResultViewModel>> getResultsAsync();
        Task<bool> deleteResultsAsync();
    }
}
