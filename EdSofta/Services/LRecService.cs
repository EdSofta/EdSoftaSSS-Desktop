using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using NotificationType = EdSofta.Constants.NotificationType;

namespace EdSofta.Services
{
    class LRecService : ILRecService
    {


        public async Task<bool> generateRecommendations()
        {
            var results = SavedResourceUtility.getTestCount();
            var study = SavedResourceUtility.getStudyCount();
            var studyRecGenerated = false;
            var practiceRecGenerated = false;

            if (results != 0 && results % 10 == 0)
            {
                studyRecGenerated =  await analyzeResultAsync();
                if(studyRecGenerated) SavedResourceUtility.clearTestCount();
            }

            if (study != 0 && study % 10 == 0)
            {
               practiceRecGenerated = await analyzeStudyMaterialsAsync();
               if(practiceRecGenerated) SavedResourceUtility.clearStudyCount();
            }

            if (!studyRecGenerated && !practiceRecGenerated) return false;

            using (var dal = new UnitOfWork())
            {

                var recNotification = dal.NotificationRepository.SingleOrDefault(x => x.Type == NotificationType.LRec);
                if (recNotification != null)
                {
                    recNotification.Date = DateTime.Now;
                    dal.NotificationRepository.UpdateEntity(recNotification);
                }
                else
                {
                    var message = NotificationClass.generateLRMessage();
                    var notification = NotificationClass.createLRNotification(message);
                    dal.NotificationRepository.Add(notification);
                }
                
                return await dal.SaveChangesAsync();
            }

        }

        public async Task<bool> analyzeResultAsync()
        {

            //var resultHistory = SavedResourceUtility.getResultHistory();
            //if (resultHistory.Count % 4 == 0) return false;

            var subjectsWithProf = SavedResourceUtility.getSubjectsWithProficiency();
            if (subjectsWithProf.Count == 0) return false;

            using (var dal = new UnitOfWork())
            {
                var studyLRecs = new List<LearningRecommendation>();
                foreach (var subject in subjectsWithProf)
                {
                    var profData = SavedResourceUtility.getProficiencyData(subject);
                    var lowProf = profData.Where(x => x.proficiency != null && x.proficiency < 0.8).ToList();
                    if (lowProf.Count == 0) continue;
                    lowProf = lowProf.OrderBy(x => x.proficiency).ToList();
                    var proficiency = lowProf[0];

                    var topic = ContentResourceUtility.getTopics(subject, QuestionType.Objectives);
                    var isExist = topic.Exists(x =>
                        x.Name.Equals(proficiency.topicName, StringComparison.OrdinalIgnoreCase));

                    if (!isExist) continue;

                    var lRec = LRecommendation.createStudyLR(subject, proficiency.topicName);
                    studyLRecs.Add(lRec);
                }

                if (studyLRecs.Count == 0) return false;

                var studyRecs = dal.LrRepository.Get(x => x.Type == LRType.Study);
                dal.LrRepository.RemoveRange(studyRecs);

                dal.LrRepository.AddRange(studyLRecs);



                return await dal.SaveChangesAsync();
            }

        }


        public async Task<bool> analyzeStudyMaterialsAsync()
        {
            var selectedSubjects = new List<Practice>();

            var subjectsWithStudyData = SavedResourceUtility.getSubjectsWithStudyData();
            if (subjectsWithStudyData.Count == 0) return false;

            foreach (var subject in subjectsWithStudyData)
            {
                var selectedTopics = new List<string>();
                var studyData = SavedResourceUtility.getStudyData(subject);

                var profData = SavedResourceUtility.getProficiencyData(subject);
                var lowProf = profData.Where(x => x.proficiency != null && x.proficiency < 0.8).ToList();

                foreach (var data in studyData)
                {
                    if (data.readingHistory.Count <= 0 || data.readingHistory.Count % 2 != 0) continue;

                    if(!lowProf.Exists(x => x.topicName == data.topicName)) continue;
                    selectedTopics.Add(data.topicName);
                    //omitted statements adding number of questions to questionbank
                }

                if (selectedTopics.Count <= 0) continue;

                selectedSubjects.Add(
                        new Practice
                        {
                            Subject = subject,
                            Topics = selectedTopics
                        });
            }

            if (selectedSubjects.Count <= 0) return false;

            using (var dal = new UnitOfWork())
            {

                var testLr = dal.LrRepository.SingleOrDefault(x => x.Type == LRType.Test);
                if (testLr != null)
                {
                    dal.LrRepository.RemoveEntity(testLr);
                }

                var lRec = LRecommendation.createPracticeLR(selectedSubjects);
                dal.LrRepository.Add(lRec);

                //var message = NotificationClass.generateLRMessage(selectedSubjects);
                //var notification = NotificationClass.createLRNotification(message, lRec.Id.ToString());
                //dal.NotificationRepository.Add(notification);

                return await dal.SaveChangesAsync();
            }
        }


        public async Task<List<LearningRecommendation>> getLearningRecommendationsAsync()
        {
            using (var dal = new UnitOfWork())
            {
                var recommendations = await dal.LrRepository.GetAllAsync();
                return recommendations.ToList();
            }
        }

        public async Task<List<Performance>> getSubjectPerformancesAsync()
        {
            var performances = new List<Performance>();
            var proficiencies = await SavedResourceUtility.getAllProficienciesAsync();
            foreach (var item in proficiencies)
            {
                var validProf = item.Value.Where(x => x.proficiency != null);
                var proficiencyData = validProf as ProficiencyData[] ?? validProf.ToArray();
                var sum = proficiencyData.Sum(x => x.proficiency);
                var average = sum / proficiencyData.Count();
                if (average == null) continue;
                var percentage = (int)(average * 100);
                performances.Add(new Performance{Subject = item.Key, Percentage = percentage});
            }

            return performances;
        }

        public async Task<int> getResultsCountAsync()
        {
            var results = await SavedResourceUtility.getResultHistoryAsync();
            return results.Count;
        }

        public async Task<string> getTestFinishedOnTimeAsync()
        {
            var results = await SavedResourceUtility.getResultHistoryAsync();
            results = results.Where(x => x.totalTime > 0).ToList();
            var completedOnTime = results.Where(x => x.timeSpent < x.totalTime).ToList();
            return $"{completedOnTime.Count}/{results.Count}";
        }

        public async Task<bool> deleteRecommendation(LearningRecommendation lRec)
        {
            using (var dal = new UnitOfWork())
            {
                var allRecs = await dal.LrRepository.GetAllAsync();
                var recRecord = allRecs.SingleOrDefault(x => x.Id == lRec.Id);
                if (recRecord != null)
                {
                    dal.LrRepository.RemoveEntity(recRecord);
                }
                return await dal.SaveChangesAsync();
            }
        }
    }
}
