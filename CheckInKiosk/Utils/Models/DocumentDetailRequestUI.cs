using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckInKiosk.Utils.Models
{
    public class DocumentDetailRequestUI
    {
        public required string DocumentNumber { get; set; }
        public required string DocumentType { get; set; }
        public required string DocumentImage { get; set; }

    }
}
