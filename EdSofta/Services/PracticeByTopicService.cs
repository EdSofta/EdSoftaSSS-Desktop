using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Services
{
    class PracticeByTopicService : IPracticeModeService
    {
        
        public async Task<List<QuestionBankViewModel>> getQuestionBanksAsync(QuestionType questionType)
        {
            return await Task.Run(() =>
                {
                    var allSubjects = ContentResourceUtility.getSubjects(questionType);
                    return allSubjects?.Select(x => new QuestionBankViewModel
                    {
                        Name = x,
                        Topics = getTopicsViewModel(x, questionType),
                        Questions = getQuestionNumbers(x, questionType),
                        SelectedQuestionType = questionType
                    }).ToList();
                }
            );
        }


        public ObservableCollection<string> getQuestionNumbers(string name)
        {
            
            var topicList = ContentResourceUtility.getTopics(name, QuestionType.Objectives);
         
            var totalSum = topicList.Select(topic => topic.TotalQuestions).Sum(x => x);

            var range = new List<string> { "10", "20", "30", "40", "50", "60" };
            var newItems = range.Where(x => Convert.ToInt32(x) <= totalSum).ToList();

            if (newItems.Count == 0)
            {
                newItems.Add("0");
            }

            return newItems.ToObservableCollection();
        }

        public ObservableCollection<string> getQuestionNumbers(string name, QuestionType questionType)
        {

            var topicList = ContentResourceUtility.getTopics(name, questionType);

            var totalSum = topicList.Select(topic => topic.TotalQuestions).Sum(x => x);

            var range = new List<string> { "10", "20", "30", "40", "50", "60" };
            var newItems = range.Where(x => Convert.ToInt32(x) <= totalSum).ToList();

            if (newItems.Count == 0)
            {
                newItems.Add("0");
            }

            return newItems.ToObservableCollection();
        }

        public async Task<IEnumerable<TopicViewModel>> getTopicsViewModelAsync(string subject, QuestionType type)
        {
            var allTopics = await ContentResourceUtility.getTopicsAsync(subject, type);
            return allTopics.Select(topic => new TopicViewModel { TopicName = topic.Name, IsSelected = true });
        }

        public  ObservableCollection<TopicViewModel> getTopicsViewModel(string subject, QuestionType type)
        {
            var allTopics = ContentResourceUtility.getTopics(subject, type);
            return allTopics.Select(topic => new TopicViewModel { TopicName = topic.Name, IsSelected = true }).ToObservableCollection();
        }
    }
}
