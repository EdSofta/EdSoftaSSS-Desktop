using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.Services;

namespace EdSofta.Interfaces
{
    internal interface ILRecService
    {
        Task<bool> generateRecommendations();

        Task<bool> analyzeResultAsync();

        Task<bool> analyzeStudyMaterialsAsync();

        Task<List<LearningRecommendation>> getLearningRecommendationsAsync();

        Task<List<Performance>> getSubjectPerformancesAsync();

        Task<int> getResultsCountAsync();

        Task<string> getTestFinishedOnTimeAsync();

        Task<bool> deleteRecommendation(LearningRecommendation lRec);
    }
}
