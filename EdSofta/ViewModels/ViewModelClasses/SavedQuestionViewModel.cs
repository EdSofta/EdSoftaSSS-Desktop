using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class SavedQuestionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<string> filterList { get; set; }

        public ObservableCollection<string> FilterList
        {
            get { return filterList; }
            set
            {
                filterList = value;
                OnPropertyChanged("FilterList");
            }
        }

        public NotifyTaskCompletion<ObservableCollection<SavedQuestion>> SavedQuestions { get; set; }

        private SavedQuestion selectedItem;
        public SavedQuestion SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        private NotifyTaskCompletion<QuestionDTO> questionData { get; set; }
        public NotifyTaskCompletion<QuestionDTO> QuestionData
        {
            get { return questionData; }
            set
            {
                questionData = value;
                OnPropertyChanged("QuestionData");
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

        public SavedQuestionViewModel(ISavedQuestionService service)
        {
            SavedQuestions = 
                new NotifyTaskCompletion<ObservableCollection<SavedQuestion>>
                    (service.getSavedQuestionsAsync(), SetFilterOnTaskCompletionEvent);

        }

        private List<string> getAvailableSubjects()
        {
            if (SavedQuestions.Result == null) return new List<string>();

            var availableSubjects = new HashSet<string>();
            return availableSubjects.Union(SavedQuestions.Result.Select(x => x.Subject)).ToList();
        }

        public void SetFilterOnTaskCompletionEvent(object sender, TaskCompletedEventArgs e)
        {
            var savedQuestions = (NotifyTaskCompletion<ObservableCollection<SavedQuestion>>) sender;
            if (savedQuestions.Result == null) return;

            var subjects = getAvailableSubjects();
            var filterOptions = new List<string> {"All subjects"};
            filterOptions.AddRange(subjects);
            FilterList = filterOptions.ToObservableCollection();
        }

        public bool RefreshFilters()
        {
            var subjects = getAvailableSubjects();

            var abs = FilterList.Where(item =>
                !item.Equals("All subjects", StringComparison.OrdinalIgnoreCase))
                .Where(item => !subjects.Contains(item)).ToList();

            foreach (var item in abs)
            {
                FilterList.Remove(item);
            }


            return abs.Count > 0;
        }

        public void SetQuestionData(SavedQuestion savedQuestion)
        {
            if (savedQuestion == null) return;

            QuestionData = new NotifyTaskCompletion<QuestionDTO>(
                new QuestionBankService().getSavedQuestionAsync(savedQuestion.Subject, savedQuestion.Year, savedQuestion.Number, ExamQuestionType.getTypeEnum(savedQuestion.Type))
                );

            IsItemSelected = SelectedItem != null;
        }

        public void ResetData()
        {
            SelectedItem = null;
            IsItemSelected = false;
        }
    }
}
