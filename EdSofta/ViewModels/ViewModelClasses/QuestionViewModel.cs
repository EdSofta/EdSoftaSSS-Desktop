using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class QuestionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        
        public int questionNumber { get; }

        public string year { get; }

        public string topic { get; set; }

        public string correctAnswer { get; set; }

        public string CorrectAnswer
        {
            get { return correctAnswer; }
            set
            {
                correctAnswer = value;
                OnPropertyChanged("CorrectAnswer");
            }
        }

        private string selectedAnswer { get; set; }
        public string SelectedAnswer
        {
            get { return selectedAnswer; }
            set
            {
                selectedAnswer = value;
                OnPropertyChanged("SelectedAnswer");
            }
        }


        private bool isAnswered;
        public bool IsAnswered
        {
            get { return isAnswered; }
            set
            {
                isAnswered = value;
                OnPropertyChanged("IsAnswered");
            }
        }

        private string tag;

        public string Tag
        {
            get { return tag; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) && isAnswered)
                {
                    tag = "Answered";
                }
                else
                {
                    tag = value;
                }
                OnPropertyChanged("Tag");
            }
        }
        public QuestionViewModel(int questionNumber, string correctAnswer, string year)
        {
            this.questionNumber = questionNumber;
            this.correctAnswer = correctAnswer;
            this.year = year;
        }

        public QuestionViewModel(int questionNumber, string year)
        {
            this.questionNumber = questionNumber;
            this.year = year;
        }

        public bool isAnsweredCorrectly => isAnswered && selectedAnswer == correctAnswer;

        public int Mark => isAnsweredCorrectly ? 1 : 0;

        public QuestionDTO loadQuestionData(string subject, QuestionType type)
        {
            return ContentResourceUtility.getQuestion(subject, year, questionNumber, type);
            //return AppResources.getQuestion("English", "2015", "2", type);
        }

        public async Task<QuestionDTO> loadQuestionDataAsync(string subject, QuestionType type, IQuestionBankService questionBankService)
        {
            return await questionBankService.getQuestionAsync(subject, year, questionNumber, type);
        }


    }
}
