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
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.GameViewModelClasses;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Views.GamePages
{
    /// <summary>
    /// Interaction logic for GameHomePage.xaml
    /// </summary>
    public partial class GameHomePage : Page
    {
        public GameHomePage()
        {
            InitializeComponent();
            //setProfessions();
        }

        internal readonly List<IModal> _modals = new List<IModal>();
        private Frame _parentFrame;
        private Window _parentWindow;
        //private ObservableCollection<ProfessionViewModel> _professions;
        //private ObservableCollection<SubjectsViewModel> _subjects;

        //private void setProfessions()
        //{
        //    var profList = new List<string>{"Doctor", "Engineer", "Teacher", "Lawyer"};
        //    _professions = new ObservableCollection<ProfessionViewModel>();

        //    foreach (var prof in profList)
        //    {
        //        _professions.Add(new ProfessionViewModel{ProfessionName = prof, IsSelected = false});
        //    }

        //    _professions[0].IsSelected = true;

        //    ProfessionListView.ItemsSource = _professions;
        //}

        //private void setSubjects()
        //{
        //    var list = new List<string> { "Doctor", "Engineer", "Teacher", "Lawyer", "Mathematics", "English", "Physics" };
        //    _subjects = new ObservableCollection<SubjectsViewModel>();

        //    foreach (var prof in list)
        //    {
        //        _subjects.Add(new SubjectsViewModel { SubjectName = prof, IsSelected = false });
        //    }


        //    SubjectsListView.ItemsSource = _subjects;
        //}

        private readonly HomeViewModel _homeViewModel;

        public GameHomePage(Frame parentFrame, Window parentWindow)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _parentWindow = parentWindow;
            _homeViewModel = new HomeViewModel(new GameService());
            DataContext = _homeViewModel;
            SetModals();
        }

        private void SetModals()
        {
            _modals.AddRange(
                new List<IModal>
                {
                    new Modal(AddProfessionModal),
                    new Modal(ChooseProfessionModal),
                    new Modal(HelpModal),
                    new Modal(SettingsModal)
                });
        }

        private void PlayGameButton_OnClick(object sender, RoutedEventArgs e)
        {
           
            if (_homeViewModel.GameExists)
            {
                var page = new GamePlayPage(_parentFrame, _parentWindow, _homeViewModel.CurrentGame);
                _parentFrame.Navigate(page);
            }
            else
            {
                _modals.SetModal(ChooseProfessionModal.Name);
            }


        }

        private void Overlay_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.CloseModals();
        }

        private void ProfessionItemGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = ((Grid)sender).DataContext as ProfessionViewModel;
            if (item == null) return;
            var selectedItem = _homeViewModel.Professions.ToList().Find(x => x.IsSelected);
            if (item == selectedItem) return;
            item.IsSelected = true;
            selectedItem.IsSelected = false;
        }

        private void SubjectItemGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var item = ((Grid)sender).DataContext as SubjectsViewModel;
            //if (item == null ) return;
            //if (item.IsSelected == false && _subjects.ToList().FindAll(x => x.IsSelected).Count == 4) return;
            //item.IsSelected = !item.IsSelected;

        }

        private void AddProfession_OnClick(object sender, RoutedEventArgs e)
        {
            //setSubjects();
            _modals.SetModal(AddProfessionModal.Name);
        }

        private void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.SetModal(ChooseProfessionModal.Name);
        }

        private void movePin()
        {

        }

        private async void ProfessionStartGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var profession = _homeViewModel.Professions.SingleOrDefault(x => x.IsSelected);
            if (profession == null) return;

            var newGame = new Game
            {
                Id = Guid.NewGuid(),
                Coins = 100,
                Current = true,
                DateCreated = DateTime.Now,
                DateLastPlayed = DateTime.Now,
                Level = 1,
                UserId = Guid.Empty,
                Profession = profession.ProfessionName
            };

            var isSuccessful = await _homeViewModel.createNewGame(newGame);
            if (!isSuccessful) return;

            var page = new GamePlayPage(_parentFrame, _parentWindow, newGame);
            _parentFrame.Navigate(page);
        }

        private void HelpButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.SetModal(HelpModal.Name);
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            _parentWindow.Close();
        }

        private async void NewGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            await _homeViewModel.resetGame();
        }

        private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            _modals.SetModal(SettingsModal.Name);
        }
    }
}
