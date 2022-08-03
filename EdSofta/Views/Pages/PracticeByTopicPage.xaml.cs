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
    /// Interaction logic for PracticeByTopicPage.xaml
    /// </summary>
    public partial class PracticeByTopicPage : Page
    {
       // private readonly ObservableCollection<QuestionBankViewModel> _allQuestionBanks = new ObservableCollection<QuestionBankViewModel>();
        public PracticeByTopicPage()
        {
            InitializeComponent();
            Loaded += PageLoaded;
            
        }

        private bool _loaded;
        private PracticeMode _practiceMode;
        private readonly Frame _parentFrame;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private readonly IPracticeModeService _practiceModeService;
        private PracticeSelectionViewModel _practiceSelectionViewModel;
        private QuestionType _questionType;


        internal PracticeByTopicPage(Frame parentFrame, IPracticeModeService practiceModeService, PracticeMode practiceMode, QuestionType questionType)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _practiceMode = practiceMode;
            _practiceModeService = practiceModeService;
            _questionType = questionType;
            _practiceSelectionViewModel = new PracticeSelectionViewModel(practiceModeService, questionType);
            DataContext = _practiceSelectionViewModel;
            Loaded += PageLoaded;
            //Unloaded += PageUnloaded;
        }

        public void PageLoaded(object sender, EventArgs e)
        {
            
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
            if (questionBank.Topics == null) return;
            foreach (var topic in questionBank.Topics.Where(x => !x.IsSelected))
            {
                topic.IsSelected = true;
            }

            var comboPanel = (Grid)((CheckBox)sender).GetParentOfType<StackPanel>().Children[1];
            foreach (var child in ((StackPanel)comboPanel.Children[0]).Children)
            {
                if ((child as ComboBox) == null) continue;
                ((ComboBox)child).SelectedIndex = 0;
            }
        }

        private async void SelectTopicButton_OnClick(object sender, RoutedEventArgs e)
        {
            var item = ((Button)sender).DataContext as QuestionBankViewModel;
            if (item == null) return;

            LandingPage mainWindow = null;
            var currentParent = VisualTreeHelper.GetParent(this);
            while (currentParent != null && mainWindow == null)
            {
                mainWindow = currentParent as LandingPage;
                currentParent = VisualTreeHelper.GetParent(currentParent);
            }

            if (mainWindow == null) return;
            mainWindow.TopicModalHeader.Text = $"Select {item.Name} topics";
            mainWindow.TopicListView.ItemsSource = null;
            mainWindow._questionBankViewModel = null;
            mainWindow._modals.SetModal(mainWindow.TopicSelectionModal.Name);

            if (item.Topics == null)
            {
                //var allTopics = await ContentResourceUtility.getTopicsAsync(item.Name, QuestionType.Objectives);
                //var topicList = new ObservableCollection<TopicViewModel>(allTopics.Select(topic => new TopicViewModel { TopicName = topic.Name, IsSelected = true }));
                var topicList = 
                    await ((PracticeByTopicService) _practiceModeService).getTopicsViewModelAsync(item.Name, QuestionType.Objectives);
                item.Topics = new ObservableCollection<TopicViewModel>(topicList);
            }

            mainWindow.TopicListView.ItemsSource = item.Topics;
            mainWindow._questionBankViewModel = item;

            if (item.Topics != null)
            {
                mainWindow.SelectAllCheckBox.IsChecked = !item.Topics.ToList().Exists(x => !x.IsSelected);
            }
        }

        private async Task validateSelections()
        {
            
            foreach (var item in _practiceSelectionViewModel.QuestionBanks.Result.Where(x => x.IsSelected && x.Topics == null))
            {
                var topicList =
                    await((PracticeByTopicService)_practiceModeService).getTopicsViewModelAsync(item.Name, QuestionType.Objectives);
                item.Topics = new ObservableCollection<TopicViewModel>(topicList);
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

        private async void StartExamButton_OnClick(object sender, RoutedEventArgs e)
        {
            var questionViewModel = _practiceSelectionViewModel.QuestionBanks.Result.Find(x => x.IsSelected && x.SelectedQuestions == 0);
            if (questionViewModel != null)
            {
                var dialog = new DialogWindow("Topics not selected",
                    $"Please select at least one topic from {questionViewModel.Name}", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
                return;
            }

            await validateSelections();
            var testWindow = new TestingWindow(_practiceSelectionViewModel.QuestionBanks.Result, PracticeType.ByTheme, _practiceMode, _questionType);
            testWindow.Owner = System.Windows.Application.Current.MainWindow;
            testWindow.ShowDialog();
        }
    }
}
