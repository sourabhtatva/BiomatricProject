namespace CheckInKiosk.Utils.Models
{
    /// <summary>
    /// Model for match faces request
    /// </summary>
    public class MatchFacesRequestUI
    {
        public required string ScannedImage { get; set; }
        public required string ClickedImage { get; set; }
    }
}
