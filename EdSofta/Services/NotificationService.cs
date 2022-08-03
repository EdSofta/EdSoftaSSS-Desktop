using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Interfaces;
using EdSofta.Repositories;
using EdSofta.ViewModels.Utility;
using Newtonsoft.Json;

namespace EdSofta.Services
{
    class NotificationService : INotificationService
    {
        public async Task<bool> deleteAllNotifications()
        {
            using (var dal = new UnitOfWork())
            {
                var allNotifications = await dal.NotificationRepository.GetAllAsync();
                dal.NotificationRepository.RemoveRange(allNotifications);
                return await dal.SaveChangesAsync();
            }
        }

        public async Task<bool> deleteNotification(Notification notification)
        {
            using (var dal = new UnitOfWork())
            {
                var allNotifications = await dal.NotificationRepository.GetAllAsync();
                var record = allNotifications.ToList().SingleOrDefault(x => x.Id == notification.Id);
                if (record == null) return false;
                dal.NotificationRepository.Remove(record);
                return await dal.SaveChangesAsync();
            }
        }

        public async Task<bool> deleteNotificationByLRec(string lRecId)
        {
            using (var dal = new UnitOfWork())
            {
                var notifications = dal.NotificationRepository.Get(x => x.Type == NotificationType.LRec);

                var record = notifications.ToList().Find(notification =>
                {
                    var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(notification.Data);
                    var recId = data[Keys.RecId];
                    return lRecId.Equals(recId, StringComparison.OrdinalIgnoreCase);
                });

                if (record == null) return false;
                dal.NotificationRepository.Remove(record);
                return await dal.SaveChangesAsync();
            }
        }

        public Task<bool> generateContentUpdateNotificationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> generateLRecNotificationAsync(string message, string lRecId)
        {
            throw new NotImplementedException();
        }

        public async Task<ObservableCollection<Notification>> getNotificationsAsync()
        {
            using (var dal = new UnitOfWork())
            {
                var notifications = await dal.NotificationRepository.GetAllAsync();
                return notifications.ToObservableCollection();
            }
        }
    }
}
