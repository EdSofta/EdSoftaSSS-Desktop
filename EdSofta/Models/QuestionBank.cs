using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Pages;

namespace EdSofta.Models
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class QuestionBank : INotifyPropertyChanged
    {

        private IQuestionBankService questionBankService;
        public QuestionViewModel currentQuestion { get; set; }

        private QuestionDTO currentQuestionData;

        public QuestionDTO CurrentQuestionData
        {
            get { return currentQuestionData; }
            set
            {
                currentQuestionData = value;
                OnPropertyChanged("CurrentQuestionData");
            }
        }

        private int currentQuestionNumber;

        public int CurrentQuestionNumber
        {
            get { return currentQuestionNumber; }
            set
            {
                currentQuestionNumber = value;
                OnPropertyChanged("CurrentQuestionNumber");
            }
        }

        private int size;
        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }

        public string subjectName { get; }

        public QuestionType _questionType { get; }

        private readonly ObservableCollection<QuestionViewModel> questionList = new ObservableCollection<QuestionViewModel>();

        public ObservableCollection<QuestionViewModel> QuestionList => questionList;


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public QuestionBank(string subjectName, string year, int totalQuestion, QuestionType questionType, IQuestionBankService questionBankService, bool shuffled = false)
        {
            this.subjectName = subjectName;
            this.questionBankService = questionBankService;
            _questionType = questionType;
            if (questionType == QuestionType.Objectives)
            {
                createQuestionBank(subjectName, year, totalQuestion);
            }
            else
            {
                createTheoryQuestionBank(subjectName, year, totalQuestion);
            }

        }

        public QuestionBank(string subjectName, List<string> topics, int totalQuestion,QuestionType questionType, IQuestionBankService questionBankService)
        {
            this.subjectName = subjectName;
            this.questionBankService = questionBankService;
            _questionType = questionType;
            if (questionType == QuestionType.Objectives)
            {
                createQuestionBankByTopic(subjectName, topics, totalQuestion);
            }
            else
            {
                createTheoryQuestionBankByTopic(subjectName, topics, totalQuestion);
            }
        }

        private void createShuffledQuestionBank(string subjectName, string year, int totalQuestion)
        {
            var answerList = ContentResourceUtility.getAnswers(subjectName, year, QuestionType.Objectives);
            var list = Enumerable.Range(1, answerList.Count).ToList();
            for (var i = 0; i < totalQuestion; i++)
            {
                var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(list.Count - 1);
                var num = list[randomInt];
                questionList.Add(new QuestionViewModel(num, answerList[num - 1], year));
                list.RemoveAt(randomInt);
            }
        }

        private async void createTheoryQuestionBank(string subjectName, string year, int totalQuestion)
        {
            var questions = new List<QuestionViewModel>();
            var failedQuestions = new Question[] { };
            var failedTopics = new List<string>();

            using (var dal = new UnitOfWork())
            {
                var user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                if (user == null) return;

                failedQuestions = dal.QuestionRepository.Get(x =>
                    x.Failed && x.QuestionYear == year &&
                    x.SubjectName == subjectName && x.UserId == user.Id.ToString()
                    && x.Type == ExamQuestionType.Objectives).ToArray();
            }

            if (failedQuestions.Length > 0)
            {
                foreach (var question in failedQuestions)
                {
                    if(!failedTopics.Contains(question.TopicName)) failedTopics.Add(question.TopicName);
                }
            }

            var failedTopicsQuestions = ContentResourceUtility.getTopicsQuestions(subjectName, year, failedTopics, QuestionType.Theory);

            if (failedTopicsQuestions.Count >= totalQuestion)
            {
                for (var i = 0; i < totalQuestion; i++)
                {
                    var item = failedTopicsQuestions[i];
                    questions.Add(
                        new QuestionViewModel(item, year));
                }
            }
            else
            {
                foreach (var item in failedTopicsQuestions)
                {
                    questions.Add(
                        new QuestionViewModel(item, year));
                }

                var numberLeft = totalQuestion - failedTopicsQuestions.Count;
                var totalQuestionNumbers = ContentResourceUtility.getQuestionNumbers(subjectName, year, QuestionType.Theory);

                var list = Enumerable.Range(1, totalQuestionNumbers).ToList();
                list.RemoveAll(x => failedTopicsQuestions.ToList().Exists(item => item == x));

                for (var i = 0; i < numberLeft; i++)
                {
                    var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(list.Count - 1);
                    var num = list[randomInt];

                    questions.Add(
                        new QuestionViewModel(
                            num, year));
                    list.RemoveAt(randomInt);
                }
            }


            foreach (var question in questions)
            {
                questionList.Add(question);
            }

            Size = questionList.Count;
            await loadQuestionAsync(1);


        }

        private async void createQuestionBank(string subjectName, string year, int totalQuestion)
        {
            //var userId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString();

            var questions = new List<QuestionViewModel>();
            var failedQuestions = new Question[] { };

            using (var dal = new UnitOfWork())
            {
                var user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                if (user == null) return;

                failedQuestions = dal.QuestionRepository.Get(x =>
                    x.Failed && x.QuestionYear == year &&
                    x.SubjectName == subjectName && x.UserId == user.Id.ToString()
                    && x.Type == ExamQuestionType.Objectives).ToArray();
            }

            if (failedQuestions.Length >= totalQuestion)
            {
                for (var i = 0; i < totalQuestion; i++)
                {
                    var item = failedQuestions[i];
                    questions.Add(
                        new QuestionViewModel(item.QuestionNumber,
                            ContentResourceUtility.getAnswer(subjectName, year, item.QuestionNumber, QuestionType.Objectives),
                            year));
                }
            }
            else
            {
                foreach (var item in failedQuestions)
                {
                    questions.Add(new QuestionViewModel(
                        item.QuestionNumber,
                        ContentResourceUtility.getAnswer(subjectName, year, item.QuestionNumber, QuestionType.Objectives),
                        year));
                }

                var numberLeft = totalQuestion - failedQuestions.Length;
                var totalQuestionNumbers = ContentResourceUtility.getQuestionNumbers(subjectName, year, QuestionType.Objectives);


                var list = Enumerable.Range(1, totalQuestionNumbers).ToList();
                list.RemoveAll(x => failedQuestions.ToList().Exists(item => item.QuestionNumber == x));

                for (var i = 0; i < numberLeft; i++)
                {
                    var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(list.Count - 1);
                    var num = list[randomInt];

                    questions.Add(
                        new QuestionViewModel(
                            num, 
                            ContentResourceUtility.getAnswer(subjectName, year, num, QuestionType.Objectives), 
                            year));
                    list.RemoveAt(randomInt);
                }
            }


            foreach (var question in questions)
            {
                questionList.Add(question);
            }

            Size = questionList.Count;
            await loadQuestionAsync(1);
        }



        private async void createTheoryQuestionBankByTopic(string subjectName, ICollection<string> topics, int totalQuestion)
        {
            var topicList = await ContentResourceUtility.getTopicsAsync(subjectName, QuestionType.Theory);
            topicList = topicList.Where(x => topics.Contains(x.Name)).ToList();
            var questionNums = new List<int>();
            var ratios = new List<int>();
            var totalRatio = topicList.Select(topic => topic.TotalQuestions).Sum(new Func<int, int>(x => x));
            var count = totalQuestion;

            foreach (var topic in topicList)
            {
                questionNums.Add(topic.TotalQuestions);
            }

            for (var i = 0; i < questionNums.Count; i++)
            {
                if (i == questionNums.Count - 1)
                {
                    ratios.Add(count);
                }
                else
                {
                    var ratio = (questionNums[i] / Convert.ToDouble(totalRatio)) * count;
                    var value = (int)Math.Round(ratio, MidpointRounding.AwayFromZero);
                    ratios.Add(value);
                    //ratios[i] = (int)Math.Round(ratio, MidpointRounding.AwayFromZero);
                    count -= value;
                    totalRatio -= questionNums[i];
                }
            }


            var questions = new List<QuestionViewModel>();
            for (var id = 0; id < ratios.Count; id++)
            {
                var topic = topicList[id];

                var totalQuestionInTopic = ratios[id];

                for (var i = 0; i < totalQuestionInTopic; i++)
                {
                    var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(topic.Questions.Count - 1);
                    var question = topic.Questions[randomInt];
                    questions.Add(new QuestionViewModel(
                        question.Number, question.Year));
                    topic.Questions.RemoveAt(randomInt);
                }
            }

            foreach (var question in questions)
            {
                questionList.Add(question);
            }

            Size = questionList.Count;
            await loadQuestionAsync(1);

        }

        private async void createQuestionBankByTopic(string subjectName, ICollection<string> topics, int totalQuestion)
        {
            var topicList = await ContentResourceUtility.getTopicsAsync(subjectName, QuestionType.Objectives);
            topicList = topicList.Where(x => topics.Contains(x.Name)).ToList();
            var questionNums = new List<int>();
            var ratios = new List<int>();
            var totalRatio = topicList.Select(topic => topic.TotalQuestions).Sum(new Func<int, int>(x => x));
            var count = totalQuestion;

            foreach (var topic in topicList)
            {
                questionNums.Add(topic.TotalQuestions);
            }

            for(var i=0; i < questionNums.Count; i++)
            {
                if (i == questionNums.Count - 1)
                {
                    ratios.Add(count);
                }
                else
                {
                    var ratio = (questionNums[i] / Convert.ToDouble(totalRatio)) * count;
                    var value = (int)Math.Round(ratio, MidpointRounding.AwayFromZero);
                    ratios.Add(value);
                    //ratios[i] = (int)Math.Round(ratio, MidpointRounding.AwayFromZero);
                    count -= value;
                    totalRatio -= questionNums[i];
                }
            }

            

            //Integration begins here

            //var userId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37").ToString();

            //var questions = new List<QuestionViewModel>();
            //var failedQuestions = new Question[] { };


            using (var dal = new UnitOfWork())
            {
                var user = dal.UserRepository.SingleOrDefault(x => x.IsCurrent);
                if (user == null) return;

                for (var id = 0; id < ratios.Count; id++)
                {
                    var topic = topicList[id];

                    var questions = new List<QuestionViewModel>();
                    var failedQuestions = new Question[] { };

                    failedQuestions = dal.QuestionRepository.Get(x =>
                        x.Failed &&
                        x.SubjectName == subjectName &&
                        x.TopicName == topic.Name &&
                        x.UserId == user.Id.ToString()).ToArray();

                    var totalQuestionInTopic = ratios[id];

                    if (failedQuestions.Length >= totalQuestionInTopic)
                    {
                        for (var i = 0; i < totalQuestionInTopic; i++)
                        {
                            var item = failedQuestions[i];
                            questions.Add(new QuestionViewModel(
                                item.QuestionNumber,
                                ContentResourceUtility.getAnswer(subjectName, item.QuestionYear, item.QuestionNumber, QuestionType.Objectives), 
                                item.QuestionYear));
                        }
                    }
                    else
                    {
                        foreach (var item in failedQuestions)
                        {
                            questions.Add(new QuestionViewModel(
                                item.QuestionNumber,
                                ContentResourceUtility.getAnswer(subjectName, item.QuestionYear, item.QuestionNumber, QuestionType.Objectives),
                                item.QuestionYear));
                        }

                        var numberLeft = totalQuestionInTopic - failedQuestions.Length;

                        topic.Questions.RemoveAll(x =>
                                failedQuestions.ToList().Exists(item => item.QuestionNumber == Convert.ToInt32(x.Number)));
                   

                        for (var i = 0; i < numberLeft; i++)
                        {
                            var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(topic.Questions.Count - 1);
                            var question = topic.Questions[randomInt];
                            questions.Add(new QuestionViewModel(
                                Convert.ToInt32(question.Number),
                                ContentResourceUtility.getAnswer(subjectName, question.Year, Convert.ToInt32(question.Number), QuestionType.Objectives),
                                question.Year));
                            topic.Questions.RemoveAt(randomInt);
                        }
                    }

                    foreach (var question in questions)
                    {
                        questionList.Add(question);
                    }
                }
            }

            Size = questionList.Count;
            await loadQuestionAsync(1);
        }


        public QuestionViewModel getQuestion(int questionIndex)
        {
            return questionIndex < questionList.Count && questionIndex >= 0 ? questionList[questionIndex] : null;
        }

        public void loadQuestion(int questionNumber)
        {
            if (currentQuestion != null)
            {
                currentQuestion.Tag = "";
            }
            currentQuestion = getQuestion(questionNumber - 1);
            currentQuestion.Tag = "Active";
            CurrentQuestionNumber = questionNumber;
            CurrentQuestionData = currentQuestion?.loadQuestionData(subjectName, _questionType);
         
        }

        private bool isCorrection { get; set; }

        public bool IsCorrection
        {
            get { return isCorrection; }
            set
            {
                isCorrection = value;
                OnPropertyChanged("IsCorrection");
            }
        }

        public async Task loadQuestionAsync(int questionNumber)
        {

            //if (currentQuestion != null)
            //{
            //    currentQuestion.Tag = "";
            //}


            if (isCorrection)
            {
                if (currentQuestion != null)
                {
                    currentQuestion.Tag = currentQuestion.isAnsweredCorrectly ? "IsPassed" : "IsFailed";
                }
            }
            else
            {
                if (currentQuestion != null)
                {
                    currentQuestion.Tag = "";
                }
            }

            currentQuestion = getQuestion(questionNumber - 1);
            currentQuestion.Tag = "Active";
            CurrentQuestionNumber = questionNumber;
            CurrentQuestionData = await currentQuestion?.loadQuestionDataAsync(subjectName, _questionType, questionBankService);
            // Radio Button Status
            if (CurrentQuestionData != null)
            {
                if(_questionType == QuestionType.Theory) return;
                foreach (var option in CurrentQuestionData.Options)
                {
                    option.IsActive = !IsCorrection;
                    option.IsCorrection = IsCorrection;
                    option.IsAnswer = option.Key == currentQuestion.CorrectAnswer;
                    option.IsAnsweredCorrectly = currentQuestion.isAnsweredCorrectly;
                }
            }
        }


        public void SetButtons()
        {
            foreach (var question in QuestionList)
            {
               
            }
        }

    }

}
