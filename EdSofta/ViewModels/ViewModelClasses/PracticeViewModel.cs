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

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class PracticeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public NotifyTaskCompletion<List<QuestionBank>> QuestionBanks { get; set; }

        
        private NotifyTaskCompletion<ResultViewModel> resultViewModel { get; set; }

        public NotifyTaskCompletion<ResultViewModel> ResultViewModel
        {
            get { return resultViewModel; }
            set
            {
                resultViewModel = value;
                OnPropertyChanged("ResultViewModel");
            }
        }

        private int percentage { get; set; }

        public int Percentage
        {
            get
            {
                if (ResultViewModel != null && ResultViewModel.IsSuccessfullyCompleted)
                    return ResultViewModel.Result.Percentage;

                return 0;
            }
        }

        private bool withTimer { get; set; }

        public bool WithTimer
        {
            get { return withTimer; }
            set
            {
                withTimer = value;
                OnPropertyChanged("WithTimer");
            }
        }

        private double timeLeft { get; set; }

        public double TimeLeft
        {
            get { return timeLeft; }
            set
            {
                timeLeft = value;
                OnPropertyChanged("TimeLeft");
            }
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

        private string comment { get; set; }

        public string Comment
        {
            get { return comment; }
            set
            {
                comment = value;
                OnPropertyChanged("Comment");
            }
        }

        public PracticeViewModel(IEnumerable<QuestionBankViewModel> questionBanks, IPracticeService practiceService, 
            PracticeType type, PracticeMode practiceMode)
        {
            WithTimer = practiceMode.withTimer;
            TimeLeft = practiceMode.timeValue;
            QuestionBanks = new NotifyTaskCompletion<List<QuestionBank>>(practiceService.generateQuestionBanksAsync(questionBanks, type));
        }

        public void setResult(IPracticeService practiceService, PracticeType type, PracticeMode practiceMode)
        {
            if (IsCorrection) return;

            IsCorrection = true;
            practiceMode.timeUsed = practiceMode.timeValue - TimeLeft;
            ResultViewModel =
                new NotifyTaskCompletion<ResultViewModel>(practiceService.markTestAsync(QuestionBanks.Result, type, practiceMode), OnResultGenerated);

        }

        private async void OnResultGenerated(object sender, TaskCompletedEventArgs e)
        {
            var results = (NotifyTaskCompletion<ResultViewModel>)sender;
            if (results.Result == null) return;
            var userService = new UserService();
            var user = await userService.getCurrentUser();
            var perc = results.Result.Percentage;

            var comment = UtilityClass.getGradeComment(perc);

            Comment = $"{comment}, {user.FirstName}";
        }
    }
}
