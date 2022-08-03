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
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EdSofta.Constants;
using EdSofta.Models;
using EdSofta.Services;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for PracticeExamSelectionPage.xaml
    /// </summary>
    public partial class PracticeExamSelectionPage : Page
    {
        public PracticeExamSelectionPage()
        {
            InitializeComponent();
        }

        public PracticeExamSelectionPage(Frame parentFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private readonly Frame _parentFrame;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            _navService = _parentFrame.NavigationService;
            _navService.Navigating += Navigating;

        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            if (_navService == null) return;
            _navService.Navigating -= Navigating;
            _navService = null;
        }

        private void Navigating(object sender, NavigatingCancelEventArgs e)
        {
            _navEventArgs = e;

            if (e.NavigationMode == NavigationMode.Back)
            {
                e.Cancel = true;

                //var storyboard1 = Application.Current.FindResource("Animate.SlideInDownDelayedMid");
                var storyboard2 = Application.Current.FindResource("Animate.SlideOutDownDelayedMid");
                if (storyboard2 == null)
                {
                    if (_navService != null) _navService.Navigating -= Navigating;
                    if (_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();
                    return;
                }
                //var myStoryboard1 = (Storyboard)storyboard1;
                var myStoryboard2 = (Storyboard)storyboard2;
                myStoryboard2.Completed += new EventHandler(AnimationCompleted);
                //myStoryboard1.Begin(SidePanel);
                myStoryboard2.Begin(ContentGrid);
            }

        }

        private void ExitAnimation(Page page)
        {
            //var storyboard1 = Application.Current.FindResource("Animate.SlideInDownDelayedMid");
            var storyboard2 = Application.Current.FindResource("Animate.SlideInDownDelayedMid");
            if (storyboard2 == null)
            {
                _parentFrame?.Navigate(page);
                return;
            }
            //var myStoryboard1 = (Storyboard)storyboard1;
            var myStoryboard2 = (Storyboard)storyboard2;
            EventHandler handler = null;
            handler = (sender, args) =>
            {
                _parentFrame?.Navigate(page);

                myStoryboard2.Completed -= handler;
            };

            myStoryboard2.Completed += handler;
            //myStoryboard1.Begin(SidePanel);
            myStoryboard2.Begin(ContentGrid);
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            if (_parentFrame.NavigationService.CanGoBack)
            {
                if (_navService != null) _navService.Navigating -= Navigating;
                _parentFrame.NavigationService.GoBack();

            }
        }

        private void SetHoursButton_OnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = HoursToggleButton.IsChecked ?? false;
            if (!isChecked)
            {
                MinutesToggleButton.IsChecked = false;
                HoursToggleButton.IsChecked = true;
            }

            var buttonTag = (string)((Button) sender).Tag;
            var hours = Convert.ToInt32(HoursToggleButton.Tag);
            switch (buttonTag)
            {
                case "Increase":
                    hours += 1;
                    hours = hours > 12 ? 0 : hours;
                    break;
                case "Decrease":
                    hours -= 1;
                    hours = hours < 0 ? 12 : hours;
                    break;
            }
            
            HoursToggleButton.Tag = hours.ToString("00");
        }

        private void SetMinutesButton_OnClick(object sender, RoutedEventArgs e)
        {
            var isChecked = MinutesToggleButton.IsChecked ?? false;
            if (!isChecked)
            {
                HoursToggleButton.IsChecked = false;
                MinutesToggleButton.IsChecked = true;
            }

            var buttonTag = (string)((Button)sender).Tag;
            var minutes = Convert.ToInt32(MinutesToggleButton.Tag);
            switch (buttonTag)
            {
                case "Increase":
                    minutes += 1;
                    minutes = minutes > 59 ? 0 : minutes;
                    break;
                case "Decrease":
                    minutes -= 1;
                    minutes = minutes < 0 ? 59 : minutes;
                    break;
            }

            MinutesToggleButton.Tag = minutes.ToString("00");
        }


        private void MinutesToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            HoursToggleButton.IsChecked = false;
            MinutesToggleButton.IsChecked = true;
        }

        private void HoursToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            MinutesToggleButton.IsChecked = false;
            HoursToggleButton.IsChecked = true;
        }


        private PracticeMode getPracticeMode()
        {
            var withTimer = ModeCheckBox.IsChecked ?? false;
            var hours = TimeSpan.FromHours(Convert.ToInt32(HoursToggleButton.Tag));
            var minutes = TimeSpan.FromMinutes(Convert.ToInt32(MinutesToggleButton.Tag));
            var seconds = hours.TotalSeconds + minutes.TotalSeconds;
            return new PracticeMode(withTimer, seconds);
        }

        private void PracticeByTopicButton_OnClick(object sender, RoutedEventArgs e)
        {
            var examType = ((ComboBoxItem)ExamTypeComboBox.SelectedItem).Content.ToString();
            var page = new PracticeByTopicPage(_parentFrame, new PracticeByTopicService(), 
                getPracticeMode(), ExamQuestionType.getTypeEnum(examType));
            ExitAnimation(page);
        }

        private void PracticeByYearButton_OnClick(object sender, RoutedEventArgs e)
        {
            var examType = ((ComboBoxItem)ExamTypeComboBox.SelectedItem).Content.ToString();
            var page = new PracticeByYearPage(_parentFrame, new PracticeByYearService(), getPracticeMode(), ExamQuestionType.getTypeEnum(examType));
            ExitAnimation(page);
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

        private void ExamTypeComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var examType = ((ComboBoxItem) ExamTypeComboBox.SelectedItem).Content.ToString();
            switch (examType)
            {
                case ExamQuestionType.Theory:
                    ModeCheckBox.IsChecked = false;
                    ModeCheckBox.IsEnabled = false;
                    break;
                case ExamQuestionType.Objectives:
                    ModeCheckBox.IsEnabled = true;
                    break;
            }
        }
    }
}
