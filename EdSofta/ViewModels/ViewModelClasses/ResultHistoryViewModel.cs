using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Interfaces;
using EdSofta.ViewModels.Utility;
using EdSofta.Services;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class ResultHistoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private NotifyTaskCompletion<ObservableCollection<ResultViewModel>> results { get; set; }

        public NotifyTaskCompletion<ObservableCollection<ResultViewModel>> Results
        {
            get { return results; }
            set
            {
                results = value;
                OnPropertyChanged("Results");
            }
        }

        private ResultViewModel selectedResult { get; set; }

        public ResultViewModel SelectedResult
        {
            get { return selectedResult; }
            set
            {
                selectedResult = value;
                OnPropertyChanged("SelectedResult");
            }
        }

        private bool isItemSelected { get; set; }

        public bool IsItemSelected
        {
            get { return isItemSelected; }
            set
            {
                isItemSelected = value;
                OnPropertyChanged("IsItemSelected");
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

        private IResultService _resultService;

        public ResultHistoryViewModel(IResultService resultService)
        {
            _resultService = resultService;
            Results = new NotifyTaskCompletion<ObservableCollection<ResultViewModel>>(_resultService.getResultsAsync());
        }

        public async Task SetResult(ResultViewModel result)
        {
            SelectedResult = result;
            IsItemSelected = result != null;
            if (SelectedResult != null) await setResultComment();   
        }


        private async Task setResultComment()
        {
            var userService = new UserService();
            var user = await userService.getAdminUser();
            var perc = SelectedResult.Percentage;

            var comment = UtilityClass.getGradeComment(perc);
            Comment = $"{comment}, {user.FirstName}";
        }

        public async Task deleteResults()
        {
            var isSuccessful = await  _resultService.deleteResultsAsync();
            if (!isSuccessful) return;
            Results.Result.Clear();
            IsItemSelected = false;
            SelectedResult = null;
        }
    }
}
