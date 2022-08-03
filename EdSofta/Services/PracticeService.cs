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
    class PracticeService : IPracticeService
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
                            QuestionType.Objectives,
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
                            QuestionType.Objectives,
                            new QuestionBankService());
                        allBanks.Add(questionBank);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return allBanks;
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
                        x.Type == ExamQuestionType.Objectives);

                    if (questionItem != null)
                    {
                        questionItem.Saved = true;
                    }
                    else
                    {
                        var questionDto = await ContentResourceUtility.getQuestionAsync(subject, question.year,
                            question.questionNumber,
                            QuestionType.Objectives);

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
                            Type = ExamQuestionType.Objectives
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
                type = ExamQuestionType.Objectives
            };

            var webClientService = new WebClientService<ErrorReport, object>();
            await webClientService.postDataSingleAsync(@"user/reportquestion", errorReport);

            return true;
        }

        public async Task<ResultViewModel> markTestAsync(IEnumerable<QuestionBank> questionBanks, PracticeType type, PracticeMode practiceMode)
        {
            var result = new Result{date = DateTime.Now};
            var subjectGrades = new List<Grade>();
            var topicGrades = new Dictionary<string, List<Grade>>();
            var failedQuestions = new List<QuestionData>();

            foreach (var questionBank in questionBanks)
            {
                questionBank.IsCorrection = true;
                var questionBankGrade = await gradeSubjectAsync(questionBank);
                subjectGrades.Add(questionBankGrade);
                var subjectTopicsGrade = gradeTopics(questionBank);
                topicGrades.Add(questionBank.subjectName, subjectTopicsGrade);

                var questions = questionBank.QuestionList.Where(x => !x.isAnsweredCorrectly).Select(x =>
                    new QuestionData
                    {
                        Number = x.questionNumber,
                        Year = x.year
                    });

                failedQuestions.AddRange(questions);
            }

            var totalQuestions = (int)subjectGrades.Sum(x => x.totalScore);

            result.timeSpent = practiceMode.timeUsed.ConvertSecondsToMilliseconds();
            result.totalTime = practiceMode.timeValue.ConvertSecondsToMilliseconds();
            result.averageTime = (practiceMode.timeValue / totalQuestions).ConvertSecondsToMilliseconds();
            result.result = subjectGrades;
            result.topicGrades = topicGrades;
            result.failedQuestions = failedQuestions;


            SavedResourceUtility.saveResult(result);
            SavedResourceUtility.incrementTestCount();

            return new ResultViewModel(result);
        }

        public List<Grade> gradeTopics(QuestionBank questionBank)
        {
            var topicGrades = new List<Grade>();

            foreach (var question in questionBank.QuestionList)
            {
                var data = ContentResourceUtility.getQuestion(questionBank.subjectName, question.year,
                    question.questionNumber, QuestionType.Objectives);

                if (data.Topic == null) continue;

                var topicGrade = topicGrades.SingleOrDefault(x => x.name == data.Topic.Trim());
                if (topicGrade == null)
                {
                    topicGrades.Add(
                        new Grade
                        {
                            name = data.Topic.Trim(),
                            score = question.Mark,
                            totalScore = 1
                        });
                }
                else
                {
                    topicGrade.score += question.Mark;
                    topicGrade.totalScore += 1;
                }
            }

            return topicGrades;
        }

        public async Task<Grade> gradeSubjectAsync(QuestionBank questionBank)
        {
            var subjectGrade = new Grade{name = questionBank.subjectName, score = 0, totalScore = 0};
            var topicGrades = new List<Grade>();

            if (questionBank.CurrentQuestionData != null)
            {
                foreach (var option in questionBank.CurrentQuestionData.Options)
                {
                    option.IsActive = false;
                    option.IsCorrection = true;
                    option.IsAnswer = option.Key == questionBank.currentQuestion.CorrectAnswer;
                    option.IsAnsweredCorrectly = questionBank.currentQuestion.isAnsweredCorrectly;
                }
            }

            using (var dal = new UnitOfWork())
            {
                var user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                if (user == null) return subjectGrade;

                foreach (var question in questionBank.QuestionList)
                {
                    if (question.Tag != "Active")
                    {
                        question.Tag = question.isAnsweredCorrectly ? "IsPassed" : "IsFailed";
                    }


                    var data = ContentResourceUtility.getQuestion(questionBank.subjectName, question.year, 
                        question.questionNumber, QuestionType.Objectives);

                    if (data.Topic == null) continue;

                    var weight = Difficulty.Weight(data.Difficulty);

                    var topicGrade = topicGrades.SingleOrDefault(x => x.name == data.Topic.Trim());
                    if (topicGrade == null)
                    {
                        topicGrades.Add(
                            new Grade
                            {
                                name = data.Topic.Trim(),
                                score = question.Mark * weight,
                                totalScore = weight
                            });
                    }
                    else
                    {
                        topicGrade.score += question.Mark * weight;
                        topicGrade.totalScore += weight;
                    }


                    if (user.UserRole == UserType.Administrator)
                    {
                        var dbQuestion = dal.QuestionRepository.SingleOrDefault(x =>
                            x.SubjectName == questionBank.subjectName &&
                            x.QuestionNumber == question.questionNumber &&
                            x.QuestionYear == question.year &&
                            x.UserId == user.Id.ToString());

                        if (dbQuestion != null)
                        {
                            dbQuestion.Failed = !question.isAnsweredCorrectly;
                        }
                        else
                        {
                            var newDbQuestion = new Question
                            {
                                SubjectName = questionBank.subjectName,
                                QuestionYear = question.year,
                                QuestionNumber = question.questionNumber,
                                UserId = user.Id.ToString(),
                                Failed = !question.isAnsweredCorrectly,
                                TopicName = data.Topic.Trim(),
                                Saved = false,
                                Reported = false,
                                Type = ExamQuestionType.Objectives
                            };

                            dal.QuestionRepository.Add(newDbQuestion);
                        }

                        await dal.SaveChangesAsync();
                    }

                    subjectGrade.score += question.Mark;
                    subjectGrade.totalScore += 1;
                }


                if (user.UserRole == UserType.Administrator)
                {
                    SavedResourceUtility.updateTopicProficiency(questionBank.subjectName, topicGrades);
                }

                return subjectGrade;
            }

            
        }

        
    }
}
