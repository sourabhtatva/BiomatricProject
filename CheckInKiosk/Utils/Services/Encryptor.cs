using CheckInKiosk.Utils.Constants;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Encryptor
{

    public static string EncryptString(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = UIConstants.ENCRYPTION_KEY;
           aes.IV = new byte[aes.BlockSize / 8]; // Use a zero IV for simplicity

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string DecryptString(string cipherText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = UIConstants.ENCRYPTION_KEY;
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
}
