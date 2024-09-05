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

        public static readonly string Haarcascade_Frontalface_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_default.xml");


    }
}
