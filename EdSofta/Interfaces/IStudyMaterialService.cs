using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Models;
using EdSofta.Repositories;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Interfaces
{
    internal interface IStudyMaterialService
    {
        Task<List<StudyMaterialDataViewModel>> getStudyMaterialsAsync(string subject);
        Task<List<string>> getStudySubjects();
        Task logStudyMaterials(string subject, StudyMaterialDataViewModel studyMaterialData);
        Task<string> getStudyMaterial(string subject, string theme);
        string formatStudyText(string studyMaterialText);

        Task<List<StudyViewModel>> getAllStudyMaterials();

        Task<bool> isTopicPracticeAvailableAsync(string subject, string topic);
    }
}
