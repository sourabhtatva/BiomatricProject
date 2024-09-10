using BiometricAuthenticationAPI.Helpers.Constants;
using System.Security.Cryptography;

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
                aes.Key = SystemConstants.Cryptography.ENCRYPTION_KEY;
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

        public static byte[] DecryptByteArray(byte[] cipherTextBytes)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = SystemConstants.Cryptography.ENCRYPTION_KEY;
                aes.IV = SystemConstants.Cryptography.IV;  // Use constant IV

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(cipherTextBytes, 0, cipherTextBytes.Length);
                        csDecrypt.FlushFinalBlock();
                    }

                    return msDecrypt.ToArray();
                }
            }
        }
    }
}
