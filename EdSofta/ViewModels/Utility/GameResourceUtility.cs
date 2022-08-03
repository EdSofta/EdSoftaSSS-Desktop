
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdSofta.ViewModels.Utility
{
    class GameResourceUtility
    {

        public static Dictionary<string, List<string>> getProfessions()
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["games"]);
            var path = $@"{contentsPath}\{App.Files["GameProfessions"]}";

            var professionsFileContent = FileParser.readFile(path);
            if (string.IsNullOrWhiteSpace(professionsFileContent)) return new Dictionary<string, List<string>>();
            var professionDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(professionsFileContent);
            return professionDictionary ?? new Dictionary<string, List<string>>();
        }

        public static void addProfession(string profession, List<string> subjects)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["games"]);
            var path = $@"{contentsPath}\{App.Files["GameProfessions"]}";

            var dictionary = getProfessions();
            if (!dictionary.ContainsKey(profession))
            {
                dictionary.Add(profession, subjects);
            }

            var jsonString = JsonConvert.SerializeObject(dictionary);
            FileParser.writeFile(path, jsonString);
        }

        public static List<string> getSubjects()
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["games"]);
            var subjectLists = FileParser.getFileNames(contentsPath);
            return subjectLists.Where(x => string.Equals(x, "profession", StringComparison.OrdinalIgnoreCase)).ToList();
        }



        public static GameQuestion getQuestion(string subject, int number)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["games"]);
            var path = $@"{contentsPath}\{subject}.json";

            //todo
            var questionsFileContent = FileParser.readFile(path);

            if (string.IsNullOrWhiteSpace(questionsFileContent)) return new GameQuestion();
            var question = JObject.Parse(questionsFileContent)[$"Question {number}"];
            return question != null ? question.ToObject<GameQuestion>() : new GameQuestion();

        }

        public static int getQuestionsSize(string subject)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["games"]);
            var path = $@"{contentsPath}\{subject}.json";

            //todo
            var professionsFileContent = FileParser.readFile(path);

            if (string.IsNullOrWhiteSpace(professionsFileContent)) return -1;
            var professionDictionary = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(professionsFileContent);
            return professionDictionary?.Count ?? -1;
        }

        public static async Task<Dictionary<string, GameQuestion>> getQuestionsAsync(string subject)
        {
            var contentsPath = Path.Combine(App.AppDataPath, App.ResourcePaths["ContentFiles"], App.MetaPaths["games"]);
            var path = $@"{contentsPath}\{subject}.json";

            //todo
            var professionsFileContent = await FileParser.readFileAsync(path);

            if (string.IsNullOrWhiteSpace(professionsFileContent)) return new Dictionary<string, GameQuestion>(); 
            var professionDictionary = JsonConvert.DeserializeObject<Dictionary<string, GameQuestion>>(professionsFileContent);
            return professionDictionary ?? new Dictionary<string, GameQuestion>();
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
    }
}
 