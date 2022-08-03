using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    internal class HomeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private string actionText { get; set; }

        public string ActionText
        {
            get { return actionText; }
            set
            {
                actionText = value;
                OnPropertyChanged("ActionText");
            }
        }

        private ObservableCollection<ProfessionViewModel> professions { get; set; }

        public ObservableCollection<ProfessionViewModel> Professions
        {
            get { return professions; }
            set
            {
                professions = value;
                OnPropertyChanged("Professions");
            }
        }

        private Game currentGame { get; set; }

        public Game CurrentGame
        {
            get { return currentGame; }
            set
            {
                currentGame = value;
                OnPropertyChanged("CurrentGame");
            }
        }

        private bool gameExists { get; set; }

        public bool GameExists
        {
            get { return gameExists; }
            set
            {
                gameExists = value;
                OnPropertyChanged("GameExists");
            }
        }
        private IGameService _gameService;
        private Game _game;

        public HomeViewModel(IGameService gameService)
        {
            _gameService = gameService;
            getCurrentGame();
            getProfessions();
            Professions[0].IsSelected = true;
        }

        private void getProfessions()
        {
            var dictionary = _gameService.getProfessions();
            Professions = dictionary.Select(x => new ProfessionViewModel {ProfessionName = x.Key}).ToObservableCollection();
        }

        private void getCurrentGame()
        {
            Game game;
            GameExists = _gameService.tryGetCurrentGame(out game);
            if (GameExists) CurrentGame = game;
            ActionText = GameExists ? "CONTINUE GAME" : "NEW GAME";
        }

        public async Task<bool> createNewGame(Game game)
        {
            return await _gameService.createNewGame(game);
        }

        public async Task<bool> resetGame()
        {
            var isSuccessful = await _gameService.resetGame();
            if(isSuccessful) getCurrentGame();
            return isSuccessful;
        }
    }
}
