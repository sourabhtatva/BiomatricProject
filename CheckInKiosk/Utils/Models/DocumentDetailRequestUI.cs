namespace CheckInKiosk.Utils.Models
{
    /// <summary>
    /// Model for document details
    /// </summary>
    public class DocumentDetailRequestUI
    {
        public required string DocumentNumber { get; set; }
        public required string DocumentType { get; set; }
        public required string DocumentImage { get; set; }

    }
}
