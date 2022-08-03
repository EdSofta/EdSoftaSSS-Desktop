using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using System.Windows.Threading;
using EdSofta.Constants;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.GameViewModelClasses;
using EdSofta.ViewModels.Utility;

namespace EdSofta.Views.GamePages
{
    /// <summary>
    /// Interaction logic for GamePlayPage.xaml
    /// </summary>
    public partial class GamePlayPage : Page
    {
        public GamePlayPage()
        {
            InitializeComponent();
        }

        internal readonly List<IModal> _modals = new List<IModal>();
        private IGameService _gameService; 
        private GameQuestionBankViewModel _questionBank = new GameQuestionBankViewModel();
        private Dictionary<string, string> _questionDictionary = new Dictionary<string, string>();
        private GameViewModel _gameViewModel;
        private int _passRequirement = 10;
        private bool _levelCleared;
        private bool _gameStarted;
        private Game _game;
        private Frame _parentFrame;
        private Window _parentWindow;

        private readonly DispatcherTimer _timer = new DispatcherTimer();
        public int _timeCount = 21;
        public string _timeString = string.Empty;
        public int _maxLevel = 6;
        private int _skipCost = 90;
        private int _fiftyFiftyCost = 30;
        private int _addTimeCost = 30;
        private int _completionReward = 100;

        public GamePlayPage(Frame parentFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _gameService = new GameService();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            initializeQuestionBank();
            DataContext = _questionBank;
        }

        public GamePlayPage(Frame parentFrame, Window parentWindow, Game game)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _parentWindow = parentWindow;
            _gameService = new GameService();
            _timer.Tick += Timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _game = game;
            _gameViewModel = new GameViewModel(new GameService(), game);
            DataContext = _gameViewModel;
            initializePrompt(_game.Level);
            RoutedEventHandler handler = null;
            handler = (sender, args) =>
            {
                promptLevel(_game.Level, _game.Level, false);
                SetModals();
                Loaded -= handler;
            };

            Loaded += handler;
        }



        private void SetModals()
        {
            _modals.AddRange(
                new List<IModal>
                {
                    new Modal(ProgressModal),
                    new Modal(HelpModal),
                    new Modal(PauseModal)
                });
        }

        #region Game Logic

        private async void initializeQuestionBank()
        {
            
            //_questionDictionary = await getLevelQuestions(3);
            //_questionBank.CurrentQuestionNumber = 1;
            _questionBank.Size = _questionDictionary.Count;
            _questionBank.loadQuestion(
                GameResourceUtility.getQuestionNumber(_questionDictionary.ElementAt(0).Key), 
                _questionDictionary.ElementAt(0).Value);

        }

        public int number = 0;
        private void AnswerButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            A.IsEnabled = false;
            B.IsEnabled = false;
            C.IsEnabled = false;
            D.IsEnabled = false;

            var buttonOption = ((Button) sender).Name;

            var isCorrectAnswer =
                _gameViewModel.QuestionBank.Result.CurrentQuestionData.isCorrectlyAnswered(buttonOption);

            flashButton((Button)sender, isCorrectAnswer);

