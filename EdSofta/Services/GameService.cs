using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Interfaces;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using HtmlAgilityPack;

namespace EdSofta.Services
{
    class GameService : IGameService
    {
        public void addProfession(string profession, List<string> subjects)
        {
            GameResourceUtility.addProfession(profession, subjects);
        }

        public Dictionary<string, List<string>> getProfessions()
        {
            return GameResourceUtility.getProfessions();
        }

        public async Task<Dictionary<string, string>> getQuestionsByDifficulty(List<string> subjects, string difficulty)
        {
            var dictionary = new  Dictionary<string, string>();
            foreach (var subject in subjects)
            {
                var temp = await GameResourceUtility.getQuestionsAsync(subject);
                //var filteredList = temp.Where(x => string.Equals(difficulty, x.Value.Difficulty, StringComparison.OrdinalIgnoreCase))
                //    .Select(q => new KeyValuePair<int, string>(getQuestionNumber(q.Key), subject));

                foreach (var item in temp.Where(x => string.Equals(difficulty, x.Value.Difficulty, StringComparison.OrdinalIgnoreCase)))
                {
                    dictionary.Add($@"{subject} {GameResourceUtility.getQuestionNumber(item.Key)}", subject);
                }
                //dictionary.ToList().AddRange(filteredList);
            }

            return dictionary;
        }


        public async Task<Dictionary<string, string>> getLevelQuestionsAsync(int level, List<string> subjectList)
        {
            switch (level)
            {
                case 1:
                    return await getQuestions(subjectList, true, false, false, 1, 0, 0);
                case 2:
                    return await getQuestions(subjectList, true, true, false, 2, 1, 0);
                case 3:
                    return await getQuestions(subjectList, true, true, true, 2, 2, 1);
                case 4:
                    return await getQuestions(subjectList, false, true, true, 0, 2, 1);
                case 5:
                    return await getQuestions(subjectList, false, true, true, 0, 1, 2);
                case 6:
                    return await getQuestions(subjectList, false, false, true, 0, 0, 1);

            }

            return new Dictionary<string, string>();
        }

        public async Task<Dictionary<string, string>> getQuestions(List<string> subjects, bool easy, bool medium, bool hard, int easyRatio, int mediumRatio, int hardRatio)
        {
            var easyDictionary = new Dictionary<string, string>();
            var mediumDictionary = new Dictionary<string, string>();
            var hardDictionary = new Dictionary<string, string>();

            if (easy)
            {
                var list = await getQuestionsByDifficulty(subjects, Difficulty.Easy);
                foreach (var item in list)
                {
                    easyDictionary.Add(item.Key, item.Value);
                }
            }

            if (medium)
            {
                var list = await getQuestionsByDifficulty(subjects, Difficulty.Medium);
                foreach (var item in list)
                {
                    mediumDictionary.Add(item.Key, item.Value);
                }
            }

            if (hard)
            {
                var list = await getQuestionsByDifficulty(subjects, Difficulty.Hard);
                foreach (var item in list)
                {
                    hardDictionary.Add(item.Key, item.Value);
                }
            }


            var questionsDictionary = new Dictionary<string, string>();
            var totalRatio = easyRatio + mediumRatio + hardRatio;
            const int questions = 15;

            var easyList = Enumerable.Range(0, easyDictionary.Count).ToList();
            var easyLimit = (easyRatio * questions) / totalRatio;
            for (var i = 0; i < easyLimit; i++)
            {
                var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(easyList.Count - 1);
                var num = easyList[randomInt];
                var element = easyDictionary.ElementAt(num);
                questionsDictionary.Add(element.Key, element.Value);
                easyList.RemoveAt(randomInt);
            }

            var mediumList = Enumerable.Range(0, mediumDictionary.Count).ToList();
            var mediumLimit = (mediumRatio * questions) / totalRatio;
            for (var i = 0; i < mediumLimit; i++)
            {
                var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(mediumList.Count - 1);
                var num = mediumList[randomInt];
                var element = mediumDictionary.ElementAt(num);
                questionsDictionary.Add(element.Key, element.Value);
                mediumList.RemoveAt(randomInt);
            }

            var hardList = Enumerable.Range(0, hardDictionary.Count).ToList();
            var hardLimit = (hardRatio * questions) / totalRatio;
            for (var i = 0; i < hardLimit; i++)
            {
                var randomInt = ThreadSafeRandom.ThisThreadsRandom.Next(hardList.Count - 1);
                var num = hardList[randomInt];
                var element = hardDictionary.ElementAt(num);
                questionsDictionary.Add(element.Key, element.Value);
                hardList.RemoveAt(randomInt);
            }

            return questionsDictionary;
        }


        public List<string> getProfessionSubjects(string profession)
        {
            var professions = GameResourceUtility.getProfessions();
            List<string> subjects;
            return professions.TryGetValue(profession, out subjects) ? subjects : new List<string>();
        }

        public Task<bool> updateCoinAsync(Guid id, int coin)
        {
            throw new NotImplementedException();
        }

        public Task<bool> updateLevelAsync(Guid id, int level)
        {
            throw new NotImplementedException();
        }

        public bool tryGetCurrentGame(out Game game)
        {
            using (var dal = new UnitOfWork())
            {
                game = dal.GameRepository.SingleOrDefault(x => x.Current);
                return game != null;
            }
        }

        public async Task<bool> createNewGame(Game game)
        {
            using (var dal = new UnitOfWork())
            {
                dal.GameRepository.Add(game);
                return await dal.SaveChangesAsync();
            }
        }

        public async Task<bool> updateGameAsync(Game game)
        {
            using (var dal = new UnitOfWork())
            {
                dal.GameRepository.UpdateEntity(game);
                return await dal.SaveChangesAsync();
            }
        }

        public async Task<bool> resetGame()
        {
            using (var dal = new UnitOfWork())
            {
                var game = dal.GameRepository.SingleOrDefault(x => x.Current);
                if (game == null) return false;
                game.Current = false;
                dal.GameRepository.UpdateEntity(game);
                return await dal.SaveChangesAsync();
            }
        }
    }
}
