using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using Newtonsoft.Json;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    class StudyMaterialsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private StudyMaterialDataViewModel selectedItem;
        public StudyMaterialDataViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (selectedItem == null) IsItemSelected = false;
                OnPropertyChanged("SelectedItem");
            }
        }

        private StudyViewModel selectedMaterials { get; set; }

        public StudyViewModel SelectedMaterials
        {
            get { return selectedMaterials; }
            set
            {
                selectedMaterials = value;
                SelectedItem = null;
                Data = null;
                OnPropertyChanged("SelectedMaterials");
            }
        }

        private string data { get; set; }

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                OnPropertyChanged("Data");
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

        private bool isPracticeAvailable { get; set; }

        public bool IsPracticeAvailable
        {
            get { return isPracticeAvailable; }
            set
            {
                isPracticeAvailable = value;
                OnPropertyChanged("IsPracticeAvailable");
            }
        }

        public NotifyTaskCompletion<List<string>> SubjectList { get; set; }

        private NotifyTaskCompletion<List<StudyMaterialDataViewModel>> studyMaterials { get; set; }

        public NotifyTaskCompletion<List<StudyMaterialDataViewModel>> StudyMaterials
        {
            get { return studyMaterials; }
            set
            {
                studyMaterials = value;
                OnPropertyChanged("StudyMaterials");
            }
        }

        private ObservableCollection<StudyMaterialDataViewModel> currentStudyMaterials { get; set; }

        public ObservableCollection<StudyMaterialDataViewModel> CurrentStudyMaterials
        {
            get { return currentStudyMaterials; }
            set
            {
                currentStudyMaterials = value;
                OnPropertyChanged("CurrentStudyMaterials");
            }
        }

        private NotifyTaskCompletion<List<StudyViewModel>> studyMaterialsList { get; set; }

        public NotifyTaskCompletion<List<StudyViewModel>> StudyMaterialsList
        {
            get { return studyMaterialsList; }
            set
            {
                studyMaterialsList = value;
                OnPropertyChanged("StudyMaterialsList");
            }
        }


        private TTSMediaViewModel ttsMedia { get; set; }

        public TTSMediaViewModel TtsMedia
        {
            get { return ttsMedia; }
            set
            {
                ttsMedia = value;
                OnPropertyChanged("TtsMedia");
            }
        }

        private readonly IStudyMaterialService studyMaterialService;
        public StudyMaterialsViewModel(IStudyMaterialService studyMaterialService)
        {
            this.studyMaterialService = studyMaterialService;
            StudyMaterialsList = new NotifyTaskCompletion<List<StudyViewModel>>(studyMaterialService.getAllStudyMaterials(), OnMaterialsLoaded);
        }

        private void OnMaterialsLoaded(object sender, TaskCompletedEventArgs e)
        {
            var materials = (NotifyTaskCompletion<List<StudyViewModel>>)sender;
            if (materials.Result == null) return;
            if (materials.Result.Count == 0) return;

            SelectedMaterials = materials.Result[0];
        }


        public StudyMaterialsViewModel(IStudyMaterialService studyMaterialService, LearningRecommendation lRec)
        {
            this.studyMaterialService = studyMaterialService;
            _lRec = lRec;
            StudyMaterialsList = new NotifyTaskCompletion<List<StudyViewModel>>(studyMaterialService.getAllStudyMaterials(), OnAllMaterialsLoaded);

        }

        private async void OnAllMaterialsLoaded(object sender, TaskCompletedEventArgs e)
        {
            var materials = (NotifyTaskCompletion<List<StudyViewModel>>) sender;
            if (materials.Result == null) return;
            if(materials.Result.Count == 0) return;
            var studyData = JsonConvert.DeserializeObject<Study>(_lRec.Data);
            if (studyData == null) return;

            var materialItem = materials.Result.SingleOrDefault(x => x.Subject == studyData.Subject);
            if (materialItem == null) return;

            SelectedMaterials = materialItem;
            SelectedItem = SelectedMaterials.StudyMaterials.SingleOrDefault(x => x.Name == studyData.Topic);

            var recService = new LRecService();
            await recService.deleteRecommendation(_lRec);

        }


        private LearningRecommendation _lRec;
       

        public void SetList(string subject)
        {
             //StudyMaterials =
             //new NotifyTaskCompletion<List<StudyMaterialDataViewModel>>(studyMaterialService.getStudyMaterialsAsync(subject));
             if (!StudyMaterialsList.IsSuccessfullyCompleted) return;
             var materials = StudyMaterialsList.Result.SingleOrDefault(x => x.Subject == subject);
             if (materials == null) return;
             SelectedMaterials = materials;
             SelectedItem = null;
             Data = null;
        }


        public async Task SetStudyData(string subject, StudyMaterialDataViewModel dataViewModel)
        {
            if (dataViewModel == null) return;
            Data = await studyMaterialService.getStudyMaterial(subject, dataViewModel.Name);
            IsPracticeAvailable = await studyMaterialService.isTopicPracticeAvailableAsync(subject, dataViewModel.Name);
            IsItemSelected = SelectedItem != null;
            TtsMedia?.CancelAll();
            TtsMedia = new TTSMediaViewModel(studyMaterialService.formatStudyText(Data));
        }
    }
}
