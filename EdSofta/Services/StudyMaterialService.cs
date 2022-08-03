using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;
using EdSofta.ViewModels.ViewModelClasses;
using HtmlAgilityPack;

namespace EdSofta.Services
{
    internal class StudyMaterialService : IStudyMaterialService
    {
        public async Task<string> getStudyMaterial(string subject, string theme)
        {
            return await ContentResourceUtility.getStudyMaterialAsync(subject, theme);
        }

        public string formatStudyText(string studyMaterialText)
        {
            var mainDoc = new HtmlDocument();
            mainDoc.LoadHtml(studyMaterialText);
            
            foreach (var htmlNode in mainDoc.DocumentNode.Descendants().ToList())
            {
                if (htmlNode.Name == "p")
                {
                    htmlNode.ParentNode.ReplaceChild(HtmlNode.CreateNode(htmlNode.InnerText + "."), htmlNode);
                }

                if (htmlNode.Name != "img") continue;
                var alt = htmlNode.GetAttributeValue("alt", "");
                var nodeForReplace = HtmlTextNode.CreateNode(alt);
                htmlNode.ParentNode.ReplaceChild(nodeForReplace, htmlNode);

            }
            var cleanText = mainDoc.DocumentNode.InnerText;
            //return cleanText;
            return cleanText.Replace("&nbsp;", " ");
        
        }

        public async Task<List<StudyMaterialDataViewModel>> getStudyMaterialsAsync(string subject)
        {
            var activeStudied = SavedResourceUtility.getSubjectsWithStudyData();
            var subjectsData = new List<StudyData>();
            if (activeStudied.Contains(subject))
            {
                subjectsData = await SavedResourceUtility.getStudyDataAsync(subject);
            }

            if (subjectsData.Count == 0)
            {
                var themes = ContentResourceUtility.getStudyThemes(subject);
                var studyMaterials = themes.Select(x =>
                    new StudyMaterialDataViewModel {Name = x, LastRead = DateTime.MinValue}).ToList();
                return studyMaterials;
            }

            return subjectsData
                .Select(x => new StudyMaterialDataViewModel{Name = x.topicName, LastRead = x.dateLastRead}).ToList();

        }

        public async Task<StudyViewModel> getStudyMaterialsDataAsync(string subject)
        {
            var activeStudied = SavedResourceUtility.getSubjectsWithStudyData();
            var subjectsData = new List<StudyData>();
            if (activeStudied.Contains(subject))
            {
                subjectsData = await SavedResourceUtility.getStudyDataAsync(subject);
            }

            if (subjectsData.Count == 0)
            {
                var themes = ContentResourceUtility.getStudyThemes(subject);
                var studyMaterials = themes.Select(x =>
                    new StudyMaterialDataViewModel { Name = x, LastRead = DateTime.MinValue }).ToList();
                return new StudyViewModel { Subject = subject, StudyMaterials = studyMaterials.ToObservableCollection()};
            }

            var data = subjectsData
                .Select(x => new StudyMaterialDataViewModel { Name = x.topicName, LastRead = x.dateLastRead }).ToList();

            return new StudyViewModel{Subject = subject, StudyMaterials = data.ToObservableCollection()};
        }

        public async Task<List<string>> getStudySubjects()
        {
            return await ContentResourceUtility.getStudySubjectsAsync();
        }

        public async Task logStudyMaterials(string subject, StudyMaterialDataViewModel studyMaterialData)
        {
            await Task.Run(() =>
            {
                SavedResourceUtility.updateStudyData(subject, studyMaterialData.Name, studyMaterialData.LastRead);
            });
        }


        public async Task<List<StudyViewModel>> getAllStudyMaterials()
        {
            var studyMaterialsTasks = new List<Task<StudyViewModel>>();
            var subjects = await getStudySubjects();
            foreach (var subject in subjects)
            {
                studyMaterialsTasks.Add(getStudyMaterialsDataAsync(subject));
            }

            var result = await Task.WhenAll(studyMaterialsTasks);
            SavedResourceUtility.incrementStudyCount();
            return result.ToList();
        }

        public async Task<bool> isTopicPracticeAvailableAsync(string subject, string topic)
        {
            var topics = await ContentResourceUtility.getTopicsAsync(subject, QuestionType.Objectives);
            return topics.Exists(x => x.Name.Equals(topic, StringComparison.OrdinalIgnoreCase));
        }
    }
}
