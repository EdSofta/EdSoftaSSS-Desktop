using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using Newtonsoft.Json;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class PracticeOnboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string SubjectHeader { get; set; }
        
        public string SubjectBody { get; set; }

        public string RecHeader { get; set; }
        public string RecText { get; set; }


        private LearningRecommendation _lRec;
        private IPracticeModeService _practiceModeService;
        public PracticeOnboardViewModel(LearningRecommendation lRec, IPracticeModeService practiceModeService)
        {
            _lRec = lRec;
            _practiceModeService = practiceModeService;
            InitializeFields();
        }

        public PracticeOnboardViewModel(string subject, string topic, IPracticeModeService practiceModeService)
        {
            var lRec = LRecommendation.createPracticeLR(
                new List<Practice>
                {
                    new Practice
                    {
                        Subject = subject,
                        Topics = new List<string> {topic}
                    }
                });

            _lRec = lRec;
            _practiceModeService = practiceModeService;
            InitializeFields();
        }

        private void InitializeFields()
        {
            var practice = JsonConvert.DeserializeObject<List<Practice>>(_lRec.Data);
            var subjects = practice.Select(x => x.Subject);
            var topics = practice.SelectMany(x => x.Topics);
            var enumerable = subjects as string[] ?? subjects.ToArray();
            SubjectHeader = $"{enumerable.Count()} subject(s) and {topics.Count()} topic(s)";
            SubjectBody = enumerable.HumanizeList();
            RecHeader = _lRec.Title;
            RecText = _lRec.ExtraText;
        }

        public async Task<List<QuestionBankViewModel>> generateQuestionBanks()
        {
            var recService = new LRecService();
            await recService.deleteRecommendation(_lRec);

            var practice = JsonConvert.DeserializeObject<List<Practice>>(_lRec.Data);
            var subjects = practice.Select(x => x.Subject).ToList();

            var banks = await _practiceModeService.getQuestionBanksAsync(QuestionType.Objectives);
            foreach (var bank in banks)
            {
                if (!subjects.Contains(bank.Name)) continue;
                bank.IsSelected = true;

                var practiceItem = practice.SingleOrDefault(x => x.Subject == bank.Name);
                if (practiceItem == null) continue;

                foreach (var topicViewModel in bank.Topics)
                {
                    if (!practiceItem.Topics.Exists(x =>
                        x.Equals(topicViewModel.TopicName, StringComparison.OrdinalIgnoreCase)))
                    {
                        topicViewModel.IsSelected = false;
                    }
                }

                var totalSum = getTotalTopicSum(bank.Name, bank.Topics);
                var numbers = bank.Questions.Select(x => Convert.ToInt32(x)).ToList();
                if (totalSum <= numbers.Min())
                {
                    bank.SelectedQuestions = totalSum;
                }
                else
                {
                    var num = numbers[0];
                    for (var i=0; i < numbers.Count; i++)
                    {
                        if (numbers[i] <= totalSum)
                        {
                            num = numbers[i];
                        }
                    }

                    bank.SelectedQuestions = num;
                }
            }

            return banks;
        }

        private int getTotalTopicSum(string subject, ObservableCollection<TopicViewModel> topics)
        {
            var topicList = ContentResourceUtility.getTopics(subject, QuestionType.Objectives);
            topicList = topicList.Where(x =>
                topics.ToList().Exists(item =>
                    item.IsSelected && item.TopicName == x.Name)).ToList();
            var totalSum = topicList.Select(topic => topic.TotalQuestions).Sum(x => x);
            return totalSum;
        }
          
    }
}
