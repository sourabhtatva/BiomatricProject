using System.IO;

namespace CheckInKiosk.Utils.Constants
{
    public static class UIConstants
    {
        public static readonly string CONTENT_TYPE = "application/json";
        public static readonly byte[] ENCRYPTION_KEY = Convert.FromBase64String("dA8L8+nF8D6e0a7H0a5lY3cFg+33r0H1LK4RmO5bI3I=");
        public static readonly byte[] IV = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F];
        public static readonly string Haarcascade_Frontalface_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml");
        public static readonly string Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
    }
}
