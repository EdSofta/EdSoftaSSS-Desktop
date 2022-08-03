using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Interfaces;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Services
{
    class ResultService : IResultService
    {
        public async Task<ObservableCollection<ResultViewModel>> getResultsAsync()
        {
            var resultHistory = await SavedResourceUtility.getResultHistoryAsync();
            return resultHistory.Select(x => new ResultViewModel(x)).Reverse().ToObservableCollection();
        }

        public async Task<bool> deleteResultsAsync()
        {
            return await SavedResourceUtility.deleteResultHistoryAsync();
        }
    }
}
