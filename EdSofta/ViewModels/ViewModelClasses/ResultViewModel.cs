using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;

namespace EdSofta.ViewModels.ViewModelClasses
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    internal class ResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private readonly List<Grade> resultGrades;
        public double TotalTime { get; set; }
        public double TimeUsed { get; set; }
        public double AverageTime { get; set; }
        public string Score => getScore();
        public int Percentage => getPercentage();
        public string Grade => getGrade();

        public string TimeUsedString => TimeUsed.HumanizeTime();

        public string AverageTimeString => AverageTime.HumanizeTime();

        public string SubjectsList => Grades.ToList().Select(x => x.Name).HumanizeList();

        public string PercentageString => $"You scored {Grade}";
    
        public DateTime Date { get; set; }

        public string LongDate => Date.ToLongDateString();

        public string ShortTime => Date.ToShortTimeString();


        public ObservableCollection<SubjectTopicGradeViewModel> Grades { get; set; }

        public ResultViewModel(Result result)
        {
            TimeUsed = result.timeSpent.ConvertMillisecondsToSeconds();
            AverageTime = result.averageTime.ConvertMillisecondsToSeconds();
            TotalTime = result.totalTime.ConvertMillisecondsToSeconds();
            Date = result.date;
            resultGrades = result.result;
            Grades = result.topicGrades.Select(x => new SubjectTopicGradeViewModel(x.Key, x.Value))
                .ToObservableCollection();
        }


        public int getPercentage()
        {
            var perc = 0.0;
            foreach (var grade in resultGrades)
            {
                perc += ((grade.score * 100) / grade.totalScore);
            }

            return (int)perc;
            

        }

        public string getScore()
        {
            return $"{resultGrades[0].score}/{resultGrades[0].totalScore}";
        }

        public string getGrade()
        {
           
            var perc = 0.0;
            foreach (var grade in resultGrades)
            {
                perc += ((grade.score * 100) / grade.totalScore);
            }

            return getGrade((int) perc);

        }


        public string getGrade(int percentage)
        {
            if (percentage >= 75)
            {
                return "A1";
            }

            if (percentage >= 70)
            {
                return "B2";
            }

            if (percentage >= 65)
            {
                return "B3";
            }

            if (percentage >= 60)
            {
                return "C4";
            }

            if (percentage >= 55)
            {
                return "C5";
            }

            if (percentage >= 50)
            {
                return "C6";
            }

            if (percentage >= 45)
            {
                return "D7";
            }

            if (percentage >= 40)
            {
                return "E8";
            }

            return "F9";
        }

    }

   
}
