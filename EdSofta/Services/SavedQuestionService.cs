using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using EdSofta.Data;
using EdSofta.DataAccess;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Services
{
    public class SavedQuestionService: ISavedQuestionService
    {
        private readonly ApplicationDbContext _context;

        
        public SavedQuestionService()
        {

        }

        public async Task<ObservableCollection<SavedQuestion>> getSavedQuestionsAsync()
        {

            using (var dal = new UnitOfWork())
            {
                var user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                if(user == null) return new ObservableCollection<SavedQuestion>();

                var savedQuestions =  await Task.Run(() =>
                    dal.QuestionRepository.Get(x => x.Saved && x.UserId == user.Id.ToString()).ToArray());

                return savedQuestions.Select(x => new SavedQuestion
                {
                    Subject = x.SubjectName,
                    Year = x.QuestionYear,
                    Topic = x.TopicName,
                    Number = x.QuestionNumber,
                    QuestionId = x.Id,
                    Type = x.Type
                }).ToObservableCollection();
            }

            
        }

        //todo: finally find a later use for it
        public async Task<bool> addSavedQuestionAsync(Question question, User user)
        {
            return false;
        }

        public async Task<bool> removeSavedQuestionAsync(int id)
        {
            using (var dal = new UnitOfWork())
            {
                var questionRecord = dal.QuestionRepository.SingleOrDefault(x =>
                    x.Id == id);

                if (questionRecord == null) return false;

                questionRecord.Saved = false;
                return await dal.SaveChangesAsync();
            }
        }

        public async Task<bool> removeAllSavedQuestion()
        {
            using (var dal = new UnitOfWork())
            {
                var user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                if (user == null) return false;

                var questions = dal.QuestionRepository.Get(x => x.UserId == user.Id.ToString() && x.Saved);

                foreach (var question in questions)
                {
                    question.Saved = false;
                }

                return await dal.SaveChangesAsync();
            }
        }
    }
}
