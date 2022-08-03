using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using EdSofta.Repositories;

namespace EdSofta.Interfaces
{
    internal interface IGameService
    {
        Dictionary<string, List<string>> getProfessions();
        void addProfession(string profession, List<string> subjects);
        Task<Dictionary<string, string>> getQuestionsByDifficulty(List<string> subjects, string difficulty);
        Task<Dictionary<string, string>> getLevelQuestionsAsync(int level, List<string> subjectList);
        List<string> getProfessionSubjects(string profession);
        Task<bool> updateCoinAsync(Guid id, int coin);
        Task<bool> updateLevelAsync(Guid id, int level);
        bool tryGetCurrentGame(out Game game);
        Task<bool> createNewGame(Game game);
        Task<bool> updateGameAsync(Game game);
        Task<bool> resetGame();
    }
}
