using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Interfaces
{
    internal interface ISavedQuestionService
    {
        Task<ObservableCollection<SavedQuestion>> getSavedQuestionsAsync();
        Task<bool> addSavedQuestionAsync(Question question, User user);
        Task<bool> removeSavedQuestionAsync(int Id);
        Task<bool> removeAllSavedQuestion();

    }
}
