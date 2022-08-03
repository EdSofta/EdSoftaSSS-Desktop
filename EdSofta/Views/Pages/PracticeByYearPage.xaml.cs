using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for PracticeByYearPage.xaml
    /// </summary>
    public partial class PracticeByYearPage : Page
    {
        
        public PracticeByYearPage()
        {
            InitializeComponent();
            Loaded += PageLoaded;
            //var questionBanks = AppResources.getSubjects(QuestionType.Objectives)?.Select(x => new QuestionBankViewModel
            //{
            //    Name = x,
            //    Years = AppResources.getYears(x, QuestionType.Objectives).ToObservableCollection(),
            //    Questions = new ObservableCollection<string>{"10", "20"}
            //});

            //if (questionBanks != null)
            //{
            //    _allQuestionBanks.AddRange(questionBanks);
            //}

            //DataContext = _allQuestionBanks.ToObservableCollection();
        }


        
        private bool _loaded;
        private PracticeMode _practiceMode;
        private readonly Frame _parentFrame;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private readonly IPracticeModeService _practiceModeService;
        private List<QuestionBankViewModel> _allQuestionBanks = new List<QuestionBankViewModel>();
        private PracticeSelectionViewModel _practiceSelectionViewModel;
        private QuestionType _questionType;

        internal PracticeByYearPage(Frame parentFrame, IPracticeModeService practiceModeService, PracticeMode practiceMode, QuestionType questionType)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _practiceMode = practiceMode;
            _practiceModeService = practiceModeService;
            _questionType = questionType;

            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private async void PageLoaded(object sender, EventArgs e)
        {

            if (!_loaded)
            {
                _loaded = true;
                _practiceSelectionViewModel = new PracticeSelectionViewModel(_practiceModeService, _questionType);
                DataContext = _practiceSelectionViewModel;
            }

            _navService = _parentFrame.NavigationService;
            _navService.Navigating += Navigating;
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_navService == null) return;
            _navService.Navigating -= Navigating;
            _navService = null;
        }

        private void Navigating(object sender, NavigatingCancelEventArgs e)
        {
            _navEventArgs = e;

            if (e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;

                var storyboard1 = Application.Current.FindResource("Animate.SlideOutDownDelayedMid");
                var storyboard2 = Application.Current.FindResource("Animate.SlideOutDownDelayedMid");
                if (storyboard1 == null || storyboard2 == null)
                {
                    if (_navService != null) _navService.Navigating -= Navigating;
                    if (_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();
                    return;
                }

                var myStoryboard1 = (Storyboard)storyboard1;
                var myStoryboard2 = (Storyboard)storyboard2;
                myStoryboard2.Completed += new EventHandler(AnimationCompleted);
                myStoryboard1.Begin(HeaderGrid);
                myStoryboard2.Begin(ListGrid);
            }

        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            if (_parentFrame.NavigationService.CanGoBack)
            {
                if (_navService != null) _navService.Navigating -= Navigating;
                _parentFrame.NavigationService.GoBack();

            }
        }

        private void SubjectCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {

            var questionBank = ((CheckBox)sender).DataContext as QuestionBankViewModel;
            if (questionBank == null) return;
            if (_practiceSelectionViewModel.QuestionBanks.Result.Count(x => x.IsSelected) > 1)
            {
                questionBank.IsSelected = false;
            }
        }

        private void SubjectCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var questionBank = ((CheckBox)sender).DataContext as QuestionBankViewModel;
            if (questionBank == null) return;
            questionBank.ShuffleQuestions = false;

            var comboPanel = (Grid)((CheckBox)sender).GetParentOfType<StackPanel>().Children[1];
            foreach (var child in ((StackPanel)comboPanel.Children[0]).Children)
            {
                if ((child as ComboBox) == null) continue;
                ((ComboBox)child).SelectedIndex = 0;
            }

        }


        void item_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item = (ComboBoxItem)sender;
            item.IsSelected = true;
            var comboBox = ItemsControl.ItemsControlFromItemContainer(item) as ComboBox;
            if (comboBox != null)
            {
                comboBox.IsDropDownOpen = false;
            }

        }

        private void StartExamButton_OnClick(object sender, RoutedEventArgs e)
        {
            var testWindow = new TestingWindow(_practiceSelectionViewModel.QuestionBanks.Result, PracticeType.ByYear, _practiceMode, _questionType);
            testWindow.Owner = System.Windows.Application.Current.MainWindow;
            testWindow.ShowDialog();
        }

        private async void YearsComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var questionBank = ((ComboBox)sender).DataContext as QuestionBankViewModel;
            if (questionBank == null) return;
            if (questionBank.SelectedQuestionType == QuestionType.Objectives) return;
            await questionBank.updateSelectedQuestionsAsync(questionBank.SelectedYear);
        }
    }
}
