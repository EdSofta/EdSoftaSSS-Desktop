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
using EdSofta.Enums;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for PracticeOnboardPage.xaml
    /// </summary>
    public partial class PracticeOnboardPage : Page
    {
        public PracticeOnboardPage()
        {
            InitializeComponent();
        }

        private readonly Frame _parentFrame;
        private readonly PracticeOnboardViewModel _practiceOnboardViewModel;
        public PracticeOnboardPage(Frame parentFrame, LearningRecommendation lrec)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _practiceOnboardViewModel = new PracticeOnboardViewModel(lrec, new PracticeByTopicService());
            DataContext = _practiceOnboardViewModel;
        }

        public PracticeOnboardPage(Frame parentFrame, string subject, string topic)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _practiceOnboardViewModel = new PracticeOnboardViewModel(subject, topic, new PracticeByTopicService());
            DataContext = _practiceOnboardViewModel;
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            var practiceMode = new PracticeMode {withTimer = false};
            var questionBanks = await _practiceOnboardViewModel.generateQuestionBanks();
            var testWindow = new TestingWindow(questionBanks, PracticeType.ByTheme, practiceMode);
            testWindow.Owner = System.Windows.Application.Current.MainWindow;
            testWindow.ShowDialog();
            if(_parentFrame.CanGoBack) _parentFrame.GoBack();
        }
    }
}
