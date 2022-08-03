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
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;
using Newtonsoft.Json;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for StudyMaterialsViewPage.xaml
    /// </summary>
    public partial class StudyMaterialsViewPage : Page
    {
        public StudyMaterialsViewPage()
        {
            InitializeComponent();
        }

        private bool _loaded;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private readonly Frame _parentFrame;
        private readonly Frame _innerFrame;
        private LandingPageViewModel _landingPageViewModel;
        private readonly IStudyMaterialService _studyMaterialService;
        private readonly StudyMaterialsViewModel _studyMaterialsViewModel;
        private LearningRecommendation _lRec;
        

        internal StudyMaterialsViewPage(Frame parentFrame, IStudyMaterialService studyMaterialService, Frame innerFrame, LandingPageViewModel landingPageViewModel)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _innerFrame = innerFrame;
            _landingPageViewModel = landingPageViewModel;
            _studyMaterialService = studyMaterialService;
            _studyMaterialsViewModel = new StudyMaterialsViewModel(studyMaterialService);
            DataContext = _studyMaterialsViewModel;
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        internal StudyMaterialsViewPage(Frame parentFrame, IStudyMaterialService studyMaterialService, LearningRecommendation lRec)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _studyMaterialService = studyMaterialService;
            _lRec = lRec;
            _studyMaterialsViewModel = new StudyMaterialsViewModel(studyMaterialService, lRec);
            DataContext = _studyMaterialsViewModel;
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

      


        private async void PageLoaded(object sender, EventArgs e)
        {

            if (!_loaded)
            {
                _loaded = true;
            }

            //_navService = _parentFrame.NavigationService;
            //_navService.Navigating += Navigating;
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            //_navService.Navigating -= Navigating;
            //_navService = null;
        }


        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_parentFrame.CanGoBack) return;
            _parentFrame.NavigationService.GoBack();
        }

        //private void SubjectComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var comboBox = (ComboBox)sender;
        //    //var selectedItem = comboBox?.SelectedItem;
        //    var selectedItem = _studyMaterialsViewModel.SelectedMaterials;
        //    if (selectedItem == null) return;
        //    _studyMaterialsViewModel.SetList(((StudyViewModel)selectedItem).Subject);
        //}

        private void item_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (ComboBoxItem)sender;
            item.IsSelected = true;
            var comboBox = ItemsControl.ItemsControlFromItemContainer(item) as ComboBox;
            if (comboBox != null)
            {
                comboBox.IsDropDownOpen = false;
            }
        }

        private void ListenButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_studyMaterialsViewModel.Data == null) return;
            MediaGrid.Visibility = Visibility.Visible;
        }

        private async void StudyMaterialsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var item = StudyMaterialsList.SelectedItem as StudyMaterialDataViewModel;

            var subject = SubjectComboBox.SelectedItem;
            if (item == null || subject == null) return;

            if (!ViewModels.Utility.App.IsActivated && ((StudyViewModel) subject).Subject != "English")
            {

                var title = "App not activated";
                var message = "To have access to all the study materials, you need to sctivate the app";

                DialogWindow dialog;
                dialog = new DialogWindow(title, message, "Activate now", "Later");

                dialog.Owner = Application.Current.MainWindow;
                var result = dialog.ShowDialog() ?? false;

                if (!result) return;

                if(_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();

                var page = new ActivationPage(_innerFrame, _landingPageViewModel);
                _innerFrame.Navigate(page);
                return;
            }

            _studyMaterialsViewModel.SelectedItem = item;

            MediaGrid.Visibility = Visibility.Collapsed;

            item.LastRead = DateTime.Now;
            await _studyMaterialService.logStudyMaterials(((StudyViewModel)subject).Subject, item);
            await _studyMaterialsViewModel.SetStudyData(((StudyViewModel)subject).Subject, item);
            ContentScrollViewer.ScrollToTop();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            MediaGrid.Visibility = Visibility.Collapsed;
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            _studyMaterialsViewModel.TtsMedia?.Stop();
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            _studyMaterialsViewModel.TtsMedia?.Play();
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _studyMaterialsViewModel.TtsMedia?.Pause();
        }

        private void RestartButton_OnClick(object sender, RoutedEventArgs e)
        {
            _studyMaterialsViewModel.TtsMedia?.Restart();
        }

        private void PracticeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var subject = SubjectComboBox.SelectedItem;
            if (_studyMaterialsViewModel.SelectedItem == null || subject == null) return;
            if (_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();


            var page = new PracticeOnboardPage(_innerFrame, ((StudyViewModel)subject).Subject,_studyMaterialsViewModel.SelectedItem.Name);
            _innerFrame.Navigate(page);
        }
    }
}
