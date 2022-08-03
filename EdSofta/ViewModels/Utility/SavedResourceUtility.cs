using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Models;
using EdSofta.ViewModels.Collections;
using Newtonsoft.Json;

namespace EdSofta.ViewModels.Utility
{
    internal abstract class SavedResourceUtility
    {

        public static List<StudyData> getStudyData(string subject)
        {
            var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Study"]);
            var path = $@"{proficiencyPath}{subject}.json";

            var jsonData = FileParser.readFile(path);
            if(string.IsNullOrWhiteSpace(jsonData)) return new List<StudyData>();
            return JsonConvert.DeserializeObject<List<StudyData>>(jsonData) ?? new List<StudyData>();

        }

        public static async Task<List<StudyData>> getStudyDataAsync(string subject)
        {
            var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Study"]);
            var path = $@"{proficiencyPath}{subject}.json";

            var jsonData = await FileParser.readFileAsync(path);
            if (string.IsNullOrWhiteSpace(jsonData)) return new List<StudyData>();
            return JsonConvert.DeserializeObject<List<StudyData>>(jsonData) ?? new List<StudyData>();

        }

        public static List<string> getSubjectsWithStudyData()
        {
            var studyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Study"]);
            return FileParser.getFileNames(studyPath);
        }

        public static void updateStudyData(string subject, string topic, DateTime date)
        {
            try
            {
                var studyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Study"]);
                var path = $@"{studyPath}{subject}.json";

                if (!File.Exists(path))
                {
                    initStudy(subject, path);
                }

                var jsonData = FileParser.readFile(path);

                if (string.IsNullOrWhiteSpace(jsonData)) return;
                var studyData = JsonConvert.DeserializeObject<List<StudyData>>(jsonData) ?? new List<StudyData>();

                var data = studyData.SingleOrDefault(x => x.topicName == topic);
                if (data == null) return;

                data.dateLastRead = date;
                data.readingHistory.Add(date);

                FileParser.SaveToJson(studyData, path);

            }
            catch
            {

            }
        }

        public static void initStudy(string subject, string filePath)
        {
            try
            {
                var topics = ContentResourceUtility.getStudyThemes(subject);

                var studyData = topics.Select(x =>
                    new StudyData{topicName = x, dateLastRead = DateTime.MinValue, readingHistory = new List<DateTime>()});

                FileParser.SaveToJson(studyData, filePath);
            }
            catch
            {

            }
        }

        public static List<ProficiencyData> getProficiencyData(string subject)
        {
            var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Proficiency"]);
            var path = $@"{proficiencyPath}{subject}.json";

            var proficiencyFileContent = FileParser.readFile(path);
            if (string.IsNullOrWhiteSpace(proficiencyFileContent)) return new List<ProficiencyData>();
            return JsonConvert.DeserializeObject<List<ProficiencyData>>(proficiencyFileContent) ??
                         new List<ProficiencyData>();
        }

        public static List<string> getSubjectsWithProficiency()
        {
            var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Proficiency"]);
            return FileParser.getFileNames(proficiencyPath);
        }

        public static async Task<List<string>> getSubjectsWithProficiencyAsync()
        {
            var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Proficiency"]);
            return await FileParser.getFileNamesAsync(proficiencyPath);
        }

        public static async Task<Dictionary<string, List<ProficiencyData>>> getAllProficienciesAsync()
        {
            var subjectsWithProficiency = await getSubjectsWithProficiencyAsync();
            var proficiencies = new Dictionary<string, List<ProficiencyData>>();
            foreach (var item in subjectsWithProficiency)
            {
                proficiencies.Add(item, getProficiencyData(item));
            }

            return proficiencies;
        }

        public static void updateTopicProficiency(string subject, List<Grade> topicGrades)
        {
            try
            {
                var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["Proficiency"]);
                var path = $@"{proficiencyPath}{subject}.json";

                if (!File.Exists(path))
                {
                    initProficiency(subject, path);
                }

                var proficiencyFileContent = FileParser.readFile(path);
                if (string.IsNullOrWhiteSpace(proficiencyFileContent)) return;
                var topics = JsonConvert.DeserializeObject<List<ProficiencyData>>(proficiencyFileContent) ??
                             new List<ProficiencyData>();

                foreach (var grade in topicGrades)
                {
                    var proficiencyData = topics.SingleOrDefault(x => x.topicName == grade.name);
                    if (proficiencyData == null) continue;

                    var currentScore = grade.score / grade.totalScore;
                    proficiencyData.proficiencyHistory.Add(currentScore);

                    var proficiency = proficiencyData.proficiencyHistory.Sum() / proficiencyData.proficiencyHistory.Count;
                    proficiencyData.proficiency = proficiency;
                    //proficiencyData.proficiencyHistory.Add(proficiency);
                }
                
                FileParser.SaveToJson(topics, path);
            }
            catch
            {
            }
        }

