using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using EdSofta.Data;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private LandingPageViewModel _landingPageViewModel;

        internal HomePage(Frame parentFrame, Frame outerFrame, LandingPageViewModel landingPageViewModel)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _outerFrame = outerFrame;
            _landingPageViewModel = landingPageViewModel;
            DataContext = landingPageViewModel;
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }


        private readonly Frame _parentFrame;
        private readonly Frame _outerFrame;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;

        private async void PageLoaded(object sender, RoutedEventArgs e)
        {
            _navService = _parentFrame.NavigationService;
            _navService.Navigating += Navigating;
            ContentScrollViewer.ScrollToTop();
            await _landingPageViewModel.fetchLatestResult();
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_navService == null) return;
            _navService.Navigating -= Navigating;
            _navService = null;
        }

        private void Navigating(object sender, NavigatingCancelEventArgs e)
        {
            //Console.WriteLine($"{Title} is navigating {e.NavigationMode} at {DateTime.Now}");
            _navEventArgs = e;

            if(e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;
              
                var storyboard1 = Application.Current.FindResource("Animate.SlideOutDownDelayedMid");
                var storyboard2 = Application.Current.FindResource("Animate.SlideOutDownDelayedMid");
                if (storyboard1 == null || storyboard2 == null)
                {
                    if (_navService != null) _navService.Navigating -= Navigating;
                    if(_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();
                    return;
                }
                var myStoryboard1 = (Storyboard)storyboard1;
                var myStoryboard2 = (Storyboard)storyboard2;
                myStoryboard2.Completed += new EventHandler(AnimationCompleted);
                myStoryboard1.Begin(ContentGrid);
                myStoryboard2.Begin(SideGrid);
            }

            
        }

     
        private void AnimationCompleted(object sender, EventArgs e)
        {
            if (!_parentFrame.NavigationService.CanGoBack) return;
            if(_navService != null) _navService.Navigating -= Navigating;
            _parentFrame.NavigationService.GoBack();
        }

        private void ExitAnimation(Page page, Frame frame)
        {
            var storyboard1 = Application.Current.FindResource("Animate.SlideInDownDelayedMid");
            var storyboard2 = Application.Current.FindResource("Animate.SlideInDownDelayedMid");
            if (storyboard1 == null || storyboard2 == null) return;
            var myStoryboard1 = (Storyboard)storyboard1;
            var myStoryboard2 = (Storyboard)storyboard2;
            EventHandler handler = null;
            handler = (sender, args) =>
            {
                frame?.Navigate(page);

                myStoryboard2.Completed -= handler;
            };

            myStoryboard2.Completed += handler;
            myStoryboard1.Begin(ContentGrid);
            myStoryboard2.Begin(SideGrid);
        }

        private void ExitAnimationOuter(Page page)
        {
            LandingPage mainWindow = null;
            var currentParent = VisualTreeHelper.GetParent(this);
            while (currentParent != null && mainWindow == null)
            {
                mainWindow = currentParent as LandingPage;
                currentParent = VisualTreeHelper.GetParent(currentParent);
            }
        }

        private void PracticeExamButton_OnClick(object sender, RoutedEventArgs e)
        {
            var page = new PracticeExamSelectionPage(_parentFrame);
            ExitAnimation(page, _parentFrame);
        }

        private void StudyMaterialsButton_OnClick(object sender, RoutedEventArgs e)
        {
            //var page = new StudyMaterialsPage();
            //ExitAnimation(page, _parentFrame);
            var page = new StudyMaterialsViewPage(_outerFrame, new StudyMaterialService(), _parentFrame, _landingPageViewModel);
            ExitAnimation(page, _outerFrame);
        }

        private void PlayGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var game = new GameWindow();
            game.ShowDialog();
        }

        private void SavedQuestionsButton_OnClick(object sender, RoutedEventArgs e)
        {
           var page = new SavedQuestionsPage(_outerFrame, new SavedQuestionService());
           ExitAnimation(page, _outerFrame);

        }

        private void SearchForTutorButton_OnClick(object sender, RoutedEventArgs e)
        {
            var longUrl = "edsofta.com/tutor";
            var uriBuilder = new UriBuilder(longUrl);
            try
            {
                Process.Start(new ProcessStartInfo(uriBuilder.Uri.AbsoluteUri));
            }
            catch
            {
                var dialog = new DialogWindow("An error occured",
                    $"Unable to open browser to link: {uriBuilder.Uri.AbsoluteUri}", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
            }
        }

        private void LearningRecommendation_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ViewModels.Utility.App.IsActivated)
            {

                var title = "App not activated";
                var message = "To have access to learning recommendations, you need to activate the app";

                DialogWindow dialog;
                dialog = new DialogWindow(title, message, "Activate now", "Later");

                dialog.Owner = Application.Current.MainWindow;
                var result = dialog.ShowDialog() ?? false;

                if (!result) return;

                var activatePage = new ActivationPage(_parentFrame, _landingPageViewModel);
                ExitAnimation(activatePage, _parentFrame);
                return;
            }


            var page = new LRPage(_parentFrame, _outerFrame);
            ExitAnimation(page, _parentFrame);
        }

        private void ForumButton_OnClick(object sender, RoutedEventArgs e)
        {
            var longUrl = "edsofta.com/forum";
            var uriBuilder = new UriBuilder(longUrl);
            try
            {
                Process.Start(new ProcessStartInfo(uriBuilder.Uri.AbsoluteUri));
            }
            catch
            {
                var dialog = new DialogWindow("An error occured",
                    $"Unable to open browser to link: {uriBuilder.Uri.AbsoluteUri}", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
            }
        }

        private void ResultHistoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            var page = new ResultHistoryPage(_outerFrame);
            ExitAnimation(page, _outerFrame);
        }
    }
}
