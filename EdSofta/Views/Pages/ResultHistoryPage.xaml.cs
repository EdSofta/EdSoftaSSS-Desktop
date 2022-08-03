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
using EdSofta.Services;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for ResultHistoryPage.xaml
    /// </summary>
    public partial class ResultHistoryPage : Page
    {
        public ResultHistoryPage()
        {
            InitializeComponent();
        }

        private bool _loaded;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private readonly Frame _parentFrame;
        private readonly ResultHistoryViewModel _resultHistoryViewModel;
        private readonly IResultService _resultService;

        public ResultHistoryPage(Frame parentFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _resultService = new ResultService();
            _resultHistoryViewModel = new ResultHistoryViewModel(new ResultService());
            DataContext = _resultHistoryViewModel;
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private void PageLoaded(object sender, EventArgs e)
        {


        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
          
        }

        private void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            if(_parentFrame.CanGoBack) _parentFrame.GoBack();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            OptionsContextMenu.IsOpen = true;
        }

        private void DeleteQuestion_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private async void DeleteAllQuestions_OnClick(object sender, RoutedEventArgs e)
        {
            OptionsContextMenu.IsOpen = false;

            var dialog = new DialogWindow("Delete result history",
                "This will clear all test results till date, are you sure?");
            dialog.Owner = Application.Current.MainWindow;
            var result = dialog.ShowDialog() ?? false;

            if (!result) return;

            await _resultHistoryViewModel.deleteResults();
        }





        private async void ResultsList_OnSelectionChangedsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ResultsList.SelectedItem as ResultViewModel;
            if (item == null) return;
            await _resultHistoryViewModel.SetResult(item);
            ContentScrollViewer.ScrollToTop();
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

        private void ItemGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var itemGrid = ((Grid)sender).DataContext as SubjectTopicGradeViewModel;
            if (itemGrid == null) return;
            itemGrid.IsOpen = !itemGrid.IsOpen;
        }
    }
}
