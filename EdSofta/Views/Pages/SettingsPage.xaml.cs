using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EdSofta.Constants;
using EdSofta.Models;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }


        private readonly Frame _parentFrame;
        private readonly Frame _outerFrame;
        private LandingPageViewModel _landingPageViewModel;
        private SettingsViewModel _settingsViewModel;

        internal SettingsPage(Frame parentFrame, Frame outerFrame, LandingPageViewModel landingPageViewModel)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _outerFrame = outerFrame;
            _landingPageViewModel = landingPageViewModel;
            _settingsViewModel = landingPageViewModel.SettingsViewModel;
            DataContext = _settingsViewModel;
        }

        private async void ToggleButton_OnPreviewMouseDown(object sender, RoutedEventArgs e)
        {
            
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await _settingsViewModel.ChangeNotificationSettings();
        }

        private async Task SetTheme(string theme)
        {
            //var button = (ToggleButton)sender;
            //var isChecked = button.IsChecked ?? false;
            //var theme = isChecked ? "Dark" : "Light";
            var value = "";
            switch (theme)
            {
                case Theme.Default:
                    value = ThemeHelper.GetWindowsTheme().ToString();
                    break;
                default:
                    value = theme;
                    break;
            }

            var isSuccessful = await _settingsViewModel.ChangeThemeSettings(theme);
            if(!isSuccessful) return;
            ThemeHelper.SetAppTheme(value);
        }

        private async void ThemeRadioButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var themeItem = ((RadioButton)sender).DataContext as ThemeViewModel;
            if (themeItem == null) return;
            await SetTheme(themeItem.Value);
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

        private void ManageUsersBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var page = new UsersPage(_parentFrame, _landingPageViewModel.UsersViewModel);
            _parentFrame.Navigate(page);
        }

        private void ContentUpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            var page = new ContentUpdatePage(_outerFrame, _parentFrame, _landingPageViewModel);
            _outerFrame.Navigate(page);
        }

        private void LicensesBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var page = new LicensesPage();
            _parentFrame.Navigate(page);
        }

        private void TermsPrivacyBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var longUrl = "edsofta.com/terms-of-use";
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

        private void AccountLinkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var longUrl = "edsofta.com/login";
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

        private void AppUpdateButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                Process process = Process.Start(ViewModels.Utility.App.UpdaterModulePath, "/checknow");
                process?.Close();
            }
            catch
            {
                var dialog = new DialogWindow("An error occured", "Looks like something went wrong", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
            }
        }

        private void SetUpdateButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                Process process = Process.Start(ViewModels.Utility.App.UpdaterModulePath, "/configure");
                process?.Close();
            }
            catch
            {
                var dialog = new DialogWindow("An error occured", "Looks like something went wrong", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
            }
        }
    }
}
