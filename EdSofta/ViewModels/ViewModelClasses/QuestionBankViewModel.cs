using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class QuestionBankViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int selectedQuestions;
        public int SelectedQuestions
        {
            get { return selectedQuestions; }
            set
            {
                selectedQuestions = Convert.ToInt32(value);
                OnPropertyChanged("SelectedQuestions");
            }
        }

        private ObservableCollection<string> questions;

        public ObservableCollection<string> Questions
        {
            get { return questions;}
            set
            {
                questions = value;
                OnPropertyChanged("Questions");
            }
        }
        
        public ObservableCollection<TopicViewModel> Topics { get; set; }

        public ObservableCollection<string> Years { get; set; }

        private bool shuffleQuestions;
        public bool ShuffleQuestions
        {
            get { return shuffleQuestions; }
            set
            {
                shuffleQuestions = value;
                OnPropertyChanged("ShuffleQuestions");
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        private string selectedYear;

        public string SelectedYear
        {
            get { return selectedYear; }
            set
            {
                selectedYear = value;
                OnPropertyChanged("SelectedYear");
            }
        }

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public List<string> SelectedTopics
        {

            get
            {
                return Topics.Where(x => x.IsSelected).Select(x => x.TopicName).ToList();
            }
        }


        public QuestionType SelectedQuestionType { get; set; }

        public QuestionBankViewModel()
        {

        }

        public QuestionBankViewModel(string name)
        {
            Name = name;
            Questions = new ObservableCollection<string>();
            updateSelectedQuestionsAsync();
        }

        public async Task updateSelectedQuestionsAsync(string year)
        {
            var totalSum = 0;
            totalSum = await ContentResourceUtility.getQuestionNumbersAsync(Name, year, SelectedQuestionType);

            var range = new List<string> { "10", "20", "30", "40", "50", "60" };
            var newItems = range.Where(x => Convert.ToInt32(x) <= totalSum).ToList();

            if (newItems.Count == 0)
            {
                newItems.Add("0");
            }

            Questions.Clear();

            foreach (var item in newItems)
            {
                Questions.Add(item);
            }

            SelectedIndex = 0;
        }

        public async void updateSelectedQuestionsAsync()
        {
            if (Topics == null)
            {
                var service = new PracticeByTopicService();
                var topics =
                    await service.getTopicsViewModelAsync(Name, SelectedQuestionType);
                Topics = new ObservableCollection<TopicViewModel>(topics);
            }

            var topicList = await ContentResourceUtility.getTopicsAsync(Name, SelectedQuestionType);
            topicList = topicList.Where(x => 
                Topics.ToList().Exists(item => 
                    item.IsSelected && item.TopicName == x.Name)).ToList();
            var totalSum = topicList.Select(topic => topic.TotalQuestions).Sum(x => x);

            var range = new List<string>{"10", "20", "30", "40", "50", "60"};
            var newItems = range.Where(x => Convert.ToInt32(x) <= totalSum).ToList();

            if (newItems.Count == 0)
            {
                newItems.Add("0");
            }

            Questions.Clear();

            foreach (var item in newItems)
            {
                Questions.Add(item);
            }

            SelectedIndex = 0;


        }

    }
}
