using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using EdSofta.Constants;
using EdSofta.Enums;
using EdSofta.Repositories;
using Newtonsoft.Json;
using NotificationType = EdSofta.Constants.NotificationType;

namespace EdSofta.Models
{
    public class NotificationClass
    {
        public static string generateLRMessage(string subject, string topic)
        {
            return $"Study materials for {topic} to improve your {subject}.";
        }

        public static string generateLRMessage(List<Practice> practiceData)
        {
            return
                $"Improve on {practiceData.SelectMany(x => x.Topics).Count()} topic(s) by taking a test on these topics selected just for you.";
        }

        public static string generateLRMessage()
        {
            return "You have new learning recommendations!";
        }


        public static Notification createLRNotification(string message)
        {
            
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Data = string.Empty,
                ExtraText = message,
                Type = NotificationType.LRec,
                Date = DateTime.Now
            };

            return notification;
        }

        public static Notification CreateContentUpdateNotification(UpdateResultType resultType)
        {
            var message = string.Empty;
            switch (resultType)
            {
                case UpdateResultType.UpdateAvailable:
                    message = "There are new content updates available.";
                    break;
                case UpdateResultType.LicenseExpired:
                    message = "There are new content updates available, activate your app to get updated materials.";
                    break;
                case UpdateResultType.NoUpdateAvailable:
                    message = "there are no updates available.";
                    break;
            }

            return new Notification
            {
                Id = Guid.NewGuid(),
                Data = string.Empty,
                ExtraText = message,
                Type = NotificationType.Content,
                Date = DateTime.Now
            };
        }

        public static Notification createLRNotification(string message, string id)
        {
            var map = new Dictionary<string, string> {{Keys.RecId, id}};
            var jsonData = JsonConvert.SerializeObject(map);

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Data = jsonData,
                ExtraText = message,
                Type = NotificationType.LRec,
                Date = DateTime.Now
            };

            return notification;
        }
    }
}
