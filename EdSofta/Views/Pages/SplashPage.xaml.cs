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
using EdSofta.Migrations;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for SplashPage.xaml
    /// </summary>
    public partial class SplashPage : Page
    {
        private readonly Frame _parentFrame;
        public SplashPage(Frame parentFrame)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            Loaded += PageLoaded;
        }

        private async void PageLoaded(object sender, RoutedEventArgs e)
        {
            await InitializeApp();
            var userService = new UserService();
            var user = await userService.getCurrentUser();

            Page page;
            if (user == null)
            {
                page = new WelcomePage(_parentFrame);
            }
            else
            {
                page = new LandingPage(_parentFrame);
            }

            _parentFrame.Navigate(page);
        }


        private async Task InitializeApp()
         {
             try
             {
                 ViewModels.Utility.App.getConfigurations();
                 var setup = new AppSetup();

                 //await setup.PerformCheck();

                 setup.checkDBFile();
                 await setup.SetAppData();
                 await setup.SetTheme();

                 var isResourceComplete = await setup.checkContentResource();
                 if (!isResourceComplete)
                 {
                     SetupPanel.Visibility = Visibility.Visible;
                     await setup.RestoreContent();
                     ViewModels.Utility.App.getMetaDirectories();
                     SetupPanel.Visibility = Visibility.Hidden;
                 }


                 using (var upgradeContextMigration = new ApplicationDbContextMigration())
                 {
                     upgradeContextMigration.Initialize();
                 }


                 ViewModels.Utility.App.ProductKey = await AppValidation.getProductKey();
                 var activationKey = SavedResourceUtility.getActivationKey();
                 if (string.IsNullOrEmpty(activationKey)) return;
                 var activationService = new ActivationService();
                 ViewModels.Utility.App.IsActivated = await activationService.activateByKeyAsync(activationKey);
             }
             catch (Exception e)
             {
                 MessageBox.Show($"{e.Message}, {e.Source}, {e.InnerException}, {e.StackTrace}");
             }
         }

    }
}
