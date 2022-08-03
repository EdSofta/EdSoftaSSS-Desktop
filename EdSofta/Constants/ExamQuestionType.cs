using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Enums;

namespace EdSofta.Constants
{
    public static class ExamQuestionType
    {
        public const string Objectives = "Objectives";
        public const string Theory = "Theory";

        public static QuestionType getTypeEnum(string examQuestionType)
        {
            switch (examQuestionType)
            {
                case Objectives:
                    return QuestionType.Objectives;
                case Theory:
                    return QuestionType.Theory;
                default:
                    return QuestionType.Objectives;
            }
        }
    }
}
