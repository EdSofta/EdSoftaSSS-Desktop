using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;
using EdSofta.Models;
using EdSofta.ViewModels.ViewModelClasses;

namespace EdSofta.Interfaces
{
    internal interface IPracticeModeService
    {
        Task<List<QuestionBankViewModel>> getQuestionBanksAsync(QuestionType questionType);
    }
}
