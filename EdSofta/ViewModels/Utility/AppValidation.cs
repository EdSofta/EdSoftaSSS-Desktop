using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace EdSofta.ViewModels.Utility
{
    internal class AppValidation
    {
        /// <summary>
        /// The product identifier
        /// </summary>
        private static string ProductID = string.Empty;



        public static async Task<bool> activateProduct(string activationKey)
        {
            var isActivated = false;
            var activationKeyText = string.Empty;
            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            try
            {
                if (!File.Exists(appDataPath)) return false;

                var jsonData = await FileParser.readFileAsync(appDataPath);
                var appData = JsonConvert.DeserializeObject<AppData>(jsonData);

                if (string.IsNullOrWhiteSpace(activationKey))
                {
                    activationKey = appData.ActivationKey;
                }

                if (matchActivationID(activationKey, appData.ProductKey))
                {
                    isActivated = true;
                    appData.ActivationKey = activationKey;
                }

                FileParser.SaveToJson(appData, appDataPath);

                return isActivated;
            }
            catch
            {
                return false;
            }

        }


        public static async Task<string> getProductKey()
        {
            var productKey = string.Empty;

            var appDataPath = Path.Combine(App.AppDataPath, App.ResourcePaths["UserData"], App.Files["AppData"]);

            try
            {
                if (File.Exists(appDataPath))
                {
                    var jsonData = await FileParser.readFileAsync(appDataPath);
                    if (string.IsNullOrWhiteSpace(jsonData)) return string.Empty;
                    var appData = JsonConvert.DeserializeObject<AppData>(jsonData);

                    if (DeviceInformationClass.GenerateProductKey() == appData.ProductKey)
                    {
                        productKey = appData.ProductKey;
                        //ProductID = appData.ProductKey;
                    }
                    else
                    {
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    productKey = DeviceInformationClass.GenerateProductKey();
                    var appData = new AppData
                    {
                        ProductKey = productKey,
                        ActivationKey = string.Empty,
                        Notifications = true,
                        Theme = ThemeHelper.InitializeAppTheme()
                    };

                    FileParser.SaveToJson(appData, appDataPath);

                }

                return productKey;
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Matches the activation identifier.
        /// </summary>
        /// <param name="activationKey">The activation key.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool matchActivationID(string activationKey, string productKey)
        {
            var isMatch = false;
            if (activationKey.Length != 12) return isMatch;
            var firstCharVal = char.IsDigit(activationKey[0]) ? int.Parse(activationKey[0].ToString()) : 6;
            var check = firstCharVal == 5 || firstCharVal == 7;
            var keyLength = check ? firstCharVal : 6;
            var privateKey = activationKey.Substring(activationKey.Length - keyLength, keyLength);
            var decodedKey = BaseOperation.fromDeci(32, BaseOperation.toDeci(DeviceInformationClass.restoreInString(privateKey), 30));
            var match = BaseOperation.fromDeci(32, BaseOperation.toDeci(DeviceInformationClass.restoreInString(productKey), 32)).Substring(3, 6);
            if (BaseOperation.toDeci(decodedKey, 32) == BaseOperation.toDeci(match, 32))
            {
                isMatch = true;
            }

            return isMatch;
        }
    }
}
