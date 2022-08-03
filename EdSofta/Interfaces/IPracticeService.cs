using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Interfaces
{
    internal interface IPracticeService
    {
        Task<List<QuestionBank>> generateQuestionBanksAsync(IEnumerable<QuestionBankViewModel> questionBanks, PracticeType type);

        Task<bool> saveQuestionAsync(QuestionViewModel question, string subject);

        Task<bool> reportQuestionAsync(QuestionBank questionBank, string error, string comment);

        Task<ResultViewModel> markTestAsync(IEnumerable<QuestionBank> questionBanks, PracticeType type, PracticeMode practiceMode);
    }
}
