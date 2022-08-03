using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;
using TheArtOfDev.HtmlRenderer.WPF;

namespace EdSofta.ViewModels.GameViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class GameQuestionBankViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public GameQuestionBankViewModel() { }


        private GameQuestionViewModel currentQuestionData;

        public GameQuestionViewModel CurrentQuestionData
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

        public int size { get; set; }
        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged("Size");
            }
        }

        public void loadQuestion(int questionNumber, string subject)
        {
            if (CurrentQuestionData != null)
            {
                passFlags[CurrentQuestionNumber - 1] = CurrentQuestionData.Passed;
            }

            var question = GameResourceUtility.getQuestion(subject, questionNumber);
            CurrentQuestionNumber += 1;
            CurrentQuestionData = new GameQuestionViewModel
            {
                Question = question.Question.formatHtmlText(),
                Answer = question.Answer.formatHtmlText(),
                OptionA = question.Options.Find(x => x.Key == "A").Value.formatHtmlText(),
                OptionB = question.Options.Find(x => x.Key == "B").Value.formatHtmlText(),
                OptionC = question.Options.Find(x => x.Key == "C").Value.formatHtmlText(),
                OptionD = question.Options.Find(x => x.Key == "D").Value.formatHtmlText()
            };
        }

        private readonly Dictionary<string, string> _questionDictionary;
        private List<bool> passFlags = new List<bool>();

        public GameQuestionBankViewModel(Dictionary<string, string> questionDictionary)
        {
            _questionDictionary = questionDictionary;
            Size = _questionDictionary.Count;
            for (var i = 0; i < Size; i++)
            {
                passFlags.Add(false);
            }
            loadQuestionIndex(0);
        }


        public void loadQuestionIndex(int index)
        {
            loadQuestion(GameResourceUtility.getQuestionNumber(
                    _questionDictionary.ElementAt(index).Key),
                _questionDictionary.ElementAt(index).Value);
        }

        public bool questionLimitPassed(int limit)
        {
            var count = passFlags.Count(x => x);
            return count >= limit;
        }

        public int QuestionsPassed => passFlags.Count(x => x);

    }
}
