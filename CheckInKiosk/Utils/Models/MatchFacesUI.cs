using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInKiosk.Utils.Models
{
    public class MatchFacesUI
    {
        public required byte[] ScannedImage { get; set; }
        public required byte[] ClickedImage { get; set; }
    }
}
