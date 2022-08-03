using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Services;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using EdSofta.Views.Windows;
using MessageBox = System.Windows.MessageBox;
using Ionic.Zip;
using Application = System.Windows.Application;

namespace EdSofta.Views.Pages
{
    /// <summary>
    /// Interaction logic for ContentUpdatePage.xaml
    /// </summary>
    public partial class ContentUpdatePage : Page
    {
        public ContentUpdatePage()
        {
            InitializeComponent();
        }

        private bool _loaded;
        private NavigationService _navService;
        private NavigatingCancelEventArgs _navEventArgs;
        private readonly Frame _parentFrame;
        private readonly Frame _innerFrame;
        private LandingPageViewModel _landingPageViewModel;
        private string _fileName = string.Empty;
        private string _url = string.Empty;
        private IWebClientService<UpdateRequestData, ContentUpdateResponse> _webClientService = new WebClientService<UpdateRequestData, ContentUpdateResponse>();
        private readonly ContentUpdateViewModel _contentUpdateViewModel = new ContentUpdateViewModel();


        internal ContentUpdatePage(Frame parentFrame, Frame innerFrame, LandingPageViewModel landingPageViewModel)
        {
            InitializeComponent();
            _parentFrame = parentFrame;
            _innerFrame = innerFrame;
            _landingPageViewModel = landingPageViewModel;
            DataContext = _contentUpdateViewModel;

            Loaded += PageLoaded;
            Unloaded += PageUnloaded;
        }

        private async void PageLoaded(object sender, RoutedEventArgs e)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var dialog = new DialogWindow("No Internet Connection",
                    "Try turning on your Wi-Fi, connecting to a network, or checking the signal in your area", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
                return;
            }

            _contentUpdateViewModel.ProcessText = "Checking for updates";
            UpdateProgressBar.IsIndeterminate = true;
            var response = await checkForUpdate();
            UpdateProgressBar.IsIndeterminate = false;
            _contentUpdateViewModel.IsProgressVisible = false;

            if (response.status)
            {
                if (string.IsNullOrEmpty(response.url))
                {
                    _contentUpdateViewModel.ProcessText = "Your content is up to date";
                    _contentUpdateViewModel.IsCheckUpdateVisible = true;
                }
                else
                {
                    _contentUpdateViewModel.ProcessText = "New content available";
                    _contentUpdateViewModel.Message = "Content updated";
                    _contentUpdateViewModel.IsDownloadAvailable = true;
                    _contentUpdateViewModel.IsCheckUpdateVisible = false;
                    _url = response.url;
                }
            }
            else
            {
                _contentUpdateViewModel.ProcessText = "Your subscription license has expired";
                _contentUpdateViewModel.Message = "Subscribe to gain access to new and updated JAMB contents";
                _contentUpdateViewModel.IsLicenseExpired = true;
            }
        }


