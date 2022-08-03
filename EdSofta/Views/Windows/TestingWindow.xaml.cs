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
using System.Windows.Threading;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Views.Windows
{
    /// <summary>
    /// Interaction logic for TestingWindow.xaml
    /// </summary>
    public partial class TestingWindow : Window
    {
        public TestingWindow()
        {
            InitializeComponent();
        }

        private PracticeMode _practiceMode;
        private PracticeViewModel _practiceViewModel;
        internal readonly List<IModal> _modals = new List<IModal>();
        private readonly List<QuestionBank> _questionBanks = new List<QuestionBank>();
        private readonly IEnumerable<QuestionBankViewModel> _questionBankViewModels;
        private IPracticeService _practiceService;
        private CalculatorWindow calculator = new CalculatorWindow();

        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private QuestionType _questionType;

        internal TestingWindow(IEnumerable<QuestionBankViewModel> questionBankViewModels, PracticeType type,
            PracticeMode practiceMode)
        {
            InitializeComponent();
            SetModals();

            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _practiceMode = practiceMode;
            var bankViewModels = questionBankViewModels as QuestionBankViewModel[] ?? questionBankViewModels.ToArray();
            _questionBankViewModels = bankViewModels;
            _practiceViewModel = new PracticeViewModel(bankViewModels, _practiceService, type, practiceMode);
            DataContext = _practiceViewModel;
            Loaded += WindowLoaded;
        }

        internal TestingWindow(IEnumerable<QuestionBankViewModel> questionBankViewModels, PracticeType type,
            PracticeMode practiceMode, QuestionType questionType)
        {
            InitializeComponent();
            SetModals();
            if (questionType == QuestionType.Theory)
            {
                setActionText(false);
                ActionCheckCanvas.Visibility = Visibility.Collapsed;
            }

            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 1);

            _practiceMode = practiceMode;
            _questionType = questionType;
            var bankViewModels = questionBankViewModels as QuestionBankViewModel[] ?? questionBankViewModels.ToArray();
            _questionBankViewModels = bankViewModels;

            injectService(questionType);

            _practiceViewModel = new PracticeViewModel(bankViewModels, _practiceService, type, practiceMode);
            DataContext = _practiceViewModel;
            Loaded += WindowLoaded;
        }

        private void injectService(QuestionType questionType)
        {
            switch (questionType)
            {
                case QuestionType.Objectives:
                    _practiceService = new PracticeService();
                    break;
                case QuestionType.Theory:
                    _practiceService = new TheoryPracticeService();
                    break;
                default:
                    _practiceService = new PracticeService();
                    break;
            }
        }

        private void setActionText(bool isAnswerVisible)
        {
            ActionTextBlock.Text = isAnswerVisible ? "Hide Answer" : "Show Answer";
        }

        private void SetModals()
        {
            _modals.AddRange(
                new List<IModal>
                {
                    new Modal(ResultModal),
                    new Modal(ReportErrorModal)
                });
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            _practiceViewModel.TimeLeft -= 1;

            if ((int)_practiceViewModel.TimeLeft <= 0)
            {
                await submitTestAsync();
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            calculator.Owner = this;

            if (_practiceViewModel.WithTimer)
            {
                _timer.Start();
            }

        }


        private void PaginatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var questionBank = button.GetParentOfType<ListView>().DataContext as QuestionBank;
            if (questionBank == null) return;
            var question = button.DataContext as QuestionViewModel;
            if (question == null) return;
            navigateToQuestionAsync(questionBank, questionBank.QuestionList.IndexOf(question) + 1);

        }

        private void navigateToQuestion(QuestionBank questionBank, int questionNumber)
        {
            if (questionNumber > questionBank.Size || questionNumber <= 0) return;
            questionBank.loadQuestion(questionNumber);
        }

        private async void navigateToQuestionAsync(QuestionBank questionBank, int questionNumber)
        {
            if (questionNumber > questionBank.Size || questionNumber <= 0) return;

            if (questionNumber >= 6 && !ViewModels.Utility.App.IsActivated)
            {
                var title = "App not activated";
                var message = "To have access to all the questions, you have to activate the app. This can be done from the home page.";

                DialogWindow dialog;
                dialog = _practiceViewModel.WithTimer ?
                    new DialogWindow(title, message, _timer, _practiceViewModel, false, "Activate now", "Cancel") : new DialogWindow(title, message, false);

                dialog.Owner = this;
                var result = dialog.ShowDialog() ?? false;

                if (!result) return;
            }

            if (_questionType == QuestionType.Theory) questionBank.CurrentQuestionData.IsTheoryAnswerVisible = false;

            await Task.Run(() => questionBank.loadQuestionAsync(questionNumber));

            if (_questionType == QuestionType.Theory) setActionText(questionBank.CurrentQuestionData.IsTheoryAnswerVisible);
        }

        private void NextButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var questionBank = button.GetParentOfType<Grid>().DataContext as QuestionBank;
            if (questionBank == null) return;

            navigateToQuestionAsync(questionBank, questionBank.CurrentQuestionNumber + 1);
        }

        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var questionBank = button.GetParentOfType<Grid>().DataContext as QuestionBank;
            if (questionBank == null) return;

            navigateToQuestionAsync(questionBank, questionBank.CurrentQuestionNumber - 1);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var title = "Logout";
            var message = "The ongoing test will be cancelled, are you sure?";

            DialogWindow dialog;
            dialog = _practiceViewModel.WithTimer ? 
                new DialogWindow(title, message, _timer, _practiceViewModel) : new DialogWindow(title, message);

            dialog.Owner = this;
            var result = dialog.ShowDialog() ?? false;

            if (!result) return;
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton) sender;
            var questionBank = radioButton.GetParentOfType<StackPanel>().DataContext as QuestionBank;
            if (questionBank == null) return;
            questionBank.currentQuestion.IsAnswered = true;
            questionBank.currentQuestion.SelectedAnswer = (string) radioButton.Tag;

            //var radioButton = (RadioButton)sender;
            //var question = radioButton.GetParentOfType<Grid>().DataContext as QuestionViewModel;
            //if (question == null) return;
            //question.IsAnswered = true;
            //question.SelectedAnswer = (string)radioButton.Tag;
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

        private async Task submitTestAsync()
        {
            if (_practiceViewModel.WithTimer)
            {
                _timer.Stop();
            }

            LoadingOverlay.Visibility = Visibility.Visible;

            await Task.Run(() => {
                _practiceViewModel.setResult(_practiceService, PracticeType.ByYear, _practiceMode); 
            });

            LoadingOverlay.Visibility = Visibility.Hidden;

        }

        private void displayPopup(string message)
        {
            AlertText.Text = message;
            AlertPopup.Visibility = Visibility.Visible;
            var dispatchTimer = new DispatcherTimer();
            dispatchTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            dispatchTimer.Tick += PopupTimer_Tick;
            dispatchTimer.Start();
        }

        private int popupCounter;
        private void PopupTimer_Tick(object sender, EventArgs e)
        {
            if (popupCounter < 2)
            {
                popupCounter += 1;
            }
            else
            {
                popupCounter = 0;
                AlertPopup.Visibility = Visibility.Hidden;
                ((DispatcherTimer)sender).Stop();
            }
        }

        private bool tryGetQuestionBank(out QuestionBank questionBank)
        {
            questionBank = null;
            if (!_practiceViewModel.QuestionBanks.IsSuccessfullyCompleted) return false;
            var index = TestTabControl.SelectedIndex;
            questionBank = _practiceViewModel.QuestionBanks.Result[index];
            return true;
        }

        private async void BookmarkButton_OnClick(object sender, RoutedEventArgs e)
        {
           
            QuestionBank questionBank;
            var isValid = tryGetQuestionBank(out questionBank);
            if (!isValid) return;
            var isSuccessful = await _practiceService.saveQuestionAsync(questionBank.currentQuestion,
                questionBank.subjectName);

            if (!isSuccessful) return;
            displayPopup("Question Saved!");
        }


        private void Overlay_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _modals.CloseModals();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.CloseModals();
        }

        private async void ActionButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (_questionType == QuestionType.Theory)
            {
                QuestionBank questionBank;
                var isValid = tryGetQuestionBank(out questionBank);
                if (!isValid) return;

                questionBank.CurrentQuestionData.IsTheoryAnswerVisible =
                    !questionBank.CurrentQuestionData.IsTheoryAnswerVisible;
                setActionText(questionBank.CurrentQuestionData.IsTheoryAnswerVisible);
                return;
            }


            if (_practiceViewModel.IsCorrection)
            {
                _modals.SetModal(ResultModal.Name);
            }
            else
            {
                var title = "Submit test";
                var message = "The ongoing test will be submitted are you sure?";

                DialogWindow dialog;
                dialog = _practiceViewModel.WithTimer ?
                    new DialogWindow(title, message, _timer, _practiceViewModel) : new DialogWindow(title, message);

                dialog.Owner = this;
                var result = dialog.ShowDialog() ?? false;

                if (!result) return;

                await submitTestAsync();
            }
            
        }

        private void ItemGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var itemGrid = ((Grid) sender).DataContext as SubjectTopicGradeViewModel;
            if (itemGrid == null) return;
            itemGrid.IsOpen = !itemGrid.IsOpen;
        }

        private async void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            
            QuestionBank questionBank;
            var isValid = tryGetQuestionBank(out questionBank);
            if (!isValid || questionBank == null) return;

            _modals.CloseModals();

            await _practiceService.reportQuestionAsync(questionBank, ((ComboBoxItem)ErrorTypeComboBox.SelectedItem).Content.ToString(), CommentTextBox.Text);

            ErrorTypeComboBox.SelectedIndex = 0;
            CommentTextBox.Text = "";
        }


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

        private void ReportErrorButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.SetModal(ReportErrorModal.Name);
        }

        private void CalculatorButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (calculator.IsVisible)
            {
                calculator.Hide();
                CalculatorButton.Tag = string.Empty;
            }
            else
            {
                calculator.Show();
                CalculatorButton.Tag = "Active";
            }
        }
    }
}
