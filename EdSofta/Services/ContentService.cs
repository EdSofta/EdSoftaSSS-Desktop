using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.DataAccess;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;
using NotificationType = EdSofta.Constants.NotificationType;

namespace EdSofta.Services
{
    class ContentService : IContentService
    {
        public async Task<UpdateResultType> checkForContentUpdateAsync()
        {
            var appData = new UpdateRequestData
            {
                productCode = Constants.Keys.ProductCode,
                activationKey = SavedResourceUtility.getActivationKey(),
                version = ContentResourceUtility.getContentVersion()
            };

            var webClientService = new WebClientService<UpdateRequestData, ContentUpdateResponse>();
            var response = await webClientService.postDataAsync(@"content/checkupdate", appData);

            if (response.status)
            {
                return string.IsNullOrEmpty(response.url) ? UpdateResultType.NoUpdateAvailable
                    : UpdateResultType.UpdateAvailable;
            }
            else
            {
                return UpdateResultType.LicenseExpired;
            }
        }

        public async Task<bool> setUpdateNotificationAsync(UpdateResultType resultType)
        {
            if(resultType == UpdateResultType.NoUpdateAvailable) return false;

            using (var dal = new UnitOfWork())
            {

                var recNotification = dal.NotificationRepository.SingleOrDefault(x => x.Type == NotificationType.Content);
                if (recNotification == null)
                {
                    var notification = NotificationClass.CreateContentUpdateNotification(resultType);
                    dal.NotificationRepository.Add(notification);
                    
                }
                else
                {
                    return false;
                }

                return await dal.SaveChangesAsync();
            }
        }
    }
}
