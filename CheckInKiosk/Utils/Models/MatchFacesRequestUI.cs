using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInKiosk.Utils.Models
{
    public class MatchFacesRequestUI
    {
        public required string ScannedImage { get; set; }
        public required string ClickedImage { get; set; }
    }
}
