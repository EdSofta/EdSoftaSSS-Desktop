using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Repositories;

namespace EdSofta.Interfaces
{
    internal interface INotificationService
    {
        Task<bool> generateLRecNotificationAsync(string message, string lRecId);

        Task<bool> generateContentUpdateNotificationAsync();

        Task<ObservableCollection<Notification>> getNotificationsAsync();

        Task<bool> deleteNotification(Notification notification);

        Task<bool> deleteNotificationByLRec(string lRecId);

        Task<bool> deleteAllNotifications();
    }
}
