using BiometricAuthenticationAPI.Helpers.Constants;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Security.Cryptography;
using System.Text;

namespace BiometricAuthenticationAPI.Helpers.Utils
{
    public class CommonHelper
    {
        public static MemoryStream ByteArrayToMemoryStream(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream();
            if (data != null)
            {
                memoryStream = new MemoryStream(data);
            }
            return memoryStream;
        }

        public static string DecryptString(string cipherText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(SystemConstants.Cryptography.ENCRYPTION_KEY);
                aes.IV = new byte[aes.BlockSize / 8]; // Use a zero IV for simplicity

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        //public static byte[] ConvertStringToByteArray(string decryptedString)
        //{
        //    if (string.IsNullOrEmpty(decryptedString))
        //        throw new ArgumentNullException(nameof(decryptedString));

        //    // Convert the decrypted string into a byte array using UTF-8 encoding
        //    byte[] byteArray = Encoding.UTF8.GetBytes(decryptedString);

        //    return byteArray;
        //}

        //public static byte[] DecryptByteArray(byte[] cipherTextBytes)
        //{
        //    using (Aes aes = Aes.Create())
        //    {
        //        aes.Key = Convert.FromBase64String(SystemConstants.Cryptography.ENCRYPTION_KEY);
        //        aes.IV = new byte[aes.BlockSize / 8]; // Use a zero IV for simplicity

        //        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        //        using (MemoryStream msDecrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
        //            {
        //                csDecrypt.Write(cipherTextBytes, 0, cipherTextBytes.Length);
        //                csDecrypt.FlushFinalBlock();
        //            }

        //            // Return the decrypted byte array
        //            return msDecrypt.ToArray();
        //        }
        //    }
        //}

        //public static byte[] DecryptByteArray(byte[] cipherText)
        //{
        //    using (Aes aes = Aes.Create())
        //    {
        //        aes.Key = Convert.FromBase64String(SystemConstants.Cryptography.ENCRYPTION_KEY);
        //        aes.IV = new byte[16];;

        //        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
        //            {
        //                cs.Write(cipherText, 0, cipherText.Length);
        //                cs.FlushFinalBlock();

        //                // Return the decrypted byte array
        //                return ms.ToArray();
        //            }
        //        }
        //    }
        //}


    }
}
