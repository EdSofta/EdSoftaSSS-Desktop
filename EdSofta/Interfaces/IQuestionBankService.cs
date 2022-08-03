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
    internal interface IQuestionBankService
    {
        void saveQuestionAsync(IQuestion question);
        Task<List<string>> getSubjectsAsync(QuestionType type);
        Task<List<string>> getYearsAsync(string subject, QuestionType type);
        Task<List<Topic>> getTopicsAsync(string subject, QuestionType type);
        Task<QuestionDTO> getQuestionAsync(string subject, string year, int number, QuestionType type);
        Task<List<string>> getAnswersAsync(string subject, string year, QuestionType type);
        Task<List<QuestionViewModel>> getQuestionsAsync(int totalQuestions, string year, string subject, string userId);
    }
}
