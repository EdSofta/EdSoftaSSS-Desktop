using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EdSofta.Constants;
using Ionic.Zip;
using Newtonsoft.Json.Linq;

namespace EdSofta.ViewModels.Utility
{
    internal class AppSetup
    {
        public async Task PerformCheck()
        {
            checkDBFile();
            await SetAppData();
            var isResourceComplete = await checkContentResource();
            if (!isResourceComplete)
            {
                await RestoreContent();
            }
        }


        public async Task SetAppData()
        {
            await AppValidation.getProductKey();
        }

        public async Task SetTheme()
        {
            var currentTheme = await SavedResourceUtility.getCurrentTheme();

            var value = "";
            switch (currentTheme)
            {
                case Theme.Default:
                    value = ThemeHelper.GetWindowsTheme().ToString();
                    break;
                default:
                    value = currentTheme;
                    break;
            }
            ThemeHelper.SetAppTheme(value);
        }

        public async Task RestoreContent()
        {
            clearCurrentContent();
            var archivePath = Path.Combine(App.AppDataPath, App.ResourcePaths["Temp"], ViewModels.Utility.App.Files["Backup"]);
            if(File.Exists(archivePath))
            {
                await restoreBackup(archivePath);
            }
            else
            {
                await copyContent();
            }
        }

        
        private async Task restoreBackup(string sourceFile)
        {
            var extractLocation = Path.Combine(App.AppDataPath, App.ResourcePaths["Resources"]);

            Task extract = Task.Run(() =>
            {
                using (var zip = ZipFile.Read(sourceFile))
                {
                    zip.ExtractAll(extractLocation, ExtractExistingFileAction.OverwriteSilently);
                }
            });

            await extract;
        }

        private void clearCurrentContent()
        {
            var contentPath = Path.Combine(ViewModels.Utility.App.AppDataPath,
                ViewModels.Utility.App.ResourcePaths["Resources"]);

            var folderClearer = new FolderClearer
            {
                SourceFolder = new DirectoryInfo(contentPath)
            };

            folderClearer.clearAllAsync();
        }

        private async Task copyContent()
        {
            
            var assetsPath = Path.Combine(ViewModels.Utility.App.AppDataPath, ViewModels.Utility.App.ResourcePaths["ContentAssets"]);
            var contentPath = Path.Combine(ViewModels.Utility.App.AppDataPath, ViewModels.Utility.App.ResourcePaths["Resources"]);

            var copier = new FolderCopier
            {
                SourceFolder = new DirectoryInfo(assetsPath),
                DestinationFolder = new DirectoryInfo(contentPath)
            };

            await copier.CopyAllAsync();
        }

        public async Task<bool> checkContentResource()
        {
            try
            {
                var metaPath = Path.Combine(ViewModels.Utility.App.AppDataPath,
                    ViewModels.Utility.App.ResourcePaths["Resources"], ViewModels.Utility.App.Files["Meta"]);

                if (!File.Exists(metaPath)) return false;

                var metaContent = await FileParser.readFileAsync(metaPath);
                if (string.IsNullOrWhiteSpace(metaContent)) return false;
                var directories = JObject.Parse(metaContent)["dirs"];
                var allDirs = directories?.ToObject<Dictionary<string, string>>();
                if (allDirs == null) return false;

                foreach (var item in allDirs)
                {
                    if (item.Value == null) continue;
                    var resourcePath = Path.Combine(ViewModels.Utility.App.AppDataPath,
                        ViewModels.Utility.App.ResourcePaths["Resources"], item.Value);
                    if (!Directory.Exists(resourcePath)) return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void checkDBFile()
        {
            var path = FileParser.GetExecutingDirectoryName();
            var packageDb = Path.Combine(path, App.Files["PackageDb"]);
            var appDb = Path.Combine(path, App.Files["AppDb"]);
            if (!File.Exists(packageDb)) return;

            try
            {
                if (File.Exists(appDb)) return;
                File.Copy(packageDb, appDb);
            }
            catch
            {
                //MessageBox.Show("An error occured", "Failed operation");
            }
        }

        
    }
}
