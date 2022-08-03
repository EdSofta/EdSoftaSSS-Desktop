using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdSofta.ViewModels.Utility
{
    abstract class App
    {
       
        public static string ProductKey = string.Empty;
        
        public static string ActivationKey = string.Empty;
        
        public static bool IsActivated = false;

        public static bool IsContentUpdateChecked = false;

        public static string UpdaterModulePath = string.Empty;

        public static Dictionary<string, string> ResourcePaths = new Dictionary<string, string>();

        public static Dictionary<string, string> Files = new Dictionary<string, string>();

        public static Dictionary<string, string> Variables = new Dictionary<string, string>();

        public static Dictionary<string, string> MetaPaths = new Dictionary<string, string>();

        //public static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string AppDataPath = @"C:\";
        public static readonly string CommonAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

        internal static void getConfigurations()
        {
            var resourcePaths = ConfigurationManager.GetSection("ResourcePaths") as NameValueCollection;
            var fileNames = ConfigurationManager.GetSection("FileNames") as NameValueCollection;
            var variables = ConfigurationManager.GetSection("Variables") as NameValueCollection;

            if (resourcePaths == null || resourcePaths.Count == 0) return;

            foreach (var key in resourcePaths.AllKeys)
            {
                ResourcePaths.Add(key, resourcePaths[key]);
            }

            if (fileNames == null || fileNames.Count == 0) return;

            foreach (var key in fileNames.AllKeys)
            {
                Files.Add(key, fileNames[key]);
            }

            if (variables == null || variables.Count == 0) return;

            foreach (var key in variables.AllKeys)
            {
                Variables.Add(key, variables[key]);
            }

            getMetaDirectories();

        }

        internal static void getMetaDirectories()
        {
            var metaPath = Path.Combine(ViewModels.Utility.App.AppDataPath,
                ViewModels.Utility.App.ResourcePaths["Resources"], ViewModels.Utility.App.Files["Meta"]);

            var metaContent = FileParser.readFile(metaPath);
            if (string.IsNullOrEmpty(metaContent)) return;

            if (string.IsNullOrWhiteSpace(metaContent)) return;
            var directories = JObject.Parse(metaContent)["dirs"];
            var allDirs = directories?.ToObject<Dictionary<string, string>>();
            if (allDirs == null) return;

            MetaPaths = allDirs;
        }
        
    }
}
