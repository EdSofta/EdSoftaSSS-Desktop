using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.GameViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class GameQuestionViewModel
    {
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }

        public string Question { get; set; }
        public  string Answer { get; set; }

        public bool Passed { get; set; }

        public string getCorrectAnswer()
        {

            var dictionary = new Dictionary<string, string>
            {
                {"A", OptionA}, {"B", OptionB}, {"C", OptionC}, {"D", OptionD}
            };

            var correctOption = string.Empty;

            foreach (var option in dictionary.Where(option => string.Equals(option.Value, Answer, StringComparison.OrdinalIgnoreCase)))
            {
                correctOption = option.Key;
            }

            return correctOption;
        }

        public bool isCorrectlyAnswered(string selectedOption)
        {
            var selectedAnswer = string.Empty;
            switch (selectedOption)
            {
                case "A":
                    selectedAnswer = OptionA;
                    break;
                case "B":
                    selectedAnswer = OptionB;
                    break;
                case "C":
                    selectedAnswer = OptionC;
                    break;
                case "D":
                    selectedAnswer = OptionD;
                    break;
            }
            Passed = string.Equals(selectedAnswer, Answer, StringComparison.OrdinalIgnoreCase);
            return Passed;
        }
    }
}
