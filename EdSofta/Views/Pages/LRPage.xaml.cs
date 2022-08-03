using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EdSofta.Constants;
using EdSofta.Interfaces;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for LRPage.xaml
    /// </summary>
    public partial class LRPage : Page
    {
        public LRPage()
        {
            InitializeComponent();
        }

        private readonly Frame _parentFrame;
        private readonly Frame _outerFrame;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private LRViewModel _lrViewModel;
        private ILRecService _lRecService = new LRecService();

        public LRPage(Frame parentFrame, Frame outerFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _outerFrame = outerFrame;
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            //_navService = _parentFrame.NavigationService;
            //_navService.Navigating += Navigating;
            _lrViewModel = new LRViewModel(_lRecService);
            DataContext = _lrViewModel;

        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            //if (_navService == null) return;
            //_navService.Navigating -= Navigating;
            //_navService = null;
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                parent.RaiseEvent(eventArg);
            }
        }

        private void RecExpand_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _lrViewModel.IsRecOpen = !_lrViewModel.IsRecOpen;
        }

        private void ReportExpand_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _lrViewModel.IsReportOpen = !_lrViewModel.IsReportOpen;
        }

        private async void OpenRecommendation_OnClick(object sender, RoutedEventArgs e)
        {
            var recItem = ((Button) sender).DataContext as LearningRecommendation;
            //await _lrViewModel.removeRecommendation(recItem);
            if (recItem == null) return;

            if (recItem.Type == LRType.Test)
            {
                openPracticeRec(recItem);
            }
            else
            {
                openStudyRec(recItem);
            }
        }

        private void openPracticeRec(LearningRecommendation lRec)
        {
            var page = new PracticeOnboardPage(_parentFrame, lRec);
            _parentFrame.Navigate(page);
        }

        private void openStudyRec(LearningRecommendation lRec)
        {
            var page = new StudyMaterialsViewPage(_outerFrame, new StudyMaterialService(), lRec);
            _outerFrame.Navigate(page);
        }

        private async void FirstLRButton_OnClick(object sender, RoutedEventArgs e)
        {
            //await _lrViewModel.removeRecommendation(_lrViewModel.FirstLRItem);

            if (_lrViewModel.FirstLRItem.Type == LRType.Test)
            {
                openPracticeRec(_lrViewModel.FirstLRItem);
            }
            else
            {
                openStudyRec(_lrViewModel.FirstLRItem);
            }
        }

        private void StartPracticeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var page = new PracticeExamSelectionPage(_parentFrame);
            _parentFrame.Navigate(page);
        }
    }
}
