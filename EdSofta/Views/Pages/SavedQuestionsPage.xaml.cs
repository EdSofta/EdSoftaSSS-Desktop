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
using System.Windows.Media.Media3D.Converters;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for SavedQuestionsPage.xaml
    /// </summary>
    public partial class SavedQuestionsPage : Page
    {
        public SavedQuestionsPage()
        {
            InitializeComponent();
            var userId = new Guid("38B9F889-AD9E-4942-BDEA-50CCA50F6B37");
            _savedQuestionService = new SavedQuestionService();
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private bool _loaded;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private readonly Frame _parentFrame;
        private readonly ISavedQuestionService _savedQuestionService;
        private readonly SavedQuestionViewModel _savedQuestionsViewModel;

        internal SavedQuestionsPage(Frame parentFrame, ISavedQuestionService savedQuestionService)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _savedQuestionService = savedQuestionService;
            _savedQuestionsViewModel = new SavedQuestionViewModel(savedQuestionService);
            DataContext = _savedQuestionsViewModel;

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

        private void Navigating(object sender, NavigatingCancelEventArgs e)
        {

        }

        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_parentFrame.CanGoBack) return;
            _parentFrame.NavigationService.GoBack();
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

        private async Task deleteItem(int id)
        {
            var isSuccessful = await _savedQuestionService.removeSavedQuestionAsync(id);
            if (!isSuccessful) return;

            var item = _savedQuestionsViewModel.SavedQuestions.Result.SingleOrDefault(x => x.QuestionId == id);
            if (item != null) _savedQuestionsViewModel.SavedQuestions.Result.Remove(item);

        }

        private async Task deleteAllItems()
        {
            var isSuccessful = await _savedQuestionService.removeAllSavedQuestion();
            if (!isSuccessful) return;

            _savedQuestionsViewModel.SavedQuestions.Result.Clear();
        }

        private async void DeleteQuestion_OnClick(object sender, RoutedEventArgs e)
        {
            OptionsContextMenu.IsOpen = false;
            if (_savedQuestionsViewModel.SelectedItem == null) return;
            await deleteItem(_savedQuestionsViewModel.SelectedItem.QuestionId);

            var itemDeleted = _savedQuestionsViewModel.RefreshFilters();

            if (itemDeleted)
            {
                FilterComboBox.SelectedIndex = 0;
            }

            _savedQuestionsViewModel.ResetData();
        }

        private async void DeleteAllQuestions_OnClick(object sender, RoutedEventArgs e)
        {
            OptionsContextMenu.IsOpen = false;

            var dialog = new DialogWindow("Delete all saved questions",
                "This will clear all questions saved, are you sure?");
            dialog.Owner = Application.Current.MainWindow;
            var result = dialog.ShowDialog() ?? false;

            if (!result) return;

            await deleteAllItems();

            var itemDeleted = _savedQuestionsViewModel.RefreshFilters();
            if (itemDeleted)
            {
                FilterComboBox.SelectedIndex = 0;
            }

            _savedQuestionsViewModel.ResetData();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            OptionsContextMenu.IsOpen = true;
        }

        private void SavedQuestionsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = SavedQuestionsList.SelectedItem as SavedQuestion;
            if (item == null) return;

            _savedQuestionsViewModel.SelectedItem = item;
            _savedQuestionsViewModel.SetQuestionData(item);
            ContentScrollViewer.ScrollToTop();
        }

        private bool FilterResults(object item)
        {
            var question = item as SavedQuestion;
            var filterName = FilterComboBox.SelectedItem;
            if (filterName == null) return false;

            var parameter = (string) filterName;

            if (parameter == "All subjects")
            {
                return true;
            }

            return question != null && question.Subject.Equals(parameter, StringComparison.OrdinalIgnoreCase);
        }


        private void FilterComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterComboBox.ItemsSource == null || _savedQuestionsViewModel.SavedQuestions.IsNotCompleted) return;

            var view = CollectionViewSource.GetDefaultView(_savedQuestionsViewModel.SavedQuestions.Result);
            view.Filter = FilterResults;

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
    }
}
