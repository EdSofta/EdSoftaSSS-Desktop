using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.ViewModels.Utility
{
    internal abstract class DecryptionUtility
    {

        private static string Decrypt(string text, byte[] key, byte[] IV)
        {
            var plaintext = string.Empty;
            if (string.IsNullOrEmpty(text)) return string.Empty;
            //var bytes = Encoding.ASCII.GetBytes(text);
            var bytes = Convert.FromBase64String(text);

            try
            {
                using (AesManaged aes = new AesManaged())
                {
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    ICryptoTransform decryptor = aes.CreateDecryptor(key, IV);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cs))
                            {
                                plaintext = reader.ReadToEnd();
                            }

                        }
                    }
                }

                return plaintext;
            }
            catch(Exception e)
            {
                var message = e.Message;
                return string.Empty;
            }
        }

        private const string key = "HUGHJANUSMIKEHUNTDILDOEGRANDMAPA";
        private const string vector = "8217810114211913";

        public static string DecryptText(string text)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var vectorBytes = Encoding.ASCII.GetBytes(vector);

            return Decrypt(text, keyBytes, vectorBytes);
        }

    }
}
