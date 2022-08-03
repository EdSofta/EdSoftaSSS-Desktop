using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class LRViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private NotifyTaskCompletion<List<LearningRecommendation>> localRecommendations { get; set; }

        public NotifyTaskCompletion<List<LearningRecommendation>> LocalRecommendations
        {
            get { return localRecommendations; }
            set
            {
                localRecommendations = value;
                OnPropertyChanged("LocalRecommendations");
            }
        }

        private NotifyTaskCompletion<List<Performance>> performanceReport { get; set; }

        public NotifyTaskCompletion<List<Performance>> PerformanceReport
        {
            get { return performanceReport; }
            set
            {
                performanceReport = value;
                OnPropertyChanged("PerformanceReport");
            }
        }

        private NotifyTaskCompletion<int> totalTestsTaken { get; set; }
        public  NotifyTaskCompletion<int> TotalTestsTaken
        {
            get { return totalTestsTaken; }
            set
            {
                totalTestsTaken = value;
                OnPropertyChanged("TotalTestsTaken");
            }
        }

        private NotifyTaskCompletion<string> testsCompletedOnTime { get; set; }
        public NotifyTaskCompletion<string> TestsCompletedOnTime
        {
            get { return testsCompletedOnTime; }
            set
            {
                testsCompletedOnTime = value;
                OnPropertyChanged("TestsCompletedOnTime");
            }
        }

        private bool isRecOpen { get; set; }
        public bool IsRecOpen
        {
            get { return isRecOpen; }
            set
            {
                isRecOpen = value;
                OnPropertyChanged("IsRecOpen");
            }
        }

        private bool isReportOpen { get; set; }
        public bool IsReportOpen
        {
            get { return isReportOpen; }
            set
            {
                isReportOpen = value;
                OnPropertyChanged("IsReportOpen");
            }
        }

        private bool isFirstAvailable { get; set; }
        public bool IsFirstAvailable
        {
            get { return isFirstAvailable; }
            set
            {
                isFirstAvailable = value;
                OnPropertyChanged("IsFirstAvailable");
            }
        }

        private LearningRecommendation firstLRItem { get; set; }

        public LearningRecommendation FirstLRItem
        {
            get { return firstLRItem; }
            set
            {
                firstLRItem = value;
                OnPropertyChanged("FirstLRItem");
            }
        }

        private readonly ILRecService _lRecService;

        public LRViewModel(ILRecService lRecService)
        {
            _lRecService = lRecService;
            LocalRecommendations = new NotifyTaskCompletion<List<LearningRecommendation>>(lRecService.getLearningRecommendationsAsync(),
                OnLRLoaded);

            TotalTestsTaken = new NotifyTaskCompletion<int>(lRecService.getResultsCountAsync());
            TestsCompletedOnTime = new NotifyTaskCompletion<string>(lRecService.getTestFinishedOnTimeAsync());
            PerformanceReport = new NotifyTaskCompletion<List<Performance>>(lRecService.getSubjectPerformancesAsync());

        }


        public void OnLRLoaded(object sender, TaskCompletedEventArgs e)
        {
            var recommendations = (NotifyTaskCompletion<List<LearningRecommendation>>) sender;
            if(recommendations == null) return;
            if (!recommendations.IsSuccessfullyCompleted) return;
            SetFirstLR(recommendations.Result);
        }

        public void SetFirstLR(List<LearningRecommendation> recommendations)
        {
            if (recommendations.Count == 0) return;
            FirstLRItem = recommendations[0];
            IsFirstAvailable = true;
        }

        //public async Task removeRecommendation(LearningRecommendation lRec)
        //{
        //    var isSuccessful = await _lRecService.deleteRecommendation(lRec.Id.ToString());
        //    if (!isSuccessful) return;
        //    LocalRecommendations.Result.Remove(lRec);
        //    FirstLRItem = null;
        //    IsFirstAvailable = false;
        //    SetFirstLR(LocalRecommendations.Result);

        //}

    }
}
