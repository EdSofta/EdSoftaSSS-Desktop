using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Services
{
    //todo: rename and find appropriate use for practice mode
    internal class PracticeByYearService : IPracticeModeService
    {
        
        
        public async Task<List<QuestionBankViewModel>> getQuestionBanksAsync(QuestionType questionType)
        {
            return await Task.Run(() =>
                {
                    var allSubjects = ContentResourceUtility.getSubjects(questionType);
                    return allSubjects?.Select(x => new QuestionBankViewModel
                    {
                        Name = x,
                        Years = ContentResourceUtility.getYears(x, questionType).ToObservableCollection(),
                        Questions = getQuestionNumbers(x, questionType),
                        SelectedQuestionType = questionType
                    }).ToList();
                }
            );
        }


        public ObservableCollection<string> getQuestionNumbers(string subject, QuestionType questionType)
        {
            //var topicList = ContentResourceUtility.getTopics(name, questionType);

            //var totalSum = topicList.Select(topic => topic.TotalQuestions).Sum(x => x);
            var totalSum = 0;
            var years = ContentResourceUtility.getYears(subject, questionType);

            if (years.Count > 0) totalSum = ContentResourceUtility.getQuestionNumbers(subject, years[0], questionType);

            var range = new List<string> { "10", "20", "30", "40", "50", "60" };
            var newItems = range.Where(x => Convert.ToInt32(x) <= totalSum).ToList();

            if (newItems.Count == 0)
            {
                newItems.Add("0");
            }

            return newItems.ToObservableCollection();
        }
    }
}
