using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.DataAccess;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Services
{
    internal class QuestionBankService : IQuestionBankService
    {
        public void saveQuestionAsync(IQuestion question)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> getSubjectsAsync(QuestionType type)
        {
            return await ContentResourceUtility.getSubjectsAsync(type);
        }

        public async Task<List<string>> getYearsAsync(string subject, QuestionType type)
        {
            return await ContentResourceUtility.getYearsAsync(subject, type);
        }

        public async Task<List<Topic>> getTopicsAsync(string subject, QuestionType type)
        {
            return await ContentResourceUtility.getTopicsAsync(subject, type);
        }

        public async Task<QuestionDTO> getQuestionAsync(string subject, string year, int number, QuestionType type)
        {
            return await ContentResourceUtility.getQuestionAsync(subject, year, number, type);
        }


        public async Task<QuestionDTO> getSavedQuestionAsync(string subject, string year, int number, QuestionType type)
        {
            var question = await ContentResourceUtility.getQuestionAsync(subject, year, number, type);
            if (type == QuestionType.Theory)
            {
                question.IsTheoryAnswerVisible = true;
            }

            return question;
        }



        //todo
        public async Task<List<QuestionViewModel>> getQuestionsAsync(int totalQuestions, string year, string subject, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<string>> getAnswersAsync(string subject, string year, QuestionType type)
        {
            return await ContentResourceUtility.getAnswersAsync(subject, year, type);
        }
    }
}
