using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    class StudyResourceUtility
    {
        public static List<string> getStudyMaterialsSubjects()
        {
            var studyMaterialsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Resources"], App.MetaPaths["study"]);
            return FileParser.getDirNames($@"{studyMaterialsPath}");
        }

        public static async Task<List<string>> getStudyMaterialsSubjectsAsync()
        {
            var studyMaterialsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Resources"], App.MetaPaths["study"]);
            return await FileParser.getDirNamesAsync($@"{studyMaterialsPath}");
        }

        public static List<string> getStudyMaterialTopics(string subject)
        {
            var studyMaterialsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Resources"], App.MetaPaths["study"]);
            return FileParser.getFileNames($@"{studyMaterialsPath}\{subject}").ToList();
        }

        public static async Task<List<string>> getStudyMaterialTopicsAsync(string subject)
        {
            var studyMaterialsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Resources"], App.MetaPaths["study"]);
            return await FileParser.getFileNamesAsync($@"{studyMaterialsPath}\{subject}");
        }

        public static string getStudyMaterial(string subject, string topic)
        {
            var studyMaterialsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Resources"], App.MetaPaths["study"]);
            var path = $@"{studyMaterialsPath}\{subject}\{topic}.html";

            //todo
            var studyMaterialFileContent = FileParser.readFile(path);

            return !string.IsNullOrWhiteSpace(studyMaterialFileContent) ? studyMaterialFileContent : string.Empty;
        }

        public static async Task<string> getStudyMaterialAsync(string subject, string topic)
        {
            var studyMaterialsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Resources"], App.MetaPaths["study"]);
            var path = $@"{studyMaterialsPath}\{subject}\{topic}.html";

            //todo
            var studyMaterialFileContent = await FileParser.readFileAsync(path);

            return !string.IsNullOrWhiteSpace(studyMaterialFileContent) ? studyMaterialFileContent : string.Empty;
        }
    }
}
