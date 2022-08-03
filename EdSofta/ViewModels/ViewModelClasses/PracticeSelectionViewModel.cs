using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class PracticeSelectionViewModel
    {
        public NotifyTaskCompletion<List<QuestionBankViewModel>> QuestionBanks { get; set; }

        
        public PracticeSelectionViewModel(IPracticeModeService practiceModeService, QuestionType questionType)
        {
            QuestionBanks = new NotifyTaskCompletion<List<QuestionBankViewModel>>(practiceModeService.getQuestionBanksAsync(questionType));
        }
    }
}