        private static void initProficiency(string subject, string filePath)
        {
            try
            {
                var topics = ContentResourceUtility.getTopics(subject, QuestionType.Objectives);

                var topicGrades = topics.Select(topic => 
                    new ProficiencyData {proficiency = null, proficiencyHistory = new List<double> {0.0}, topicName = topic.Name}).ToList();
                

                FileParser.SaveToJson(topicGrades, filePath);
            }
            catch
            {

            }
        }


        public static void saveResult(Result result)
        {
            try
            {
                var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"]);
                var path = $@"{proficiencyPath}{App.Files["Results"]}";

                var jsonData = FileParser.readFile(path);
                var results = JsonConvert.DeserializeObject<List<Result>>(jsonData) ??
                              new List<Result>();
                results.Add(result);

                FileParser.SaveToJson(results, path);

            }
            catch
            {

            }
        }

        public static List<Result> getResultHistory()
        {
            var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"]);
            var path = $@"{proficiencyPath}{App.Files["Results"]}";

            var jsonData = FileParser.readFile(path);
            if(string.IsNullOrWhiteSpace(jsonData)) return new List<Result>();

            return JsonConvert.DeserializeObject<List<Result>>(jsonData) ??
                          new List<Result>();
        }

        public static async Task<List<Result>> getResultHistoryAsync()
        {
            var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"]);
            var path = $@"{proficiencyPath}{App.Files["Results"]}";

            var jsonData = await FileParser.readFileAsync(path);
            if (string.IsNullOrWhiteSpace(jsonData)) return new List<Result>();

            return JsonConvert.DeserializeObject<List<Result>>(jsonData) ??
                   new List<Result>();
        }


        public static async Task<bool> deleteResultHistoryAsync()
        {
            var proficiencyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"]);
            var path = $@"{proficiencyPath}{App.Files["Results"]}";

            var jsonData = await FileParser.readFileAsync(path);
            if (string.IsNullOrWhiteSpace(jsonData)) return false;

            var results = JsonConvert.DeserializeObject<List<Result>>(jsonData);
            if (results == null) return false;

            results.Clear();
            FileParser.SaveToJson(results, path);
            return true;
        }

        public static async Task<bool> changeTheme(string value)
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = await FileParser.readFileAsync(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return false;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return false;

            appData.Theme = value;
            return FileParser.SaveToJson(appData, appDataPath);

        }

        public static async Task<string> getCurrentTheme()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = await FileParser.readFileAsync(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return ThemeHelper.GetWindowsTheme().ToString();

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return ThemeHelper.GetWindowsTheme().ToString();

            return appData.Theme;

        }

        public static async Task<bool> changeNotification(bool value)
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = await FileParser.readFileAsync(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return false;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return false;

            appData.Notifications = value;
            return FileParser.SaveToJson(appData, appDataPath);
        }

        public static async Task<bool> getNotificationValue()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = await FileParser.readFileAsync(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return false;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return false;

            return appData.Notifications;
        }


        public static int getStudyCount()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return 0;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return 0;

            return appData.StudyCount;
        }

        public static void incrementStudyCount()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return;

            appData.StudyCount += 1;
            FileParser.SaveToJson(appData, appDataPath);
        }

        public static void clearStudyCount()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return;

            appData.StudyCount = 0;
            FileParser.SaveToJson(appData, appDataPath);
        }

        public static int getTestCount()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return 0;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return 0;

            return appData.TestCount;
        }

        public static void incrementTestCount()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return;

            appData.TestCount += 1;
            FileParser.SaveToJson(appData, appDataPath);
        }

        public static void clearTestCount()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return;

            appData.TestCount = 0;
            FileParser.SaveToJson(appData, appDataPath);
        }

        public static string getActivationKey()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return string.Empty;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return string.Empty;

            return appData.ActivationKey;
        }


        public static bool getUserRegistered()
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return false;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return false;

            return appData.UserRegistered;
        }

        public static void setUserRegistered(bool value)
        {
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            var jsonData = FileParser.readFile(appDataPath);
            if (string.IsNullOrWhiteSpace(jsonData)) return;

            var appData = JsonConvert.DeserializeObject<AppData>(jsonData);
            if (appData == null) return;

            appData.UserRegistered = value;
            FileParser.SaveToJson(appData, appDataPath);
        }

    }
}
