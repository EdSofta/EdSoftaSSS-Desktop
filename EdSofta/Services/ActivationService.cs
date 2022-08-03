using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdSofta.Constants;
using EdSofta.DataAccess;
using EdSofta.Enums;
using EdSofta.Interfaces;
using EdSofta.Models;
using EdSofta.ViewModels.Utility;

namespace EdSofta.Services
{
    class ActivationService : IActivationService
    {
        public async Task<bool> activateByKeyAsync(string key)
        {
            return await AppValidation.activateProduct(key);
        }

        public async Task<bool> activateByPinAsync(string pin)
        {
            using (var dal = new UnitOfWork())
            {
                var keyResponse = new KeyResponse();

                var owner = dal.UserRepository.SingleOrDefault(x => x.UserRole == UserType.Administrator);
                if (owner == null) return false;

                var productKey = await AppValidation.getProductKey();
                if (string.IsNullOrWhiteSpace(productKey)) return false;
                
                var product = new Product
                {
                    productKey = productKey,
                    pin = pin,
                    phoneNumber = owner.PhoneNumber,
                    client = Keys.ClientType,
                    type = Keys.PinActivation,
                    productCode = Keys.ProductCode
                };

                var webClientService = new WebClientService<Product, KeyResponse>();
                keyResponse =  await webClientService.postDataAsync(@"user/activate", product);
                if (string.IsNullOrEmpty(keyResponse.key)) return false;

                return await AppValidation.activateProduct(keyResponse.key);
                //keyResponse.status = isSuccessful;
                //keyResponse.message = isSuccessful ? keyResponse.message : "App activation failed, please try again.";
                //return keyResponse;

            }

            

        }
    }
}
