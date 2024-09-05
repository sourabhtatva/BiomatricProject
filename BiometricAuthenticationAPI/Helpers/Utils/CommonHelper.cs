using Amazon.Rekognition.Model;
using Azure.Core;

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
    }
}
