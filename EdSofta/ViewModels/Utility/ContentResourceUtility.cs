using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.ViewModels.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdSofta.ViewModels.Utility
{
    internal abstract class ContentResourceUtility
    {
        private static string getMetaDir(QuestionType type)
        {
            var metaPath = string.Empty;
            switch (type)
            {
                case QuestionType.Objectives:
                    metaPath = App.MetaPaths["objectives"];
                    break;
                case QuestionType.Theory:
                    metaPath = App.MetaPaths["theory"];
                    break;
                default:
                    metaPath = App.MetaPaths["objectives"];
                    break;
            }

            return metaPath;
        }

        public static List<string> getSubjects(QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            //return FileParser.getDirNames($@"{contentsPath}{App.MetaPaths["objectives"]}");
            switch (type)
            {
                case QuestionType.Objectives:
                    return FileParser.getDirNames($@"{contentsPath}{App.MetaPaths["objectives"]}");
                case QuestionType.Theory:
                    return FileParser.getDirNames($@"{contentsPath}{App.MetaPaths["theory"]}");
                default:
                    return new List<string>();
            }
            
        }

        public static async Task<List<string>> getSubjectsAsync(QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            //return await FileParser.getDirNamesAsync($@"{contentsPath}{App.MetaPaths["objectives"]}");
            switch (type)
            {
                case QuestionType.Objectives:
                    return await FileParser.getDirNamesAsync($@"{contentsPath}{App.MetaPaths["objectives"]}");
                case QuestionType.Theory:
                    return await FileParser.getDirNamesAsync($@"{contentsPath}{App.MetaPaths["theory"]}");
                default:
                    return new List<string>();
            }
        }

        public static List<string> getYears(string subject, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var metaPath = getMetaDir(type);
            return FileParser.getFileNames($@"{contentsPath}{metaPath}\{subject}").Where(x => x != "Topics").ToList();
        }

        public static async Task<List<string>> getYearsAsync(string subject, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);

            var files = await FileParser.getFileNamesAsync($@"{contentsPath}{getMetaDir(type)}\{subject}");
            return files.Where(x => x != "Topics").ToList();
        }

        public static List<Topic> getTopics(string subject, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{App.Files["Topics"]}";
            var topicFileContent =  FileParser.readFile(path);
            if (string.IsNullOrWhiteSpace(topicFileContent)) return new List<Topic>();
            var topics = JsonConvert.DeserializeObject<List<Topic>>(topicFileContent) ?? new List<Topic>();

            foreach (var topic in topics)
            {
                topic.Name = topic.Name.Trim();
            }
            return topics;
        }

        public static async Task<List<Topic>> getTopicsAsync(string subject, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{App.Files["Topics"]}";
            var topicFileContent = await FileParser.readFileAsync(path);
            if (string.IsNullOrWhiteSpace(topicFileContent)) return new List<Topic>();
            var topics = JsonConvert.DeserializeObject<List<Topic>>(topicFileContent) ?? new List<Topic>();

            foreach (var topic in topics)
            {
                topic.Name = topic.Name.Trim();
            }
            return topics;
        }

        public static QuestionDTO getQuestion(string subject, string year, int number, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{year}.json";

            //todo
            var questionsFileContent = FileParser.readFile(path);

            if (string.IsNullOrWhiteSpace(questionsFileContent)) return new QuestionDTO();
            var question = JObject.Parse(questionsFileContent)[$"Question {number.ToString()}"];
            return question != null ? question.ToObject<QuestionDTO>() : new QuestionDTO();

        }

        public static async Task<QuestionDTO> getQuestionAsync(string subject, string year, int number, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{year}.json";

            //todo
            var questionsFileContent = await FileParser.readFileAsync(path);

            if (string.IsNullOrWhiteSpace(questionsFileContent)) return new QuestionDTO();
            var question = JObject.Parse(questionsFileContent)[$"Question {number.ToString()}"];
            return question != null ? question.ToObject<QuestionDTO>() : new QuestionDTO();

        }

        public static async Task<int> getQuestionNumbersAsync(string subject, string year, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{year}.json";

            //todo
            var questionsFileContent = await FileParser.readFileAsync(path);

            if (string.IsNullOrWhiteSpace(questionsFileContent)) return 0;
            var count = JObject.Parse(questionsFileContent).Count;
            return count;
        }

        public static int getQuestionNumbers(string subject, string year, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{year}.json";

            //todo
            var questionsFileContent = FileParser.readFile(path);

            if (string.IsNullOrWhiteSpace(questionsFileContent)) return 0;
            var count = JObject.Parse(questionsFileContent).Count;
            return count;
        }

        public static List<string> getAnswers(string subject, string year, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{year}.json";

            //todo
            var questionsFileContent = FileParser.readFile(path);

            if (string.IsNullOrWhiteSpace(questionsFileContent)) return new List<string>();
            var answers = JObject.Parse(questionsFileContent)["Answers"];
            return answers != null ? answers.ToObject<List<string>>() : new List<string>();
        }

        public static async Task<List<string>> getAnswersAsync(string subject, string year, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{year}.json";

            //todo
            var questionsFileContent = await FileParser.readFileAsync(path);

            if (string.IsNullOrWhiteSpace(questionsFileContent)) return new List<string>();
            var answers = JObject.Parse(questionsFileContent)["Answers"];
            return answers != null ? answers.ToObject<List<string>>() : new List<string>();
        }

        public static string getAnswer(string subject, string year, int number, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{year}.json";

            //todo
            var questionsFileContent = FileParser.readFile(path);

            if (string.IsNullOrWhiteSpace(questionsFileContent)) return string.Empty;
            var answers = JObject.Parse(questionsFileContent)["Answers"];
            var answersList = answers != null ? answers.ToObject<string[]>() : new string[0];
            var answer = string.Empty;

            if (answersList != null && answersList.Length >= number && answersList.Length > 0)
            {
                answer = answersList[number - 1];
            }

            return answer;
        }


        public static List<int> getTopicsQuestions(string subject, string year, List<string> topics, QuestionType type)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"]);
            var path = $@"{contentsPath}{getMetaDir(type)}\{subject}\{year}.json";
            var result = new List<int>();
            //todo
            var questionsFileContent = FileParser.readFile(path);

            if (string.IsNullOrEmpty(questionsFileContent)) return result;
            var questions = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(questionsFileContent);
            if (questions == null) return result;

            var allQuestions = new Dictionary<string, QuestionDTO>();

            foreach (var question in questions)
            {
                try
                {
                    var parsed = question.Value.ToObject<QuestionDTO>();
                    allQuestions.Add(question.Key, parsed);
                }
                catch
                {

                }
            }

            foreach (var topic in topics)
            {
                
                var items = allQuestions.Where(x => x.Value.Topic.Trim().Equals(topic, StringComparison.OrdinalIgnoreCase)).Select(item => getQuestionNumber(item.Key));
                result.AddRange(items);
            }

            result.RemoveAll(x => x == -1);

            return result;

        }

        public static List<Topic> getTopicsQuestions(string subject, List<string> topics, QuestionType type)
        {
            var allTopics = getTopics(subject, type);
            return allTopics.Where(x => topics.Contains(x.Name.Trim())).ToList();
        }

        public static int getQuestionNumber(string idString)
        {
            if (idString == null) return -1;
            var list = idString.Split(' ').ToList();
            if (list.Count > 1)
            {
                return Convert.ToInt32(list[1]);
            }

            return -1;
        }

        public static List<string> getStudyThemes(string subject)
        {
            var studyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["study"]);
            //var studyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["StudyMaterials"]);
            var path = $@"{studyPath}\{subject}";

            return FileParser.getFileNames(path);
        }

        public static List<string> getStudySubjects()
        {
            var studyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["study"]);
            return FileParser.getDirNames(studyPath);
        }

        public static async Task<List<string>> getStudySubjectsAsync()
        {
            var studyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["study"]);
            return await FileParser.getDirNamesAsync(studyPath);
        }

        public static async Task<string> getStudyMaterialAsync(string subject, string theme)
        {
            var studyPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["study"]);
            var path = $@"{studyPath}\{subject}\{theme}.html";

            //todo
            return await FileParser.readFileAsync(path);
        }

        public static string getContentVersion()
        {
            var metaPath = Path.Combine(ViewModels.Utility.App.AppDataPath,
                ViewModels.Utility.App.ResourcePaths["Resources"], ViewModels.Utility.App.Files["Meta"]);

            var metaContent = FileParser.readFile(metaPath);
            if (string.IsNullOrWhiteSpace(metaContent)) return string.Empty;
            var version = JObject.Parse(metaContent)["version"];
            var currentContentVersion = version?.ToObject<string>();
            return currentContentVersion ?? string.Empty;
        }
    }
}
