using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInKiosk.Utils.Constants
{
    public static class UIConstants
    {
        public static readonly string CONTENT_TYPE = "application/json";
        //public static readonly string Haarcascade_Frontalface_Path = @"haarcascade_frontalface_default.xml";


        public static readonly byte[] ENCRYPTION_KEY = Convert.FromBase64String("dA8L8+nF8D6e0a7H0a5lY3cFg+33r0H1LK4RmO5bI3I=");

        public static readonly string Haarcascade_Frontalface_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml");


    }
}
