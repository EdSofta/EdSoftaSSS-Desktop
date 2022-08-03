using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    internal class QuestionDTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Question { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Topic { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Answer { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Explanation { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Difficulty { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public List<Option> Options { get; set; }

        private bool isTheoryAnswerVisible { get; set; }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool IsTheoryAnswerVisible
        {
            get { return isTheoryAnswerVisible; }
            set
            {
                isTheoryAnswerVisible = value;
                OnPropertyChanged("IsTheoryAnswerVisible");
            }
        }
    }

    internal class Option : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Key { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public string Value { get; set; }

        private bool isAnsweredCorrectly { get; set; }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool IsAnsweredCorrectly
        {
            get { return isAnsweredCorrectly; }
            set
            {
                isAnsweredCorrectly = value;
                OnPropertyChanged("IsAnsweredCorrectly");
            }
        }

        private bool isAnswer { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool IsAnswer 
        {
            get { return isAnswer; }
            set
            {
                isAnswer = value;
                OnPropertyChanged("IsAnswer");
            }
        }

        private bool isCorrection { get; set; }

        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool IsCorrection
        {
            get { return isCorrection; }
            set
            {
                isCorrection = value;
                OnPropertyChanged("IsCorrection");
            }
        }

        private bool isActive { get; set; }
        [Obfuscation(Feature = "renaming", Exclude = true)]
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive");
            }
        }
    }
}
