using CheckInKiosk.Utils.Constants;
using System.IO;
using System.Security.Cryptography;

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

    public static byte[] EncryptByteArray(byte[] plainTextBytes)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = UIConstants.ENCRYPTION_KEY;
            aes.IV = UIConstants.IV;  // Use constant IV

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(plainTextBytes, 0, plainTextBytes.Length);
                    csEncrypt.FlushFinalBlock();
                }

                return msEncrypt.ToArray();
            }
        }
    }
}
