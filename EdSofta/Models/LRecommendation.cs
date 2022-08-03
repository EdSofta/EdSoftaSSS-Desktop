using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.Repositories;
using Newtonsoft.Json;

namespace EdSofta.Models
{
    public class LRecommendation
    {
        public static LearningRecommendation createStudyLR(string subject, string topic)
        {
            var studyData = new Study
            {
                Subject = subject,
                Topic = topic
            };

            var jsonData = JsonConvert.SerializeObject(studyData);

            var lRec = new LearningRecommendation
            {
                Id = Guid.NewGuid(),
                Title = $"Boost your {subject} scores",
                ExtraText = $"View study materials to help you improve on {topic}.",
                Type = LRType.Study,
                Data = jsonData
            };

            return lRec;
        }

        public static LearningRecommendation createPracticeLR(List<Practice> practiceData)
        {
            var jsonData = JsonConvert.SerializeObject(practiceData);

            var lRec = new LearningRecommendation
            {
                Id = Guid.NewGuid(),
                Title = "Practice recommendation",
                ExtraText = $"Improve on {practiceData.SelectMany(x => x.Topics).Count()} topic(s) by taking a test on these topics selected just for you.",
                Type = LRType.Test,
                Data = jsonData
            };

            return lRec;
        }
    }

    

    
}