            if (!isCorrectAnswer)
            {
                var button = getButton(_gameViewModel.QuestionBank.Result.CurrentQuestionData.getCorrectAnswer());

                flashButton(button, true);
            }

            
        }

        private void FifyFiftyButton_OnClick(object sender, RoutedEventArgs e)
        {
            fiftyFifty(_gameViewModel.QuestionBank.Result.CurrentQuestionData.getCorrectAnswer());
        }

        private void AddTimeButton_OnClick(object sender, RoutedEventArgs e)
        {
            addTime();
        }

        private void SkipQuestionButton_OnClick(object sender, RoutedEventArgs e)
        {
            skipChallenge();
        }

        private void resetElements()
        {
            AddTimeButton.IsEnabled = true;
            FiftyFiftyButton.IsEnabled = true;
            A.IsEnabled = true;
            B.IsEnabled = true;
            C.IsEnabled = true;
            D.IsEnabled = true;
            A.Visibility = Visibility.Visible;
            B.Visibility = Visibility.Visible;
            C.Visibility = Visibility.Visible;
            D.Visibility = Visibility.Visible;
        }


        private async Task loadNextQuestion()
        {

            var levelEnded = _gameViewModel.loadNextQuestion();

            if (!levelEnded)
            {
                resetElements();

                _timeCount = 20;
                _timer.Start();
            }
            else
            {
                //finish up and save data
                _levelCleared = _gameViewModel.QuestionBank.Result.questionLimitPassed(_passRequirement);
                await _gameViewModel.saveGame(_levelCleared);
                setResults();
            }
            

        }

        private void fiftyFifty(string correctAnswer)
        {
            if (_gameViewModel.removeCoins(_fiftyFiftyCost))
            {
                var optionsList = new List<string> { "A", "B", "C", "D" };
                optionsList.Remove(correctAnswer);
                var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(optionsList.Count - 1);
                optionsList.RemoveAt(randomInt);

                if (optionsList.Contains("A"))
                {
                    A.Visibility = Visibility.Hidden;
                }

                if (optionsList.Contains("B"))
                {
                    B.Visibility = Visibility.Hidden;
                }

                if (optionsList.Contains("C"))
                {
                    C.Visibility = Visibility.Hidden;
                }

                if (optionsList.Contains("D"))
                {
                    D.Visibility = Visibility.Hidden;
                }

                FiftyFiftyButton.IsEnabled = false;
                displayPopup("Fifty-Fifty used!");
            }
            else
            {
                displayPopup("Out of coins!");
            }
        }

        private void addTime()
        {
            if (_gameViewModel.removeCoins(_addTimeCost))
            {
                _timeCount += 11;
                displayPopup("+11 seconds added");
                AddTimeButton.IsEnabled = false;
            }
            else
            {
                displayPopup("Out of coins!");
            }
            
        }

        private void skipChallenge()
        {
            if(_gameViewModel.removeCoins(_skipCost))
            {
                var answer = _gameViewModel.QuestionBank.Result.CurrentQuestionData.getCorrectAnswer();
                _gameViewModel.QuestionBank.Result.CurrentQuestionData.isCorrectlyAnswered(answer);
                animateLoadNextQuestion();
            }
            else
            {
                displayPopup("Out of coins!");
            }
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

        private void animateLoadNextQuestion()
        {
            _timer.Stop();
            var storyboard1 = Application.Current.FindResource("Game.Animate.SlideOutFast");
            var storyboard2 = Application.Current.FindResource("Game.Animate.SlideOutFastDelayed");
            if (storyboard1 == null || storyboard2 == null)
            {
                loadNextQuestion();
                return;
            }
            var myStoryboard1 = (Storyboard)storyboard1;
            var myStoryboard2 = (Storyboard)storyboard2;
            myStoryboard1.Completed += new EventHandler(AnimationCompleted);
            myStoryboard1.Begin(TopOptionsGrid);
            myStoryboard2.Begin(BottomOptionsGrid);
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            TopOptionsGrid.Visibility = Visibility.Hidden;
            BottomOptionsGrid.Visibility = Visibility.Hidden;
            var storyboard1 = Application.Current.FindResource("Game.Animate.SlideToPosition");
            var storyboard2 = Application.Current.FindResource("Game.Animate.SlideToPosition");
            if (storyboard1 == null || storyboard2 == null)
            {
                loadNextQuestion();
                return;
            }
            var myStoryboard1 = (Storyboard)storyboard1;
            var myStoryboard2 = (Storyboard)storyboard2;
            myStoryboard2.Completed += new EventHandler(ResetAnimationCompleted);
            myStoryboard1.Begin(TopOptionsGrid);
            myStoryboard2.Begin(BottomOptionsGrid);
        }

        private async void ResetAnimationCompleted(object sender, EventArgs e)
        {
            await loadNextQuestion();
            TopOptionsGrid.Visibility = Visibility.Visible;
            BottomOptionsGrid.Visibility = Visibility.Visible;
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timeCount -= 1;
            TimerText.Text = _timeCount.ToString("00:00");
            if (_timeCount == 0)
            {
                animateLoadNextQuestion();
                //_timeCount = 11;
            }
        }

        private Button getButton(string option)
        {
            var button = new Button();
            switch (option)
            {
                case "A":
                    button = A;
                    break;
                case "B":
                    button = B;
                    break;
                case "C":
                    button = C;
                    break;
                case "D":
                    button = D;
                    break;
            }

            return button;
        }

        private Button correctButton;
        private int correctCounter;
        private Button incorrectButton;
        private int incorrectCounter;
        private void flashButton(Button button, bool isAnsweredCorrectly)
        {
            var dispatchTimer = new DispatcherTimer();
            dispatchTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            if (isAnsweredCorrectly)
            {
                correctButton = button;
                correctButton.Tag = "Correct";
                dispatchTimer.Tick += CorrectFlashTimer_Tick;
            }
            else
            {
                incorrectButton = button;
                incorrectButton.Tag = "Incorrect";
                dispatchTimer.Tick += IncorrectFlashTimer_Tick;
            }

            dispatchTimer.Start();

        }

        private void CorrectFlashTimer_Tick(object sender, EventArgs e)
        {
            if (correctCounter < 1)
            {
                correctCounter += 1;
            }
            else
            {
                correctButton.Tag = string.Empty;
                correctCounter = 0;
                ((DispatcherTimer)sender).Stop();
                animateLoadNextQuestion();
            }
        }

        private void IncorrectFlashTimer_Tick(object sender, EventArgs e)
        {
            if (incorrectCounter < 1)
            {
                incorrectCounter += 1;
            }
            else
            {
                incorrectButton.Tag = string.Empty;
                incorrectCounter = 0;
                ((DispatcherTimer)sender).Stop();
            }
        }



        //private async Task<Dictionary<string, string>> getLevelQuestions(int level)
        //{
            
        //    var list = new List<string> {"Chemistry", "Commerce", "English", "Government"};
        //    switch (level)
        //    {
        //        case 1:
        //            return await getQuestions(list, true, false, false, 1, 0, 0);
        //        case 2:
        //            return await getQuestions(list, true, true, false, 2, 1, 0);
        //        case 3:
        //            return await getQuestions(list, true, true, true, 2, 2, 1);
        //        case 4:
        //            return await getQuestions(list, false, true, true, 0, 2, 1);
        //        case 5:
        //            return await getQuestions(list, false, true, true, 0, 1, 2);
        //        case 6:
        //            return await getQuestions(list, false, false, true, 0, 0, 1);

        //    }

        //    return new Dictionary<string, string>();
        //}


        //public async Task<Dictionary<string, string>> getQuestions(List<string> subjects, bool easy, bool medium, bool hard, int easyRatio, int mediumRatio, int hardRatio)
        //{
        //    var easyDictionary = new Dictionary<string, string>();
        //    var mediumDictionary = new Dictionary<string, string>();
        //    var hardDictionary = new Dictionary<string, string>();

        //    if (easy)
        //    {
        //        var list = await _gameService.getQuestionsByDifficulty(subjects, Difficulty.Easy);
        //        foreach (var item in list)
        //        {
        //            easyDictionary.Add(item.Key, item.Value);
        //        }
        //    }

        //    if (medium)
        //    {
        //        var list = await _gameService.getQuestionsByDifficulty(subjects, Difficulty.Medium);
        //        foreach (var item in list)
        //        {
        //            mediumDictionary.Add(item.Key, item.Value);
        //        }
        //    }

        //    if (hard)
        //    {
        //        var list = await _gameService.getQuestionsByDifficulty(subjects, Difficulty.Hard);
        //        foreach (var item in list)
        //        {
        //            hardDictionary.Add(item.Key, item.Value);
        //        }
        //    }

            
        //    var questionsDictionary = new Dictionary<string, string>();
        //    var totalRatio = easyRatio + mediumRatio + hardRatio;
        //    const int questions = 15;

        //    var easyList = Enumerable.Range(0, easyDictionary.Count).ToList();
        //    var easyLimit = (easyRatio * questions) / totalRatio;
        //    for (var i = 0; i < easyLimit; i++)
        //    {
        //        var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(easyList.Count - 1);
        //        var num = easyList[randomInt];
        //        var element = easyDictionary.ElementAt(num);
        //        questionsDictionary.Add(element.Key, element.Value);
        //        easyList.RemoveAt(randomInt);
        //    }

        //    var mediumList = Enumerable.Range(0, mediumDictionary.Count).ToList();
        //    var mediumLimit = (mediumRatio * questions) / totalRatio;
        //    for (var i = 0; i < mediumLimit; i++)
        //    {
        //        var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(mediumList.Count - 1);
        //        var num = mediumList[randomInt];
        //        var element = mediumDictionary.ElementAt(num);
        //        questionsDictionary.Add(element.Key, element.Value);
        //        mediumList.RemoveAt(randomInt);
        //    }

        //    var hardList = Enumerable.Range(0, hardDictionary.Count).ToList();
        //    var hardLimit = (hardRatio * questions) / totalRatio;
        //    for (var i = 0; i < hardLimit; i++)
        //    {
        //        var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(hardList.Count - 1);
        //        var num = hardList[randomInt];
        //        var element = hardDictionary.ElementAt(num);
        //        questionsDictionary.Add(element.Key, element.Value);
        //        hardList.RemoveAt(randomInt);
        //    }

        //    return questionsDictionary;
        //}


        private void setResults()
        {
            
            if (!_levelCleared)
            {
                _timeCount = 20;
                _gameViewModel.ProgressHeader = "Level Completed";
                _gameViewModel.ProgressComment = "You can do better";
                _gameViewModel.ProgressBody =
                    $"You scored a {_gameViewModel.QuestionBank.Result.QuestionsPassed} out of {_gameViewModel.QuestionBank.Result.Size}. A minimum of 10 challenges are required to advance to the next level";
                _gameViewModel.ProgressButtonText = "TRY AGAIN";
                promptLevel(_game.Level, _game.Level, _levelCleared);
                _gameViewModel.ProgressButtonActive = true;
            }
            else
            {
                _gameViewModel.ProgressButtonActive = false;
                _gameViewModel.ProgressHeader = $"Level {_game.Level - 1} Completed";
                _gameViewModel.ProgressComment = "Great work";
                _gameViewModel.ProgressBody =
                    $"You are {_game.Level}/{_maxLevel} ready on your way to being a {_game.Profession}";
                promptLevel(_game.Level-1, _game.Level, _levelCleared);
            }
        }


        private void initializePrompt(int level)
        {
            _gameViewModel.ProgressHeader = $"Level {level}";
            _gameViewModel.ProgressComment = $"{GameConstants.getLevelTitle(level)} level";
            if (level == _maxLevel)
            {
                _gameViewModel.ProgressBody = "Complete this level to finish the game";
            }
            else
            {
                _gameViewModel.ProgressBody =
                    $"Complete a series of challenges to advance to the {GameConstants.getLevelTitle(level + 1)} level";
            }

            _gameViewModel.ProgressButtonText = "CONTINUE";
            _gameViewModel.ProgressButtonActive = true;
        }
        #endregion Game Logic

        private void promptLevel(int previousLevel, int currentLevel, bool levelCleared)
        {
            if (previousLevel < _maxLevel)
            {
                _modals.SetModal(ProgressModal.Name);
                var start = (previousLevel * 100) / _maxLevel;
                var end = (currentLevel * 100) / _maxLevel;

                if (levelCleared)
                {
                    movePin(start, end, currentLevel);
                    return;
                }

                movePin(start, end);

            }
            else
            {
                //finish game;
                //save necessary
                _game.Current = false;
                _game.Coins = _completionReward;
                _gameViewModel.saveGame(false);
                var page = new GameFinishedPage(_parentFrame, _parentWindow, _game.Profession);
                _parentFrame.Navigate(page);
            }

        }

        private void Overlay_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.CloseModals();
            _timer.Start();
        }

        private void movePin(int start, int end)
        {
            var barLength = ProgressCanvas.ActualWidth;
            var startPoint = ((start - 10) * barLength) / 100;
            var endPoint = ((end - 10) * barLength) / 100;
            animatePin(startPoint, endPoint);
        }

        private void movePin(int start, int end, int level)
        {
            var barLength = ProgressCanvas.ActualWidth;
            var startPoint = ((start - 10) * barLength) / 100;
            var endPoint = ((end - 10) * barLength) / 100;
            animatePinPrompt(startPoint, endPoint, level);
        }

        private void animatePin(double startPoint, double endPoint)
        {
            var a = new DoubleAnimation();
            a.From = startPoint;
            a.To = endPoint;
            a.AccelerationRatio = 0.33;
            a.DecelerationRatio = 0.33;
            a.Duration = new Duration(TimeSpan.Parse("0:0:5"));
            var trans = new TranslateTransform();
            MapPin.RenderTransform = trans;
            trans.BeginAnimation(TranslateTransform.XProperty, a);
        }

        private void animatePinPrompt(double startPoint, double endPoint, int level)
        {
            var a = new DoubleAnimation();
            a.From = startPoint;
            a.To = endPoint;
            a.AccelerationRatio = 0.33;
            a.DecelerationRatio = 0.33;
            a.Duration = new Duration(TimeSpan.Parse("0:0:5"));
            var trans = new TranslateTransform();
            MapPin.RenderTransform = trans;

            EventHandler handler = null;
            handler = (sender, args) =>
            {
                //_gameViewModel.increaseLevel();
                initializePrompt(level);
                a.Completed -= handler;
            };

            a.Completed += handler;
            trans.BeginAnimation(TranslateTransform.XProperty, a);
            

        }

        private int level = 10;
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            movePin(level, level + 10);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _modals.SetModal(PauseModal.Name);
        }


        private void ActionButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_gameStarted)
            {
                _levelCleared = false;
                _gameViewModel = new GameViewModel(new GameService(), _game);
                DataContext = _gameViewModel;
                resetElements();
                _modals.CloseModals();
            }
            else
            {
                _gameStarted = true;
                resetElements();
                _modals.CloseModals();
            }

            _timer.Start();

        }


        private void ContinueButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.CloseModals();
            _timer.Start();
        }

        private void TipsButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.SetModal(HelpModal.Name);
        }

        private void HomeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var page = new GameHomePage(_parentFrame, _parentWindow);
            _parentFrame.Navigate(page);
        }
    }
}
