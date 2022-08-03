using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EdSofta.ViewModels.Utility
{
    internal class FileParser
    {
        public static string readFile(string filePath)
        {
            try
            {
                string fileData;
                var fileStream = File.Open(filePath, File.Exists(filePath) ? FileMode.Open : FileMode.OpenOrCreate);
                using (var reader = new StreamReader(fileStream))
                {
                    fileData = reader.ReadToEnd();
                }

                fileStream.Close();
                return fileData;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static async Task<string> readFileAsyncs(string filePath)
        {
            try
            {
                string fileData = string.Empty;
                //var fileStream = new FileStream(filePath,
                //    File.Exists(filePath) ? FileMode.Open : FileMode.OpenOrCreate,
                //    FileAccess.Read, FileShare.None);
                using (var fileStream = File.Open(filePath, File.Exists(filePath) ? FileMode.Open : FileMode.OpenOrCreate))
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        fileData = await reader.ReadToEndAsync();
                    }
                }
                
                return fileData;
            }
            catch
            {
                return string.Empty;
            }
        }


        public static async Task<string> readFileAsync(string filePath)
        {
            try
            {
                using (var sourceStream =
                    new FileStream(
                        filePath,
                        File.Exists(filePath) ? FileMode.Open : FileMode.OpenOrCreate,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 4096, useAsync: true))
                {
                    var sb = new StringBuilder();

                    byte[] buffer = new byte[0x1000];
                    int numRead;
                    while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                        sb.Append(text);
                    }

                    return sb.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }

            
        }

        public static void writeFile(string filePath, string fileContent)
        {
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, fileContent);
            }
        }


        //ToDo
        public static void writeFileAsync(string filePath, string fileContent)
        {
            if (File.Exists(filePath))
            {
                
            }
        }

        public static async Task<bool> writeBytesAsync(string filePath, byte[] fileContent)
        {
            try
            {
                using (FileStream sourceStream = new FileStream(filePath,
                    FileMode.Append, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    await sourceStream.WriteAsync(fileContent, 0, fileContent.Length);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async Task writeTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }

        public static List<string> getDirNames(string filePath)
        {
            if (!Path.IsPathRooted(filePath)) return new List<string>();
            try
            {
                return Directory.GetDirectories(filePath).ToList().Select(x => new DirectoryInfo(x).Name)
                    .ToList();

            }
            catch
            {
                return new List<string>();
            }
        }


        public static async Task<List<string>> getDirNamesAsync(string filePath)
        {
            try
            {
                return await Task.Run(() => getDirNames(filePath));
            }
            catch
            {
                return new List<string>();
            }
        }


        public static List<string> getFileNames(string filePath)
        {
            if (!Path.IsPathRooted(filePath)) return new List<string>();
            try
            {
                return new DirectoryInfo(filePath).GetFiles().Select(x => x.Name.Split('.')[0]).ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public static async Task<List<string>> getFileNamesAsync(string filePath)
        {
            try
            {
                return await Task.Run(() => getFileNames(filePath));
            }
            catch
            {
                return new List<string>();
            }
        }


        public static bool SaveToJson<T>(T data, string filePath)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetExecutingDirectoryName()
        {
            try
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly == null) return Assembly.GetExecutingAssembly().Location;
                var location = new Uri(entryAssembly.GetName().CodeBase);
                var path = new FileInfo(location.LocalPath).Directory.FullName;
                return path;

            }
            catch
            {
                return Assembly.GetExecutingAssembly().Location;
            }
        }

    }
}
