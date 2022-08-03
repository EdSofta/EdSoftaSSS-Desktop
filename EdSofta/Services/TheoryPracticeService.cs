using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Services
{
    class TheoryPracticeService : IPracticeService
    {
        public async Task<List<QuestionBank>> generateQuestionBanksAsync(IEnumerable<QuestionBankViewModel> questionBanks, PracticeType type)
        {
            var allBanks = new List<QuestionBank>();

            switch (type)
            {
                case PracticeType.ByYear:
                    foreach (var bank in questionBanks)
                    {
                        if (!bank.IsSelected) continue;
                        var questionBank = new QuestionBank(bank.Name,
                            bank.SelectedYear,
                            bank.SelectedQuestions,
                            QuestionType.Theory,
                            new QuestionBankService());
                        allBanks.Add(questionBank);
                    }
                    break;

                case PracticeType.ByTheme:
                    foreach (var bank in questionBanks)
                    {
                        if (!bank.IsSelected) continue;
                        var questionBank = new QuestionBank(bank.Name,
                            bank.SelectedTopics,
                            bank.SelectedQuestions,
                            QuestionType.Theory,
                            new QuestionBankService());
                        allBanks.Add(questionBank);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return allBanks;
        }

        public Task<ResultViewModel> markTestAsync(IEnumerable<QuestionBank> questionBanks, PracticeType type, PracticeMode practiceMode)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> reportQuestionAsync(QuestionBank questionBank, string error, string comment)
        {
            User user;
            using (var dal = new UnitOfWork())
            {
                user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
            }

            if (user == null) return false;

            var errorReport = new ErrorReport
            {
                client = Keys.ClientType,
                comment = comment ?? "",
                error = error,
                name = $"{user.LastName} {user.FirstName}",
                phoneNumber = user.PhoneNumber,
                questionNumber = questionBank.currentQuestion.questionNumber.ToString(),
                subject = questionBank.subjectName,
                year = questionBank.currentQuestion.year,
                type = ExamQuestionType.Theory
            };

            var webClientService = new WebClientService<ErrorReport, object>();
            await webClientService.postDataSingleAsync(@"user/reportquestion", errorReport);

            return true;
        }

        public async Task<bool> saveQuestionAsync(QuestionViewModel question, string subject)
        {
            try
            {
                using (var dal = new UnitOfWork())
                {
                    var user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                    if (user == null) return false;


                    var questionItem = dal.QuestionRepository.SingleOrDefault(x =>
                        x.QuestionYear == question.year &&
                        x.QuestionNumber == question.questionNumber &&
                        x.SubjectName == subject &&
                        x.UserId == user.Id.ToString() &&
                        x.Type == ExamQuestionType.Theory);

                    if (questionItem != null)
                    {
                        questionItem.Saved = true;
                    }
                    else
                    {
                        var questionDto = await ContentResourceUtility.getQuestionAsync(subject, question.year,
                            question.questionNumber,
                            QuestionType.Theory);

                        if (questionDto.Topic == null) return true;


                        var newQuestion = new Question
                        {
                            SubjectName = subject,
                            QuestionYear = question.year,
                            QuestionNumber = question.questionNumber,
                            TopicName = questionDto.Topic.Trim(),
                            UserId = user.Id.ToString(),
                            Saved = true,
                            Failed = false,
                            Reported = false,
                            Type = ExamQuestionType.Theory
                        };
                        dal.QuestionRepository.Add(newQuestion);
                    }

                    return await dal.SaveChangesAsync();
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
