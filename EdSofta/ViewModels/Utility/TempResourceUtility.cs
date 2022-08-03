using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    internal abstract class TempResourceUtility
    {
        public static async Task<bool> writeContentUpdateFileAsync(byte[] downloadedBytes, string fileName)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //var path = Path.Combine(desktopPath, "Downloads");

            var tempPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Temp"]);
            var path = $@"{tempPath}{fileName}";

            return await FileParser.writeBytesAsync(path, downloadedBytes);
        }
    }
}