        private void navigateToActivation()
        {
            
            if (_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();

            var page = new ActivationPage(_innerFrame, _landingPageViewModel);
            _innerFrame.Navigate(page);
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
           
        }

        private async void PreviousButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_contentUpdateViewModel.IsProcessStarted)
            {
                var message = "All current progress will be lost, are you sure?";
                var dialog = new DialogWindow("Cancel update", message);
                dialog.Owner = System.Windows.Application.Current.MainWindow;

                var result = dialog.ShowDialog() ?? false;


                if (!result) return;

                CancelUpdate();
                await RollBackChanges();

                if (!_parentFrame.CanGoBack) return;
                _parentFrame.NavigationService.GoBack();

            }
            else
            {
                if(_parentFrame.CanGoBack) _parentFrame.NavigationService.GoBack();
            }
        }

        private async Task<ContentUpdateResponse> checkForUpdate()
        {
            var appData = new UpdateRequestData
            {
                productCode = Constants.Keys.ProductCode,
                activationKey = SavedResourceUtility.getActivationKey(),
                version = ContentResourceUtility.getContentVersion()  
            };

            return await _webClientService.postDataAsync(@"content/checkupdate", appData);
        }



        private string getFilename(string link)
        {
            var uri = new Uri(link);
            var filename = System.IO.Path.GetFileName(uri.LocalPath);
            return filename;
        }

        private readonly WebClient webClient = new WebClient();
        private void downloadFile()
        {
            _contentUpdateViewModel.IsDownloadAvailable = false;

            var tempPath = Path.Combine(ViewModels.Utility.App.AppDataPath, ViewModels.Utility.App.ResourcePaths["Temp"]);

            // This will download a large image from the web, you can change the value
            // i.e a textbox : textBox1.Text
            _fileName = getFilename(_url);

            using (webClient)
            {
                webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;
                webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                webClient.DownloadFileAsync(new Uri(_url), tempPath + "/" + _fileName);
            }
        }

        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // In case you don't have a progressBar Log the value instead 
            // Console.WriteLine(e.ProgressPercentage);
            _contentUpdateViewModel.ProgressValue = e.ProgressPercentage;
            _contentUpdateViewModel.ProgressText = 
                $"Downloaded {UtilityClass.SizeSuffix(e.BytesReceived)} of {UtilityClass.SizeSuffix(e.TotalBytesToReceive)}";
        }

        private async void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _contentUpdateViewModel.ProgressValue = 0;

            if (e.Cancelled)
            {
                
                var dialog = new DialogWindow("Update Cancelled",
                    "Content download has been stopped.", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();

                _contentUpdateViewModel.IsProgressVisible = false;
                _contentUpdateViewModel.ProcessText = "Check for update";
                _contentUpdateViewModel.IsCheckUpdateVisible = true;
                return;
            }

            if (e.Error != null) // We have an error! Retry a few times, then abort.
            {
                var dialog = new DialogWindow("An Error Occurred",
                    "Looks like something went wrong, try updating again.", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();

                _contentUpdateViewModel.IsProgressVisible = false;
                _contentUpdateViewModel.ProcessText = "Check for update";
                _contentUpdateViewModel.IsCheckUpdateVisible = true;
                _contentUpdateViewModel.IsDownloadAvailable = false;
                _contentUpdateViewModel.IsProcessStarted = false;
                return;
            }


            var tempPath = Path.Combine(ViewModels.Utility.App.AppDataPath, ViewModels.Utility.App.ResourcePaths["Temp"]);
            var path = $"{tempPath}{_fileName}";

            if (_contentUpdateViewModel.IsExtracting) return;

            //Extracting Download
            UpdateProgressBar.IsIndeterminate = true;
            _contentUpdateViewModel.ProcessText = "Unpacking updates";
            _contentUpdateViewModel.IsExtracting = true;
            //await extract(path, tempPath);

            //Extracts the objectives, games and others from downloaded file to a Content folder
            await extract(path, Path.Combine(tempPath, "Content"));
            _contentUpdateViewModel.ProgressText = string.Empty;

            //Backing Up/Compressing...
            //UpdateProgressBar.IsIndeterminate = true; 

            //var compPath = $"{tempPath}dist";
            //await compress(compPath, tempPath);

            //Compresses the files and folders of the current content folder and backs them up in Temp folder
            var currentContentPath = Path.Combine(ViewModels.Utility.App.AppDataPath,
                ViewModels.Utility.App.ResourcePaths["Resources"]);
            await compress(currentContentPath, tempPath);


            //Deleting Current Content
            clearCurrentContent();


            //Copying Update
            _contentUpdateViewModel.ProcessText = "Applying updates";
            UpdateProgressBar.IsIndeterminate = true;
            await copyContent();
            //UpdateProgressBar.IsIndeterminate = false;
            //_contentUpdateViewModel.ProgressValue = 0;

            clearTempFiles();
        }

        private void cancelDownload()
        {
            webClient.CancelAsync();
        }


        private async Task extract(string sourceFile, string extractLocation)
        {
            _extractCancelRequested = false;

            Task extract = Task.Run(() =>
            {
                using (var zip = ZipFile.Read(sourceFile))
                {
                    zip.ExtractProgress += zip_ExtractProgress;
                    zip.ExtractAll(extractLocation, ExtractExistingFileAction.OverwriteSilently);
                }
            });

            await extract;
        }

        void zip_ExtractProgress(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer <= 0) return;
            e.Cancel = _extractCancelRequested;
            _contentUpdateViewModel.ProgressValue = Convert.ToInt32(100 * e.BytesTransferred / e.TotalBytesToTransfer);
            _contentUpdateViewModel.ProgressText =
                $"Extracted {UtilityClass.SizeSuffix(e.BytesTransferred)} of {UtilityClass.SizeSuffix(e.TotalBytesToTransfer)}";
        }

        private bool _extractCancelRequested;

        private void cancelExtract()
        {
            if (!_extractCancelRequested) _extractCancelRequested = true;
        }


        private async Task<bool> compress(string sourceDir, string compressLocation)
        {
            Task<bool> comp = Task.Run(() =>
            {
                var archiveNameAndPath = Path.Combine(compressLocation, ViewModels.Utility.App.Files["Backup"]);
                using (var zip = new ZipFile())
                {
                    zip.AlternateEncoding = Encoding.UTF8;  // utf-8
                    //zip.AddDirectory(sourceDir, new DirectoryInfo(sourceDir).Name);

                    foreach (var item in Directory.GetDirectories(sourceDir))
                    {
                        var folderName = new DirectoryInfo(item).Name;
                        zip.AddDirectory(item, folderName);
                    }
                    // Adding files in the base directory
                    foreach (var file in Directory.GetFiles(sourceDir))
                    {
                        zip.AddFile(file);
                    }

                    zip.Comment = "This backup was created at " + System.DateTime.Now.ToString("G");
                    //await zip.SaveAsync(archiveNameAndPath).ConfigureAwait(false);
                    zip.Save(archiveNameAndPath);
                }
                return true;

            });

            return await comp;
        }

        private void StartUpdate()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var dialog = new DialogWindow("No Internet Connection",
                    "Try turning on your Wi-Fi, connecting to a network, or checking the signal in your area", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
                return;
            }

            if (!_contentUpdateViewModel.IsProcessStarted)
            {
                _contentUpdateViewModel.ProcessText = "Downloading updates";
                _contentUpdateViewModel.Message = string.Empty;
                _contentUpdateViewModel.IsProgressVisible = true;
                _contentUpdateViewModel.IsProcessStarted = true;
                downloadFile();
            }
        }

        private void CancelUpdate()
        {
            if (!_contentUpdateViewModel.IsExtracting)
            {
                cancelDownload();
            }
            else
            {
                cancelExtract();
                _contentUpdateViewModel.IsExtracting = false;
            }

            _contentUpdateViewModel.IsProcessStarted = false;
        }

        private async Task copyContent()
        {
            var tempContentFolder = Path.Combine(ViewModels.Utility.App.AppDataPath, ViewModels.Utility.App.ResourcePaths["Temp"], "Content");

            var appContentFolder = Path.Combine(ViewModels.Utility.App.AppDataPath, ViewModels.Utility.App.ResourcePaths["Resources"]);

            var copier = new FolderCopier
            {
                SourceFolder = new DirectoryInfo(tempContentFolder),
                DestinationFolder = new DirectoryInfo(appContentFolder)
            };

            copier.EventFileCopied += (sender, e) =>
            {
                _contentUpdateViewModel.ProgressText = "Finishing up";
            };

            UpdateProgressBar.IsIndeterminate = true;
            await copier.CopyAllAsync();
        }

        private void clearCurrentContent()
        {
            var contentPath = Path.Combine(ViewModels.Utility.App.AppDataPath,
                ViewModels.Utility.App.ResourcePaths["Resources"]);
            clearFolder(contentPath);
        }


        private void clearTempFiles()
        {
            var tempPath = Path.Combine(ViewModels.Utility.App.AppDataPath, ViewModels.Utility.App.ResourcePaths["Temp"]);
            clearFolder(tempPath);
            UpdateProgressBar.IsIndeterminate = false;
            _contentUpdateViewModel.IsProgressVisible = false;
            _contentUpdateViewModel.ProcessText = string.Empty;
            _contentUpdateViewModel.ProcessText = "Your content is up to date";
            _contentUpdateViewModel.IsCheckUpdateVisible = true;
            _contentUpdateViewModel.IsProcessStarted = false;
        }

        private void clearFolder(string path)
        {
            var folderClearer = new FolderClearer
            {
                SourceFolder = new DirectoryInfo(path)
            };

            folderClearer.clearAllAsync();
        }


        private async void CheckUpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                var dialog = new DialogWindow("No Internet Connection",
                    "Try turning on your Wi-Fi, connecting to a network, or checking the signal in your area", false);
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowDialog();
                return;
            }

            _contentUpdateViewModel.ProcessText = "Checking for updates";
            UpdateProgressBar.IsIndeterminate = true;
            _contentUpdateViewModel.IsProgressVisible = true;
            var response = await checkForUpdate();
            UpdateProgressBar.IsIndeterminate = false;
            _contentUpdateViewModel.IsProgressVisible = false;

            if (response.status)
            {
                if (string.IsNullOrEmpty(response.url))
                {
                    _contentUpdateViewModel.ProcessText = "Your content is up to date";
                    _contentUpdateViewModel.IsCheckUpdateVisible = true;
                }
                else
                {
                    _contentUpdateViewModel.ProcessText = "New content available";
                    _contentUpdateViewModel.Message = "Content updated";
                    _contentUpdateViewModel.IsDownloadAvailable = true;
                    _contentUpdateViewModel.IsCheckUpdateVisible = false;
                    _url = response.url;
                }
            }
            else
            {
                _contentUpdateViewModel.ProcessText = "Your subscription license has expired";
                _contentUpdateViewModel.Message = "Subscribe to gain access to new and updated JAMB contents";
            }
        }


        private async void DownloadButton_OnClick(object sender, RoutedEventArgs e)
        {
            StartUpdate();
        }


        private async void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            CancelUpdate();
            await RollBackChanges();
        }

        private async Task RollBackChanges()
        {
            var setup = new AppSetup();
            _contentUpdateViewModel.ProcessText = "Rolling back changes";
            _contentUpdateViewModel.IsCheckUpdateVisible = false;
            _contentUpdateViewModel.IsDownloadAvailable = false;
            _contentUpdateViewModel.IsProgressVisible = true;
            UpdateProgressBar.IsIndeterminate = true;
            await setup.PerformCheck();
            _contentUpdateViewModel.IsProcessStarted = false;
            UpdateProgressBar.IsIndeterminate = false;
        }

        private void ActivateButton_OnClick(object sender, RoutedEventArgs e)
        { 
            navigateToActivation();
        }
    }
}
