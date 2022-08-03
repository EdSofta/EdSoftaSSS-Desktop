using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Interfaces;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.GameViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private NotifyTaskCompletion<GameQuestionBankViewModel> questionBank { get; set; }

        public NotifyTaskCompletion<GameQuestionBankViewModel> QuestionBank
        {
            get { return questionBank; }
            set
            {
                questionBank = value;
                OnPropertyChanged("QuestionBank");
            }
        }


        private Dictionary<string, string> questionDictionary { get; set; }

        public Dictionary<string, string> QuestionDictionary
        {
            get { return questionDictionary; }
            set
            {
                questionDictionary = value;
                OnPropertyChanged("QuestionDictionary");
            }
        }


        private int level { get; set; }

        public int Level
        {
            get { return level; }
            set
            {
                level = value;
                OnPropertyChanged("Level");
            }
        }

        private int coins { get; set; }

        public int Coins
        {
            get { return coins; }
            set
            {
                coins = value;
                OnPropertyChanged("Coins");
            }
        }

        private string profession { get; set; }

        public string Profession
        {
            get { return profession; }
            set
            {
                profession = value;
                OnPropertyChanged("Profession");
            }
        }


        private string progessHeader { get; set; }

        public string ProgressHeader
        {
            get { return progessHeader; }
            set
            {
                progessHeader = value;
                OnPropertyChanged("ProgressHeader");
            }
        }


        private string progessComment { get; set; }

        public string ProgressComment
        {
            get { return progessComment; }
            set
            {
                progessComment = value;
                OnPropertyChanged("ProgressComment");
            }
        }


        private string progessBody { get; set; }

        public string ProgressBody
        {
            get { return progessBody; }
            set
            {
                progessBody = value;
                OnPropertyChanged("ProgressBody");
            }
        }


        private string progessButtonText { get; set; }

        public string ProgressButtonText
        {
            get { return progessButtonText; }
            set
            {
                progessButtonText = value;
                OnPropertyChanged("ProgressButtonText");
            }
        }

        private bool progressButtonActive { get; set; }

        public bool ProgressButtonActive
        {
            get { return progressButtonActive; }
            set
            {
                progressButtonActive = value;
                OnPropertyChanged("ProgressButtonActive");
            }
        }

        private bool loading { get; set; }

        public bool Loading
        {
            get { return loading; }
            set
            {
                loading = value;
                OnPropertyChanged("Loading");
            }
        }




        public GameViewModel()
        {

        }

        private IGameService _gameService;
        private Game _game;
        public GameViewModel(IGameService gameService, Game game)
        {
            Loading = true;
            _gameService = gameService;
            _game = game;
            Level = game.Level;
            Coins = game.Coins;
            Profession = game.Profession;
            QuestionBank = new NotifyTaskCompletion<GameQuestionBankViewModel>(generateQuestionBank(), OnQuestionBankCompleted);
        }

        private void OnQuestionBankCompleted(object sender, TaskCompletedEventArgs e)
        {
            var bank = (NotifyTaskCompletion<GameQuestionBankViewModel>) sender;
            if (bank.Result == null) return;
            Loading = false;
        }

        private async Task<GameQuestionBankViewModel> generateQuestionBank()
        {
            var subjects = _gameService.getProfessionSubjects(_game.Profession);
            if(subjects.Count == 0) return new GameQuestionBankViewModel(); //todo: possible null exception when method return bubbles up
            var questionData =  await _gameService.getLevelQuestionsAsync(_game.Level, subjects);
            return new GameQuestionBankViewModel(questionData);
            
        }

        private int Number = 0;

        public bool loadNextQuestion()
        {
            if (QuestionBank.Result.CurrentQuestionNumber >= QuestionBank.Result.Size)
            {
                Number = 0;
                return true;
            }

            Number += 1;
            QuestionBank.Result.loadQuestionIndex(Number);
            return false;
        }

        public void increaseLevel()
        {
            _game.Level += 1;
            Level = _game.Level;
        }

        public async Task<bool> saveGame(bool levelCleared)
        {
            if (levelCleared)
            {
                _game.Coins += 30;
                Coins = _game.Coins;
                increaseLevel();
            }
            _game.DateLastPlayed = DateTime.Now;
            return await _gameService.updateGameAsync(_game);
        }

        public bool removeCoins(int coins)
        {
            if (coins > _game.Coins) return false;
            _game.Coins -= coins;
            Coins = _game.Coins;
            return true;
        }

    }
}
